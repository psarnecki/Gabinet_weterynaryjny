namespace VetClinicManager.Models;

public class AnimalMedication
{
    public string? VetId { get; set; }
    public User? Vet { get; set; }
    
    public int HealthRecordId { get; set; }
    public Animal HealthRecord { get; set; }

    public int MedicationId { get; set; }
    public Medication Medication { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}