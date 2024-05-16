import GestureDetector from './GestureDetector.js';

function sendRequest(controller, action, param) {
  return fetch(`/api/v1/${controller}/${action}/${param === undefined ? '' : param}`);
}

// volume slider

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

function init(initialValue) {
  volumeNumericValue = Number(initialValue);

  if (!Number.isNaN(volumeNumericValue)) {
    setVolumeBar(toPixels(volumeNumericValue));

    if (numericEnabled) {
      setVolumeBarNumeric(volumeNumericValue);
    }
  }
}

function createVolumeBar(slider, scale, initValue = 0, numeric = false) {
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

// touch

let touchElement;
const touchPointers = [];

function showPointer(p) {
  const elem = p;
  elem.style.display = 'block';
}

function movePointer(p, x, y) {
  const elem = p;
  elem.style.left = x - p.offsetWidth / 2;
  elem.style.top = y - p.offsetHeight / 2;
}

function hidePointer(p) {
  const elem = p;
  elem.style.display = 'none';
}

function processTouchMove(e) {
  Array.from(e.changedTouches).forEach((element) => {
    movePointer(touchPointers[element.identifier], element.clientX, element.clientY);
  });
}

function startTouch(e) {
  Array.from(e.changedTouches).forEach((element) => {
    showPointer(touchPointers[element.identifier]);
    movePointer(touchPointers[element.identifier], element.clientX, element.clientY);
  });
}

function endTouch(e) {
  Array.from(e.changedTouches).forEach((element) => {
    hidePointer(touchPointers[element.identifier]);
  });
}

function createTouch(element) {
  touchElement = element;
  const pointsCount = 5;

  const pointer = new DOMParser().parseFromString('<div class="touch-pointer"></div>', 'text/html').body.firstChild;

  for (let i = 0; i < pointsCount; i += 1) {
    touchPointers.push(pointer.cloneNode());
  }

  touchPointers.forEach((x) => {
    touchElement.append(x);
  });

  touchElement.addEventListener('touchmove', processTouchMove);
  touchElement.addEventListener('touchstart', startTouch);
  touchElement.addEventListener('touchend', endTouch);
  touchElement.addEventListener('touchcancel', endTouch);
}

let prev = '';
let prevBorder = '';

function touchHide() {
  prev = touchElement.style.display;
  prevBorder = touchElement.style.border;
  touchElement.style.border = 'none';
  touchElement.style.display = 'none';
}

function touchShow() {
  touchElement.style.display = prev;
  setTimeout(() => {
    touchElement.style.border = prevBorder;
  }, 200);
}

// keys

const buttonschangeCLICKINTERVAL = 1000;

let keyRepeadIntervalId;
let keyRepeatTimerId;

const clickEvent = new Event('keyclick');

let buttonsGlobal;

function start(value) {
  clickEvent.value = value;

  keyRepeatTimerId = setTimeout(() => {
    keyRepeadIntervalId = setInterval(() => {
      buttonsGlobal.dispatchEvent(clickEvent);
    }, 100);
  }, 1000);
}

function end() {
  clearInterval(keyRepeatTimerId);
  clearTimeout(keyRepeadIntervalId);
}

function toggleDisplay(buttonsArray) {
  buttonsArray.forEach((x) => {
    const elem = x;
    elem.style.display = elem.style.display === 'none' ? 'block' : 'none';
  });
}

function createKeys(buttons) {
  buttonsGlobal = buttons;

  const buttonBack = document.createElement('button');
  buttonBack.innerHTML = '&#60;&#60;';

  const buttonPause = document.createElement('button');
  buttonPause.innerHTML = '&#10074;&#10074;';

  const buttonForth = document.createElement('button');
  buttonForth.innerHTML = '&#62;&#62;';

  const buttonMediaForth = document.createElement('button');
  buttonMediaForth.innerHTML = '&#45;&#62;';
  buttonMediaForth.style.display = 'none';

  const buttonMediaBack = document.createElement('button');
  buttonMediaBack.innerHTML = '&#60;&#45;';
  buttonMediaBack.style.display = 'none';

  const fillers = [document.createElement('span'), document.createElement('span')];
  fillers.forEach((e) => {
    e.style.float = 'left';
    e.style.height = '1px';
    e.style.width = '0.5%';
  });

  buttons.append(
    buttonMediaBack,
    buttonBack,
    fillers[0],
    buttonPause,
    fillers[1],
    buttonForth,
    buttonMediaForth,
  );

  buttonBack.addEventListener('touchstart', () => start('back'));
  buttonBack.addEventListener('touchend', end);
  buttonBack.addEventListener('click', () => {
    clickEvent.value = 'back';
    buttons.dispatchEvent(clickEvent);
  });
  buttonBack.addEventListener('touchcancel', end);

  buttonForth.addEventListener('touchstart', () => start('forth'));
  buttonForth.addEventListener('touchend', end);
  buttonForth.addEventListener('click', () => {
    clickEvent.value = 'forth';
    buttons.dispatchEvent(clickEvent);
  });
  buttonForth.addEventListener('touchcancel', end);

  buttonMediaBack.addEventListener('click', () => {
    clickEvent.value = 'mediaback';
    buttons.dispatchEvent(clickEvent);
  });
  buttonMediaForth.addEventListener('click', () => {
    clickEvent.value = 'mediaforth';
    buttons.dispatchEvent(clickEvent);
  });

  const buttonsArray = [buttonBack, buttonForth, buttonMediaBack, buttonMediaForth];

  let timeout;
  let startTime;

  buttonPause.addEventListener('pointerdown', () => {
    startTime = new Date();
    timeout = setTimeout(() => toggleDisplay(buttonsArray), buttonschangeCLICKINTERVAL);
  });

  buttonPause.addEventListener('pointerup', () => {
    clearTimeout(timeout);
    if ((new Date()).getTime() - startTime.getTime() < buttonschangeCLICKINTERVAL) {
      clickEvent.value = 'pause';
      buttons.dispatchEvent(clickEvent);
    }
  });
}

// text

const textEvent = new Event('textinput');

let textElement;

function processText(e) {
  if (e.key === 'Enter') {
    if (e.target.value) {
      textEvent.text = encodeURIComponent(e.target.value);
      textElement.dispatchEvent(textEvent);
    }
    e.target.blur();
  }
}

function createTextInput(element) {
  textElement = element;

  element.addEventListener('keyup', processText);
}

function drawDevices(devices) {
  const devicesElement = document.getElementById('devices');
  devicesElement.innerHTML = '';

  devices.forEach((x) => {
    devicesElement.innerHTML += `
                <div class="device" onclick="setDevice('${x.Id}')">
                    <div class="device-icon">${x.IsCurrentControlDevice ? '✅' : '&#160'}</div>
                    <div>${x.Name}</div>
                </div>
                `;
  });
}

function showDevices() {
  const element = document.getElementById('devices');
  element.style.display = element.style.display === 'block' ? 'none' : 'block';
}

// init
function initElements(volume, devices) {
  const slider = document.getElementById('slider');
  if (slider) {
    createVolumeBar(slider, 100, volume, true);
    slider.addEventListener('volumechanged', (e) => sendRequest('audio', 'setvolume', e.value));
  }

  const touch = document.getElementById('touch');
  if (touch) {
    createTouch(touch);

    GestureDetector.Create(touch, {
      oneFingerClick: () => sendRequest('mouse', 'left'),
      oneFingerMove: (x, y) => sendRequest('mouse', 'move', `${x},${y}`),
      oneFingerDrag: (e) => sendRequest('mouse', e ? 'dragstart' : 'dragstop'),
      twoFingersClick: () => sendRequest('mouse', 'right'),
      twoFingersMove: (x, y) => {
        const absx = Math.abs(x);
        const absy = Math.abs(y);

        if (absx > absy && absx > 10) {
          sendRequest('keyboard', x < 0 ? 'browserforward' : 'browserback');
        }
        if (absy > 3) {
          sendRequest('mouse', y > 0 ? 'wheelup' : 'wheeldown');
        }
      },
      threeFingersClick: () => sendRequest('mouse', 'middle'),
    });
  }

  const buttons = document.getElementById('buttons');
  if (buttons) {
    createKeys(buttons);
    buttons.addEventListener('keyclick', (e) => sendRequest('keyboard', e.value));
  }

  const textInput = document.getElementById('text-input');
  if (textInput) {
    createTextInput(textInput);
    textInput.addEventListener('textinput', (e) => sendRequest('keyboard', 'text', e.text));
    textInput.addEventListener('focus', () => touchHide());
    textInput.addEventListener('blur', (e) => {
      e.target.value = '';
      touchShow();
    });
  }

  document.getElementById('darken').onclick = () => sendRequest('display', 'darken');

  if (devices.length > 1) {
    document.getElementById('devicon').style.display = 'block';
    document.getElementById('devicon').onclick = () => showDevices();

    drawDevices(devices);
  }

  document.getElementById('loader').style.display = 'none';
}

// eslint-disable-next-line no-unused-vars
async function setDevice(id) {
  document.getElementById('devices').style.display = 'none';

  const devices = JSON.parse(await (await sendRequest('audio', 'setdevice', id)).text());
  const volume = await (await sendRequest('audio', 'getvolume')).text();

  init(volume);

  drawDevices(devices);
}

document.addEventListener('DOMContentLoaded', async () => {
  let pageInPortraitMode = window.innerHeight > window.innerWidth;

  window.addEventListener('resize', () => {
    if (((pageInPortraitMode === true)
      && (window.innerHeight < window.innerWidth))
      || ((pageInPortraitMode === false)
      && (window.innerHeight > window.innerWidth))) {
      pageInPortraitMode = window.innerHeight > window.innerWidth;
      document.querySelector('meta[name=viewport]').setAttribute('content', `width=${window.innerWidth}, height=${window.innerHeight}, initial-scale=1.0, maximum-scale=1.0, user-scalable=0`);
    }
  });

  document.querySelector('meta[name=viewport]').setAttribute('content', `width=${window.innerWidth}, height=${window.innerHeight}, initial-scale=1.0, maximum-scale=1.0, user-scalable=0`);

  const devices = JSON.parse(await (await sendRequest('audio', 'getdevices')).text());
  const volume = await (await sendRequest('audio', 'getvolume')).text();

  initElements(volume, devices);
});
