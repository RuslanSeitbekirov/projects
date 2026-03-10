#include <iostream>
#include <sstream>
#include <vector>
#include <string>
#include <fstream>
#include <algorithm>


    
    struct PassInfo {
    int PassengerId;
    int Survived;
    int Pclass;
    std::string Name;
    std::string Sex;
    double Age; 
    int SibSp;
    int Parch;
    std::string Ticket;
    double Fare;
    std::string Cabin;
    std::string Embarked;
};


PassInfo make_struct(std::string& line) {
    PassInfo passenger;
    std::vector<std::string> tokens;
    std::string token;
    std::stringstream ss(line);

    bool in_quotes = false;
    char c;

    while (ss.get(c)) { // Функция get() в языке программирования C++ предназначена для чтения символов из потока ввода. Она является членом класса istream и входит в стандартную библиотеку потоков ввода/вывода. 
        if (c == '"') { // если находит кавычки
            in_quotes = !in_quotes;  // Переключаем состояние " в кавычках "
        } else if (c == ',' && !in_quotes) {
            // Запятая вне кавычек - разделитель
            /*
            Если символ с != ' " ' и с != ',' то символ добавляется в строку token
            Если символ с == ',' и !in_quotes == true, то token записывается в tokens и строка token чистится
            Если символ с != ' " ' и in_quotes == true (читаем Name) то накапливаем символы в строке
            */
            tokens.push_back(token);
            token.clear();
        } else {
            token += c;
        }
    }
    tokens.push_back(token);
}

int main(){

    std::string line = "147,1,3,"Andersson, Mr. August Edvard (""Wennerstrom"")",male,27,0,0,350043,7.7958,,S"";

    while (std::getline(line_start, line, '\r')){
        PassInfo passenger = make_struct(line);

        if (passenger.Pclass == pclass && passenger.Age > age && passenger.Sex == "female") {
            arr.push_back(passenger);
        }
    }
            for (const auto& passenger : arr) {
            std::cout << passenger.Name << std::endl;
        }
}