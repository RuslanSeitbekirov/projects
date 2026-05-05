#include <iostream>
#include <cmath>
#include <vector>
#include <functional>
#include <iomanip>
#include <string>
#include <algorithm>
#include <fstream>
#include <sstream>
using namespace std;

// ============================================================
//  Метод Симпсона для численного интегрирования
// ============================================================
double simpson(std::function<double(double)> f, double a, double b, int n = 1000) {
    if (n % 2 != 0) n++;
    double h = (b - a) / n;
    double result = f(a) + f(b);
    for (int i = 1; i < n; i++) {
        double x = a + i * h;
        result += (i % 2 == 0 ? 2.0 : 4.0) * f(x);
    }
    return result * h / 3.0;
}

// ============================================================
//  Коэффициенты Фурье
// ============================================================
struct FourierCoeffs {
    double a0;
    std::vector<double> an;
    std::vector<double> bn; 
};

FourierCoeffs computeFourier(std::function<double(double)> f,
                              double L, int N, int simpsonN = 2000) {
    FourierCoeffs c;
    c.a0 = (1.0 / L) * simpson(f, -L, L, simpsonN);
    for (int n = 1; n <= N; n++) {
        c.an.push_back((1.0 / L) * simpson(
            [&](double x){ return f(x) * std::cos(n * M_PI * x / L); }, -L, L, simpsonN));
        c.bn.push_back((1.0 / L) * simpson(
            [&](double x){ return f(x) * std::sin(n * M_PI * x / L); }, -L, L, simpsonN));
    }
    return c;
}

double fourierSum(const FourierCoeffs& c, double L, double x) {
    double s = c.a0 / 2.0;
    int N = static_cast<int>(c.an.size());
    for (int n = 1; n <= N; n++)
        s += c.an[n-1] * std::cos(n * M_PI * x / L)
           + c.bn[n-1] * std::sin(n * M_PI * x / L);
    return s;
}

// ============================================================
//  ЭКСПОРТ В CSV
//
//  Файл 1: fourier_series_signal.csv
//    Колонки: x, f(x), S_N(x), error
//
//  Файл 2: fourier_series_coeffs.csv
//    Колонки: n, an, bn, amplitude (= sqrt(an²+bn²))
// ============================================================
void exportCSV(std::function<double(double)> f,
               const FourierCoeffs& c,
               double L, int N,
               const std::string& fnameStr,
               int plotPoints = 500) {

    // --- Файл 1: сигнал ---
    std::ofstream sig("csv/fourier_series_signal.csv");
    sig << "# function=" << fnameStr << " N=" << N << " L=" << L << "\n";
    sig << "x,f_x,S_N_x,error\n";
    sig << std::fixed << std::setprecision(8);
    for (int i = 0; i < plotPoints; i++) {
        double x  = -L + 2.0 * L * i / (plotPoints - 1);
        double fx = f(x);
        double sn = fourierSum(c, L, x);
        sig << x << "," << fx << "," << sn << "," << std::abs(fx - sn) << "\n";
    }
    sig.close();
    std::cout << "  -> fourier_series_signal.csv  (" << plotPoints << " точек)\n";

    // --- Файл 2: коэффициенты ---
    std::ofstream cof("csv/fourier_series_coeffs.csv");
    cof << "# Fourier coefficients: function=" << fnameStr << " N=" << N << "\n";
    cof << "n,an,bn,amplitude\n";
    cof << std::fixed << std::setprecision(8);
    // n=0: только a0
    cof << "0," << c.a0 << ",0," << std::abs(c.a0) << "\n";
    for (int n = 1; n <= N; n++) {
        double amp = std::sqrt(c.an[n-1]*c.an[n-1] + c.bn[n-1]*c.bn[n-1]);
        cof << n << "," << c.an[n-1] << "," << c.bn[n-1] << "," << amp << "\n";
    }
    cof.close();
    std::cout << "  -> fourier_series_coeffs.csv  (" << N+1 << " строк)\n";
}

