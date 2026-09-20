document.addEventListener('DOMContentLoaded', function () {

    const textToType = "Personalized support. Smarter recovery.";

    const element = document.getElementById('typewriter-text');

    // Element না থাকলে function run করবে না
    if (!element) {
        return;
    }

    let charIndex = 0;
    let isDeleting = false;

    function typeEffect() {

        const currentSpeed = isDeleting ? 40 : 100;

        if (!isDeleting && charIndex <= textToType.length) {

            element.textContent = textToType.substring(0, charIndex);

            charIndex++;

            if (charIndex > textToType.length) {

                isDeleting = true;

                setTimeout(typeEffect, 2000);

                return;
            }
        }
        else if (isDeleting && charIndex >= 0) {

            element.textContent = textToType.substring(0, charIndex);

            charIndex--;

            if (charIndex < 0) {

                isDeleting = false;

                setTimeout(typeEffect, 500);

                return;
            }
        }

        setTimeout(typeEffect, currentSpeed);
    }

    typeEffect();
});