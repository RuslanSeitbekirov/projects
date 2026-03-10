#include <iostream>

class Vector;

struct Point{
    int x, y;

    Point(){
        this -> x = 0;
        this -> y = 0;
    }

    Point(int x, int y){
        this -> x = x;
        this -> y = y;
    }

    void move(const Vector& v);

};


struct Vector{
    int x, y;

    Vector(const Point& start, const Point& end){
        x = end.x - start.x;
        y = end.y - start.y;
    }


};


void Point::move(const Vector& v) {
    x += v.x;
    y += v.y;
}


int main(){
    int x1, y1, x2, y2, x3, y3;
    std::cin >> x1 >> y1 >> x2 >> y2 >> x3 >> y3;
    
    Point p(x1, y1);
    Vector dir(Point(x2, y2), Point(x3, y3));
    p.move(dir);
    
    std::cout << p.x << ' ' << p.y;
}