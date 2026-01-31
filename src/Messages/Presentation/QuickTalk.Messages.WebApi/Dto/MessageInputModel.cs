using System.ComponentModel.DataAnnotations;

namespace QuickTalk.Messages.WebApi.Dto;

public class MessageInputModel
{
    [Required]
    public Guid MessageId { get; set; }

    [Required]
    public Guid FromUserId { get; set; }

    [Required]
    public Guid ToUserId { get; set; }

    [Required]
    public string UserName { get; set; } = null!;

    [Required(AllowEmptyStrings = false)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    public string Text { get; set; } = null!;
}
