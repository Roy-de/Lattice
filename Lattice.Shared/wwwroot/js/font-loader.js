export async function registerFont(family, base64, weight, style) {
    const source = `url(data:font/ttf;base64,${base64})`;

    const fontFace = new FontFace(family, source, {
        weight: weight,
        style: style
    });

    await fontFace.load();

    document.fonts.add(fontFace);

    return true;
}

export function isFontRegistered(family) {
    return document.fonts.check(`16px "${family}"`);
}