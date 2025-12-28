let dlls = [];
let selectedDlls = [];
let tests = [];

// DOM элементы
const dllListEl = document.querySelector('.dlls__list');
const historyListEl = document.querySelector('.history__list');
const addDllBtn = document.querySelector('.dlls__btn--add');
const runSelectedBtn = document.querySelector('.dlls__btn--run-selected');
const runAllBtn = document.querySelector('.dlls__btn--run-all');
const dllInput = document.querySelector('.dlls__input');
const clearHistoryBtn = document.querySelector('.history__btn--clear');

// Добавление DLL
addDllBtn.addEventListener('click', () => dllInput.click());
dllInput.addEventListener('change', e => {
  const files = Array.from(e.target.files); // получаем массив выбранных файлов
  files.forEach(file => {
    if (!dlls.includes(file.name)) {
      dlls.push(file.name); // добавляем только уникальные
    }
    if (!selectedDlls.includes(file.name)) selectedDlls.push(file.name);
  });

  renderDlls();
  e.target.value = null; // чтобы можно было выбрать те же файлы снова
});

// Очистка истории
clearHistoryBtn.addEventListener('click', () => {
  tests = [];
  renderHistory();
});

// Запуск выбранных DLL
runSelectedBtn.addEventListener('click', () => {
  runTests(selectedDlls);
});

// Запуск всех DLL
runAllBtn.addEventListener('click', () => {
  runTests(dlls);
});

// Отображение DLL
function renderDlls() {
  dllListEl.innerHTML = '';
  dlls.forEach(dll => {
    const li = document.createElement('li');
    li.textContent = dll;
    if (selectedDlls.includes(dll)) li.classList.add('dlls__item--selected');

    li.addEventListener('click', () => {
      if (selectedDlls.includes(dll)) {
        selectedDlls = selectedDlls.filter(d => d !== dll);
      } else {
        selectedDlls.push(dll);
      }
      renderDlls();
    });

    dllListEl.appendChild(li);
  });

  runSelectedBtn.disabled = selectedDlls.length === 0;
  runAllBtn.disabled = dlls.length === 0;
}

// Симуляция запуска тестов
function runTests(dllArray) {
  dllArray.forEach(dll => {
    const testResult = {
      name: dll,
      status: Math.random() > 0.3 ? 'Passed' : 'Failed',
      time: (Math.random() * 0.5 + 0.01).toFixed(2) + 's',
      error: null
    };
    if (testResult.status === 'Failed') {
      testResult.error = 'Simulated error for ' + dll;
    }
    tests.push(testResult);
  });

  renderHistory();
}

// Рендер истории
function renderHistory() {
  historyListEl.innerHTML = '';

  tests.forEach((test, index) => {
    const li = document.createElement('li');
    if (index === tests.length - 1) li.classList.add('history__item--latest');

    li.textContent = `${test.name} - ${test.status} (${test.time})`;

    if (test.status === 'Failed') {
      const errorDiv = document.createElement('div');
      errorDiv.textContent = test.error;
      errorDiv.classList.add('history__error');
      li.appendChild(errorDiv);
    }

    historyListEl.appendChild(li);
  });
}

function renderDlls() {
  dllListEl.innerHTML = '';
  dlls.forEach(dll => {
    const li = document.createElement('li');
    li.classList.add('mynunit__dll-item');
    li.textContent = dll;

    if (selectedDlls.includes(dll)) li.classList.add('selected');

    // Клик по элементу для выбора/снятия выделения
    li.addEventListener('click', () => {
      if (selectedDlls.includes(dll)) {
        selectedDlls = selectedDlls.filter(d => d !== dll);
      } else {
        selectedDlls.push(dll);
      }
      renderDlls();
    });

    // Кнопка удаления
    const removeBtn = document.createElement('button');
    removeBtn.textContent = '✕';
    removeBtn.classList.add('mynunit__dll-remove-btn');
    removeBtn.addEventListener('click', (e) => {
      e.stopPropagation();
      dlls = dlls.filter(d => d !== dll);
      selectedDlls = selectedDlls.filter(d => d !== dll);
      renderDlls();
    });

    li.appendChild(removeBtn);
    dllListEl.appendChild(li);
  });
}


// Инициализация
renderDlls();
renderHistory();
