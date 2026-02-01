using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using PdfTools.Data;

namespace PdfTools.Pages
{
    public partial class Home(TranslationService _ts, PdfService _ps, BusyService _bs)
    {
        [Parameter]
        public string? Text { get; set; }

        private MudFileUpload<IBrowserFile>? _fileUpload;
        private bool _open;

        private bool _row = false;
        private Wrap _wrap = Wrap.NoWrap;
        private Justify _justify = Justify.FlexStart;
        private AlignItems _alignItems = AlignItems.Center;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _bs.ShowBusy();

                if (Text is null)
                    await _ts.InitializeAsync();
                else
                    await _ts.SetTranslationsAsync(Text);

                await _ps.InitAsny();
                _ps.ScaleChanged += (_, _) => StateHasChanged();
                _ps.Changed += (_, _) => StateHasChanged();

                StateHasChanged();
                _bs.HideBusy();
            }
        }

        private async Task OpenFilePicker()
        {
            if (_fileUpload is not null)
            {
                await _fileUpload.OpenFilePickerAsync();
            }

            _open = false;
        }

        private async Task LoadPdfAsync(IBrowserFile file)
        {
            await _ps.LoadPdfAsync(file);
        }

        private void ToggleGrid()
        {
            _row = !_row;
            if (_row)
            {
                _wrap = Wrap.Wrap;
                _justify = Justify.Center;
                _alignItems = AlignItems.Start;
            }
            else
            {
                _wrap = Wrap.NoWrap;
                _justify = Justify.FlexStart;
                _alignItems = AlignItems.Center;
            }
        }
    }
}
