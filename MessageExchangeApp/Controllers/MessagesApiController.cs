using MessageExchangeApp.Interfaces;
using MessageExchangeApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace MessageExchangeApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesApiController : ControllerBase
    {
        private readonly IMessageRepository _messageRepository;

        public MessagesApiController(IMessageRepository messageRepository)
        {
            _messageRepository = messageRepository;
        }

        /// <summary>
        /// Sends a message to the system.
        /// </summary>
        /// <param name="model">The message model containing the message details.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        [HttpPost("SendMessage")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SendMessage(string text, int sequenceNumber)
        {
            var result = await _messageRepository.AddMessageAndReturnAsync(text, sequenceNumber);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves recent messages since a given date.
        /// </summary>
        /// <param name="since">The date to retrieve recent messages from.</param>
        /// <returns>A list of recent messages.</returns>
        [HttpGet("RecentMessages")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> RecentMessages(DateTime startDate, DateTime endDate)
        {
            var result = await _messageRepository.GetMessagesByDateRangeAsync(startDate, endDate);
            return Ok(result);
        }
    }
}
