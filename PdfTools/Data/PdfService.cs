using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using PdfTools.ViewModels;

namespace PdfTools.Data
{
    public sealed class PdfService(TranslationService _ts, IJSRuntime _js, PdfWorkerClient _pwc, BusyService _bs, IDialogService _dlg)
    {
        public event EventHandler? ScaleChanged;
        public double Scale
        {
            get { return field; }
            set { field = value; ScaleChanged?.Invoke(this, EventArgs.Empty); }
        } = 1;
        public string GetScaleAsyString() => $"{Math.Round(Scale * 100, 0)} %";

        public event EventHandler? Changed;
        public List<PdfPageViewModel> Pages { get; set; } = [];
        public int WindowWidth { get; set; }
        
        private IJSObjectReference _ps = default!;
        private readonly Dictionary<int, byte[]> _pdfDatas = [];

        public async Task InitAsny()
        {
            await _pwc.InitAsync();
            _ps = await _js.InvokeConstructorAsync("PdfService");
        }

        public async Task LoadPdfAsync(IBrowserFile file)
        {
            _bs.ShowBusy();
            try
            {
                using var stream = file.OpenReadStream(file.Size);
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);

                var bytes = ms.ToArray();

                var doc = await _js.InvokeConstructorAsync("PdfService");
                var count = await doc.InvokeAsync<int>("loadPdf", bytes);

                if (count > 0)
                {
                    for (int i = 1; i <= count; i++)
                    {
                        Pages.Add(new PdfPageViewModel(_pdfDatas.Count, i, this, doc, _dlg, _bs, _ts));
                    }
                    _pdfDatas[_pdfDatas.Count] = bytes;

                    WindowWidth = await _ps.InvokeAsync<int>("getWindowWidth");
                }
            }
            catch (Exception ex)
            {
                await _dlg.ShowMessageBox(_ts.I18n.Error, ex.Message, _ts.I18n.Ok);
            }
            _bs.HideBusy();
        }

        public async Task SavePdfAsync()
        {
            JobData jobData = new()
            {
                PdfFiles = _pdfDatas,
                Pages = Pages
            };

            await SavePdfAsync(jobData);
        }

        public async Task SavePdfAsync(JobData jobData)
        {
            _bs.ShowBusy();
            try
            {
                var res = await _pwc.JobToWorkerAsync(jobData);
                if (res is not null && res.Length > 0)
                {
                    await _js.InvokeVoidAsync("saveAsFile", "Pdf.pdf", res);
                }
            }
            catch (Exception ex)
            {
                await _dlg.ShowMessageBox(_ts.I18n.Error, ex.Message, _ts.I18n.Ok);
            }
            _bs.HideBusy();
        }

        public async Task DeleteAsync(PdfPageViewModel? p)
        {
            if (p is not null)
            {
                try
                {
                    Pages.Remove(p);
                    Changed?.Invoke(this, EventArgs.Empty);

                    if (Pages.Any(x => x.PdfId == p.PdfId) == false)
                    {
                        await p.DisposeAsync();
                        _pdfDatas.Remove(p.PdfId);
                    }
                }
                catch (Exception ex)
                {
                    await _dlg.ShowMessageBox(_ts.I18n.Error, ex.Message, _ts.I18n.Ok);
                }
            }
        }

        #region SelectedElements

        public void SelectAll()
        {
            foreach (var item in Pages)
            {
                item.IsSecected = true;
            }
        }

        public void DeselectAll()
        {
            foreach (var item in Pages)
            {
                item.IsSecected = false;
            }
        }

        public async Task DeleteSelect()
        {
            foreach (var item in Pages)
            {
                if (item.IsSecected)
                {
                    await DeleteAsync(item);
                }
            }
        }

        public async Task SaveSelected()
        {
            var p = Pages.Where(x => x.IsSecected).ToList();
            if (p.Count > 0)
            {
                _bs.ShowBusy();
                try
                {
                    JobData data = new() { Pages = p };
                    var f = p.Select(x => x.PdfId).Distinct();
                    foreach (var item in f)
                    {
                        data.PdfFiles[item] = _pdfDatas[item];
                    }

                    await SavePdfAsync(data);
                }
                catch (Exception ex)
                {
                    await _dlg.ShowMessageBox(_ts.I18n.Error, ex.Message, _ts.I18n.Ok);
                }
                _bs.HideBusy();
            }
        }

        #endregion

        public void MovePageTo(PdfPageViewModel page, int index)
        {
            if (index > -1 && index < Pages.Count)
            {
                Pages.Remove(page);
                Pages.Insert(index, page);
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
