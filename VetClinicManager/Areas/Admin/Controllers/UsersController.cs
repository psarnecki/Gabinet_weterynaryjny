using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetClinicManager.Models;
using VetClinicManager.Areas.Admin.DTOs;

namespace VetClinicManager.Areas.Admin.Controllers;

// TODO: Użyć DTOs i Mapperly do mapowania między modelami encji a DTOs.
// TODO: Po zalogowaniu na konto admina przekierować na /Admin/Users

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class UsersController : Controller
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UsersController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // GET: Admin/Users
    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.ToListAsync();
        var userListDtos = new List<UserListDto>(); 
        
        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userListDtos.Add(new UserListDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Roles = roles.ToList()
            });
        }
        
        return View(userListDtos);
    }
    
    // GET: Admin/Users/Create
    public async Task<IActionResult> Create()
    {
        var roles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();

        var model = new UserCreateDto
        {
            AvailableRoles = roles
        };

        return View(model);
    }
    
    // POST: Admin/Users/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserCreateDto model)
    {
        model.AvailableRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new User
        {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Specialization = model.Specialization,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            if (model.SelectedRoles != null && model.SelectedRoles.Any())
            {
                 var validRoles = await _roleManager.Roles
                                                    .Where(r => model.SelectedRoles.Contains(r.Name))
                                                    .Select(r => r.Name)
                                                    .ToListAsync();

                await _userManager.AddToRolesAsync(user, validRoles);
            }

            return RedirectToAction(nameof(Index));
        }
        
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    // GET: Admin/Users/Edit/5
    public async Task<IActionResult> Edit(string? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var allRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();

        var userRoles = await _userManager.GetRolesAsync(user);

        var model = new UserEditDto // TODO: Tutaj możesz użyć Mapperly do mapowania prostych pól User -> UserEditDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Specialization = user.Specialization,
            AvailableRoles = allRoles,
            SelectedRoles = userRoles.ToList()
        };
        
        return View(model);
    }
    
    // POST: Admin/Users/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, UserEditDto model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }
        
        model.AvailableRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();
        
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByIdAsync(model.Id);
        if (user == null)
        {
            return NotFound();
        }

        // TODO: Użyć Mapperly do mapowania
        user.FirstName = model.FirstName;
        user.LastName = model.LastName;
        user.Specialization = model.Specialization;
        if (user.Email != model.Email)
        {
            var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
            
            if (!setEmailResult.Succeeded)
            {
                foreach (var error in setEmailResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                
                return View(model);
            }
            
            var setUserNameResult = await _userManager.SetUserNameAsync(user, model.Email);
            if (!setUserNameResult.Succeeded)
            {
                foreach (var error in setUserNameResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                
                return View(model);
            }
        }

        var updateResult = await _userManager.UpdateAsync(user);

        if (!updateResult.Succeeded)
        {
            foreach (var error in updateResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            
            return View(model);
        }

        var currentRoles = await _userManager.GetRolesAsync(user);
        var rolesToAdd = model.SelectedRoles.Except(currentRoles).ToList();
        var rolesToRemove = currentRoles.Except(model.SelectedRoles).ToList();

        if (rolesToAdd.Any())
        {
            await _userManager.AddToRolesAsync(user, rolesToAdd);
        }

        if (rolesToRemove.Any())
        {
            await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
        }

        // TODO: Można dodać logikę zmiany hasła ( trzeba dodać pola NewPassword/ConfirmNewPassword do DTO)

        return RedirectToAction(nameof(Index));
    }
    
    // GET: Admin/Users/Delete/5
    public async Task<IActionResult> Delete(string? id) // Parametr id z routingu
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }
        
        var model = new UserDeleteDto // TODO: Można użyć Mapperly do mapowania User -> UserDeleteDto
        {
            Id = user.Id,
            FirstName = user.FirstName ?? string.Empty,
            LastName = user.LastName ?? string.Empty,
            Email = user.Email ?? string.Empty
        };

        return View(model);
    }
    
    // POST: Admin/Users/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
        {
            return RedirectToAction(nameof(Index));
        }

        var result = await _userManager.DeleteAsync(user);

        if (result.Succeeded)
        {
            return RedirectToAction(nameof(Index));
        }
        
        // Dla niestandardowej logiki walidacji przed usunięciem (sprawdzanie powiązanych danych) będzie trzeba zmodyfikować
        string errorMessage = "Nie udało się usunąć użytkownika: " + string.Join(", ", result.Errors.Select(e => e.Description));
        TempData["ErrorMessage"] = errorMessage;
        return RedirectToAction(nameof(Index));
    }
}