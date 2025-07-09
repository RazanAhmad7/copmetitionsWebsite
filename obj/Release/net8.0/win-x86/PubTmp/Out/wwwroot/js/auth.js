// Authentication state
let currentUser = null;

// DOM Elements
const loginForm = document.getElementById("loginForm");

// Event Listeners
document.addEventListener("DOMContentLoaded", () => {
  // Check if user is logged in
  checkAuthStatus();

  // Login form submission
  if (loginForm) {
    loginForm.addEventListener("submit", handleLogin);
  }
});

// Check authentication status
function checkAuthStatus() {
  const user = localStorage.getItem("currentUser");
  if (user) {
    currentUser = JSON.parse(user);
    updateUIForLoggedInUser();
  } else {
    updateUIForLoggedOutUser();
  }
}

// Handle login
function handleLogin(e) {
  e.preventDefault();

  const email = document.getElementById("email").value;
  const password = document.getElementById("password").value;

  // In a real application, this would be an API call to the backend
  // For demo purposes, we'll use a mock login
  if (email && password) {
    // Mock successful login
    currentUser = {
      id: 1,
      name: "مستخدم تجريبي",
      email: email,
      role: "user",
    };

    // Save user to localStorage
    localStorage.setItem("currentUser", JSON.stringify(currentUser));

    // Update UI
    updateUIForLoggedInUser();

    // Redirect to home page
    window.location.href = "../index.html";
  } else {
    alert("الرجاء إدخال البريد الإلكتروني وكلمة المرور");
  }
}

// Handle logout
function handleLogout() {
  // Clear user data
  localStorage.removeItem("currentUser");
  currentUser = null;

  // Update UI
  updateUIForLoggedOutUser();

  // Redirect to home page
  window.location.href = "../index.html";
}

// Update UI for logged in user
function updateUIForLoggedInUser() {
  const navLinks = document.querySelector(".nav-links");
  if (navLinks) {
    navLinks.innerHTML = `
            <a href="../index.html">الرئيسية</a>
            <a href="profile.html">الملف الشخصي</a>
            ${
              currentUser.role === "admin"
                ? '<a href="admin.html">لوحة التحكم</a>'
                : ""
            }
            <a href="#" onclick="handleLogout()">تسجيل الخروج</a>
        `;
  }
}

// Update UI for logged out user
function updateUIForLoggedOutUser() {
  const navLinks = document.querySelector(".nav-links");
  if (navLinks) {
    navLinks.innerHTML = `
            <a href="../index.html">الرئيسية</a>
            <a href="login.html">تسجيل الدخول</a>
            <a href="register.html">إنشاء حساب</a>
        `;
  }
}

// Check if user is admin
function isAdmin() {
  return currentUser && currentUser.role === "admin";
}

// Protect admin routes
function protectAdminRoute() {
  if (!isAdmin()) {
    window.location.href = "../index.html";
  }
}

// Check if user is logged in
function isLoggedIn() {
  return currentUser !== null;
}

// Protect authenticated routes
function protectAuthRoute() {
  if (!isLoggedIn()) {
    window.location.href = "login.html";
  }
}
