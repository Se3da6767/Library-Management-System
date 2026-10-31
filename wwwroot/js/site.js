// Hide the success message automatically after a few seconds.
document.addEventListener('DOMContentLoaded', function () {
    const flash = document.querySelector('.flash');
    if (flash) {
        setTimeout(function () {
            flash.style.transition = 'opacity .4s';
            flash.style.opacity = '0';
            setTimeout(function () { flash.remove(); }, 400);
        }, 5000);
    }
});
