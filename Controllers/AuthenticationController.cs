using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using MailUtils;
using InterFace;

namespace App.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]

public class AuthenticationController : ControllerBase
{
    private readonly TaphoaEntities context;
    private readonly UserManager<ma_user> userManager;
    private readonly SignInManager<ma_user> signInManager;

    public AuthenticationController(TaphoaEntities _context, UserManager<ma_user> _userManager, SignInManager<ma_user> _signInManager)
    {
        context = _context;
        userManager = _userManager;
        signInManager = _signInManager;
    }

    [HttpPost]
    public async Task<IActionResult> Register([FromBody] RegisUser register)
    {
        if (string.IsNullOrEmpty(register.cd_phone_number) || string.IsNullOrEmpty(register.password))
            return NotFound(new ResponseResult("E400", "Thông tin đăng kí chưa đủ."));

        var user = new ma_user
        {
            cd_phone_number = register.cd_phone_number,
            UserName = register.name,
            cd_store = register.cd_store,
            cd_password = register.cd_password,
            flg_announce_exp = register.flg_announce_exp,
            flg_announce_fast = register.flg_announce_fast,
            flg_announce_slow = register.flg_announce_slow,
            flg_announce_soldout = register.flg_announce_soldout,
            lst_announce = register.lst_announce,
            flg_delete = register.flg_delete,
        };

        var result = await userManager.CreateAsync(user, register.password);

        if (result.Succeeded)
            return Ok(new ResponseResult("I200", "Đăng kí thành công.", new { result = result }));

        if (result.Errors.Count() > 0)
            return NotFound(new ResponseResult("E400", "Đăng kí thất bại.", new { result = result.Errors }));

        return NotFound(new ResponseResult("E400", "Đăng kí thất bại."));

    }

    [HttpPost]
    public async Task<IActionResult> LogOut()
    {
        var user = await userManager.GetUserAsync(User);
        if (user == null)
        {
            return Ok(new ResponseResult("I200", "Người dùng chưa đăng nhập."));
        }
        await signInManager.SignOutAsync();

        HttpContext.Response.Cookies.Delete(".AspNetCore.Identity.Application");

        return Ok(new ResponseResult("I200", "Đăng xuất thành công."));
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginInfo info)
    {

        if (signInManager.IsSignedIn(User))
            return NotFound(new ResponseResult("E400", "Bận đã đăng nhập rồi.", ""));

        var result = await signInManager.PasswordSignInAsync(
                    info.cd_phone_number,
                    info.password,
                    false,
                    true
                );

        if (!result.Succeeded)
            return NotFound(new ResponseResult("E400", "Tài khoản hoặc mật khẩu không đúng.", ""));

        string token = string.Empty;
        token = JwtTokenCreateModule.GenerateToken(info.cd_phone_number);

        return Ok(new ResponseResult("I200", "Đăng nhập thành công.", new { Token = token, userName = User.Identity.Name }));
    }

    [HttpPost]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePass pw)
    {
        var user = await userManager.GetUserAsync(User);

        if (user == null)
        {
            return NotFound(new ResponseResult("E400", "Tài khoản hoặc mật khẩu không đúng."));
        }

        var result = await userManager.ChangePasswordAsync(user, pw.oldPw, pw.newPw);

        if (!result.Succeeded)
            return NotFound(new ResponseResult("E400", "Thay đổi mật khẩu thất bại.", result.Errors));

        await signInManager.SignOutAsync();
        HttpContext.Response.Cookies.Delete(".AspNetCore.Identity.Application");

        return Ok(new ResponseResult("I400", "Thay đổi mật khẩu thành công."));
    }

    [HttpPost]
    public async Task<IActionResult> ChangePhone([FromBody] ChangePhone phone)
    {

        var user = await userManager.GetUserAsync(User);

        if (user == null)
        {
            return NotFound(new ResponseResult("E400", "Tài khoản hoặc mật khẩu không đúng."));
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

        if(!(await userManager.IsEmailConfirmedAsync(user)))
            return NotFound(new ResponseResult("E400", "Người dùng chưa có mail."));

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var email = user.Email ?? "";

        var resetLink = Url.Action("ResetPassword", "Authentication", new { email = email, password = password, token = token }, Request.Scheme);

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
    public async Task<IActionResult> ResetPassword(string email, string password, string token)
    {
        var user = context.ma_user.Where(x => x.Email == email).FirstOrDefault();
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

public class RegisUser
{
    public string cd_phone_number { get; set; }
    public string cd_store { get; set; }
    public string cd_password { get; set; }
    public string name { get; set; }
    public string password { get; set; }
    public short flg_announce_exp { get; set; }
    public short flg_announce_fast { get; set; }
    public short flg_announce_slow { get; set; }
    public short flg_announce_soldout { get; set; }
    public string lst_announce { get; set; }
    public short flg_delete { get; set; }
}