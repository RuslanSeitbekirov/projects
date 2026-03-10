// #include <iostream>
// #include <cstdlib>  // Для environ

// extern char** environ;

// int main() {
//     for (int i=0; environ[i] != nullptr; i++) {
//         std::cout << environ[i] << std::endl;
//     }
// }

#undef UNICODE       // Чтобы использовать обычные char вместо широких wchar
#include <windows.h> // Внути подключается processenv.h
#include <iostream>

int main() {
    auto env = GetEnvironmentStrings();
    if (env == nullptr) {
        std::cerr << "GetEnvironmentStrings failed" << std::endl;
        return 1;
    }
    
    auto current = env; // Компируем, чтобы не менять env, он понадобится в конце
    while (*current) {
        std::cout << current << std::endl;
        current += strlen(current) + 1;
    }
    
    FreeEnvironmentStrings(env);
}