window.JsFunctions = {
    addKeyboardListenerEvent: function (foo) {
        let serializeEvent = function (e) {
            if (e) {
                return {
                    key: e.key,
                    code: e.keyCode.toString(),
                    location: e.location,
                    repeat: e.repeat,
                    ctrlKey: e.ctrlKey,
                    shiftKey: e.shiftKey,
                    altKey: e.altKey,
                    metaKey: e.metaKey,
                    type: e.type
                };
            }
        };

        window.document.addEventListener('keydown', function (e) {
            DotNet.invokeMethodAsync('Website', 'JsKeyDown', serializeEvent(e));
        });
    }
};

window.EnableKeyboardCapture = (componentRef) => {
  document.onkeydown = function (ev) {
    const serialized = serializeEvent(ev);
    console.log(ev.key);
    raiseKeyDownEvent(ev, serialized);
  }

  function raiseKeyDownEvent(ev, serialized) {
    componentRef.invokeMethodAsync('JsKeyDown', serialized);
  }

  function serializeEvent(ev) {
    return {
      key:      ev.key,
      code:     ev.keyCode.toString(),
      location: ev.location,
      repeat:   ev.repeat,
      ctrlKey:  ev.ctrlKey,
      shiftKey: ev.shiftKey,
      altKey:   ev.altKey,
      metaKey:  ev.metaKey,
      type:     ev.type
    }
  }
}