using PdfTools.Data;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace PdfTools.Helpers
{
    public static class Json
    {
        private static readonly JsonSerializerOptions options = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private static readonly JsonSerializerOptions optionsUnsafe = new()
        {
            PropertyNameCaseInsensitive = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public static string GetJson(object model) => JsonSerializer.Serialize(model, options);
        public static T? GetModel<T>(string json) => JsonSerializer.Deserialize<T>(json, options);
        public static string ToCamelCase(string s) => JsonNamingPolicy.CamelCase.ConvertName(s);

        public static bool TryGetModel<T>(string json, [MaybeNullWhen(false)] out T data)
        {
            try
            {
                data = JsonSerializer.Deserialize<T>(json, options);
                if (data is not null)
                    return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            data = default;
            return false;
        }

        public static bool TryGetTranslation(string json, [MaybeNullWhen(false)] out Translation data)
        {
            try
            {
                data = JsonSerializer.Deserialize<Translation>(json, optionsUnsafe);
                if (data is not null)
                    return true;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            data = default;
            return false;
        }
    }
}
