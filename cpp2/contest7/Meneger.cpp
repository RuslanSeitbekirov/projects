#include<iostream>
#include<string>
#include<map>
#include<cmath>

std::map<std::string, double> bonuses;

class Employee{
protected:
    std::string name;
    std::string post;
    int money;
public:
    Employee(){
        
    }

    Employee(std::string n, std::string p, int m){
        name = (n);
        post = p;
        money = m;
    }

    virtual int bonus(double b) const{
        return std::round(money * b);
    }

    int result()const {
        return money + bonus(bonuses[post]);
    }

    friend std::ostream& operator<<(std::ostream& os, const Employee& emp) {
        os << emp.name << " (" << emp.post << "): " << emp.result();
        return os;
    }
};

class Manager : public Employee{
public:
    Manager(std::string name, int money = 16242)
    : Employee(name, "manager", money){
    }

    int bonus(double percent) const {
        if (percent < 0.1){
            percent = 0.1;
        }

        return std::round(money * percent);
    }
};

class Boss: public Employee{
public:
    Boss(std::string name, int money = 16242)
    :Employee(name, "boss", money){
    }
};

int main(){
    double boss_bonus, manager_bonus;
    std::cin >> boss_bonus >> manager_bonus;
    bonuses["manager"] = manager_bonus;
    bonuses["boss"] = boss_bonus;

    int john_money, bob_money, alice_money;
    std::cin >> john_money >> bob_money >> alice_money;
    Boss john("John", john_money);
    Manager bob("Bob", bob_money);
    Manager alice("Alice", alice_money);

    std::cout << john << '\n' << bob << '\n' << alice << std::endl;
}