using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CollectionStore.ViewModels
{
    public class SignUpViewModel
    {
        [Required(ErrorMessage = "Required")]
        [RegularExpression(@"^\w*\d*_*$", ErrorMessage = "UserNameSyntaxError")]
        [DataType(DataType.Text)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "PasswordMismatch")]
        public string PasswordConfirm { get; set; }
    }
}
