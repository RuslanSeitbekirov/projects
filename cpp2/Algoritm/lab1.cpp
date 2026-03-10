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

double simpson_adaptive(double f(double), double a, double b, double eps, int max_n, double h) {
    int n = 4;                     // начальное число отрезков
    double I_h = (h / 3.0) * (f(a) + simpson_odd(a, h, n) + simpson_even(a, h, n) + f(b));
    while (n <= max_n) {
        n *= 2;
        double I_h2 = (h / 3.0) * (f(a) + simpson_odd(a, h, n) + simpson_even(a, h, n) + f(b));
        double err = fabs(I_h2 - I_h) / 15.0;   // оценка погрешности по Рунге
        if (err < eps) {
            // уточнённое значение (экстраполяция)
            return I_h2 + (I_h2 - I_h) / 15.0;
        }
        I_h = I_h2;
    }
    // если не сошлись — обработка ошибки
    return I_h; // или throw
}


int main() {
    double a, b, eps = 1.0/10000;
    int n;
    cout << "Введите начало интервала: ";
    cin >> a;
    cout << "Введите конец интервала(через точку): ";
    cin >> b;
    cout << "Введите максимальное количество разбиений(n - должно быть чётным): ";
    cin >> n;          // число отрезков (должно быть чётным)
    if (n % 2 != 0){
        return 0;
    }
    double h = (b - a) / n;
    // Вычисляем интеграл по составной формуле Симпсона
    double I = (h / 3.0) * (f(a) + simpson_odd(a, h, n) + simpson_even(a, h, n) + f(b));
    cout << "Результат: " << I << endl;
    cout << "Результат: " << simpson_adaptive(f, a, b, eps, n, h) << endl;
    return 0;
} 