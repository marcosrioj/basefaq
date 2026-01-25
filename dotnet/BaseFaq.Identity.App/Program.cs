using BaseFaq.Identity.App.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

var app = builder
    .ConfigureServices()
    .ConfigurePipeline();

app.Run();