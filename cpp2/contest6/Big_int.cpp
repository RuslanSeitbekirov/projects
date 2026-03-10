#include <iostream>
#include <string>
#include <algorithm>
#include <vector>

class Bigint{
private:
    long long value;
    std::string number;
    bool is_negative = false;

    // Сравнение двух положительных чисел-строк
    static bool absolute_less(const std::string& a, const std::string& b) {
        if (a.length() != b.length()) {
            return a.length() < b.length();
        }
        return a < b;
    }

    // Вычитание двух положительных чисел-строк (a >= b)
    static std::string absolute_subtract(std::string a, const std::string& b) {
        int borrow = 0;
        int i = a.length() - 1;
        int j = b.length() - 1;
        std::string result;
        
        while (i >= 0 || j >= 0) {
            int digit_a = (i >= 0) ? a[i] - '0' : 0;
            int digit_b = (j >= 0) ? b[j] - '0' : 0;
            
            int diff = digit_a - digit_b - borrow;
            if (diff < 0) {
                diff += 10;
                borrow = 1;
            } else {
                borrow = 0;
            }
            
            result = char(diff + '0') + result;
            i--;
            j--;
        }
        
        // Удаляем ведущие нули
        size_t pos = result.find_first_not_of('0');
        return (pos == std::string::npos) ? "0" : result.substr(pos);
    }

public:
    Bigint(){
        value = 0;
        number = "0";
    }

    Bigint(const std::string& str){
        number = str;
        if (!str.empty() && str[0] == '-') {
            is_negative = true;
            number = str.substr(1); // Store without negative sign
        } else {
            number = str;
        }

        if(str.size() < 19){
            value = std::stoll(str);
        } else{
            value = 0;
        }
    }

    Bigint(long long num){
        value = num; 
        if(num < 0){
            is_negative = true;
            number = std::to_string(-num);
        } else {
            number = std::to_string(num);
        }
    }

    Bigint operator+(const Bigint& other) const {
        // Если оба числа маленькие - используем value
        if (number.size() < 19 && other.number.size() < 19) {
            return Bigint(value + other.value);
        }
        
        // Для больших чисел: a + b
        if (is_negative == other.is_negative) {
            // Одинаковые знаки - складываем
            std::string a = number;
            std::string b = other.number;
            
            std::string result;
            int carry = 0;
            int i = a.size() - 1;
            int j = b.size() - 1;
            
            while (i >= 0 || j >= 0 || carry) {
                int sum = carry;
                if (i >= 0) sum += a[i--] - '0';
                if (j >= 0) sum += b[j--] - '0';
                result = char(sum % 10 + '0') + result;
                carry = sum / 10;
            }
            
            Bigint final_result(result);
            final_result.is_negative = is_negative;
            return final_result;
        }
        else {
            // Разные знаки: a + b = a - (-b)
            if (is_negative) {
                // -a + b = b - a
                Bigint temp = *this;
                temp.is_negative = false;
                return other - temp;
            } else {
                // a + (-b) = a - b
                Bigint temp = other;
                temp.is_negative = false;
                return *this - temp;
            }
        }
    }

    Bigint operator-(const Bigint& other) const {
        // Если оба числа маленькие - используем value
        if (number.size() < 19 && other.number.size() < 19) {
            return Bigint(value - other.value);
        }
        
        std::string a = number;
        std::string b = other.number;
        
        // Определяем знак результата и операцию
        bool result_negative = false;
        std::string result;
        
        if (!is_negative && !other.is_negative) {
            // a - b
            if (absolute_less(a, b)) {
                result = absolute_subtract(b, a);
                result_negative = true;
            } else {
                result = absolute_subtract(a, b);
                result_negative = false;
            }
        }
        else if (is_negative && !other.is_negative) {
            // -a - b = -(a + b)
            result = absolute_add(a, b);
            result_negative = true;
        }
        else if (!is_negative && other.is_negative) {
            // a - (-b) = a + b
            result = absolute_add(a, b);
            result_negative = false;
        }
        else {
            // -a - (-b) = -a + b = b - a
            if (absolute_less(a, b)) {
                result = absolute_subtract(b, a);
                result_negative = false;
            } else {
                result = absolute_subtract(a, b);
                result_negative = true;
            }
        }
        
        // Добавляем знак если нужно
        if (result_negative && result != "0") {
            result = "-" + result;
        }
        
        return Bigint(result);
    }

