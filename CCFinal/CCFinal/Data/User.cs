using Microsoft.AspNetCore.Identity;

namespace CCFinal.Data;

public class User : IdentityUser {
    public bool Canvas { get; set; }
}