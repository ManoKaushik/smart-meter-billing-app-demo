namespace SmartMeterWeb.Configs
{
    public class AuthSettings
    {
        public const int MaxAttempts = 3;
        public static readonly TimeSpan Duration = TimeSpan.FromMinutes(5);
    }
}
