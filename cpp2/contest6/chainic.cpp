#include <iostream>
#include <deque>

class Water {
private:
    int temperature; // текущая температура воды

public:
    // Конструктор - вызывается при создании объекта Water
    // temp - начальная температура воды
    Water (int temp) {
        temperature = temp; //устанавливаем начальную температуру
    }
    // Метод для нагрева воды
    // degress - насеолько градусов надо нагреть воду 
    void heat_up(int degrees) {
        temperature += degrees;
    }

    bool is_boiling() const{
        return temperature >= 100;
    }

};

class Teapot{
private:
    Water& water;
public:
    Teapot (Water& w) : water(w) {}

    // Метод для нагрева воды в чайнике
    // degrees - на сколько градусов нагреть
    void heat_up(int degrees) {
        water.heat_up(degrees); // Вызываем метод нагрева у воды
    }

    // Метод для проверки, закипел ли чайник
    bool is_boiling() const {
        return water.is_boiling(); // спрашиваем у воды кипит ли она
    }
};

int main()
{
    int temperature, count;
    std::cin >> temperature >> count;
    std::deque<int> heat(count);
    for(auto& t : heat) std::cin >> t;
    
    // Создаём объект воды с начальной температурой
    Water water(temperature);
    // Создаём чайник и памещаем в него нашу воду
    Teapot teapot(water);
    
    while(!teapot.is_boiling()){
        teapot.heat_up(heat.front());
        heat.pop_front();
    }
    
    for(auto t : heat) std::cout << t << ' ';
}
