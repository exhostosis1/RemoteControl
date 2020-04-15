import { Point } from './point.js';
import { Events } from './constants.js';

let lastMovePosition;

let touchElement;
let touchPointer;

let touchesCache = [];

let dragStartTimer;

let touchEvent = new Event(Events.Touch);
let moveEvent = new Event(Events.Move);
let scrollEvent = new Event(Events.Scroll);
let dragEvent = new Event(Events.Drag);

const MODIFIER = 3;

function processTouchMove(e) {
    if (e.cancelable) e.preventDefault();

    if (e.targetTouches.length === 1) {
        movePointer(e.targetTouches[0].clientX, e.targetTouches[0].clientY);
    } else {
        hidePointer();
    }

    if (e.targetTouches.length <= 2) {
        if (e.targetTouches.length === 1 && dragStartTimer !== undefined) {
            if (!(touchesCache[0].checkRange(new Point(e.targetTouches[0].clientX, e.targetTouches[0].clientY)))) {
                tryStopDrag();
            }
        }

        let pos = new Point(e.targetTouches[0].clientX, e.targetTouches[0].clientY);
        if (!lastMovePosition) {
            lastMovePosition = new Point(touchesCache[0].x, touchesCache[0].y)
        }

        let diff = new Point(pos.x - lastMovePosition.x, pos.y - lastMovePosition.y);

        lastMovePosition = pos;

        if (e.targetTouches.length === 1) {
            moveEvent.position = JSON.stringify({ x: diff.x * MODIFIER, y: diff.y * MODIFIER });
            touchElement.dispatchEvent(moveEvent);
        }
        if (e.targetTouches.length === 2 && Math.abs(diff.y) > 10) {
            scrollEvent.direction = diff.y > 0 ? "up" : "down";
            touchElement.dispatchEvent(scrollEvent);
        }
    }
}

function startTouch(e) {
    if (e.targetTouches.length === 1) {
        movePointer(e.targetTouches[0].clientX, e.targetTouches[0].clientY);
    }

    for (let i = 0; i < e.changedTouches.length; i++) {
        addTouch(e.changedTouches[i]);
    }

    if (touchesCache.length === 1) {
        dragStartTimer = setTimeout(() => {
            dragEvent.start = true;
            touchElement.dispatchEvent(dragEvent);
            dragStartTimer = undefined;
        }, 1000);
    } else {
        tryStopDrag();
    }
}

function tryStopDrag() {
    if (dragStartTimer !== undefined) {
        clearTimeout(dragStartTimer);
        dragStartTimer = undefined;
    }
    if (dragEvent.start) {
        dragEvent.start = false;
        touchElement.dispatchEvent(dragEvent);
    }
}

function endTouch(e) {
    hidePointer();

    for (var i = 0; i < e.changedTouches.length; i++) {

        var id = getTouchIndex(e.changedTouches[i].identifier);
        if (id === -1) continue;

        let temp = new Point(e.changedTouches[i].clientX, e.changedTouches[i].clientY);

        if (!(touchesCache[id].checkTime(temp)) || !(touchesCache[id].checkRange(temp))) {
            touchesCache.splice(id, 1);
        }
    }

    if (e.targetTouches.length === 0) {
        if (touchesCache.length >= 1 && touchesCache.length <= 3) {
            touchEvent.touches = touchesCache.length;
            touchElement.dispatchEvent(touchEvent);
        }

        touchesCache = [];
        lastMovePosition = undefined;
        tryStopDrag();
    }
}

function addTouch(t) {
    if (getTouchIndex(t.identifier) === -1)
        touchesCache.push(new Point(t.clientX, t.clientY, t.identifier));
}

function getTouchIndex(identifier) {
    for (var i = 0; i < touchesCache.length; i++) {
        if (touchesCache[i].id === identifier)
            return i;
    }

    return -1;
}

function movePointer(x, y) {    
    touchPointer.style.display = "block";

    touchPointer.style.left = x - touchPointer.offsetWidth / 2;
    touchPointer.style.top = y - touchPointer.offsetHeight / 2;
}

function hidePointer() {
    touchPointer.style.display = "none";
}

function createTouch(element) {
    touchElement = element;

    touchPointer = new DOMParser().parseFromString('<div id="touch-pointer" style="display: none"></div>', 'text/html').body.firstChild;
    touchElement.append(touchPointer);

    touchElement.addEventListener("touchmove", processTouchMove);
    touchElement.addEventListener("touchstart", startTouch);
    touchElement.addEventListener("touchend", endTouch);
    touchElement.addEventListener("touchcancel", endTouch);
}

export { createTouch };