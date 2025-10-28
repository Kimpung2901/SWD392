using System.Threading.Tasks;
using FirebaseAdmin.Messaging;
using BLL.IService;

namespace BLL.Services
{
    public class NotificationService : INotificationService
    {
        public async Task<string> SendNotificationAsync(string title, string body, string deviceToken)
        {
            // Tạo message gửi tới 1 device cụ thể (theo FCM token)
            var message = new Message
            {
                Token = deviceToken,
                Notification = new Notification
                {
                    Title = title,
                    Body = body
                },
                // Bạn có thể gửi thêm data tuỳ ý cho app (optional)
                Data = new Dictionary<string, string>
                {
                    { "type", "order_update" },
                    { "extra", "hello_from_backend" }
                }
            };

            // Gửi lên FCM
            string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);

            // response thường là messageId
            return response;
        }
    }
}
