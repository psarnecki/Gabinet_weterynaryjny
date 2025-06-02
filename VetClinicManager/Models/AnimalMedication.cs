using System.ComponentModel.DataAnnotations;
namespace VetClinicManager.Models;

public class AnimalMedication
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public int AnimalId { get; set; } 
    public Animal Animal { get; set; }
    
    [Required]
    public int MedicationId { get; set; }
    public Medication Medication { get; set; }
    
    [Required]
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    
    public int? VisitUpdateId { get; set; }
    public VisitUpdate? VisitUpdate { get; set; }
}