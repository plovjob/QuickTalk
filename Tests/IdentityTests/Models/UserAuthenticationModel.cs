using System.ComponentModel.DataAnnotations;

namespace QuickTalk.Identity.ComponentTests.Models;

class UserAuthenticationModel
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
