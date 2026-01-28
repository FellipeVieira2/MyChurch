
using System.ComponentModel.DataAnnotations;

namespace MyChurch.Domain.Enum
{
    public enum MaritalStatus
    {
        [Display(Name = "Solteiro(a)")]
        Solteiro,
        [Display(Name = "Casado(a)")]
        Casado,
        [Display(Name = "Divorciado(a)")]
        Divorciado,
        [Display(Name = "Viuvo(a)")]
        Viuvo,
        [Display(Name = "União Estavel")]
        UniaoEstavel,

    }
}
