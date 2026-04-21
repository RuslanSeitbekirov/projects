#include <iostream>
#include <vector>
#include <cmath>
#include <iomanip>
#include <string>
#include <algorithm>

// ============================================================================
// ФУНКЦИЯ И ЕЁ ПРОИЗВОДНАЯ
// ============================================================================
double f(double x) {
    return std::exp(-x);
}

double f_prime(double x) {
    return -std::exp(-x);
}

// ============================================================================
// ПОСТРОЕНИЕ ПОЛИНОМА ЭРМИТА (разделённые разности)
// ============================================================================
struct HermitePolynomial {
    std::vector<double> z;           // узлы (с кратностью)
    std::vector<double> coefficients; // коэффициенты (диагональ таблицы)
};

HermitePolynomial hermite_interpolation(const std::vector<double>& x_nodes,
                                        const std::vector<double>& y_nodes,
                                        const std::vector<double>& y_deriv_nodes,
                                        double eps = 1e-10) {
    const size_t n = x_nodes.size();
    const size_t m = 2 * n;  // количество узлов с учётом кратности

    // 1. Формируем массив узлов z (каждый узел повторяется дважды)
    std::vector<double> z(m);
    for (size_t i = 0; i < n; ++i) {
        z[2*i]     = x_nodes[i];
        z[2*i + 1] = x_nodes[i];
    }

    // 2. Инициализируем таблицу разделённых разностей Q (m x m)
    std::vector<std::vector<double>> Q(m, std::vector<double>(m, 0.0));

    // Заполняем первый столбец значениями функции
    for (size_t i = 0; i < n; ++i) {
        Q[2*i][0]     = y_nodes[i];
        Q[2*i + 1][0] = y_nodes[i];
    }

    // 3. Вычисляем разделённые разности
    for (size_t j = 1; j < m; ++j) {
        for (size_t i = j; i < m; ++i) {
            if (std::abs(z[i] - z[i - j]) < eps) {
                // Совпадающие узлы: используем производную (только для j==1)
                if (j == 1) {
                    size_t original_idx = i / 2;  // индекс исходного узла
                    Q[i][j] = y_deriv_nodes[original_idx];
                } else {
                    // Для высших порядков при совпадении — 0
                    // (в полной реализации здесь нужны пределы/рекуррентные формулы)
                    Q[i][j] = 0.0;
                }
            } else {
                // Стандартная формула разделённых разностей
                Q[i][j] = (Q[i][j-1] - Q[i-1][j-1]) / (z[i] - z[i - j]);
            }
        }
    }

    // 4. Извлекаем коэффициенты (диагональ таблицы)
    HermitePolynomial poly;
    poly.z.resize(m);
    poly.coefficients.resize(m);
    for (size_t i = 0; i < m; ++i) {
        poly.z[i] = z[i];
        poly.coefficients[i] = Q[i][i];
    }

    return poly;
}

// ============================================================================
// ВЫЧИСЛЕНИЕ ЗНАЧЕНИЯ ПОЛИНОМА (схема Горнера)
// ============================================================================
double eval_hermite(const std::vector<double>& z,
                    const std::vector<double>& coefficients,
                    double x_val) {
    const size_t n = coefficients.size();
    double result = coefficients[n - 1];

    for (int i = static_cast<int>(n) - 2; i >= 0; --i) {
        result = result * (x_val - z[i]) + coefficients[i];
    }
    return result;
}

