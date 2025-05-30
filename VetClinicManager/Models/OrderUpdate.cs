namespace VetClinicManager.Models;

public class OrderUpdate
{
    public int Id { get; set; }
    public string Notes { get; set; }
    public DateTime UpdateDate { get; set; }
    public string ImageUrl { get; set; }
    public string PrescribedMedications { get; set; }

    public int MedicalOrderId { get; set; }
    public Order MedicalOrder { get; set; }
}