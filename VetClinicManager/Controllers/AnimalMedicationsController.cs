using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetClinicManager.DTOs.AnimalMedications;
using VetClinicManager.Services;

namespace VetClinicManager.Controllers
{
    [Authorize]
    public class AnimalMedicationsController : Controller
    {
        private readonly IAnimalMedicationService _animalMedicationService;
        private readonly ILogger<AnimalMedicationsController> _logger;

        public AnimalMedicationsController(IAnimalMedicationService animalMedicationService, ILogger<AnimalMedicationsController> logger)
        {
            _animalMedicationService = animalMedicationService;
            _logger = logger; 
        }

        // GET: AnimalMedications
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var animalMedications = await _animalMedicationService.GetAnimalMedicationsAsync();
            return View(animalMedications);
        }

        // GET: AnimalMedications/Details/5
        [Authorize(Roles = "Admin,Receptionist,Vet,Client")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var animalMedication = await _animalMedicationService.GetAnimalMedicationByIdAsync(id.Value);
            if (animalMedication == null)
                return NotFound();

            return View(animalMedication);
        }

        // GET: AnimalMedications/Create
        [Authorize(Roles = "Admin,Vet")]
        public async Task<IActionResult> Create()
        {
            try
            {
                ViewData["AnimalId"] = await _animalMedicationService.GetAnimalsSelectListAsync();
                ViewData["MedicationId"] = await _animalMedicationService.GetMedicationsSelectListAsync();
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Wystąpił błąd podczas przypisywania leku od zwierzęcia");
            
                ModelState.AddModelError("", "Wystąpił nieoczekiwany błąd.");
                return View("Error");
            }
        }

        // POST: AnimalMedications/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Vet")]
        public async Task<IActionResult> Create(AnimalMedicationCreateVetDto dto)
        {
            if (!ModelState.IsValid)
            {
                ViewData["AnimalId"] = await _animalMedicationService.GetAnimalsSelectListAsync();
                ViewData["MedicationId"] = await _animalMedicationService.GetMedicationsSelectListAsync();
                return View(dto);
            }

            await _animalMedicationService.CreateAnimalMedicationAsync(dto);
            return RedirectToAction(nameof(Index));
        }

        // GET: AnimalMedications/Edit/5
        [Authorize(Roles = "Admin,Vet")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) 
                return NotFound();

            var editDto = await _animalMedicationService.GetForEditAsync(id.Value);    
            
            if (editDto == null) 
                return NotFound();

            ViewData["AnimalId"] = await _animalMedicationService.GetAnimalsSelectListAsync();
            ViewData["MedicationId"] = await _animalMedicationService.GetMedicationsSelectListAsync();

            return View(editDto);
        }

        // POST: AnimalMedications/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Vet")]
        public async Task<IActionResult> Edit(int id, AnimalMedicationEditVetDto dto)
        {
            if (id != dto.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewData["AnimalId"] = await _animalMedicationService.GetAnimalsSelectListAsync();
                ViewData["MedicationId"] = await _animalMedicationService.GetMedicationsSelectListAsync();
                return View(dto);
            }

            try
            {
                await _animalMedicationService.UpdateAnimalMedicationAsync(dto);
            }
            catch (Exception)
            {
                if (!await _animalMedicationService.AnimalMedicationExistsAsync(dto.Id))
                    return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: AnimalMedications/Delete/5
        [Authorize(Roles = "Admin,Vet")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var animalMedication = await _animalMedicationService.GetAnimalMedicationByIdAsync(id.Value);
            if (animalMedication == null)
                return NotFound();

            return View(animalMedication);
        }

        // POST: AnimalMedications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Vet")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _animalMedicationService.DeleteAnimalMedicationAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}