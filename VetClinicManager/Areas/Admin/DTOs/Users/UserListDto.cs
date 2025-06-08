using System.ComponentModel.DataAnnotations;

namespace VetClinicManager.Areas.Admin.DTOs.Users;

public class UserListDto
{
    public string Id { get; set; }
    
    [Display(Name = "Imię")] 
    public string FirstName { get; set; }
    
    [Display(Name = "Nazwisko")]
    public string LastName { get; set; }
    
    [Display(Name = "Email")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }
    public List<string> Roles { get; set; } = new List<string>();
}