using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VetClinicManager.Data;
using VetClinicManager.DTOs.AnimalDTOs;
using VetClinicManager.Models;
using VetClinicManager.Services;

namespace VetClinicManager.Controllers
{
    [Authorize]
    public class AnimalsController : Controller
    {
        private readonly IAnimalService _animalService;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        public AnimalsController(
            IAnimalService animalService,
            UserManager<User> userManager,
            ApplicationDbContext context)
        {
            _animalService = animalService;
            _userManager = userManager;
            _context = context;
        }

        [Authorize(Roles = "Admin,Receptionist,Vet")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var animalDtos = await _animalService.GetAnimalsForPersonnelAsync();
                return View(animalDtos);
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        [Authorize(Roles = "Admin,Receptionist,Vet,Client")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var currentUserId = currentUser?.Id;

                if (await IsPersonnelAsync(currentUser))
                {
                    var dto = await _animalService.GetAnimalDetailsForPersonnelAsync(id.Value);
                    return View(dto);
                }
                else if (await _animalService.IsAnimalOwnerAsync(id.Value, currentUserId))
                {
                    var dto = await _animalService.GetAnimalDetailsForOwnerAsync(id.Value, currentUserId);
                    return View(dto);
                }

                return Forbid();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Create()
        {
            var clientUsers = await GetClientUsersAsync();
            ViewData["UserId"] = new SelectList(clientUsers, "Id", "Email");
            return View(new CreateAnimalDto());
        }

        [Authorize(Roles = "Admin,Receptionist")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAnimalDto createAnimalDto)
        {
            if (!ModelState.IsValid)
            {
                var clientUsers = await GetClientUsersAsync();
                ViewData["UserId"] = new SelectList(clientUsers, "Id", "Email", createAnimalDto.UserId);
                return View(createAnimalDto);
            }

            try
            {
                await _animalService.CreateAnimalAsync(createAnimalDto);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                var clientUsers = await GetClientUsersAsync();
                ViewData["UserId"] = new SelectList(clientUsers, "Id", "Email", createAnimalDto.UserId);
                return View(createAnimalDto);
            }
        }

        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var updateAnimalDto = await _animalService.GetAnimalForEditAsync(id.Value);
                var clientUsers = await GetClientUsersAsync();
                ViewData["UserId"] = new SelectList(clientUsers, "Id", "Email", updateAnimalDto.UserId);
                return View(updateAnimalDto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        [Authorize(Roles = "Admin,Receptionist")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AnimalEditDto animalEditDto)
        {
            if (id != animalEditDto.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                var clientUsers = await GetClientUsersAsync();
                ViewData["UserId"] = new SelectList(clientUsers, "Id", "Email", animalEditDto.UserId);
                return View(animalEditDto);
            }

            try
            {
                await _animalService.UpdateAnimalAsync(id, animalEditDto);
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                var clientUsers = await GetClientUsersAsync();
                ViewData["UserId"] = new SelectList(clientUsers, "Id", "Email", animalEditDto.UserId);
                return View(animalEditDto);
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var animalDto = await _animalService.GetAnimalForDeleteAsync(id.Value);
                return View(animalDto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _animalService.DeleteAnimalAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                return HandleError(ex);
            }
        }

        private async Task<List<User>> GetClientUsersAsync()
        {
            return (await _userManager.GetUsersInRoleAsync("Client")).ToList();
        }

        private async Task<bool> IsPersonnelAsync(User user)
        {
            return await _userManager.IsInRoleAsync(user, "Admin") ||
                   await _userManager.IsInRoleAsync(user, "Receptionist") ||
                   await _userManager.IsInRoleAsync(user, "Vet");
        }

        private IActionResult HandleError(Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View("Error");
        }
    }
}