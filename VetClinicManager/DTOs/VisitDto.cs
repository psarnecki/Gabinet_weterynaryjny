using VetClinicManager.Models.Enums;
using System.Collections.Generic;

namespace VetClinicManager.Dtos
{
    public class VisitDtoReceptionist
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public VisitStatus Status { get; set; }
        public VisitPriority Priority { get; set; }
        public int AnimalId { get; set; }
        public AnimalBriefDto? Animal { get; set; }
        public string? AssignedVetId { get; set; }
        public  VetBriefDto? AssignedVet { get; set; }
        public List<VisitUpdateDto> Updates { get; set; } = new();
    }
    
    public class VisitDtoVet
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public VisitStatus Status { get; set; }
        public VisitPriority Priority { get; set; }
        public int AnimalId { get; set; }
        public AnimalBriefDto? Animal { get; set; }
    }
    
    public class VisitDtoUser
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public VisitStatus Status { get; set; }
        public int AnimalId { get; set; }
        public AnimalBriefDto? Animal { get; set; }
        public  VetBriefDto? AssignedVet { get; set; }
    }
    
    public class CreateVisitDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public VisitStatus Status { get; set; }
        public VisitPriority Priority { get; set; }
        public int AnimalId { get; set; }
        public AnimalBriefDto? Animal { get; set; }
        public string? AssignedVetId { get; set; }
        public  VetBriefDto? AssignedVet { get; set; }
        public List<VisitUpdateDto> Updates { get; set; } = new();
    }
    
    public class UpdateVisitDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public VisitStatus Status { get; set; }
        public VisitPriority Priority { get; set; }
        public AnimalBriefDto? Animal { get; set; }
        public string? AssignedVetId { get; set; }
        public  VetBriefDto? AssignedVet { get; set; }
        public List<VisitUpdateDto> Updates { get; set; } = new();
    }
    
    public class AnimalBriefDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Breed { get; set; } = string.Empty;
    }

    public class VetBriefDto
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class VisitUpdateDto
    {
        public int Id { get; set; }
        public string Notes { get; set; } = string.Empty;
        public DateTime UpdateDate { get; set; }
    }
    
}