window.MyJsInterop = class {

    setHtmlLang(code) {
        document.documentElement.lang = code;
    }

    getWindowSize() {
        return {
            width: window.innerWidth,
            height: window.innerHeight
        }
    }

    saveAsFile(filename, bytesBase64) {
        var link = document.createElement('a');
        link.download = filename;
        link.href = "data:application/octet-stream;base64," + bytesBase64;
        document.body.appendChild(link); // Needed for Firefox
        link.click();
        document.body.removeChild(link);
    }
}