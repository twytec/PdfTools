window.PdfWorkerClient = class {
    pendingRequests = {};
    pendingRequestId = 0;

    constructor() {
        this.dotnetWorker = new Worker('./js/pdfWorker.js', { type: "module" });
        this.dotnetWorker.onmessage = async (e) => {
            if (e.data.command === "response") {
                if (!e.data.requestId) {
                    console.error("No requestId in response from worker");
                }

                const request = this.pendingRequests[e.data.requestId];
                delete this.pendingRequests[e.data.requestId];
                if (e.data.error) {
                    request.reject(new Error(e.data.error));
                }
                request.resolve(e.data.result);
            }
        };
    }

    jobToWorker(text) {
        this.pendingRequestId++;
        const promise = new Promise((resolve, reject) => {
            this.pendingRequests[this.pendingRequestId] = { resolve, reject };
        });
        this.dotnetWorker.postMessage({
            text: text,
            requestId: this.pendingRequestId
        });
        return promise;
    }
}