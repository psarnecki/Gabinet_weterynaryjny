using System.ComponentModel.DataAnnotations;
using VetClinicManager.Models;

namespace VetClinicManager.DTOs.AnimalMedications;

public class AnimalMedicationEditVetDto
{
    public int Id { get; set; }
    
    public int MedicationId { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime? EndDate { get; set; }
}