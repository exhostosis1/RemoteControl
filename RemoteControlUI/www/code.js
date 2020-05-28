import createVolumeBar from './slider.js';
import createTouch from './touch.js';
import createKeys from './keys.js';
import { Events, Modes, EventValues } from './constants.js';

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
        createVolumeBar(slider, 100, await sendRequest(Modes.Audio, EventValues.Init), true);
        slider.addEventListener(Events.ValueChanged, e => sendRequest(Modes.Audio, e.value));
    }

    const touch = document.getElementById('touch');

    if (touch) {
        createTouch(touch);
        touch.addEventListener(Events.Touch, e => {
            let event;
            switch (e.touches) {
            case 1:
                event = EventValues.LeftButton;
                break;
            case 2:
                event = EventValues.RightButton;
                break;
            case 3:
                event = EventValues.MiddleButton;
                break;
            default:
                return;
            }
            sendRequest(Modes.Mouse, event);
        });
        touch.addEventListener(Events.Move, e => { if (e.position) sendRequest(Modes.Mouse, e.position); });
        touch.addEventListener(Events.Scroll, e => { sendRequest(Modes.Mouse, e.direction); });
        touch.addEventListener(Events.Drag, e => { sendRequest(Modes.Mouse, e.start ? EventValues.DragStart : EventValues.DragStop); });
    }

    const buttons = document.getElementById('buttons');

    if (buttons) {
        createKeys(buttons);
        buttons.addEventListener(Events.Click, e => sendRequest(Modes.Keyboard, e.value));
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
