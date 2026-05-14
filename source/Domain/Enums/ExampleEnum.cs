using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FSP.Api.Domain.Enums
{
    public enum TipoOperacao
    {
        [Display(Name = "Importação")]
        Importação,
        
        [Display(Name = "Exportação")]
        Exportação
    }
}