export function applyTypographyCss(css, themeName, styleId = "lattice-typography") {
    let style = document.getElementById(styleId);

    if (!style) {
        style = document.createElement("style");
        style.id = styleId;
        document.head.appendChild(style);
    }

    style.textContent = css;

    document.documentElement.setAttribute(
        "data-theme",
        themeName.toLowerCase()
    );
}

export function removeTypographyCss(styleId = "lattice-typography") {
    document.getElementById(styleId)?.remove();
}