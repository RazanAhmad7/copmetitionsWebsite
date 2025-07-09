// Quiz state management
let currentQuestion = 0;
let score = 0;
let timer = null;
let timeLeft = 0;

// ✅ Use questions from server
const questions = window.questionsFromServer;

// تعريف درجات الألوان المختلفة
const colorShades = [
  { bg: "#FFF3E0", border: "#FF9800" }, // برتقالي فاتح
  { bg: "#F3E5F5", border: "#9C27B0" }, // بنفسجي فاتح
  { bg: "#FFEBEE", border: "#F44336" }, // أحمر فاتح
  { bg: "#E0F7FA", border: "#00BCD4" }, // سماوي فاتح
  { bg: "#F1F8E9", border: "#8BC34A" }, // أخضر مصفر
  { bg: "#FFF8E1", border: "#FFC107" }, // أصفر فاتح
  { bg: "#E8EAF6", border: "#3F51B5" }, // نيلي فاتح
  { bg: "#FCE4EC", border: "#E91E63" }, // وردي فاتح
];

// دالة لخلط المصفوفة
function shuffleArray(array) {
  for (let i = array.length - 1; i > 0; i--) {
    const j = Math.floor(Math.random() * (i + 1));
    [array[i], array[j]] = [array[j], array[i]];
  }
  return array;
}


// Initialize the quiz
function initQuiz() {
  const urlParams = new URLSearchParams(window.location.search);
  const quizType = urlParams.get("type");

  // Set quiz title based on type
  const quizTitles = {
    quran: "مسابقة القرآن الكريم",
    sunnah: "مسابقة السنة النبوية",
    aqeedah: "مسابقة العقيدة",
    culture: "مسابقة الثقافة الإسلامية",
    puzzles: "الألغاز الإسلامية",
  };

  document.getElementById("quizTitle").textContent =
    quizTitles[quizType] || "المسابقة";

  // Start the quiz
  showQuestion();
  startTimer(60); // 60 seconds per question
}

// Display the current question
function showQuestion() {
    const container = document.getElementById("questionContainer");
    container.innerHTML = ""; // مسح السؤال السابق

    const question = questions[currentQuestion];

    const questionBox = document.createElement("div");
    questionBox.className = "question-type";
    questionBox.innerHTML = `
    <div class="question-text">${question.text}</div>
    <div class="options-container"></div>
  `;
    container.appendChild(questionBox);

    document.querySelector(".progress-bar-fill").style.width =
        ((currentQuestion + 1) / questions.length) * 100 + "%";

    switch (question.type.toLowerCase()) {
        case "mcq":
            setupMultipleChoice(question);
            break;
        case "matching":
            setupMatching(question);
            break;
        case "spelling":
            setupWordBuilding(question);
            break;
    }
}

// Hide all question types
function hideAllQuestionTypes() {
  document.querySelectorAll(".question-type").forEach((type) => {
    type.style.display = "none";
  });
}

// Setup multiple choice question
function setupMultipleChoice(question) {
  const optionsContainer = document.querySelector(".options-container");
  optionsContainer.innerHTML = "";

  question.options.forEach((option, index) => {
    const optionElement = document.createElement("div");
    optionElement.className = "option";
    optionElement.dataset.option = String.fromCharCode(65 + index); // A, B, C, D
    optionElement.textContent = option;
    optionElement.addEventListener("click", () => selectOption(optionElement));
    optionsContainer.appendChild(optionElement);
  });
}

// دالة لإعادة ترتيب البطاقات المتبقية
function reorderRemainingItems(leftColumn) {
  const unmatched = Array.from(
    leftColumn.querySelectorAll(".definition-item:not(.matched)")
  );
  const matched = Array.from(
    leftColumn.querySelectorAll(".definition-item.matched")
  );
  const itemHeight = unmatched[0]?.offsetHeight || 0;
  const gap = 16; // المسافة بين العناصر

  // إعادة ترتيب العناصر غير المتطابقة
  unmatched.forEach((item, index) => {
    item.style.transform = `translateY(${index * (itemHeight + gap)}px)`;
  });
}

