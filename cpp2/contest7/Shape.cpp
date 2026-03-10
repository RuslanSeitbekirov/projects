#include <iostream>
#include <vector>

struct Shape{
    // virtual нужен чтобы через указатель на базовый класс вызывались методы ПРАВИЛЬНОГО дочернего класса
    virtual double getPerimeter(){
        return 0;
    }

    virtual ~Shape() {}
};

class Rectangle : public Shape{
    int a;
    int b;
public:


    Rectangle(int a, int b){
        this -> a = a;
        this -> b = b;
    }

    double getPerimeter(){
        return 2 * (a + b);
    }
};

class Circle : public Shape{
    int r;
public:


    Circle(int r){
        this -> r = r;
    }

    double getPerimeter(){
        return 2 * 3.14 * r;
    }
};

struct Triangle : public Shape{
    public:
        int a;
        int b;
        int c;

        Triangle(int a, int b, int c){
            this -> a = a;
            this -> b = b;
            this -> c = c;
        }

        double getPerimeter(){
            if (a + b > c && a + c > b && b + c > a) {
            return a + b + c;
            }
        return 0;
    }
};


int main(){
    int momentum;
    std::cin >> momentum;
    srand(momentum);
    std::vector<Shape*> shapes(rand() % 100 + 1);
    for (int i=0; i<shapes.size(); i++){
        int a = rand()%10 + 1, b = rand()%10 + 1, c = rand()%10 + 1;
        if (rand() % 3 == 0) shapes[i] = new Rectangle(a, b);
        else if (rand() % 3 == 1) shapes[i] = new Circle(c);
        else shapes[i] = new Triangle(a, b, c);
    }
    
    double sum = 0;
    for (auto shape : shapes) { sum += shape->getPerimeter();}
    std::cout << (int)sum;
}