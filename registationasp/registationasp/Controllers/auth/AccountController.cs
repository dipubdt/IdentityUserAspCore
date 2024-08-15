using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using registationasp.Models;
using registationasp.Viewmodel;

namespace registationasp.Controllers.auth;

public class AccountController(SignInManager<AppUser> inManager, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager) : Controller
{

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login()
    {
        return View();
    }


    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginVm loginVm)
    {
        if (ModelState.IsValid)
        {
            var result =
                await inManager.PasswordSignInAsync(loginVm.UserName!, loginVm.Password!, loginVm.RememberMe,
                    false);
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "Invalid login attempt.");
            return View(loginVm);
        }
        return View(loginVm);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        return View();
    }


    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterVm registrar)
    {
        if (ModelState.IsValid)
        {
            AppUser appUser = new()
            {
                UserName = registrar.Email,
                Name = registrar.Name,
                Email = registrar.Email,
                Address = registrar.Address
            };

            var result = await userManager.CreateAsync(appUser, registrar.Password!);

            if (result.Succeeded)
            {
                // Check if the role exists, and create it if not
                var roleExists = await roleManager.RoleExistsAsync("User");
                if (!roleExists)
                {
                    await roleManager.CreateAsync(new IdentityRole("User"));
                }

                // Assign the "User" role to the registered user
                await userManager.AddToRoleAsync(appUser, "User");

                await inManager.SignInAsync(appUser, false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        return View();
    }

    public async Task<IActionResult> Logout()
    {
        await inManager.SignOutAsync();
        return RedirectToAction("Login", "Account");
    }


    [HttpGet]
    [AllowAnonymous]
    public IActionResult AddRole()
    {
        return View();
    }



    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddRole(RoleVm model)
    {
        if (ModelState.IsValid)
        {
           
            // Check if the role already exists
            var roleExists = await roleManager.RoleExistsAsync(model.role);
            if (roleExists)
            {
                ModelState.AddModelError("", "Role already exists.");
                return View(model);
            }

            // Create the role
            var result = await roleManager.CreateAsync(new IdentityRole(model.role));
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            // Add errors to the ModelState
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        
        return View(model);
    }



    [HttpGet]

    [AllowAnonymous]
    // Restrict this action to users with the "Admin" role
    public async Task<IActionResult> AssignRole()
    {
        var viewModel = new AssignRoleVm
        {
            Roles = await roleManager.Roles.Select(role => role.Name).ToListAsync(),
            Users = await userManager.Users.Select(user => user.UserName).ToListAsync()
        };

        return View(viewModel);
    }





    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignRole(AssignRoleVm model)
    {
        var user = await userManager.FindByNameAsync(model.SelectedUser);

        var currentRoles = await userManager.GetRolesAsync(user);
        var result = await userManager.RemoveFromRolesAsync(user, currentRoles);


        var results = await userManager.AddToRoleAsync(user, model.SelectedRole);
        if (results.Succeeded)
        {
            // Successfully assigned role
            return RedirectToAction("Index", "Home");
        }

        return View(model);


    }



    }
