import { Events } from './constants.js';

const CLICKINTERVAL = 1000;

let keyRepeadIntervalId;
let keyRepeatTimerId;

let double = false;

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

function toggleDisplay(buttonsArray) {
    buttonsArray.forEach(x => {
        x.style.display = x.style.display === "none" ? "block" : "none";
    });
}  


function createKeys(buttons) {

    let buttonBack = document.createElement("button");
    buttonBack.innerHTML = "&#60;&#60;"; 
    let buttonPause = document.createElement("button");
    buttonPause.innerHTML = "&#10074;&#10074;"; 
    let buttonForth = document.createElement("button");
    buttonForth.innerHTML = "&#62;&#62;"; 
    let buttonMediaForth = document.createElement("button");
    buttonMediaForth.innerHTML = "&#45;&#62;"; 
    let buttonMediaBack = document.createElement("button");
    buttonMediaBack.innerHTML = "&#60;&#45;"; 

    let fillers = [document.createElement("span"), document.createElement("span")];
    fillers.forEach(e => {
        e.style.float = "left";
        e.style.height = "1px";
        e.style.width = "0.5%";
    });

    buttonMediaBack.style.display = "none";
    buttonMediaForth.style.display = "none";    
        
    buttons.append(buttonMediaBack, buttonBack, fillers[0], buttonPause, fillers[1], buttonForth, buttonMediaForth);

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

    let buttonsArray = [buttonBack, buttonForth, buttonMediaBack, buttonMediaForth];

    buttonPause.addEventListener("click", e => {
        if(double) {
            toggleDisplay(buttonsArray);
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