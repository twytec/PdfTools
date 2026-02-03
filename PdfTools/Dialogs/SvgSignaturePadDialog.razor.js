export function scaleSignaturePad(id) {
    const svg = document.getElementById(id);
    const w = svg.parentElement.parentElement.clientWidth;
    const h = w / 3;
    const sw = `${w}px`;
    const sh = `${h}px`;

    svg.setAttribute("width", w);
    svg.setAttribute("height", h);
    svg.setAttribute("viewBox", `0 0 ${w} ${h}`);
    svg.parentElement.style.width = sw;
    svg.parentElement.style.height = sh;
}

export function getSvg(id) {
    const svg = document.getElementById(id);
    return svg.parentElement.innerHTML;
}

export function getSignaturFromStorage() {
    return localStorage.getItem("mySignatur");
}

export function setSignaturToStorage(data) {
    return localStorage.setItem("mySignatur", data);
}
