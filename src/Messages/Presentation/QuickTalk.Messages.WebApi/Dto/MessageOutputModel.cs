using System.ComponentModel.DataAnnotations;

namespace QuickTalk.Messages.WebApi.Dto;

public class MessageOutputModel()
{
    [Required]
    public Guid Id { get; set; }

    [Required(AllowEmptyStrings = false)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    public string Text { get; set; } = null!;

    public DateTime SentAt { get; set; }
    public DateTime? EditedAt { get; set; } = null;

    public Guid FromUserId { get; set; }
    public Guid ToUserId { get; set; }
}
