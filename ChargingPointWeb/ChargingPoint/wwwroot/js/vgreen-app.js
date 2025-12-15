document.addEventListener('DOMContentLoaded', () => {
    const sidebar = document.getElementById('sidebar');
    const openBtn = document.getElementById('sidebarOpen');
    const closeBtn = document.getElementById('sidebarClose');
    const overlay = document.getElementById('sidebarOverlay');
    const themeBtn = document.getElementById('themeToggle');

    // Mobile sidebar
    openBtn?.addEventListener('click', () => {
        sidebar.classList.remove('-translate-x-full');
        overlay.classList.add('active');
    });

    closeBtn?.addEventListener('click', () => {
        sidebar.classList.add('-translate-x-full');
        overlay.classList.remove('active');
    });

    overlay?.addEventListener('click', () => {
        sidebar.classList.add('-translate-x-full');
        overlay.classList.remove('active');
    });

    // Dark mode
    if (localStorage.theme === 'dark' || (!('theme' in localStorage) && window.matchMedia('(prefers-color-scheme: dark)').matches)) {
        document.documentElement.classList.add('dark');
    }

    themeBtn?.addEventListener('click', () => {
        document.documentElement.classList.toggle('dark');
        localStorage.theme = document.documentElement.classList.contains('dark') ? 'dark' : 'light';
    });
});