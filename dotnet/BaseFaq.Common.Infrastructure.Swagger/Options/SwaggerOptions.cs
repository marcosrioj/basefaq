using System.ComponentModel.DataAnnotations;

namespace BaseFaq.Common.Infrastructure.Swagger.Options;

public class SwaggerOptions
{
    public const string Name = "SwaggerOptions";
    [Required] public string Title { get; set; }
    [Required] public string Version { get; set; }
    public SwaggerAuthOptions swaggerAuth { get; set; }
}

public class SwaggerAuthOptions
{
    public bool EnableClientCredentials { get; set; }

    [Required] public string AuthorizeEndpoint { get; set; }
    [Required] public string TokenEndpoint { get; set; }
    [Required] public Dictionary<string, string> Scopes { get; set; }
}