using MediatR;
using Microsoft.EntityFrameworkCore;
using Querify.Common.EntityFramework.Tenant;
using Querify.Models.Common.Dtos;
using Querify.Models.Tenant.Dtos.ChannelConnection;
using Querify.Tenant.Portal.Business.ChannelConnection.Rules;
using ChannelConnectionEntity = Querify.Common.EntityFramework.Tenant.Entities.ChannelConnection;

namespace Querify.Tenant.Portal.Business.ChannelConnection.Queries.GetChannelConnectionList;

public sealed class ChannelConnectionsGetListQueryHandler(
    TenantDbContext dbContext,
    ChannelConnectionTenantResolver tenantResolver)
    : IRequestHandler<ChannelConnectionsGetListQuery, PagedResultDto<ChannelConnectionDto>>
{
    public async Task<PagedResultDto<ChannelConnectionDto>> Handle(
        ChannelConnectionsGetListQuery request,
        CancellationToken cancellationToken)
    {
        var baseTenantId = await tenantResolver.ResolveAsync(request.TenantId, false, cancellationToken);
        var dto = request.Request;
        var query = dbContext.ChannelConnections.AsNoTracking()
            .Where(connection => connection.TenantId == baseTenantId);

        if (!string.IsNullOrWhiteSpace(dto.SearchText))
        {
            var search = $"%{dto.SearchText}%";
            query = query.Where(connection =>
                EF.Functions.ILike(connection.Name, search) ||
                EF.Functions.ILike(connection.ProviderKey, search));
        }

        if (dto.Kind.HasValue)
            query = query.Where(connection => connection.Kind == dto.Kind.Value);
        if (dto.Status.HasValue)
            query = query.Where(connection => connection.Status == dto.Status.Value);
        if (dto.IsEnabled.HasValue)
            query = query.Where(connection => connection.IsEnabled == dto.IsEnabled.Value);

        query = ApplySorting(query, dto.Sorting);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.Skip(dto.SkipCount).Take(dto.MaxResultCount)
            .Select(connection => new ChannelConnectionDto
            {
                Id = connection.Id,
                TenantId = connection.TenantId,
                Name = connection.Name,
                ProviderKey = connection.ProviderKey,
                Kind = connection.Kind,
                Status = connection.Status,
                IsEnabled = connection.IsEnabled,
                CredentialsExpireAtUtc = connection.CredentialsExpireAtUtc,
                LastCredentialsRefreshAtUtc = connection.LastCredentialsRefreshAtUtc,
                LastConnectedAtUtc = connection.LastConnectedAtUtc,
                LastSynchronizedAtUtc = connection.LastSynchronizedAtUtc,
                LastErrorAtUtc = connection.LastErrorAtUtc,
                LastErrorMessage = connection.LastErrorMessage,
                CreatedAtUtc = connection.CreatedDate,
                LastUpdatedAtUtc = connection.UpdatedDate ?? connection.CreatedDate
            })
            .ToListAsync(cancellationToken);

        return new PagedResultDto<ChannelConnectionDto>(totalCount, items);
    }

    private static IQueryable<ChannelConnectionEntity> ApplySorting(
        IQueryable<ChannelConnectionEntity> query,
        string? sorting)
    {
        return sorting?.Trim().ToLowerInvariant() switch
        {
            "name" or "name asc" => query.OrderBy(connection => connection.Name),
            "name desc" => query.OrderByDescending(connection => connection.Name),
            "kind" or "kind asc" => query.OrderBy(connection => connection.Kind).ThenBy(connection => connection.Name),
            "kind desc" => query.OrderByDescending(connection => connection.Kind).ThenBy(connection => connection.Name),
            "status" or "status asc" => query.OrderBy(connection => connection.Status).ThenBy(connection => connection.Name),
            "status desc" => query.OrderByDescending(connection => connection.Status).ThenBy(connection => connection.Name),
            "lastupdatedatutc asc" => query.OrderBy(connection => connection.UpdatedDate ?? connection.CreatedDate),
            _ => query.OrderByDescending(connection => connection.UpdatedDate ?? connection.CreatedDate)
                .ThenBy(connection => connection.Name)
        };
    }
}
