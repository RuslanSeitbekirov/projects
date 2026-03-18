#include <iostream>
#include <cmath>
using namespace std;
 

double f(double x) {
    return cos(3*x)/(1+0.7*cos(x));
}

double rule(double f(double), double a, double b){
    double h = (b - a)/2;
    return (h/3) * (f(a) + 4*f((a+b)/2) + f(b));
}

double simpson_adapt(double f(double), double a, double b, double eps, int max_depth=50){
    double m = (a+b)/2;
    double whole = rule(f, a, b);
    double left = rule(f, a, m);
    double right = rule(f, m, b);
    // Подсчёт погрешности по правилу Рунге
    auto delta = abs(left + right - whole) / 15;

    if (delta < eps * (m-a)/(b-a) || max_depth == 0){
        return left + right;
    } else{
        return (simpson_adapt(f, a, m, eps / 2, max_depth - 1) + simpson_adapt(f, m, b, eps / 2, max_depth - 1));
    }
    
}

int main() {
    double a, b, eps;
    cout << "Введите начало интервала: ";
    cin >> a;
    cout << "Введите конец интервала(через точку): ";
    cin >> b;
    cout << "Введите желаемую точность(1e-4): ";
    cin >> eps;
    double result = simpson_adapt(f, a, b, eps);

    cout << "Результат: " << result << endl;
    return 0;
} 