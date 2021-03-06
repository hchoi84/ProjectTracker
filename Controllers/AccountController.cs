using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Models;
using ProjectTracker.ViewModels;
using ProjectTracker.Securities;
using ProjectTracker.Utilities;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Security.Claims;

namespace ProjectTracker.Controllers
{
  [AllowAnonymous]
  public class AccountController : Controller
  {
    private readonly IMember _member;
    private readonly SignInManager<Member> _signInManager;
    private readonly UserManager<Member> _userManager;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IMember member, SignInManager<Member> signInManager, UserManager<Member> userManager, ILogger<AccountController> logger)
    {
      _signInManager = signInManager;
      _userManager = userManager;
      _member = member;
      _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Login(string returnUrl)
    {
      LoginViewModel loginVM = new LoginViewModel
      {
        ReturnURL = returnUrl,
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
      };

      return View(loginVM);
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel loginVM, string returnUrl)
    {
      loginVM.ReturnURL = returnUrl;
      loginVM.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

      if (ModelState.IsValid)
      {
        Member member = await _userManager.FindByEmailAsync(loginVM.Email);

        if (member != null)
        {
          bool isValidPassword = await _userManager.CheckPasswordAsync(member, loginVM.Password);
          bool isEmailConfirmed = member.EmailConfirmed;

          if (isValidPassword && !isEmailConfirmed)
          {
            await SendEmailConfirmationLinkAsync(member);

            ModelState.AddModelError(string.Empty, "Email is not confirmed yet. New confirmation link as been sent. Please check your Inbox for email confirmation link. Alternatively, please check your Spam as well.");
            return View();
          }
        }
        else
        {
          ModelState.AddModelError(string.Empty, "Invalid login attempt");
          return View(loginVM);
        }

        var result = await _signInManager.PasswordSignInAsync(loginVM.Email, loginVM.Password, loginVM.RememberMe, false);

        if (result.Succeeded)
        {
          if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);
          else
            return RedirectToAction("Index", "Home");
        }
      }

      ModelState.AddModelError(string.Empty, "Invalid login attempt");
      return View(loginVM);
    }

    [HttpGet]
    public IActionResult ForgotPassword() => View();

    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
      if (ModelState.IsValid)
      {
        Member member = await _userManager.FindByEmailAsync(model.Email);

        if (member != null && member.EmailConfirmed)
        {
          string token = await _userManager.GeneratePasswordResetTokenAsync(member);

          var passwordResetLink = Url.Action("ResetPassword", "Account", new { email = model.Email, token = token }, Request.Scheme);

          EmailClient.SendLink(member, passwordResetLink, EmailType.PasswordReset);

          ViewBag.ConfirmMessage = "Your request to reset your password has been confirmed. Please check your email for reset link";
          return View("Confirmation");
        }
      }