// Setup matching question
function setupMatching(question) {
  const container = document.querySelector(".matching-container");
  container.innerHTML = "";

  // متغيرات لتتبع الاختيارات
  let selectedWord = null;
  let selectedDefinition = null;
  let matchCount = 0;

  // إنشاء العمود الأيمن (الكلمات)
  const rightColumn = document.createElement("div");
  rightColumn.className = "matching-column fixed-column";

  // إنشاء العمود الأيسر (التعريفات)
  const leftColumn = document.createElement("div");
  leftColumn.className = "matching-column movable-column";

  // إضافة الكلمات إلى العمود الأيمن
  question.items.forEach((item) => {
    const wordElement = document.createElement("div");
    wordElement.className = "matching-item word-item";
    wordElement.dataset.word = item.text;
    wordElement.textContent = item.text;

    // إضافة مستمع النقر
    wordElement.addEventListener("click", () => {
      if (wordElement.classList.contains("matched")) return;

      // إزالة التحديد السابق إن وجد
      document.querySelectorAll(".word-item.selected").forEach((el) => {
        if (el !== wordElement) el.classList.remove("selected");
      });

      // تبديل حالة التحديد
      wordElement.classList.toggle("selected");
      selectedWord = wordElement.classList.contains("selected")
        ? wordElement
        : null;

      // التحقق من وجود تطابق
      checkForMatch();
    });

    rightColumn.appendChild(wordElement);
  });

  // خلط التعريفات
  const definitions = [...question.matches];
  shuffleArray(definitions);

  // إضافة التعريفات إلى العمود الأيسر
  definitions.forEach((match) => {
    const definitionElement = document.createElement("div");
    definitionElement.className = "matching-item definition-item";
    definitionElement.dataset.definition = match.text;
    definitionElement.textContent = match.text;

    // إضافة مستمع النقر
    definitionElement.addEventListener("click", () => {
      if (definitionElement.classList.contains("matched")) return;

      // إزالة التحديد السابق إن وجد
      document.querySelectorAll(".definition-item.selected").forEach((el) => {
        if (el !== definitionElement) el.classList.remove("selected");
      });

      // تبديل حالة التحديد
      definitionElement.classList.toggle("selected");
      selectedDefinition = definitionElement.classList.contains("selected")
        ? definitionElement
        : null;

      // التحقق من وجود تطابق
      checkForMatch();
    });

    leftColumn.appendChild(definitionElement);
  });

  // دالة للتحقق من تطابق العناصر المحددة
  function checkForMatch() {
    if (!selectedWord || !selectedDefinition) return;

    const word = selectedWord.dataset.word;
    const definition = selectedDefinition.dataset.definition;
    const colorShade = colorShades[matchCount % colorShades.length];

    // تطبيق التنسيق على الزوج المحدد
    selectedWord.classList.remove("selected");
    selectedDefinition.classList.remove("selected");

    selectedWord.classList.add("matched");
    selectedDefinition.classList.add("matched");

    selectedWord.style.backgroundColor = colorShade.bg;
    selectedWord.style.borderColor = colorShade.border;
    selectedWord.style.color = colorShade.border;

    selectedDefinition.style.backgroundColor = colorShade.bg;
    selectedDefinition.style.borderColor = colorShade.border;
    selectedDefinition.style.color = colorShade.border;

    // زيادة عداد التطابقات
    matchCount++;

    // إعادة تعيين المتغيرات
    selectedWord = null;
    selectedDefinition = null;

    // التحقق من اكتمال جميع التطابقات
    if (matchCount === question.items.length) {
      setTimeout(() => {
        checkMatchingCompletion();
      }, 500);
    }
  }

  container.appendChild(leftColumn);
  container.appendChild(rightColumn);
}

// Setup word building question
function setupWordBuilding(question) {
  const container = document.querySelector(".word-building-container");
  container.innerHTML = "";

  // Shuffle letters
  const shuffledLetters = [...question.letters].sort(() => Math.random() - 0.5);

  shuffledLetters.forEach((letter) => {
    const letterBox = document.createElement("div");
    letterBox.className = "letter-box";
    letterBox.textContent = letter;
    letterBox.addEventListener("click", () => selectLetter(letterBox));
    container.appendChild(letterBox);
  });
}

