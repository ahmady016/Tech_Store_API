namespace Dtos;

public class CommentDto
{
    public Guid Id { get; set; }
    public string Text { get; set; }
    public bool IsApproved { get; set; } = false;
    public string ApprovedBy { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public string CustomerId { get; set; }
    public Guid ModelId { get; set; }

    public List<ReplyDto> Replies { get; set; }
}