    // Вспомогательная функция для сложения положительных чисел
    static std::string absolute_add(const std::string& a, const std::string& b) {
        std::string result;
        int carry = 0;
        int i = a.size() - 1;
        int j = b.size() - 1;
        
        while (i >= 0 || j >= 0 || carry) {
            int sum = carry;
            if (i >= 0) sum += a[i--] - '0';
            if (j >= 0) sum += b[j--] - '0';
            result = char(sum % 10 + '0') + result;
            carry = sum / 10;
        }
        
        return result;
    }

    Bigint operator*(const Bigint& other) const {
        if (number == "0" || other.number == "0") {
            return Bigint("0");
        }
        
        std::string a = number;
        std::string b = other.number;
        
        // Убираем минусы для умножения
        bool result_negative = (is_negative != other.is_negative);
        
        // Создаем результат достаточного размера
        std::vector<int> result(a.size() + b.size(), 0);
        
        // Умножаем в столбик
        for (int i = a.size() - 1; i >= 0; i--) {
            for (int j = b.size() - 1; j >= 0; j--) {
                int product = (a[i] - '0') * (b[j] - '0');
                int sum = product + result[i + j + 1];
                
                result[i + j + 1] = sum % 10;
                result[i + j] += sum / 10;
            }
        }
        
        // Преобразуем обратно в строку
        std::string result_str;
        for (int num : result) {
            if (!(result_str.empty() && num == 0)) {
                result_str += std::to_string(num);
            }
        }
        
        if (result_str.empty()) {
            result_str = "0";
        }
        
        if (result_negative && result_str != "0") {
            result_str = "-" + result_str;
        }
        
        return Bigint(result_str);
    }

    Bigint operator/(const Bigint& other) const {
        // Проверка деления на ноль
        if (other.number == "0" || (other.number.size() == 1 && other.number[0] == '0')) {
            throw std::runtime_error("Division by zero");
        }
        
        // Если числа маленькие - используем обычное деление
        if (number.size() < 19 && other.number.size() < 19) {
            return Bigint(value / other.value);
        }
        
        std::string a = number;
        std::string b = other.number;
        
        // Убираем знаки для вычислений
        bool result_negative = (is_negative != other.is_negative);
        
        // Если делимое меньше делителя - результат 0
        if (absolute_less(a, b)) {
            return Bigint("0");
        }
        
        // Алгоритм деления в столбик
        std::string result;
        std::string current;
        
        for (size_t i = 0; i < a.length(); i++) {
            current += a[i];
            
            // Удаляем ведущие нули
            size_t pos = current.find_first_not_of('0');
            if (pos == std::string::npos) {
                current = "0";
            } else {
                current = current.substr(pos);
            }
            
            // Считаем, сколько раз b помещается в current
            int count = 0;
            std::string temp = current;
            
            while (!absolute_less(temp, b)) {
                temp = absolute_subtract(temp, b);
                count++;
            }
            
            result += std::to_string(count);
            current = temp;
            
            // Если current стал 0, сбрасываем для следующей цифры
            if (current == "0") {
                current = "";
            }
        }
        
        // Удаляем ведущие нули из результата
        size_t pos = result.find_first_not_of('0');
        if (pos == std::string::npos) {
            result = "0";
        } else {
            result = result.substr(pos);
        }
        
        // Добавляем знак если нужно
        if (result_negative && result != "0") {
            result = "-" + result;
        }
        
        return Bigint(result);
    }

    // Сравнения возвращают bool!
    bool operator>(const Bigint& other) const {
        if (is_negative != other.is_negative) {
            return !is_negative; // положительное > отрицательного
        }
        
        if (is_negative) {
            // оба отрицательные: -a > -b если a < b
            return absolute_less(number, other.number);
        } else {
            // оба положительные: a > b если a больше b
            return !absolute_less(number, other.number) && number != other.number;
        }
    }

    bool operator<(const Bigint& other) const {
        return other > *this;
    }

    bool operator==(const Bigint& other) const {
        return number == other.number && is_negative == other.is_negative;
    }

    friend std::ostream& operator<<(std::ostream& out, const Bigint& c){
        if (c.is_negative && c.number != "0") {
            out << "-";
        }
        out << c.number;
        return out;
    }
};

int main() {
    std::string a, b;
    std::cin >> a >> b;

    Bigint first(a);
    Bigint second(b);

    std::cout << "a + b = " << (first + second) << std::endl;
    std::cout << "a - b = " << (first - second) << std::endl;
    std::cout << "a * b = " << (first * second) << std::endl;
    std::cout << "a / b = " << (first / second) << std::endl;

    int c;
    std::cin >> c;
    Bigint third(c);
    Bigint zero;

    std::cout << "c > 0 = " << (third > zero) << std::endl;
    std::cout << "c < 0 = " << (third < zero) << std::endl;
    std::cout << "c == 0 = " << (third == zero) << std::endl;
}