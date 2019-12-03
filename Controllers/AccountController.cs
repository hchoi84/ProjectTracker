using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectTracker.Models;
using ProjectTracker.ViewModels;

namespace ProjectTracker.Controllers
{
  [AllowAnonymous]
  public class AccountController : Controller
  {
    private readonly IMember _member;
    private readonly SignInManager<Member> _signInManager;
    public AccountController(IMember member, SignInManager<Member> signInManager)
    {
      _signInManager = signInManager;
      _member = member;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel loginVM, string returnUrl)
    {
      if (ModelState.IsValid)
      {
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
        return RedirectToAction("Login");
      }

      foreach (var error in result.Errors)
      {
        ModelState.AddModelError(string.Empty, error.Description);
      }

      return View();
    }
  }
}