using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using MudBlazor;
using PdfTools.Data;
using SkiaSharp;
using System.Globalization;

namespace PdfTools.ViewModels
{
    public sealed class PdfPageViewModel : IAsyncDisposable
    {
        public const double Scale = 1.5;
        public bool IsSecected { get; set; }
        public int PdfId { get; set; }
        public int PageNumber { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string WrapperStyle { get; set; } = string.Empty;
        public string ScaleStyle { get; set; } = string.Empty;
        public List<PdfSignature> Signatures { get; set; } = [];

        private readonly PdfService _ps = default!;
        private readonly IJSObjectReference _doc = default!;
        private readonly IDialogService _dlg = default!;
        private readonly BusyService _bs = default!;
        private readonly TranslationService _ts = default!;
        private static readonly CultureInfo _ci = new("en-US");
        private Action? _stateHasChanged;

        public PdfPageViewModel() { }

        public PdfPageViewModel(int pdfId, int pageNumber, PdfService ps, IJSObjectReference doc, IDialogService dlg, BusyService bs, TranslationService ts)
        {
            PdfId = pdfId;
            PageNumber = pageNumber;
            _ps = ps;
            _doc = doc;
            _dlg = dlg;
            _bs = bs;
            _ts = ts;
        }

        public string GetKey()
        {
            return $"pp{PdfId}{PageNumber}";
        }

        public string GetCanvasId()
        {
            return $"can{PdfId}{PageNumber}";
        }

        public async Task RenderPageAsync(Action stateHasChanged)
        {
            _stateHasChanged = stateHasChanged;
            _ps.ScaleChanged += (_, _) => ScalePage();

            try
            {
                var m = await _doc.InvokeAsync<PdfPageViewModel>("renderPage", PageNumber, Scale, GetCanvasId());
                Width = m.Width;
                Height = m.Height;

                if (Width > _ps.WindowSize.Width)
                {
                    double scale = Math.Round(_ps.WindowSize.Width / (double)Width, 1, MidpointRounding.ToZero);
                    _ps.Scale = scale;
                }
                else
                    ScalePage();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public async Task MovePageTo()
        {
            DialogParameters<Dialogs.MovePageToDialog> para = new()
            {
                { x => x.ViewModel, this }
            };

            var dref = await _dlg.ShowAsync<Dialogs.MovePageToDialog>(_ts.I18n.MovePage, para);
            await dref.Result;
        }

        private void ScalePage()
        {
            WrapperStyle = $"width: {Math.Ceiling(Width * _ps.Scale)}px; height: {Math.Ceiling(Height * _ps.Scale) + 40}px;";

            string s1 = "position: relative; transform-origin: left top; display: block; overflow: hidden;";
            ScaleStyle = $"{s1} width: {Width}px; height: {Height}px; transform: scale({_ps.Scale.ToString(_ci)});";
            _stateHasChanged?.Invoke();
        }

        #region Signature

        public async Task AddSignature()
        {
            DialogParameters<Dialogs.SvgSignaturePadDialog> para = new() { { x => x.Width, Width } };
            var dref = await _dlg.ShowAsync<Dialogs.SvgSignaturePadDialog>(_ts.I18n.Signature, para);

            var res = await dref.Result;
            if (res is not null && res.Canceled == false && res.Data is string s)
            {
                using var bitmap = SKBitmap.Decode(Convert.FromBase64String(s));
                float f = Width * 0.33F / bitmap.Width;

                //Image size is relative to the page size
                PdfSignature sign = new()
                {
                    ImageData = s,
                    Width = (float)((bitmap.Width * f) / Width),
                    Height = (float)((bitmap.Height * f) / Height),
                    X = 0.5F,
                    Y = 0.5F
                };
                Signatures.Add(sign);
                await MoveSignature(sign);
            }
        }

        public async Task MoveSignature(PdfSignature sign)
        {
            DialogParameters<Dialogs.MoveSignatureDialog> para = new()
            {
                { x => x.ViewModel, this },
                { x => x.Signatur, sign }
            };

            DialogOptions opt = new() { MaxWidth = MaxWidth.Medium, NoHeader = true };
            var dref = await _dlg.ShowAsync<Dialogs.MoveSignatureDialog>(null, para, opt);
            await dref.Result;
        }

        public string GetSignStyle(PdfSignature item)
        {
            var w = Math.Ceiling(Width * item.Width);
            var h = Math.Ceiling(Height * item.Height);
            var x = Math.Ceiling(Width * item.X);
            var y = Math.Ceiling(Height * item.Y);

            return $"border: 1px solid var(--mud-palette-secondary); position: absolute; background-color: transparent; width: {w}px; height: {h}px; left: {x}px; top: {y}px;";
        }

        #endregion

        public async ValueTask DisposeAsync()
        {
            try
            {
                await _doc.DisposeAsync();
            }
            catch (Exception)
            {
            }
        }
    }
}