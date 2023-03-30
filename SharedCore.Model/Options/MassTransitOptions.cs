namespace SharedCore.Model.Options
{
    public class MassTransitOptions
    {
        public const string MassTransit = "MassTransit";

        public string? Host { get; set; }
        public string? VirtualHost { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
    }
}