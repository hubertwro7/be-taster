namespace Taster.Infrastructure.Auth
{
    public class JwtAuthenticationOptions
    {
        public string? Secret { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public int ExpireInDays { get; set; } = 30;
    }
}
