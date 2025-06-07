using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VetClinicManager.DTOs.Visits;
using VetClinicManager.Models;
using VetClinicManager.Models.Enums;
using VetClinicManager.Services;

namespace VetClinicManager.Controllers
{
    [Authorize]
    public class VisitsController : Controller
    {
        private readonly IVisitService _visitService;
        private readonly UserManager<User> _userManager;

        public VisitsController(
            IVisitService visitService,
            UserManager<User> userManager)
        {
            _visitService = visitService;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin,Receptionist,Vet,Client")]
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var currentUserId = currentUser?.Id;

            if (currentUserId == null) return Unauthorized();

            object visitDtos = null;
            string viewName = null;

            try
            {
                if (User.IsInRole("Client"))
                {
                    visitDtos = await _visitService.GetVisitsForOwnerAnimalsAsync(currentUserId);
                    viewName = "IndexUser";
                }
                else if (User.IsInRole("Admin") || User.IsInRole("Receptionist"))
                {
                    visitDtos = await _visitService.GetVisitsForReceptionistAsync();
                    viewName = "IndexReceptionist";
                }
                else if (User.IsInRole("Vet"))
                {
                    visitDtos = await _visitService.GetVisitsForVetAsync(currentUserId);
                    viewName = "IndexVet";
                }
                else
                {
                    return Forbid();
                }

                return View(viewName, visitDtos);
            }
            catch (Exception ex)
            {
                // TODO: Loguj błąd ex
                ModelState.AddModelError("", "Wystąpił błąd podczas ładowania listy wizyt.");
                return View("Error");
            }
        }

        [Authorize(Roles = "Admin,Receptionist,Vet,Client")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            var currentUserId = currentUser?.Id;

            if (currentUserId == null) return Unauthorized();

            object dto = null;
            string viewName = null;

            try
            {
                if (User.IsInRole("Receptionist") || User.IsInRole("Admin"))
                {
                    dto = await _visitService.GetVisitDetailsForReceptionistAsync(id.Value);
                    viewName = "DetailsReceptionist";
                }
                else if (User.IsInRole("Vet"))
                {
                    dto = await _visitService.GetVisitDetailsForVetAsync(id.Value, currentUserId);
                    viewName = "DetailsVet";
                }
                else if (User.IsInRole("Client"))
                {
                    dto = await _visitService.GetVisitDetailsForUserAsync(id.Value, currentUserId);
                    viewName = "DetailsUser";
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
                // TODO: Loguj błąd ex
                ModelState.AddModelError("", "Wystąpił błąd podczas ładowania szczegółów wizyty.");
                return View("Error");
            }
        }

        // W pliku VisitsController.cs

        [Authorize(Roles = "Receptionist,Vet")]
        public async Task<IActionResult> Create()
        {
            // 1. Przygotuj listę dla Statusu (z enuma) - robimy to ręcznie
            ViewBag.Statuses = Enum.GetValues(typeof(VisitStatus))
                .Cast<VisitStatus>()
                .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() });

            // 2. Przygotuj listę dla Priorytetu (z enuma) - robimy to ręcznie
            ViewBag.Priorities = Enum.GetValues(typeof(VisitPriority))
                .Cast<VisitPriority>()
                .Select(e => new SelectListItem { Value = e.ToString(), Text = e.ToString() });

            // 3. Przygotuj listę dla Zwierząt (z serwisu)
            ViewBag.Animals = await _visitService.GetAnimalsSelectListAsync();

            // 4. Przygotuj listę dla Weterynarzy (z serwisu)
            ViewBag.Vets = await _visitService.GetVetsSelectListAsync();

            // Przekaż pusty model do widoku, aby formularz mógł się poprawnie zainicjować
            return View(new VisitCreateDto());
        }

