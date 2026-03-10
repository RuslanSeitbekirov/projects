#include <iostream>
#include <vector>
#include <string>
#include <algorithm>

struct Student {
    std::string name;
    std::string group;
};

Student make_student(std::string line){
    Student student;
    size_t pos = line.find(';'); // позиция разделителя

    if (pos != std::string::npos){ // std::string::npos - это специальная константа в C++, которая означает "не найдено" или "неверная позиция".
        student.name = line.substr(0, pos); // Имя - часть до ';'
        student.group = line.substr(pos+1);  // Группа - часть после ';'
    } else {
        // если разделитель не найден
        student.name = line;
        student.group = "";
    }

    return student;
}

bool is_upper(Student& a, Student& b){
    if (a.group != b.group){
        return a.group < b.group; // сортировка по алфавиту
    }
    return a.name < b.name;
}

void print(std::vector<Student> students){
    if (students.empty()){
        std::cout << "Empty list!" << std::endl;
    }

    std::string current_group = "";

    for (auto student : students){
// строка current_group изначально пустая поэтому условие выполняется и становится group1
        if (student.group != current_group){
            current_group = student.group;
            std::cout << current_group << std::endl;
        }


        std::cout << "- " << student.name << std::endl;
    }
}

int main()
{
    int count;
    std::cin >> count;
    std::cin.ignore(1);  // Убираем из потока символ \n для корректной работы getline
    std::string a;
    std::vector<Student> students(count);
    for(auto& student: students) {
        std::string line;
        std::getline(std::cin, line);
        student = make_student(line);
    }
    
    std::sort(students.begin(), students.end(), is_upper);
    std::cout << std::endl << a;
    print(students);
}