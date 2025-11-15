
    window.scrollLeftBy = (element, amount) => {
    element.scrollBy({ left: -amount, behavior: 'smooth' });
};
    window.scrollRightBy = (element, amount) => {
    element.scrollBy({ left: amount, behavior: 'smooth' });
};
    window.getScrollInfo = (element) => {
    return {
    scrollLeft: element.scrollLeft,
    scrollWidth: element.scrollWidth,
    clientWidth: element.clientWidth
};
};

