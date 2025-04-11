using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using MailUtils;
using InterFace;

namespace App.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]

public class AuthenticationController : ControllerBase
{
    private readonly AppDBContext context;
    private readonly UserManager<ma_user> userManager;
    private readonly SignInManager<ma_user> signInManager;

    public AuthenticationController(AppDBContext _context, UserManager<ma_user> _userManager, SignInManager<ma_user> _signInManager)
    {
        context = _context;
        userManager = _userManager;
        signInManager = _signInManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetSearchUser(string phone)
    {
        var result = context.ma_user.Where(x => x.cd_phone_number == phone).FirstOrDefault();

        return Ok(new ResponseResult("I200", SystemMessages.Get("99_SearchSusscess"), result));
    }

    [HttpPost]
    public async Task<IActionResult> PostUser([FromBody] User user)
    {
        var result = context.ma_user.Where(x => x.cd_phone_number == user.cd_phone_number).FirstOrDefault();
        if (result == null)
            return Ok(new ResponseResult("E400", SystemMessages.Get("A_UserNotFound")));

        result.name = user.name;
        result.cd_store = user.cd_store;
        result.Email = user.Email;

        try
        {
            context.Update(result);
            context.SaveChanges();
        }
        catch (Exception ex)
        {
            throw ex;
        }

        return Ok(new ResponseResult("I200", SystemMessages.Get("A_RegisDone"), new { result = "" }));
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] User register)
    {
        if (string.IsNullOrEmpty(register.cd_phone_number) || string.IsNullOrEmpty(register.password))
            return NotFound(new ResponseResult("E400", "Thông tin đăng kí chưa đủ."));

        var isReadyExistsUser = context.ma_user.Where(x => x.cd_phone_number == register.cd_phone_number).FirstOrDefault();
        if (isReadyExistsUser != null)
            return NotFound(new ResponseResult("E400", "Tài khoản đã tồn tại, vui lòng đăng kí số điện thoại khác."));

        var user = new ma_user
        {
            cd_phone_number = register.cd_phone_number,
            UserName = register.cd_phone_number,
            cd_store = register.cd_store,
            cd_password = register.cd_password,
            Email = register.Email,
            flg_announce_exp = 0,
            flg_announce_fast = 0,
            flg_announce_slow = 0,
            flg_announce_soldout = 0,
            lst_announce = "",
            flg_delete = 0,
        };

        var result = await userManager.CreateAsync(user, register.password);

        if (!result.Succeeded)
            return NotFound(new ResponseResult("E400", "Đăng kí thất bại.", new { result = result.Errors }));

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var email = user.Email ?? "";

        var resetLink = Url.Action("ConformMail", "Authentication", new { phone = user.cd_phone_number, email = email, token = token }, Request.Scheme);

        bool res = await MailProider.SendGmail(
           email
            , email
            , "Confirm Account"
            , "Nhấn vào <a href='" + resetLink + "' >đây</a> để hoàn tất đăng kí tài khoản."
            , email
            , ""
        );

        if (!res)
            return NotFound(new ResponseResult("E400", "Đăng kí thất bại."));

        await userManager.SetAuthenticationTokenAsync(user, "Gmail", "ConformMail", token);

        return Ok(new ResponseResult("I200", "Đăng kí thành công. Xác thực mail để hoàn tất đăng kí", new { result = result }));
    }

    [HttpGet]
    public async Task<IActionResult> ConformMail(string phone, string email, string token)
    {
        var user = context.ma_user.Where(x => x.Email == email && x.cd_phone_number == phone).FirstOrDefault();
        if (user == null)
        {
            return NotFound(new ResponseResult("E400", "Người dùng không tồn tại!"));
        }

        var userToken = await userManager.GetAuthenticationTokenAsync(user, "Gmail", "ConformMail");

        if (userToken == null || userToken != token)
            return NotFound(new ResponseResult("E400", "Xác thực Token không thành công!"));


        var res = await userManager.ConfirmEmailAsync(user, token);

        if (!res.Succeeded)
            return NotFound(new ResponseResult("E400", "Đăng kí thất bại.", new { result = res.Errors }));

        await userManager.RemoveAuthenticationTokenAsync(user, "Gmail", "ConformMail");

        return Ok(new ResponseResult("E400", "Đăng kí tài khoản hoàn tất."));

    }

