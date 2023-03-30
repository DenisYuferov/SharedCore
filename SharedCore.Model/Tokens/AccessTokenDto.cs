namespace SharedCore.Model.Tokens
{
    public class AccessTokenDto
    {
        public string? Token { get; set; }
        public DateTimeOffset Expires { get; set; }
    }
}
