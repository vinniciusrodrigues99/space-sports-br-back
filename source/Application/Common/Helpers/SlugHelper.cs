using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace FSP.Api.Application.Common.Helpers
{
    public static class SlugHelper
    {
        public static string Generate(string titulo)
        {
            var normalized = titulo.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder();

            foreach (var c in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    builder.Append(c);
            }

            var slug = builder.ToString()
                .Normalize(NormalizationForm.FormC)
                .ToLowerInvariant()
                .Replace(" ", "-");

            slug = Regex.Replace(slug, @"[^a-z0-9\-]", "");
            slug = Regex.Replace(slug, @"-{2,}", "-");
            slug = slug.Trim('-');

            return slug;
        }

        public static int CalcularMinutosLeitura(string conteudo)
        {
            var palavras = conteudo.Split([' ', '\n', '\r', '\t'], StringSplitOptions.RemoveEmptyEntries).Length;
            return Math.Max(2, (int)Math.Ceiling(palavras / 220.0));
        }
    }
}
