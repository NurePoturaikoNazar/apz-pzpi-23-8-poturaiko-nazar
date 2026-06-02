#include <sys/types.h>
#include <sys/wait.h>
#include <unistd.h>
#include <stdio.h>
#include <stdlib.h>
#include <err.h>

// Функція непривілейованого робочого процесу
void worker_process() {
    printf("Дочірній процес: ініціалізація ізоляції...\n");

    // Надаємо доступ лише до директорії /tmp для читання
    if (unveil("/tmp", "r") == -1) {
        err(1, "unveil error in worker");
    }

    // Фіксуємо правила unveil
    if (unveil(NULL, NULL) == -1) {
        err(1, "unveil lock error in worker");
    }

    // Обмежуємо системні виклики лише базовим вводом/виводом
    if (pledge("stdio rpath", NULL) == -1) {
        err(1, "pledge error in worker");
    }

    printf("Дочірній процес: успішно ізольовано.\n");
    
    // Основна логіка непривілейованого процесу
    // Спроба відкрити мережевий сокет тут призведе до SIGABRT
    
    exit(0);
}

// Функція привілейованого керуючого процесу
void master_process(pid_t worker_pid) {
    int status;
    printf("Батьківський процес: очікування завершення дочірнього...\n");
    
    // Батьківський процес зберігає права для виконання інших задач
    waitpid(worker_pid, &status, 0);
    
    printf("Батьківський процес: дочірній процес завершився.\n");
}

int main() {
    pid_t pid;

    printf("Запуск системної служби OpenBSD...\n");

    // Розділення процесів
    pid = fork();

    if (pid == -1) {
        err(1, "fork failed");
    } else if (pid == 0) {
        // Код виконується у дочірньому процесі
        worker_process();
    } else {
        // Код виконується у батьківському процесі
        master_process(pid);
    }

    return 0;
}