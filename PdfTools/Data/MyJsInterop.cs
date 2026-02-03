using Microsoft.JSInterop;
using System.Drawing;

namespace PdfTools.Data
{
    public sealed class MyJsInterop(IJSRuntime _js) : IAsyncDisposable
    {
        private IJSObjectReference _module = default!;

        public async ValueTask InitAsync()
        {
            _module ??= await _js.InvokeConstructorAsync("MyJsInterop");
        }

        public ValueTask<Size> GetWindowSizeAsync()
        {
            return _module.InvokeAsync<Size>("getWindowSize");
        }

        public async ValueTask SaveFileAsync(string filename, string dataAsBase64)
        {
            await _module.InvokeVoidAsync("saveAsFile", filename, dataAsBase64);
        }

        public async ValueTask DisposeAsync()
        {
            if (_module is not null)
            {
                try
                {
                    await _module.DisposeAsync();
                }
                catch (Exception)
                {
                }
            }
        }
    }
}
