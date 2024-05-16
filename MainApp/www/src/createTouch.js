let touchElement;
const touchPointers = [];

function showPointer(p) {
  const elem = p;
  elem.style.display = 'block';
}

function movePointer(p, x, y) {
  const elem = p;
  elem.style.left = x - p.offsetWidth / 2;
  elem.style.top = y - p.offsetHeight / 2;
}

function hidePointer(p) {
  const elem = p;
  elem.style.display = 'none';
}

function processTouchMove(e) {
  Array.from(e.changedTouches).forEach((element) => {
    movePointer(touchPointers[element.identifier], element.clientX, element.clientY);
  });
}

function startTouch(e) {
  Array.from(e.changedTouches).forEach((element) => {
    showPointer(touchPointers[element.identifier]);
    movePointer(touchPointers[element.identifier], element.clientX, element.clientY);
  });
}

function endTouch(e) {
  Array.from(e.changedTouches).forEach((element) => {
    hidePointer(touchPointers[element.identifier]);
  });
}

export function createTouch(element) {
  touchElement = element;
  const pointsCount = 5;

  const pointer = new DOMParser().parseFromString('<div class="touch-pointer"></div>', 'text/html').body.firstChild;

  for (let i = 0; i < pointsCount; i += 1) {
    touchPointers.push(pointer.cloneNode());
  }

  touchPointers.forEach((x) => {
    touchElement.append(x);
  });

  touchElement.addEventListener('touchmove', processTouchMove);
  touchElement.addEventListener('touchstart', startTouch);
  touchElement.addEventListener('touchend', endTouch);
  touchElement.addEventListener('touchcancel', endTouch);
}

let prev = '';
let prevBorder = '';

export function touchHide() {
  prev = touchElement.style.display;
  prevBorder = touchElement.style.border;
  touchElement.style.border = 'none';
  touchElement.style.display = 'none';
}

export function touchShow() {
  touchElement.style.display = prev;
  setTimeout(() => {
    touchElement.style.border = prevBorder;
  }, 200);
}
