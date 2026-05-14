using System.ComponentModel.DataAnnotations;

namespace FSP.Api.Domain.Enums
{
    public enum CategoriaPost
    {
        [Display(Name = "Futebol")]
        Futebol = 1,

        [Display(Name = "NBA")]
        NBA = 2,

        [Display(Name = "UFC")]
        UFC = 3,

        [Display(Name = "F1")]
        F1 = 4,

        [Display(Name = "Tênis")]
        Tenis = 5
    }
}
