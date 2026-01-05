using System.ComponentModel.DataAnnotations;

namespace QuickTalk.Identity.WebApi.Dto;

public class LoginRequest
{
    [Required(AllowEmptyStrings = false)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    public string Email { get; set; } = "";

    [Required(AllowEmptyStrings = false)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    public string Password { get; set; } = "";
}
