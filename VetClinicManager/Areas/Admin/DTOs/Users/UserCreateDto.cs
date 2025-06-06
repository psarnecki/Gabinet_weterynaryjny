using System.ComponentModel.DataAnnotations;

namespace VetClinicManager.Areas.Admin.DTOs.Users;

public class UserCreateDto
{
    [Required(ErrorMessage = "Adres email jest wymagany.")]
    [EmailAddress(ErrorMessage = "Podaj prawidłowy adres email.")]
    [Display(Name = "Email")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Hasło jest wymagane.")]
    [StringLength(100, ErrorMessage = "{0} musi mieć co najmniej {2} i co najwyżej {1} znaków.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Hasło")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Potwierdzenie hasła jest wymagane.")]
    [DataType(DataType.Password)]
    [Display(Name = "Potwierdź hasło")]
    [Compare("Password", ErrorMessage = "Hasła nie pasują do siebie.")]
    public string ConfirmPassword { get; set; }

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