﻿namespace Graduation.DTOs
{
    public class MessageDto
{
    public int Id { get; set; }
    public int SenderId { get; set; }
    public string SenderUsername { get; set; }
    public string SenderPhotoUrl { get; set; }
    public int RecipientId { get; set; }
    public string RecipientUsername { get; set; }
    public string RecipientPhotoUrl { get; set; }
    public string Content { get; set; }
    public string? ImageUrl { get; set; }
    public string? AudioUrl { get; set; }


    public string MessageType { get; set; }
    public DateTime? DateRead { get; set; }
    public DateTime MessageSent { get; set; }
}
}
