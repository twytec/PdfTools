
window.setSceneEventListner = () => {
    window.addEventListener('resize', setScenesScale);
};

window.setSceneById = (id) => {
    const el = document.getElementById(id);
    setScaleOfElement(el);
};

function setScenesScale() {
    const items = document.getElementsByName("scene");
    for (var i = 0; i < items.length; i++) {
        const el = items[i];
        setScaleOfElement(el);
    }
}

function setScaleOfElement(el) {
    if (el !== undefined) {
        let p = el.parentElement;

        let f = (p.parentElement.clientWidth) / el.clientWidth;
        if (el.clientHeight * f > p.parentElement.clientHeight) {
            f = (p.parentElement.clientHeight) / el.clientHeight;
        }

        if (f > 1)
            f = 1;

        let w = Math.ceil(el.clientWidth * f);
        let h = Math.ceil(el.clientHeight * f);

        el.style.transform = "scale(" + f + ")"
        p.style.width = w + "px";
        p.style.height = h + "px";
    }
}