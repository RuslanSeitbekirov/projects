#include <iostream>
#include <string>
#include <vector>

struct Complex{
  double re;  // Действительная часть
  double im;  // Мнимая часть
};


template<typename T>
void print(T value, std::string end=""){
    std::cout << value << end;
}

void print(const Complex& c, std::string end) {
    std::cout << c.re;
    if (c.im >= 0) {
        std::cout << "+";
    }
    std::cout << c.im << "j" << end;
}

template<typename T>
void print(const std::vector<T>& vec, std::string end) {
    std::cout << "[";
    for (size_t i = 0; i < vec.size(); ++i) {
        print(vec[i], "");
        if (i != vec.size() - 1) {
            std::cout << ", ";
        }
    }
    std::cout << "]" << end;
}

// Вспомогательная функция для вывода без завершающего символа
template<typename T>
void print_no_end(T value) {
    print(value, "");
}

template<typename T1, typename T2>
void print(T1 value1, T2 value2, std::string sep=" ", std::string end="\n"){
    print(value1, sep);
    print(value2, end);
}

template<typename T1, typename T2, typename T3>
void print(T1 value1, T2 value2, T3 value3, std::string sep=" ", std::string end="\n"){
    print(value1, sep);
    print(value2, sep);
    print(value3, end);
}

template<typename T1, typename T2, typename T3, typename T4>
void print(T1 value1, T2 value2, T3 value3, T4 value4, std::string sep=" ", std::string end="\n"){
    print(value1, sep);
    print(value2, sep);
    print(value3, sep);
    print(value4, end);
}

template<typename T1, typename T2, typename T3, typename T4, typename T5>
void print(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, std::string sep=" ", std::string end="\n"){
    print(value1, sep);
    print(value2, sep);
    print(value3, sep);
    print(value4, sep);
    print(value5, end);
}

template<typename T1, typename T2, typename T3, typename T4, typename T5, typename T6>
void print(T1 value1, T2 value2, T3 value3, T4 value4, T5 value5, T6 value6, std::string sep=" ", std::string end="\n"){
    print(value1, sep);
    print(value2, sep);
    print(value3, sep);
    print(value4, sep);
    print(value5, sep);
    print(value6, end);
}


int main()
{
    double v1, v2, v3, v4, v5, v6;
    std::cin >> v1 >> v2 >> v3 >> v4 >> v5 >> v6;

    Complex cpx = {v1, v2};
    std::vector<double> darr = {v1, v2, v3, v4};
    std::vector<Complex> carr = {{v1, v2}, {v3, v4}};
    std::vector<std::vector<double>> iarr = {{v1, v2, v3}, {v4, v5, v6}};
    
    print(v1, v2, cpx, darr, carr, iarr, std::string(" + "));
}
