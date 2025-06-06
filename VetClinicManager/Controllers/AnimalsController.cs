using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public AnimalsController(
            IAnimalService animalService,
            UserManager<User> userManager
            )
        {
            _animalService = animalService;
            _userManager = userManager;
        }

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
                else if (User.IsInRole("Admin") || User.IsInRole("Receptionist") || User.IsInRole("Vet"))
                {
                    animalDtos = await _animalService.GetAnimalsForPersonnelAsync();
                    viewName = "IndexVetRec";
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
                // TODO: Loguj błąd ex
                return View("Error");
            }
        }

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

            object dto = null; // Używamy object, bo typ DTO się zmienia
            string viewName = null;

            try
            {
                if (User.IsInRole("Client"))
                {
                    // Serwis zwraca AnimalDetailsUserDto? i sprawdza własność w zapytaniu
                    dto = await _animalService.GetAnimalDetailsForOwnerAsync(id.Value, currentUserId);
                    viewName = "DetailsUser"; // Widok dla Klienta
                }
                else if (User.IsInRole("Admin") || User.IsInRole("Receptionist") || User.IsInRole("Vet"))
                {
                    // Serwis zwraca AnimalDetailsVetRecDto? i sprawdza tylko istnienie
                    dto = await _animalService.GetAnimalDetailsForPersonnelAsync(id.Value);
                    viewName = "DetailsVetRec"; // Widok dla Personelu
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
            catch (KeyNotFoundException) // Złap jeśli serwis rzuca (ale lepiej gdy serwis zwraca null)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException) // Złap jeśli serwis rzuca (ale lepiej gdy serwis zwraca null)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Wystąpił błąd podczas ładowania szczegółów zwierzęcia.");
                 // TODO: Loguj błąd ex
                return View("Error");
            }
        }

        [Authorize(Roles = "Admin,Receptionist")] // Tylko Admin i Rec mogą tworzyć
        public async Task<IActionResult> Create()
        {
            try
            {
                var clientUsers = await GetClientUsersAsync(); // TODO: Przenieś do IUserService
                ViewData["UserId"] = new SelectList(clientUsers, "Id", "Email"); // TODO: Zmień "Email" na pełne imię
                // TODO: Jeśli serwis wymaga GetCreateAnimalDtoAsync do wypełnienia formularza, wywołaj go
                // var createAnimalDto = await _animalService.GetCreateAnimalDtoAsync();
                // return View(createAnimalDto);
                return View(new CreateAnimalDto()); // Tymczasowo zwraca pusty DTO
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                 // TODO: Loguj błąd ex
                return View("Error");
            }
        }

        [Authorize(Roles = "Admin,Receptionist")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAnimalDto createAnimalDto)
        {
            if (!ModelState.IsValid)
            {
                var clientUsers = await GetClientUsersAsync(); // TODO: Przenieś do IUserService
                ViewData["UserId"] = new SelectList(clientUsers, "Id", "Email", createAnimalDto.UserId); // TODO: Zmień "Email"
                return View(createAnimalDto);
            }

            try
            {
                // TODO: Serwis tworzy zwierzę. Upewnij się, że UserId jest poprawnie obsługiwany.
                await _animalService.CreateAnimalAsync(createAnimalDto);
                // TODO: Komunikat sukcesu (TempData)
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                 // TODO: Loguj błąd ex
                 // Wróć do widoku z błędami i ponownie załaduj dane dropdowna
                var clientUsers = await GetClientUsersAsync(); // TODO: Przenieś do IUserService
                ViewData["UserId"] = new SelectList(clientUsers, "Id", "Email", createAnimalDto.UserId); // TODO: Zmień "Email"
                return View(createAnimalDto);
            }
        }

        [Authorize(Roles = "Admin,Receptionist")] // TODO: Jeśli Klient edytuje SWOJE, potrzebna inna akcja lub logika
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                // Serwis pobiera DTO do edycji i sprawdza uprawnienia. Powinien zwracać AnimalEditDto?
                var animalEditDto = await _animalService.GetAnimalForEditAsync(id.Value);
                if (animalEditDto == null)
                {
                    return NotFound();
                }

                var clientUsers = await GetClientUsersAsync(); // TODO: Przenieś do IUserService
                ViewData["UserId"] = new SelectList(clientUsers, "Id", "Email", animalEditDto.UserId); // TODO: Zmień "Email"

                return View(animalEditDto);
            }
            catch (KeyNotFoundException) // Złap jeśli serwis rzuca (ale lepiej gdy serwis zwraca null)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Wystąpił błąd podczas ładowania danych do edycji.");
                 // TODO: Loguj błąd ex
                return View("Error");
            }
        }

        [Authorize(Roles = "Admin,Receptionist")] // TODO: Jeśli Klient edytuje SWOJE, potrzebna inna akcja lub logika
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AnimalEditDto animalEditDto)
        {
            if (id != animalEditDto.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                var clientUsers = await GetClientUsersAsync(); // TODO: Przenieś do IUserService
                ViewData["UserId"] = new SelectList(clientUsers, "Id", "Email", animalEditDto.UserId); // TODO: Zmień "Email"
                return View(animalEditDto);
            }

            try
            {
                // TODO: Serwis aktualizuje zwierzę. Implementacja UpdateAnimalAsync jest placeholderem.
                // TODO: Jeśli Klient edytuje SWOJE, użyj IsAnimalOwnerAsync PRZED wywołaniem UpdateAnimalAsync.
                await _animalService.UpdateAnimalAsync(id, animalEditDto);
                // TODO: Komunikat sukcesu (TempData)
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException) // Złap jeśli serwis rzuca
            {
                return NotFound();
            }
             catch (UnauthorizedAccessException) // Złap jeśli serwis rzuca (np. User nie jest właścicielem, jeśli sprawdzanie jest w serwisie)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                 // TODO: Loguj błąd ex
                 // Wróć do widoku z błędami i ponownie załaduj dane dropdowna
                var clientUsers = await GetClientUsersAsync(); // TODO: Przenieś do IUserService
                ViewData["UserId"] = new SelectList(clientUsers, "Id", "Email", animalEditDto.UserId); // TODO: Zmień "Email"
                return View(animalEditDto);
            }
        }

        [Authorize(Roles = "Admin")] // TODO: Jeśli inne role mogą usuwać, potrzebna inna akcja lub logika
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                // Serwis pobiera DTO do usunięcia. Powinien zwracać AnimalEditDto? (lub AnimalDeleteDto?)
                var animalDto = await _animalService.GetAnimalForDeleteAsync(id.Value);
                 if (animalDto == null)
                {
                    return NotFound();
                }
                // TODO: Zwróć widok dedykowany do potwierdzenia usunięcia (np. DeleteConfirm.cshtml)
                return View(animalDto);
            }
             catch (Exception ex)
            {
                ModelState.AddModelError("", "Wystąpił błąd podczas ładowania danych do usunięcia.");
                 // TODO: Loguj błąd ex
                return View("Error");
            }
        }

        [Authorize(Roles = "Admin")] // TODO: Jeśli inne role mogą usuwać, potrzebna inna akcja lub logika
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                 // TODO: Serwis usuwa zwierzę. Implementacja DeleteAnimalAsync jest placeholderem.
                 // TODO: Jeśli Klient usuwa SWOJE, użyj IsAnimalOwnerAsync PRZED wywołaniem DeleteAnimalAsync.
                await _animalService.DeleteAnimalAsync(id);
                // TODO: Komunikat sukcesu (TempData)
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException) // Złap jeśli serwis rzuca
            {
                return NotFound();
            }
             catch (UnauthorizedAccessException) // Złap jeśli serwis rzuca (np. User nie jest właścicielem)
            {
                 return Forbid();
            }
             catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                 // TODO: Loguj błąd ex
                return RedirectToAction(nameof(Index), new { error = ex.Message });
            }
        }

        // TODO: Przenieś helpery jak GetClientUsersAsync i IsPersonnelAsync do dedykowanego serwisu użytkowników (IUserService)
        private async Task<List<User>> GetClientUsersAsync()
        {
            // Tymczasowo używa UserManager bezpośrednio
             return (await _userManager.GetUsersInRoleAsync("Client")).ToList();
        }

         private async Task<bool> IsPersonnelAsync(User user)
        {
            // Tymczasowo używa UserManager bezpośrednio
             return await _userManager.IsInRoleAsync(user, "Admin") ||
                   await _userManager.IsInRoleAsync(user, "Receptionist") ||
                   await _userManager.IsInRoleAsync(user, "Vet");
        }
         // TODO: Dodaj helper do wyświetlania błędów z ModelState w widokach, jeśli nie masz globalnej obsługi błędów
         // private IActionResult HandleError(Exception ex) { ... }
    }
}