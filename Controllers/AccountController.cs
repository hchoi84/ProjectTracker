using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MailKit.Net.Smtp;
using ProjectTracker.Models;
using ProjectTracker.ViewModels;
using ProjectTracker.Securities;

namespace ProjectTracker.Controllers
{
  [AllowAnonymous]
  public class AccountController : Controller
  {
    private readonly IMember _member;
    private readonly SignInManager<Member> _signInManager;
    private readonly UserManager<Member> _userManager;

    public AccountController(IMember member, SignInManager<Member> signInManager, UserManager<Member> userManager)
    {
      _signInManager = signInManager;
      _userManager = userManager;
      _member = member;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel loginVM, string returnUrl)
    {
      if (ModelState.IsValid)
      {
        Member member = await _userManager.FindByEmailAsync(loginVM.Email);

        if (member != null)
        {
          bool isValidPassword = await _userManager.CheckPasswordAsync(member, loginVM.Password);
          bool isEmailConfirmed = member.EmailConfirmed;

          if (isValidPassword && !isEmailConfirmed)
          {
            ModelState.AddModelError(string.Empty, "Email is not confirmed yet. Please check your inbox for email confirmation link");
            return View();
          }
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

          // TODO: Implement ResetPassword action
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
        var token = await _userManager.GenerateEmailConfirmationTokenAsync(member);
        var emailConfirmationLink = Url.Action("ConfirmEmail", "Account", new { memberId = member.Id, token = token }, Request.Scheme);

        EmailClient.SendLink(member, emailConfirmationLink, EmailType.EmailConfirmation);

        // return RedirectToAction("Login");
        ViewBag.ConfirmMessage = "Registeration successful. Please check your inbox for confirmation link";
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
        return View();
      }

      ViewBag.ErrorMessage = "Can't be confirmed";
      return View("Error");
    }
  }
}