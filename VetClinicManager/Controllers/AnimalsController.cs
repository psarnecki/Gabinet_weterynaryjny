using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VetClinicManager.DTOs.Animals;
using VetClinicManager.Models;
using VetClinicManager.Models.Enums;
using VetClinicManager.Services;

namespace VetClinicManager.Controllers
{
    [Authorize]
    public class AnimalsController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IAnimalService _animalService;

        public AnimalsController(UserManager<User> userManager, IAnimalService animalService)
        {
            _animalService = animalService;
            _userManager = userManager;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Receptionist,Vet,Client")]
        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Client"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null) return Unauthorized();
                
                var animals = await _animalService.GetAnimalsForOwnerAsync(currentUser.Id);
                return View("IndexUser", animals);
            }
            else
            {
                var animals = await _animalService.GetAnimalsForPersonnelAsync();
                var viewName = User.IsInRole("Vet") ? "IndexVet" : "IndexRec";
                
                return View(viewName, animals);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Receptionist,Vet,Client")]
        public async Task<IActionResult> Details(int id)
        {
            object? dto;
            string viewName;

            if (User.IsInRole("Client"))
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null) return Unauthorized();

                dto = await _animalService.GetAnimalDetailsForOwnerAsync(id, currentUser.Id);
                viewName = "DetailsUser";
            }
            else
            {
                dto = await _animalService.GetAnimalDetailsForPersonnelAsync(id);
                viewName = "DetailsVetRec";
            }
            
            if (dto == null) return NotFound();
            
            return View(viewName, dto);
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Create()
        {
            await PrepareViewDataForForm();
            return View(new CreateAnimalDto());
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Receptionist")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAnimalDto createAnimalDto)
        {
            if (!ModelState.IsValid)
            {
                await PrepareViewDataForForm(createAnimalDto.UserId);
                return View(createAnimalDto);
            }
            
            try
            {
                await _animalService.CreateAnimalAsync(createAnimalDto);
                TempData["SuccessMessage"] = "Pomyślnie dodano nowe zwierzę.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Wystąpił nieoczekiwany błąd podczas dodawania zwierzęcia.");
                await PrepareViewDataForForm(createAnimalDto.UserId);
                return View(createAnimalDto);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Receptionist,Vet")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var animalEditDto = await _animalService.GetAnimalForEditAsync(id.Value);
            if (animalEditDto == null) return NotFound();
                
            await PrepareViewDataForForm(animalEditDto.UserId);

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

            if (!ModelState.IsValid)
            {
                await PrepareViewDataForForm(animalEditDto.UserId);
                return View(animalEditDto);
            }

            try
            {
                await _animalService.UpdateAnimalAsync(id, animalEditDto);
                TempData["SuccessMessage"] = "Dane zwierzęcia zostały zaktualizowane.";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Wystąpił nieoczekiwany błąd podczas zapisywania zmian.");
                await PrepareViewDataForForm(animalEditDto.UserId);
                return View(animalEditDto);
            }
        }
        
        [HttpGet]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var animalDto = await _animalService.GetAnimalForDeleteAsync(id.Value);
            if (animalDto == null) {return NotFound();}
            
            return View(animalDto);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin,Receptionist")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _animalService.DeleteAnimalAsync(id);
            TempData["SuccessMessage"] = "Zwierzę zostało usunięte.";
            
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
                    TempData["InfoMessage"] = $"Brak karty zdrowia dla tego zwierzęcia. Możesz ją utworzyć.";
                    return RedirectToAction("Create", "HealthRecords", new { animalId = id.Value });
                }
            }
            return RedirectToAction("Details", "HealthRecords", new { id = healthRecordId.Value });
        }
        
        private async Task PrepareViewDataForForm(string? selectedUserId = null)
        {
            ViewBag.GenderOptions = _animalService.GetEnumSelectList<Gender>();
            var clientUsers = await _animalService.GetClientUsersForSelectListAsync();
            ViewData["UserId"] = new SelectList(clientUsers, "Value", "Text", selectedUserId);
        }
    }
}