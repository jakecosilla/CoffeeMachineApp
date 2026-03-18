namespace Application
{
    public static class Constants
    {
        public static class Messages
        {
            public const string AprilFoolsLog = "It's April 1st - returning 418 I'm a teapot";
            public const string CoffeeBrewCallLog = "Coffee brew call #{CallCount}";
            public const string OutOfCoffeeLog = "Out of coffee - returning 503 Service Unavailable on call #{CallCount}";
            public const string CoffeeReadyMessage = "Your piping hot coffee is ready";
            public const string CoffeeBrewedSuccessfullyLog = "Coffee brewed successfully at {Prepared}";
        }

        public static class Formats
        {
            public const string IsoDateFormat = "O"; // ISO-8601 format
        }

        public static class HttpStatusCodes
        {
            public const int ImATeapot = 418;
            public const int ServiceUnavailable = 503;
            public const int Ok = 200;
        }
    }
}
