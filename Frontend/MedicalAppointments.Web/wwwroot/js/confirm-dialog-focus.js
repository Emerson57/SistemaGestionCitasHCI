window.confirmDialogFocus = {
  lastElement: null,
  capture() {
    const el = document.activeElement;
    this.lastElement = el instanceof HTMLElement ? el : null;
  },
  restore() {
    if (this.lastElement && typeof this.lastElement.focus === 'function') {
      this.lastElement.focus();
    }
    this.lastElement = null;
  }
};
