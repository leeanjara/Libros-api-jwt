namespace Library.API.Configuration
{
    public class JwtConfig
    {
        public string SecretKey { get; init; } = null!;
        public string Audience { get; init; } = null!;
    }
}
