using BookingAPI.src.Modules.Booking.Application.Services.Interfaces;
using BookingAPI.src.Modules.Booking.Domain;
using BookingAPI.src.Modules.Booking.Domain.Dto.ReservationDto;
using BookingAPI.src.Modules.Booking.Domain.Enums;
using BookingAPI.src.Modules.Booking.Domain.Mappers;
using BookingAPI.src.Modules.Booking.Infrastructure;
using BookingAPI.src.Shared.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Hybrid;

namespace BookingAPI.src.Modules.Booking.Application.Services.Implementations;


public class ReservationService(
    BookingDbContext context,
    HybridCache cache,
    ILogger<ReservationService> logger
) : IReservationService
{
    private const string CACHE_TAG = "reservations";
    private const string LIST_CACHE_TAG = "reservations_list";

    public async Task<ReservationResponseDto> Create(CreateReservationDto request)
    {
        logger.LogInformation("Criando Reserva - GuestId: {GuestId} - RoomId: {RoomId}", request.GuestId, request.RoomId);

        var room = await context.Rooms.FirstOrDefaultAsync(r => r.Id == request.RoomId)
            ?? throw new NullReferenceException("Quarto não encontrado.");

        var guest = await context.Guests.FirstOrDefaultAsync(g => g.Id == request.GuestId)
            ?? throw new NullReferenceException("Hóspede não encontrado.");

        int totalNights = (request.CheckOutDate - request.CheckInDate).Days;
        if (totalNights <= 0) throw new ArgumentException("A data de check-out deve ser maior que a de check-in.");

        var entity = new Reservation
        {
            GuestId = request.GuestId,
            Guest = guest,
            RoomId = request.RoomId,
            Room = room,
            CheckInDate = request.CheckInDate,
            CheckOutDate = request.CheckOutDate,
            TotalPrice = totalNights * room.PricePerNight,
            Status = ReservationStatus.Pending 
        };


        context.Reservations.Add(entity);
        await context.SaveChangesAsync();

        var dto = ReservationMapper.ToDto(entity);

        await cache.RemoveByTagAsync(LIST_CACHE_TAG);
        await cache.SetAsync(
            $"reservation:{dto.Id}", 
            dto, 
            tags: [CACHE_TAG]
        );

        logger.LogInformation("Reserva Criada - Id: {Id} - Total: {TotalPrice}", dto.Id, dto.TotalPrice);
        return dto;
    }

    public async Task<ReservationResponseDto?> GetById(Guid id)
    {
        return await cache.GetOrCreateAsync(
            $"reservation:{id}",
            async ct => await context.Reservations
                .Include(r => r.Guest)
                .Include(r => r.Room)
                .Select(r => ReservationMapper.ToDto(r))
                .FirstOrDefaultAsync(r => r.Id == id, ct),
            tags: [CACHE_TAG]
        );
    }

    public async Task<PaginatedResult<ReservationResponseDto>> List(ReservationQueryDto request)
    {
        var queryable = context.Reservations
            .Include(r => r.Guest)
            .Include(r => r.Room)
            .AsNoTracking();

        if (request.Deleted is true)
        {
            queryable = queryable.IgnoreQueryFilters();
        }

        queryable = queryable
            .WhereIf(request.GuestId is not null, r => r.GuestId == request.GuestId)
            .WhereIf(request.RoomId is not null, r => r.RoomId == request.RoomId)
            .WhereIf(request.Status is not null, r => r.Status == request.Status);

        
        var cacheKey = $"reservations:list:p:{request.PageNumber}" +
                       $":g:{request.GuestId?.ToString() ?? "all"}" +
                       $":r:{request.RoomId?.ToString() ?? "all"}" +
                       $":s:{request.Status?.ToString() ?? "all"}" +
                       $":d:{request.Deleted ?? false}";

        return await cache.GetOrCreateAsync(
            cacheKey,
            async ct => await queryable
            .ToPaginatedResultAsync(
                request.PageNumber, 
                r => ReservationMapper.ToDto(r), 
                cancellationToken: ct
            ),
            tags: [LIST_CACHE_TAG]
        );
    }

    public async Task<ReservationResponseDto> Update(UpdateReservationDto request)
    {
        logger.LogInformation("Atualizando Reserva - Id: {Id} - Status Request: {Status}", request.ReservationId, request.Status);

        var entity = await context.Reservations
            .Include(r => r.Guest)
            .Include(r => r.Room)
            .FirstOrDefaultAsync(r => r.Id == request.ReservationId)
            ?? throw new NullReferenceException($"Reserva {request.ReservationId} não encontrada");

        if (request.CheckInDate.HasValue) entity.CheckInDate = request.CheckInDate.Value;
        if (request.CheckOutDate.HasValue) entity.CheckOutDate = request.CheckOutDate.Value;
        if (request.Status.HasValue) entity.Status = request.Status.Value;

        if (request.CheckInDate.HasValue || request.CheckOutDate.HasValue)
        {
            int totalNights = (entity.CheckOutDate - entity.CheckInDate).Days;
            entity.TotalPrice = totalNights * entity.Room.PricePerNight;
        }

        await context.SaveChangesAsync();
        var dto = ReservationMapper.ToDto(entity);


        await cache.RemoveAsync($"reservation:{request.ReservationId}");
        await cache.RemoveByTagAsync(LIST_CACHE_TAG);
        await cache.SetAsync(
            $"reservation:{dto.Id}", 
            dto, 
            tags: [CACHE_TAG]
        );

        logger.LogInformation("Reserva Atualizada - Id: {Id} - Novo Total: {TotalPrice}", dto.Id, dto.TotalPrice);
        return dto;
    }

    public async Task SoftDelete(Guid id)
    {
        logger.LogInformation("Soft Delete Reserva - Id: {Id}", id);

        var entity = await context.Reservations.FirstOrDefaultAsync(r => r.Id == id)
            ?? throw new NullReferenceException($"Reserva {id} não encontrada");

        entity.IsDeleted = true;
        
        await cache.RemoveAsync($"reservation:{id}");
        await cache.RemoveByTagAsync(LIST_CACHE_TAG);
        
        await context.SaveChangesAsync();
    }

    public async Task Restore(Guid id)
    {
        logger.LogInformation("Restaurando Reserva - Id: {Id}", id);

        var entity = await context
            .Reservations
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(r => r.Id == id)
            ?? throw new NullReferenceException($"Reserva {id} não encontrada");

        entity.IsDeleted = false;
        await context.SaveChangesAsync();

        await cache.RemoveByTagAsync(LIST_CACHE_TAG);
        await cache.SetAsync(
            $"reservation:{entity.Id}", 
            ReservationMapper.ToDto(entity), 
            tags: [CACHE_TAG]
        );
    }

    public async Task HardDelete(Guid id)
    {
        logger.LogInformation("Hard Delete Reserva - Id: {Id}", id);

        var entity = await context
            .Reservations
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(r => r.Id == id)
            ?? throw new NullReferenceException($"Reserva {id} não encontrada");

        context.Reservations.Remove(entity);
        
        await cache.RemoveAsync($"reservation:{id}");
        await cache.RemoveByTagAsync(LIST_CACHE_TAG);
        
        await context.SaveChangesAsync();
    }
}