using BaseFaq.Common.EntityFramework.Tenant;
using BaseFaq.Common.EntityFramework.Tenant.Entities;
using MediatR;

namespace BaseFaq.Tenant.TenantWeb.Business.Commands.CreateTenantConnection;

public class TenantConnectionsCreateTenantConnectionCommandHandler(TenantDbContext dbContext)
    : IRequestHandler<TenantConnectionsCreateTenantConnectionCommand, Guid>
{
    public async Task<Guid> Handle(TenantConnectionsCreateTenantConnectionCommand request,
        CancellationToken cancellationToken)
    {
        var connection = new TenantConnection
        {
            TenantId = request.TenantId,
            ConnectionString = request.ConnectionString,
            IsCurrent = request.IsCurrent
        };

        await dbContext.TenantConnections.AddAsync(connection, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return connection.Id;
    }
}