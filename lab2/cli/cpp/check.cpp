#include <iostream>
#include <string>
#include <cstdlib>  // Для getenv, putenv, unsetenv

int main(int argc, char* argv[]) {
    if (argc != 2) return 1;

    // Проверяем значение переменной
    if (std::getenv(argv[1]) != nullptr) std::cout << argv[1] << " = " << std::getenv(argv[1]) << std::endl;
    else std::cout << argv[1] << " not exist" << std::endl;

    // Изменяем значение переменной
    std::string str = argv[1];
    str += "=VALUE"; // Формат ИМЯ=ЗНАЧЕНИЕ
    putenv(str.data());
    // Проверяем
    if (std::getenv(argv[1]) != nullptr) std::cout << argv[1] << " = " << std::getenv(argv[1]) << std::endl;
    else std::cout << argv[1] << " not exist" << std::endl;

    // Удаляем переменную
    unsetenv(argv[1]);
    // Проверяем
    if (std::getenv(argv[1]) != nullptr) std::cout << argv[1] << " = " << std::getenv(argv[1]) << std::endl;
    else std::cout << argv[1] << " not exist" << std::endl;
}