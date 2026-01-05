using System.ComponentModel.DataAnnotations;

namespace QuickTalk.Messages.WebApi.Dto;

public class MessageInputModel
{
    [Required]
    public Guid Id { get; set; }

    [Required(AllowEmptyStrings = false)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    public string UserName { get; set; } = null!;

    [Required(AllowEmptyStrings = false)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    public string Text { get; set; } = null!;
}
