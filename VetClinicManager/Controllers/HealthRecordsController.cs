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
    public class HealthRecordsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HealthRecordsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: HealthRecords
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.HealthRecords.Include(h => h.Animal);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: HealthRecords/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var healthRecord = await _context.HealthRecords
                .Include(h => h.Animal)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (healthRecord == null)
            {
                return NotFound();
            }

            return View(healthRecord);
        }

        // GET: HealthRecords/Create
        public IActionResult Create()
        {
            ViewData["AnimalId"] = new SelectList(_context.Animals, "Id", "Id");
            return View();
        }

        // POST: HealthRecords/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AnimalId,IsSterilized,ChronicDiseases,Allergies,Vaccinations,LastVaccinationDate")] HealthRecord healthRecord)
        {
            if (ModelState.IsValid)
            {
                _context.Add(healthRecord);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AnimalId"] = new SelectList(_context.Animals, "Id", "Id", healthRecord.AnimalId);
            return View(healthRecord);
        }

        // GET: HealthRecords/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var healthRecord = await _context.HealthRecords.FindAsync(id);
            if (healthRecord == null)
            {
                return NotFound();
            }
            ViewData["AnimalId"] = new SelectList(_context.Animals, "Id", "Id", healthRecord.AnimalId);
            return View(healthRecord);
        }

        // POST: HealthRecords/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AnimalId,IsSterilized,ChronicDiseases,Allergies,Vaccinations,LastVaccinationDate")] HealthRecord healthRecord)
        {
            if (id != healthRecord.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(healthRecord);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HealthRecordExists(healthRecord.Id))
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
            ViewData["AnimalId"] = new SelectList(_context.Animals, "Id", "Id", healthRecord.AnimalId);
            return View(healthRecord);
        }

        // GET: HealthRecords/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var healthRecord = await _context.HealthRecords
                .Include(h => h.Animal)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (healthRecord == null)
            {
                return NotFound();
            }

            return View(healthRecord);
        }

        // POST: HealthRecords/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var healthRecord = await _context.HealthRecords.FindAsync(id);
            if (healthRecord != null)
            {
                _context.HealthRecords.Remove(healthRecord);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HealthRecordExists(int id)
        {
            return _context.HealthRecords.Any(e => e.Id == id);
        }
    }
}
