using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VetClinicManager.Data;
using VetClinicManager.Models;
using VetClinicManager.DTOs;
using VetClinicManager.Mappers;

namespace VetClinicManager.Controllers
{
    [Authorize]
    public class AnimalsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public AnimalsController(
            ApplicationDbContext context, 
            UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Animals
        [Authorize(Roles = "Admin,Receptionist,Vet")]
        public async Task<IActionResult> Index()
        {
            var animals = await _context.Animals
                .Include(a => a.User)
                .ToListAsync();
                
            var animalDtos = animals.ToDtos(); 
            return View(animalDtos);
        }

        // GET: Animals/Details/5
        [Authorize(Roles = "Admin,Receptionist,Vet,Client")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _context.Animals
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (animal == null)
            {
                return NotFound();
            }
        
            var currentUser = await _userManager.GetUserAsync(User);
            var currentUserId = currentUser?.Id;

            bool isPersonnel = await _userManager.IsInRoleAsync(currentUser, "Admin") ||
                               await _userManager.IsInRoleAsync(currentUser, "Receptionist") ||
                               await _userManager.IsInRoleAsync(currentUser, "Vet");

            bool isOwner = await _userManager.IsInRoleAsync(currentUser, "Client") && animal.UserId == currentUserId;
            
            if (!isPersonnel && !isOwner)
            {
                return Forbid();
            }
            
            var animalDto = animal.ToDto(); 
            return View(animalDto);
        }

        // GET: Animals/Create
        [Authorize(Roles = "Admin,Receptionist")]
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View(new CreateAnimalDto());
        }

        // POST: Animals/Create
        [Authorize(Roles = "Admin,Receptionist")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAnimalDto createAnimalDto)
        {
            if (ModelState.IsValid)
            {
                var animal = createAnimalDto.ToEntity(); 
                _context.Add(animal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", createAnimalDto.UserId);
            return View(createAnimalDto);
        }

        // GET: Animals/Edit/5
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _context.Animals.FindAsync(id);
            if (animal == null)
            {
                return NotFound();
            }
    
            var updateAnimalDto = animal.ToUpdateDto();
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", animal.UserId);
            return View(updateAnimalDto);
        }
        
        // POST: Animals/Edit/5
        [Authorize(Roles = "Admin,Receptionist")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateAnimalDto updateAnimalDto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var animal = await _context.Animals.FindAsync(id);
                    if (animal == null)
                    {
                        return NotFound();
                    }
                    
                    updateAnimalDto.UpdateFromDto(animal);
                    _context.Update(animal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnimalExists(id))
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
            
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", updateAnimalDto.UserId);
            return View(updateAnimalDto);
        }

        // GET: Animals/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var animal = await _context.Animals
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);
                
            if (animal == null)
            {
                return NotFound();
            }

            var animalDto = animal.ToDto();
            return View(animalDto);
        }

        // POST: Animals/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var animal = await _context.Animals.FindAsync(id);
            if (animal != null)
            {
                _context.Animals.Remove(animal);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AnimalExists(int id)
        {
            return _context.Animals.Any(e => e.Id == id);
        }
    }
}