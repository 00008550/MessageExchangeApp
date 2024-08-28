using MessageExchangeApp.Models;

namespace MessageExchangeApp.Interfaces
{
    public interface IMessageRepository
    {
        Task<Message> AddMessageAndReturnAsync(string content, int sequentialNumber);
        Task<IEnumerable<Message>> GetMessagesByDateRangeAsync(DateTime fromDate, DateTime toDate);
    }
}