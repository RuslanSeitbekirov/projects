#include <iostream>
#include <cmath>
#include <iomanip>
using namespace std;

double f(double x) {
    return cos(3 * x) / (1 + 0.7 * cos(x));
}

// Функция вычисления интеграла методом Симпсона для заданного n (чётное)
double simpson(double a, double b, int n) {
    double h = (b - a) / n;
    double sum = f(a) + f(b); // крайние точки
    // Нечётные индексы (1,3,5,...,n-1) с коэффициентом 4
    for (int i = 1; i < n; i += 2) {
        double x = a + i * h;
        sum += 4 * f(x);
    }
    // Чётные индексы (2,4,6,...,n-2) с коэффициентом 2
    for (int i = 2; i < n; i += 2) {
        double x = a + i * h;
        sum += 2 * f(x);
    }
    return (h / 3.0) * sum;
}

int main() {
    double a, b;
    cout << "Введите начало интервала: ";
    cin >> a;
    cout << "Введите конец интервала: ";
    cin >> b;

    const double eps = 1e-4;          // требуемая точность
    int n = 2;                        // начальное число разбиений (чётное)
    double I_old = simpson(a, b, n);  // интеграл при текущем n
    double I_new;
    int max_iter = 20;                 // ограничение на число удвоений

    cout << "n = " << n << ", I = " << I_old << endl;

    for (int iter = 1; iter <= max_iter; ++iter) {
        n *= 2;                        // удваиваем разбиение
        I_new = simpson(a, b, n);
        cout << "n = " << n << ", I = " << I_new << endl;

        if (fabs(I_new - I_old) < eps) {
            cout << "\nДостигнута точность " << eps << endl;
            cout << "Результат: " << I_new << " при n = " << n << endl;
            return 0;
        }
        I_old = I_new;
    }

    cout << "\nНе удалось достичь точности за " << max_iter << " удвоений." << endl;
    return 0;
}