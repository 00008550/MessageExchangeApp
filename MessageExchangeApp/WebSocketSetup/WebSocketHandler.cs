using Newtonsoft.Json;
using System.Net.WebSockets;
using System.Text;

namespace MessageExchangeApp.WebSocketSetup
{
    public class WebSocketHandler
    {
        private readonly List<WebSocket> _connectedSockets = new List<WebSocket>();
        private readonly ILogger<WebSocketHandler> _logger;

        public WebSocketHandler(ILogger<WebSocketHandler> logger)
        {
            _logger = logger;
        }

        public async Task Handle(HttpContext context, WebSocket webSocket)
        {
            _connectedSockets.Add(webSocket);
            _logger.LogInformation($"New WebSocket connection established. Total connections: {_connectedSockets.Count}");

            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                _logger.LogInformation($"Received message from WebSocket: {message}");

                // Example of a received message structure
                var data = new
                {
                    Text = message,
                    Timestamp = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
                    SequentialNumber = 1 // Replace with actual sequential number logic
                };

                var jsonMessage = JsonConvert.SerializeObject(data);

                // Broadcast the JSON message to all connected clients
                await BroadcastMessageAsync(jsonMessage);

                _logger.LogInformation($"Broadcasted message: {jsonMessage}");

                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            _connectedSockets.Remove(webSocket);
            _logger.LogInformation("WebSocket connection closed. Total connections: {Count}", _connectedSockets.Count);

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }

        public async Task BroadcastMessageAsync(string message)
        {
            foreach (var socket in _connectedSockets.ToList())
            {
                if (socket.State == WebSocketState.Open)
                {
                    var encodedMessage = Encoding.UTF8.GetBytes(message);
                    await socket.SendAsync(new ArraySegment<byte>(encodedMessage), WebSocketMessageType.Text, true, CancellationToken.None);
                    _logger.LogInformation($"Sent message to a WebSocket: {message}");
                }
            }
        }
    }
}
