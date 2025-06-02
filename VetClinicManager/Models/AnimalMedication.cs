namespace VetClinicManager.Models;

public class AnimalMedication
{
    public int Id { get; set; }
    
    public int AnimalId { get; set; } 
    public Animal Animal { get; set; }

    public int MedicationId { get; set; }
    public Medication Medication { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}