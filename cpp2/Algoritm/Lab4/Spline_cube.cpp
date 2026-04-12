#include <iostream>
#include <vector>
#include <iomanip>
#include <cmath>
#include <algorithm>
#include <string>
using namespace std;

// Структура для хранения коэффициентов кубического полинома на одном отрезке
struct SplineSegment {
    double a, b, c, d;      // Коэффициенты: a + b*dx + c*dx^2 + d*dx^3 ||| a Задаёт высоту в начале, b Задаёт начальный наклон,
    //  c Задаёт, как быстро меняется наклон, d Задаёт, как быстро меняется закругление
    double x_left, x_right; // Границы отрезка [x_left, x_right]
    
    // Вывод формулы в читаемом виде
    string get_formula() const {
        ostringstream oss;
        oss << fixed << setprecision(6);
        oss << "S(x) = " << a;
        if (b >= 0) oss << " + " << b << "*(x - " << x_left << ")";
        else oss << " - " << -b << "*(x - " << x_left << ")";
        if (c >= 0) oss << " + " << c << "*(x - " << x_left << ")^2";
        else oss << " - " << -c << "*(x - " << x_left << ")^2";
        if (d >= 0) oss << " + " << d << "*(x - " << x_left << ")^3";
        else oss << " - " << -d << "*(x - " << x_left << ")^3";
        return oss.str();
    }
};

class CubicSpline {
private:
    vector<double> x, y;
    vector<SplineSegment> segments;

    // Вспомогательная функция: найти индекс отрезка для точки
    int find_segment(double x_val) const {
        if (x_val < x.front() || x_val > x.back()) return -1;
        int idx = upper_bound(x.begin(), x.end(), x_val) - x.begin() - 1;
        if (idx >= static_cast<int>(segments.size())) idx = segments.size() - 1;
        return idx;
    }

public:
    CubicSpline(const vector<double>& x_vals, const vector<double>& y_vals): 
    x(x_vals), y(y_vals) { // записывает вектор точек в приватные x, y
        build_spline();
    }

    // Метод построения сплайна (метод прогонки)
    void build_spline() {
        int n = x.size() - 1;
        vector<double> c(n + 1);          // Вторые производные M_i
        vector<double> alpha(n + 1), beta(n + 1);
        
        // Естественные граничные условия: S''(x0) = S''(xn) = 0
        alpha[0] = 0; beta[0] = 0;
        
        // Прямой ход прогонки
        for (int i = 1; i < n; ++i) {
            double h_prev = x[i] - x[i - 1];
            double h_curr = x[i + 1] - x[i];
            double A = h_prev, B = 2 * (h_prev + h_curr), C = h_curr;
            double F = 6 * ((y[i + 1] - y[i]) / h_curr - (y[i] - y[i - 1]) / h_prev);
            
            double delta = B - A * alpha[i - 1];
            alpha[i] = -C / delta;
            beta[i] = (F - A * beta[i - 1]) / delta;
        }
        
        // Обратный ход
        c[n] = 0;
        for (int i = n - 1; i >= 0; --i) {
            c[i] = alpha[i] * c[i + 1] + beta[i];
        }
        
        // Вычисление коэффициентов полиномов
        segments.resize(n);
        for (int i = 0; i < n; ++i) {
            double h = x[i + 1] - x[i];
            segments[i].a = y[i];
            segments[i].x_left = x[i];
            segments[i].x_right = x[i + 1];
            segments[i].c = c[i] / 2.0;
            segments[i].d = (c[i + 1] - c[i]) / (6.0 * h);
            segments[i].b = (y[i + 1] - y[i]) / h - h * (2.0 * c[i] + c[i + 1]) / 6.0;
        }
    }

    // Интерполяция в точке
    double interpolate(double x_val) const {
        //ищем в каком сигменте наш x
        int idx = find_segment(x_val);
        if (idx == -1) return NAN;
        // берем коэфиценты из сегмента в котором наш и считаем
        const SplineSegment& seg = segments[idx];
        double dx = x_val - seg.x_left;
        return seg.a + dx * (seg.b + dx * (seg.c + dx * seg.d)); // Схема Горнера
    }

    //Вывод формулы для отрезка, содержащего x_val
    void print_formula_at(double x_val) const {
        int idx = find_segment(x_val);
        if (idx == -1) {
            cout << " Точка x = " << x_val << " вне диапазона [" 
                 << x.front() << ", " << x.back() << "]\n";
            return;
        }
        const SplineSegment& seg = segments[idx];
        cout << "\n Формула сплайна на отрезке [" << seg.x_left << ", " << seg.x_right << "]:\n";
        cout << "   " << seg.get_formula() << "\n";
        cout << "   где dx = (x - " << seg.x_left << ")\n";
    }

    //Детальная информация о сегменте
    struct SegmentInfo {
        int index;
        double x_left, x_right;
        double a, b, c, d;
        bool valid;
    };
    
