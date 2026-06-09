using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.Parking.Commands.CreatePrakingUnit;
using RGroupConstruction.Application.Features.Parking.Commands.DeleteParkingUnit;
using RGroupConstruction.Application.Features.Parking.Commands.UpdatePrakingUnit;
using RGroupConstruction.Application.Features.Parking.Queries.GetAllParkingUnits;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Resources;
using RGroupConstruction.Domain.Entities;
using RGroupConstruction.Logging;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace RGroupConstruction.Application.Services;

public class ParkingService(IUnitOfWork _uow, IMapper _mapper, ILogger<UnitService> _logger, IMessageLocalizer _localizer) : IParkingService
{
    public async Task<Result<ParkingDto>> CreateParkingAsync(CreatePrakingCommand request, CancellationToken cancellationToken = default)
    {

        try
        {
            var project = await _uow.Repository<Project>()
              .FirstOrDefaultAsync(p => p.Id.ToString() == request.ProjectId && !p.IsDeleted, cancellationToken);

            if (project is null)
                return Result<ParkingDto>.Error(_localizer[MessageKeys.Error.Parking.ProjectNotFound]);

            var parking = new Parking
            {
                Project = project,
                TotalFlorParking = request.TotalFlorParking,
                AvailableParking = request.AvailableParking,
                FloorNr = request.FloorNr,
                Price = request.Price,
            };

            await _uow.Repository<Parking>().AddAsync(parking, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Result<ParkingDto>.Success(_mapper.Map<ParkingDto>(parking));
        }
        catch (Exception ex)
        {
            await _uow.RollbackTransactionAsync(cancellationToken);
            _logger.Error("Failed to create unit.", ex);
            return Result<ParkingDto>.Error(_localizer[MessageKeys.Error.Parking.CreateFailed]);
        }
    }

    public async Task<Result<bool>> DeleteParkingAsync(DeleteParkingCommand request, CancellationToken cancellationToken = default)
    {
        var existingUnit = await _uow.Repository<Parking>()
          .FirstOrDefaultAsync(c => c.Id.ToString() == request.Id && !c.IsDeleted, cancellationToken);

        if (existingUnit is null)
            return Result<bool>.Error(_localizer[MessageKeys.Error.Parking.NotFound]);


        existingUnit.IsDeleted = true;

        await _uow.Repository<Parking>().UpdateAsync(existingUnit, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    public async Task<Result<PagedResponse<ParkingDto>>> GetAllParkingsAsync(GetAllParkingsQuery request, CancellationToken cancellationToken = default)
    {

        var query = _uow.Repository<Parking>()
            .AsQueryable()
            .Include(u => u.Project)
            .Where(c => !c.IsDeleted);

        var totalCount = await query.CountAsync(cancellationToken);

        var units = await query
            .OrderByDescending(c => c.CreatedOn)
            .Skip((request.PageNr - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var mapped = _mapper.Map<List<ParkingDto>>(units);

        var pagedResponse = new PagedResponse<ParkingDto>(mapped, totalCount, request.PageNr, request.PageSize);

        return Result<PagedResponse<ParkingDto>>.Success(pagedResponse);
    }

    public async Task<Result<ParkingDto>> UpdateParkingAsync(UpdatePrakingCommand request, CancellationToken cancellationToken = default)
    {
        try
        {

            var existingUnit = await _uow.Repository<Parking>()
                .FirstOrDefaultAsync(c => c.Id.ToString() == request.Id && !c.IsDeleted, cancellationToken);

            if (existingUnit is null)
                return Result<ParkingDto>.Error(_localizer[MessageKeys.Error.Parking.NotFound]);


            existingUnit.FloorNr = request.FloorNr;
            existingUnit.TotalFlorParking = request.TotalFlorParking;
            existingUnit.AvailableParking = request.AvailableParking;
            existingUnit.Price = request.Price;


            await _uow.Repository<Parking>().UpdateAsync(existingUnit, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            return Result<ParkingDto>.Success(_mapper.Map<ParkingDto>(existingUnit));
        }
        catch (Exception ex)
        {
            await _uow.RollbackTransactionAsync(cancellationToken);
            _logger.Error("Failed to create unit.", ex);
            return Result<ParkingDto>.Error(_localizer[MessageKeys.Error.Parking.CreateFailed]);
        }
    }


}

