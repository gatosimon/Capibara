using System;
using System.Globalization;
using System.Text.RegularExpressions;

namespace GeneradorDeCapas.Utilidades
{

    public static class TextoHelper
    {
        /// <summary>
        /// Convierte un texto a formato "Titulo":
        /// - Separa palabras pegadas cuando hay mayúsculas en el medio.
        /// - Capitaliza la primera letra de cada palabra.
        /// </summary>
        public static string FormatearTitulo(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // 1) Insertar espacio antes de mayúsculas internas (ej: "holaMundo" → "hola Mundo")
            string separado = Regex.Replace(input, "([a-z])([A-Z])", "$1 $2");

            // 2) Poner en minúsculas todo, para evitar cosas raras
            separado = separado.ToLowerInvariant();

            // 3) Usar TextInfo para capitalizar cada palabra
            TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
            string resultado = ti.ToTitleCase(separado);

            return resultado;
        }
    }
}