// contest5_zadanie6.cpp : Этот файл содержит функцию "main". Здесь начинается и заканчивается выполнение программы.
//

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
    double Age;  // Изменено на double для вещественного возраста
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
        }
        else if (c == ',' && !in_quotes) {
            // Запятая вне кавычек - разделитель
            /*
            Если символ с != ' " ' и с != ',' то символ добавляется в строку token
            Если символ с == ',' и !in_quotes == true, то token записывается в tokens и строка token чистится
            Если символ с != ' " ' и in_quotes == true (читаем Name) то накапливаем символы в строке
            */
            tokens.push_back(token);
            token.clear();
        }
        else {
            token += c;
        }
    }
    tokens.push_back(token);

    // Заполняем поля структуры. Пустыми могут быть только Age и Cabin

    if (tokens.size() >= 12) {
        passenger.PassengerId = tokens[0].empty() ? 0 : std::stoi(tokens[0]);
        passenger.Survived = tokens[1].empty() ? 0 : std::stoi(tokens[1]);
        passenger.Pclass = tokens[2].empty() ? 0 : std::stoi(tokens[2]);
        passenger.Name = tokens[3].empty() ? "" : (tokens[3]);
        passenger.Sex = tokens[4].empty() ? "" : (tokens[4]);
        passenger.Age = tokens[5].empty() ? 0.0 : std::stod(tokens[5]); // обработка пустого возраста
        /* Аналог tokens[5].empty() ? -1 : std::stoi(tokens[5]); // обработка пустого возраста
        if (tokens[5].empty()) {
            passenger.Age = -1; если строка пустая
        } else {
            passenger.Age = std::stoi(tokens[5]); если строка не пустая
        }
            надо потому что число, а не строка
        */
        passenger.SibSp = tokens[6].empty() ? 0 : std::stoi(tokens[6]);
        passenger.Parch = tokens[7].empty() ? 0 : std::stoi(tokens[7]);
        passenger.Ticket = tokens[8].empty() ? "" : (tokens[8]);
        passenger.Fare = tokens[9].empty() ? 0.0 : std::stod(tokens[9]);
        passenger.Cabin = tokens[10].empty() ? "" : (tokens[10]);
        passenger.Embarked = tokens[11].empty() ? "" : (tokens[11]);
    }
    return passenger;
}

bool comparePassengers(const PassInfo& a, const PassInfo& b) {
    if (a.Age == b.Age) {
        return a.Name < b.Name; // лексикографический порядок по имени
    }
    return a.Age < b.Age; // по возрастанию возраста
}
 
int main() {
    int pclass;
    double age;
    std::cin >> pclass >> age;
    std::cin.ignore(1);  // Убираем из потока символ \n для корректной работы getline

    std::vector<PassInfo> arr;

    std::ifstream file("C:\\projects\\cpp\\contest5_zadanie6\\train.csv");
    std::string line;

    // Пропускаем заголовок CSV
    std::getline(file, line, '\r');

    // while (!file.eof()){
    while (std::getline(file, line, '\r')) {
        PassInfo passenger = make_struct(line);

        if (passenger.Pclass == pclass && passenger.Age > age && passenger.Sex == "female") {
            arr.push_back(passenger);
        }
    }

    std::sort(arr.begin(), arr.end(), comparePassengers);

    if (arr.empty()) {
        // Если нет подходящих пассажиров, ничего не выводим
    }
    else {
        for (const auto& passenger : arr) {
            std::cout << passenger.Name << std::endl;
        }
    }

    return 0;

}


// Советы по началу работы 
//   1. В окне обозревателя решений можно добавлять файлы и управлять ими.
//   2. В окне Team Explorer можно подключиться к системе управления версиями.
//   3. В окне "Выходные данные" можно просматривать выходные данные сборки и другие сообщения.
//   4. В окне "Список ошибок" можно просматривать ошибки.
//   5. Последовательно выберите пункты меню "Проект" > "Добавить новый элемент", чтобы создать файлы кода, или "Проект" > "Добавить существующий элемент", чтобы добавить в проект существующие файлы кода.
//   6. Чтобы снова открыть этот проект позже, выберите пункты меню "Файл" > "Открыть" > "Проект" и выберите SLN-файл.
