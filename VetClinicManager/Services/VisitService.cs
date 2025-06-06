using Microsoft.EntityFrameworkCore;
using VetClinicManager.Data;
using VetClinicManager.DTOs.VisitDTOs;
using VetClinicManager.DTOs.Visits.VisitBriefs;
using VetClinicManager.Mappers;
using VetClinicManager.Models;

namespace VetClinicManager.Services
{
    public class VisitService : IVisitService
    {
        private readonly ApplicationDbContext _context;
        private readonly VisitMapper _visitMapper;

        public VisitService(ApplicationDbContext context, VisitMapper visitMapper)
        {
            _context = context;
            _visitMapper = visitMapper;
        }

        public async Task<IEnumerable<VisitListReceptionistDto>> GetVisitsForReceptionistAsync()
        {
            var visits = await _context.Visits
                .Include(v => v.Animal)
                .Include(v => v.AssignedVet)
                .Include(v => v.Updates)
                    .ThenInclude(u => u.AnimalMedications)
                        .ThenInclude(am => am.Medication)
                .ToListAsync();

            return visits.Select(v => _visitMapper.ToReceptionistDto(v));
        }

        public async Task<IEnumerable<VisitListVetDto>> GetVisitsForVetAsync(string userId)
        {
            var visits = await _context.Visits
                .Include(v => v.Animal)
                .Include(v => v.AssignedVet)
                .Include(v => v.Updates)
                    .ThenInclude(u => u.AnimalMedications)
                        .ThenInclude(am => am.Medication)
                .Where(v => v.AssignedVetId == userId)
                .ToListAsync();

            return visits.Select(v => _visitMapper.ToVetDto(v));
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

        // Pozostałe metody pozostają bez zmian (Create/Update/Delete nie wymagają modyfikacji)
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
                CreatedDate = visit.CreatedDate,
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
            var user = await _context.Users.FindAsync(userId);
            return await _context.UserRoles.AnyAsync(ur => 
                ur.UserId == userId && 
                ur.RoleId == _context.Roles.FirstOrDefault(r => r.Name == "Vet").Id);
        }
    }
}