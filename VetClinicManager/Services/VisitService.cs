using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VetClinicManager.Data;
using VetClinicManager.DTOs.Visits;
using VetClinicManager.Mappers;
using VetClinicManager.Models;

namespace VetClinicManager.Services;

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

    private IQueryable<Visit> GetBaseVisitQuery()
    {
        return _context.Visits.AsNoTracking()
            .Include(v => v.Animal).ThenInclude(a => a.User)
            .Include(v => v.AssignedVet)
            .Include(v => v.Updates).ThenInclude(u => u.UpdatedBy)
            .Include(v => v.Updates).ThenInclude(u => u.AnimalMedications).ThenInclude(am => am.Medication);
    }
    
    public async Task<IEnumerable<VisitListReceptionistDto>> GetVisitsForReceptionistAsync()
    {
        var visits = await GetBaseVisitQuery().OrderByDescending(v => v.CreatedDate).ToListAsync();
        return _visitMapper.ToReceptionistDtos(visits);
    }

    public async Task<IEnumerable<VisitListVetDto>> GetVisitsForVetAsync(string vetId)
    {
        var visits = await GetBaseVisitQuery().Where(v => v.AssignedVetId == vetId)
            .OrderByDescending(v => v.CreatedDate).ToListAsync();
        return _visitMapper.ToVetDtos(visits);
    }

    public async Task<IEnumerable<VisitListUserDto>> GetVisitsForOwnerAsync(string ownerId)
    {
        var visits = await GetBaseVisitQuery().Where(v => v.Animal.UserId == ownerId)
            .OrderByDescending(v => v.CreatedDate).ToListAsync();
        return _visitMapper.ToUserDtos(visits);
    }

    public async Task<VisitListReceptionistDto?> GetDetailsForReceptionistAsync(int id)
    {
        var visit = await GetBaseVisitQuery().FirstOrDefaultAsync(v => v.Id == id);
        return visit == null ? null : _visitMapper.ToReceptionistDto(visit);
    }

    public async Task<VisitListVetDto?> GetDetailsForVetAsync(int id, string vetId)
    {
        var visit = await GetBaseVisitQuery().FirstOrDefaultAsync(v => v.Id == id);
        if (visit == null || visit.AssignedVetId != vetId)
        {
            throw new UnauthorizedAccessException();
        }
        return _visitMapper.ToVetDto(visit);
    }

    public async Task<VisitListUserDto?> GetDetailsForUserAsync(int id, string ownerId)
    {
        var visit = await GetBaseVisitQuery().FirstOrDefaultAsync(v => v.Id == id);
        if (visit == null || visit.Animal.UserId != ownerId)
        {
            throw new UnauthorizedAccessException();
        }
        return _visitMapper.ToUserDto(visit);
    }

    public async Task CreateAsync(VisitCreateDto createDto)
    {
        var visit = _visitMapper.ToEntity(createDto);
        visit.CreatedDate = DateTime.UtcNow;
        _context.Visits.Add(visit);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(int id, VisitEditDto editDto, string userId, bool isVet)
    {
        var visit = await _context.Visits.FindAsync(id);
        if (visit == null) throw new KeyNotFoundException("Nie znaleziono wizyty.");
        if (isVet && visit.AssignedVetId != userId) throw new UnauthorizedAccessException();
        
        _visitMapper.ToEntity(editDto, visit);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var visit = await _context.Visits.FindAsync(id);
        if (visit == null) throw new KeyNotFoundException("Nie znaleziono wizyty.");
        _context.Visits.Remove(visit);
        await _context.SaveChangesAsync();
    }

    public async Task<VisitEditDto?> GetForEditAsync(int id, string userId, bool isVet)
    {
        var visit = await GetBaseVisitQuery().FirstOrDefaultAsync(v => v.Id == id);
        if (visit == null) return null;
        if (isVet && visit.AssignedVetId != userId) throw new UnauthorizedAccessException();
        
        return _visitMapper.ToEditDto(visit);
    }

    public async Task<VisitListReceptionistDto?> GetForDeleteAsync(int id)
    {
        return await GetDetailsForReceptionistAsync(id);
    }

    public async Task<IEnumerable<SelectListItem>> GetAnimalsSelectListAsync()
    {
        return await _context.Animals.AsNoTracking().OrderBy(a => a.Name)
            .Select(a => new SelectListItem { Value = a.Id.ToString(), Text = $"{a.Name} ({a.Species})" })
            .ToListAsync();
    }

    public async Task<IEnumerable<User>> GetVetUsersAsync()
    {
        return await _userManager.GetUsersInRoleAsync("Vet");
    }
}