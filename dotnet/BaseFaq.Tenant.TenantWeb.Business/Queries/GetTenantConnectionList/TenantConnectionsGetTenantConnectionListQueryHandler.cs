using BaseFaq.Common.EntityFramework.Tenant;
using BaseFaq.Models.Common.Dtos;
using BaseFaq.Models.Tenant.Dtos.TenantConnection;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseFaq.Tenant.TenantWeb.Business.Queries.GetTenantConnectionList;

public class TenantConnectionsGetTenantConnectionListQueryHandler(TenantDbContext dbContext)
    : IRequestHandler<TenantConnectionsGetTenantConnectionListQuery, PagedResultDto<TenantConnectionDto>>
{
    public async Task<PagedResultDto<TenantConnectionDto>> Handle(TenantConnectionsGetTenantConnectionListQuery request,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.Request);

        var query = dbContext.TenantConnections.AsNoTracking();
        query = ApplySorting(query, request.Request.Sorting);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(request.Request.SkipCount)
            .Take(request.Request.MaxResultCount)
            .Select(connection => new TenantConnectionDto
            {
                Id = connection.Id,
                TenantId = connection.TenantId,
                ConnectionString = connection.ConnectionString,
                IsCurrent = connection.IsCurrent
            })
            .ToListAsync(cancellationToken);

        return new PagedResultDto<TenantConnectionDto>(totalCount, items);
    }

    private static IQueryable<BaseFaq.Common.EntityFramework.Tenant.Entities.TenantConnection> ApplySorting(
        IQueryable<BaseFaq.Common.EntityFramework.Tenant.Entities.TenantConnection> query, string? sorting)
    {
        if (string.IsNullOrWhiteSpace(sorting))
        {
            return query.OrderBy(connection => connection.TenantId);
        }

        IOrderedQueryable<BaseFaq.Common.EntityFramework.Tenant.Entities.TenantConnection>? orderedQuery = null;
        var fields = sorting.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        foreach (var field in fields)
        {
            var parts = field.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length == 0)
            {
                continue;
            }

            var fieldName = parts[0];
            var desc = parts.Length > 1 && parts[1].Equals("DESC", StringComparison.OrdinalIgnoreCase);

            orderedQuery = ApplyOrder(orderedQuery ?? query, fieldName, desc, orderedQuery is null);
        }

        return orderedQuery ?? query.OrderBy(connection => connection.TenantId);
    }

    private static IOrderedQueryable<BaseFaq.Common.EntityFramework.Tenant.Entities.TenantConnection> ApplyOrder(
        IQueryable<BaseFaq.Common.EntityFramework.Tenant.Entities.TenantConnection> query,
        string fieldName,
        bool desc,
        bool isFirst)
    {
        return fieldName.ToLowerInvariant() switch
        {
            "tenantid" => isFirst
                ? (desc
                    ? query.OrderByDescending(connection => connection.TenantId)
                    : query.OrderBy(connection => connection.TenantId))
                : (desc
                    ? ((IOrderedQueryable<BaseFaq.Common.EntityFramework.Tenant.Entities.TenantConnection>)query)
                    .ThenByDescending(connection => connection.TenantId)
                    : ((IOrderedQueryable<BaseFaq.Common.EntityFramework.Tenant.Entities.TenantConnection>)query)
                    .ThenBy(connection => connection.TenantId)),
            "iscurrent" => isFirst
                ? (desc
                    ? query.OrderByDescending(connection => connection.IsCurrent)
                    : query.OrderBy(connection => connection.IsCurrent))
                : (desc
                    ? ((IOrderedQueryable<BaseFaq.Common.EntityFramework.Tenant.Entities.TenantConnection>)query)
                    .ThenByDescending(connection => connection.IsCurrent)
                    : ((IOrderedQueryable<BaseFaq.Common.EntityFramework.Tenant.Entities.TenantConnection>)query)
                    .ThenBy(connection => connection.IsCurrent)),
            "createddate" => isFirst
                ? (desc
                    ? query.OrderByDescending(connection => connection.CreatedDate)
                    : query.OrderBy(connection => connection.CreatedDate))
                : (desc
                    ? ((IOrderedQueryable<BaseFaq.Common.EntityFramework.Tenant.Entities.TenantConnection>)query)
                    .ThenByDescending(connection => connection.CreatedDate)
                    : ((IOrderedQueryable<BaseFaq.Common.EntityFramework.Tenant.Entities.TenantConnection>)query)
                    .ThenBy(connection => connection.CreatedDate)),
            "updateddate" => isFirst
                ? (desc
                    ? query.OrderByDescending(connection => connection.UpdatedDate)
                    : query.OrderBy(connection => connection.UpdatedDate))
                : (desc
                    ? ((IOrderedQueryable<BaseFaq.Common.EntityFramework.Tenant.Entities.TenantConnection>)query)
                    .ThenByDescending(connection => connection.UpdatedDate)
                    : ((IOrderedQueryable<BaseFaq.Common.EntityFramework.Tenant.Entities.TenantConnection>)query)
                    .ThenBy(connection => connection.UpdatedDate)),
            "id" => isFirst
                ? (desc
                    ? query.OrderByDescending(connection => connection.Id)
                    : query.OrderBy(connection => connection.Id))
                : (desc
                    ? ((IOrderedQueryable<BaseFaq.Common.EntityFramework.Tenant.Entities.TenantConnection>)query)
                    .ThenByDescending(connection => connection.Id)
                    : ((IOrderedQueryable<BaseFaq.Common.EntityFramework.Tenant.Entities.TenantConnection>)query)
                    .ThenBy(connection => connection.Id)),
            _ => isFirst
                ? query.OrderBy(connection => connection.TenantId)
                : ((IOrderedQueryable<BaseFaq.Common.EntityFramework.Tenant.Entities.TenantConnection>)query)
                .ThenBy(connection => connection.TenantId)
        };
    }
}