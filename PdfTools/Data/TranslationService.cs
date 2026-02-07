using System.Net.Http.Json;

namespace PdfTools.Data
{
    public class TranslationService(HttpClient _hc, MyJsInterop _myJs)
    {
        public Translation I18n { get; set; } = new();
        public List<string> Languages { get; set; } = [
                "en", "de"
            ];

        public async Task InitializeAsync()
        {
            await SetTranslationsAsync(System.Globalization.CultureInfo.CurrentCulture.Name);
        }

        public async Task SetTranslationsAsync(string code)
        {
            if (code.Contains('-'))
            {
                code = code.Split('-')[0];
            }

            if (Languages.Contains(code) == false)
            {
                code = "en";
            }

            var res = await _hc.GetStringAsync($"i18n/{code}.json");
            if (res is not null && Helpers.Json.TryGetTranslation(res, out var i18n))
            {
                await _myJs.SetHtmlLangAsync(code);
                I18n = i18n;
            }
        }
    }
}
