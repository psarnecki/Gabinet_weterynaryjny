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
    public class OrderUpdatesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrderUpdatesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: OrderUpdates
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.OrderUpdates.Include(o => o.MedicalOrder);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: OrderUpdates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderUpdate = await _context.OrderUpdates
                .Include(o => o.MedicalOrder)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderUpdate == null)
            {
                return NotFound();
            }

            return View(orderUpdate);
        }

        // GET: OrderUpdates/Create
        public IActionResult Create()
        {
            ViewData["MedicalOrderId"] = new SelectList(_context.Orders, "Id", "Id");
            return View();
        }

        // POST: OrderUpdates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Notes,UpdateDate,ImageUrl,PrescribedMedications,MedicalOrderId")] OrderUpdate orderUpdate)
        {
            if (ModelState.IsValid)
            {
                _context.Add(orderUpdate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MedicalOrderId"] = new SelectList(_context.Orders, "Id", "Id", orderUpdate.MedicalOrderId);
            return View(orderUpdate);
        }

        // GET: OrderUpdates/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderUpdate = await _context.OrderUpdates.FindAsync(id);
            if (orderUpdate == null)
            {
                return NotFound();
            }
            ViewData["MedicalOrderId"] = new SelectList(_context.Orders, "Id", "Id", orderUpdate.MedicalOrderId);
            return View(orderUpdate);
        }

        // POST: OrderUpdates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Notes,UpdateDate,ImageUrl,PrescribedMedications,MedicalOrderId")] OrderUpdate orderUpdate)
        {
            if (id != orderUpdate.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(orderUpdate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderUpdateExists(orderUpdate.Id))
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
            ViewData["MedicalOrderId"] = new SelectList(_context.Orders, "Id", "Id", orderUpdate.MedicalOrderId);
            return View(orderUpdate);
        }

        // GET: OrderUpdates/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var orderUpdate = await _context.OrderUpdates
                .Include(o => o.MedicalOrder)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (orderUpdate == null)
            {
                return NotFound();
            }

            return View(orderUpdate);
        }

        // POST: OrderUpdates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var orderUpdate = await _context.OrderUpdates.FindAsync(id);
            if (orderUpdate != null)
            {
                _context.OrderUpdates.Remove(orderUpdate);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderUpdateExists(int id)
        {
            return _context.OrderUpdates.Any(e => e.Id == id);
        }
    }
}
