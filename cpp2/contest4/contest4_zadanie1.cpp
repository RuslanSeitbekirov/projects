#include <iostream>
#include <iomanip>
#include <math.h>


// Перегрузка для расчета по сторонам
double triangle_area(int a, int b, int c) {
    double p = (a + b + c) / 2.0;
    if (a + b > c && a + c > b && b + c > a) {
        return sqrt(p * (p - a) * (p - b) * (p - c));
    } else {
        return -1;
    }
    
}

// Перегрузка для расчета по координатам
double triangle_area(int x1, int y1, int x2, int y2, int x3, int y3) {
    int result;
    int a, b, c;
    a = sqrt(pow(x2-x1, 2)+ pow(y2-y1, 2));
    b = sqrt(pow(x3-x2, 2)+ pow(y3-y2, 2));
    c = sqrt(pow(x3-x1, 2)+ pow(y3-y1, 2));

    if ( a + b > c && a + c > b && b + c > a) {
        return 0.5 * abs(x1 * (y2 - y3) + x2 * (y3 - y1) + x3 * (y1 - y2));
    }
    return -1;
}

int main()
{
    int type;
    std::cin >> type;
    
    double S;
    if (type == 0){
        int a, b, c;
        std::cin >> a >> b >> c;
        S = triangle_area(a, b, c);
    }else{
        int x1, y1, x2, y2, x3, y3;
        std::cin >> x1 >> y1 >> x2 >> y2 >> x3 >> y3;
        S = triangle_area(x1, y1, x2, y2, x3, y3);
    }
    
    std::cout << std::setprecision(5) << S;
}