// ============================================================
//  ASCII-график
// ============================================================
void plotASCII(std::function<double(double)> f,
               const FourierCoeffs& c, double L,
               int cols = 72, int rows = 24) {

    std::vector<double> xs(cols), fv(cols), sv(cols);
    for (int i = 0; i < cols; i++) {
        xs[i] = -L + 2.0 * L * i / (cols - 1);
        fv[i] = f(xs[i]);
        sv[i] = fourierSum(c, L, xs[i]);
    }
    double ymin = *std::min_element(fv.begin(), fv.end());
    double ymax = *std::max_element(fv.begin(), fv.end());
    for (int i = 0; i < cols; i++) {
        ymin = std::min(ymin, sv[i]);
        ymax = std::max(ymax, sv[i]);
    }
    double margin = (ymax - ymin) * 0.08;
    if (margin < 1e-9) margin = 0.5;
    ymin -= margin; ymax += margin;

    std::vector<std::string> canvas(rows, std::string(cols, ' '));
    auto toRow = [&](double y) -> int {
        int r = static_cast<int>((ymax - y) / (ymax - ymin) * (rows - 1) + 0.5);
        return std::max(0, std::min(rows - 1, r));
    };
    if (ymin <= 0.0 && 0.0 <= ymax) {
        int axisRow = toRow(0.0);
        for (int i = 0; i < cols; i++) canvas[axisRow][i] = '-';
    }
    int axisCol = static_cast<int>((0.0 - (-L)) / (2.0 * L) * (cols - 1) + 0.5);
    axisCol = std::max(0, std::min(cols - 1, axisCol));
    for (int r = 0; r < rows; r++)
        canvas[r][axisCol] = (canvas[r][axisCol] == '-') ? '+' : '|';

    for (int i = 0; i < cols; i++) {
        auto plot = [&](int row, char ch) {
            char cur = canvas[row][i];
            if (cur == ' ' || cur == '-' || cur == '|' || cur == '+') canvas[row][i] = ch;
            else if ((cur == '*' && ch == '#') || (cur == '#' && ch == '*')) canvas[row][i] = '@';
        };
        plot(toRow(fv[i]), '*');
        plot(toRow(sv[i]), '#');
    }

    const int labelW = 9;
    std::string hBorder(cols + 2, '-');
    std::cout << "\n--- График: * = f(x)  # = S_N(x)  @ = совпадение ---\n";
    std::cout << std::string(labelW, ' ') << "+" << hBorder << "+\n";
    for (int r = 0; r < rows; r++) {
        double yLabel = ymax - (ymax - ymin) * r / (rows - 1);
        if (r % 4 == 0)
            std::cout << std::setw(labelW - 1) << std::fixed << std::setprecision(2) << yLabel << " ";
        else
            std::cout << std::string(labelW, ' ');
        std::cout << "|" << canvas[r] << "|\n";
    }
    std::cout << std::string(labelW, ' ') << "+" << hBorder << "+\n";
    std::cout << std::string(labelW + 1, ' ');
    int prevEnd = 0;
    for (int k = 0; k <= 6; k++) {
        double xLabel = -L + 2.0 * L * k / 6;
        int col = static_cast<int>((double)k / 6 * (cols - 1));
        std::ostringstream oss; oss << std::fixed << std::setprecision(2) << xLabel;
        std::string lbl = oss.str();
        int spaces = col - prevEnd - (int)lbl.size() / 2;
        if (spaces < 1) spaces = 1;
        std::cout << std::string(spaces, ' ') << lbl;
        prevEnd = col + (int)lbl.size() - (int)lbl.size() / 2;
    }
    std::cout << "\n";
}

// ============================================================
//  Таблица коэффициентов
// ============================================================
void printCoeffs(const FourierCoeffs& c, int N) {
    std::cout << std::fixed << std::setprecision(6);
    std::cout << "\n--- Коэффициенты Фурье ---\n";
    std::cout << "a0 = " << c.a0 << "\n";
    std::cout << std::setw(5) << "n" << std::setw(14) << "an" << std::setw(14) << "bn" << "\n";
    std::cout << std::string(33, '-') << "\n";
    for (int n = 1; n <= N; n++)
        std::cout << std::setw(5) << n << std::setw(14) << c.an[n-1] << std::setw(14) << c.bn[n-1] << "\n";
}

// ============================================================
//  Таблица сравнения
// ============================================================
void printComparison(std::function<double(double)> f,
                     const FourierCoeffs& c, double L, int points = 10) {
    std::cout << "\n--- Сравнение f(x) и S_N(x) ---\n";
    std::cout << std::setw(10) << "x" << std::setw(14) << "f(x)"
              << std::setw(14) << "S_N(x)" << std::setw(14) << "|погрешность|" << "\n";
    std::cout << std::string(52, '-') << "\n";
    for (int i = 0; i <= points; i++) {
        double x  = -L + 2.0 * L * i / points;
        double fx = f(x);
        double sn = fourierSum(c, L, x);
        std::cout << std::setw(10) << x << std::setw(14) << fx
                  << std::setw(14) << sn << std::setw(14) << std::abs(fx - sn) << "\n";
    }
}

// ============================================================
//  Функции-примеры
// ============================================================
double funcSquareWave(double x) { return (x >= 0.0) ?  1.0 : -1.0; }
double funcSawtooth  (double x) { return (sin(x)*sin(x))/(13-12*cos(x)); }
double funcParabola  (double x) { return x * x; }
double funcAbsX      (double x) { return std::abs(x); }

// ============================================================
//  MAIN
// ============================================================
int main() {
    std::cout << "=== Разложение в ряд Фурье (метод Симпсона) ===\n\n";
    std::cout << "Выберите функцию:\n";
    std::cout << "  1 - Прямоугольный меандр\n";
    std::cout << "  2 - sin²(x)/(13-12cos(x))\n";
    std::cout << "  3 - Парабола (f = x^2)\n";
    std::cout << "  4 - f(x) = |x|\n";
    std::cout << "Ваш выбор: ";
    int choice; std::cin >> choice;

    std::function<double(double)> f;
    std::string fname;
    switch (choice) {
        case 1: f = funcSquareWave; fname = "Прямоугольный меандр"; break;
        case 2: f = funcSawtooth;   fname = "sin2(x)/(13-12cos(x))"; break;
        case 3: f = funcParabola;   fname = "Парабола x^2"; break;
        case 4: f = funcAbsX;       fname = "|x|"; break;
        default: f = funcSawtooth;  fname = "sin2(x)/(13-12cos(x))";
    }

    std::cout << "Число гармоник N (рекомендуется 5-15): ";
    int N; std::cin >> N;
    if (N < 1) N = 1;
    if (N > 50) N = 50;

    double L = M_PI;
    std::cout << "\nФункция  : " << fname << "\n";
    std::cout << "Гармоник : N = " << N   << "\n";
    std::cout << "Период   : 2L = 2*pi\n";

    FourierCoeffs coeffs = computeFourier(f, L, N);

    plotASCII(f, coeffs, L, 72, 24);
    printCoeffs(coeffs, N);
    printComparison(f, coeffs, L);

    // Экспорт
    std::cout << "\n--- Экспорт данных ---\n";
    exportCSV(f, coeffs, L, N, fname);
    std::cout << "Запустите plot_fourier_series.py для построения графика.\n";

    std::cout << "\nГотово!\n";
    return 0;
}