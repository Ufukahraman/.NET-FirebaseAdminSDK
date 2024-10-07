namespace FcmService
{
    public class Variables
    {
        public string? Value { get; set; }
    }

    public class NotificationRequest
    {
        public string? DeviceToken { get; set; }
        public string? Title { get; set; }
        public string? Body { get; set; }
    }
}
