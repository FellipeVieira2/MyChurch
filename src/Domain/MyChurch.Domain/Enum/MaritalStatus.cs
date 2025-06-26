
using System.ComponentModel.DataAnnotations;

namespace MyChurch.Domain.Enum
{
    public enum MaritalStatus
    {
        [Display(Name = "Solteiro")]
        Solteiro,
        [Display(Name = "Casado")]
        Casado,
        [Display(Name = "Divorciado")]
        Divorciado,
        [Display(Name = "Viuvo")]
        Viuvo
    }
}
