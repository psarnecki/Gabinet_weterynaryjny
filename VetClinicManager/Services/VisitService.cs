using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VetClinicManager.Data;
using VetClinicManager.Mappers;
using VetClinicManager.Models;
using VetClinicManager.DTOs.Visits;
using VetClinicManager.DTOs.Visits.VisitBriefs;

namespace VetClinicManager.Services
{
    public class VisitService : IVisitService
    {
        private readonly ApplicationDbContext _context;
        private readonly VisitMapper _visitMapper;
        private readonly UserManager<User> _userManager;

        public VisitService(ApplicationDbContext context, VisitMapper visitMapper, UserManager<User> userManager)
        {
            _context = context;
            _visitMapper = visitMapper;
            _userManager = userManager;
        }

        public async Task<IEnumerable<VisitListReceptionistDto>> GetVisitsForReceptionistAsync()
        {
            var visits = await _context.Visits
                .Include(v => v.Animal)
                    .ThenInclude(a => a.User) 
                .Include(v => v.AssignedVet)
                .Include(v => v.Updates)
                    .ThenInclude(u => u.AnimalMedications)
                        .ThenInclude(am => am.Medication)
                .ToListAsync();

            return _visitMapper.ToReceptionistDtos(visits);
        }

        public async Task<IEnumerable<VisitListVetDto>> GetVisitsForVetAsync(string vetUserId)
        {
            var visits = await _context.Visits
                .Where(v => v.AssignedVetId == vetUserId)
                .Include(v => v.Animal)
                    .ThenInclude(a => a.User)
                .Include(v => v.AssignedVet)
                .Include(v => v.Updates)
                    .ThenInclude(u => u.AnimalMedications)
                        .ThenInclude(am => am.Medication)
                .ToListAsync();
            return _visitMapper.ToVetDtos(visits);
        }

        public async Task<IEnumerable<VisitListUserDto>> GetVisitsForOwnerAnimalsAsync(string ownerUserId)
        {
            var visits = await _context.Visits
                .Where(v => v.Animal.UserId == ownerUserId)
                .Include(v => v.Animal)
                .Include(v => v.AssignedVet)
                .Include(v => v.Updates)
                    .ThenInclude(u => u.AnimalMedications)
                        .ThenInclude(am => am.Medication)
                .ToListAsync();

            return _visitMapper.ToUserDtos(visits);
        }

        public async Task<VisitListReceptionistDto> GetVisitDetailsForReceptionistAsync(int id)
        {
            var visit = await GetVisitByIdAsync(id);
            return _visitMapper.ToReceptionistDto(visit);
        }

        public async Task<VisitListVetDto> GetVisitDetailsForVetAsync(int id, string userId)
        {
            var visit = await GetVisitByIdAsync(id);
            
            if (visit.AssignedVetId != userId)
                throw new UnauthorizedAccessException("You are not assigned to this visit");

            return _visitMapper.ToVetDto(visit);
        }

        public async Task<VisitListUserDto> GetVisitDetailsForUserAsync(int id, string userId)
        {
            var visit = await GetVisitByIdAsync(id);
            
            if (visit.Animal?.UserId != userId)
                throw new UnauthorizedAccessException("This is not your animal's visit");

            return _visitMapper.ToUserDto(visit);
        }
        
        public async Task<IEnumerable<SelectListItem>> GetAnimalsSelectListAsync()
        {
            return await _context.Animals
                .AsNoTracking()
                .OrderBy(a => a.Name)
                .Select(a => new SelectListItem
                {
                    Value = a.Id.ToString(),
                    Text = $"{a.Name} ({a.Species})"
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<SelectListItem>> GetVetsSelectListAsync()
        {
            // Zakładamy, że weterynarze to użytkownicy z przypisaną rolą "Vet"
            // To jest bardziej niezawodne niż sprawdzanie pola `Specialization`.
            var vets = await _userManager.GetUsersInRoleAsync("Vet");

            return vets
                .OrderBy(u => u.LastName)
                .Select(u => new SelectListItem
                {
                    Value = u.Id,
                    Text = $"{u.FirstName} {u.LastName}"
                });
        }
        public async Task CreateVisitAsync(VisitCreateDto createVisitDto)
        {
            var visit = _visitMapper.ToEntity(createVisitDto);
            _context.Add(visit);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateVisitAsync(int id, VisitEditDto visitEditDto, string currentUserId, bool isVet)
        {
            var visit = await GetVisitByIdAsync(id);

            if (isVet && visit.AssignedVetId != currentUserId)
                throw new UnauthorizedAccessException("You are not assigned to this visit");

            if (isVet)
            {
                visit.Description = visitEditDto.Description;
                visit.Status = visitEditDto.Status;
            }
            else
            {
                _visitMapper.ToEntity(visitEditDto, visit);
            }

            _context.Update(visit);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteVisitAsync(int id)
        {
            var visit = await GetVisitByIdAsync(id);
            _context.Visits.Remove(visit);
            await _context.SaveChangesAsync();
        }

        public async Task<VisitEditDto> GetVisitForEditAsync(int id, string currentUserId)
        {
            var visit = await GetVisitByIdAsync(id);
            
            var isAssignedVet = visit.AssignedVetId == currentUserId;
            if (await IsUserVetAsync(currentUserId) && !isAssignedVet)
                throw new UnauthorizedAccessException("You are not assigned to this visit");

            return new VisitEditDto
            {
                Id = visit.Id,
                Title = visit.Title,
                Description = visit.Description,
                Status = visit.Status,
                Priority = visit.Priority,
                AssignedVetId = visit.AssignedVetId,
                Animal = new VisitAnimalBriefDto
                {
                    Id = visit.AnimalId,
                    Name = visit.Animal?.Name ?? string.Empty,
                    Breed = visit.Animal?.Breed ?? string.Empty
                }
            };
        }

        public async Task<IEnumerable<User>> GetVetUsersAsync()
        {
            return await (from user in _context.Users
                        join userRole in _context.UserRoles on user.Id equals userRole.UserId
                        join role in _context.Roles on userRole.RoleId equals role.Id
                        where role.Name == "Vet"
                        select user).ToListAsync();
        }

        private async Task<Visit> GetVisitByIdAsync(int id)
        {
            var visit = await _context.Visits
                .Include(v => v.Animal)
                    .ThenInclude(a => a.User) 
                .Include(v => v.AssignedVet)
                .Include(v => v.Updates)
                    .ThenInclude(u => u.AnimalMedications)
                        .ThenInclude(am => am.Medication)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (visit == null)
                throw new KeyNotFoundException("Visit not found");

            return visit;
        }

        private async Task<bool> IsUserVetAsync(string userId)
        {
            return await _context.UserRoles
                .Join(_context.Roles,
                    ur => ur.RoleId,
                    r => r.Id,
                    (ur, r) => new { ur.UserId, RoleName = r.Name })
                .AnyAsync(x => x.UserId == userId && x.RoleName == "Vet");
        }
    }
}