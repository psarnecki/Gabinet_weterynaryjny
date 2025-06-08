using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VetClinicManager.DTOs.HealthRecords;
using VetClinicManager.Services; // Popraw namespace, jeśli trzeba

namespace VetClinicManager.Controllers
{
    [Authorize(Roles = "Admin,Vet,Receptionist")]
    public class HealthRecordsController : Controller
    {
        private readonly IHealthRecordService _healthRecordService;

        public HealthRecordsController(IHealthRecordService healthRecordService)
        {
            _healthRecordService = healthRecordService;
        }

        // Akcja Details może używać GetForEditAsync, bo zwraca potrzebne DTO
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var dto = await _healthRecordService.GetForEditAsync(id.Value);
            if (dto == null) return NotFound();
            return View(dto);
        }
        
        [HttpGet]
        public async Task<IActionResult> Create(int animalId)
        {
            var animalName = await _healthRecordService.GetAnimalNameForCreateAsync(animalId);
            if (animalName == null)
            {
                TempData["ErrorMessage"] = "Nie można utworzyć karty zdrowia dla tego zwierzęcia.";
                return RedirectToAction("Index", "Animals");
            }
            
            ViewBag.AnimalName = await _healthRecordService.GetAnimalNameForCreateAsync(animalId);
            var model = new HealthRecordEditVetDto { AnimalId = animalId };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(HealthRecordEditVetDto createDto)
        {
            if (ModelState.IsValid)
            {
                var newId = await _healthRecordService.CreateAsync(createDto);
                TempData["SuccessMessage"] = "Karta zdrowia została utworzona.";
                return RedirectToAction(nameof(Details), new { id = newId });
            }
            
            ViewBag.AnimalName = await _healthRecordService.GetAnimalNameForCreateAsync(createDto.AnimalId);
            return View(createDto);
        }

        [Authorize(Roles = "Admin,Vet")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var editDto = await _healthRecordService.GetForEditAsync(id.Value);
            if (editDto == null) return NotFound();
            return View(editDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Vet")]
        public async Task<IActionResult> Edit(int id, HealthRecordEditVetDto editDto)
        {
            if (id != editDto.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var success = await _healthRecordService.UpdateAsync(editDto);
                if (!success) return NotFound();
                
                TempData["SuccessMessage"] = "Karta zdrowia została zaktualizowana.";
                return RedirectToAction(nameof(Details), new { id = editDto.Id });
            }
            return View(editDto);
        }

        [Authorize(Roles = "Admin,Vet")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var dto = await _healthRecordService.GetForDeleteAsync(id.Value);
            if (dto == null) return NotFound();
            return View(dto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Vet")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _healthRecordService.DeleteAsync(id);
            if (success) TempData["SuccessMessage"] = "Karta zdrowia została usunięta.";
            
            // Tutaj prawdopodobnie chcesz wrócić do listy zwierząt, a nie kart zdrowia
            return RedirectToAction("Index", "Animals"); 
        }
    }
}