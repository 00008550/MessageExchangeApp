using MessageExchangeApp.Interfaces;
using MessageExchangeApp.WebSocketSetup;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace MessageExchangeApp.Controllers
{
    public class MessagesController : Controller
    {
        private readonly IMessageRepository _messageRepository;
        private readonly WebSocketHandler _webSocketHandler;
        private readonly ILogger<MessagesController> _logger;

        public MessagesController(ILogger<MessagesController> logger, IMessageRepository messageRepository, WebSocketHandler webSocketHandler)
        {
            _messageRepository = messageRepository;
            _webSocketHandler = webSocketHandler;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage(string text, int sequentialNumber)
        {
            if (ModelState.IsValid)
            {
                // Save the message to the database and retrieve the full message with Timestamp and Id
                var message = await _messageRepository.AddMessageAndReturnAsync(text, sequentialNumber);

                // Broadcast the exact saved message (with correct Timestamp and SequentialNumber)
                var messageData = new
                {
                    Text = message.Text,
                    Timestamp = message.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
                    SequentialNumber = message.SequenceNumber
                };

                var jsonMessage = JsonConvert.SerializeObject(messageData);

                await _webSocketHandler.BroadcastMessageAsync(jsonMessage);

                return View();
            }

            return View();
        }

        [HttpGet]
        public IActionResult SendMessage()
        {
            return View();
        }

        [HttpGet]
        public IActionResult RealTimeDisplay()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetRecentMessagesJson()
        {
            var tenMinutesAgo = DateTime.UtcNow.AddMinutes(-10);
            var messages = await _messageRepository.GetMessagesByDateRangeAsync(tenMinutesAgo, DateTime.UtcNow);
            return Json(messages);
        }

        [HttpGet]
        public IActionResult RecentMessages()
        {
            return View();
        }

    }
}
