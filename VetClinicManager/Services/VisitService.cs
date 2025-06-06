using Microsoft.EntityFrameworkCore;
using VetClinicManager.Data;
using VetClinicManager.Mappers;
using VetClinicManager.Models;
using VetClinicManager.DTOs.Visits;

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
                .ToListAsync();

            return _visitMapper.ToReceptionistDtos(visits);
        }

        public async Task<IEnumerable<VisitListVetDto>> GetVisitsForVetAsync(string vetUserId)
        {
            var visits = await _context.Visits
                .Where(v => v.AssignedVetId == vetUserId)
                .Include(v => v.Animal)
                    .ThenInclude(a => a.User)
                .ToListAsync();

            return _visitMapper.ToVetDtos(visits);
        }

        public async Task<IEnumerable<VisitListUserDto>> GetVisitsForOwnerAnimalsAsync(string ownerUserId)
        {
            var visits = await _context.Visits
                .Where(v => v.Animal.UserId == ownerUserId)
                .Include(v => v.Animal)
                .Include(v => v.AssignedVet)
                .ToListAsync();

            return _visitMapper.ToUserDtos(visits);
        }
        
        // To nie są pełne DTOs szczegółów i brakuje w nich wielu danych (np. pełnych aktualizacji)

        public async Task<VisitListReceptionistDto?> GetVisitDetailsForReceptionistAsync(int id)
        {
             var visit = await _context.Visits
                 .Include(v => v.Animal)
                 .Include(v => v.AssignedVet)
                 .FirstOrDefaultAsync(v => v.Id == id);

             if (visit == null) return null;

             return _visitMapper.ToReceptionistDto(visit);
        }

        public async Task<VisitListVetDto?> GetVisitDetailsForVetAsync(int id, string userId)
        {
             var visit = await _context.Visits
                 .Include(v => v.Animal)
                     .ThenInclude(a => a.User)
                 // TODO: Dodaj filtr WHERE jeśli Weterynarz widzi tylko szczegóły WIZYT DO KTÓRYCH JEST PRZYPISANY
                 .FirstOrDefaultAsync(v => v.Id == id /* && v.AssignedVetId == userId */);

             if (visit == null) return null;

             return _visitMapper.ToVetDto(visit);
        }

        public async Task<VisitListUserDto?> GetVisitDetailsForUserAsync(int id, string userId)
        {
             var visit = await _context.Visits
                 .Include(v => v.Animal)
                 .Include(v => v.AssignedVet)
                 .FirstOrDefaultAsync(v => v.Id == id && v.Animal.UserId == userId);

             if (visit == null) return null;

             return _visitMapper.ToUserDto(visit);
        }

        // TODO: Nadal wymagają uzupełnienia implementacji i przemyślenia logiki uprawnień
        public async Task CreateVisitAsync(VisitCreateDto createVisitDto) { throw new NotImplementedException(); }
        public async Task UpdateVisitAsync(int id, VisitEditDto visitEditDto, string currentUserId, bool isVet) { throw new NotImplementedException(); }
        public async Task DeleteVisitAsync(int id) { throw new NotImplementedException(); }
        public async Task<VisitEditDto> GetVisitForEditAsync(int id, string currentUserId) { throw new NotImplementedException(); }

        // TODO: Przenieś GetVetUsersAsync do dedykowanego serwisu użytkowników (IUserService)
        public async Task<IEnumerable<User>> GetVetUsersAsync() { throw new NotImplementedException(); }
        // private async Task<bool> IsUserVetAsync(string userId) { throw new NotImplementedException(); }
        // TODO: Dodaj helper IsUserInRoleAsync jeśli potrzebujesz
    }
}