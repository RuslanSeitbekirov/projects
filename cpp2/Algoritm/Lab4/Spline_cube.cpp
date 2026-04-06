#include <iostream>
#include <vector>
#include <iomanip>
#include <cmath>
#include <algorithm>

using namespace std;

// Структура для хранения коэффициентов кубического полинома на одном отрезке
struct SplineSegment {
    double a, b, c, d; // Коэффициенты полинома a + b*dx + c*dx^2 + d*dx^3
    double x_left;     // Левая граница отрезка
};

class CubicSpline {
private:
    vector<double> x, y;
    vector<SplineSegment> segments;

public:
    CubicSpline(const vector<double>& x_vals, const vector<double>& y_vals) : x(x_vals), y(y_vals) {
        build_spline();
    }

    // Метод построения сплайна (решение трехдиагональной матрицы методом прогонки)
    void build_spline() {
        int n = x.size() - 1; // Количество отрезков
        
        // Массив для вторых производных (c_i)
        // В математических формулах это часто обозначается как M_i
        vector<double> c(n + 1);
        
        // Массивы для метода прогонки
        vector<double> alpha(n + 1); // Прогоночный коэффициент
        vector<double> beta(n + 1);  // Прогоночный коэффициент
        
        // Граничные условия: Естественный сплайн (S'' = 0 на концах)
        // Это означает c[0] = 0.
        alpha[0] = 0;
        beta[0] = 0;
        
        // Прямой ход метода прогонки
        // Уравнение для внутренних узлов (i от 1 до n-1):
        // h_{i-1}*c_{i-1} + 2*(h_{i-1} + h_i)*c_i + h_i*c_{i+1} = 6 * ( (y_{i+1}-y_i)/h_i - (y_i-y_{i-1})/h_{i-1} )
        
        // Так как шаг равномерный (h = 0.04), упростим, но оставим универсальным для надежности
        
        for (int i = 1; i < n; ++i) {
            double h_prev = x[i] - x[i - 1];
            double h_curr = x[i + 1] - x[i];
            
            double A = h_prev;
            double B = 2 * (h_prev + h_curr);
            double C = h_curr;
            double F = 6 * ((y[i + 1] - y[i]) / h_curr - (y[i] - y[i - 1]) / h_prev);
            
            // Формулы прогонки
            double delta = B - A * alpha[i - 1];
            alpha[i] = -C / delta;
            beta[i] = (F - A * beta[i - 1]) / delta;
        }
        
        // Граничное условие справа: c[n] = 0
        c[n] = 0;
        
        // Обратный ход: вычисляем c_i
        for (int i = n - 1; i >= 0; --i) {
            c[i] = alpha[i] * c[i + 1] + beta[i];
        }
        
        // Вычисление остальных коэффициентов для каждого отрезка
        // Формула сплайна на отрезке [x_i, x_{i+1}]:
        // S(x) = a_i + b_i(x - x_i) + c_i(x - x_i)^2 + d_i(x - x_i)^3
        
        segments.resize(n);
        for (int i = 0; i < n; ++i) {
            double h = x[i + 1] - x[i];
            
            segments[i].a = y[i];
            segments[i].x_left = x[i];
            
            // Коэффициенты b, c, d
            segments[i].c = c[i] / 2.0;
            segments[i].d = (c[i + 1] - c[i]) / (6.0 * h);
            segments[i].b = (y[i + 1] - y[i]) / h + h * (2 * c[i] + c[i + 1]) / 6.0; 
            // Исправление формулы b для полинома вида a + b*dx + c*dx^2 + d*dx^3
            // Стандартная формула через c_i: b_i = f[x_i, x_{i+1}] - h/6 * (2*c_i + c_{i+1})
            // Но так как мы ищем разложение по степеням (x-x_i), коэффициенты пересчитываются.
            // Для полинома P(x) = y_i + y'_i(x-x_i) + y''_i/2(x-x_i)^2 + y'''_i/6(x-x_i)^3
            // y_i = a
            // y'_i = (y_{i+1}-y_i)/h - h/6*(2*c_i + c_{i+1})
            // y''_i = c_i
            // y'''_i = (c_{i+1} - c_i)/h
            
            segments[i].b = (y[i + 1] - y[i]) / h - h * (2.0 * c[i] + c[i + 1]) / 6.0;
            // d в полиноме = y''' / 6 = (c_{i+1} - c_i) / (6h)
            // c в полиноме = y'' / 2 = c_i / 2
            // Перезапишем c и d для полиномиальной формы:
            segments[i].c = c[i] / 2.0;
            segments[i].d = (c[i + 1] - c[i]) / (6.0 * h);
        }
    }

    // Поиск значения сплайна в точке x_val
    double interpolate(double x_val) const {
        if (x_val < x.front() || x_val > x.back()) {
            return NAN; // Точка вне диапазона
        }

        // Поиск индекса отрезка (бинарный поиск)
        // Нам нужно найти i, такое что x[i] <= x_val <= x[i+1]
        int idx = std::upper_bound(x.begin(), x.end(), x_val) - x.begin() - 1;
        
        // Если x_val совпадает с последней точкой
        if (idx >= segments.size()) {
            idx = segments.size() - 1;
        }

        const SplineSegment& seg = segments[idx];
        double dx = x_val - seg.x_left;
        
        // Схема Горнера для полинома
        return seg.a + dx * (seg.b + dx * (seg.c + dx * seg.d));
    }
};

int main() {
    // Данные из таблицы (колонка e^x)
    vector<double> x_data = {
        1.00, 1.01, 1.02, 1.03, 1.04, 1.05, 1.06, 1.07, 1.08, 1.09,
        1.10, 1.11, 1.12, 1.13, 1.14, 1.15, 1.16, 1.17, 1.18, 1.19, 1.20
    };

    vector<double> y_data = {
        2.7183, 2.7456, 2.7732, 2.8011, 2.8292, 2.8577, 2.8864, 2.9154,
        2.9447, 2.9743, 3.0042, 3.0344, 3.0649, 3.0957, 3.1268, 3.1582,
        3.1899, 3.2220, 3.2544, 3.2871, 3.3201
    };

    // Точки для интерполяции
    vector<double> query_points = {1.05, 1.09, 1.13, 1.15, 1.17};

    // Построение сплайна
    CubicSpline spline(x_data, y_data);

    // Вывод заголовка таблицы
    cout << fixed << setprecision(6);
    cout << "------------------------------------------------------------------------------------------------" << endl;
    cout << "|    X     | S(x) (Spline) | e^x (Exact)   | Error (abs)                                        |" << endl;
    cout << "------------------------------------------------------------------------------------------------" << endl;

    // Расчет и вывод
    for (double x_val : query_points) {
        double s_val = spline.interpolate(x_val);
        double exact_val = std::exp(x_val);
        double error = std::abs(s_val - exact_val);

        cout << "| " << setw(8) << x_val 
             << " | " << setw(13) << s_val 
             << " | " << setw(13) << exact_val 
             << " | " << setw(10) << scientific << setprecision(2) << error << "         |" << endl;
    }
    cout << "------------------------------------------------------------------------------------------------" << endl;

    return 0;
}