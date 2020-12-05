using DigitexConnector.Enums;

namespace DigitexConnector
{
    public static class Configuration
    {
        public static Servers? Server;
        public static string Token { set; internal get; }
    }
}
