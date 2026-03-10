#include <cstdlib>
#include <ctime>
#include <iostream>
using namespace std;


struct Cat {
    // Поле для хранения состояния кота (жив или мертв)
    bool alive;
    
    // Метод, который проверяет состояние кота
    bool is_alive() {
        return alive;
    }
    
    // Конструктор - вызывается при создании кота
    Cat(bool is_alive) {
        alive = is_alive;
    }
};

// Структура для коробки
struct Box {
    // Конструктор - вызывается при создании коробки
    Box() {
    }
    
    // Метод для открытия коробки
    Cat open() {
        // Генерируем случайное число: 0 или 1
        // rand() % 2 дает остаток от деления на 2 (0 или 1)
        int random_number = std::rand() % 2;
        
        // Если random_number равен 0 - кот жив, иначе - мертв
        bool cat_alive = (random_number == 0);
        
        // Создаем и возвращаем кота с определенным состоянием
        Cat new_cat(cat_alive);
        return new_cat;
    }
};

// Пример использования
int main() {
    // Создаем коробку
    Box box;
    
    // Открываем коробку и достаем кота
    Cat cat = box.open();
    
    // Проверяем состояние кота
    if (cat.is_alive()) {
        std::cout << "alive";
    } else {
        std::cout << "dead";
    }
    
    return 0;
}
