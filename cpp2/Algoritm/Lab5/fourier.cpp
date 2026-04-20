#include <iostream>
#include <cmath>
#include <iomanip>
#include <functional>   // <-- ИСПРАВЛЕНИЕ ОШИБКИ: 'function' does not name a type
#include <vector>
#include <fstream>

using namespace std;

// ============================================================
// 1. ИСХОДНАЯ ФУНКЦИЯ (из лаб. №1)
// ============================================================
double target_f(double x) {
    return (sin(x) * sin(x)) / (13 - 12 * cos(x));
}

// ============================================================
// 2. АДАПТИВНЫЙ СИМПСОН (на основе лаб. №1)
//    Принимает подынтегральную функцию через std::function
// ============================================================
double simpson_adaptive(double a, double b, double eps, const function<double(double)>& integrand) {
    int n = 2; // начальное чётное число разбиений
    
    // Лямбда для вычисления суммы Симпсона при заданном N
    auto calc_integral = [&](int N) -> double {
        double h = (b - a) / N;
        double sum = integrand(a) + integrand(b);
        for (int i = 1; i < N; i += 2) sum += 4 * integrand(a + i * h);
        for (int i = 2; i < N; i += 2) sum += 2 * integrand(a + i * h);
        return (h / 3.0) * sum;
    };

    double I_old = calc_integral(n);
    
    // Удваиваем N до достижения точности eps
    for (int iter = 1; iter <= 25; ++iter) {
        n *= 2;
        double I_new = calc_integral(n);
        if (fabs(I_new - I_old) < eps) return I_new;
        I_old = I_new;
    }
    return I_old; // возвращаем последнее вычисление
}

// ============================================================
// 3. ОСНОВНАЯ ПРОГРАММА
// ============================================================
int main() {
    setlocale(LC_ALL, "Russian");
    const double PI = acos(-1.0);
    
    const double eps = 1e-9;  // точность интегрирования
    const int N = 60;         // количество гармоник ряда
    const double L = PI;      // интервал [-π, π]

    cout << "=== Ряд Фурье для f(x) = sin^2(x)/(13-12cos(x)) ===\n";
    cout << "Вычисление коэффициентов... (может занять несколько секунд)\n";

    vector<double> a(N + 1), b(N + 1);

    // --- a0 ---
    a[0] = simpson_adaptive(-L, L, eps, target_f) / L;

    // --- an и bn ---
    for (int k = 1; k <= N; ++k) {
        int n = k; // копия для безопасного захвата в лямбду
        
        // Подынтегральные выражения для an и bn
        auto integrand_an = [n](double x) { return target_f(x) * cos(n * x); };
        auto integrand_bn = [n](double x) { return target_f(x) * sin(n * x); };

        a[n] = simpson_adaptive(-L, L, eps, integrand_an) / L;
        b[n] = simpson_adaptive(-L, L, eps, integrand_bn) / L;
        
        // Прогресс в консоли
        if (k % 10 == 0) cout << "Вычислено гармоник: " << k << "/" << N << "\n";
    }

    // Вывод коэффициентов
    cout << "\nКоэффициенты ряда:\n";
    cout << fixed << setprecision(7);
    cout << "a0 = " << a[0] << "\n";
    for (int i = 1; i <= N; ++i) {
        cout << "a" << i << " = " << setw(12) << a[i] 
             << " | b" << i << " = " << setw(12) << b[i] << "\n";
    }

    // --- Экспорт данных для графика ---
    ofstream out("fourier_data.csv");
    out << fixed << setprecision(6);
    out << "# x\tf(x)\tS_N(x)\n";

    int plot_pts = 1000;
    for (int i = 0; i <= plot_pts; ++i) {
        double x = -L + 2 * L * i / plot_pts;
        
        // Частичная сумма ряда
        double S = a[0] / 2.0;
        for (int n = 1; n <= N; ++n) {
            S += a[n] * cos(n * x) + b[n] * sin(n * x);
        }
        out << x << "\t" << target_f(x) << "\t" << S << "\n";
    }
    out.close();

    cout << "\n✅ Успешно! Данные сохранены в fourier_data.csv\n";
    cout << "📊 Для построения графика выполните: python plot_fourier.py\n";

    return 0;
}