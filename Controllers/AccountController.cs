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
    private readonly UserManager<Member> _userManager;
    private readonly SignInManager<Member> _signInManager;
    public AccountController(UserManager<Member> userManager, SignInManager<Member> signInManager)
    {
      _signInManager = signInManager;
      _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel loginVM)
    {
      if (ModelState.IsValid)
      {
        var result = await _signInManager.PasswordSignInAsync(loginVM.Email, loginVM.Password, loginVM.RememberMe, false);

        if (result.Succeeded)
          return RedirectToAction("Index", "Home");
      }

      ModelState.AddModelError(string.Empty, "Invalid login attempt");
      return View(loginVM);
    }
  }
}