using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using VetClinicManager.DTOs.Animals;
using VetClinicManager.Interfaces;
using VetClinicManager.Models;
using VetClinicManager.Models.Enums;
using VetClinicManager.Services;

namespace VetClinicManager.Controllers
{
    [Authorize]
    public class AnimalsController : Controller
    {
        private readonly IAnimalService _animalService;
        private readonly UserManager<User> _userManager;
        private readonly IFileService _fileService;

        public AnimalsController(
            IAnimalService animalService,
            UserManager<User> userManager,
            IFileService fileService)
        {
            _animalService = animalService;
            _userManager = userManager;
            _fileService = fileService;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Receptionist,Vet,Client")]
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var currentUserId = currentUser?.Id;
            if (string.IsNullOrEmpty(currentUserId)) return Unauthorized();
            
            object animalDtos;
            string viewName;

            if (User.IsInRole("Client"))
            {
                animalDtos = await _animalService.GetAnimalsForOwnerAsync(currentUserId);
                viewName = "IndexUser";
            }
            else
            {
                animalDtos = await _animalService.GetAnimalsForPersonnelAsync();
                viewName = User.IsInRole("Vet") ? "IndexVet" : "IndexRec";
            }
            return View(viewName, animalDtos);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Receptionist,Vet,Client")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();

            object dto;
            string viewName;

            if (User.IsInRole("Client"))
            {
                dto = await _animalService.GetAnimalDetailsForOwnerAsync(id.Value, currentUser.Id);
                viewName = "DetailsUser";
            }
            else
            {
                dto = await _animalService.GetAnimalDetailsForPersonnelAsync(id.Value);
                viewName = "DetailsVetRec";
            }
            
            if (dto == null) return NotFound();
            return View(viewName, dto);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Create()
        {
            ViewBag.GenderOptions = GetEnumSelectList<Gender>();
            var clientUsers = await GetClientUsersForSelectListAsync();
            // Używamy "FullName" jako tekstu do wyświetlenia
            ViewData["UserId"] = new SelectList(clientUsers, "Id", "FullName"); 
            return View(new CreateAnimalDto());
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Receptionist")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAnimalDto createAnimalDto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.GenderOptions = GetEnumSelectList<Gender>();
                var clientUsers = await GetClientUsersForSelectListAsync();
                ViewData["UserId"] = new SelectList(clientUsers, "Id", "FullName", createAnimalDto.UserId);
                return View(createAnimalDto);
            }
            
            try
            {
                if (createAnimalDto.ImageFile != null)
                {
                    createAnimalDto.ImageUrl = await _fileService.SaveFileAsync(createAnimalDto.ImageFile, "uploads/animals");
                }
                
                await _animalService.CreateAnimalAsync(createAnimalDto);
                TempData["SuccessMessage"] = "Pomyślnie dodano nowe zwierzę.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Wystąpił błąd: {ex.Message}");
                var clientUsers = await GetClientUsersForSelectListAsync();
                ViewData["UserId"] = new SelectList(clientUsers, "Id", "FullName", createAnimalDto.UserId);
                return View(createAnimalDto);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Receptionist,Vet")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            
            var animalEditDto = await _animalService.GetAnimalForEditAsync(id.Value);
            if (animalEditDto == null) return NotFound();
                
            ViewBag.GenderOptions = GetEnumSelectList<Gender>();
            var clientUsers = await GetClientUsersForSelectListAsync();
            ViewData["UserId"] = new SelectList(clientUsers, "Id", "FullName", animalEditDto.UserId); 

            return View(animalEditDto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Receptionist,Vet")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AnimalEditDto animalEditDto)
        {
            if (id != animalEditDto.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.GenderOptions = GetEnumSelectList<Gender>();
                var clientUsers = await GetClientUsersForSelectListAsync();
                ViewData["UserId"] = new SelectList(clientUsers, "Id", "FullName", animalEditDto.UserId);
                return View(animalEditDto);
            }

            try
            {
                var existingAnimal = await _animalService.GetAnimalForEditAsync(id);
                var oldImageUrl = existingAnimal?.ImageUrl;

                if (animalEditDto.ImageFile != null)
                {
                    _fileService.DeleteFile(oldImageUrl);
                    animalEditDto.ImageUrl = await _fileService.SaveFileAsync(animalEditDto.ImageFile, "uploads/animals");
                }
                else
                {
                    animalEditDto.ImageUrl = oldImageUrl;
                }

                await _animalService.UpdateAnimalAsync(id, animalEditDto);
                TempData["SuccessMessage"] = "Dane zwierzęcia zostały zaktualizowane.";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Wystąpił błąd podczas zapisywania zmian.");
                var clientUsers = await GetClientUsersForSelectListAsync();
                ViewData["UserId"] = new SelectList(clientUsers, "Id", "FullName", animalEditDto.UserId);
                return View(animalEditDto);
            }
        }
        
        [HttpGet]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var animalDto = await _animalService.GetAnimalForDeleteAsync(id.Value);
            if (animalDto == null) return NotFound();
            return View(animalDto);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin,Receptionist")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _animalService.DeleteAnimalAsync(id);
            return RedirectToAction(nameof(Index));
        }
        
        [HttpGet]
        [Authorize(Roles = "Admin,Receptionist,Vet,Client")]
        public async Task<IActionResult> RedirectToHealthRecord(int? id)
        {
            if (id == null) return NotFound();
            var healthRecordId = await _animalService.GetHealthRecordIdByAnimalIdAsync(id.Value);
            if (healthRecordId == null)
            {
                if (User.IsInRole("Client"))
                {
                    TempData["InfoMessage"] = "Karta zdrowia dla tego zwierzęcia nie została jeszcze utworzona."; 
                    return RedirectToAction("Index", "Animals");
                }
                else
                {
                    TempData["InfoMessage"] = $"Brak karty zdrowia dla zwierzęcia ID {id.Value}. Możesz ją utworzyć.";
                    return RedirectToAction("Create", "HealthRecords", new { animalId = id.Value });
                }
            }
            return RedirectToAction("Details", "HealthRecords", new { id = healthRecordId.Value });
        }
        
        private List<SelectListItem> GetEnumSelectList<TEnum>() where TEnum : Enum
        {
            return Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Select(enumValue =>
            {
                var fieldInfo = typeof(TEnum).GetField(enumValue.ToString());
                var displayAttribute = fieldInfo?.GetCustomAttributes(typeof(DisplayAttribute), false).Cast<DisplayAttribute>().FirstOrDefault();
                return new SelectListItem
                {
                    Value = enumValue.ToString(),
                    Text = displayAttribute?.Name ?? enumValue.ToString()
                };
            }).ToList();
        }

        private async Task<IEnumerable<object>> GetClientUsersForSelectListAsync()
        {
             var clients = await _userManager.GetUsersInRoleAsync("Client");
             return clients.OrderBy(c => c.LastName).Select(c => new {
                 c.Id,
                 FullName = $"{c.FirstName} {c.LastName}"
             }).ToList();
        }
    }
}