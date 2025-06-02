using VetClinicManager.Models.Enums;

namespace VetClinicManager.Models;

public class Visit {
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public VisitStatus Status { get; set; }
    public VisitPriority Priority { get; set; }

    public int AnimalId { get; set; }
    public Animal Animal { get; set; }

    public string? AssignedVetId { get; set; }
    public User? AssignedVet { get; set; } // główny lekarz przypisany do wizyty

    public ICollection<VisitUpdate> Updates { get; set; } = new List<VisitUpdate>();
}