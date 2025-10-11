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