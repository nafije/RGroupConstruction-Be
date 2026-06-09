using RGroupConstruction.Application.Common;
using RGroupConstruction.Application.DTOs;
using RGroupConstruction.Application.Features.City.Commands.CreateCity;
using RGroupConstruction.Application.Features.City.Commands.DeleteCity;
using RGroupConstruction.Application.Features.City.Commands.UpdateCity;
using RGroupConstruction.Application.Features.City.Queries.GetAllPagedCities;
using RGroupConstruction.Application.Features.City.Queries.GetAllCities;
using RGroupConstruction.Application.Interfaces.Services;
using RGroupConstruction.Application.Resources;
using RGroupConstruction.Domain.Entities;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace RGroupConstruction.Application.Services;

public class CityService(IUnitOfWork _uow, IMapper _mapper, ILogger<CityService> _logger, IMessageLocalizer _localizer) : ICityService
{
    public async Task<Result<CityDto>> CreateCityAsync(CreateCityCommand request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Creating city {CityName}", request.Name);

        if (request.Name is not null)
        {
            var existingCity = await _uow.Repository<City>()
                .FirstOrDefaultAsync(c => c.Name == request.Name, cancellationToken);

            if (existingCity is not null)
            {
                _logger.Warn("City {CityName} already exists", request.Name);
                return Result<CityDto>.Error(_localizer[MessageKeys.Error.City.CityExists]);
            }
        }

        try
        {
            var newCity = new City
            {
                Name = request.Name
            };

            await _uow.Repository<City>().AddAsync(newCity, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            _logger.Info("Created city {CityId}", newCity.Id);
            return Result<CityDto>.Success(_mapper.Map<CityDto>(newCity));
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to create city {CityName}", request.Name);
            return Result<CityDto>.Error(_localizer[MessageKeys.Error.City.CreateFailed]);
        }
    }

    public async Task<Result<bool>> DeleteCityAsync(DeleteCityCommand request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Deleting city {CityId}", request.Id);

        var existingCity = await _uow.Repository<City>()
           .FirstOrDefaultAsync(c => c.Id.ToString() == request.Id && !c.IsDeleted, cancellationToken);

        if (existingCity is null)
        {
            _logger.Warn("City {CityId} was not found for delete", request.Id);
            return Result<bool>.Error(_localizer[MessageKeys.Error.City.NotFound]);
        }

        existingCity.IsDeleted = true;

        await _uow.Repository<City>().UpdateAsync(existingCity, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        _logger.Info("Deleted city {CityId}", request.Id);
        return Result<bool>.Success(true);
    }

    public async Task<Result<IEnumerable<CityDto>>> GetAllCitiesAsync(GetAllCitiesQuery request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Getting all cities");

        var cities = await _uow.Repository<City>()
           .AsQueryable()
           .Where(c => !c.IsDeleted)
           .ToListAsync(cancellationToken);

        var mapped = _mapper.Map<IEnumerable<CityDto>>(cities);
        _logger.Info("Retrieved {CityCount} cities", cities.Count);
        return Result.Success(mapped);
    }

    public async Task<Result<PagedResponse<CityDto>>> GetAllPagedCitiesAsync(GetAllPagedCitiesQuery request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Getting paged cities page {PageNumber} with size {PageSize} and search {Search}", request.PageNr, request.PageSize, request.Search);

        var query = _uow.Repository<City>()
            .AsQueryable()
            .Where(c => !c.IsDeleted);

        if (!string.IsNullOrWhiteSpace(request.Search))
            query = query.Where(c => c.Name != null && c.Name.ToLower().Contains(request.Search.ToLower()));

        var totalCount = await query.CountAsync(cancellationToken);

        var cities = await query
            .Skip((request.PageNr - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectTo<CityDto>(_mapper.ConfigurationProvider)
            .ToListAsync(cancellationToken);

        var pagedResponse = new PagedResponse<CityDto>(cities, totalCount, request.PageNr, request.PageSize);
        _logger.Info("Retrieved {CityCount} cities from total {TotalCount}", cities.Count, totalCount);
        return Result<PagedResponse<CityDto>>.Success(pagedResponse);
    }

    public async Task<Result<CityDto>> UpdateCityAsync(UpdateCityCommand request, CancellationToken cancellationToken = default)
    {
        _logger.Info("Updating city {CityId}", request.Id);

        var existingCity = await _uow.Repository<City>()
            .FirstOrDefaultAsync(c => c.Id.ToString() == request.Id, cancellationToken);

        if (existingCity is null)
        {
            _logger.Warn("City {CityId} was not found for update", request.Id);
            return Result<CityDto>.Error(_localizer[MessageKeys.Error.City.NotFound]);
        }

        if (request.Name is not null)
        {
            var duplicateCity = await _uow.Repository<City>()
                .FirstOrDefaultAsync(c => c.Name == request.Name && c.Id.ToString() != request.Id, cancellationToken);

            if (duplicateCity is not null)
            {
                _logger.Warn("City name {CityName} already exists for another city", request.Name);
                return Result<CityDto>.Error(_localizer[MessageKeys.Error.City.CityExists]);
            }
        }

        try
        {
            existingCity.Name = request.Name;

            await _uow.Repository<City>().UpdateAsync(existingCity, cancellationToken);
            await _uow.SaveChangesAsync(cancellationToken);

            _logger.Info("Updated city {CityId}", request.Id);
            return Result<CityDto>.Success(_mapper.Map<CityDto>(existingCity));
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to update city {CityId}", request.Id);
            return Result<CityDto>.Error(_localizer[MessageKeys.Error.City.UpdateFailed]);
        }
    }
}
