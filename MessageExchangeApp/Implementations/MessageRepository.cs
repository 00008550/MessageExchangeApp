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

            using (var command = _dbConnection.CreateCommand())
            {
                command.CommandText = sql;
                command.CommandType = CommandType.Text;

                var textParam = command.CreateParameter();
                textParam.ParameterName = "@Text";
                textParam.Value = content;
                command.Parameters.Add(textParam);

                var seqNumParam = command.CreateParameter();
                seqNumParam.ParameterName = "@SequentialNumber";
                seqNumParam.Value = sequentialNumber;
                command.Parameters.Add(seqNumParam);

                var timestampParam = command.CreateParameter();
                timestampParam.ParameterName = "@Timestamp";
                timestampParam.Value = DateTime.UtcNow;
                command.Parameters.Add(timestampParam);

                _dbConnection.Open();
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        var message = new Message
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Text = reader.GetString(reader.GetOrdinal("Text")),
                            SequenceNumber = reader.GetInt32(reader.GetOrdinal("SequenceNumber")),
                            Timestamp = reader.GetDateTime(reader.GetOrdinal("Timestamp"))
                        };

                        _logger.LogInformation($"Added message into database with text: {message.Text}, sequence number: {message.SequenceNumber} at {message.Timestamp}");
                        return message;
                    }
                }
                _dbConnection.Close();
            }

            return null;
        }


        public async Task<IEnumerable<Message>> GetMessagesByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            var sql = "SELECT * FROM Messages WHERE Timestamp >= @FromDate AND Timestamp <= @ToDate ORDER BY Timestamp;";
            var messages = new List<Message>();

            using (var command = _dbConnection.CreateCommand())
            {
                command.CommandText = sql;
                command.CommandType = CommandType.Text;

                var fromDateParam = command.CreateParameter();
                fromDateParam.ParameterName = "@FromDate";
                fromDateParam.Value = fromDate;
                command.Parameters.Add(fromDateParam);

                var toDateParam = command.CreateParameter();
                toDateParam.ParameterName = "@ToDate";
                toDateParam.Value = toDate;
                command.Parameters.Add(toDateParam);

                _dbConnection.Open();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var message = new Message
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Text = reader.GetString(reader.GetOrdinal("Text")),
                            SequenceNumber = reader.GetInt32(reader.GetOrdinal("SequenceNumber")),
                            Timestamp = reader.GetDateTime(reader.GetOrdinal("Timestamp"))
                        };

                        messages.Add(message);
                    }
                }
                _logger.LogInformation($"Requested messages from: {fromDate} to: {toDate}");
                _dbConnection.Close();
            }

            return messages;
        }
    }
}
