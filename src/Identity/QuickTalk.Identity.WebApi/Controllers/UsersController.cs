//using Microsoft.AspNetCore.Mvc;
//using QuickTalk.Identity.Application.Interfaces;

//namespace QuickTalk.Identity.WebApi.Controllers;

//[Route("api/[controller]")]
//[ApiController]
//public class UsersController(IUserService userService) : ControllerBase
//{
//    [HttpGet]
//    public async Task<IActionResult> GetRegisteredUsersIdentifiers()
//    {
//        var result = await userService.GetUsersIdAsync();

//        return Ok(result.Data?.ToList());
//    }
//}
