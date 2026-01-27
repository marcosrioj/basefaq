using System.ComponentModel.DataAnnotations;

namespace BaseFaq.Common.Infrastructure.Swagger.Options;

public class SwaggerOptions
{
    public const string Name = "SwaggerOptions";
    [Required] public string Title { get; set; } = string.Empty;
    [Required] public string Version { get; set; } = string.Empty;
    public SwaggerAuthOptions? swaggerAuth { get; set; }
}

public class SwaggerAuthOptions
{
    public bool EnableClientCredentials { get; set; }

    [Required] public string AuthorizeEndpoint { get; set; } = string.Empty;
    [Required] public string TokenEndpoint { get; set; } = string.Empty;
    [Required] public Dictionary<string, string> Scopes { get; set; } =  new Dictionary<string, string>();
}