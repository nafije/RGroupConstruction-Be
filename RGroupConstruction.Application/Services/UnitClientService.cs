using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.UnitClient.Commands.CreateUnitClient;
using RGroupConstruction.Application.Features.UnitClient.Commands.DeleteUnitClient;
using RGroupConstruction.Application.Features.UnitClient.Commands.UpdateUnitClient;
using RGroupConstruction.Application.Features.UnitClient.Queries.GetAllPagedUnitClients;
using RGroupConstruction.Application.Features.UnitClient.Queries.GetDashboardStats;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Resources;
using RGroupConstruction.Domain.Entities;
using RGroupConstruction.Domain.Enums;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MySqlX.XDevAPI;
using System.Globalization;

namespace RGroupConstruction.Application.Services;

public class UnitClientService(IUnitOfWork _uow, IMapper _mapper, ILogger<UnitClientService> _logger, IMessageLocalizer _localizer) : IUnitClientService
{
    public async Task<Result<UnitClientDto>> CreateUnitClientAsync(CreateUnitClientCommand request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Creating unit client {ClientFullName} for unit {UnitId}", request.ClientFullName, request.UnitId);

        var unit = await _uow.Repository<Unit>()
            .FirstOrDefaultAsync(u => u.Id.ToString() == request.UnitId && !u.IsDeleted, cancellationToken);

        if (unit is null)
        {
            _logger.Warn("Unit {UnitId} was not found while creating unit client {ClientFullName}", request.UnitId, request.ClientFullName);
            return Result<UnitClientDto>.Error(_localizer[MessageKeys.Error.UnitClient.UnitNotFound]);
        }

        Enum.TryParse<UnitClientStatus>(request.Status, out var status);
        Enum.TryParse<PaymentType>(request.PaymentType, out var type);

        await _uow.BeginTransactionAsync(cancellationToken);

        try
        {
            var newUnitClient = new UnitClient
            {
                Unit = unit,
                ClientFullName = request.ClientFullName,
                PhoneNumber = request.PhoneNumber,
                Status = status,
                PaymentType = type,
                ClosingPriceM2 = request.ClosingPriceM2,
                ClosinTotalPrice = request.ClosinTotalPrice,
                Date = request.Date,
                Installments = request.Installments,
                Comment = request.Comment,
            };

            await _uow.Repository<UnitClient>().AddAsync(newUnitClient, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
            await _uow.CommitTransactionAsync(cancellationToken);

            _logger.Info("Created unit client {UnitClientId} for unit {UnitId}", newUnitClient.Id, unit.Id);

            return Result<UnitClientDto>.Success(_mapper.Map<UnitClientDto>(newUnitClient));
        }
        catch (Exception ex)
        {
            await _uow.RollbackTransactionAsync(cancellationToken);
            _logger.Error(ex, "Failed to create unit client {ClientFullName}", request.ClientFullName);
            return Result<UnitClientDto>.Error(_localizer[MessageKeys.Error.UnitClient.CreateFailed]);
        }
    }

    public async Task<Result<bool>> DeleteUnitClientAsync(DeleteUnitClientCommand request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Deleting unit client {UnitClientId}", request.Id);

        var existingUnitClient = await _uow.Repository<UnitClient>()
            .FirstOrDefaultAsync(c => c.Id.ToString() == request.Id && !c.IsDeleted, cancellationToken);

        if (existingUnitClient is null)
        {
            _logger.Warn("Unit client {UnitClientId} was not found for delete", request.Id);
            return Result<bool>.Error(_localizer[MessageKeys.Error.UnitClient.NotFound]);
        }

        existingUnitClient.IsDeleted = true;

        await _uow.Repository<UnitClient>().UpdateAsync(existingUnitClient, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        _logger.Info("Deleted unit client {UnitClientId}", request.Id);
        return Result<bool>.Success(true);
    }

    public async Task<Result<PagedResponse<UnitClientDto>>> GetAllPagedUnitClientsAsync(GetAllPagedUnitClientsQuery request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Getting paged unit clients page {PageNumber} with size {PageSize}, search {Search}, unit {UnitId}", request.PageNr, request.PageSize, request.Search);

        var query = _uow.Repository<UnitClient>()
            .AsQueryable()
            .Include(uc => uc.Unit)
            .Where(c => !c.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();

            query = query.Where(c =>
                (c.ClientFullName != null && c.ClientFullName.ToLower().Contains(search)) ||
                (c.PhoneNumber != null && c.PhoneNumber.ToLower().Contains(search)) ||
                (c.Unit != null && c.Unit.UnitRefNumber != null && c.Unit.UnitRefNumber.ToLower().Contains(search)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var unitClients = await query
            .OrderByDescending(c => c.CreatedOn)
            .Skip((request.PageNr - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var mapped = _mapper.Map<List<UnitClientDto>>(unitClients);

        var pagedResponse = new PagedResponse<UnitClientDto>(mapped, totalCount, request.PageNr, request.PageSize);

        _logger.Info("Retrieved {UnitClientCount} unit clients from total {TotalCount}", mapped.Count, totalCount);

        return Result<PagedResponse<UnitClientDto>>.Success(pagedResponse);
    }

    public async Task<Result<UnitClientDto>> UpdateUnitClientAsync(UpdateUnitClientCommand request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Updating unit client {UnitClientId}", request.Id);

        var unitClient = await _uow.Repository<UnitClient>()
            .FirstOrDefaultAsync(u => u.Id.ToString() == request.Id && !u.IsDeleted, cancellationToken);

        if (unitClient is null)
        {
            _logger.Warn("Unit client {UnitClientId} was not found for update", request.Id);
            return Result<UnitClientDto>.Error(_localizer[MessageKeys.Error.UnitClient.NotFound]);
        }

        Enum.TryParse<UnitClientStatus>(request.Status, out var status);
        Enum.TryParse<PaymentType>(request.PaymentType, out var type);

        await _uow.BeginTransactionAsync(cancellationToken);

        try
        {
            unitClient.ClientFullName = request.ClientFullName;
            unitClient.PhoneNumber = request.PhoneNumber;
            unitClient.Status = status;
            unitClient.PaymentType = type;
            unitClient.ClosingPriceM2 = request.ClosingPriceM2;
            unitClient.ClosinTotalPrice = request.ClosinTotalPrice;
            unitClient.Date = request.Date;
            unitClient.Installments = request.Installments;
            unitClient.Comment = request.Comment;

            await _uow.Repository<UnitClient>().UpdateAsync(unitClient, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);
            await _uow.CommitTransactionAsync(cancellationToken);

            _logger.Info("Updated unit client {UnitClientId}", request.Id);
            return Result<UnitClientDto>.Success(_mapper.Map<UnitClientDto>(unitClient));
        }
        catch (Exception ex)
        {
            await _uow.RollbackTransactionAsync(cancellationToken);
            _logger.Error(ex, "Failed to update unit client {UnitClientId}", request.Id);
            return Result<UnitClientDto>.Error(_localizer[MessageKeys.Error.UnitClient.UpdateFailed]);
        }
    }

    public async Task<Result<DashboardStatsDto>> GetDashboardStatsAsync(GetDashboardStatsQuery request, CancellationToken cancellationToken = default)
    {
        // 1. Single query for all unit stats
        var unitCounts = await _uow.Repository<Unit>()
            .AsQueryable()
            .Where(u => !u.IsDeleted &&
                       (u.UnitStatus == UnitStatus.Reserved || u.UnitStatus == UnitStatus.Sold))
            .GroupBy(_ => 1)
            .Select(g => new
            {
                TotalReserved = g.Count(u => u.UnitStatus == UnitStatus.Reserved),
                TotalSold = g.Count(u => u.UnitStatus == UnitStatus.Sold)
            })
            .FirstOrDefaultAsync(cancellationToken);

        // 2. Single query for all client stats
        var clientCounts = await _uow.Repository<UnitClient>()
            .AsQueryable()
            .Where(c => !c.IsDeleted)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Total = g.Count(),
                TotalCash = g.Count(c => c.PaymentType == PaymentType.Cash),
                TotalInstallment = g.Count(c => c.PaymentType == PaymentType.Installments)
            })
            .FirstOrDefaultAsync(cancellationToken);

        // 3. Most sold project
        var mostSoldProject = await _uow.Repository<Unit>()
            .AsQueryable()
            .Where(u => !u.IsDeleted && u.UnitStatus == UnitStatus.Sold)
            .GroupBy(u => new { u.Project!.Id, u.Project.Name })
            .Select(g => new { ProjectName = g.Key.Name, SoldCount = g.Count() })
            .OrderByDescending(x => x.SoldCount)
            .Select(x => x.ProjectName)
            .FirstOrDefaultAsync(cancellationToken);

        // 4. Projects list
        var projects = await _uow.Repository<Project>()
            .AsQueryable()
            .Include(p => p.ProjectTranslations!.Where(t => !t.IsDeleted))
            .Include(p => p.ProjectImages!.Where(i => !i.IsDeleted))
            .Where(p => !p.IsDeleted)
            .ProjectTo<ProjectDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        var projectIds = projects.Select(p => p.Id).ToList();

        // 5. Per-project unit stats
        var unitStats = await _uow.Repository<Unit>()
            .AsQueryable()
            .Where(u => projectIds.Contains(u.Project!.Id) && !u.IsDeleted
                     && (u.UnitStatus == UnitStatus.Reserved || u.UnitStatus == UnitStatus.Sold))
            .GroupBy(u => u.Project!.Id)
            .Select(g => new
            {
                ProjectId = g.Key,
                BusyResidentialUnits = g.Count(u => u.UnitCategory!.Name == "Apartment"),
                BusyCommercialUnits = g.Count(u => u.UnitCategory!.Name == "Shop"),
            })
            .ToDictionaryAsync(x => x.ProjectId, cancellationToken);

        // 6. Per-project parking stats
        var parkingStats = await _uow.Repository<Parking>()
            .AsQueryable()
            .Where(p => projectIds.Contains(p.Project!.Id) && !p.IsDeleted)
            .GroupBy(p => p.Project!.Id)
            .Select(g => new
            {
                ProjectId = g.Key,
                AvailableParkingUnits = g.Sum(p => p.AvailableParking),
            })
            .ToDictionaryAsync(x => x.ProjectId, cancellationToken);

        // 7. Map availability — always compute even if project has no busy units
        foreach (var project in projects)
        {
            var units = unitStats.GetValueOrDefault(project.Id);
            project.AvailableResidentialUnits = project.ResidentialUnits - (units?.BusyResidentialUnits ?? 0);
            project.AvailableCcomercialUnits  = project.ComercialUnits   - (units?.BusyCommercialUnits  ?? 0);

            if (parkingStats.TryGetValue(project.Id, out var parking))
            {
                project.ParkingUnits          = project.ParkingUnits;
                project.AvailableParkingUnits = parking.AvailableParkingUnits;
            }

        }

        _logger.Info("Dashboard stats retrieved: {TotalClients} clients, {TotalReserved} reserved, {TotalSold} sold",
            clientCounts?.Total, unitCounts?.TotalReserved, unitCounts?.TotalSold);

        return Result<DashboardStatsDto>.Success(new DashboardStatsDto
        {
            TotalClients            = clientCounts?.Total            ?? 0,
            TotalReservedUnits      = unitCounts?.TotalReserved      ?? 0,
            TotalSoldUnits          = unitCounts?.TotalSold          ?? 0,
            TotalCashClients        = clientCounts?.TotalCash        ?? 0,
            TotalInstallmentClients = clientCounts?.TotalInstallment ?? 0,
            MostSoldProject         = mostSoldProject,
            Projects                = projects
        });
    }

}
