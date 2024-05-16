import sendRequest from './sendRequest.js';
import { init } from './createVolumeBar.js';

export function drawDevices(devices) {
  const devicesElement = document.getElementById('devices');
  devicesElement.innerHTML = '';

  devices.forEach((x) => {
    devicesElement.innerHTML += `
                <div class="device" onclick="setDevice('${x.Id}')">
                    <div class="device-icon">${x.IsCurrentControlDevice ? '✅' : '&#160'}</div>
                    <div>${x.Name}</div>
                </div>
                `;
  });
}

// eslint-disable-next-line no-unused-vars
async function setDevice(id) {
  document.getElementById('devices').style.display = 'none';

  const devices = JSON.parse(await (await sendRequest('audio', 'setdevice', id)).text());
  const volume = await (await sendRequest('audio', 'getvolume')).text();

  init(volume);

  drawDevices(devices);
}

export function showDevices() {
  const element = document.getElementById('devices');
  element.style.display = element.style.display === 'block' ? 'none' : 'block';
}
