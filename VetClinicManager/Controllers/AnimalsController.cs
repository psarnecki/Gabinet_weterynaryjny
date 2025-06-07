using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using VetClinicManager.DTOs.Animals;
using VetClinicManager.Models;
using VetClinicManager.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using VetClinicManager.Models.Enums;


namespace VetClinicManager.Controllers
{
    [Authorize]
    public class AnimalsController : Controller
    {
        private readonly IAnimalService _animalService;
        private readonly UserManager<User> _userManager;
        // TODO: Consider injecting IUserService here if helpers are moved

        public AnimalsController(
            IAnimalService animalService,
            UserManager<User> userManager
            // TODO: Inject IUserService here
            )
        {
            _animalService = animalService;
            _userManager = userManager;
            // TODO: Assign IUserService here
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Receptionist,Vet,Client")]
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var currentUserId = currentUser?.Id;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized();
            }

            object animalDtos = null;
            string viewName = null;

            try
            {
                if (User.IsInRole("Client"))
                {
                    animalDtos = await _animalService.GetAnimalsForOwnerAsync(currentUserId);
                    viewName = "IndexUser";
                }
                else if (User.IsInRole("Admin") || User.IsInRole("Receptionist"))
                {
                    animalDtos = await _animalService.GetAnimalsForPersonnelAsync();
                    viewName = "IndexRec";
                }
                else if (User.IsInRole("Admin") || User.IsInRole("Vet"))
                {
                    animalDtos = await _animalService.GetAnimalsForPersonnelAsync();
                    viewName = "IndexVet";
                }
                else
                {
                    return Forbid();
                }

                return View(viewName, animalDtos);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Wystąpił błąd podczas ładowania listy zwierząt.");
                // TODO: Log error ex
                return View("Index", new List<AnimalListUserDto>());
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Receptionist,Vet,Client")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            var currentUserId = currentUser?.Id;

             if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized();
            }

            object dto = null;
            string viewName = null;

