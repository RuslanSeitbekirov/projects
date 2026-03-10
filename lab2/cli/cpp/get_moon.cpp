#undef UNICODE       // Чтобы использовать обычные char вместо широких wchar
#include <windows.h> // Внути подключается processenv.h
#include <iostream>

int main() {
    const int bufSize = 32767;
    char buffer[bufSize];
    // Читаем переменную FULLMOON
    auto count = GetEnvironmentVariable("FULLMOON", buffer, bufSize);
    if (count == 0) {
        // Если FULLMOON не установлено, устанавливаем
        if (!SetEnvironmentVariable("FULLMOON", "default value")){
            std::cerr << "SetEnvironmentVariable failed" << std::endl;
            return 1;
        }
    }
    
    // Повторно читаем переменную FULLMOON
    count = GetEnvironmentVariable("FULLMOON", buffer, bufSize);
    if (count == 0) {
        std::cerr << "GetEnvironmentVariable failed" << std::endl;
        return 1;
    }

    std::cout << "FULLMOON = " << buffer << std::endl;
}