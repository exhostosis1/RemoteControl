import createVolumeBar from './slider.js';
import createTouch from './touch.js';
import createKeys from './keys.js';

const Modes = {
    Audio: 'audio',
    Mouse: 'mouse',
    Keyboard: 'keyboard',
    Text: 'text',
};

async function sendRequest(method, param) {
    return (await fetch(`/api/${method}/${param}`)).text();
}

function processText(e) {
    if (e.key === 'Enter') {
        if (e.target.value) {
            sendRequest(Modes.Text, encodeURIComponent(e.target.value));
        }
        e.target.blur();
    }
}

document.addEventListener('DOMContentLoaded', async () => {
    const slider = document.getElementById('slider');

    if (slider) {
        createVolumeBar(slider, 100, await sendRequest(Modes.Audio, 'init'), true);
        slider.addEventListener('volumechanged', e => sendRequest(Modes.Audio, e.value));
    }

    const touch = document.getElementById('touch');

    if (touch) {
        createTouch(touch);
        touch.addEventListener('touchpanelclick', e => sendRequest(Modes.Mouse, e.touches));
        touch.addEventListener('touchpanelmove', e => { if (e.position) sendRequest(Modes.Mouse, e.position); });
        touch.addEventListener('touchpanelscroll', e => { sendRequest(Modes.Mouse, e.direction); });
        touch.addEventListener('touchpaneldrag', e => { sendRequest(Modes.Mouse, e.start ? 'dragstart' : 'dragstop'); });
    }

    const buttons = document.getElementById('buttons');

    if (buttons) {
        createKeys(buttons);
        buttons.addEventListener('keyclick', e => sendRequest(Modes.Keyboard, e.value));
    }

    const textInput = document.getElementById('text-input');

    if (textInput) {
        let prev = '';
        let prevBorder = '';

        textInput.addEventListener('keyup', processText);
        textInput.addEventListener('focus', () => {
            prev = touch.style.display;
            prevBorder = touch.style.border;
            touch.style.border = 'none';
            touch.style.display = 'none';
        });
        textInput.addEventListener('blur', e => {
            e.target.value = '';
            touch.style.display = prev;
            setTimeout(() => { touch.style.border = prevBorder; }, 200);
        });
    }

    const loader = document.getElementById('loader');

    if (loader) {
        document.getElementById('loader').style.display = 'none';
    }
});
