using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace goldStore.Models.ViewModel
{
    public class Login
    {   
        [Display(Name ="Eposta")]
        [DataType(DataType.EmailAddress,ErrorMessage ="Geçerli email adresi giriniz")]
        [Required(ErrorMessage ="Boş bırakılamaz")]
        public string email { get; set; }

        [Display(Name = "Parola")]
        [DataType(DataType.Password)]
        [MinLength(6,ErrorMessage ="en az 6 hane")]
        [Required(ErrorMessage = "Boş bırakılamaz")]
        public string password { get; set; }

        [Display(Name = "Beni Hatırla")]
        public bool rememberMe { get; set; }

    }
}