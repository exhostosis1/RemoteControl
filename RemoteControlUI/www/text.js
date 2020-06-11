const textEvent = new Event('textinput');

let textElement;

function processText(e) {
    if (e.key === 'Enter') {
        if (e.target.value) {
            textEvent.text = encodeURIComponent(e.target.value);
            textElement.dispatchEvent(textEvent);
        }
        e.target.blur();
    }
}

function createTextInput(element) {
    textElement = element;

    element.addEventListener('keyup', processText);
}

export default createTextInput;
