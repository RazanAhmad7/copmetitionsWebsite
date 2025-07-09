// Admin Panel State
let currentTab = "questions";
let isEditing = false;
let currentQuestionId = null;

// DOM Elements
const addQuestionBtn = document.getElementById("addQuestionBtn");
const questionForm = document.querySelector(".question-form");
const questionsList = document.querySelector(".questions-list");
const questionTypeSelect = document.getElementById("questionType");
const multipleChoiceOptions = document.getElementById("multipleChoiceOptions");
const matchingPairs = document.getElementById("matchingPairs");
const wordBuildingLetters = document.getElementById("wordBuildingLetters");

// Event Listeners
document.addEventListener("DOMContentLoaded", () => {
  // Tab switching
  document.querySelectorAll(".admin-tab").forEach((tab) => {
    tab.addEventListener("click", () => switchTab(tab.dataset.tab));
  });

  // Add question button
  addQuestionBtn.addEventListener("click", showQuestionForm);

  // Question type change
  questionTypeSelect.addEventListener("change", handleQuestionTypeChange);

  // Form submission
  questionForm.addEventListener("submit", handleFormSubmit);

  // Add option buttons
  document.querySelectorAll(".add-option").forEach((btn) => {
    btn.addEventListener("click", () => addOption(btn));
  });

  // Remove option buttons
  document.querySelectorAll(".remove-option").forEach((btn) => {
    btn.addEventListener("click", () => removeOption(btn));
  });

  // Edit and delete buttons
  document.querySelectorAll(".edit-btn").forEach((btn) => {
    btn.addEventListener("click", () => editQuestion(btn));
  });

  document.querySelectorAll(".delete-btn").forEach((btn) => {
    btn.addEventListener("click", () => deleteQuestion(btn));
  });
});

// Switch between tabs
function switchTab(tabName) {
  currentTab = tabName;

  // Update active tab
  document.querySelectorAll(".admin-tab").forEach((tab) => {
    tab.classList.toggle("active", tab.dataset.tab === tabName);
  });

  // Show appropriate content
  // In a real application, this would load different content based on the tab
  console.log(`Switched to ${tabName} tab`);
}

// Show question form
function showQuestionForm() {
  questionsList.style.display = "none";
  questionForm.style.display = "grid";
  isEditing = false;
  currentQuestionId = null;
  questionForm.reset();
  handleQuestionTypeChange();
}

// Handle question type change
function handleQuestionTypeChange() {
  const type = questionTypeSelect.value;

  // Hide all question type specific sections
  multipleChoiceOptions.style.display = "none";
  matchingPairs.style.display = "none";
  wordBuildingLetters.style.display = "none";

  // Show the selected type
  switch (type) {
    case "multipleChoice":
      multipleChoiceOptions.style.display = "block";
      break;
    case "matching":
      matchingPairs.style.display = "block";
      break;
    case "wordBuilding":
      wordBuildingLetters.style.display = "block";
      break;
  }
}

// Add new option
function addOption(button) {
  const container = button.closest(".form-group");
  const optionsList = container.querySelector(
    ".options-list, .matching-items, .matching-answers"
  );

  const optionItem = document.createElement("div");
  optionItem.className = "option-item";

  if (container.id === "multipleChoiceOptions") {
    optionItem.innerHTML = `
            <input type="text" placeholder="الخيار الجديد" required>
            <input type="radio" name="correctAnswer" value="${optionsList.children.length}" required>
            <button type="button" class="remove-option"><i class="fas fa-times"></i></button>
        `;
  } else {
    optionItem.innerHTML = `
            <input type="text" placeholder="${
              container.id === "matchingPairs" ? "عنصر جديد" : "إجابة جديدة"
            }" required>
            <button type="button" class="remove-option"><i class="fas fa-times"></i></button>
        `;
  }

  optionsList.appendChild(optionItem);

  // Add event listener to new remove button
  optionItem
    .querySelector(".remove-option")
    .addEventListener("click", () =>
      removeOption(optionItem.querySelector(".remove-option"))
    );
}

