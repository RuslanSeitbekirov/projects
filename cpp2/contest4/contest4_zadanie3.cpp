#include <iostream>
#include <string>
#include <typeinfo>
using namespace std;


template <typename T> T formatter(T n) {
    return n;
}

string formatter(bool n){
    if (n){
        return "true";
    } else {
        return "false";
    }
}

int main()
{
    int n;
    std::cin >> n;

    std::cout << formatter(n) << ' '
              << formatter(1.5*n) <<  ' '
              << formatter(static_cast<bool>(n)) <<  ' '
              << formatter("\"" + std::to_string(n) + "\"");
}