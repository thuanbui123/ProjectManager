namespace CORE.Entities;

public class AttachmentEntity
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey("Task")]
    public Guid TaskId { get; set; }
    public TaskEntity? Task { get; set; }

    [Required]
    public string FileUrl { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? FileType { get; set; }

    public long? FileSize { get; set; }

    public DateTime UploadAt { get; set; } = DateTime.UtcNow;
    [MaxLength(200)]
    public string? UploadBy { get; set; }
}
