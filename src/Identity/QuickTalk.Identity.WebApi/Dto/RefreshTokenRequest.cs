using System.ComponentModel.DataAnnotations;

namespace QuickTalk.Identity.WebApi.Dto;

public class RefreshTokenRequest
{
    //[Required(AllowEmptyStrings = false)]
    //[DisplayFormat(ConvertEmptyStringToNull = false)]
    //public string AccessToken { get; set; } = "";

    [Required(AllowEmptyStrings = false)]
    [DisplayFormat(ConvertEmptyStringToNull = false)]
    public string RefreshToken { get; set; } = null!;
}
