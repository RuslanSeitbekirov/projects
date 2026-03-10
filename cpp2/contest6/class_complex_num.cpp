#include <iostream>
#include <string>
#include <sstream>

struct Complex{
private:
    double real;
    double imag;
public:

    // Конструктор 1. Определяет real и imag
    Complex(const std::string& str){

        std::string new_str = "";
        for (int i = 0; i < str.size(); i++){
            if (str[i] != ' ' && str[i] != 'j'){
                new_str = new_str + str[i];
            }
        }

        size_t separator_pos = std::string::npos; // size_t — это беззнаковый целочисленный тип
        // Ищем разделитель (плюс или минус не в начале строки)
        for (size_t i = 1; i < new_str.length(); i++) {
            if (new_str[i] == '+' || new_str[i] == '-') {
                separator_pos = i;
                break;
            }
        }

        
        if (separator_pos != std::string::npos) {
            // Разделяем строку на две части
            std::string real_str = new_str.substr(0, separator_pos);
            std::string imag_str = new_str.substr(separator_pos);
            
            real = std::stod(real_str);
            imag = std::stod(imag_str);
        } else {
            // Если разделителя нет, значит только действительная часть
            real = std::stod(new_str);
        }
    }

    // Конструктор 2. Определяет real и imag как r и i. Сюда возвращаются значения перегрузки операторов
    Complex(double r = 0.0, double i = 0.0){
        real = r;
        imag = i;
    }

    // Перешрузка операторов. с1 вызывает перегрузку, а other  подтягивает данные с с2.
    Complex operator+(const Complex& other){
        return Complex(real + other.real, imag + other.imag);
    }

    Complex operator-(const Complex& other) const {
        return Complex(real - other.real, imag - other.imag);
    }

    Complex operator*(const Complex& other){
        double r = real * other.real - imag * other.imag;
        double i = real * other.imag + imag * other.real;
        return Complex(r, i);
    }

    Complex operator/(const Complex& other){
        double denominator = other.real * other.real + other.imag * other.imag;
        double r = (real * other.real + imag * other.imag) / denominator;
        double i = (imag * other.real - real * other.imag) / denominator;
        return Complex(r, i);
    }

    friend std::ostream& operator<<(std::ostream& out, const Complex& c){
            out << c.real;
        if (c.imag < 0){
            out << c.imag << 'j';
        } else if(c.imag == 0){
            out<< '+' << 0 << 'j';
        } else{
            out<< '+' << c.imag << 'j';
        }
        return out;
    }

};


int main()
{
    std::string num;
    std::getline(std::cin, num, 'j');
    Complex c1(num);
    
    std::getline(std::cin, num, 'j');
    Complex c2(num);
    
    std::cout << c1+c2 << '\n' << c1-c2 << '\n'
              << c1*c2 << '\n' << c1/c2 << std::endl;
}