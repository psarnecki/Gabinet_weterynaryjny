namespace VetClinicManager.Models;

public class VisitUpdate {
    public int Id { get; set; }
    public string Notes { get; set; }
    public DateTime UpdateDate { get; set; }
    public string? ImageUrl { get; set; }
    public string? PrescribedMedications { get; set; }

    public int VisitId { get; set; }
    public Visit Visit { get; set; }

    public string UpdatedByVetId { get; set; }
    public User UpdatedBy { get; set; } // weterynarz
}