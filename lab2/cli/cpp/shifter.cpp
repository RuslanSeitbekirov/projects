#include <iostream>
#include <string>
#include <cstdlib>  // Для getenv

int main() {
    std::string str;
    if (std::getenv("FULLMOON") != nullptr) str = std::getenv("FULLMOON"); // Если переменная существует

    if (str == "True") std::cout << "I am a werewolf" << std::endl;
    else std::cout << "I am human" << std::endl;
}