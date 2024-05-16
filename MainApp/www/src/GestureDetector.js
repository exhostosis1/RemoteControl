import Point from './Point.js';

class GestureDetector {
  static Create(element, touches) {
    return new GestureDetector(element, touches);
  }

  constructor(element, touches) {
    element.addEventListener('touchmove', this.processTouchMove.bind(this));
    element.addEventListener('touchstart', this.startTouch.bind(this));
    element.addEventListener('touchend', this.endTouch.bind(this));
    element.addEventListener('touchcancel', this.endTouch.bind(this));

    this.dragStartTimer = undefined;
    this.lastMovePosition = undefined;
    this.touchesCache = [];
    this.MODIFIER = 3;
    this.dragging = false;

    this.oneFingerMove = touches.oneFingerMove;
    this.oneFingerClick = touches.oneFingerClick;
    this.oneFingerDrag = touches.oneFingerDrag;

    this.twoFingersMove = touches.twoFingersMove;
    this.twoFingersClick = touches.twoFingersClick;

    this.threeFingersClick = touches.threeFingersClick;
  }

  tryStopDrag() {
    if (this.dragStartTimer !== undefined) {
      clearTimeout(this.dragStartTimer);
      this.dragStartTimer = undefined;
    }
    if (this.dragging) {
      this.dragging = false;
      this.oneFingerDrag(false);
    }
  }

  processTouchMove(e) {
    if (e.targetTouches.length <= 2) {
      if (e.targetTouches.length === 1 && this.dragStartTimer !== undefined) {
        if (!(this.touchesCache[0]
          .checkRange(new Point(e.targetTouches[0].clientX, e.targetTouches[0].clientY)))) {
          this.tryStopDrag();
        }
      }

      const pos = new Point(e.targetTouches[0].clientX, e.targetTouches[0].clientY);
      if (!this.lastMovePosition) {
        this.lastMovePosition = new Point(this.touchesCache[0].x, this.touchesCache[0].y);
      }

      const diff = new Point(pos.x - this.lastMovePosition.x, pos.y - this.lastMovePosition.y);

      if (diff.x === 0 && diff.y === 0) return;

      this.lastMovePosition = pos;

      if (Math.abs(diff.x) > 50 || Math.abs(diff.y) > 50) return;

      if (e.targetTouches.length === 1) {
        this.oneFingerMove(diff.x * this.MODIFIER, diff.y * this.MODIFIER);
      }
      if (e.targetTouches.length === 2) {
        this.twoFingersMove(diff.x, diff.y);
      }
    }
  }

  getTouchIndex(identifier) {
    for (let i = 0; i < this.touchesCache.length; i += 1) {
      if (this.touchesCache[i].id === identifier) {
        return i;
      }
    }

    return -1;
  }

  addTouch(t) {
    if (this.getTouchIndex(t.identifier) === -1) {
      this.touchesCache.push(new Point(t.clientX, t.clientY, t.identifier));
    }
  }

  startTouch(e) {
    for (let i = 0; i < e.changedTouches.length; i += 1) {
      this.addTouch(e.changedTouches[i]);
    }

    if (this.touchesCache.length === 1) {
      this.dragStartTimer = setTimeout(() => {
        this.oneFingerDrag(true);
        this.dragStartTimer = undefined;
        this.dragging = true;
      }, 1000);
    } else {
      this.tryStopDrag();
    }
  }

  endTouch(e) {
    for (let i = 0; i < e.changedTouches.length; i += 1) {
      const id = this.getTouchIndex(e.changedTouches[i].identifier);

      if (id !== -1) {
        const temp = new Point(e.changedTouches[i].clientX, e.changedTouches[i].clientY);

        if (!(this.touchesCache[id].checkTime(temp)) || !(this.touchesCache[id].checkRange(temp))) {
          this.touchesCache.splice(id, 1);
        }
      }
    }

    if (e.targetTouches.length === 0) {
      const { length } = this.touchesCache;
      if (length >= 1 && length <= 3) {
        switch (length) {
          case 1:
            this.oneFingerClick();
            break;
          case 2:
            this.twoFingersClick();
            break;
          case 3:
            this.threeFingersClick();
            break;
          default:
            break;
        }
      }

      this.touchesCache = [];
      this.lastMovePosition = undefined;
      this.tryStopDrag();
    }
  }
}

export default GestureDetector;
