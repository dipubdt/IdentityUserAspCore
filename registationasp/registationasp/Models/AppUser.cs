using Microsoft.AspNetCore.Identity;

namespace registationasp.Models;

public class AppUser: IdentityUser
{

    public string Name { get; set; }
    public string Address { get; set; }

}
