using Microsoft.AspNetCore.Identity;

namespace VetClinicManager.Models;

public class User : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Specialization { get; set; }
    public ICollection<Order> AssignedOrders { get; set; } = new List<Order>();
}