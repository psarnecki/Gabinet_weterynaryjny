using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using VetClinicManager.Data;
using VetClinicManager.DTOs.VisitUpdates;
using VetClinicManager.Interfaces;

namespace VetClinicManager.Controllers
{
    [Authorize]
    public class VisitUpdatesController : Controller
    {
        private readonly IVisitUpdateService _visitUpdateService;
        private readonly ApplicationDbContext _context;

        public VisitUpdatesController(
            IVisitUpdateService visitUpdateService,
            ApplicationDbContext context)
        {
            _visitUpdateService = visitUpdateService;
            _context = context;
        }

        // GET: VisitUpdates
        public async Task<IActionResult> Index()
        {
            var visitUpdates = await _visitUpdateService.GetVisitUpdatesAsync();
            return View(visitUpdates);
        }

        // GET: VisitUpdates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitUpdate = await _visitUpdateService.GetVisitUpdateByIdAsync(id.Value);
            if (visitUpdate == null)
            {
                return NotFound();
            }

            return View(visitUpdate);
        }

        // GET: VisitUpdates/Create
        [Authorize(Roles = "Vet")]
        public IActionResult Create(int visitId)
        {
            // Pobierz tytuł wizyty i nazwę zwierzęcia
            var visitInfo = _context.Visits
                .Where(v => v.Id == visitId)
                .Select(v => new { v.Title, AnimalName = v.Animal.Name })
                .FirstOrDefault();

            if (visitInfo == null)
            {
                return NotFound();
            }

            ViewBag.VisitTitle = visitInfo.Title;
            ViewBag.AnimalName = visitInfo.AnimalName;

            var model = new VisitUpdateCreateDto
            {
                VisitId = visitId
            };

            return View(model);
        }

        // POST: VisitUpdates/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Vet")]
        public async Task<IActionResult> Create(VisitUpdateCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                // Ponowne załadowanie danych do widoku
                var visit = await _context.Visits
                    .Include(v => v.Animal)
                    .FirstOrDefaultAsync(v => v.Id == createDto.VisitId);
        
                ViewBag.VisitTitle = visit?.Title;
                ViewBag.AnimalName = visit?.Animal?.Name;
        
                return View(createDto);
            }

            try
            {
                var vetId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var result = await _visitUpdateService.CreateVisitUpdateAsync(createDto, vetId);
                return RedirectToAction("Details", "Visits", new { id = createDto.VisitId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(createDto);
            }
        }
        
        // GET: VisitUpdates/Edit/5
        [Authorize(Roles = "Vet")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitUpdate = await _visitUpdateService.GetVisitUpdateByIdAsync(id.Value);
            if (visitUpdate == null)
            {
                return NotFound();
            }

            var currentVetId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (visitUpdate.UpdatedByVetId != currentVetId)
            {
                return Forbid();
            }

            var editDto = new VisitUpdateEditVetDto
            {
                Id = visitUpdate.Id,
                Notes = visitUpdate.Notes,
                ImageUrl = visitUpdate.ImageUrl,
                PrescribedMedications = visitUpdate.PrescribedMedications,
                AnimalMedications = visitUpdate.AnimalMedications
            };

            ViewData["VisitId"] = new SelectList(_context.Visits, "Id", "Id", visitUpdate.VisitId);
            return View("Edit", editDto);
        }

        // POST: VisitUpdates/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Vet")]
        public async Task<IActionResult> Edit(int id, VisitUpdateEditVetDto editDto)
        {
            if (id != editDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var currentVetId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var result = await _visitUpdateService.UpdateVisitUpdateAsync(id, editDto, currentVetId);
                    
                    if (result == null)
                    {
                        return NotFound();
                    }
                    
                    return RedirectToAction(nameof(Index));
                }
                catch (UnauthorizedAccessException)
                {
                    return Forbid();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error updating visit update: {ex.Message}");
                }
            }

            ViewData["VisitId"] = new SelectList(_context.Visits, "Id", "Id");
            return View("Edit",editDto);
        }

        // GET: VisitUpdates/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitUpdate = await _visitUpdateService.GetVisitUpdateByIdAsync(id.Value);
            if (visitUpdate == null)
            {
                return NotFound();
            }

            return View(visitUpdate);
        }

        // POST: VisitUpdates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _visitUpdateService.DeleteVisitUpdateAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}