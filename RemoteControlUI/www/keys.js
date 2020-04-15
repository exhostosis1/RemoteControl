import { Events } from './constants.js';

let keyRepeadIntervalId;
let keyRepeatTimerId;

let clickEvent = new Event(Events.Click);

function start(value) {
    clickEvent.value = value;

    keyRepeatTimerId = setTimeout(() => {
        keyRepeadIntervalId = setInterval(() => {
            buttons.dispatchEvent(clickEvent);
        }, 100)
    }, 1000);
}

function end() {
    clearInterval(keyRepeatTimerId);
    clearTimeout(keyRepeadIntervalId);
}

function createKeys(buttons) {

    let [buttonBack, buttonPause, buttonForth] = new DOMParser().parseFromString(`
        <button>&#60;&#60;</button>
        <button>&#10074;&#10074;</button>
        <button>&#62;&#62;</button>`, 'text/html').body.getElementsByTagName("button");

    buttons.append(buttonBack, buttonPause, buttonForth);

    buttonBack.addEventListener("touchstart", () => start("back"));
    buttonBack.addEventListener("touchend", end);
    buttonBack.addEventListener("click", () => {clickEvent.value = "back"; buttons.dispatchEvent(clickEvent); });
    buttonBack.addEventListener("touchcancel", end);

    buttonForth.addEventListener("touchstart", () => start("forth"));
    buttonForth.addEventListener("touchend", end);
    buttonForth.addEventListener("click", () => {clickEvent.value = "forth"; buttons.dispatchEvent(clickEvent); });
    buttonForth.addEventListener("touchcancel", end);

    buttonPause.addEventListener("click", e => { clickEvent.value = "pause"; buttons.dispatchEvent(clickEvent); });
}

export { createKeys };