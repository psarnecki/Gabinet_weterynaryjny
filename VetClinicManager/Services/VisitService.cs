using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VetClinicManager.Data;
using VetClinicManager.DTOs.Visits;
using VetClinicManager.Mappers;
using VetClinicManager.Models;
using QuestPDF.Fluent;
using System.Linq;

namespace VetClinicManager.Services;

public class VisitService : IVisitService
{
    private readonly ApplicationDbContext _context;
    private readonly VisitMapper _visitMapper;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<VisitService> _logger;
    
    public VisitService(ApplicationDbContext context, VisitMapper visitMapper, UserManager<User> userManager, ILogger<VisitService> logger)
    {
        _context = context;
        _visitMapper = visitMapper;
        _userManager = userManager;
        _logger = logger;
    }

    private IQueryable<Visit> GetBaseVisitQuery()
    {
        return _context.Visits.AsNoTracking()
            .Include(v => v.Animal).ThenInclude(a => a.User)
            .Include(v => v.AssignedVet)
            .Include(v => v.Updates).ThenInclude(u => u.UpdatedBy)
            .Include(v => v.Updates).ThenInclude(u => u.AnimalMedications).ThenInclude(am => am.Medication);
    }
    
    public async Task<IEnumerable<VisitListReceptionistDto>> GetVisitsForReceptionistAsync(string sortOrder)
    {
        var visitsQuery = GetBaseVisitQuery();
    
        visitsQuery = SortVisits(visitsQuery, sortOrder);

        var visits = await visitsQuery.ToListAsync();
        return _visitMapper.ToReceptionistDtos(visits);
    }

    public async Task<IEnumerable<VisitListVetDto>> GetVisitsForVetAsync(string vetId, string sortOrder)
    {
        var visitsQuery = GetBaseVisitQuery().Where(v => v.AssignedVetId == vetId);
    
        visitsQuery = SortVisits(visitsQuery, sortOrder);

        var visits = await visitsQuery.ToListAsync();
        return _visitMapper.ToVetDtos(visits);
    }

    public async Task<IEnumerable<VisitListUserDto>> GetVisitsForOwnerAsync(string ownerId, string sortOrder)
    {
        var visitsQuery = GetBaseVisitQuery().Where(v => v.Animal.UserId == ownerId);
    
        visitsQuery = SortVisits(visitsQuery, sortOrder);

        var visits = await visitsQuery.ToListAsync();
    
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
        
        visit.Title = editDto.Title;
        visit.Description = editDto.Description;
        visit.Status = editDto.Status;
        visit.Priority = editDto.Priority;
        visit.AssignedVetId = editDto.AssignedVetId;
        
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

    public async Task<byte[]?> GeneratePdfReportAsync(int visitId, string userId, IEnumerable<string> userRoles)
    {
        try
        {
            VisitReportDto? reportDto = null;

            if (userRoles.Contains("Admin") || userRoles.Contains("Receptionist"))
            {
                var visitData = await GetDetailsForReceptionistAsync(visitId);
                if (visitData == null) return null;
                reportDto = new VisitReportDto
                {
                    Title = visitData.Title, Description = visitData.Description,
                    CreatedDate = visitData.CreatedDate,
                    Status = visitData.Status, Priority = visitData.Priority, Animal = visitData.Animal,
                    Owner = visitData.Owner, AssignedVet = visitData.AssignedVet, Updates = visitData.Updates
                };
            }
            else if (userRoles.Contains("Vet"))
            {
                var visitData = await GetDetailsForVetAsync(visitId, userId);
                if (visitData == null) return null;
                reportDto = new VisitReportDto
                {
                    Title = visitData.Title, Description = visitData.Description,
                    CreatedDate = visitData.CreatedDate,
                    Status = visitData.Status, Priority = visitData.Priority, Animal = visitData.Animal,
                    Owner = visitData.Owner, Updates = visitData.Updates
                };
            }
            else if (userRoles.Contains("Client"))
            {
                var visitData = await GetDetailsForUserAsync(visitId, userId);
                if (visitData == null) return null;
                reportDto = new VisitReportDto
                {
                    Title = visitData.Title, Description = visitData.Description,
                    CreatedDate = visitData.CreatedDate,
                    Status = visitData.Status, Animal = visitData.Animal, AssignedVet = visitData.AssignedVet,
                    Updates = visitData.Updates
                };
            }

            if (reportDto == null)
            {
                return null;
            }

            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;
            var reportDocument = new VisitPdfReport(reportDto);
            return reportDocument.GeneratePdf();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Wystąpił błąd podczas próby generowania raportu.");
            return null;
        }
    }
    
    public async Task<IEnumerable<Visit>> GetOpenVisitsForReportAsync()
    {
        return await _context.Visits
            .AsNoTracking()
            .Include(v => v.Animal).ThenInclude(a => a.User)
            .Include(v => v.AssignedVet)
            .Where(v => v.Status == Models.Enums.VisitStatus.Scheduled || v.Status == Models.Enums.VisitStatus.InProgress)
            .OrderBy(v => v.CreatedDate)
            .ToListAsync();
    }
    
    private IQueryable<Visit> SortVisits(IQueryable<Visit> visits, string sortOrder)
    {
        switch (sortOrder)
        {
            case "title_desc":
                return visits.OrderByDescending(v => v.Title);
            case "Date":
                return visits.OrderBy(v => v.CreatedDate);
            case "date_desc":
                return visits.OrderByDescending(v => v.CreatedDate);
            case "Animal":
                return visits.OrderBy(v => v.Animal.Name);
            case "animal_desc":
                return visits.OrderByDescending(v => v.Animal.Name);
            case "Owner":
                return visits.OrderBy(v => v.Animal.User.LastName).ThenBy(v => v.Animal.User.FirstName);
            case "owner_desc":
                return visits.OrderByDescending(v => v.Animal.User.LastName).ThenByDescending(v => v.Animal.User.FirstName);
            case "Status":
                return visits.OrderBy(v => v.Status);
            case "status_desc":
                return visits.OrderByDescending(v => v.Status);
            case "Priority":
                return visits.OrderBy(v => v.Priority);
            case "priority_desc":
                return visits.OrderByDescending(v => v.Priority);
            case "Vet":
                return visits.OrderBy(v => v.AssignedVet.LastName).ThenBy(v => v.AssignedVet.FirstName);
            case "vet_desc":
                return visits.OrderByDescending(v => v.AssignedVet.LastName).ThenByDescending(v => v.AssignedVet.FirstName);
            default:
                return visits.OrderBy(v => v.Title);
        }
    }
}