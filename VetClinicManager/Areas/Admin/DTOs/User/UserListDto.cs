namespace VetClinicManager.Areas.Admin.DTOs.User;

public class UserListDto
{
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public List<string> Roles { get; set; } = new List<string>();
}