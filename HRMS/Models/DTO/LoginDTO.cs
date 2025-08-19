using System.ComponentModel.DataAnnotations;

public class LoginDTO
{
    [Required] public string Email { get; set; } = null!;
    [Required] public string Password { get; set; } = null!;
    public bool RememberMe { get; set; } = false;
}