using Microsoft.JSInterop;
using System.Text;

namespace PdfTools.Data
{
    public class PdfWorkerClient(IJSRuntime _js)
    {
        private IJSObjectReference? _client;

        public async Task InitAsync()
        {
            _client ??= await _js.InvokeConstructorAsync("PdfWorkerClient");
        }

        public async Task<string?> JobToWorkerAsync(JobData data)
        {
            if (_client is not null)
            {
                return await _client.InvokeAsync<string>("jobToWorker", Helpers.Json.GetJson(data));
            }
            return null;
        }
    }
}
