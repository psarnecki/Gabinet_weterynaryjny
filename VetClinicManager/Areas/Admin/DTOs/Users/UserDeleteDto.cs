using System.ComponentModel.DataAnnotations;

namespace VetClinicManager.Areas.Admin.DTOs.Users;

public class UserDeleteDto
{
    [Required]
    public string Id { get; set; }

    [Display(Name = "Imię")]
    public string FirstName { get; set; } = string.Empty;

    [Display(Name = "Nazwisko")]
    public string LastName { get; set; } = string.Empty;

    [Display(Name = "Email")]
    public string Email { get; set; } = string.Empty;
}