const buttonschangeCLICKINTERVAL = 1000;
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
  buttonsArray.forEach((x) => {
    const elem = x;
    elem.style.display = elem.style.display === 'none' ? 'block' : 'none';
  });
}
function createKeys(buttons) {
  buttonsGlobal = buttons;

  const buttonBack = document.createElement('button');
  buttonBack.innerHTML = '&#60;&#60;';

  const buttonPause = document.createElement('button');
  buttonPause.innerHTML = '&#10074;&#10074;';

  const buttonForth = document.createElement('button');
  buttonForth.innerHTML = '&#62;&#62;';

  const buttonMediaForth = document.createElement('button');
  buttonMediaForth.innerHTML = '&#45;&#62;';
  buttonMediaForth.style.display = 'none';

  const buttonMediaBack = document.createElement('button');
  buttonMediaBack.innerHTML = '&#60;&#45;';
  buttonMediaBack.style.display = 'none';

  const fillers = [document.createElement('span'), document.createElement('span')];
  fillers.forEach((e) => {
    e.style.float = 'left';
    e.style.height = '1px';
    e.style.width = '0.5%';
  });

  buttons.append(
    buttonMediaBack,
    buttonBack,
    fillers[0],
    buttonPause,
    fillers[1],
    buttonForth,
    buttonMediaForth,
  );

  buttonBack.addEventListener('touchstart', () => start('back'));
  buttonBack.addEventListener('touchend', end);
  buttonBack.addEventListener('click', () => {
    clickEvent.value = 'back';
    buttons.dispatchEvent(clickEvent);
  });
  buttonBack.addEventListener('touchcancel', end);

  buttonForth.addEventListener('touchstart', () => start('forth'));
  buttonForth.addEventListener('touchend', end);
  buttonForth.addEventListener('click', () => {
    clickEvent.value = 'forth';
    buttons.dispatchEvent(clickEvent);
  });
  buttonForth.addEventListener('touchcancel', end);

  buttonMediaBack.addEventListener('click', () => {
    clickEvent.value = 'mediaback';
    buttons.dispatchEvent(clickEvent);
  });
  buttonMediaForth.addEventListener('click', () => {
    clickEvent.value = 'mediaforth';
    buttons.dispatchEvent(clickEvent);
  });

  const buttonsArray = [buttonBack, buttonForth, buttonMediaBack, buttonMediaForth];

  let timeout;
  let startTime;

  buttonPause.addEventListener('pointerdown', () => {
    startTime = new Date();
    timeout = setTimeout(() => toggleDisplay(buttonsArray), buttonschangeCLICKINTERVAL);
  });

  buttonPause.addEventListener('pointerup', () => {
    clearTimeout(timeout);
    if ((new Date()).getTime() - startTime.getTime() < buttonschangeCLICKINTERVAL) {
      clickEvent.value = 'pause';
      buttons.dispatchEvent(clickEvent);
    }
  });
}

export default createKeys;
