const box = document.getElementById('content');
const counter = document.getElementById('counter');

box.addEventListener('input', () => {
    box.style.height = 'auto';
    box.style.height = box.scrollHeight + 'px';
    counter.textContent = 1000 - box.value.length;
});