using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FSP.Api.Domain.Enums
{
    public enum Perfil
    {
        [Display(Name = "Administrador")]
        Administrador, 
        [Display(Name = "Aprovador")]
        Aprovador, 
        [Display(Name = "Diretoria")]
        Diretoria
    }
}