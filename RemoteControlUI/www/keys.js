import { Events, EventValues } from './constants.js';

const CLICKINTERVAL = 1000;

let keyRepeadIntervalId;
let keyRepeatTimerId;

const clickEvent = new Event(Events.Click);

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
    buttonsArray.forEach(x => {
        // eslint-disable-next-line no-param-reassign
        x.style.display = x.style.display === 'none' ? 'block' : 'none';
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
    const buttonMediaBack = document.createElement('button');
    buttonMediaBack.innerHTML = '&#60;&#45;';

    const fillers = [document.createElement('span'), document.createElement('span')];
    fillers.forEach(e => {
        e.style.float = 'left';
        e.style.height = '1px';
        e.style.width = '0.5%';
    });

    buttonMediaBack.style.display = 'none';
    buttonMediaForth.style.display = 'none';

    buttons.append(buttonMediaBack, buttonBack, fillers[0], buttonPause, fillers[1], buttonForth, buttonMediaForth);

    buttonBack.addEventListener('touchstart', () => start(EventValues.Back));
    buttonBack.addEventListener('touchend', end);
    buttonBack.addEventListener('click', () => { clickEvent.value = EventValues.Back; buttons.dispatchEvent(clickEvent); });
    buttonBack.addEventListener('touchcancel', end);

    buttonForth.addEventListener('touchstart', () => start(EventValues.Forth));
    buttonForth.addEventListener('touchend', end);
    buttonForth.addEventListener('click', () => { clickEvent.value = EventValues.Forth; buttons.dispatchEvent(clickEvent); });
    buttonForth.addEventListener('touchcancel', end);

    buttonMediaBack.addEventListener('click', () => { clickEvent.value = EventValues.MediaBack; buttons.dispatchEvent(clickEvent); });
    buttonMediaForth.addEventListener('click', () => { clickEvent.value = EventValues.MediaForth; buttons.dispatchEvent(clickEvent); });

    const buttonsArray = [buttonBack, buttonForth, buttonMediaBack, buttonMediaForth];

    let timeout;

    buttonPause.addEventListener('click', () => {
        clickEvent.value = EventValues.Pause;
        buttons.dispatchEvent(clickEvent);
    });

    buttonPause.addEventListener('touchstart', () => {
        timeout = setTimeout(() => toggleDisplay(buttonsArray), CLICKINTERVAL);
    });
    buttonPause.addEventListener('touchend', () => {
        clearTimeout(timeout);
    });
}

export default createKeys;
