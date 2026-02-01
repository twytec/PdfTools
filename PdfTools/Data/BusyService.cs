using MudBlazor;

namespace PdfTools.Data
{
    public class BusyService
    {
        public bool IsShow { get; set; }
        private Action? _update;

        public void Init(Action update)
        {
            _update = update; 
        }

        public void ShowBusy()
        {
            IsShow = true;
            _update?.Invoke();
        }

        public void HideBusy()
        {
            IsShow = false;
            _update?.Invoke();
        }
    }
}
