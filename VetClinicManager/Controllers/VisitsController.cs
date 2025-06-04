using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VetClinicManager.Data;
using VetClinicManager.Models;
using VetClinicManager.Dtos;
using VetClinicManager.Mappers;

namespace VetClinicManager.Controllers
{
    [Authorize]
    public class VisitsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public VisitsController(
            ApplicationDbContext context,
            UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Visits
        [Authorize(Roles = "Admin,Receptionist,Vet")]
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var isReceptionist = await _userManager.IsInRoleAsync(currentUser, "Admin") || 
                                 await _userManager.IsInRoleAsync(currentUser, "Receptionist");
            var isVet = await _userManager.IsInRoleAsync(currentUser, "Vet");

            IQueryable<Visit> visitsQuery = _context.Visits
                .Include(v => v.Animal)
                .Include(v => v.AssignedVet)
                .Include(v => v.Updates);

            if (isVet && !isReceptionist)
            {
                visitsQuery = visitsQuery.Where(v => v.AssignedVetId == currentUser.Id);
            }

            var visits = await visitsQuery.ToListAsync();

            if (isReceptionist)
            {
                var receptionistDtos = visits.Select(v => v.ToReceptionistDto()).ToList();
                return View("IndexReceptionist", receptionistDtos);
            }
            else if (isVet)
            {
                var vetDtos = visits.Select(v => v.ToVetDto()).ToList(); // Używamy mappera
                return View("IndexVet", vetDtos);
            }

            return Forbid();
        }
        // GET: Visits/Details/5
        [Authorize(Roles = "Admin,Receptionist,Vet,Client")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visit = await _context.Visits
                .Include(v => v.Animal)
                .Include(v => v.AssignedVet)
                .Include(v => v.Updates)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (visit == null)
            {
                return NotFound();
            }

            var currentUser = await _userManager.GetUserAsync(User);
            var currentUserId = currentUser?.Id;

            bool isReceptionist = await _userManager.IsInRoleAsync(currentUser, "Admin") ||
                                 await _userManager.IsInRoleAsync(currentUser, "Receptionist");
            bool isVet = await _userManager.IsInRoleAsync(currentUser, "Vet");
            bool isClient = await _userManager.IsInRoleAsync(currentUser, "Client");
            bool isAssignedVet = visit.AssignedVetId == currentUserId;

            if (isReceptionist)
            {
                var dto = visit.ToReceptionistDto();
                return View("DetailsReceptionist", dto);
            }
            else if (isVet)
            {
                if (!isAssignedVet)
                {
                    return Forbid();
                }
    
                var vetDto = visit.ToVetDto();
                return View("DetailsVet", vetDto);
            }
            else if (isClient && visit.Animal?.UserId == currentUserId)
            {
                var dto = visit.ToUserDto();
                return View("DetailsUser", dto);
            }

            return Forbid();
        }

        // GET: Visits/Create
        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Create()
        {
            var vetUsers = await GetVetUsersAsync();
            
            ViewData["AnimalId"] = new SelectList(_context.Animals, "Id", "Name");
            ViewData["AssignedVetId"] = new SelectList(vetUsers, "Id", "Email");
            return View(new CreateVisitDto { CreatedDate = DateTime.Now });
        }

        // POST: Visits/Create
        [Authorize(Roles = "Admin,Receptionist")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateVisitDto createVisitDto)
        {
            if (ModelState.IsValid)
            {
                var visit = createVisitDto.ToEntity();
                _context.Add(visit);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            var vetUsers = await GetVetUsersAsync();
            
            ViewData["AnimalId"] = new SelectList(_context.Animals, "Id", "Name", createVisitDto.AnimalId);
            ViewData["AssignedVetId"] = new SelectList(vetUsers, "Id", "Email", createVisitDto.AssignedVetId);
            return View(createVisitDto);
        }

        // GET: Visits/Edit/5
        [Authorize(Roles = "Admin,Receptionist,Vet")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visit = await _context.Visits.FindAsync(id);
            if (visit == null)
            {
                return NotFound();
            }

            var vetUsers = await GetVetUsersAsync();
            var currentUser = await _userManager.GetUserAsync(User);
            var isVet = await _userManager.IsInRoleAsync(currentUser, "Vet");
            var isAssignedVet = visit.AssignedVetId == currentUser?.Id;

            if (isVet && !isAssignedVet)
            {
                return Forbid();
            }

            var updateDto = new UpdateVisitDto
            {
                Id = visit.Id,
                Title = visit.Title,
                Description = visit.Description,
                CreatedDate = visit.CreatedDate,
                Status = visit.Status,
                Priority = visit.Priority,
                AssignedVetId = visit.AssignedVetId,
                Animal = new AnimalBriefDto
                {
                    Id = visit.AnimalId,
                    Name = _context.Animals.Find(visit.AnimalId)?.Name ?? string.Empty,
                    Breed = _context.Animals.Find(visit.AnimalId)?.Breed ?? string.Empty
                }
            };

            ViewData["AnimalId"] = new SelectList(_context.Animals, "Id", "Name", visit.AnimalId);
            ViewData["AssignedVetId"] = new SelectList(vetUsers, "Id", "Email", visit.AssignedVetId);
            return View(updateDto);
        }

        // POST: Visits/Edit/5
        [Authorize(Roles = "Admin,Receptionist,Vet")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateVisitDto updateVisitDto)
        {
            if (id != updateVisitDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var visit = await _context.Visits.FindAsync(id);
                    if (visit == null)
                    {
                        return NotFound();
                    }

                    var currentUser = await _userManager.GetUserAsync(User);
                    var isVet = await _userManager.IsInRoleAsync(currentUser, "Vet");
                    var isAssignedVet = visit.AssignedVetId == currentUser?.Id;

                    if (isVet && !isAssignedVet)
                    {
                        return Forbid();
                    }

                    if (isVet)
                    {
                        // Weterynarz może edytować tylko określone pola
                        visit.Description = updateVisitDto.Description;
                        visit.Status = updateVisitDto.Status;
                    }
                    else
                    {
                        // Admin/Recepcja może edytować wszystkie pola
                        updateVisitDto.ToEntity(visit);
                    }

                    _context.Update(visit);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VisitExists(updateVisitDto.Id))
                    {
                        return NotFound();
                    }
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            var vetUsers = await GetVetUsersAsync();
            
            ViewData["AnimalId"] = new SelectList(_context.Animals, "Id", "Name", updateVisitDto.Animal?.Id);
            ViewData["AssignedVetId"] = new SelectList(vetUsers, "Id", "Email", updateVisitDto.AssignedVetId);
            return View(updateVisitDto);
        }

        // GET: Visits/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var visit = await _context.Visits
                .Include(v => v.Animal)
                .Include(v => v.AssignedVet)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (visit == null)
            {
                return NotFound();
            }

            var dto = visit.ToReceptionistDto();
            return View(dto);
        }

        // POST: Visits/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var visit = await _context.Visits.FindAsync(id);
            if (visit != null)
            {
                _context.Visits.Remove(visit);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VisitExists(int id)
        {
            return _context.Visits.Any(e => e.Id == id);
        }

        private async Task<List<User>> GetVetUsersAsync()
        {
            return await (from user in _context.Users
                        join userRole in _context.UserRoles on user.Id equals userRole.UserId
                        join role in _context.Roles on userRole.RoleId equals role.Id
                        where role.Name == "Vet"
                        select user).ToListAsync();
        }
    }
}