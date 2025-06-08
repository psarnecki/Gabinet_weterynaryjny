using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetClinicManager.Areas.Admin.DTOs.Medications;
using VetClinicManager.Models;
using VetClinicManager.Services;

namespace VetClinicManager.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class MedicationsController : Controller
    {
        private readonly IMedicationService _medicationService;

        public MedicationsController(IMedicationService medicationService)
        {
            _medicationService = medicationService;
        }

        // GET: Admin/Medications
        [Authorize(Roles = "Admin, Receptionist, Vet")]
        public async Task<IActionResult> Index()
        {
            var medicationListDtos = await _medicationService.GetAllMedicationsAsync();
            return View(medicationListDtos);
        }

        // GET: Admin/Medications/Details/5
        [Authorize(Roles = "Admin,Receptionist,Vet")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicationDto = await _medicationService.GetMedicationForDetailsAsync(id.Value);

            if (medicationDto == null)
            {
                return NotFound();
            }

            return View("Details", medicationDto);
        }

        // GET: Admin/Medications/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View(new MedicationCreateDto());
        }

        // POST: Admin/Medications/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(MedicationCreateDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return View(createDto);
            }

            var resultDto = await _medicationService.CreateMedicationAsync(createDto);

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Medications/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var editDto = await _medicationService.GetMedicationForEditAsync(id.Value);

            if (editDto == null)
            {
                return NotFound();
            }

            return View(editDto);
        }

        // POST: Admin/Medications/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, MedicationEditDto editDto)
        {
            if (id != editDto.Id)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(editDto);
            }

            try
            {
                await _medicationService.UpdateMedicationAsync(editDto);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _medicationService.MedicationExistsAsync(editDto.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch
            {
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Medications/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var deleteDto = await _medicationService.GetMedicationForDeleteAsync(id.Value);

            if (deleteDto == null)
            {
                return NotFound();
            }

            return View(deleteDto);
        }

        // POST: Admin/Medications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _medicationService.DeleteMedicationAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}