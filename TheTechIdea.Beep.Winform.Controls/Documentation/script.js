// Navigation functionality
document.addEventListener('DOMContentLoaded', function() {
    const navLinks = document.querySelectorAll('.nav-link');
    const sections = document.querySelectorAll('.section');
    const submenuItems = document.querySelectorAll('.has-submenu');

    // Handle navigation clicks
    navLinks.forEach(link => {
        link.addEventListener('click', function(e) {
            e.preventDefault();
            
            const targetId = this.getAttribute('href').substring(1);
            const targetSection = document.getElementById(targetId);
            
            if (targetSection) {
                // Remove active class from all sections and nav links
                sections.forEach(section => section.classList.remove('active'));
                navLinks.forEach(navLink => navLink.classList.remove('active'));
                
                // Add active class to target section and clicked link
                targetSection.classList.add('active');
                this.classList.add('active');
                
                // Update page title
                const sectionTitle = targetSection.querySelector('h1');
                if (sectionTitle) {
                    document.title = `${sectionTitle.textContent} - Beep Controls Documentation`;
                }
                
                // Scroll to top of content
                document.querySelector('.content').scrollTop = 0;
            }
        });
    });

    // Handle submenu toggles
    submenuItems.forEach(item => {
        const link = item.querySelector('a');
        const submenu = item.querySelector('.submenu');
        
        if (link && submenu) {
            link.addEventListener('click', function(e) {
                // If the link has an href that points to a section, navigate there
                const href = this.getAttribute('href');
                if (href && href !== '#' && document.getElementById(href.substring(1))) {
                    return; // Let the navigation happen
                }
                
                // Otherwise, toggle the submenu
                e.preventDefault();
                item.classList.toggle('open');
            });
        }
    });

    // Handle hash changes (for direct links)
    window.addEventListener('hashchange', function() {
        const hash = window.location.hash.substring(1);
        if (hash) {
            const targetLink = document.querySelector(`[href="#${hash}"]`);
            if (targetLink) {
                targetLink.click();
            }
        }
    });

    // Initialize with current hash if present
    if (window.location.hash) {
        const hash = window.location.hash.substring(1);
        const targetLink = document.querySelector(`[href="#${hash}"]`);
        if (targetLink) {
            targetLink.click();
        }
    }

    // Mobile menu toggle (for responsive design)
    function createMobileMenuToggle() {
        const sidebar = document.querySelector('.sidebar');
        const content = document.querySelector('.content');
        
        // Create mobile menu button
        const mobileToggle = document.createElement('button');
        mobileToggle.className = 'mobile-menu-toggle';
        mobileToggle.innerHTML = '?';
        mobileToggle.style.cssText = `
            display: none;
            position: fixed;
            top: 20px;
            left: 20px;
            z-index: 1001;
            background: #2c3e50;
            color: white;
            border: none;
            padding: 10px 15px;
            border-radius: 5px;
            font-size: 18px;
            cursor: pointer;
        `;
        
        document.body.appendChild(mobileToggle);
        
        mobileToggle.addEventListener('click', function() {
            sidebar.classList.toggle('open');
        });
        
        // Close sidebar when clicking on content on mobile
        content.addEventListener('click', function() {
            if (window.innerWidth <= 768) {
                sidebar.classList.remove('open');
            }
        });
        
        // Show/hide mobile toggle based on screen size
        function checkScreenSize() {
            if (window.innerWidth <= 768) {
                mobileToggle.style.display = 'block';
            } else {
                mobileToggle.style.display = 'none';
                sidebar.classList.remove('open');
            }
        }
        
        window.addEventListener('resize', checkScreenSize);
        checkScreenSize();
    }
    
    createMobileMenuToggle();

    // Search functionality (basic)
    function addSearchFunctionality() {
        const searchInput = document.createElement('input');
        searchInput.type = 'text';
        searchInput.placeholder = 'Search documentation...';
        searchInput.className = 'search-input';
        searchInput.style.cssText = `
            width: calc(100% - 40px);
            padding: 10px;
            margin: 0 20px 20px 20px;
            border: 1px solid #34495e;
            border-radius: 5px;
            background: #34495e;
            color: white;
            font-size: 14px;
        `;
        
        const sidebar = document.querySelector('.sidebar');
        const logo = sidebar.querySelector('.logo');
        logo.after(searchInput);
        
        searchInput.addEventListener('input', function() {
            const searchTerm = this.value.toLowerCase();
            const menuItems = document.querySelectorAll('.nav-menu li');
            
            menuItems.forEach(item => {
                const text = item.textContent.toLowerCase();
                if (text.includes(searchTerm) || searchTerm === '') {
                    item.style.display = 'block';
                } else {
                    item.style.display = 'none';
                }
            });
        });
    }
    
    addSearchFunctionality();

    // Smooth scrolling for internal links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            const targetId = this.getAttribute('href').substring(1);
            const targetElement = document.getElementById(targetId);
            
            if (targetElement && targetElement.closest('.section')) {
                e.preventDefault();
                
                // Navigate to the section first
                const navLink = document.querySelector(`.nav-link[href="#${targetId}"]`);
                if (navLink) {
                    navLink.click();
                    
                    // Then scroll to the specific element within the section
                    setTimeout(() => {
                        const content = document.querySelector('.content');
                        const elementRect = targetElement.getBoundingClientRect();
                        const contentRect = content.getBoundingClientRect();
                        
                        content.scrollTo({
                            top: content.scrollTop + elementRect.top - contentRect.top - 20,
                            behavior: 'smooth'
                        });
                    }, 100);
                }
            }
        });
    });

    // Code copy functionality
    function addCodeCopyButtons() {
        const codeBlocks = document.querySelectorAll('pre code');
        
        codeBlocks.forEach(codeBlock => {
            const pre = codeBlock.parentElement;
            const copyButton = document.createElement('button');
            copyButton.textContent = 'Copy';
            copyButton.className = 'copy-button';
            copyButton.style.cssText = `
                position: absolute;
                top: 10px;
                right: 10px;
                background: #3498db;
                color: white;
                border: none;
                padding: 5px 10px;
                border-radius: 3px;
                font-size: 12px;
                cursor: pointer;
                opacity: 0.8;
                transition: opacity 0.3s;
            `;
            
            pre.style.position = 'relative';
            pre.appendChild(copyButton);
            
            copyButton.addEventListener('click', async function() {
                try {
                    await navigator.clipboard.writeText(codeBlock.textContent);
                    this.textContent = 'Copied!';
                    setTimeout(() => {
                        this.textContent = 'Copy';
                    }, 2000);
                } catch (err) {
                    console.error('Failed to copy code: ', err);
                }
            });
            
            copyButton.addEventListener('mouseenter', function() {
                this.style.opacity = '1';
            });
            
            copyButton.addEventListener('mouseleave', function() {
                this.style.opacity = '0.8';
            });
        });
    }
    
    // Add copy buttons after a short delay to ensure Prism.js has processed the code
    setTimeout(addCodeCopyButtons, 1000);

    // Table of contents generation for long sections
    function generateTableOfContents() {
        const sections = document.querySelectorAll('.section');
        
        sections.forEach(section => {
            const headings = section.querySelectorAll('h2, h3');
            if (headings.length > 3) {
                const toc = document.createElement('div');
                toc.className = 'table-of-contents';
                toc.innerHTML = '<h4>Table of Contents</h4>';
                
                const tocList = document.createElement('ul');
                tocList.style.cssText = `
                    background: #f8f9fa;
                    padding: 15px;
                    border-radius: 5px;
                    border-left: 4px solid #3498db;
                    margin: 20px 0;
                `;
                
                headings.forEach((heading, index) => {
                    const id = heading.id || `heading-${index}`;
                    heading.id = id;
                    
                    const listItem = document.createElement('li');
                    const link = document.createElement('a');
                    link.href = `#${id}`;
                    link.textContent = heading.textContent;
                    link.style.cssText = `
                        text-decoration: none;
                        color: #3498db;
                        ${heading.tagName === 'H3' ? 'margin-left: 20px; font-size: 0.9em;' : ''}
                    `;
                    
                    listItem.appendChild(link);
                    tocList.appendChild(listItem);
                });
                
                toc.appendChild(tocList);
                
                const firstH2 = section.querySelector('h2');
                if (firstH2) {
                    firstH2.after(toc);
                }
            }
        });
    }
    
    generateTableOfContents();
});

// Theme toggle functionality (optional)
function addThemeToggle() {
    const themeToggle = document.createElement('button');
    themeToggle.innerHTML = '??';
    themeToggle.className = 'theme-toggle';
    themeToggle.style.cssText = `
        position: fixed;
        top: 20px;
        right: 20px;
        background: #3498db;
        color: white;
        border: none;
        padding: 10px;
        border-radius: 50%;
        font-size: 16px;
        cursor: pointer;
        z-index: 1001;
        width: 40px;
        height: 40px;
        display: flex;
        align-items: center;
        justify-content: center;
    `;
    
    document.body.appendChild(themeToggle);
    
    themeToggle.addEventListener('click', function() {
        document.body.classList.toggle('dark-theme');
        this.innerHTML = document.body.classList.contains('dark-theme') ? '??' : '??';
    });
}

// Uncomment to enable theme toggle
// addThemeToggle();