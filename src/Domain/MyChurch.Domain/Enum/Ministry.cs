using System.ComponentModel.DataAnnotations;

namespace MyChurch.Domain.Enum
{
    public enum Ministry
    {
        [Display(Name = "Sem Ministério")]
        None,
        [Display(Name = "Louvor")]
        Worship,
        [Display(Name = "Infantil")]
        Children,
        [Display(Name = "Jovens")]
        Youth,
        [Display(Name = "Evangelismo")]
        Evangelism,
        [Display(Name = "Diaconia")]
        Deaconry,
        // Adicione outros ministérios conforme necessário
    }
}