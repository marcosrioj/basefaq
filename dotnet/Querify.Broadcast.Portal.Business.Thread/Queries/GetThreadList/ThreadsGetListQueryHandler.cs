using MediatR;
using Microsoft.EntityFrameworkCore;
using Querify.Broadcast.Common.Persistence.BroadcastDb.DbContext;
using Querify.Common.Infrastructure.Core.Abstractions;
using Querify.Models.Broadcast.Dtos.Thread;
using Querify.Models.Common.Dtos;
using Querify.Models.Common.Enums;
using ThreadEntity = Querify.Broadcast.Common.Domain.Entities.Thread;

namespace Querify.Broadcast.Portal.Business.Thread.Queries.GetThreadList;

public sealed class ThreadsGetListQueryHandler(
    BroadcastDbContext dbContext,
    ISessionService sessionService)
    : IRequestHandler<ThreadsGetListQuery, PagedResultDto<ThreadDto>>
{
    public async Task<PagedResultDto<ThreadDto>> Handle(
        ThreadsGetListQuery request,
        CancellationToken cancellationToken)
    {
        var tenantId = sessionService.GetTenantId(ModuleEnum.Broadcast);
        var dto = request.Request;
        var query = dbContext.Threads.AsNoTracking().Where(thread => thread.TenantId == tenantId);

        if (!string.IsNullOrWhiteSpace(dto.SearchText))
        {
            var search = $"%{dto.SearchText}%";
            query = query.Where(thread => EF.Functions.ILike(thread.Title ?? string.Empty, search));
        }

        if (dto.ChannelConnectionId.HasValue)
            query = query.Where(thread => thread.ChannelConnectionId == dto.ChannelConnectionId.Value);
        if (dto.Status.HasValue)
            query = query.Where(thread => thread.Status == dto.Status.Value);

        query = ApplySorting(query, dto.Sorting);
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.Skip(dto.SkipCount).Take(dto.MaxResultCount)
            .Select(thread => new ThreadDto
            {
                Id = thread.Id,
                TenantId = thread.TenantId,
                ChannelConnectionId = thread.ChannelConnectionId,
                Title = thread.Title,
                Status = thread.Status,
                ItemCount = thread.Items.Count,
                LastItemAtUtc = thread.Items
                    .OrderByDescending(item => item.CapturedAtUtc)
                    .Select(item => (DateTime?)item.CapturedAtUtc)
                    .FirstOrDefault(),
                CreatedAtUtc = thread.CreatedDate,
                LastUpdatedAtUtc = thread.UpdatedDate ?? thread.CreatedDate
            })
            .ToListAsync(cancellationToken);

        return new PagedResultDto<ThreadDto>(totalCount, items);
    }

    private static IQueryable<ThreadEntity> ApplySorting(
        IQueryable<ThreadEntity> query,
        string? sorting) => sorting?.Trim().ToLowerInvariant() switch
        {
            "title" or "title asc" => query.OrderBy(thread => thread.Title),
            "title desc" => query.OrderByDescending(thread => thread.Title),
            "status" or "status asc" => query.OrderBy(thread => thread.Status),
            "status desc" => query.OrderByDescending(thread => thread.Status),
            "itemcount" or "itemcount asc" => query.OrderBy(thread => thread.Items.Count),
            "itemcount desc" => query.OrderByDescending(thread => thread.Items.Count),
            "lastitematutc asc" => query.OrderBy(thread => thread.Items.Max(item => (DateTime?)item.CapturedAtUtc)),
            "lastitematutc desc" => query.OrderByDescending(thread => thread.Items.Max(item => (DateTime?)item.CapturedAtUtc)),
            "lastupdatedatutc asc" => query.OrderBy(thread => thread.UpdatedDate ?? thread.CreatedDate),
            _ => query.OrderByDescending(thread => thread.UpdatedDate ?? thread.CreatedDate)
        };
}
