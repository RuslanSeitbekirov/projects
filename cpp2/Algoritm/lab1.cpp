#include <iostream>
#include <cmath>
using namespace std;

double f(double x) {
    //знаменатель не должен быть равен нулю на [a,b]
    return 1.0 / ((0.5 * sin(x) + 3 * cos(x)) * (0.5 * sin(x) + 3 * cos(x)));
}

double simpson_odd(double a, double h, int n) {
    double total = 0;
    for (int i = 1; i < n; i += 2) {
        double x = a + i * h;
        total += f(x);
    }
    return 4 * total;
}

double simpson_even(double a, double h, int n) {
    double total = 0;
    for (int i = 2; i < n; i += 2) {
        double x = a + i * h;
        total += f(x);
    }
    return 2 * total;
}

int main() {
    double a, b;
    int n;
    cout << "Введите начало интервала: ";
    cin >> a;
    cout << "Введите конец интервала(через точку): ";
    cin >> b;
    cout << "Введите количество разбиений(n - должно быть чётным): ";
    cin >> n;          // число отрезков (должно быть чётным)
    if (n % 2 != 0){
        return 0;
    }
    double h = (b - a) / n;
    // Вычисляем интеграл по составной формуле Симпсона
    double I = (h / 3.0) * (f(a) + simpson_odd(a, h, n) + simpson_even(a, h, n) + f(b));
    cout << "Результат: " << I << endl;
    return 0;
}