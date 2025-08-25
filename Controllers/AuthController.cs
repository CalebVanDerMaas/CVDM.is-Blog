
using CVDMBlog.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace CVDMBlog.Controllers;

public class AuthController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private SignInManager<IdentityUser> _signInManager;

    public AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }
    
    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }
    
    [HttpGet]
    public IActionResult Login(string returnUrl = null, string content = null)
    {
        var viewModel = new LoginViewModel
        {
            ReturnUrl = returnUrl,
            Content = content
        };
        return View(viewModel);
    }
    
    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {
        // Remove ReturnUrl and Content from ModelState if they're null or empty
        if (string.IsNullOrEmpty(vm.ReturnUrl))
        {
            ModelState.Remove(nameof(vm.ReturnUrl));
        }
        if (string.IsNullOrEmpty(vm.Content))
        {
            ModelState.Remove(nameof(vm.Content));
        }

        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(vm.UserName, vm.Password, false, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(vm.ReturnUrl) && Url.IsLocalUrl(vm.ReturnUrl))
                {
                    return Redirect(vm.ReturnUrl + (!string.IsNullOrEmpty(vm.Content) ? $"?content={Uri.EscapeDataString(vm.Content)}" : ""));
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
        }

        // If we got this far, something failed, redisplay form
        return View(vm);
    }
    
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
    
    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            // Create a new user with the provided email and username
            var user = new IdentityUser { UserName = model.Username, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Assign the user to a role, e.g., "User" role
                await _userManager.AddToRoleAsync(user, "User");

                // Sign in the user and redirect to home page
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            // Add errors to ModelState to show them in the view
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        return View(model);
    }


}

