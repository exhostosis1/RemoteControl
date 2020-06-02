const CLICKINTERVAL = 1000;

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
    buttonsArray.forEach(x => {
        // eslint-disable-next-line no-param-reassign
        x.style.display = x.hidden ? 'block' : 'none';
        // eslint-disable-next-line no-param-reassign
        x.hidden = !x.hidden;
    });
}

function createKeys(buttons) {
    buttonsGlobal = buttons;

    const buttonBack = document.createElement('button');
    buttonBack.innerHTML = '&#60;&#60;';
    buttonBack.hidden = false;

    const buttonPause = document.createElement('button');
    buttonPause.innerHTML = '&#10074;&#10074;';
    buttonPause.hidden = false;

    const buttonForth = document.createElement('button');
    buttonForth.innerHTML = '&#62;&#62;';
    buttonPause.hidden = false;

    const buttonMediaForth = document.createElement('button');
    buttonMediaForth.innerHTML = '&#45;&#62;';
    buttonMediaForth.hidden = true;
    buttonMediaForth.style.display = 'none';

    const buttonMediaBack = document.createElement('button');
    buttonMediaBack.innerHTML = '&#60;&#45;';
    buttonMediaBack.hidden = true;
    buttonMediaBack.style.display = 'none';

    const fillers = [document.createElement('span'), document.createElement('span')];
    fillers.forEach(e => {
        e.style.float = 'left';
        e.style.height = '1px';
        e.style.width = '0.5%';
    });

    buttons.append(buttonMediaBack, buttonBack, fillers[0], buttonPause, fillers[1], buttonForth, buttonMediaForth);

    buttonBack.addEventListener('touchstart', () => start('back'));
    buttonBack.addEventListener('touchend', end);
    buttonBack.addEventListener('click', () => { clickEvent.value = 'back'; buttons.dispatchEvent(clickEvent); });
    buttonBack.addEventListener('touchcancel', end);

    buttonForth.addEventListener('touchstart', () => start('forth'));
    buttonForth.addEventListener('touchend', end);
    buttonForth.addEventListener('click', () => { clickEvent.value = 'forth'; buttons.dispatchEvent(clickEvent); });
    buttonForth.addEventListener('touchcancel', end);

    buttonMediaBack.addEventListener('click', () => { clickEvent.value = 'mediaback'; buttons.dispatchEvent(clickEvent); });
    buttonMediaForth.addEventListener('click', () => { clickEvent.value = 'mediaforth'; buttons.dispatchEvent(clickEvent); });

    const buttonsArray = [buttonBack, buttonForth, buttonMediaBack, buttonMediaForth];

    let timeout;

    buttonPause.addEventListener('click', () => {
        clickEvent.value = 'pause';
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
