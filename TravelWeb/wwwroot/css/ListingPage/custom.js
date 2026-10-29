function f(id, btn) {debugger
    const panel = document.getElementById(id);
    if (!panel) return;
    const isOpen = panel.classList.contains('open');
    document.querySelectorAll('.backdrop-modal').forEach(el => el.remove());
    document.querySelectorAll('.details-panel').forEach(p => {
        p.classList.remove('open');
    });
    document.querySelectorAll('.btn-details').forEach(b => {
        b.classList.remove('open');
        const textNode = b.childNodes[0];
        if (textNode && textNode.nodeType === Node.TEXT_NODE) {
            textNode.textContent = 'Show Details ';
        }

    });

    if (!isOpen) {
        panel.classList.add('open');
        btn.classList.add('open');
        const textNode = btn.childNodes[0];
        if (textNode && textNode.nodeType === Node.TEXT_NODE) {
            textNode.textContent = 'Hide Details ';
        }
        const backdrop = document.createElement('div');
        backdrop.className = 'backdrop-modal';
        panel.insertAdjacentElement('afterend', backdrop);
        backdrop.addEventListener('click', () => {
            panel.classList.remove('open');
            btn.classList.remove('open');
            if (textNode && textNode.nodeType === Node.TEXT_NODE) {
                textNode.tfextContent = 'Show Details ';
            }
            backdrop.remove();
        });
        setTimeout(() => {
            panel.scrollIntoView({
                behavior: 'smooth',
                block: 'nearest'
            });
        }, 100);
    }
}

// =====================================================
// DETAILS TAB SWITCH (Itinerary / Fare / Baggage / Price)
// =====================================================
function switchTab(tabEl, contentId) {
    const detailsPanel = tabEl.closest('.details-panel');
    if (!detailsPanel) return;

    // Deactivate all tabs & content panes within this card
    detailsPanel.querySelectorAll('.tab-link').forEach(t => t.classList.remove('active'));
    detailsPanel.querySelectorAll('.tab-content').forEach(c => c.classList.remove('active'));

    // Activate the clicked tab and its content
    tabEl.classList.add('active');
    const content = document.getElementById(contentId);
    if (content) content.classList.add('active');
}

