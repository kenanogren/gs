﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace goldStore.Models.ViewModel
{
    public class Register
    {   
        [Display(Name = "Eposta")]
        [Required(ErrorMessage ="Boş Bırakılamaz")]
        [DataType(DataType.EmailAddress,ErrorMessage = "Geçerli bir Email adresi giriniz")]
        public string email { get; set; }

        [Display(Name = "Parola")]
        [Required(ErrorMessage = "Boş Bırakılamaz")]
        [DataType(DataType.Password)]
        [MinLength(6,ErrorMessage ="Minumun 6 karakter girmeiniz gerekir")]
        public string password { get; set; }

        [Display(Name = "Parola Tekrarı")]
        [Required(ErrorMessage = "Boş Bırakılamaz")]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Minumun 6 karakter girmeiniz gerekir")]
        [Compare("password",ErrorMessage ="Parolanız eşleşmemedi")]
        public string confirmPassword { get; set; }
    }
}