    [HttpPost]
    public async Task<IActionResult> LogOut()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return Ok(new ResponseResult("I200", SystemMessages.Get("A_UserLoginYet")));
        }
        await signInManager.SignOutAsync();

        HttpContext.Response.Cookies.Delete(".AspNetCore.Identity.Application");

        return Ok(new ResponseResult("I200", SystemMessages.Get("A_UserLoginYet")));
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginInfo info)
    {

        if (signInManager.IsSignedIn(User))
        {
            await signInManager.SignOutAsync();
            HttpContext.Response.Cookies.Delete(".AspNetCore.Identity.Application");
        }

        var user = context.ma_user.Where(x => x.cd_phone_number == info.cd_phone_number).FirstOrDefault();
        if (user == null)
            return NotFound(new ResponseResult("E400", SystemMessages.Get("A_UserNotFound"), ""));

        var isConfirmMail = await userManager.IsEmailConfirmedAsync(user);
        if (!isConfirmMail)
            return NotFound(new ResponseResult("E400", SystemMessages.Get("A_ConfirmEmailYet"), ""));

        var result = await signInManager.PasswordSignInAsync(
                    info.cd_phone_number,
                    info.password,
                    false,
                    true
                );

        if (!result.Succeeded)
            return NotFound(new ResponseResult("E400", SystemMessages.Get("A_NotCorrectUser"), ""));

        string token = string.Empty;

        token = JwtTokenCreateModule.GenerateToken(info.cd_phone_number);

        return Ok(new ResponseResult("I200", SystemMessages.Get("A_LoginSuccess"), new { Token = token, phone = user.cd_phone_number }));
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePass pw)
    {
        var user = await userManager.GetUserAsync(User);

        if (user == null)
        {
            return NotFound(new ResponseResult("E400", SystemMessages.Get("A_NotCorrectUser")));
        }

        var result = await userManager.ChangePasswordAsync(user, pw.oldPw, pw.newPw);

        if (!result.Succeeded)
            return NotFound(new ResponseResult("E400", SystemMessages.Get("A_FailChangePassWord"), result.Errors));

        await signInManager.SignOutAsync();
        HttpContext.Response.Cookies.Delete(".AspNetCore.Identity.Application");

        return Ok(new ResponseResult("I400", SystemMessages.Get("A_SusscessChangePassWord")));
    }

    [HttpPost]
    public async Task<IActionResult> ChangePhone([FromBody] ChangePhone phone)
    {

        var user = await userManager.GetUserAsync(User);

        if (user == null)
        {
            return NotFound(new ResponseResult("E400", SystemMessages.Get("A_NotCorrectUser")));
        }

        var delUser = await userManager.DeleteAsync(user);

        if (!delUser.Succeeded)
            return NotFound(new ResponseResult("E400", "Thay đổi số điện thoại thất bại.", delUser.Errors));

        user.cd_phone_number = phone.nwPhone;
        user.UserName = phone.nwPhone;

        var result = await userManager.CreateAsync(user);

        if (!result.Succeeded)
            return NotFound(new ResponseResult("E400", "Thay đổi số điện thoại thất bại.", result.Errors));

        await signInManager.SignOutAsync();
        HttpContext.Response.Cookies.Delete(".AspNetCore.Identity.Application");

        return Ok(new ResponseResult("I200", "Thay đổi số điện thoại thành công."));
    }

    [HttpGet]
    public async Task<IActionResult> ForgotPw(string phone, string password)
    {
        var user = context.ma_user.Where(x => x.cd_phone_number == phone).FirstOrDefault();

        if (user == null)
            return NotFound(new ResponseResult("E400", "Số điện thoại này không tồn tại."));

        if (!(await userManager.IsEmailConfirmedAsync(user)))
            return NotFound(new ResponseResult("E400", "Người dùng chưa xác thực mail."));

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var email = user.Email ?? "";

        var resetLink = Url.Action("ResetPassword", "Authentication", new { phone = phone, email = email, password = password, token = token }, Request.Scheme);

        bool res = await MailProider.SendGmail(
           email
            , email
            , "Reset Password"
            , "Nhấn vào <a href='" + resetLink + "' >đây</a> để xác nhận lấy lại mật khẩu."
            , email
            , ""
        );

        if (!res)
            return NotFound(new ResponseResult("E400", "Đã có quá trình lỗi ngoại lệ xác nhận mail. Vui lòng thử lại sau."));

        await userManager.SetAuthenticationTokenAsync(user, "Gmail", "ResetPassword", token);

        return Ok(new ResponseResult("I200", "Đã gữi link xác nhận về địa chỉ mail: " + email + " .Vui lòng kiểm tra hộp thư!"));
    }

    [HttpGet]
    public async Task<IActionResult> ResetPassword(string phone, string email, string password, string token)
    {
        var user = context.ma_user.Where(x => x.Email == email && x.cd_password == phone).FirstOrDefault();
        if (user == null || !(await userManager.IsEmailConfirmedAsync(user)))
        {
            return NotFound(new ResponseResult("E400", "Email người dùng không tồn tại!"));
        }

        var userToken = await userManager.GetAuthenticationTokenAsync(user, "Gmail", "ResetPassword");

        if (userToken == null || userToken != token)
            return NotFound(new ResponseResult("E400", "Xác thực Token không thành công!"));

        var res = await userManager.ResetPasswordAsync(user, token, password);

        if (!res.Succeeded)
            return NotFound(new ResponseResult("E400", "Thay đổi mật khẩu không thành công!"));

        return Ok("Thay đổi mật khẩu thành công.");
    }
}

public class LoginInfo
{
    public LoginInfo() { }
    public string cd_phone_number { get; set; }
    public string password { get; set; }
}

public class ChangePass
{
    public string oldPw { get; set; }
    public string newPw { get; set; }
}

public class ChangePhone
{
    public string nwPhone { get; set; }
    public string password { get; set; }
}

public class User
{
    public string? cd_phone_number { get; set; }
    public string? cd_store { get; set; }
    public string? cd_password { get; set; }
    public string? name { get; set; }
    public string? Email { get; set; }
    public string? password { get; set; }
}