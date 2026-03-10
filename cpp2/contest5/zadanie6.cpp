#include <iostream>
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
    
    bool in_quotes = false;
    
    for (int i = 0; i < line.length(); i++) {
        char c = line[i];
        
        if (c == '"') {
            // Проверяем, это выходная кавычка или открывающая/закрывающая
            if (in_quotes && i + 1 < line.length() && line[i + 1] == '"') { // вне кавычек && в строке && следующий символ == " " "
                // Это выходная кавычка "" - добавляем одну кавычку в токен
                token += '"';
                i++; // Пропускаем следующую кавычку
            } else {
                // Это открывающая или закрывающая кавычка
                in_quotes = !in_quotes;
            }
        } else if (c == ',' && !in_quotes) {
            // Запятая вне кавычек - разделитель
            tokens.push_back(token);
            token.clear();
        } else {
            token += c;
        }
    }
    tokens.push_back(token);  // Последний токен

    // Заполняем поля структуры
    if (tokens.size() >= 12) {
        passenger.PassengerId = std::stoi(tokens[0]);
        passenger.Survived = std::stoi(tokens[1]);
        passenger.Pclass = tokens[2].empty() ? 0 : std::stoi(tokens[2]);
        passenger.Name = tokens[3];
        passenger.Sex = tokens[4];
        passenger.Age = tokens[5].empty() ? 0.0 : std::stod(tokens[5]); // обработка пустого возраста
        /* Аналог tokens[5].empty() ? -1 : std::stoi(tokens[5]); // обработка пустого возраста
        if (tokens[5].empty()) {
            passenger.Age = -1; если строка пустая
        } else {
            passenger.Age = std::stoi(tokens[5]); если строка не пустая
        }
            надо потому что число, а не строка
        */
        passenger.SibSp = std::stoi(tokens[6]);
        passenger.Parch = std::stoi(tokens[7]);
        passenger.Ticket = tokens[8];
        passenger.Fare = std::stod(tokens[9]);
        passenger.Cabin = tokens[10];
        passenger.Embarked = tokens[11];
    }
    return passenger;
}

bool comparePassengers(const PassInfo& a, const PassInfo& b) {
    if (a.Age == b.Age) {
        return a.Name < b.Name; // лексикографический порядок по имени
    }
    return a.Age < b.Age; // по возрастанию возраста
}

int main(){
    int pclass;
    double age;
    std::cin >> pclass >> age;


    std::vector<PassInfo> arr;

    std::ifstream file("C:\\projects\\cpp2\\contest5\\train.csv");
    std::string line;

    // Пропускаем заголовок CSV
    std::getline(file, line, '\r');

    // while (!file.eof()){
    while (std::getline(file, line, '\r')){
        PassInfo passenger = make_struct(line);

        if (passenger.Pclass == pclass && passenger.Age > age && passenger.Sex == "female") {
            arr.push_back(passenger);
        }
    }

    std::sort(arr.begin(), arr.end(), comparePassengers);

    if (arr.empty()) {
        // Если нет подходящих пассажиров, ничего не выводим
    } else {
        for (const auto& passenger : arr) {
            std::cout << passenger.Name << std::endl;
        }
    }

    return 0;
    
}