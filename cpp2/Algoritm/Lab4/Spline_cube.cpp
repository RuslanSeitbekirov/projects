#include <iostream>
#include <vector>
#include <cmath>
#include <iomanip>

using namespace std;

struct SplineSegment {
    double a, b, c, d;   // коэффициенты полинома: a + b*(x-x_i) + c*(x-x_i)^2 + d*(x-x_i)^3
    double x_left;       // левая граница интервала
};

// Построение естественного кубического сплайна
vector<SplineSegment> build_cubic_spline(const vector<double>& x, const vector<double>& y) {
    int n = x.size();
    if (n < 2) throw runtime_error("Недостаточно узлов");

    vector<double> h(n - 1);
    for (int i = 0; i < n - 1; ++i) h[i] = x[i + 1] - x[i];

    // Вторые производные m (размер n), m[0] = m[n-1] = 0 (естественные условия)
    vector<double> m(n, 0.0);

    // Построение трёхдиагональной системы для m[1..n-2]
    int N = n - 2; // число внутренних узлов
    vector<double> diag(N, 0.0), sub(N - 1, 0.0), sup(N - 1, 0.0), rhs(N, 0.0);

    for (int i = 1; i <= N; ++i) {
        double hi = h[i];
        double hi_1 = h[i - 1];
        diag[i - 1] = 2.0 * (hi_1 + hi);
        if (i - 2 >= 0) sub[i - 2] = hi_1;
        if (i < N) sup[i - 1] = hi;
        rhs[i - 1] = 6.0 * ((y[i + 1] - y[i]) / hi - (y[i] - y[i - 1]) / hi_1);
    }

    // Метод прогонки
    vector<double> alpha(N, 0.0), beta(N, 0.0);
    alpha[0] = sup[0] / diag[0];
    beta[0] = rhs[0] / diag[0];
    for (int i = 1; i < N - 1; ++i) {
        double denom = diag[i] - sub[i - 1] * alpha[i - 1];
        alpha[i] = sup[i] / denom;
        beta[i] = (rhs[i] - sub[i - 1] * beta[i - 1]) / denom;
    }
    int last = N - 1;
    double denom_last = diag[last] - sub[last - 1] * alpha[last - 1];
    beta[last] = (rhs[last] - sub[last - 1] * beta[last - 1]) / denom_last;

    vector<double> m_inner(N);
    m_inner[last] = beta[last];
    for (int i = last - 1; i >= 0; --i) {
        m_inner[i] = beta[i] - alpha[i] * m_inner[i + 1];
    }
    for (int i = 1; i <= N; ++i) m[i] = m_inner[i - 1];

    // Вычисление коэффициентов сплайнов на каждом интервале
    vector<SplineSegment> segments(n - 1);
    for (int i = 0; i < n - 1; ++i) {
        double hi = h[i];
        double a = y[i];
        double b = (y[i + 1] - y[i]) / hi - hi * (2.0 * m[i] + m[i + 1]) / 6.0;
        double c = m[i] / 2.0;
        double d = (m[i + 1] - m[i]) / (6.0 * hi);
        segments[i] = {a, b, c, d, x[i]};
    }
    return segments;
}

// Вычисление значения сплайна в точке x
double spline_eval(const vector<SplineSegment>& segments, double x) {
    for (const auto& seg : segments) {
        double x_right = seg.x_left + (segments[0].x_left - segments[0].x_left); // не очень красиво, лучше определить отдельно
        // Найдём нужный интервал (простой линейный поиск)
    }
    // Более корректный поиск:
    int idx = -1;
    for (size_t i = 0; i < segments.size(); ++i) {
        double x_left = segments[i].x_left;
        double x_right = (i + 1 < segments.size()) ? segments[i + 1].x_left : x_left + 0.04; // последний интервал
        if (x >= x_left && x <= x_right) {
            idx = i;
            break;
        }
    }
    if (idx == -1) throw runtime_error("Точка вне диапазона");
    double dx = x - segments[idx].x_left;
    return segments[idx].a + segments[idx].b * dx + segments[idx].c * dx * dx + segments[idx].d * dx * dx * dx;
}

int main() {
    // Узлы с шагом 0.04
    vector<double> x_nodes = {1.00, 1.04, 1.08, 1.12, 1.16, 1.20};
    // Значения e^x из таблицы (округлены до 4 знаков)
    vector<double> y_nodes = {2.7183, 2.8292, 2.9447, 3.0649, 3.1899, 3.3201};

    auto exact = [](double x) { return exp(x); };

    try {
        auto spline = build_cubic_spline(x_nodes, y_nodes);
        vector<double> points = {1.05, 1.09, 1.13, 1.15, 1.17};

        cout << fixed << setprecision(6);
        cout << "Кубический сплайн (естественные граничные условия) для e^x\n";
        cout << "Узлы: ";
        for (double x : x_nodes) cout << x << " ";
        cout << "\n\n";
        cout << "x\t\tСплайн\t\tТочное e^x\tПогрешность\n";
        for (double xp : points) {
            double spl_val = spline_eval(spline, xp);
            double exact_val = exact(xp);
            double err = fabs(spl_val - exact_val);
            cout << xp << "\t\t" << spl_val << "\t" << exact_val << "\t" << err << endl;
        }

        // Проверка в узлах
        cout << "\nПроверка в узлах:\n";
        for (size_t i = 0; i < x_nodes.size(); ++i) {
            double spl_val = spline_eval(spline, x_nodes[i]);
            cout << "x=" << x_nodes[i] << ", сплайн=" << spl_val << ", исходное y=" << y_nodes[i] << endl;
        }
    } catch (const exception& e) {
        cerr << "Ошибка: " << e.what() << endl;
    }
    return 0;
}