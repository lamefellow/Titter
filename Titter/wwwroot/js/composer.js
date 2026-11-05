const box = document.getElementById('content');
const counter = document.getElementById('counter');
const btn = document.getElementById('post-btn');

if (box) {
    const limit = parseInt(box.getAttribute('maxlength'), 10);

    const update = () => {
        box.style.height = 'auto';
        box.style.height = box.scrollHeight + 'px';
        if (counter && limit) counter.textContent = limit - box.value.length;
        if (btn) btn.disabled = box.value.trim().length === 0;
    };

    box.addEventListener('input', update);
    update();
}