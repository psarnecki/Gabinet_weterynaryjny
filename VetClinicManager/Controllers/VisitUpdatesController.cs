using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using VetClinicManager.Data;
using VetClinicManager.DTOs.AnimalMedications;
using VetClinicManager.DTOs.VisitUpdates;
using VetClinicManager.Interfaces;
using VetClinicManager.Services;

namespace VetClinicManager.Controllers
{
    [Authorize]
    public class VisitUpdatesController : Controller
    {
        private readonly IVisitUpdateService _visitUpdateService;
        private readonly IAnimalMedicationService _animalMedicationService;
        private readonly ApplicationDbContext _context;

        public VisitUpdatesController(
            IVisitUpdateService visitUpdateService,
            IAnimalMedicationService animalMedicationService,
            ApplicationDbContext context)
        {
            _visitUpdateService = visitUpdateService;
            _animalMedicationService = animalMedicationService;
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

            var visitUpdate = await _context.VisitUpdates
                .Include(vu => vu.Visit)
                .Include(vu => vu.UpdatedBy)
                .Include(vu => vu.AnimalMedications)
                    .ThenInclude(am => am.Medication)
                .FirstOrDefaultAsync(vu => vu.Id == id);

            if (visitUpdate == null)
            {
                return NotFound();
            }

            return View(visitUpdate);
        }

        // GET: VisitUpdates/Create
        [Authorize(Roles = "Vet")]
        public async Task<IActionResult> Create(int visitId)
        {
            var visit = await _context.Visits
                .Include(v => v.Animal)
                .FirstOrDefaultAsync(v => v.Id == visitId);

            if (visit == null)
            {
                return NotFound();
            }

            ViewBag.VisitTitle = visit.Title;
            ViewBag.AnimalName = visit.Animal?.Name;
            ViewBag.AnimalId = visit.AnimalId;
            ViewBag.Medications = await _animalMedicationService.GetMedicationsSelectListAsync();

            var model = new VisitUpdateCreateDto
            {
                VisitId = visitId,
                AnimalMedications = new List<AnimalMedicationCreateVetDto>
                {
                    new AnimalMedicationCreateVetDto()
                }
            };

            return View(model);
        }

