using Microsoft.AspNetCore.Identity;

namespace AddressBookWebClient.Models
{
    public class AppUser : IdentityUser
    {

        public string Token {  get; set; }


    }

}
