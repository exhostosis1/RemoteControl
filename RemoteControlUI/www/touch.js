import Point from './point.js';

let lastMovePosition;

let touchElement;
let touchPointer;

let touchesCache = [];

let dragStartTimer;

const touchEvent = new Event('touchpanelclick');
const moveEvent = new Event('touchpanelmove');
const scrollEvent = new Event('touchpanelscroll');
const dragEvent = new Event('touchpaneldrag');

const MODIFIER = 3;

function movePointer(x, y) {
    touchPointer.style.left = x - touchPointer.offsetWidth / 2;
    touchPointer.style.top = y - touchPointer.offsetHeight / 2;
}

function hidePointer() {
    touchPointer.style.display = 'none';
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

        const pos = new Point(e.targetTouches[0].clientX, e.targetTouches[0].clientY);
        if (!lastMovePosition) {
            lastMovePosition = new Point(touchesCache[0].x, touchesCache[0].y);
        }

        const diff = new Point(pos.x - lastMovePosition.x, pos.y - lastMovePosition.y);

        lastMovePosition = pos;

        if (e.targetTouches.length === 1) {
            moveEvent.position = JSON.stringify({ x: diff.x * MODIFIER, y: diff.y * MODIFIER });
            touchElement.dispatchEvent(moveEvent);
        }
        if (e.targetTouches.length === 2 && Math.abs(diff.y) > 10) {
            scrollEvent.direction = diff.y > 0 ? 'up' : 'down';
            touchElement.dispatchEvent(scrollEvent);
        }
    }
}

function getTouchIndex(identifier) {
    for (let i = 0; i < touchesCache.length; i += 1) {
        if (touchesCache[i].id === identifier) {
            return i;
        }
    }

    return -1;
}

function addTouch(t) {
    if (getTouchIndex(t.identifier) === -1) {
        touchesCache.push(new Point(t.clientX, t.clientY, t.identifier));
    }
}

function showPointer() {
    touchPointer.style.display = 'block';
}

function startTouch(e) {
    if (e.targetTouches.length === 1) {
        showPointer();
        movePointer(e.targetTouches[0].clientX, e.targetTouches[0].clientY);
    }

    for (let i = 0; i < e.changedTouches.length; i += 1) {
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

const mouseButtons = ['', 'left', 'right', 'middle'];

function endTouch(e) {
    hidePointer();

    for (let i = 0; i < e.changedTouches.length; i += 1) {
        const id = getTouchIndex(e.changedTouches[i].identifier);

        if (id !== -1) {
            const temp = new Point(e.changedTouches[i].clientX, e.changedTouches[i].clientY);

            if (!(touchesCache[id].checkTime(temp)) || !(touchesCache[id].checkRange(temp))) {
                touchesCache.splice(id, 1);
            }
        }
    }

    if (e.targetTouches.length === 0) {
        const { length } = touchesCache;
        if (length >= 1 && length <= 3) {
            touchEvent.touches = mouseButtons[length];
            touchElement.dispatchEvent(touchEvent);
        }

        touchesCache = [];
        lastMovePosition = undefined;
        tryStopDrag();
    }
}

function createTouch(element) {
    touchElement = element;

    touchPointer = new DOMParser().parseFromString('<div id="touch-pointer" style="display: none"></div>', 'text/html').body.firstChild;
    touchElement.append(touchPointer);

    touchElement.addEventListener('touchmove', processTouchMove);
    touchElement.addEventListener('touchstart', startTouch);
    touchElement.addEventListener('touchend', endTouch);
    touchElement.addEventListener('touchcancel', endTouch);
}


let prev = '';
let prevBorder = '';

function touchHide() {
    prev = touchElement.style.display;
    prevBorder = touchElement.style.border;
    touchElement.style.border = 'none';
    touchElement.style.display = 'none';
}

function touchShow() {
    touchElement.style.display = prev;
    setTimeout(() => { touchElement.style.border = prevBorder; }, 200);
}

export { createTouch, touchHide, touchShow };
export default createTouch;