// ============================================================================
// ВЫВОД КОЭФФИЦИЕНТОВ
// ============================================================================
void print_hermite_coefficients(const std::vector<double>& z,
                                const std::vector<double>& coefficients) {
    std::cout << "\n" << std::string(80, '=') << "\n";
    std::cout << "КОЭФФИЦИЕНТЫ ПОЛИНОМА ЭРМИТА (разделённые разности)\n";
    std::cout << std::string(80, '=') << "\n";
    
    std::cout << std::fixed << std::setprecision(4);
    std::cout << std::left << std::setw(5) << "№"
              << std::setw(12) << "Узел z_i"
              << std::setw(20) << "Коэффициент c_i" << "\n";
    std::cout << std::string(80, '-') << "\n";

    std::cout << std::setprecision(10);
    for (size_t i = 0; i < coefficients.size(); ++i) {
        std::cout << std::left << std::setw(5) << i
                  << std::setw(12) << z[i]
                  << std::setw(20) << coefficients[i] << "\n";
    }
    std::cout << std::string(80, '=') << "\n";
}

// ============================================================================
// ОСНОВНАЯ ПРОГРАММА
// ============================================================================
int main() {
    // Узлы интерполяции
    std::vector<double> x_nodes = {1.00, 1.04, 1.08, 1.12, 1.16, 1.20};
    const size_t n = x_nodes.size();

    // Значения функции и производной
    std::vector<double> y_nodes(n), y_deriv_nodes(n);
    for (size_t i = 0; i < n; ++i) {
        y_nodes[i]      = f(x_nodes[i]);
        y_deriv_nodes[i] = f_prime(x_nodes[i]);
    }

    std::cout << std::string(80, '=') << "\n";
    std::cout << "ИНТЕРПОЛЯЦИОННЫЙ ПОЛИНОМ ЭРМИТА ДЛЯ ФУНКЦИИ y = e^(-x)\n";
    std::cout << std::string(80, '=') << "\n\n";

    std::cout << "Узлы интерполяции:\n";
    std::cout << "x: ";
    std::cout << std::fixed << std::setprecision(2);
    for (double x : x_nodes) std::cout << x << " ";
    std::cout << "\n";

    std::cout << "y = e^(-x): ";
    std::cout << std::setprecision(6);
    for (double y : y_nodes) std::cout << y << " ";
    std::cout << "\n";

    std::cout << "y' = -e^(-x): ";
    for (double dy : y_deriv_nodes) std::cout << dy << " ";
    std::cout << "\n";

    // Построение полинома Эрмита
    HermitePolynomial poly = hermite_interpolation(x_nodes, y_nodes, y_deriv_nodes);

    // Вывод коэффициентов
    print_hermite_coefficients(poly.z, poly.coefficients);

    // Точки для вычисления
    std::vector<double> points_to_eval = {1.05, 1.09, 1.13, 1.15, 1.17};

    std::cout << "\n" << std::string(80, '=') << "\n";
    std::cout << "ЗНАЧЕНИЯ ПОЛИНОМА ЭРМИТА В ЗАДАННЫХ ТОЧКАХ\n";
    std::cout << std::string(80, '=') << "\n";

    std::cout << std::left << std::setw(12) << "Точка x"
              << std::setw(18) << "Полином H(x)"
              << std::setw(18) << "Точное e^(-x)"
              << std::setw(15) << "Погрешность" << "\n";
    std::cout << std::string(80, '-') << "\n";

    std::cout << std::fixed << std::setprecision(2);
    for (double x_val : points_to_eval) {
        double hermite_val = eval_hermite(poly.z, poly.coefficients, x_val);
        double exact_val   = f(x_val);
        double error       = std::abs(hermite_val - exact_val);

        std::cout << std::setprecision(2)  << std::left << std::setw(12) << x_val;
        std::cout << std::setprecision(10) << std::left << std::setw(18) << hermite_val;
        std::cout << std::setprecision(10) << std::left << std::setw(18) << exact_val;
        std::cout << std::setprecision(2)  << std::scientific << std::setw(15) << error << "\n";
    }

    std::cout << std::string(80, '=') << "\n";
    std::cout << "Количество узлов интерполяции: " << n << "\n";
    std::cout << "Степень полинома Эрмита: " << (2*n - 1) << "\n";
    std::cout << "Количество коэффициентов: " << poly.coefficients.size() << "\n";

    return 0;
}