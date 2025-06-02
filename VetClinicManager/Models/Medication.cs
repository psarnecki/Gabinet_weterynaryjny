namespace VetClinicManager.Models;

public class Medication
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<AnimalMedication> AnimalMedications { get; set; }
}