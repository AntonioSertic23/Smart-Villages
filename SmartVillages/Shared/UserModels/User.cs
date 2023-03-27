using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartVillages.Shared.UserModels
{
    public class User
    {
        public int Id { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Molimo unesite ime")]
        [StringLength(20, ErrorMessage = "Ime mora biti manje od 20 znakova")]
        public string FirstName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Molimo unesite prezime")]
        [StringLength(20, ErrorMessage = "Prezime mora biti manje od 20 znakova")]
        public string LastName { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Molimo unesite email"), EmailAddress]
        public string Email { get; set; }
        public string Bio { get; set; }
        public string Sex { get; set; }
        [Required(ErrorMessage = "Molimo unesite svoj oib")]
        [RegularExpression(@"^(?:HR)?(\d{10}(\d))$", ErrorMessage = "Uneseni oib nije pravilnog formata")]
        public string OIB { get; set; }
        public bool EmailConfirm { get; set; } = false;
        public bool Locked { get; set; } = true;
        public Place Place { get; set; }
        [Required(ErrorMessage = "Molimo unesite svoju adresu")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Molimo unesite svoj broj")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Uneseni broj nije pravilnog formata")]
        public string Number { get; set; }
        [Required(AllowEmptyStrings = false, ErrorMessage = "Molimo unesite lozinku")]
        [StringLength(30, ErrorMessage = "Potrebno je unijeti barem 8 znakova", MinimumLength = 8)]
        public string Password { get; set; }
        public string SecretCode { get; set; }
        [Required(ErrorMessage = "Molimo unesite svoj datum rođenja")]
        public DateTime? BirthDate { get; set; }
        public UserType UserType { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        [Range(typeof(bool), "true", "true", ErrorMessage = "You must agree")]
        public bool TermsAndConditions { get; set; }
        public string EmailConfirmationCode { get; set; }
        public UserImage UserImage { get; set; }
    }
}