// Handle letter selection in word building
function selectLetter(letterBox) {
  const wordPreview = document.querySelector(".word-preview");
  wordPreview.textContent += letterBox.textContent;
  letterBox.style.visibility = "hidden";
}

// Handle option selection in multiple choice
function selectOption(optionElement) {
  // Remove selection from other options
  document.querySelectorAll(".option").forEach((opt) => {
    opt.classList.remove("selected");
  });

  // Add selection to clicked option
  optionElement.classList.add("selected");
}

// Start the timer
function startTimer(seconds) {
  timeLeft = seconds;
  updateTimer();

  timer = setInterval(updateTimer, 1000);
}

function updateTimer() {
  const minutes = Math.floor(timeLeft / 60);
  const seconds = timeLeft % 60;
  const timerElement = document.querySelector(".timer");
  const timerText = document.querySelector(".timer-text");
  const timerProgress = document.querySelector(".timer-progress");

  // تحديث النص
  timerText.textContent = `الوقت المتبقي: ${minutes}:${seconds
    .toString()
    .padStart(2, "0")}`;

  // تحديث شريط التقدم
  const progress = (timeLeft / quiz.timeLimit) * 100;
  timerProgress.style.width = `${progress}%`;

  // تحديث الألوان بناءً على الوقت المتبقي
  if (timeLeft <= 30) {
    timerElement.classList.add("danger");
    timerElement.classList.remove("warning");
  } else if (timeLeft <= 60) {
    timerElement.classList.add("warning");
    timerElement.classList.remove("danger");
  } else {
    timerElement.classList.remove("warning", "danger");
  }

  if (timeLeft <= 0) {
    clearInterval(timer);
    endQuiz();
  } else {
    timeLeft--;
  }
}

// Move to next question
function nextQuestion() {
  currentQuestion++;

  if (currentQuestion < questions.length) {
    clearInterval(timer);
    showQuestion();
    startTimer(60);
  } else {
    // Quiz completed
    showResults();
  }
}

// Show quiz results
function showResults() {
  const quizContainer = document.querySelector(".quiz-container");
  quizContainer.innerHTML = `
        <h2>انتهت المسابقة!</h2>
        <p>النتيجة النهائية: ${score} من ${questions.length}</p>
        <button class="btn" onclick="window.location.href='../index.html'">العودة للرئيسية</button>
    `;
}

// التحقق من اكتمال جميع التطابقات
function checkMatchingCompletion() {
  const matchedItems = document.querySelectorAll(
    ".matching-item.matched.correct"
  );
  const totalItems = questions[currentQuestion].items.length;

  if (matchedItems.length === totalItems) {
    // جميع التطابقات صحيحة
    setTimeout(() => {
      showFeedback(true);
      setTimeout(() => {
        nextQuestion();
      }, 1500);
    }, 500);
  }
}

// إضافة مستمع لتحميل الصفحة
document.addEventListener("DOMContentLoaded", initQuiz);

// Handle next question button
document.getElementById("nextQuestion").addEventListener("click", nextQuestion);

