using System.ComponentModel.DataAnnotations;
using VetClinicManager.Models;

namespace VetClinicManager.DTOs.AnimalMedications;

public class AnimalMedicationCreateVetDto
{
    public int Id { get; set; }
    
    public int AnimalId { get; set; } 
    public int MedicationId { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; }
    
    public int? VisitUpdateId { get; set; }
}
