namespace Graduation.DTOs
{
    public class CreateMessageDto
    {
        public string RecipientUsername { get; set; }
        public string Content { get; set; }
        public string ImageUrl { get; set; }        // ✅ جديد
        public string? AudioUrl { get; set; }

        public string MessageType { get; set; } = "Text";  // ✅ جديد
    }
}
