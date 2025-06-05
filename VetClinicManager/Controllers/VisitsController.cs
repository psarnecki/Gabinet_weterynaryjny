using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using VetClinicManager.DTOs.VisitDTOs;
using VetClinicManager.Models;
using VetClinicManager.Services;

namespace VetClinicManager.Controllers
{
    [Authorize]
    public class VisitsController : Controller
    {
        private readonly IVisitService _visitService;
        private readonly UserManager<User> _userManager;

        public VisitsController(
            IVisitService visitService,
            UserManager<User> userManager)
        {
            _visitService = visitService;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin,Receptionist,Vet")]
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var isReceptionist = await _userManager.IsInRoleAsync(currentUser, "Admin") || 
                                await _userManager.IsInRoleAsync(currentUser, "Receptionist");
            var isVet = await _userManager.IsInRoleAsync(currentUser, "Vet");

            try
            {
                if (isReceptionist)
                {
                    var receptionistDtos = await _visitService.GetVisitsForReceptionistAsync();
                    return View("IndexReceptionist", receptionistDtos);
                }
                else if (isVet)
                {
                    var vetDtos = await _visitService.GetVisitsForVetAsync(currentUser.Id);
                    return View("IndexVet", vetDtos);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Error");
            }

            return Forbid();
        }

        [Authorize(Roles = "Admin,Receptionist,Vet,Client")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            var isReceptionist = await _userManager.IsInRoleAsync(currentUser, "Admin") ||
                                await _userManager.IsInRoleAsync(currentUser, "Receptionist");
            var isVet = await _userManager.IsInRoleAsync(currentUser, "Vet");
            var isClient = await _userManager.IsInRoleAsync(currentUser, "Client");

            try
            {
                if (isReceptionist)
                {
                    var dto = await _visitService.GetVisitDetailsForReceptionistAsync(id.Value);
                    return View("DetailsReceptionist", dto);
                }
                else if (isVet)
                {
                    var vetDto = await _visitService.GetVisitDetailsForVetAsync(id.Value, currentUser.Id);
                    return View("DetailsVet", vetDto);
                }
                else if (isClient)
                {
                    var userDto = await _visitService.GetVisitDetailsForUserAsync(id.Value, currentUser.Id);
                    return View("DetailsUser", userDto);
                }
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Error");
            }

            return Forbid();
        }

        [Authorize(Roles = "Admin,Receptionist")]
        public async Task<IActionResult> Create()
        {
            try
            {
                var vetUsers = await _visitService.GetVetUsersAsync();
                ViewData["AssignedVetId"] = new SelectList(vetUsers, "Id", "Email");
                return View(new VisitCreateDto { CreatedDate = DateTime.Now });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Error");
            }
        }

        [Authorize(Roles = "Admin,Receptionist")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VisitCreateDto createVisitDto)
        {
            if (!ModelState.IsValid)
            {
                var vetUsers = await _visitService.GetVetUsersAsync();
                ViewData["AssignedVetId"] = new SelectList(vetUsers, "Id", "Email", createVisitDto.AssignedVetId);
                return View(createVisitDto);
            }

            try
            {
                await _visitService.CreateVisitAsync(createVisitDto);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                var vetUsers = await _visitService.GetVetUsersAsync();
                ViewData["AssignedVetId"] = new SelectList(vetUsers, "Id", "Email", createVisitDto.AssignedVetId);
                return View(createVisitDto);
            }
        }

        [Authorize(Roles = "Admin,Receptionist,Vet")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var visitEditDto = await _visitService.GetVisitForEditAsync(id.Value, currentUser.Id);

                var vetUsers = await _visitService.GetVetUsersAsync();
                ViewData["AssignedVetId"] = new SelectList(vetUsers, "Id", "Email", visitEditDto.AssignedVetId);
                return View(visitEditDto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Error");
            }
        }

        [Authorize(Roles = "Admin,Receptionist,Vet")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, VisitEditDto visitEditDto)
        {
            if (id != visitEditDto.Id) return NotFound();

            if (!ModelState.IsValid)
            {
                var vetUsers = await _visitService.GetVetUsersAsync();
                ViewData["AssignedVetId"] = new SelectList(vetUsers, "Id", "Email", visitEditDto.AssignedVetId);
                return View(visitEditDto);
            }

            try
            {
                var currentUser = await _userManager.GetUserAsync(User);
                var isVet = await _userManager.IsInRoleAsync(currentUser, "Vet");
                await _visitService.UpdateVisitAsync(id, visitEditDto, currentUser.Id, isVet);
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                var vetUsers = await _visitService.GetVetUsersAsync();
                ViewData["AssignedVetId"] = new SelectList(vetUsers, "Id", "Email", visitEditDto.AssignedVetId);
                return View(visitEditDto);
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            try
            {
                var dto = await _visitService.GetVisitDetailsForReceptionistAsync(id.Value);
                return View(dto);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Error");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _visitService.DeleteVisitAsync(id);
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View("Error");
            }
        }
    }
}