        [Authorize(Roles = "Admin,Receptionist")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VisitCreateDto createVisitDto)
        {
            if (!ModelState.IsValid)
            {
                var vetUsers = await _visitService.GetVetUsersAsync();
                ViewData["AssignedVetId"] = new SelectList(vetUsers, "Id", "Email", createVisitDto.AssignedVetId); // TODO: Zmień "Email"
                 // TODO: Załaduj ViewData dla AnimalId jeśli formularz tego wymaga
                return View(createVisitDto);
            }

            try
            {
                await _visitService.CreateVisitAsync(createVisitDto);
                // TODO: Komunikat sukcesu (TempData)
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // TODO: Loguj błąd ex
                ModelState.AddModelError("", ex.Message);
                var vetUsers = await _visitService.GetVetUsersAsync();
                ViewData["AssignedVetId"] = new SelectList(vetUsers, "Id", "Email", createVisitDto.AssignedVetId); // TODO: Zmień "Email"
                 // TODO: Załaduj ViewData dla AnimalId jeśli formularz tego wymaga
                return View(createVisitDto);
            }
        }

        [Authorize(Roles = "Admin,Receptionist,Vet")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var visitEditDto = await _visitService.GetVisitForEditAsync(id.Value, currentUser.Id);

                if (visitEditDto == null)
                {
                    return NotFound();
                }

                var vetUsers = await _visitService.GetVetUsersAsync();
                ViewData["AssignedVetId"] = new SelectList(vetUsers, "Id", "Email", visitEditDto.AssignedVetId); // TODO: Zmień "Email"
                 // TODO: Załaduj ViewData dla AnimalId jeśli jest edytowalne
                return View(visitEditDto);
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
                // TODO: Loguj błąd ex
                ModelState.AddModelError("", "Wystąpił błąd podczas ładowania danych do edycji wizyty.");
                return View("Error");
            }
        }

        [Authorize(Roles = "Admin,Receptionist,Vet")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VisitEditDto visitEditDto)
        {
            if (id != visitEditDto.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                var vetUsers = await _visitService.GetVetUsersAsync();
                ViewData["AssignedVetId"] = new SelectList(vetUsers, "Id", "Email", visitEditDto.AssignedVetId); // TODO: Zmień "Email"
                 // TODO: Załaduj ViewData dla AnimalId jeśli jest edytowalne
                return View(visitEditDto);
            }

            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var isVet = await _userManager.IsInRoleAsync(currentUser, "Vet");

                await _visitService.UpdateVisitAsync(id, visitEditDto, currentUser.Id, isVet);
                 // TODO: Komunikat sukcesu (TempData)
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
                // TODO: Loguj błąd ex
                ModelState.AddModelError("", ex.Message);
                var vetUsers = await _visitService.GetVetUsersAsync();
                ViewData["AssignedVetId"] = new SelectList(vetUsers, "Id", "Email", visitEditDto.AssignedVetId); // TODO: Zmień "Email"
                 // TODO: Załaduj ViewData dla AnimalId jeśli jest edytowalne
                return View(visitEditDto);
            }
        }

        [Authorize(Roles = "Admin, Receptionist")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var dto = await _visitService.GetVisitDetailsForReceptionistAsync(id.Value);
                if (dto == null) return NotFound();

                // TODO: Zwróć widok dedykowany do potwierdzenia usunięcia (np. DeleteConfirm.cshtml)
                return View(dto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                // TODO: Loguj błąd ex
                ModelState.AddModelError("", "Wystąpił błąd podczas ładowania danych do usunięcia.");
                return View("Error");
            }
        }

        [Authorize(Roles = "Admin, Receptionist")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _visitService.DeleteVisitAsync(id);
                // TODO: Komunikat sukcesu (TempData)
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                // TODO: Loguj błąd ex
                ModelState.AddModelError("", ex.Message);
                return RedirectToAction(nameof(Index), new { error = ex.Message });
            }
        }
         // TODO: Przenieś helpery jak GetVetUsersAsync do dedykowanego serwisu użytkowników (IUserService)
         // TODO: Dodaj helper do wyświetlania błędów z ModelState w widokach, jeśli nie masz globalnej obsługi błędów
    }
}