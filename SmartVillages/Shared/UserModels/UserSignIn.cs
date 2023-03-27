using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartVillages.Shared.UserModels
{
    public class UserSignIn
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "Molimo unesite email"), EmailAddress]
        public string Email { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Molimo unesite lozinku")]
        [StringLength(30, ErrorMessage = "Potrebno je unijeti barem 8 znakova", MinimumLength = 8)]
        public string Password { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Molimo unesite tajni kod")]
        public string SecretCode { get; set; }
    }
}
