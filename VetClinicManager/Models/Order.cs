namespace VetClinicManager.Models;

public class Order
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreatedDate { get; set; }
    public int AnimalId { get; set; }
    public Animal Animal { get; set; }

    public ICollection<OrderUpdate> Updates { get; set; } = new List<OrderUpdate>();
}