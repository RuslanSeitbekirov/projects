#include <iostream>
#include <algorithm>
#include <iterator>
#include <string>
#include <vector>
#include <sstream>

struct Course {
    std::string name;
    int semester;
    bool finished;
};

struct Student {
    std::string name;
    std::string group;
    std::vector<Course> courses;
};

Student make_student(std::string& line){
    Student student;
    std::vector<std::string> tokens;
    std::string token;
    std::stringstream ss(line);
    
    // Разбиваем строку по символу ';' и помещаем в массив
    while (std::getline(ss, token, ';')) {
        tokens.push_back(token);
    }


    if (tokens.size() >= 2){
        student.name = tokens[0];
        student.group = tokens[1];
    }
    // i = 2 так как 0 и 1 уже исп. 
    for (size_t i = 2; i + 2 < tokens.size(); i += 3){
        Course course;
        course.name = tokens[i]; // название курса
        course.semester = std::stoi(tokens[i + 1]); // семестр
        course.finished = (tokens[i+2] == "1"); // закончен ли курс
        student.courses.push_back(course); // добавляет всё в массив courses, у каждого студента свой массив курсов
    }

    return student;
}

bool is_debtor(Student student, int cur_semestr, int max_debt){
    int dept_count = 0;

    for (auto course : student.courses){
        // Если курс незакончен и курс сместра меньше текущего семестра
        if (!course.finished && course.semester < cur_semestr){
            dept_count++; // посчёт долгов
        }
    }

    return dept_count > max_debt; // сравнение кол-ва имеющихся долгов с их макс количеством
}


bool is_upper(Student a, Student b){
    if (a.group != b.group){
        return a.group < b.group;
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

int main(){
    int cur_semester, max_debt, stud_count;
    std::cin >> stud_count >> cur_semester >> max_debt;
    std::cin.ignore(1);  // Убираем из потока символ \n для корректной работы getline

    std::vector<Student> students(stud_count);
    for(auto& student: students) {
        std::string line; 
        std::getline(std::cin, line);
        student = make_student(line);
    }
    
    std::vector<Student> expulsion_candidates;
    std::copy_if(students.begin(),
                 students.end(),
                 std::back_inserter(expulsion_candidates),
                 [cur_semester, max_debt](const Student& student){
                     return is_debtor(student, cur_semester, max_debt);
                 });

    std::sort(expulsion_candidates.begin(), expulsion_candidates.end(), is_upper);
    print(expulsion_candidates);
}