import { dotnet } from '../_framework/dotnet.js'

let assemblyExports = null;
let startupError = undefined;

try {
    const { getAssemblyExports, getConfig } = await dotnet.create();
    const config = getConfig();
    assemblyExports = await getAssemblyExports(config.mainAssemblyName);
}
catch (err) {
    startupError = err.message;
}

self.addEventListener('message', async function (e) {
    try {
        if (!assemblyExports) {
            throw new Error(startupError || "worker exports not loaded");
        }
        let result = assemblyExports.PdfTools.Data.PdfWorker.JobToWorker(e.data.text);
        self.postMessage({
            command: "response",
            requestId: e.data.requestId,
            result,
        });
    }
    catch (err) {
        self.postMessage({
            command: "response",
            requestId: e.data.requestId,
            error: err.message,
        });
    }
}, false);