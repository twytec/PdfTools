
export function moveSignatureInit(canvasId, pageId, width, height) {
    const canvas = document.getElementById(canvasId);
    const scaleElement = document.getElementById(pageId);

    if (canvas !== null && scaleElement !== null) {
        scaleElement.style.backgroundImage = `url(${canvas.toDataURL("image/png", 100)})`;
        const wrapper = scaleElement.parentElement;

        let w = wrapper.clientWidth - 20;
        let h = wrapper.clientHeight - 20;
        
        let f = w / width;
        if (f > 1)
            f = 1;

        if (height * f > h) {
            f = h / height;
        }

        w = Math.floor(width * f);
        h = Math.floor(height * f);
        const sw = `${w}px`;
        const sh = `${h}px`;

        scaleElement.style.width = sw;
        scaleElement.style.height = sh;
        scaleElement.style.backgroundSize = `${sw} ${sh}`;

        return {
            width: w,
            height: h
        }
    }
}