using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BaseFaq.Identity.Persistence.IdentityDb.Models;

namespace BaseFaq.Identity.Persistence.IdentityDb;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ApplicationUser>(options), IDataProtectionKeyContext
{
    public DbSet<Group> Groups { get; set; }

    public DbSet<DataProtectionKey> DataProtectionKeys { get; }
}