    SegmentInfo get_segment_info(double x_val) const {
        SegmentInfo info;
        int idx = find_segment(x_val);
        info.valid = (idx != -1);
        if (info.valid) {
            info.index = idx;
            info.x_left = segments[idx].x_left;
            info.x_right = segments[idx].x_right;
            info.a = segments[idx].a;
            info.b = segments[idx].b;
            info.c = segments[idx].c;
            info.d = segments[idx].d;
        }
        return info;
    }

    //Вывод всех коэффициентов сплайна (для отладки)
    void print_all_coefficients() const {
        cout << "\n Коэффициенты всех отрезков:\n";
        cout << string(80, '-') << "\n";
        cout << setw(12) << "Отрезок" 
             << setw(12) << "a" 
             << setw(12) << "b" 
             << setw(12) << "c" 
             << setw(12) << "d" << "\n";
        cout << string(80, '-') << "\n";
        for (size_t i = 0; i < segments.size(); ++i) {
            const auto& s = segments[i];
            cout << "[" << fixed << setprecision(2) << s.x_left << ", " << s.x_right << "] "
                 << scientific << setprecision(4)
                 << setw(12) << s.a << setw(12) << s.b 
                 << setw(12) << s.c << setw(12) << s.d << "\n";
        }
        cout << string(80, '-') << "\n";
    }
};

int main() {
    // Данные: колонка e^x из таблицы
    vector<double> x_data = {
        1.00, 1.01, 1.02, 1.03, 1.04, 1.05, 1.06, 1.07, 1.08, 1.09,
        1.10, 1.11, 1.12, 1.13, 1.14, 1.15, 1.16, 1.17, 1.18, 1.19, 1.20
    };
    vector<double> y_data = {
        2.7183, 2.7456, 2.7732, 2.8011, 2.8292, 2.8577, 2.8864, 2.9154,
        2.9447, 2.9743, 3.0042, 3.0344, 3.0649, 3.0957, 3.1268, 3.1582,
        3.1899, 3.2220, 3.2544, 3.2871, 3.3201
    };

    CubicSpline spline(x_data, y_data);

    // Режим 1: Тестовые точки из задания
    vector<double> query_points = {1.05, 1.09, 1.13, 1.15, 1.17};
    
    cout << fixed << setprecision(6);
    cout << "\nРЕЖИМ 1: Проверка в заданных точках\n";
    cout << string(90, '=') << "\n";
    cout << "|" << setw(10) << "X" 
         << "|" << setw(15) << "S(x)" 
         << "|" << setw(15) << "e^x (exact)" 
         << "|" << setw(15) << "Error" << "|\n";
    cout << string(90, '-') << "\n";
    
    for (double xv : query_points) {
        double s_val = spline.interpolate(xv);
        double exact = exp(xv);
        cout << "|" << setw(10) << xv 
             << "|" << setw(15) << s_val 
             << "|" << setw(15) << exact 
             << "|" << scientific << setw(13) << abs(s_val - exact) << "|\n";
    }
    cout << string(90, '=') << "\n";

    //a РЕЖИМ 2: Интерактивный запрос произвольной точки
    cout << "\nРЕЖИМ 2: Произвольная точка\n";
    cout << "Введите x для вычисления (или 'q' для выхода, диапазон [1.00, 1.20]):\n";
    
    string input;
    while (true) {
        cout << "\nx = ";
        cin >> input;
        
        // Проверка на выход
        if (input == "q" || input == "Q") break;
        
        try {
            double x_query = stod(input);
            
            // Вычисление значения
            double result = spline.interpolate(x_query);
            
            if (isnan(result)) {
                cout << " Ошибка: x = " << x_query << " вне диапазона [1.00, 1.20]\n";
                continue;
            }
            
            // Вывод результата
            cout << fixed << setprecision(8);
            cout << " S(" << x_query << ") = " << result << "\n";
            cout << "   e^(" << x_query << ')' << " = " << exp(x_query) << "\n";
            cout << "   Погрешность: " << scientific << abs(result - exp(x_query)) << "\n";
            
            // 🔹 Вывод формулы для этого отрезка
            spline.print_formula_at(x_query);
            
            // 🔹 Детальная информация о коэффициентах
            auto info = spline.get_segment_info(x_query);
            if (info.valid) {
                cout << "\n Коэффициенты полинома на этом отрезке:\n";
                cout << "   a = " << fixed << setprecision(8) << info.a << "  (y_i)\n";
                cout << "   b = " << info.b << "  (первая производная в левой границе)\n";
                cout << "   c = " << info.c << "  (вторая производная / 2)\n";
                cout << "   d = " << info.d << "  (третья производная / 6)\n";
            }
            
        } catch (const exception& e) {
            cout << "⚠ Неверный ввод. Попробуйте число или 'q' для выхода.\n";
        }
    }
    return 0;
}