window.PdfDoc = class {
    plib;
    pages = [];

    async loadPdf(pdfData) {
        pdfjsLib.GlobalWorkerOptions.workerSrc = './js/pdfjs/pdf.worker.mjs';
        this.plib = await pdfjsLib.getDocument({
            data: pdfData,
            useSystemFonts: true,
        }).promise;
        return this.plib.numPages;
    }

    async renderPage(number, scale, id) {
        if (window.devicePixelRatio !== 1) {
            scale = scale * window.devicePixelRatio;
        }

        if (scale > 1.5)
            scale = 1.5;

        const page = await this.plib.getPage(number);
        const pw = page.view[2];

        if (pw / window.innerWidth > 1.5) {
            scale = window.innerWidth * 1.5 / pw;
        }

        let viewport = page.getViewport({ scale: scale });
        const canvas = document.getElementById(id);
        const ctx = canvas.getContext('2d');

        canvas.width = Math.floor(viewport.width);
        canvas.height = Math.floor(viewport.height);

        /*const text = await page.getTextContent();*/
        await page.render({ canvasContext: ctx, viewport: viewport }).promise;
        this.pages.push(page);

        const p = canvas.parentElement;
        p.style.width = `${canvas.width}px`;
        p.style.height = `${canvas.height}px`;

        return {
            Width: canvas.width,
            Height: canvas.height
        };
    }

    getPdfPagesAsImages(pdfData, scale){
        return new Promise(async (resolve) => {
            pdfjsLib.GlobalWorkerOptions.workerSrc = './js/pdfjs/pdf.worker.mjs';
            const doc = await pdfjsLib.getDocument({
                data: pdfData,
                useSystemFonts: true,
            }).promise;

            const pages = [];
            if (window.devicePixelRatio !== 1) {
                scale = scale * window.devicePixelRatio;
            }
            
            for (let i = 1; i < doc.numPages + 1; i++) {
                const page = await doc.getPage(i);
                const viewport = page.getViewport({ scale: scale });
                const canvas = document.createElement('canvas');
                const ctx = canvas.getContext('2d');

                canvas.width = Math.floor(viewport.width);
                canvas.height = Math.floor(viewport.height);

                await page.render({ canvasContext: ctx, viewport: viewport }).promise;
                pages.push({
                    PageNumber: i,
                    Width: canvas.width,
                    Height: canvas.height,
                    ImageData: canvas.toDataURL("image/png", 100)
                });
            }

            resolve(pages);
        })
    }
}