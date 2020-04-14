import { createVolumeBar } from './slider.js';
import { createTouch } from './touch.js';
import { createKeys } from './keys.js';
import { Events, Modes } from './constants.js';

async function sendRequest(mode, value) {
    return (await fetch(`/api?mode=${mode}&value=${value}`)).text();
}

function processText(e) {
    if (e.key === "Enter") {
        if(e.target.value) {
            sendRequest(Modes.Text, encodeURIComponent(e.target.value));
        }
        e.target.blur();
    }
}

document.addEventListener("DOMContentLoaded", async () => {
    let slider = document.getElementById("slider");
    createVolumeBar(slider, 100, await sendRequest(Modes.Audio, "init"), true);
    slider.addEventListener(Events.ValueChanged, e => sendRequest(Modes.Audio, e.value));

    let touch = document.getElementById("touch");
    createTouch(touch);
    touch.addEventListener(Events.Touch, e => sendRequest(Modes.Mouse, e.touches));
    touch.addEventListener(Events.Move, e => sendRequest(Modes.Mouse, e.position));
    touch.addEventListener(Events.Scroll, e => sendRequest(Modes.Wheel, e.direction));
    touch.addEventListener(Events.Drag, e => sendRequest(Modes.Mouse, `drag${e.start ? 'start' : 'stop'}`));

    let buttons = document.getElementById("buttons");
    createKeys(buttons);
    buttons.addEventListener(Events.Click, e => sendRequest(Modes.Keyboard, e.value));

    let prev = "";
    let prevBorder = "";

    document.getElementById("text-input").addEventListener("keyup", processText);
    document.getElementById("text-input").addEventListener("focus", () => { 
        prev = touch.style.display; 
        prevBorder = touch.style.border; 
        touch.style.border = "none"; 
        touch.style.display = "none"; 
    })
    document.getElementById("text-input").addEventListener("blur", e => { 
        e.target.value = ""; 
        touch.style.display = prev; 
        setTimeout(() => touch.style.border = prevBorder, 200); 
    });

    document.getElementById("loader").style.display = "none";
});
