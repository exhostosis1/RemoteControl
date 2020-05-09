import { Events } from './constants.js';

const CLICKINTERVAL = 1000;

let keyRepeadIntervalId;
let keyRepeatTimerId;

let double = false;

let buttonBack, buttonForth, buttonPause, buttonMediaBack, buttonMediaForth;

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

function toggleDisplay() {
    if(buttonBack.style.display == "none") {
        buttonBack.style.display = "block";
    } else {
        buttonBack.style.display = "none";
    }

    if(buttonForth.style.display == "none") {
        buttonForth.style.display = "block";
     } else {
        buttonForth.style.display = "none";
     }

     if(buttonMediaBack.style.display == "none") {
        buttonMediaBack.style.display = "block";
     } else {
        buttonMediaBack.style.display = "none";
     }

     if(buttonMediaForth.style.display == "none") {
        buttonMediaForth.style.display = "block";
     } else {
        buttonMediaForth.style.display = "none";
     }
}  


function createKeys(buttons) {

    [buttonBack, buttonPause, buttonForth, buttonMediaForth, buttonMediaBack] = new DOMParser().parseFromString(`
        <button>&#60;&#60;</button>
        <button>&#10074;&#10074;</button>
        <button>&#62;&#62;</button>
        <button>&#45;&#62;</button>
        <button>&#60;&#45;</button>`, 'text/html').body.getElementsByTagName("button");

    buttonMediaBack.style.display = "none";
    buttonMediaForth.style.display = "none";    
        
    buttons.append(buttonMediaBack, buttonBack, buttonPause, buttonForth, buttonMediaForth);

    buttonBack.addEventListener("touchstart", () => start("back"));
    buttonBack.addEventListener("touchend", end);
    buttonBack.addEventListener("click", () => {clickEvent.value = "back"; buttons.dispatchEvent(clickEvent); });
    buttonBack.addEventListener("touchcancel", end);

    buttonForth.addEventListener("touchstart", () => start("forth"));
    buttonForth.addEventListener("touchend", end);
    buttonForth.addEventListener("click", () => {clickEvent.value = "forth"; buttons.dispatchEvent(clickEvent); });
    buttonForth.addEventListener("touchcancel", end);

    buttonMediaBack.addEventListener("click", () => {clickEvent.value = "mediaback"; buttons.dispatchEvent(clickEvent)});
    buttonMediaForth.addEventListener("click", () => {clickEvent.value = "mediaforth"; buttons.dispatchEvent(clickEvent)});

    buttonPause.addEventListener("click", e => {
        if(double) {
            toggleDisplay();
        }

        double = true;
        setTimeout(() => {
            double = false;
        }, CLICKINTERVAL);

        clickEvent.value = "pause"; 
        buttons.dispatchEvent(clickEvent); 
    });
}

export { createKeys };