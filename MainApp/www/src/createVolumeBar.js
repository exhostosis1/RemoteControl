let SLIDERSCALE = 0;
const HANDLECLASS = 'volume-handle';
const COLOREDCLASS = 'volume-colored';
const BARID = 'volume-bar';
const NUMERICID = 'volume-numeric';
const VOLUMEBARWRAPPER = 'volume-bar-wrapper';
let volumeBar;
let volumeBarColored;
let volumeBarHandle;
let sliderElement;
let volumeBarNumericElement;
let volumeNumericValue = 0;
let numericEnabled = false;
let drag = false;
let correction = 0;
const changeEvent = new Event('volumechanged');

function toNumeric(value) {
  return Math.round(((value - correction) * SLIDERSCALE)
    / (volumeBar.offsetWidth - correction * 2));
}

function setVolumeBar(value) {
  let pixelValue = value - volumeBar.offsetLeft;
  pixelValue = pixelValue <= volumeBar.offsetWidth - correction
    ? pixelValue : volumeBar.offsetWidth - correction;
  pixelValue = pixelValue >= correction ? pixelValue : correction;

  volumeBarColored.style.width = pixelValue;

  return toNumeric(pixelValue);
}

function setVolumeBarNumeric(value) {
  volumeBarNumericElement.innerText = value;
}

function toPixels(value) {
  return ((volumeBar.offsetWidth - correction * 2) / SLIDERSCALE)
    * value + correction + volumeBar.offsetLeft;
}

function checkSendRequest(numericValue) {
  if (numericValue !== volumeNumericValue) {
    volumeNumericValue = numericValue;

    changeEvent.value = numericValue;
    sliderElement.dispatchEvent(changeEvent);
  }
}

function updateElementAndSendRequest(absolutePixels) {
  const numeric = setVolumeBar(absolutePixels);
  checkSendRequest(numeric);

  if (numericEnabled) {
    setVolumeBarNumeric(numeric);
  }
}

function wheel(e) {
  updateElementAndSendRequest(toPixels(e > 0 ? volumeNumericValue + 2 : volumeNumericValue - 2));
}

export function init(initialValue) {
  volumeNumericValue = Number(initialValue);

  if (!Number.isNaN(volumeNumericValue)) {
    setVolumeBar(toPixels(volumeNumericValue));

    if (numericEnabled) {
      setVolumeBarNumeric(volumeNumericValue);
    }
  }
}

export function createVolumeBar(slider, scale, initValue = 0, numeric = false) {
  SLIDERSCALE = scale;
  numericEnabled = numeric;

  sliderElement = slider;

  const html = `
                ${numericEnabled ? `<div id='${NUMERICID}'>0</div>\n` : ''}
                <div id=${VOLUMEBARWRAPPER}>
                    <div id='${BARID}'>
                        <div class='${COLOREDCLASS}'></div>
                        <div class='${HANDLECLASS}'></div>
                    </div>
                </div>`;

  sliderElement.innerHTML = html;

  volumeBar = sliderElement.querySelector(`#${BARID}`);

  if (numeric) {
    volumeBarNumericElement = sliderElement.querySelector(`#${NUMERICID}`);
  }

  volumeBarHandle = sliderElement.querySelector(`.${HANDLECLASS}`);
  volumeBarColored = sliderElement.querySelector(`.${COLOREDCLASS}`);

  correction = volumeBarHandle.offsetHeight / 2;
  volumeBarHandle.style.marginLeft = -correction;

  const resizeOb = new ResizeObserver((entries) => {
    const { height } = entries[0].contentRect;
    volumeBarHandle.style.width = height;
    correction = height / 2;
    volumeBarHandle.style.marginLeft = -correction;
  });
  resizeOb.observe(volumeBarHandle);

  volumeBarHandle.addEventListener('touchmove', (e) => {
    updateElementAndSendRequest(e.targetTouches[0].clientX);
  });
  volumeBar.addEventListener('click', (e) => {
    updateElementAndSendRequest(e.clientX);
  });
  volumeBar.addEventListener('wheel', (e) => {
    wheel(e.wheelDeltaY);
  });

  volumeBarHandle.addEventListener('mousedown', () => {
    drag = true;
  });
  volumeBarHandle.addEventListener('mouseup', () => {
    drag = false;
  });
  volumeBarHandle.addEventListener('mousemove', (e) => {
    if (drag) updateElementAndSendRequest(e.clientX);
  });

  init(initValue);
}