      return View();
    }

    [HttpGet]
    public IActionResult ResetPassword(string token, string email)
    {
      if (token == null || email == null)
      {
        ModelState.AddModelError(string.Empty, "Invalid password reset token");
      }

      return View();
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
      if (ModelState.IsValid)
      {
        Member member = await _userManager.FindByEmailAsync(model.Email);

        if (member != null)
        {
          var result = await _userManager.ResetPasswordAsync(member, model.Token, model.Password);

          if (result.Succeeded)
          {
            ViewBag.ConfirmMessage = "Your password has been reset";
            return View("Confirmation");
          }

          foreach (var error in result.Errors)
          {
            ModelState.AddModelError(string.Empty, error.Description);
          }

          return View();
        }

        ViewBag.ConfirmMessage = "Your password has been reset";
        return View("Confirmation");
      }

      return View();
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
      await _signInManager.SignOutAsync();
      return RedirectToAction("Login");
    }

    [HttpGet]
    public IActionResult Register() => View();

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel regVM)
    {
      if (!ModelState.IsValid)
      {
        return View();
      }

      var result = await _member.RegisterAsync(regVM);

      if (result.Succeeded)
      {
        Member member = await _userManager.FindByEmailAsync(regVM.Email);
        await SendEmailConfirmationLinkAsync(member);

        // return RedirectToAction("Login");
        ViewBag.ConfirmTitle = "Registration Successful";
        ViewBag.ConfirmMessage = "Please check your Inbox or Spam folder for confirmation link. If you do not receive it within 5 minutes, try to login to receive new confirmation link.";
        return View("Confirmation");
      }

      foreach (var error in result.Errors)
      {
        ModelState.AddModelError(string.Empty, error.Description);
      }

      return View();
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string memberId, string token)
    {
      if (memberId == null || token == null)
      {
        return RedirectToAction("Index", "Home");
      }

      Member member = await _userManager.FindByIdAsync(memberId);

      if (member == null)
      {
        ViewBag.ErrorMessage = $"The Member Id is invalid";
        return View("Error");
      }

      var result = await _userManager.ConfirmEmailAsync(member, token);

      if (result.Succeeded)
      {
        ViewBag.ConfirmTitle = "Email Confirmed";
        ViewBag.ConfirmMessage = "Thank you for confirming your email address. You may now login by <a href=\"/Account/Login\" class=\"text-primary\"><u>clicking here</u></a>";
        return View("Confirmation");
      }

      ViewBag.ErrorTitle = "Can't Confirm";
      ViewBag.ErrorMessage = "Either the confirmation link has expired or it's been tampered. Please request a new link by trying to <a href=\"/Account/Login\" class=\"text-primary\"><u>Login</u></a>";
      return View("Error");
    }

    private async System.Threading.Tasks.Task SendEmailConfirmationLinkAsync(Member member)
    {
      var token = await _userManager.GenerateEmailConfirmationTokenAsync(member);
      var emailConfirmationLink = Url.Action("ConfirmEmail", "Account", new { memberId = member.Id, token = token }, Request.Scheme);

      // TODO: Remove this line for public
      _logger.Log(LogLevel.Warning, emailConfirmationLink);

      EmailClient.SendLink(member, emailConfirmationLink, EmailType.EmailConfirmation);
    }

    [HttpPost]
    public IActionResult ExternalLogin(string provider, string returnUrl)
    {
      var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { returnUrl = returnUrl });

      var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

      return new ChallengeResult(provider, properties);
    }

    public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
    {
      returnUrl ??= Url.Content("~/");

      LoginViewModel loginViewModel = new LoginViewModel
      {
        ReturnURL = returnUrl,
        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
      };

      if (remoteError != null)
      {
        ModelState.AddModelError(string.Empty, $"Error: {remoteError}");

        return View("Login", loginViewModel);
      }

      ExternalLoginInfo info = await _signInManager.GetExternalLoginInfoAsync();

      if (info == null)
      {
        ModelState.AddModelError(string.Empty, $"Error: Unknown");

        return View("Login", loginViewModel);
      }

      string email = info.Principal.FindFirstValue(ClaimTypes.Email);

      Member member = null;

      if (email != null)
      {
        member = await _member.GetMemberByEmailAsync(email);

        if (member != null && !member.EmailConfirmed)
        {
          ModelState.AddModelError(string.Empty, "Email not confirmed yet");
          return View("Login", loginViewModel);
        }
      }

      var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

      if (signInResult.Succeeded)
      {
        return LocalRedirect(returnUrl);
      }

      if (email != null)
      {
        if (member == null)
        {
          member = new Member
          {
            UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
            Email = info.Principal.FindFirstValue(ClaimTypes.Email),
            FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? info.Principal.FindFirstValue(ClaimTypes.Name),
            LastName = info.Principal.FindFirstValue(ClaimTypes.Surname),
          };

          var result = await _member.RegisterExternalLogin(member, info);

          if (result.Succeeded)
          {
            await SendEmailConfirmationLinkAsync(member);

            ViewBag.ConfirmTitle = "Registration Successful";
            ViewBag.ConfirmMessage = "Please check your Inbox or Spam folder for confirmation link. If you do not receive it within 5 minutes, try to login to receive new confirmation link.";
            return View("Confirmation");
          }

          return LocalRedirect(returnUrl);
        }
      }

      ViewBag.ErrorTitle = $"Response error from {info.LoginProvider}";
      ViewBag.ErrorMessage = $"Please contact support";

      return View("Error");
    }
  }
}