            try
            {
                if (User.IsInRole("Client"))
                {
                    dto = await _animalService.GetAnimalDetailsForOwnerAsync(id.Value, currentUserId);
                    viewName = "DetailsUser";
                }
                else if (User.IsInRole("Admin") || User.IsInRole("Receptionist") || User.IsInRole("Vet"))
                {
                    dto = await _animalService.GetAnimalDetailsForPersonnelAsync(id.Value);
                    viewName = "DetailsVetRec";
                }
                else
                {
                    return Forbid();
                }

                 if (dto == null)
                {
                    return NotFound();
                }

                return View(viewName, dto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Wystąpił błąd podczas ładowania szczegółów zwierzęcia.");
                 // TODO: Log error ex
                return View("Error");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Create()
        {
            try
            {
                ViewBag.GenderOptions = GetEnumSelectList<Gender>();
                var clientUsers = await GetClientUsersAsync();
                ViewData["UserId"] = new SelectList(clientUsers, "Id", "Email"); // TODO: Change "Email" to full name
                // TODO: If GetCreateAnimalDtoAsync is needed to pre-fill the form, call it
                // var createAnimalDto = await _animalService.GetCreateAnimalDtoAsync();
                // return View(createAnimalDto);
                return View(new CreateAnimalDto());
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                 // TODO: Log error ex
                 return View("Error");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Receptionist")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAnimalDto createAnimalDto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.GenderOptions = GetEnumSelectList<Gender>();
                var clientUsers = await GetClientUsersAsync();
                ViewData["UserId"] = new SelectList(clientUsers, "Id", "Email", createAnimalDto.UserId); // TODO: Change "Email"
                return View(createAnimalDto);
            }

            try
            {
                // TODO: Service CreateAnimalAsync must handle setting the UserId from the DTO
                await _animalService.CreateAnimalAsync(createAnimalDto);
                // TODO: Success message (TempData)
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                 // TODO: Log error ex
                 var clientUsers = await GetClientUsersAsync();
                ViewData["UserId"] = new SelectList(clientUsers, "Id", "Email", createAnimalDto.UserId); // TODO: Change "Email"
                return View(createAnimalDto);
            }
        }

        // TODO: Client needs SEPARATE Edit action for THEIR animal (e.g., EditOwnerAnimal)
        // or this action must contain logic to differentiate roles and check ownership
        [HttpGet]
        [Authorize(Roles = "Admin,Receptionist,Vet")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var animalEditDto = await _animalService.GetAnimalForEditAsync(id.Value);
                if (animalEditDto == null)
                {
                    return NotFound();
                }
                
                ViewBag.GenderOptions = GetEnumSelectList<Gender>();
                var clientUsers = await GetClientUsersAsync();
                ViewData["UserId"] = new SelectList(clientUsers, "Id", "Email", animalEditDto.UserId); // TODO: Change "Email"

                return View(animalEditDto);
            }
             catch (Exception ex)
            {
                ModelState.AddModelError("", "Wystąpił błąd podczas ładowania danych do edycji.");
                 // TODO: Log error ex
                return View("Error");
            }
        }

        // TODO: Client needs SEPARATE POST Edit action for THEIR animal
        [HttpPost]
        [Authorize(Roles = "Admin,Receptionist,Vet")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AnimalEditDto animalEditDto)
        {
            if (id != animalEditDto.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.GenderOptions = GetEnumSelectList<Gender>();
                var clientUsers = await GetClientUsersAsync();
                ViewData["UserId"] = new SelectList(clientUsers, "Id", "Email", animalEditDto.UserId); // TODO: Change "Email"
                return View(animalEditDto);
            }

            try
            {
                await _animalService.UpdateAnimalAsync(id, animalEditDto);
                // TODO: Success message (TempData)
                return RedirectToAction(nameof(Index));
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
                ModelState.AddModelError("", ex.Message);
                 // TODO: Log error ex
                ViewBag.GenderOptions = GetEnumSelectList<Gender>();
                var clientUsers = await GetClientUsersAsync();
                ViewData["UserId"] = new SelectList(clientUsers, "Id", "Email", animalEditDto.UserId); // TODO: Change "Email"
                return View(animalEditDto);
            }
        }

        // TODO: Client CANNOT delete animals
        [HttpGet]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var animalDto = await _animalService.GetAnimalForDeleteAsync(id.Value);
                if (animalDto == null)
                {
                    return NotFound();
                }
                // TODO: Return a dedicated confirmation view (e.g., DeleteConfirm.cshtml)
                return View(animalDto);
            }
             catch (Exception ex)
            {
                ModelState.AddModelError("", "Wystąpił błąd podczas ładowania danych do usunięcia.");
                 // TODO: Log error ex
                return View("Error");
            }
        }

        // TODO: Client CANNOT delete animals
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin,Receptionist")] // Only Admin can confirm deletion
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _animalService.DeleteAnimalAsync(id);
                // TODO: Success message (TempData)
                return RedirectToAction(nameof(Index));
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
                ModelState.AddModelError("", ex.Message);
                 // TODO: Log error ex
                return RedirectToAction(nameof(Index), new { error = ex.Message });
            }
        }
        
        // GET: /Animals/RedirectToHealthRecord/5
        [HttpGet]
        [Authorize(Roles = "Admin,Receptionist,Vet,Client")]
        public async Task<IActionResult> RedirectToHealthRecord(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // TODO: Opcjonalnie dodaj sprawdzanie własności dla Klienta
            // var currentUser = await _userManager.GetUserAsync(User);
            // var currentUserId = currentUser?.Id;
            // if (User.IsInRole("Client") && !(await _animalService.IsAnimalOwnerAsync(id.Value, currentUserId)))
            // {
            //      return Forbid(); // Klient nie jest właścicielem
            // }

            var healthRecordId = await _animalService.GetHealthRecordIdByAnimalIdAsync(id.Value);

            if (healthRecordId == null)
            {

                // SPRAWDŹ ROLĘ
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
        
        // Helper method to get SelectListItem for enums
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

        // TODO: Move helper GetClientUsersAsync to a dedicated user service (IUserService)
        private async Task<List<User>> GetClientUsersAsync()
        {
             return (await _userManager.GetUsersInRoleAsync("Client")).ToList();
        }

         // TODO: Add helper for displaying ModelState errors in views if no global error handling

    }
}