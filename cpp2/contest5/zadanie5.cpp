#include <iostream>
#include "C:\projects\cpp2\contest5\json.hpp"
#include <fstream>
#include <vector>
#include <string>
using json = nlohmann::json;

struct Task{
    int UserId;
    int ID;
    std::string Title;
    bool Compleated;
};

struct Project{
    int Project_ID;
    std::vector<Task> Tasks;
};

// Функции для преобразования JSON в структуры
void from_json(const json& j, Task& t) {
    j.at("user_id").get_to(t.UserId);
    j.at("id").get_to(t.ID);
    j.at("title").get_to(t.Title);
    j.at("completed").get_to(t.Compleated);
}

void from_json(const json& j, Project& p) {
    j.at("project_id").get_to(p.Project_ID);
    j.at("tasks").get_to(p.Tasks);
}

int main() {
    std::ifstream i("data.json");
    json j;
    i >> j;

    int ID;
    std::cin >> ID;
    
    // Преобразуем весь JSON в вектор проектов
    std::vector<Project> projects = j.get<std::vector<Project>>();

    int completedCount = 0;
    for (auto project : projects){
        for (auto task : project.Tasks){
            if (task.UserId == ID && task.Compleated) {
                completedCount++;
            }
        }
    }

    std::cout << completedCount;
}
