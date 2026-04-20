#include <iostream>
#include <cmath>
#include <vector>
#include <iomanip>
#include <functional>
#include <fstream>
#include <string>

using namespace std;

// ============================================================
// 1. ЦЕЛЕВАЯ ФУНКЦИЯ (легко изменить)
// ============================================================
double target_function(double x) {
    return (sin(x)*sin(x))/(13-12*cos(x));
}

// ============================================================
// 2. АДАПТИВНЫЙ СИМПСОН (из лаб. работы №1)
// ============================================================
double simpson_adaptive(double a, double b, double eps, const function<double(double)>& func) {
    int n = 2;
    auto compute_sum = [&](int N) -> double {
        double h = (b - a) / N;
        double sum = func(a) + func(b);
        for (int i = 1; i < N; i += 2) sum += 4 * func(a + i * h);
        for (int i = 2; i < N; i += 2) sum += 2 * func(a + i * h);
        return (h / 3.0) * sum;
    };

    double I_old = compute_sum(n);
    for (int iter = 1; iter <= 20; ++iter) {
        n *= 2;
        double I_new = compute_sum(n);
        if (fabs(I_new - I_old) < eps) return I_new;
        I_old = I_new;
    }
    return I_old;
}

// ============================================================
// 3. ВЫЧИСЛЕНИЕ РЯДА ФУРЬЕ
// ============================================================
void compute_fourier(const function<double(double)>& f, double L, int N, double eps,
                     vector<double>& a, vector<double>& b) {
    a.resize(N+1); b.resize(N+1);
    
    // a0
    a[0] = simpson_adaptive(-L, L, eps, f) / L;
    
    // an, bn
    for (int n = 1; n <= N; ++n) {
        int k = n;
        a[n] = simpson_adaptive(-L, L, eps, [f, k](double x) { return f(x) * cos(k * x); }) / L;
        b[n] = simpson_adaptive(-L, L, eps, [f, k](double x) { return f(x) * sin(k * x); }) / L;
    }
}

// Частичная сумма ряда Фурье
double fourier_partial_sum(double x, const vector<double>& a, const vector<double>& b) {
    double S = a[0] / 2.0;
    for (size_t n = 1; n < a.size(); ++n) {
        S += a[n] * cos(n * x) + b[n] * sin(n * x);
    }
    return S;
}

// ============================================================
// 4. ЭКСПОРТ ДАННЫХ И ГЕНЕРАЦИЯ СКРИПТА ПЛОТТИНГА
// ============================================================
void export_data(const string& filename, double L, int points, 
                 const function<double(double)>& f, 
                 const vector<double>& a, const vector<double>& b) {
    ofstream out(filename);
    out << fixed << setprecision(6);
    out << "# x\tf(x)\tS_N(x)\n";
    
    for (int i = 0; i <= points; ++i) {
        double x = -L + 2 * L * i / points;
        out << x << "\t" << f(x) << "\t" << fourier_partial_sum(x, a, b) << "\n";
    }
    out.close();
    cout << "✓ Данные экспортированы в " << filename << endl;
}

void generate_python_plot(const string& data_file, const string& plot_file) {
    ofstream out(plot_file);
    out << "import matplotlib.pyplot as plt\n"
        << "import numpy as np\n\n"
        << "# Загрузка данных\ndata = np.loadtxt('" << data_file << "', comments='#')\n"
        << "x, f_x, S_x = data[:,0], data[:,1], data[:,2]\n\n"
        << "# Построение графика\nplt.figure(figsize=(10, 6))\n"
        << "plt.plot(x, f_x, 'b-', linewidth=2, label='f(x) - исходная функция')\n"
        << "plt.plot(x, S_x, 'r--', linewidth=1.5, label='S_N(x) - ряд Фурье')\n"
        << "plt.xlabel('x'); plt.ylabel('y'); plt.title('Разложение в ряд Фурье');\n"
        << "plt.grid(True, alpha=0.3); plt.legend(); plt.axhline(0, color='k', linewidth=0.5)\n"
        << "plt.tight_layout(); plt.savefig('fourier_plot.png', dpi=300)\n"
        << "plt.show()\n";
    out.close();
    cout << "✓ Скрипт для построения графика создан: " << plot_file << endl;
}

// ============================================================
// 5. ОСНОВНАЯ ПРОГРАММА
// ============================================================
int main() {
    setlocale(LC_ALL, "Russian");
    const double PI = acos(-1.0);
    
    // Параметры
    const double eps = 1e-6;      // точность интегрирования
    const int N = 50;             // количество гармоник
    const double L = PI;          // интервал [-π, π]
    const int PLOT_POINTS = 1000; // точек для графика

    cout << "=== Разложение в ряд Фурье с экспортом графика ===\n";
    
    // Вычисление коэффициентов
    vector<double> a, b;
    compute_fourier(target_function, L, N, eps, a, b);

    // Вывод коэффициентов
    cout << "\nКоэффициенты (первые 5 гармоник):\n";
    cout << fixed << setprecision(5);
    cout << "a0 = " << a[0] << "\n";
    for (int n = 1; n <= min(5, N); ++n)
        cout << "a" << n << " = " << a[n] << ", b" << n << " = " << b[n] << "\n";

    // Экспорт данных и генерация скрипта
    export_data("fourier_data.csv", L, PLOT_POINTS, target_function, a, b);
    generate_python_plot("fourier_data.csv", "plot_fourier.py");

    // Инструкция для пользователя
    cout << "\n=== Как построить график ===\n";
    cout << "1. Убедитесь, что установлен Python с matplotlib:\n";
    cout << "   pip install matplotlib numpy\n";
    cout << "2. Запустите скрипт:\n";
    cout << "   python plot_fourier.py\n";
    cout << "3. График сохранится как 'fourier_plot.png' и откроется в окне.\n";
    cout << "\nАльтернативно: откройте fourier_data.csv в Excel или используйте gnuplot:\n";
    cout << "   gnuplot -e \"plot 'fourier_data.csv' using 1:2 with lines title 'f(x)', '' using 1:3 with lines title 'S_N(x)'\"\n";

    return 0;
}