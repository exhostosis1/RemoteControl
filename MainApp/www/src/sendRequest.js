function sendRequest(controller, action, param) {
  return fetch(`/api/v1/${controller}/${action}/${param === undefined ? '' : param}`);
}

export default sendRequest;
