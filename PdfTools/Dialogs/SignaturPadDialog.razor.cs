using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;

namespace PdfTools.Dialogs
{
    public partial class SignaturPadDialog
    {
        [CascadingParameter]
        private IMudDialogInstance? MudDialog { get; set; }

        [Parameter]
        public int Width { get; set; } = 600;

        private const string CanvasId = "signPad";
        private IJSObjectReference _spad = default!;
        private readonly List<Data.Signatur> _signaturs = [];

        private string _name = string.Empty;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _spad = await _js.InvokeConstructorAsync("MySignaturPad", CanvasId);
                var sing = await _spad.InvokeAsync<string?>("getSignaturFromStorage");
                if (sing is not null && Helpers.Json.TryGetModel<IEnumerable<Data.Signatur>>(sing, out var m))
                {
                    _signaturs.AddRange(m);
                }

                StateHasChanged();
            }
        }

        private async Task UploadFiles(IBrowserFile file)
        {
            using var s = file.OpenReadStream(file.Size);
            using MemoryStream ms = new();
            await s.CopyToAsync(ms);

            var data = Convert.ToBase64String(ms.ToArray());

            await Finish(data);
        }

        #region List

        private async Task ListSelected(int index)
        {
            await Finish(_signaturs[index].DataAsBase64);
        }

        private async Task DownloadSignature(int index)
        {
            var sign = _signaturs[index];
            await _js.InvokeVoidAsync("saveAsFile", $"{sign.Name}", sign.DataAsBase64);
        }

        private async Task DeleteSignature(int index)
        {
            _signaturs.RemoveAt(index);
            await _spad.InvokeVoidAsync("setSignaturToStorage", Helpers.Json.GetJson(_signaturs));
        }

        #endregion

        #region Signatur-Pad

        private async Task ColorSelected(MudBlazor.Utilities.MudColor c)
        {
            await _spad.InvokeVoidAsync("setColor", $"rgb({c.R}, {c.G}, {c.B})");
        }

        private async Task Clear()
        {
            await _spad.InvokeVoidAsync("clear");
        }

        private async Task GetPngFromCanvas()
        {
            var data = await _spad.InvokeAsync<string?>("getSvg");
            if (data is not null && data.Length > 0)
            {
                var bytes = Helpers.Image.SvgToPng(data, Width);
                if (bytes is not null)
                {
                    await Finish(Convert.ToBase64String(bytes));
                }
            }
        }

        #endregion

        private async Task Finish(string data)
        {
            if (_name.Length > 0)
            {
                _signaturs.Add(new() { Name = _name, DataAsBase64 = data });
                await _spad.InvokeVoidAsync("setSignaturToStorage", Helpers.Json.GetJson(_signaturs));
            }

            MudDialog?.Close(data);
        }
    }
}
