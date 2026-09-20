// ============================================================
// Mindora — "I'm Bored" Feature
// ============================================================
(function () {
    'use strict';

    // State
    let currentActivity = null;
    let currentSession = null;
    let selectedCategory = '';
    let timerInterval = null;
    let sessionStartTime = null;
    let selectedMood = null;

    // DOM elements
    const els = {};

    document.addEventListener('DOMContentLoaded', () => {
        cacheElements();
        bindEvents();
    });

    function cacheElements() {
        els.imBoredBtn = document.getElementById('imBoredBtn');
        els.skipBtn = document.getElementById('skipBtn');
        els.startBtn = document.getElementById('startBtn');
        els.completeBtn = document.getElementById('completeBtn');
        els.submitFeedbackBtn = document.getElementById('submitFeedbackBtn');
        els.moodSelector = document.getElementById('moodSelector');

        els.idleState = document.getElementById('idleState');
        els.loadingState = document.getElementById('loadingState');
        els.activityState = document.getElementById('activityState');
        els.sessionState = document.getElementById('sessionState');
        els.feedbackState = document.getElementById('feedbackState');

        els.categoryBadge = document.getElementById('categoryBadge');
        els.activityEmoji = document.getElementById('activityEmoji');
        els.activityTitle = document.getElementById('activityTitle');
        els.activityDesc = document.getElementById('activityDesc');
        els.activityDuration = document.getElementById('activityDuration');
        els.sessionTitle = document.getElementById('sessionTitle');

        els.timerText = document.getElementById('timerText');
        els.timerProgress = document.getElementById('timerProgress');

        // Category buttons
        document.querySelectorAll('.boredom-cat-btn').forEach(btn => {
            btn.addEventListener('click', () => {
                document.querySelectorAll('.boredom-cat-btn').forEach(b => b.classList.remove('active'));
                btn.classList.add('active');
                selectedCategory = btn.dataset.category || '';
            });
        });
    }

    function bindEvents() {
        els.imBoredBtn?.addEventListener('click', () => loadActivity());
        els.skipBtn?.addEventListener('click', () => loadActivity());
        els.startBtn?.addEventListener('click', startSession);
        els.completeBtn?.addEventListener('click', completeSession);
        els.submitFeedbackBtn?.addEventListener('click', submitFeedback);

        // Mood selector
        els.moodSelector?.querySelectorAll('.boredom-mood').forEach(btn => {
            btn.addEventListener('click', () => {
                els.moodSelector.querySelectorAll('.boredom-mood').forEach(b => b.classList.remove('selected'));
                btn.classList.add('selected');
                selectedMood = parseInt(btn.dataset.mood);
                els.submitFeedbackBtn.disabled = false;
            });
        });
    }

    // ============================================================
    // STATE SWITCHER
    // ============================================================
    function showState(state) {
        [els.idleState, els.loadingState, els.activityState, els.sessionState, els.feedbackState]
            .forEach(el => el?.classList.add('d-none'));

        state?.classList.remove('d-none');
    }

    // ============================================================
    // LOAD ACTIVITY
    // ============================================================
    async function loadActivity() {
        showState(els.loadingState);

        try {
            const url = `/Boredom/GetRandom${selectedCategory ? '?category=' + encodeURIComponent(selectedCategory) : ''}`;
            const res = await fetch(url, { credentials: 'include' });
            const data = await res.json();

            if (!data.success) {
                alert(data.message || 'No activity found.');
                showState(els.idleState);
                return;
            }

            currentActivity = data.activity;
            renderActivity(currentActivity);
            showState(els.activityState);

        } catch (err) {
            console.error('[Boredom] Error:', err);
            alert('Failed to load activity. Please try again.');
            showState(els.idleState);
        }
    }

    function renderActivity(activity) {
        els.categoryBadge.textContent = activity.category;
        els.categoryBadge.style.background = activity.categoryColor + '20';
        els.categoryBadge.style.color = activity.categoryColor;

        els.activityEmoji.textContent = activity.emoji;
        els.activityTitle.textContent = activity.title;
        els.activityDesc.textContent = activity.description || '';
        els.activityDuration.textContent = activity.durationMinutes
            ? `${activity.durationMinutes} minutes`
            : 'Take your time';
    }

    // ============================================================
    // START SESSION
    // ============================================================
    async function startSession() {
        if (!currentActivity) return;

        try {
            const res = await fetch('/Boredom/Start', {
                method: 'POST',
                credentials: 'include',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': getAntiForgeryToken()
                },
                body: JSON.stringify({
                    activityId: currentActivity.activityId,
                    moodBefore: null
                })
            });

            const data = await res.json();
            if (!data.success) {
                alert(data.message || 'Failed to start.');
                return;
            }

            currentSession = data.session;
            els.sessionTitle.textContent = currentActivity.title;
            showState(els.sessionState);

            startTimer(currentActivity.durationMinutes || 5);

        } catch (err) {
            console.error('[Boredom] Start error:', err);
            alert('Failed to start session.');
        }
    }

    // ============================================================
    // TIMER
    // ============================================================
    function startTimer(durationMinutes) {
        const totalSeconds = durationMinutes * 60;
        sessionStartTime = Date.now();

        timerInterval = setInterval(() => {
            const elapsed = Math.floor((Date.now() - sessionStartTime) / 1000);
            const remaining = Math.max(0, totalSeconds - elapsed);

            updateTimerDisplay(remaining, totalSeconds);

            if (remaining === 0) {
                clearInterval(timerInterval);
                timerInterval = null;
            }
        }, 1000);

        // Initial display
        updateTimerDisplay(totalSeconds, totalSeconds);
    }

    function updateTimerDisplay(remaining, total) {
        const mins = Math.floor(remaining / 60);
        const secs = remaining % 60;
        els.timerText.textContent = `${String(mins).padStart(2, '0')}:${String(secs).padStart(2, '0')}`;

        // Progress circle
        const circumference = 2 * Math.PI * 45;
        const progress = remaining / total;
        const offset = circumference * (1 - progress);
        if (els.timerProgress) {
            els.timerProgress.style.strokeDasharray = `${circumference} ${circumference}`;
            els.timerProgress.style.strokeDashoffset = offset;
        }
    }

    // ============================================================
    // COMPLETE SESSION
    // ============================================================
    function completeSession() {
        if (!currentSession) return;

        if (timerInterval) {
            clearInterval(timerInterval);
            timerInterval = null;
        }

        selectedMood = null;
        els.submitFeedbackBtn.disabled = true;
        els.moodSelector?.querySelectorAll('.boredom-mood').forEach(b => b.classList.remove('selected'));

        showState(els.feedbackState);
    }

    // ============================================================
    // SUBMIT FEEDBACK
    // ============================================================
    async function submitFeedback() {
        if (!currentSession || !selectedMood) return;

        try {
            const res = await fetch('/Boredom/Complete', {
                method: 'POST',
                credentials: 'include',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': getAntiForgeryToken()
                },
                body: JSON.stringify({
                    sessionId: currentSession.sessionId,
                    moodAfter: selectedMood
                })
            });

            const data = await res.json();

            if (data.success) {
                // Reset & show idle
                currentActivity = null;
                currentSession = null;
                setTimeout(() => {
                    showState(els.idleState);
                }, 500);
            } else {
                alert('Failed to save. Please try again.');
            }

        } catch (err) {
            console.error('[Boredom] Complete error:', err);
            alert('Failed to save. Please try again.');
        }
    }

    // ============================================================
    // HELPERS
    // ============================================================
    function getAntiForgeryToken() {
        const input = document.querySelector('input[name="__RequestVerificationToken"]');
        return input?.value || '';
    }
})();