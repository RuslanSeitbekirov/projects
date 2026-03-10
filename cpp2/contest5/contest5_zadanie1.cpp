#include <iostream>
#include <string>
#include <cctype>


struct Complex{
  double re;  // Действительная часть
  double im;  // Мнимая часть
};

Complex make_complex(const std::string& str) {
    Complex result = {0, 0};
    std::string new_str = "";
    for (int i = 0; i < str.size(); i++){
      if (str[i] != ' '){
        new_str = new_str + str[i];
      }
    }
    size_t separator_pos = std::string::npos;
    
    // Ищем разделитель (плюс или минус не в начале строки)
    for (size_t i = 1; i < new_str.length(); i++) {
        if (new_str[i] == '+' || new_str[i] == '-') {
            separator_pos = i;
            // std::cout << separator_pos << std::endl;
            break;
        }
    }

    
    if (separator_pos != std::string::npos) {
        // Разделяем строку на две части
        std::string real_str = new_str.substr(0, separator_pos);
        std::string imag_str = new_str.substr(separator_pos+1);
        
        result.re = std::stod(real_str);
        result.im = std::stod(imag_str);
        // std::cout << "hgk" << result.re << "im>" << result.im << std::endl;
    } else {
        // Если разделителя нет, значит только действительная часть
        result.re = std::stod(new_str);
    }
    
    return result;
}

Complex sum(const Complex c1, const Complex c2){
  Complex result;
  result.re = c1.re + c2.re; 
  result.im = c1.im + c2.im; 
  return result;
}

Complex sub(const Complex c1, const Complex c2){
  Complex result;
  result.re = c1.re - c2.re; 
  result.im = c1.im - c2.im; 
  return result;
}


Complex mul(const Complex& a, const Complex& b) {
    Complex result;
    result.re = a.re * b.re - a.im * b.im;
    result.im = a.re * b.im + a.im * b.re;
    return result;
}

Complex div(const Complex& a, const Complex& b) {
    Complex result;
    double denominator = b.re * b.re + b.im * b.im;
    
    result.re = (a.re * b.re + a.im * b.im) / denominator;
    result.im = (a.im * b.re - a.re * b.im) / denominator;
    return result;
}

void print(Complex result) {
    std::cout << result.re;
    if (result.im < 0){
      std::cout <<  result.im << 'j' << std::endl;
    } else if(result.im == 0){
      std::cout<< '+' << 0 << 'j' << std::endl;
    } else{
      std::cout<< '+' << result.im << 'j' << std::endl;
    }
}
// erer


int main()
{
    std::string num;
    std::getline(std::cin, num, 'j');
    Complex c1 = make_complex(num);
    
    std::getline(std::cin, num, 'j');
    Complex c2 = make_complex(num);
     
    print(sum(c1, c2));
    print(sub(c1, c2));
    print(mul(c1, c2));
    print(div(c1, c2));

}