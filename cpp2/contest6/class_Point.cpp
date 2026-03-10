#include <iostream>
#include "json.hpp"
using json = nlohmann::json;

struct Point{
private:
    double x_cord, y_cord;

public:
    Point(double x, double y){
        x_cord = x;
        y_cord = y; 
    }
    // Метод для получения коорлинаты х
    double get_x() const {
        return x_cord;
    }
    // Метод для получения коорлинаты у
    double get_y() const {
        return y_cord;
    }

    double get_r() const {
        return std::sqrt(x_cord * x_cord + y_cord * y_cord);
    }

    double get_a() const {
        // угол в радианах. Сначала передаётся У потом Х
        return std::atan2(y_cord, x_cord);
    }

    void set_x(double new_x) {
        x_cord = new_x;
        // y_cord остаётся без изменений
    }

    void set_y(double new_y) {
        y_cord = new_y;
        // х_cord остаётся без изменений
    }

    // Метод для установки радиуса r (полярная координата)
    void set_r(double new_r){
        double curr_a = get_a(); // сохраняем текущий угол
        // Пересчитываем х и у по старому радиусу и новому углу
        x_cord = new_r * std::cos(curr_a);
        y_cord = new_r * std::sin(curr_a);
    }

    // Метод для установки угла альфа (полярная координата)
    void set_a(double new_a){
        double curr_r = get_r(); // сохраняем текущий радиус
        // Пересчитываем х и у по старому радиусу и новому углу
        x_cord = curr_r * std::cos(new_a);
        y_cord = curr_r * std::sin(new_a);
    }
};

int main(){
    double x, y;
    std::cin >> x >> y;

    Point p(x, y);
    json moves;
    std::cin >> moves;
    
    for(auto& move : moves){
        std::string ort = move["ort"];
        double value = move["value"];
        if (ort == "x") p.set_x(p.get_x() + value); //p.get_x() + value == new_x
        else if (ort == "y") p.set_y(p.get_y() + value); // p.get_y() + value == new_y
        else if (ort == "r") p.set_r(p.get_r() + value); // p.get_r() + value == new_r
        else p.set_a(p.get_a() + value); // p.get_a() + value == new_a
        std::cout << round(p.get_x()*(1E+5))/(1E+5) << ' ' << round(p.get_y()*(1E+5))/(1E+5) << std::endl;    
    }
}