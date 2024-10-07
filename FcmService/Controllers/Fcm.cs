using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace FcmService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FCMController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public FCMController(IConfiguration configuration)
        {
            _configuration = configuration;

            // Firebase App'in yalnýzca bir kere baþlatýldýðýndan emin olun
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(_configuration["Firebase:Credentials"])
                });
            }
        }

        [HttpPost("send-notification")]
        public async Task<IActionResult> SendNotification([FromBody] NotificationRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.DeviceToken) || string.IsNullOrEmpty(request.Title) || string.IsNullOrEmpty(request.Body))
            {
                return BadRequest("Invalid notification request.");
            }

            var message = new Message()
            {
                Notification = new Notification
                {
                    Title = request.Title,
                    Body = request.Body
                },
                Token = request.DeviceToken,
                Android = new AndroidConfig
                {
                    Priority = Priority.High
                },
                Apns = new ApnsConfig
                {
                    Aps = new Aps
                    {
                        ContentAvailable = true,
                        Alert = new ApsAlert
                        {
                            Title = request.Title,
                            Body = request.Body
                        }
                    }
                }
            };

            try
            {
                string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
                return Ok(new { MessageId = response });
            }
            catch (FirebaseMessagingException ex)
            {
                return StatusCode(500, $"Error sending message: {ex.Message}");
            }
        }
    }
}
