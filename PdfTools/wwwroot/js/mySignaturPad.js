window.MySignaturPad = class {
    //https://github.com/szimek/signature_pad

    constructor(id) {
        this.canvas = document.getElementById(id);
        this.canvas.width = this.canvas.parentElement.clientWidth;
        this.canvas.height = this.canvas.width / 2;

        this.signaturePad = new SignaturePad(this.canvas);
    }

    getSignaturFromStorage() {
        return localStorage.getItem("mySignatur");
    }

    setSignaturToStorage(data) {
        return localStorage.setItem("mySignatur", data);
    }

    getSvg() {
        if (this.signaturePad.isEmpty() === false) {
            return this.signaturePad.toSVG();
        }
    }

    //rgb(66, 133, 244)
    setColor(rgb) {
        this.signaturePad.penColor = rgb;
    }

    clear() {
        this.signaturePad.clear();
    }
}