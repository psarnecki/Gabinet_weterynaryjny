using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VetClinicManager.Data;
using VetClinicManager.Models;

namespace VetClinicManager.Controllers
{
    public class AnimalMedicationsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AnimalMedicationsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: AnimalMedications
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.AnimalMedications.Include(a => a.Animal).Include(a => a.Medication);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: AnimalMedications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animalMedication = await _context.AnimalMedications
                .Include(a => a.Animal)
                .Include(a => a.Medication)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (animalMedication == null)
            {
                return NotFound();
            }

            return View(animalMedication);
        }

        // GET: AnimalMedications/Create
        public IActionResult Create()
        {
            ViewData["AnimalId"] = new SelectList(_context.Animals, "Id", "Id");
            ViewData["MedicationId"] = new SelectList(_context.Medications, "Id", "Id");
            return View();
        }

        // POST: AnimalMedications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AnimalId,MedicationId,StartDate,EndDate")] AnimalMedication animalMedication)
        {
            if (ModelState.IsValid)
            {
                _context.Add(animalMedication);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AnimalId"] = new SelectList(_context.Animals, "Id", "Id", animalMedication.AnimalId);
            ViewData["MedicationId"] = new SelectList(_context.Medications, "Id", "Id", animalMedication.MedicationId);
            return View(animalMedication);
        }

        // GET: AnimalMedications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animalMedication = await _context.AnimalMedications.FindAsync(id);
            if (animalMedication == null)
            {
                return NotFound();
            }
            ViewData["AnimalId"] = new SelectList(_context.Animals, "Id", "Id", animalMedication.AnimalId);
            ViewData["MedicationId"] = new SelectList(_context.Medications, "Id", "Id", animalMedication.MedicationId);
            return View(animalMedication);
        }

        // POST: AnimalMedications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AnimalId,MedicationId,StartDate,EndDate")] AnimalMedication animalMedication)
        {
            if (id != animalMedication.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(animalMedication);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnimalMedicationExists(animalMedication.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AnimalId"] = new SelectList(_context.Animals, "Id", "Id", animalMedication.AnimalId);
            ViewData["MedicationId"] = new SelectList(_context.Medications, "Id", "Id", animalMedication.MedicationId);
            return View(animalMedication);
        }

        // GET: AnimalMedications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animalMedication = await _context.AnimalMedications
                .Include(a => a.Animal)
                .Include(a => a.Medication)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (animalMedication == null)
            {
                return NotFound();
            }

            return View(animalMedication);
        }

        // POST: AnimalMedications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var animalMedication = await _context.AnimalMedications.FindAsync(id);
            if (animalMedication != null)
            {
                _context.AnimalMedications.Remove(animalMedication);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AnimalMedicationExists(int id)
        {
            return _context.AnimalMedications.Any(e => e.Id == id);
        }
    }
}
