import { Events } from './constants.js';

let SLIDERSCALE = 0;
const HANDLECLASS = 'volume-handle';
const COLOREDCLASS = 'volume-colored';
const BARID = 'volume-bar';
const NUMERICID = 'volume-numeric';

let volumeBar;
let volumeBarColored;
let volumeBarHandle;
let sliderElement;

let volumeBarNumericElement;

let volumeNumericValue = 0;

let numericEnabled = false;

let drag = false;

const changeEvent = new Event(Events.ValueChanged);

function setVolumeBar(value) {
    const correction = volumeBarHandle.offsetWidth / 2;

    let pixelValue = value - volumeBar.offsetLeft;
    pixelValue = pixelValue <= volumeBar.offsetWidth - correction ? pixelValue : volumeBar.offsetWidth - correction;
    pixelValue = pixelValue >= correction ? pixelValue : correction;

    volumeBarHandle.style.marginLeft = pixelValue - correction;
    volumeBarColored.style.width = pixelValue;

    return pixelValue;
}

function setVolumeBarNumeric(value) {
    volumeBarNumericElement.innerText = value;
}

function toPixels(value) {
    return ((volumeBar.offsetWidth - volumeBarHandle.offsetWidth) / SLIDERSCALE) * value + (volumeBarHandle.offsetWidth / 2) + volumeBar.offsetLeft;
}

function toNumeric(value) {
    return Math.round(((value - (volumeBarHandle.offsetWidth / 2)) * SLIDERSCALE) / (volumeBar.offsetWidth - volumeBarHandle.offsetWidth));
}

function checkSendRequest(value) {
    const newNumeric = toNumeric(setVolumeBar(value));

    if (newNumeric !== volumeNumericValue) {
        volumeNumericValue = newNumeric;

        changeEvent.value = newNumeric;
        sliderElement.dispatchEvent(changeEvent);
        if (numericEnabled) {
            setVolumeBarNumeric(newNumeric);
        }
    }
}

function wheel(e) {
    checkSendRequest(toPixels(e > 0 ? volumeNumericValue + 2 : volumeNumericValue - 2));
}

function init(initialValue) {
    volumeNumericValue = Number(initialValue);

    if (!Number.isNaN(volumeNumericValue)) {
        setVolumeBar(toPixels(initialValue));

        if (numericEnabled) {
            setVolumeBarNumeric(initialValue);
        }
    }
}

function createVolumeBar(slider, scale, initValue = 0, numeric = false) {
    SLIDERSCALE = scale;
    numericEnabled = numeric;

    sliderElement = slider;

    let html = `<div id='${BARID}'></div>`;
    html = numericEnabled ? `<div id='${NUMERICID}'>0</div>\n${html}` : html;

    sliderElement.innerHTML = html;

    volumeBar = sliderElement.querySelector(`#${BARID}`);

    if (numeric) {
        volumeBarNumericElement = sliderElement.querySelector(`#${NUMERICID}`);
    }

    volumeBar.innerHTML = `
        <div class='${COLOREDCLASS}'></div>
        <div class='${HANDLECLASS}'></div>
    `;

    volumeBarHandle = volumeBar.querySelector(`.${HANDLECLASS}`);
    volumeBarColored = volumeBar.querySelector(`.${COLOREDCLASS}`);

    volumeBarHandle.addEventListener('touchmove', e => { e.preventDefault(); checkSendRequest(e.targetTouches[0].clientX); });
    volumeBar.addEventListener('click', e => { e.preventDefault(); checkSendRequest(e.clientX); });
    volumeBar.addEventListener('wheel', e => { e.preventDefault(); wheel(e.wheelDeltaY); });

    volumeBarHandle.addEventListener('mousedown', () => { drag = true; });
    volumeBarHandle.addEventListener('mouseup', () => { drag = false; });
    volumeBarHandle.addEventListener('mousemove', e => { e.preventDefault(); if (drag) checkSendRequest(e.clientX); });

    init(initValue);
}

export default createVolumeBar;
