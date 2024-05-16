import GestureDetector from './GestureDetector.js';
import createKeys from './createKeys.js';
import createTextInput from './createTextInput.js';
import { showDevices, drawDevices } from './drawDevices.js';
import { createTouch, touchHide, touchShow } from './createTouch.js';
import { createVolumeBar } from './createVolumeBar.js';
import sendRequest from './sendRequest.js';

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

export default initElements;
