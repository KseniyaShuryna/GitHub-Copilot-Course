namespace ToDo.Application.Options
{
    public class JwtOptions
    {
        public const string ConfigKey = "Jwt";

        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
    }
}
