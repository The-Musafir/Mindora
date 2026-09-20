// ============================================================
// Mindora — Profile Page Controller
// Bio live counter + Avatar upload preview
// ============================================================
(function () {
    'use strict';

    document.addEventListener('DOMContentLoaded', () => {
        initBioCounter();
        initAvatarUpload();
    });

    // ============================================================
    // BIO COUNTER — Word + Character count
    // ============================================================
    function initBioCounter() {
        const bioInput = document.getElementById('bioInput');
        const wordCountEl = document.getElementById('bioWordCount');
        const charCountEl = document.getElementById('bioCharCount');

        if (!bioInput) return;

        function update() {
            const text = bioInput.value.trim();
            const words = text ? text.split(/\s+/).filter(w => w.length > 0).length : 0;
            const chars = bioInput.value.length;

            if (wordCountEl) {
                wordCountEl.textContent = `${words} ${words === 1 ? 'word' : 'words'}`;

                // Highlight green when 150+ words
                if (words >= 150) {
                    wordCountEl.style.color = '#10B981';
                } else {
                    wordCountEl.style.color = '#06B6D4';
                }
            }

            if (charCountEl) {
                charCountEl.textContent = chars;
            }
        }

        bioInput.addEventListener('input', update);
        update();
    }

    // ============================================================
    // AVATAR UPLOAD — preview before submit
    // ============================================================
    function initAvatarUpload() {
        const input = document.getElementById('avatarInput');
        if (!input) return;

        input.addEventListener('change', (e) => {
            const file = e.target.files?.[0];
            if (!file) return;

            // Client-side size check
            const maxSize = 2 * 1024 * 1024;
            if (file.size > maxSize) {
                alert('File is too large. Maximum size is 2 MB.');
                input.value = '';
                return;
            }

            // Optional: preview
            const reader = new FileReader();
            reader.onload = (evt) => {
                const img = document.querySelector('.profile-avatar-img');
                const initial = document.querySelector('.profile-avatar-initial');

                if (img) {
                    img.src = evt.target.result;
                } else if (initial) {
                    const newImg = document.createElement('img');
                    newImg.src = evt.target.result;
                    newImg.alt = 'Profile';
                    newImg.className = 'profile-avatar-img';
                    initial.replaceWith(newImg);
                }
            };
            reader.readAsDataURL(file);
        });
    }
})();