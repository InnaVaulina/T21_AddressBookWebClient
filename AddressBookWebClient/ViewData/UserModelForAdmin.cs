using System.ComponentModel.DataAnnotations;

namespace AddressBookWebClient.ViewData
{
    public class UserModelForAdmin
    {
        [Required]
        [Display(Name = "ID")]
        public string Id { get; set; }

        [Required]
        [Display(Name = "Имя пользователя")]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Роль")]
        public string UserRole { get; set; }
    }

}
