using System.Net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Querify.Broadcast.Common.Persistence.BroadcastDb.DbContext;
using Querify.Common.Infrastructure.ApiErrorHandling.Exception;
using Querify.Common.Infrastructure.Core.Abstractions;
using Querify.Models.Broadcast.Dtos.Item;
using Querify.Models.Common.Dtos;
using Querify.Models.Common.Enums;

namespace Querify.Broadcast.Portal.Business.Item.Queries.GetItemList;

public sealed class ItemsGetListQueryHandler(
    BroadcastDbContext dbContext,
    ISessionService sessionService)
    : IRequestHandler<ItemsGetListQuery, PagedResultDto<ItemDto>>
{
    public async Task<PagedResultDto<ItemDto>> Handle(
        ItemsGetListQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = sessionService.GetTenantId(ModuleEnum.Broadcast);
        var dto = request.Request;
        if (!await dbContext.Threads.AsNoTracking().AnyAsync(
                thread => thread.Id == dto.ThreadId && thread.TenantId == tenantId,
                cancellationToken))
            throw new ApiErrorException($"Thread '{dto.ThreadId}' was not found.", (int)HttpStatusCode.NotFound);

        var query = dbContext.Items.AsNoTracking()
            .Where(item => item.ThreadId == dto.ThreadId && item.TenantId == tenantId);
        if (dto.Kind.HasValue)
            query = query.Where(item => item.Kind == dto.Kind.Value);
        if (dto.ActorKind.HasValue)
            query = query.Where(item => item.ActorKind == dto.ActorKind.Value);

        query = dto.Sorting?.Trim().ToLowerInvariant() switch
        {
            "capturedatutc asc" => query.OrderBy(item => item.CapturedAtUtc),
            "createdatutc asc" => query.OrderBy(item => item.CreatedDate),
            "createdatutc desc" => query.OrderByDescending(item => item.CreatedDate),
            _ => query.OrderByDescending(item => item.CapturedAtUtc)
        };

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.Skip(dto.SkipCount).Take(dto.MaxResultCount)
            .Select(item => new ItemDto
            {
                Id = item.Id,
                TenantId = item.TenantId,
                ThreadId = item.ThreadId,
                Kind = item.Kind,
                ActorKind = item.ActorKind,
                Body = item.Body,
                CapturedAtUtc = item.CapturedAtUtc,
                CreatedAtUtc = item.CreatedDate
            })
            .ToListAsync(cancellationToken);

        return new PagedResultDto<ItemDto>(totalCount, items);
    }
}
