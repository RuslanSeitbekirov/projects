#include <iostream>
#include <cmath>
#include <iomanip>
#include <optional>
#include <string>

double f(double x) {
    return std::pow(2.0, x) - 2.0 * x * x + 1.0;
}

std::optional<double> quadratic_interpolation_min_direct(double x1, double x2, double x3, double eps = 1e-6, int max_iter = 1000) {
    std::cout << std::string(80, '=') << "\n";
    std::cout << "ПРЯМАЯ КВАДРАТИЧНАЯ ИНТЕРПОЛЯЦИЯ (поиск минимума)\n";
    std::cout << std::string(80, '=') << "\n\n";
 
    // Фиксируем формат вывода для таблицы
    std::cout << std::fixed << std::setprecision(6);
    std::cout << "Начальные точки: x1=" << x1 << ", x2=" << x2 << ", x3=" << x3 << "\n";
    std::cout << "f(x1)=" << f(x1) << ", f(x2)=" << f(x2) << ", f(x3)=" << f(x3) << "\n\n";

    if (!(x1 < x2 && x2 < x3)) {
        std::cerr << "Ошибка: точки должны быть упорядочены x1 < x2 < x3\n";
        return std::nullopt;
    }

    std::cout << std::left << std::setw(8) << "Итерация   "
              << std::setw(10) << "x1"
              << std::setw(10) << "x2"
              << std::setw(10) << "x3"
              << std::setw(12) << "x_min"
              << std::setw(12) << "f(x_min)" << "\n";
    std::cout << std::string(80, '-') << "\n";

    double x_min = x2; // На случай выхода по max_iter

    for (int k = 0; k < max_iter; ++k) {
        double f1 = f(x1), f2 = f(x2), f3 = f(x3);

        double numerator   = (x2*x2 - x3*x3)*f1 + (x3*x3 - x1*x1)*f2 + (x1*x1 - x2*x2)*f3;
        double denominator = (x2 - x3)*f1 + (x3 - x1)*f2 + (x1 - x2)*f3;

        if (std::abs(denominator) < 1e-10) {
            std::cerr << "Ошибка: деление на ноль!\n";
            return std::nullopt;
        }

        x_min = 0.5 * numerator / denominator;
        double f_min = f(x_min);

        std::cout << std::left << std::setw(8) << k
                  << std::setw(10) << x1
                  << std::setw(10) << x2
                  << std::setw(10) << x3
                  << std::setw(12) << x_min
                  << std::setw(12) << f_min << "\n";

        if (std::abs(x2 - x_min) < eps) {
            std::cout << "\nМинимум найден за " << k << " итераций!\n";
            return x_min;
        }

        // Сжимаем интервал
        if (x_min < x2) {
            if (f_min < f2) {
                x3 = x2;
                x2 = x_min;
            } else {
                x1 = x_min;
            }
        } else {
            if (f_min < f2) {
                x1 = x2;
                x2 = x_min;
            } else {
                x3 = x_min;
            }
        }
    }

    std::cout << "\nДостигнуто максимальное число итераций!\n";
    return x_min;
}

double f_double_prime(double x) {
    double log2 = std::log(2.0);
    return std::pow(2.0, x) * log2 * log2 - 4.0;
}

int main() {
    // Начальные точки охватывают минимум (для данной функции минимум ~4.6)
    double x1 = 2.0, x2 = 4.5, x3 = 7.0;

    auto result = quadratic_interpolation_min_direct(x1, x2, x3);

    if (result.has_value()) {
        double x_min = result.value();
        
        std::cout << "\n" << std::string(80, '=') << "\n";
        std::cout << "РЕЗУЛЬТАТ:\n";
        std::cout << std::string(80, '=') << "\n";
        
        std::cout << std::fixed << std::setprecision(8);
        std::cout << "Точка минимума: x_min = " << x_min << "\n";
        std::cout << "f(x_min) = " << f(x_min) << "\n";

        double f2 = f_double_prime(x_min);
        std::cout << std::setprecision(6);
        std::cout << "f''(x_min) = " << f2 << "\n";

        if (f2 > 0) {
            std::cout << "✓ Это ТОЧКА МИНИМУМА\n";
        } else {
            std::cout << "✗ Это НЕ минимум!\n";
        }
    } else {
        std::cout << "Минимум не найден из-за ошибки.\n";
    }

    return 0;
}