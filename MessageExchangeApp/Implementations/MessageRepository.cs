using Dapper;
using MessageExchangeApp.Interfaces;
using MessageExchangeApp.Models;
using System.Data;

namespace MessageExchangeApp.Implementations
{
    public class MessageRepository : IMessageRepository
    {
        private readonly IDbConnection _dbConnection;
        private readonly ILogger<MessageRepository> _logger;

        public MessageRepository(ILogger<MessageRepository> logger, IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
            _logger = logger;
        }

        public async Task<Message> AddMessageAndReturnAsync(string content, int sequentialNumber)
        {
            var sql = @"
            INSERT INTO Messages (Text, SequenceNumber, Timestamp)
            VALUES (@Text, @SequentialNumber, @Timestamp)
            RETURNING Id, Text, SequenceNumber, Timestamp;";

            var parameters = new
            {
                Text = content,
                SequentialNumber = sequentialNumber,
                Timestamp = DateTime.UtcNow
            };

            var result = await _dbConnection.QuerySingleAsync<Message>(sql, parameters);
            _logger.LogInformation($"Added message into databse with text: {result.Text}, sequence number: {result.SequenceNumber} at {result.Timestamp}");

            return result;
        }

        public async Task<IEnumerable<Message>> GetMessagesByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            var sql = "SELECT * FROM Messages WHERE Timestamp >= @FromDate AND Timestamp <= @ToDate ORDER BY Timestamp;";
            var result = await _dbConnection.QueryAsync<Message>(sql, new { FromDate = fromDate, ToDate = toDate });

            _logger.LogInformation($"Requested messages from: {fromDate} to: {toDate}");
            return result;
        }
    }
}
