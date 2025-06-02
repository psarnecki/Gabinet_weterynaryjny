using System.ComponentModel.DataAnnotations;
namespace VetClinicManager.Models;

public class VisitUpdate {
    [Key] 
    public int Id { get; set; }

    [MaxLength(5000)]
    public string? Notes { get; set; }
    public DateTime UpdateDate { get; set; } = DateTime.Now;
    
    [MaxLength(500)]
    public string? ImageUrl { get; set; }
    
    [MaxLength(500)]
    public string? PrescribedMedications { get; set; }
    
    [Required]
    public int VisitId { get; set; }
    public Visit Visit { get; set; }

    [Required]
    [MaxLength(500)] 
    public string UpdatedByVetId { get; set; }
    public User UpdatedBy { get; set; } // weterynarz
}