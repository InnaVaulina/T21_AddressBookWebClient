using System.ComponentModel.DataAnnotations;

namespace AddressBookWebClient.ViewData
{
    public interface IEntryViewModel 
    {
        [Required]
        [Display(Name = "Логин")]
        public string LoginProp { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }
    }




    public class EntryViewModel: IEntryViewModel
    {
        string loginProp;
        string password;
 
        public string LoginProp { get; set; }
    
        public string Password { get; set; }

        public EntryViewModel() 
        {
            loginProp = "";
            password = "";
        }
       
    }



    public class UserModel
    {

        public string UserName { get; set; }

        public string UserRole { get; set; }

        public string Token { get; set; }


    }
}
