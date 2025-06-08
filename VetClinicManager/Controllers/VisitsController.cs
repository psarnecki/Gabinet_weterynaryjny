using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Reflection;
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

        public VisitsController(IVisitService visitService, UserManager<User> userManager)
        {
            _visitService = visitService;
            _userManager = userManager;
        }

        // GET: Visits
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();

            object visitDtos;
            string viewName;

            if (User.IsInRole("Client"))
            {
                visitDtos = await _visitService.GetVisitsForOwnerAsync(currentUser.Id);
                viewName = "IndexUser";
            }
            else if (User.IsInRole("Vet"))
            {
                visitDtos = await _visitService.GetVisitsForVetAsync(currentUser.Id);
                viewName = "IndexVet";
            }
            else
            {
                visitDtos = await _visitService.GetVisitsForReceptionistAsync();
                viewName = "IndexReceptionist";
            }
            
            return View(viewName, visitDtos);
        }

        // GET: Visits/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();

            try
            {
                object? dto;
                string viewName;

                if (User.IsInRole("Client"))
                {
                    dto = await _visitService.GetDetailsForUserAsync(id.Value, currentUser.Id);
                    viewName = "DetailsUser";
                }
                else if (User.IsInRole("Vet"))
                {
                    dto = await _visitService.GetDetailsForVetAsync(id.Value, currentUser.Id);
                    viewName = "DetailsVet";
                }
                else
                {
                    dto = await _visitService.GetDetailsForReceptionistAsync(id.Value);
                    viewName = "DetailsReceptionist";
                }

                if (dto == null) return NotFound();
                return View(viewName, dto);
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (KeyNotFoundException) { return NotFound(); }
        }

        // GET: Visits/Create
        [Authorize(Roles = "Admin,Receptionist,Vet")]
        public async Task<IActionResult> Create()
        {
            await PrepareViewBagForCreate();
            return View(new VisitCreateDto());
        }

        // POST: Visits/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Create(VisitCreateDto createVisitDto)
        {
            if (!ModelState.IsValid)
            {
                await PrepareViewBagForCreate(createVisitDto.AssignedVetId);
                return View(createVisitDto);
            }
            
            await _visitService.CreateAsync(createVisitDto);
            TempData["SuccessMessage"] = "Wizyta została pomyślnie utworzona.";
            return RedirectToAction(nameof(Index));
        }

        // GET: Visits/Edit/5
        [Authorize(Roles = "Admin,Receptionist,Vet")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Unauthorized();

            try
            {
                var visitEditDto = await _visitService.GetForEditAsync(id.Value, currentUser.Id, User.IsInRole("Vet"));
                if (visitEditDto == null) return NotFound();
                
                await PrepareViewBagForEdit(visitEditDto.AssignedVetId);
                return View(visitEditDto);
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (KeyNotFoundException) { return NotFound(); }
        }

        // POST: Visits/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Receptionist,Vet")]
        public async Task<IActionResult> Edit(int id, VisitEditDto visitEditDto)
        {
            if (id != visitEditDto.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                await PrepareViewBagForEdit(visitEditDto.AssignedVetId);
                
                return View(visitEditDto);
            }
            
            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null) return Unauthorized();

                await _visitService.UpdateAsync(id, visitEditDto, currentUser.Id, User.IsInRole("Vet"));
                TempData["SuccessMessage"] = "Wizyta została zaktualizowana.";
                return RedirectToAction(nameof(Index));
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (KeyNotFoundException) { return NotFound(); }
        }
        
        // GET: Visits/Delete/5
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var dto = await _visitService.GetForDeleteAsync(id.Value);
            if (dto == null) return NotFound();
            
            return View(dto);
        }

        // POST: Visits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _visitService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Wizyta została usunięta.";
            }
            catch (KeyNotFoundException)
            {
                TempData["ErrorMessage"] = "Nie znaleziono wizyty do usunięcia.";
            }
            return RedirectToAction(nameof(Index));
        }

        // Metoda pomocnicza do przygotowania danych dla formularza tworzenia
        private async Task PrepareViewBagForCreate(string? selectedVetId = null)
        {
            ViewBag.Animals = await _visitService.GetAnimalsSelectListAsync();
            var vetUsers = await _visitService.GetVetUsersAsync();
            
            var vetSelectListItems = vetUsers.Select(v => new 
            {
                Id = v.Id,
                FullName = $"{v.FirstName} {v.LastName}".Trim()
            }).ToList();
            
            ViewBag.Vets = new SelectList(vetSelectListItems, "Id", "FullName", selectedVetId);
            ViewBag.Statuses = GetEnumSelectList<VisitStatus>();
            ViewBag.Priorities = GetEnumSelectList<VisitPriority>();
        }
        
        // Metoda pomocnicza do przygotowania danych dla formularza edycji
        private async Task PrepareViewBagForEdit(string? selectedVetId = null)
        {
            var vetUsers = await _visitService.GetVetUsersAsync();

            var vetSelectListItems = vetUsers.Select(v => new
            {
                Id = v.Id,
                FullName = $"{v.FirstName} {v.LastName}".Trim()
            }).ToList();

            ViewBag.Vets = new SelectList(vetSelectListItems, "Id", "FullName", selectedVetId);
            ViewBag.Statuses = GetEnumSelectList<VisitStatus>();
            ViewBag.Priorities = GetEnumSelectList<VisitPriority>();
        }

        // Generyczna metoda pomocnicza do tworzenia SelectList z enuma
        private SelectList GetEnumSelectList<TEnum>() where TEnum : Enum
        {
            return new SelectList(Enum.GetValues(typeof(TEnum)).Cast<TEnum>().Select(e =>
                new { Value = e, Text = e.GetType().GetMember(e.ToString()).First()
                    .GetCustomAttribute<System.ComponentModel.DataAnnotations.DisplayAttribute>()?.GetName() ?? e.ToString() }),
                "Value", "Text");
        }
        
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GenerateVisitReport(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(user);

            try
            {
                var pdfBytes = await _visitService.GeneratePdfReportAsync(id, user.Id, roles);
                if (pdfBytes == null)
                {
                    return Forbid(); 
                }
            
                return File(pdfBytes, "application/pdf", $"Raport_Wizyty_{id}.pdf");
            }
            catch(UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch(KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}