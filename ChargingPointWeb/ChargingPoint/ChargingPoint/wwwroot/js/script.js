// script.js - Fixed and Complete
document.addEventListener("DOMContentLoaded", () => {
    const sidebar = document.querySelector(".sidebar");
    const sidebarToggleBtn = document.querySelectorAll(".sidebar-toggle"); // Lỗi: đánh máy sai
    const themeToggleBtn = document.querySelector(".theme-toggle");
    const searchForm = document.querySelector(".search-form");

    // Kiểm tra elements có tồn tại không
    if (!sidebar || !themeToggleBtn) {
        console.error("Sidebar hoặc theme toggle button không tìm thấy!");
        return;
    }

    const themeIcon = themeToggleBtn.querySelector(".theme-icon");

    // Hàm cập nhật icon theme
    const updateThemeIcon = () => {
        const isDark = document.body.classList.contains("dark-theme");
        if (themeIcon) {
            if (sidebar.classList.contains("collapsed")) {
                themeIcon.textContent = isDark ? "light_mode" : "dark_mode";
            } else {
                themeIcon.textContent = isDark ? "light_mode" : "dark_mode";
            }
        }
    };

    // Load theme từ localStorage
    const savedTheme = localStorage.getItem("theme");
    const systemPrefersDark = window.matchMedia("(prefers-color-scheme: dark)").matches;
    const shouldUseDarkTheme = savedTheme === "dark" || (!savedTheme && systemPrefersDark);

    if (shouldUseDarkTheme) {
        document.body.classList.add("dark-theme");
    }
    updateThemeIcon();

    // Toggle sidebar khi click button
    sidebarToggleBtn.forEach((btn) => {
        btn.addEventListener("click", (e) => {
            e.preventDefault();
            sidebar.classList.toggle("collapsed");
            document.body.classList.toggle("sidebar-collapsed");

            // Lưu trạng thái sidebar
            const isCollapsed = sidebar.classList.contains("collapsed");
            localStorage.setItem("sidebarCollapsed", isCollapsed);

            updateThemeIcon();
            console.log("Sidebar toggled:", isCollapsed ? "collapsed" : "expanded");
        });
    });

    // Load trạng thái sidebar từ localStorage
    const savedSidebarState = localStorage.getItem("sidebarCollapsed");
    if (savedSidebarState === "true") {
        sidebar.classList.add("collapsed");
        document.body.classList.add("sidebar-collapsed");
    }

    // Expand sidebar khi click vào search form (khi collapsed)
    if (searchForm) {
        searchForm.addEventListener("click", () => {
            if (sidebar.classList.contains("collapsed")) {
                sidebar.classList.remove("collapsed");
                document.body.classList.remove("sidebar-collapsed");
                localStorage.setItem("sidebarCollapsed", false);

                // Focus vào input sau khi expand
                setTimeout(() => {
                    const input = searchForm.querySelector("input");
                    if (input) input.focus();
                }, 100);
            }
        });
    }

    // Toggle theme (dark/light mode)
    themeToggleBtn.addEventListener("click", () => {
        const isDark = document.body.classList.toggle("dark-theme");
        localStorage.setItem("theme", isDark ? "dark" : "light");
        updateThemeIcon();

        // Animate theme toggle indicator
        const indicator = themeToggleBtn.querySelector(".theme-toggle-indicator");
        if (indicator) {
            indicator.style.transform = isDark ? "translateX(24px)" : "translateX(0)";
        }

        console.log("Theme toggled:", isDark ? "dark" : "light");
    });

    // Set initial theme toggle indicator position
    const indicator = themeToggleBtn.querySelector(".theme-toggle-indicator");
    if (indicator && document.body.classList.contains("dark-theme")) {
        indicator.style.transform = "translateX(24px)";
    }

    // Submenu toggle
    const hasSubmenuItems = document.querySelectorAll(".has-submenu > .menu-link");
    hasSubmenuItems.forEach(link => {
        link.addEventListener("click", (e) => {
            e.preventDefault();

            // Không mở submenu khi sidebar collapsed
            if (sidebar.classList.contains("collapsed")) {
                return;
            }

            const parentItem = link.closest(".has-submenu");
            const isOpen = parentItem.classList.contains("open");

            // Đóng tất cả submenu khác
            document.querySelectorAll(".has-submenu").forEach(item => {
                if (item !== parentItem) {
                    item.classList.remove("open");
                }
            });

            // Toggle submenu hiện tại
            parentItem.classList.toggle("open");

            console.log("Submenu toggled:", isOpen ? "closed" : "opened");
        });
    });

    // Set active menu dựa trên URL hiện tại
    const setActiveMenu = () => {
        const currentPath = window.location.pathname.toLowerCase();

        // Remove all active classes
        document.querySelectorAll(".menu-link, .submenu-link").forEach(link => {
            link.classList.remove("active");
        });

        // Set active cho link hiện tại
        document.querySelectorAll(".menu-link, .submenu-link").forEach(link => {
            const href = link.getAttribute("href");
            if (href && href !== "#" && currentPath.includes(href.toLowerCase())) {
                link.classList.add("active");

                // Nếu là submenu, mở parent
                const parentSubmenu = link.closest(".has-submenu");
                if (parentSubmenu) {
                    parentSubmenu.classList.add("open");
                }
            }
        });
    };

    setActiveMenu();

    // Mobile: Click outside sidebar để đóng
    document.addEventListener("click", (e) => {
        if (window.innerWidth <= 768) {
            if (!sidebar.contains(e.target) && !e.target.closest(".sidebar-toggle")) {
                if (!sidebar.classList.contains("collapsed")) {
                    sidebar.classList.add("collapsed");
                    document.body.classList.add("sidebar-collapsed");
                }
            }
        }
    });

    // Responsive: Auto adjust khi resize window
    let resizeTimer;
    window.addEventListener("resize", () => {
        clearTimeout(resizeTimer);
        resizeTimer = setTimeout(() => {
            if (window.innerWidth > 768) {
                // Desktop: restore saved state
                const savedState = localStorage.getItem("sidebarCollapsed");
                if (savedState === "true") {
                    sidebar.classList.add("collapsed");
                    document.body.classList.add("sidebar-collapsed");
                } else {
                    sidebar.classList.remove("collapsed");
                    document.body.classList.remove("sidebar-collapsed");
                }
            } else {
                // Mobile: always collapsed by default
                sidebar.classList.add("collapsed");
                document.body.classList.add("sidebar-collapsed");
            }
        }, 250);
    });

    console.log("Sidebar script initialized successfully");
});