// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

// Show loading indicator when forms are submitted
document.addEventListener('DOMContentLoaded', function() {
    const searchForm = document.getElementById('searchForm');
    const loadDataForm = document.getElementById('loadDataForm');
    const searchLoading = document.getElementById('searchLoading');
    const loadLoading = document.getElementById('loadLoading');

    if (searchForm) {
        searchForm.addEventListener('submit', function() {
            if (searchLoading) {
                searchLoading.classList.add('active');
            }
        });
    }

    if (loadDataForm) {
        loadDataForm.addEventListener('submit', function() {
            if (loadLoading) {
                loadLoading.classList.add('active');
                
                // Disable the button to prevent multiple submissions
                const loadButton = loadDataForm.querySelector('button[type="submit"]');
                if (loadButton) {
                    loadButton.disabled = true;
                    loadButton.textContent = 'Loading Data...';
                }
            }
        });
    }

    // Mobile menu toggle
    const menuToggle = document.getElementById('menuToggle');
    const closeSidebar = document.getElementById('closeSidebar');
    const sidebar = document.getElementById('sidebar');

    if (menuToggle && sidebar) {
        menuToggle.addEventListener('click', function() {
            sidebar.classList.add('active');
        });
    }

    if (closeSidebar && sidebar) {
        closeSidebar.addEventListener('click', function() {
            sidebar.classList.remove('active');
        });
    }

    // Close sidebar when clicking a menu item on mobile
    const sidebarLinks = document.querySelectorAll('.sidebar-menu a');
    if (sidebarLinks.length > 0 && sidebar) {
        sidebarLinks.forEach(link => {
            link.addEventListener('click', function() {
                if (window.innerWidth < 768) {
                    sidebar.classList.remove('active');
                }
            });
        });
    }
});
