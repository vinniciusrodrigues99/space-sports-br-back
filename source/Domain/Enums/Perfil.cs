using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace FSP.Api.Domain.Enums
{
    public enum Perfil
    {
        [Display(Name = "Administrador")]
        Administrador,
        [Display(Name = "Autor")]
        Autor,
        [Display(Name = "Leitor")]
        Leitor
    }
}