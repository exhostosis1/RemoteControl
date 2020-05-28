const Events = {
    Move: 'touchmove',
    Touch: 'touchclick',
    Click: 'buttonclick',
    Drag: 'touchdrag',
    Scroll: 'touchscroll',
    ValueChanged: 'slidervaluechanged',
};
const EventValues = {
    Back: 'back',
    Forth: 'forth',
    MediaBack: 'mediaback',
    MediaForth: 'mediaforth',
    Pause: 'pause',
    Up: 'up',
    Down: 'down',
    DragStart: 'dragstart',
    DragStop: 'dragsopt',
    LeftButton: 'left',
    RightButton: 'right',
    MiddleButton: 'middle',
    Init: 'init',
};
const Modes = {
    Audio: 'audio',
    Mouse: 'mouse',
    Keyboard: 'keyboard',
    Text: 'text',
};

export { Events, Modes, EventValues };