// Remove option
function removeOption(button) {
  const optionItem = button.closest(".option-item");
  if (optionItem) {
    optionItem.remove();
  }
}

// Edit question
function editQuestion(button) {
  const questionCard = button.closest(".question-card");
  const questionInfo = questionCard.querySelector(".question-info");

  // In a real application, this would fetch the question data from the backend
  const questionData = {
    id: questionCard.dataset.id,
    type: questionInfo.querySelector("p").textContent.split(": ")[1],
    text: questionInfo.querySelector("h3").textContent,
    category: questionInfo.querySelectorAll("p")[1].textContent.split(": ")[1],
  };

  // Show form and populate with question data
  showQuestionForm();
  isEditing = true;
  currentQuestionId = questionData.id;

  // Populate form fields
  questionTypeSelect.value = questionData.type;
  document.getElementById("questionText").value = questionData.text;
  document.getElementById("questionCategory").value = questionData.category;

  handleQuestionTypeChange();
}

// Delete question
function deleteQuestion(button) {
  if (confirm("هل أنت متأكد من حذف هذا السؤال؟")) {
    const questionCard = button.closest(".question-card");
    // In a real application, this would send a delete request to the backend
    questionCard.remove();
  }
}

// Handle form submission
function handleFormSubmit(e) {
  e.preventDefault();

  const formData = new FormData(e.target);
  const questionData = {
    type: formData.get("questionType"),
    text: formData.get("questionText"),
    category: formData.get("questionCategory"),
    options: [],
    correctAnswer: formData.get("correctAnswer"),
  };

  // Collect options based on question type
  switch (questionData.type) {
    case "multipleChoice":
      document
        .querySelectorAll(
          '#multipleChoiceOptions .option-item input[type="text"]'
        )
        .forEach((input) => {
          questionData.options.push(input.value);
        });
      break;
    case "matching":
      const items = [];
      const answers = [];
      document
        .querySelectorAll(".matching-items input")
        .forEach((input) => items.push(input.value));
      document
        .querySelectorAll(".matching-answers input")
        .forEach((input) => answers.push(input.value));
      questionData.pairs = items.map((item, index) => ({
        item,
        answer: answers[index],
      }));
      break;
    case "wordBuilding":
      const letters = [];
      document.querySelectorAll(".letter-input").forEach((input) => {
        if (input.value) letters.push(input.value);
      });
      questionData.letters = letters;
      questionData.correctWord = document.getElementById("correctWord").value;
      break;
  }

  // In a real application, this would send the data to the backend
  console.log("Question data:", questionData);

  // Add the question to the list
  addQuestionToList(questionData);

  // Reset form and hide it
  e.target.reset();
  questionForm.style.display = "none";
  questionsList.style.display = "grid";
}

// Add question to the list
function addQuestionToList(questionData) {
  const questionCard = document.createElement("div");
  questionCard.className = "question-card";
  questionCard.dataset.id = currentQuestionId || Date.now();

  questionCard.innerHTML = `
        <div class="question-info">
            <h3>${questionData.text}</h3>
            <p>نوع السؤال: ${questionData.type}</p>
            <p>القسم: ${questionData.category}</p>
        </div>
        <div class="question-actions">
            <button class="action-btn edit-btn"><i class="fas fa-edit"></i></button>
            <button class="action-btn delete-btn"><i class="fas fa-trash"></i></button>
        </div>
    `;

  // Add event listeners to new buttons
  questionCard
    .querySelector(".edit-btn")
    .addEventListener("click", () =>
      editQuestion(questionCard.querySelector(".edit-btn"))
    );
  questionCard
    .querySelector(".delete-btn")
    .addEventListener("click", () =>
      deleteQuestion(questionCard.querySelector(".delete-btn"))
    );

  questionsList.appendChild(questionCard);
}