// تحسين دالة إنشاء سؤال التوصيل
function createMatchingQuestion(question) {
  const container = document.createElement("div");
  container.className = "matching-container";

  // إنشاء العمود الأيمن (الكلمات) - ثابت
  const rightColumn = document.createElement("div");
  rightColumn.className = "matching-column fixed-column";

  // إنشاء العمود الأيسر (التعريفات) - متحرك
  const leftColumn = document.createElement("div");
  leftColumn.className = "matching-column movable-column";

  // خلط التعريفات فقط
  const words = [...question.words];
  const definitions = [...question.definitions];
  shuffleArray(definitions);

  // إنشاء عناصر الكلمات (ثابتة)
  words.forEach((word, index) => {
    const wordElement = document.createElement("div");
    wordElement.className = "matching-item word-item";
    wordElement.dataset.word = word;
    wordElement.textContent = word;
    rightColumn.appendChild(wordElement);
  });

  // إنشاء عناصر التعريفات (قابلة للسحب)
  definitions.forEach((definition, index) => {
    const definitionElement = document.createElement("div");
    definitionElement.className = "matching-item definition-item";
    definitionElement.draggable = true;
    definitionElement.dataset.definition = definition;
    definitionElement.textContent = definition;

    // إضافة تأثيرات السحب
    definitionElement.addEventListener("dragstart", (e) => {
      e.dataTransfer.setData("text/plain", definition);
      definitionElement.classList.add("dragging");
      definitionElement.style.boxShadow = "0 4px 8px rgba(0,0,0,0.2)";
      definitionElement.style.transform = "scale(1.05)";
    });

    definitionElement.addEventListener("dragend", () => {
      definitionElement.classList.remove("dragging");
      definitionElement.style.boxShadow = "";
      definitionElement.style.transform = "";
    });

    leftColumn.appendChild(definitionElement);
  });

  // إضافة مستمع السحب والإفلات للعمود الأيمن
  rightColumn.addEventListener("dragover", (e) => {
    e.preventDefault();
    const wordItem = e.target.closest(".word-item");
    if (wordItem && !wordItem.classList.contains("matched")) {
      wordItem.classList.add("drag-over");
    }
  });

  rightColumn.addEventListener("dragleave", (e) => {
    const wordItem = e.target.closest(".word-item");
    if (wordItem) {
      wordItem.classList.remove("drag-over");
    }
  });

  rightColumn.addEventListener("drop", (e) => {
    e.preventDefault();
    const wordItem = e.target.closest(".word-item");
    if (!wordItem || wordItem.classList.contains("matched")) return;

    wordItem.classList.remove("drag-over");
    const definition = e.dataTransfer.getData("text/plain");
    const word = wordItem.dataset.word;
    const correctDefinition =
      question.definitions[question.words.indexOf(word)];

    if (definition === correctDefinition) {
      // تطابق صحيح
      const definitionElement = leftColumn.querySelector(
        `[data-definition="${definition}"]`
      );

      // نقل التعريف إلى موضع الكلمة المقابلة
      const wordRect = wordItem.getBoundingClientRect();
      const leftColumnRect = leftColumn.getBoundingClientRect();
      const newTop = wordRect.top - leftColumnRect.top;

      definitionElement.style.position = "absolute";
      definitionElement.style.top = `${newTop}px`;
      definitionElement.style.left = "0";
      definitionElement.style.right = "0";

      // إضافة تأثيرات النجاح
      wordItem.classList.add("matched", "correct");
      definitionElement.classList.add("matched", "correct");
      wordItem.style.backgroundColor = "#4CAF50";
      wordItem.style.color = "white";
      definitionElement.style.backgroundColor = "#4CAF50";
      definitionElement.style.color = "white";

      // تعطيل السحب
      definitionElement.draggable = false;
      definitionElement.classList.add("disabled");
    } else {
      // تطابق خاطئ
      const definitionElement = leftColumn.querySelector(
        `[data-definition="${definition}"]`
      );

      // إضافة تأثيرات الخطأ
      wordItem.classList.add("matched", "incorrect");
      definitionElement.classList.add("matched", "incorrect");
      wordItem.style.backgroundColor = "#f44336";
      wordItem.style.color = "white";
      definitionElement.style.backgroundColor = "#f44336";
      definitionElement.style.color = "white";

      // إعادة العناصر لحالتها الأصلية بعد فترة
      setTimeout(() => {
        wordItem.classList.remove("matched", "incorrect");
        definitionElement.classList.remove("matched", "incorrect");
        wordItem.style.backgroundColor = "";
        wordItem.style.color = "";
        definitionElement.style.backgroundColor = "";
        definitionElement.style.color = "";
      }, 1000);
    }

    // التحقق من اكتمال جميع التطابقات
    checkMatchingCompletion();
  });

  container.appendChild(leftColumn);
  container.appendChild(rightColumn);

  return container;
}
