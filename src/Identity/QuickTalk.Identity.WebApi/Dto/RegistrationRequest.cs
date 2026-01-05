using System.ComponentModel.DataAnnotations;

namespace QuickTalk.Identity.WebApi.Dto;

public class RegistrationRequest
{
    [Required(AllowEmptyStrings = false)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    public string Email { get; set; } = "";

    [Required(AllowEmptyStrings = false)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    public string UserName { get; set; } = "";

    [Required(AllowEmptyStrings = false)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    public string Password { get; set; } = "";
}
