using BaseFaq.Common.EntityFramework.Tenant;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BaseFaq.Tenant.TenantWeb.Business.Commands.UpdateTenantConnection;

public class TenantConnectionsUpdateTenantConnectionCommandHandler(TenantDbContext dbContext)
    : IRequestHandler<TenantConnectionsUpdateTenantConnectionCommand>
{
    public async Task Handle(TenantConnectionsUpdateTenantConnectionCommand request,
        CancellationToken cancellationToken)
    {
        var connection = await dbContext.TenantConnections
            .FirstOrDefaultAsync(entity => entity.Id == request.Id, cancellationToken);

        if (connection is null)
        {
            throw new KeyNotFoundException($"Tenant connection '{request.Id}' was not found.");
        }

        connection.App = request.App;
        connection.ConnectionString = request.ConnectionString;
        connection.IsCurrent = request.IsCurrent;

        dbContext.TenantConnections.Update(connection);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}