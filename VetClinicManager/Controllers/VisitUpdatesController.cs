using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VetClinicManager.DTOs.VisitUpdates;
using VetClinicManager.Services;

namespace VetClinicManager.Controllers
{
    [Authorize(Roles = "Admin,Vet")]
    public class VisitUpdatesController : Controller
    {
        private readonly IVisitUpdateService _visitUpdateService;
        private readonly IAnimalMedicationService _animalMedicationService;

        public VisitUpdatesController(IVisitUpdateService visitUpdateService, IAnimalMedicationService animalMedicationService)
        {
            _visitUpdateService = visitUpdateService;
            _animalMedicationService = animalMedicationService;
        }

        public async Task<IActionResult> Create(int visitId)
        {
            ViewBag.Medications = await _animalMedicationService.GetMedicationsSelectListAsync();
            var model = new VisitUpdateCreateDto { VisitId = visitId };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VisitUpdateCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Medications = await _animalMedicationService.GetMedicationsSelectListAsync();
                return View(createDto);
            }

            try
            {
                var vetId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var visitId = await _visitUpdateService.CreateAsync(createDto, vetId!);
                TempData["SuccessMessage"] = "Dodano aktualizację wizyty.";
                return RedirectToAction("Details", "Visits", new { id = visitId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.Medications = await _animalMedicationService.GetMedicationsSelectListAsync();
                return View(createDto);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            var vetId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var editDto = await _visitUpdateService.GetForEditAsync(id, vetId!);
            if (editDto == null) return Forbid();

            ViewBag.Medications = await _animalMedicationService.GetMedicationsSelectListAsync();
            return View(editDto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VisitUpdateEditVetDto editDto)
        {
            if (id != editDto.Id) return BadRequest();
            if (!ModelState.IsValid)
            {
                ViewBag.Medications = await _animalMedicationService.GetMedicationsSelectListAsync();
                return View(editDto);
            }

            try
            {
                var vetId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var visitId = await _visitUpdateService.UpdateAsync(id, editDto, vetId!);
                TempData["SuccessMessage"] = "Aktualizacja została zapisana.";
                return RedirectToAction("Details", "Visits", new { id = visitId });
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (KeyNotFoundException) { return NotFound(); }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                ViewBag.Medications = await _animalMedicationService.GetMedicationsSelectListAsync();
                return View(editDto);
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            var vetId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var deleteDto = await _visitUpdateService.GetForDeleteAsync(id, vetId!);
            if (deleteDto == null) return Forbid();

            return View(deleteDto);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(VisitUpdateDeleteDto dto)
        {
            try
            {
                var vetId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var visitId = await _visitUpdateService.DeleteAsync(dto.Id, vetId!);
                TempData["SuccessMessage"] = "Aktualizacja została usunięta.";
                return RedirectToAction("Details", "Visits", new { id = visitId });
            }
            catch (UnauthorizedAccessException) { return Forbid(); }
            catch (KeyNotFoundException) { return NotFound(); }
        }
    }
}