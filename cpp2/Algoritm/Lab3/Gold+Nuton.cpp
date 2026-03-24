#include <iostream>
#include <cmath>
#include <functional>
#include <iomanip>
using namespace std;

// Параметры по умолчанию
const double EPS = 1e-12;        // точность
const int MAX_ITER = 10000;        // максимальное число итераций

// Золотое сечение: φ = (sqrt(5)-1)/2 ≈ 0.6180339887498949
const double PHI = (sqrt(5.0) - 1.0) / 2.0;

/**
 * Метод золотого сечения для поиска корня на отрезке [a, b],
 * где f(a) * f(b) < 0 (функция непрерывна и меняет знак).
 * Возвращает приближённый корень.
 */
double golden_section_root(function<double(double)> f, double a, double b, double eps = EPS) {
    if (f(a) * f(b) > 0) {
        throw runtime_error("На отрезке [a, b] функция не меняет знак. Корень не гарантирован.");
    }

    double x1 = a + (1.0 - PHI) * (b - a);
    double x2 = a + PHI * (b - a);
    double f1 = f(x1);
    double f2 = f(x2);

    while (fabs(b - a) > eps) {
        if (f1 * f(a) <= 0) {
            b = x1;
            x2 = x1;
            f2 = f1;
            x1 = a + (1.0 - PHI) * (b - a);
            f1 = f(x1);
        } else {
            a = x2;
            x1 = x2;
            f1 = f2;
            x2 = a + PHI * (b - a);
            f2 = f(x2);
        }
    }
    return (a + b) / 2.0;
}
/**
 * Метод Ньютона (касательных) для поиска корня.
 * f      – функция
 * df     – производная функции
 * x0     – начальное приближение
 * eps    – точность
 * max_it – максимальное число итераций
 */
double newton_root(function<double(double)> f, function<double(double)> df,
                   double x0, double eps = EPS, int max_it = MAX_ITER) {
    double x = x0;
    for (int iter = 0; iter < max_it; ++iter) {
        double fx = f(x);
        double dfx = df(x);
        if (fabs(dfx) < 1e-15) {
            throw runtime_error("Производная близка к нулю. Метод Ньютона может расходиться.");
        }
        double dx = fx / dfx;
        x -= dx;
        if (fabs(dx) < eps) {
            return x;
        }
    }
    throw runtime_error("Метод Ньютона не сошёлся за максимальное число итераций.");
}

/**
 * Комбинированный метод:
 * 1) локализация корня методом золотого сечения на отрезке [a, b];
 * 2) уточнение методом Ньютона, используя результат как начальное приближение.
 */
double combined_root(function<double(double)> f, function<double(double)> df,
                     double a, double b, double eps = EPS) {
    double approx = golden_section_root(f, a, b, eps * 10); // грубое приближение
    return newton_root(f, df, approx, eps);
}

// Пример использования
int main() {
    // Задаём функцию и её производную
    auto f = [](double x) -> double { return log(x)/log(10)-7/(2*x+6); };
    auto df = [](double x) -> double { return (1/x*log(10))+(14/(2*x+6)*(2*x+6)); };

    double a = 0.0, b = 5;   // отрезок, где f(a)*f(b) < 0

    try {
        cout << fixed << setprecision(12);

        // 1. Метод золотого сечения
        double root_gs = golden_section_root(f, a, b);
        cout << "Метод золотого сечения:\n";
        cout << "Корень = " << root_gs << "\n";
        cout << "f(корень) = " << f(root_gs) << "\n\n";

        // 2. Метод Ньютона (начальное приближение – середина отрезка)
        double root_newton = newton_root(f, df, (a + b) / 2.0);
        cout << "Метод Ньютона:\n";
        cout << "Корень = " << root_newton << "\n";
        cout << "f(корень) = " << f(root_newton) << "\n\n";

        // 3. Комбинированный метод
        double root_comb = combined_root(f, df, a, b);
        cout << "Комбинированный метод (золотое сечение + Ньютон):\n";
        cout << "Корень = " << root_comb << "\n";
        cout << "f(корень) = " << f(root_comb) << "\n";

    } catch (const exception& e) {
        cerr << "Ошибка: " << e.what() << endl;
    }

    return 0;
}