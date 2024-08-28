﻿namespace MessageExchangeApp.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public DateTime Timestamp { get; set; }
        public int SequenceNumber { get; set; }
    }
}
