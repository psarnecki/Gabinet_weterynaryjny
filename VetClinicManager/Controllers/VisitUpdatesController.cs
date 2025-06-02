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
    public class VisitUpdatesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VisitUpdatesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: VisitUpdates
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.VisitUpdates.Include(v => v.Visit);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: VisitUpdates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitUpdate = await _context.VisitUpdates
                .Include(v => v.Visit)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (visitUpdate == null)
            {
                return NotFound();
            }

            return View(visitUpdate);
        }

        // GET: VisitUpdates/Create
        public IActionResult Create()
        {
            ViewData["VisitId"] = new SelectList(_context.Visits, "Id", "Id");
            return View();
        }

        // POST: VisitUpdates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Notes,UpdateDate,ImageUrl,PrescribedMedications,VisitId,UpdatedByVetId")] VisitUpdate visitUpdate)
        {
            if (ModelState.IsValid)
            {
                _context.Add(visitUpdate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["VisitId"] = new SelectList(_context.Visits, "Id", "Id", visitUpdate.VisitId);
            return View(visitUpdate);
        }

        // GET: VisitUpdates/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitUpdate = await _context.VisitUpdates.FindAsync(id);
            if (visitUpdate == null)
            {
                return NotFound();
            }
            ViewData["VisitId"] = new SelectList(_context.Visits, "Id", "Id", visitUpdate.VisitId);
            return View(visitUpdate);
        }

        // POST: VisitUpdates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Notes,UpdateDate,ImageUrl,PrescribedMedications,VisitId,UpdatedByVetId")] VisitUpdate visitUpdate)
        {
            if (id != visitUpdate.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(visitUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VisitUpdateExists(visitUpdate.Id))
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
            ViewData["VisitId"] = new SelectList(_context.Visits, "Id", "Id", visitUpdate.VisitId);
            return View(visitUpdate);
        }

        // GET: VisitUpdates/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visitUpdate = await _context.VisitUpdates
                .Include(v => v.Visit)
                .FirstOrDefaultAsync(m => m.Id == id);
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
            var visitUpdate = await _context.VisitUpdates.FindAsync(id);
            if (visitUpdate != null)
            {
                _context.VisitUpdates.Remove(visitUpdate);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VisitUpdateExists(int id)
        {
            return _context.VisitUpdates.Any(e => e.Id == id);
        }
    }
}
