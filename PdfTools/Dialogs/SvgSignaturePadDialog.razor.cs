using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using System.Globalization;

namespace PdfTools.Dialogs
{
    public sealed partial class SvgSignaturePadDialog(Data.MyJsInterop _myJs) : IAsyncDisposable
    {
        [CascadingParameter]
        private IMudDialogInstance? MudDialog { get; set; }

        [Parameter]
        public int Width { get; set; } = 600;

        private const string signaturePadId = "signPad";
        private IJSObjectReference? _module;

        private readonly List<Data.Signatur> _signaturs = [];

        private string _name = string.Empty;

        private readonly List<string> _pens = ["#000000", "#0000ff", "#00ff00", "#ff0000"];
        private string _penColor = "#000000";
        private readonly int _penWidth = 2;
        private readonly List<string> _paths = [];
        private string? _path;
        private static readonly CultureInfo _ci = CultureInfo.GetCultureInfo("en-US");

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _module = await _js.InvokeAsync<IJSObjectReference>("import", "./Dialogs/SvgSignaturePadDialog.razor.js");
                await _module.InvokeVoidAsync("scaleSignaturePad", signaturePadId);

                var sing = await _module.InvokeAsync<string?>("getSignaturFromStorage");
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
            await _myJs.SaveFileAsync($"{sign.Name}", sign.DataAsBase64);
        }

        private async Task DeleteSignature(int index)
        {
            if (_module is not null)
            {
                _signaturs.RemoveAt(index);
                await _module.InvokeVoidAsync("setSignaturToStorage", Helpers.Json.GetJson(_signaturs));
            }
        }

        #endregion

        #region Signatur-Pad

        private async Task Clear()
        {
            _paths.Clear();
        }

        private async Task GetPngFromCanvas()
        {
            if (_module is not null)
            {
                var data = await _module.InvokeAsync<string>("getSvg", signaturePadId);
                if (data is not null && data.Length > 0)
                {
                    var bytes = Helpers.Image.SvgToPng(data, Width);
                    if (bytes is not null)
                    {
                        await Finish(Convert.ToBase64String(bytes));
                    }
                }
            }
        }

        private void OnStart(PointerEventArgs args)
        {
            _path ??= $"M {args.OffsetX.ToString(_ci)},{args.OffsetY.ToString(_ci)}";
        }

        private void OnMove(PointerEventArgs args)
        {
            if (_path is not null)
            {
                _path += $" {args.OffsetX.ToString(_ci)},{args.OffsetY.ToString(_ci)}";
            }
        }

        private void OnEnd(PointerEventArgs args)
        {
            if (_path is not null)
            {
                _paths.Add(_path);
                _path = null;
            }
        }

        #endregion

        private async Task Finish(string data)
        {
            if (_module is not null && _name.Length > 0)
            {
                _signaturs.Add(new() { Name = _name, DataAsBase64 = data });
                await _module.InvokeVoidAsync("setSignaturToStorage", Helpers.Json.GetJson(_signaturs));
            }

            MudDialog?.Close(data);
        }

        public async ValueTask DisposeAsync()
        {
            if (_module is not null)
            {
                try
                {
                    await _module.DisposeAsync();
                }
                catch (JSDisconnectedException)
                {
                }
            }
        }
    }
}
