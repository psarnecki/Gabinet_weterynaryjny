using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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
        private readonly ApplicationDbContext _context; // Zachowujemy context dla prostych operacji GET

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
                .AsNoTracking()
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

        // GET: VisitUpdates/Create - BEZ ZMIAN, logika jest OK
        [Authorize(Roles = "Vet")]
        public async Task<IActionResult> Create(int visitId)
        {
            var visit = await _context.Visits
                .Include(v => v.Animal)
                .AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == visitId);

            if (visit == null) return NotFound();

            ViewBag.VisitTitle = visit.Title;
            ViewBag.AnimalName = visit.Animal?.Name;
            ViewBag.AnimalId = visit.AnimalId;
            ViewBag.Medications = await _animalMedicationService.GetMedicationsSelectListAsync();

            var model = new VisitUpdateCreateDto
            {
                VisitId = visitId,
                AnimalMedications = new List<AnimalMedicationCreateVetDto>() // Pusta lista na start
            };

            return View(model);
        }

        // POST: VisitUpdates/Create - Uproszczony, cała logika w serwisach
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Vet")]
        public async Task<IActionResult> Create(VisitUpdateCreateDto createDto)
        {
            // Pobieramy visit, aby uzyskać AnimalId
            var visit = await _context.Visits.AsNoTracking().FirstOrDefaultAsync(v => v.Id == createDto.VisitId);
            if (visit == null) return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.VisitTitle = visit.Title;
                ViewBag.AnimalName = (await _context.Animals.FindAsync(visit.AnimalId))?.Name;
                ViewBag.AnimalId = visit.AnimalId;
                ViewBag.Medications = await _animalMedicationService.GetMedicationsSelectListAsync();
                return View(createDto);
            }

            // Ustaw AnimalId dla wszystkich nowo dodawanych leków
            if (createDto.AnimalMedications != null)
            {
                foreach (var medication in createDto.AnimalMedications)
                {
                    medication.AnimalId = visit.AnimalId;
                }
            }

            try
            {
                var vetId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var visitUpdate = await _visitUpdateService.CreateVisitUpdateAsync(createDto, vetId);

                // Logika dodawania leków została przeniesiona do serwisu AnimalMedicationService
                // dla spójności, ale zostawmy ją tutaj dla uproszczenia, jeśli działa.
                // Idealnie byłoby mieć jedną metodę w serwisie, która robi wszystko.
                if (createDto.AnimalMedications != null && createDto.AnimalMedications.Any())
                {
                    foreach (var medicationDto in createDto.AnimalMedications.Where(m => m.MedicationId > 0))
                    {
                        medicationDto.VisitUpdateId = visitUpdate.Id;
                        await _animalMedicationService.CreateAnimalMedicationAsync(medicationDto);
                    }
                }

                return RedirectToAction("Details", "Visits", new { id = createDto.VisitId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.VisitTitle = visit.Title;
                ViewBag.AnimalName = (await _context.Animals.FindAsync(visit.AnimalId))?.Name;
                ViewBag.AnimalId = visit.AnimalId;
                ViewBag.Medications = await _animalMedicationService.GetMedicationsSelectListAsync();
                return View(createDto);
            }
        }

        // GET: VisitUpdates/Edit - BEZ ZMIAN, logika jest OK
        [Authorize(Roles = "Vet")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            
            var visitUpdate = await _context.VisitUpdates
                .AsNoTracking()
                .Include(vu => vu.AnimalMedications)
                .ThenInclude(am => am.Medication)
                .FirstOrDefaultAsync(vu => vu.Id == id);

            if (visitUpdate == null) return NotFound();

            var currentVetId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (visitUpdate.UpdatedByVetId != currentVetId) return Forbid();

            var editDto = new VisitUpdateEditVetDto
            {
                Id = visitUpdate.Id,
                Notes = visitUpdate.Notes,
                ImageUrl = visitUpdate.ImageUrl,
                PrescribedMedications = visitUpdate.PrescribedMedications,
                ExistingAnimalMedications = visitUpdate.AnimalMedications.Select(am => new AnimalMedicationEditVetDto
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

        // POST: VisitUpdates/Edit - KLUCZOWE ZMIANY
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Vet")]
        public async Task<IActionResult> Edit(int id, VisitUpdateEditVetDto editDto)
        {
            if (id != editDto.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                await PopulateEditViewBag(id);
                return View(editDto);
            }

            try
            {
                var currentVetId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Uzupełnij AnimalId dla nowo dodawanych leków PRZED wywołaniem serwisu
                if (editDto.NewAnimalMedications != null && editDto.NewAnimalMedications.Any(m => m.MedicationId > 0))
                {
                    var visitId = (await _context.VisitUpdates.AsNoTracking().FirstOrDefaultAsync(vu => vu.Id == id))?.VisitId;
                    var animalId = (await _context.Visits.AsNoTracking().FirstOrDefaultAsync(v => v.Id == visitId))?.AnimalId;

                    if (!animalId.HasValue || animalId.Value <= 0)
                    {
                        ModelState.AddModelError("", "Nie można odnaleźć powiązanego zwierzęcia.");
                        await PopulateEditViewBag(id);
                        return View(editDto);
                    }
                    
                    foreach (var newMed in editDto.NewAnimalMedications)
                    {
                        newMed.AnimalId = animalId.Value;
                    }
                }
                
                // Wywołaj jedną, kompletną metodę serwisową
                var result = await _visitUpdateService.UpdateVisitUpdateAsync(id, editDto, currentVetId);

                if (result == null) return NotFound();

                return RedirectToAction("Details", "Visits", new { id = result.VisitId });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Błąd podczas aktualizacji: {ex.Message}");
                await PopulateEditViewBag(id);
                return View(editDto);
            }
        }
        
        [HttpPost] // Usunięto ActionName
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id) // Zmieniono nazwę i parametr
        {
            // Pobierz visitId ZANIM usuniesz rekord, aby wiedzieć, dokąd wrócić
            var visitUpdate = await _context.VisitUpdates.AsNoTracking().FirstOrDefaultAsync(vu => vu.Id == id);
            if (visitUpdate == null)
            {
                return NotFound();
            }
            var visitIdToReturnTo = visitUpdate.VisitId;

            await _visitUpdateService.DeleteVisitUpdateAsync(id);
    
            // Przekieruj z powrotem do szczegółów wizyty, a nie do listy wszystkich aktualizacji
            return RedirectToAction("Details", "Visits", new { id = visitIdToReturnTo });
        }
        
        private async Task PopulateEditViewBag(int visitUpdateId)
        {
            ViewBag.Medications = await _animalMedicationService.GetMedicationsSelectListAsync();
            ViewBag.VisitId = (await _context.VisitUpdates.AsNoTracking().FirstOrDefaultAsync(vu => vu.Id == visitUpdateId))?.VisitId;
        }
    }
}
   
  