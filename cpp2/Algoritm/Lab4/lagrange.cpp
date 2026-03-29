#include <iostream>
#include <vector>
#include <cmath>
#include <stdexcept>
using namespace std;

/**
 * Вычисляет значение интерполяционного полинома Лагранжа в точке x.
 *
 * @param x_nodes - вектор узлов x_i (должны быть различны)
 * @param y_nodes - вектор значений функции в узлах y_i = f(x_i)
 * @param x       - точка, в которой вычисляется полином
 * @return значение полинома Лагранжа в точке x
 */
double lagrange_interpolation(const vector<double>& x_nodes, const vector<double>& y_nodes, double x) {
    size_t n = x_nodes.size();
    if (n != y_nodes.size()) {
        throw invalid_argument("Размеры векторов x и y должны совпадать");
    }
    if (n == 0) {
        throw invalid_argument("Нет узлов интерполяции");
    }

    double result = 0.0;

    // Для каждого i-го узла вычисляем базисный полином L_i(x)
    for (size_t i = 0; i < n; ++i) {
        double term = y_nodes[i];   // y_i
        for (size_t j = 0; j < n; ++j) {
            if (j != i) {
                // Проверка на возможное деление на ноль (дублирующиеся узлы)
                if (fabs(x_nodes[i] - x_nodes[j]) < 1e-12) {
                    throw invalid_argument("Узлы x_i должны быть различны");
                }
                term *= (x - x_nodes[j]) / (x_nodes[i] - x_nodes[j]);
            }
        }
        result += term;
    }
    return result;
}

// Пример использования
int main() {
    auto p = M_PI;
    try {
        // Пример 1: интерполяция функции sin(x) на отрезке [0, π] по 5 узлам
        vector<double> x_vals = {1, 2, 3, 4, 5};
        vector<double> y_vals = {7.5, 6.2, 5.5, 3.5, 3};

        double test_x = M_PI/3;   // π/3 ≈ 1.0472
        double interp = lagrange_interpolation(x_vals, y_vals, test_x);
        double exact = sin(test_x);

        cout << "Интерполяция sin(x) в точке x = " << test_x << endl;
        cout << "Полином Лагранжа: " << interp << endl;
        cout << "Точное значение: " << exact << endl;
        cout << "Погрешность: " << fabs(interp - exact) << endl << endl;

        // Пример 2: интерполяция квадратичной функции по трём точкам
        vector<double> x2 = {0.0, 1.0, 2.0};
        vector<double> y2 = {1.0, 3.0, 7.0};  // f(x) = x^2 + x + 1
        double test2 = 1.5;
        double interp2 = lagrange_interpolation(x2, y2, test2);
        double exact2 = test2*test2 + test2 + 1;

        cout << "Интерполяция f(x)=x^2+x+1 в x=" << test2 << endl;
        cout << "Полином Лагранжа: " << interp2 << endl;
        cout << "Точное значение: " << exact2 << endl;
        cout << "Погрешность: " << fabs(interp2 - exact2) << endl;

    } catch (const exception& e) {
        cerr << "Ошибка: " << e.what() << endl;
    }

    return 0;
}