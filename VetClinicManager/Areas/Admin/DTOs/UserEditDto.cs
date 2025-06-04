using System.ComponentModel.DataAnnotations;

namespace VetClinicManager.Areas.Admin.DTOs;

public class UserEditDto
{
    [Required]
    public string Id { get; set; }
    
    [Required(ErrorMessage = "Adres email jest wymagany.")]
    [EmailAddress(ErrorMessage = "Podaj prawidłowy adres email.")]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Imię jest wymagane.")]
    [MaxLength(80)]
    [Display(Name = "Imię")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Nazwisko jest wymagane.")]
    [MaxLength(80)]
    [Display(Name = "Nazwisko")]
    public string LastName { get; set; }

    [Display(Name = "Specjalizacja")]
    [MaxLength(80)]
    public string? Specialization { get; set; }

    public List<string> AvailableRoles { get; set; } = new List<string>();

    [Display(Name = "Role")]
    public List<string> SelectedRoles { get; set; } = new List<string>();
}