        // POST: VisitUpdates/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Vet")]
        public async Task<IActionResult> Create(VisitUpdateCreateDto createDto)
        {
            var visit = await _context.Visits
                .Include(v => v.Animal)
                .FirstOrDefaultAsync(v => v.Id == createDto.VisitId);

            if (visit == null)
            {
                return NotFound();
            }

            // Set animal ID for all medications
            if (createDto.AnimalMedications != null)
            {
                foreach (var medication in createDto.AnimalMedications)
                {
                    medication.AnimalId = visit.AnimalId;
                }
            }

            if (!ModelState.IsValid)
            {
                ViewBag.VisitTitle = visit.Title;
                ViewBag.AnimalName = visit.Animal?.Name;
                ViewBag.AnimalId = visit.AnimalId;
                ViewBag.Medications = await _animalMedicationService.GetMedicationsSelectListAsync();
                return View(createDto);
            }

            try
            {
                var vetId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                
                // First create the visit update
                var visitUpdate = await _visitUpdateService.CreateVisitUpdateAsync(createDto, vetId);
                
                // Then add medications if any
                if (createDto.AnimalMedications != null && createDto.AnimalMedications.Any())
                {
                    foreach (var medicationDto in createDto.AnimalMedications)
                    {
                        if (medicationDto.MedicationId > 0) // Only add if medication was selected
                        {
                            medicationDto.VisitUpdateId = visitUpdate.Id;
                            await _animalMedicationService.CreateAnimalMedicationAsync(medicationDto);
                        }
                    }
                }

                return RedirectToAction("Details", "Visits", new { id = createDto.VisitId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                
                ViewBag.VisitTitle = visit.Title;
                ViewBag.AnimalName = visit.Animal?.Name;
                ViewBag.AnimalId = visit.AnimalId;
                ViewBag.Medications = await _animalMedicationService.GetMedicationsSelectListAsync();
                
                return View(createDto);
            }
        }
      [Authorize(Roles = "Vet")]
      public async Task<IActionResult> Edit(int? id)
      {
          if (id == null)
          {
              return NotFound();
          }

          // Załaduj VisitUpdate z dołączonymi AnimalMedications
          var visitUpdate = await _context.VisitUpdates
              .Include(vu => vu.AnimalMedications)
              .ThenInclude(am => am.Medication) // Jeśli potrzebujesz danych o leku
              .FirstOrDefaultAsync(vu => vu.Id == id);

          if (visitUpdate == null)
          {
              return NotFound();
          }

          var currentVetId = User.FindFirstValue(ClaimTypes.NameIdentifier);
          if (visitUpdate.UpdatedByVetId != currentVetId)
          {
              return Forbid();
          }

          // Mapuj do DTO
          var editDto = new VisitUpdateEditVetDto
          {
              Id = visitUpdate.Id,
              Notes = visitUpdate.Notes,
              ImageUrl = visitUpdate.ImageUrl,
              PrescribedMedications = visitUpdate.PrescribedMedications,
              ExistingAnimalMedications = visitUpdate.AnimalMedications
                  .Select(am => new AnimalMedicationEditVetDto
                  {
                      Id = am.Id,
                      MedicationId = am.MedicationId,
                      StartDate = am.StartDate,
                      EndDate = am.EndDate
                  }).ToList(),
              NewAnimalMedications = new List<AnimalMedicationCreateVetDto>()
          };

          ViewBag.Medications = await _animalMedicationService.GetMedicationsSelectListAsync();
          ViewBag.VisitId = visitUpdate.VisitId;
    
          return View(editDto);
      }

[HttpPost]
[ValidateAntiForgeryToken]
[Authorize(Roles = "Vet")]
public async Task<IActionResult> Edit(int id, VisitUpdateEditVetDto editDto)
{
    if (id != editDto.Id)
    {
        return NotFound();
    }

    // Najpierw ręczna walidacja
    if (editDto.ExistingAnimalMedications != null)
    {
        foreach (var med in editDto.ExistingAnimalMedications)
        {
            if (med.MedicationId <= 0)
            {
                ModelState.AddModelError("ExistingAnimalMedications", "Please select a medication for all entries");
            }
        }
    }

    if (!ModelState.IsValid)
    {
        ViewBag.Medications = await _animalMedicationService.GetMedicationsSelectListAsync();
        ViewBag.VisitId = (await _context.VisitUpdates.FindAsync(id))?.VisitId;
        return View(editDto);
    }

    try
    {
        var currentVetId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
        // Update the visit update
        var result = await _visitUpdateService.UpdateVisitUpdateAsync(id, editDto, currentVetId);
        
        if (result == null)
        {
            return NotFound();
        }
        
        // Handle removed medications
        if (editDto.RemovedMedicationIds != null && editDto.RemovedMedicationIds.Any())
        {
            foreach (var medId in editDto.RemovedMedicationIds)
            {
                await _animalMedicationService.DeleteAnimalMedicationAsync(medId);
            }
        }

        // Handle new medications
        if (editDto.NewAnimalMedications != null && editDto.NewAnimalMedications.Any())
        {
            foreach (var medDto in editDto.NewAnimalMedications)
            {
                if (medDto.MedicationId > 0) // Only add if medication was selected
                {
                    medDto.VisitUpdateId = id;
                    await _animalMedicationService.CreateAnimalMedicationAsync(medDto);
                }
            }
        }

        return RedirectToAction("Details", "Visits", new { id = result.VisitId });
    }
    catch (UnauthorizedAccessException)
    {
        return Forbid();
    }
    catch (Exception ex)
    {
        ModelState.AddModelError("", $"Error updating visit update: {ex.Message}");
        ViewBag.Medications = await _animalMedicationService.GetMedicationsSelectListAsync();
        ViewBag.VisitId = (await _context.VisitUpdates.FindAsync(id))?.VisitId;
        return View(editDto);
    }
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