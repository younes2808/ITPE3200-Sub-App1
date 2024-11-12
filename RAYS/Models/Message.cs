namespace RAYS.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int ReceiverId { get; set; }
        public required string Content { get; set; } // Required to be set during object creation
        public DateTime Timestamp { get; set; }
    }

}
