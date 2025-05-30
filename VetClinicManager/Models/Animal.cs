namespace VetClinicManager.Models;

public class Animal
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Species { get; set; }
    public string Breed { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; }
    public string ImageUrl { get; set; }
    public string UserId { get; set; }
    public User User { get; set; }

    public ICollection<Order> MedicalOrders { get; set; } = new List<Order>();
}