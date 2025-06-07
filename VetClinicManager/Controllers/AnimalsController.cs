using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using VetClinicManager.DTOs.Animals;
using VetClinicManager.Interfaces; // Zakładam, że IFileService jest tutaj
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

        // --- AKCJE GET (Index, Details, Create, Edit, Delete) - BEZ ZMIAN ---
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
            var clientUsers = await GetClientUsersAsync();
            ViewData["UserId"] = new SelectList(clientUsers, "Id", "Email");
            return View(new CreateAnimalDto());
        }

        // --- POCZĄTEK ZMIAN: AKCJA POST CREATE ---
        [HttpPost]
        [Authorize(Roles = "Admin,Receptionist")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAnimalDto createAnimalDto)
        {
            if (!ModelState.IsValid)
            {
                // Ponownie załaduj dane dla list rozwijanych
                ViewBag.GenderOptions = GetEnumSelectList<Gender>();
                var clientUsers = await GetClientUsersAsync();
                ViewData["UserId"] = new SelectList(clientUsers, "Id", "Email", createAnimalDto.UserId);
                return View(createAnimalDto);
            }
            
            try
            {
                // Jeśli użytkownik przesłał plik, zapisz go i ustaw ImageUrl w DTO
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
                var clientUsers = await GetClientUsersAsync();
                ViewData["UserId"] = new SelectList(clientUsers, "Id", "Email", createAnimalDto.UserId);
                return View(createAnimalDto);
            }
        }
        // --- KONIEC ZMIAN: AKCJA POST CREATE ---

        [HttpGet]
        [Authorize(Roles = "Admin,Receptionist,Vet")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            
            var animalEditDto = await _animalService.GetAnimalForEditAsync(id.Value);
            if (animalEditDto == null) return NotFound();
                
            ViewBag.GenderOptions = GetEnumSelectList<Gender>();
            var clientUsers = await GetClientUsersAsync();
            ViewData["UserId"] = new SelectList(clientUsers, "Id", "Email", animalEditDto.UserId); 

            return View(animalEditDto);
        }

 [HttpPost]
[Authorize(Roles = "Admin,Receptionist,Vet")]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(int id, AnimalEditDto animalEditDto)
{
    if (id != animalEditDto.Id)
    {
        return NotFound();
    }

    var existingAnimal = await _animalService.GetAnimalForEditAsync(id);
    if (existingAnimal == null)
    {
        return NotFound();
    }
    var oldImageUrl = existingAnimal.ImageUrl;

    if (!ModelState.IsValid)
    {
        ViewBag.GenderOptions = GetEnumSelectList<Gender>();
        var clientUsers = await GetClientUsersAsync();
        ViewData["UserId"] = new SelectList(clientUsers, "Id", "Email", animalEditDto.UserId);
        return View(animalEditDto);
    }

    try
    {
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
        ViewBag.GenderOptions = GetEnumSelectList<Gender>();
        var clientUsers = await GetClientUsersAsync();
        ViewData["UserId"] = new SelectList(clientUsers, "Id", "Email", animalEditDto.UserId);
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
            var selectListItems = new List<SelectListItem>();
            var enumValues = Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
            foreach (var enumValue in enumValues)
            {
                var fieldInfo = typeof(TEnum).GetField(enumValue.ToString());
                var displayAttribute = fieldInfo.GetCustomAttributes(typeof(DisplayAttribute), false).Cast<DisplayAttribute>().FirstOrDefault();
                selectListItems.Add(new SelectListItem
                {
                    Value = enumValue.ToString(),
                    Text = displayAttribute?.Name ?? enumValue.ToString()
                });
            }
            return selectListItems;
        }

        private async Task<List<User>> GetClientUsersAsync()
        {
             return (await _userManager.GetUsersInRoleAsync("Client")).ToList();
        }
    }
}