window.addEventListener('devtoolschange', function(event) {
    alert('Developer tools are not allowed!');
});

document.addEventListener('contextmenu', function(event) {
    event.preventDefault();
});

// script.js
document.addEventListener('DOMContentLoaded', function() {
    // Delayed animation to allow CSS transition to take effect
    setTimeout(function() {
        document.querySelector('.buttons-container').style.left = '20px'; // Bring buttons up
    }, 500);
});

// script.js
function animateButton1(button) {
    button.classList.toggle('large'); // Toggle the 'large' class on button click
    window.location.href = "https://github.com/s6lite-hype/AFExplorer";
}

function animateButton2(button) {
    button.classList.toggle('large'); // Toggle the 'large' class on button click
    window.location.href = "https://github.com/s6lite-hype/AFExplorer/releases/tag/Preview-Release";
}