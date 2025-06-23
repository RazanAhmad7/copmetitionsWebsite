// Profile state
let userProfile = null;
let quizHistory = [];

// DOM Elements
const profileContainer = document.querySelector(".profile-container");

// Event Listeners
document.addEventListener("DOMContentLoaded", () => {
  // Check if user is logged in
  if (!isLoggedIn()) {
    window.location.href = "login.html";
    return;
  }

  // Load user profile
  loadUserProfile();

  // Load quiz history
  loadQuizHistory();
});

// Load user profile
function loadUserProfile() {
  // In a real application, this would be an API call to the backend
  // For demo purposes, we'll use mock data
  userProfile = {
    id: currentUser.id,
    name: currentUser.name,
    email: currentUser.email,
    joinDate: "يناير 2024",
    completedQuizzes: 15,
    totalPoints: 1250,
    rank: "المركز الثالث",
  };

  updateProfileUI();
}

// Load quiz history
function loadQuizHistory() {
  // In a real application, this would be an API call to the backend
  // For demo purposes, we'll use mock data
  quizHistory = [
    {
      quizName: "مسابقة القرآن الكريم",
      date: "15/01/2024",
      score: "95%",
      rank: "gold",
    },
    {
      quizName: "مسابقة السنة النبوية",
      date: "10/01/2024",
      score: "85%",
      rank: "silver",
    },
    {
      quizName: "مسابقة العقيدة",
      date: "05/01/2024",
      score: "80%",
      rank: "bronze",
    },
  ];

  updateQuizHistoryUI();
}

// Update profile UI
function updateProfileUI() {
  if (!profileContainer) return;

  const profileHeader = profileContainer.querySelector(".profile-header");
  if (profileHeader) {
    profileHeader.querySelector("h2").textContent = userProfile.name;
    profileHeader.querySelector(
      "p"
    ).textContent = `عضو منذ: ${userProfile.joinDate}`;
  }

  const infoCards = profileContainer.querySelectorAll(".info-card");
  if (infoCards.length >= 3) {
    infoCards[0].querySelector("p").textContent = userProfile.completedQuizzes;
    infoCards[1].querySelector("p").textContent = userProfile.totalPoints;
    infoCards[2].querySelector("p").textContent = userProfile.rank;
  }
}

// Update quiz history UI
function updateQuizHistoryUI() {
  const historyTable = document.querySelector(".history-table tbody");
  if (!historyTable) return;

  historyTable.innerHTML = quizHistory
    .map(
      (quiz) => `
        <tr>
            <td>${quiz.quizName}</td>
            <td>${quiz.date}</td>
            <td>${quiz.score}</td>
            <td><span class="badge badge-${quiz.rank}">المركز ${getRankText(
        quiz.rank
      )}</span></td>
        </tr>
    `
    )
    .join("");
}

// Get rank text
function getRankText(rank) {
  switch (rank) {
    case "gold":
      return "الأول";
    case "silver":
      return "الثاني";
    case "bronze":
      return "الثالث";
    default:
      return "";
  }
}

// Update profile
function updateProfile(profileData) {
  // In a real application, this would be an API call to the backend
  userProfile = {
    ...userProfile,
    ...profileData,
  };

  updateProfileUI();
}

// Add quiz result
function addQuizResult(quizResult) {
  // In a real application, this would be an API call to the backend
  quizHistory.unshift(quizResult);

  // Update user stats
  userProfile.completedQuizzes++;
  userProfile.totalPoints += calculatePoints(quizResult.score);

  // Update UI
  updateProfileUI();
  updateQuizHistoryUI();
}

// Calculate points from score
function calculatePoints(score) {
  // Remove % and convert to number
  const scoreValue = parseInt(score);

  // Calculate points based on score
  if (scoreValue >= 90) return 100;
  if (scoreValue >= 80) return 80;
  if (scoreValue >= 70) return 60;
  if (scoreValue >= 60) return 40;
  return 20;
}
