/*
 * ============================================================
 *  Быстрое преобразование Фурье (FFT) + классический ряд Фурье
 *  Метод Кули-Тьюки (Cooley-Tukey), основание 2, рекурсия
 * ============================================================
 *  Структура файла:
 *   1. Функции-примеры        — легко добавить свою
 *   2. FFT (Кули-Тьюки)       — fft()
 *   3. FFT для произвольной f — computeFFT()
 *   4. Обратное FFT           — ifft()
 *   5. Классический ряд Фурье — simpson / computeFourier / fourierSum
 *   6. ЭКСПОРТ В CSV          — exportCSV()
 *   7. ASCII-график           — plotASCII()
 *   8. main()
 * ============================================================
 */

#include <iostream>
#include <cmath>
#include <complex>
#include <vector>
#include <functional>
#include <iomanip>
#include <sstream>
#include <string>
#include <algorithm>
#include <fstream>

using cd = std::complex<double>;
const double PI = std::acos(-1.0);

// ============================================================
//  1. ФУНКЦИИ-ПРИМЕРЫ
//     Чтобы добавить свою — напишите такую же функцию
//     и добавьте её в таблицу FUNCTIONS в main().
// ============================================================
double funcSquareWave(double x) { return (x >= 0.0) ?  1.0 : -1.0; }
double funcSawtooth  (double x) { return x / PI; }
double funcParabola  (double x) { return (x * x) / (PI * PI); }
double funcAbsX      (double x) { return std::abs(x) / PI; }
double funcTriangle  (double x) { return (2.0/PI) * std::asin(std::sin(x)); }
double funcSinc      (double x) { return (x == 0.0) ? 1.0 : std::sin(x) / x; }
double MyFunc        (double x) { return (x == 0.0) ? 1.0 : std::sin(x) / x; }

// ============================================================
//  2. FFT — алгоритм Кули-Тьюки (рекурсивный, основание 2)
//
//  Вход:  a — вектор комплексных чисел, длина — степень 2
//  invert = false → прямое ДПФ
//  invert = true  → обратное ДПФ (ОДПФ)
// ============================================================
void fft(std::vector<cd>& a, bool invert) {
    int n = static_cast<int>(a.size());
    if (n == 1) return;

    // Разбиение на чётные и нечётные (butterfly)
    std::vector<cd> a0(n / 2), a1(n / 2);
    for (int i = 0; i < n / 2; i++) {
        a0[i] = a[2 * i];
        a1[i] = a[2 * i + 1];
    }
    fft(a0, invert);
    fft(a1, invert);

    // Поворачивающий множитель W = e^{±2πi/n}
    double angle = 2.0 * PI / n * (invert ? -1.0 : 1.0);
    cd w(1.0, 0.0);
    cd wn(std::cos(angle), std::sin(angle));
    for (int i = 0; i < n / 2; i++) {
        cd t = w * a1[i];
        a[i]         = a0[i] + t;
        a[i + n / 2] = a0[i] - t;
        if (invert) {
            a[i]         /= 2.0;
            a[i + n / 2] /= 2.0;
        }
        w *= wn;
    }
}

// ============================================================
//  3. computeFFT — выборка произвольной функции и FFT
//
//  f    — любая функция double(double)  ← ВЫНЕСЕНА ОТДЕЛЬНО
//  a, b — отрезок дискретизации
//  N    — число точек (округляется до степени 2)
//
//  Возвращает: вектор комплексных спектральных коэффициентов
// ============================================================
std::vector<cd> computeFFT(std::function<double(double)> f,
                            double a, double b, int N) {
    int M = 1;
    while (M < N) M <<= 1;

    std::vector<cd> signal(M);
    double step = (b - a) / M;
    for (int i = 0; i < M; i++)
        signal[i] = cd(f(a + i * step), 0.0);

    fft(signal, false);
    return signal;
}

// ============================================================
//  4. ifft — обратное преобразование Фурье через fft()
// ============================================================
std::vector<cd> ifft(std::vector<cd> spectrum) {
    fft(spectrum, true);
    return spectrum;
}

// ============================================================
//  5. Классический ряд Фурье (метод Симпсона)
// ============================================================
double simpson(std::function<double(double)> f, double a, double b, int n = 1000) {
    if (n % 2 != 0) n++;
    double h = (b - a) / n;
    double s = f(a) + f(b);
    for (int i = 1; i < n; i++)
        s += (i % 2 == 0 ? 2.0 : 4.0) * f(a + i * h);
    return s * h / 3.0;
}

struct FourierCoeffs { double a0; std::vector<double> an, bn; };

FourierCoeffs computeFourier(std::function<double(double)> f, double L, int N, int sN = 2000) {
    FourierCoeffs c;
    c.a0 = (1.0 / L) * simpson(f, -L, L, sN);
    for (int n = 1; n <= N; n++) {
        c.an.push_back((1.0/L) * simpson(
            [&](double x){ return f(x)*std::cos(n*PI*x/L); }, -L, L, sN));
        c.bn.push_back((1.0/L) * simpson(
            [&](double x){ return f(x)*std::sin(n*PI*x/L); }, -L, L, sN));
    }
    return c;
}

double fourierSum(const FourierCoeffs& c, double L, double x) {
    double s = c.a0 / 2.0;
    for (int n = 1; n <= (int)c.an.size(); n++)
        s += c.an[n-1]*std::cos(n*PI*x/L) + c.bn[n-1]*std::sin(n*PI*x/L);
    return s;
}

// ============================================================
//  6. ЭКСПОРТ В CSV
//
//  Файл 1: fourier_fft_signal.csv
//    Колонки: x, f(x), fourier_sum, ifft_restored, error_fourier, error_ifft
//
//  Файл 2: fourier_fft_spectrum.csv
//    Колонки: k, re, im, amplitude (= |X[k]|/N)
//
//  Файл 3: fourier_fft_coeffs.csv
//    Колонки: n, an, bn, amplitude (= sqrt(an²+bn²))
// ============================================================
void exportCSV(std::function<double(double)> f,
               const FourierCoeffs& fc,
               const std::vector<cd>& spec,
               double L, int N,
               const std::string& fnameStr,
               int plotPoints = 500) {

    int M = static_cast<int>(spec.size());
    std::vector<cd> restored = ifft(spec);

    // --- Файл 1: сигнал ---
    std::ofstream sig("csv/fourier_fft_signal.csv");
    sig << "# function=" << fnameStr << " N=" << N << " FFT_points=" << M << "\n";
    sig << "x,f_x,fourier_sum,ifft_restored,error_fourier,error_ifft\n";
    sig << std::fixed << std::setprecision(8);
    for (int i = 0; i < plotPoints; i++) {
        double x   = -L + 2.0 * L * i / (plotPoints - 1);
        double fx  = f(x);
        double sn  = fourierSum(fc, L, x);
        // IFFT: интерполяция в произвольную точку
        double pos = (double)i / (plotPoints - 1) * (M - 1);
        int lo = (int)pos, hi = std::min(lo + 1, M - 1);
        double t   = pos - lo;
        double rv  = restored[lo].real()*(1-t) + restored[hi].real()*t;
        sig << x   << "," << fx << "," << sn << "," << rv
            << "," << std::abs(fx - sn) << "," << std::abs(fx - rv) << "\n";
    }
    sig.close();
    std::cout << "  -> fourier_fft_signal.csv    (" << plotPoints << " точек)\n";

    // --- Файл 2: спектр FFT ---
    std::ofstream sp("csv/fourier_fft_spectrum.csv");
    sp << "# FFT spectrum: function=" << fnameStr << " M=" << M << "\n";
    sp << "k,re,im,amplitude\n";
    sp << std::fixed << std::setprecision(8);
    int Kmax = M / 2;  // только первая половина (симметрия)
    for (int k = 0; k < Kmax; k++) {
        double amp = std::abs(spec[k]) / M;
        sp << k << "," << spec[k].real() << "," << spec[k].imag() << "," << amp << "\n";
    }
    sp.close();
    std::cout << "  -> fourier_fft_spectrum.csv  (" << Kmax << " частот)\n";

    // --- Файл 3: коэффициенты ряда Фурье ---
    std::ofstream cof("csv/fourier_fft_coeffs.csv");
    cof << "# Fourier series coefficients: function=" << fnameStr << " N=" << N << "\n";
    cof << "n,an,bn,amplitude\n";
    cof << std::fixed << std::setprecision(8);
    cof << "0," << fc.a0 << ",0," << std::abs(fc.a0) << "\n";
    for (int n = 1; n <= N; n++) {
        double amp = std::sqrt(fc.an[n-1]*fc.an[n-1] + fc.bn[n-1]*fc.bn[n-1]);
        cof << n << "," << fc.an[n-1] << "," << fc.bn[n-1] << "," << amp << "\n";
    }
    cof.close();
    std::cout << "  -> fourier_fft_coeffs.csv    (" << N+1 << " строк)\n";
}

// ============================================================
//  7. ASCII-график
// ============================================================
void plotASCII(std::function<double(double)> f, const FourierCoeffs& fc, const std::vector<cd>& fftSpec, double L,
               int cols = 72, int rows = 26) {

    int M = static_cast<int>(fftSpec.size());
    double a = -L, b = L;
    std::vector<cd> restored = ifft(fftSpec);
    std::vector<double> xs(cols), fv(cols), sv(cols), rv(cols);
    for (int i = 0; i < cols; i++) {
        xs[i] = a + (b - a) * i / (cols - 1);
        fv[i] = f(xs[i]);
        sv[i] = fourierSum(fc, L, xs[i]);
        double pos = (double)i / (cols - 1) * (M - 1);
        int lo = (int)pos, hi = std::min(lo + 1, M - 1);
        double t = pos - lo;
        rv[i] = restored[lo].real() * (1 - t) + restored[hi].real() * t;
    }
    double ymin = *std::min_element(fv.begin(), fv.end());
    double ymax = *std::max_element(fv.begin(), fv.end());
    for (int i = 0; i < cols; i++) {
        ymin = std::min({ymin, sv[i], rv[i]});
        ymax = std::max({ymax, sv[i], rv[i]});
    }
    double mg = (ymax - ymin) * 0.08;
    if (mg < 1e-9) mg = 0.3;
    ymin -= mg; ymax += mg;

    std::vector<std::string> canvas(rows, std::string(cols, ' '));
    auto toRow = [&](double y) -> int {
        int r = (int)((ymax - y) / (ymax - ymin) * (rows - 1) + 0.5);
        return std::max(0, std::min(rows - 1, r));
    };
    if (ymin <= 0.0 && 0.0 <= ymax) {
        int ax = toRow(0.0);
        for (int i = 0; i < cols; i++) canvas[ax][i] = '-';
    }
    int ayc = (int)((0.0 - a) / (b - a) * (cols - 1) + 0.5);
    ayc = std::max(0, std::min(cols - 1, ayc));
    for (int r = 0; r < rows; r++)
        canvas[r][ayc] = (canvas[r][ayc] == '-') ? '+' : '|';
    for (int i = 0; i < cols; i++) {
        auto put = [&](int row, char ch) {
            char& c = canvas[row][i];
            if (c == ' ' || c == '-' || c == '|' || c == '+') c = ch;
            else if (c != ch) c = '@';
        };
        put(toRow(fv[i]), '*');
        put(toRow(sv[i]), '#');
        put(toRow(rv[i]), '~');
    }
    const int LW = 9;
    std::string hb(cols + 2, '-');
    std::cout << "\n--- График: * f(x)  # ряд Фурье  ~ IFFT(FFT)  @ совпадение ---\n";
    std::cout << std::string(LW, ' ') << "+" << hb << "+\n";
    for (int r = 0; r < rows; r++) {
        double yL = ymax - (ymax - ymin) * r / (rows - 1);
        if (r % 4 == 0)
            std::cout << std::setw(LW-1) << std::fixed << std::setprecision(2) << yL << " ";
        else
            std::cout << std::string(LW, ' ');
        std::cout << "|" << canvas[r] << "|\n";
    }
    std::cout << std::string(LW, ' ') << "+" << hb << "+\n";
    std::cout << std::string(LW + 1, ' ');
    int prevEnd = 0;
    for (int k = 0; k <= 6; k++) {
        double xL = a + (b - a) * k / 6;
        int col   = (int)((double)k / 6 * (cols - 1));
        std::ostringstream oss; oss << std::fixed << std::setprecision(2) << xL;
        std::string lbl = oss.str();
        int sp = col - prevEnd - (int)lbl.size() / 2;
        if (sp < 1) sp = 1;
        std::cout << std::string(sp, ' ') << lbl;
        prevEnd = col + (int)lbl.size() - (int)lbl.size() / 2;
    }
    std::cout << "\n";
}

void printSpectrum(const std::vector<cd>& spec, int K = 12) {
    int M = static_cast<int>(spec.size());
    K = std::min(K, M / 2);
    std::cout << "\n--- Спектр FFT (первые " << K << " частот) ---\n";
    std::cout << std::setw(6) << "k" << std::setw(14) << "Re(X[k])"
              << std::setw(14) << "Im(X[k])" << std::setw(14) << "|X[k]|/N" << "\n";
    std::cout << std::string(48, '-') << "\n";
    for (int k = 0; k < K; k++)
        std::cout << std::setw(6) << k
                  << std::setw(14) << std::fixed << std::setprecision(5) << spec[k].real()
                  << std::setw(14) << spec[k].imag()
                  << std::setw(14) << std::abs(spec[k]) / M << "\n";
}

void printCoeffs(const FourierCoeffs& c, int N) {
    std::cout << "\n--- Коэффициенты ряда Фурье (Симпсон) ---\n";
    std::cout << "a0 = " << std::fixed << std::setprecision(6) << c.a0 << "\n";
    std::cout << std::setw(5) << "n" << std::setw(14) << "an" << std::setw(14) << "bn" << "\n";
    std::cout << std::string(33, '-') << "\n";
    for (int n = 1; n <= N; n++)
        std::cout << std::setw(5) << n << std::setw(14) << c.an[n-1] << std::setw(14) << c.bn[n-1] << "\n";
}

// ============================================================
//  8. MAIN
// ============================================================
int main() {
    std::cout << "=== БПФ (Кули-Тьюки) + Ряд Фурье (Симпсон) ===\n\n";

    struct Entry { const char* name; std::function<double(double)> fn; };
    std::vector<Entry> FUNCTIONS = {
        { "Прямоугольный меандр",  funcSquareWave },
        { "Пилообразная f(x)=x",   funcSawtooth   },
        { "Парабола f(x)=x^2",     funcParabola   },
        { "f(x) = |x|",            funcAbsX       },
        { "Треугольная волна",      funcTriangle   },
        { "Sinc f(x)=sin(x)/x",    funcSinc       },
        { "Моя функция",           MyFunc         },
    };

    std::cout << "Выберите функцию:\n";
    for (int i = 0; i < (int)FUNCTIONS.size(); i++)
        std::cout << "  " << (i+1) << " - " << FUNCTIONS[i].name << "\n";
    std::cout << "Ваш выбор: ";
    int choice; std::cin >> choice;
    if (choice < 1 || choice > (int)FUNCTIONS.size()) choice = 1;

    auto f     = FUNCTIONS[choice-1].fn;
    auto fname = FUNCTIONS[choice-1].name;

    std::cout << "Число гармоник N для ряда Фурье (5-20): ";
    int N; std::cin >> N;
    if (N < 1) N = 1; if (N > 50) N = 50;

    std::cout << "Число точек для FFT (рекомендуется 256-4096): ";
    int Nfft; std::cin >> Nfft;
    if (Nfft < 4) Nfft = 256;

    double L = PI;
    int M = 1; while (M < Nfft) M <<= 1;

    std::cout << "\n[Функция]  " << fname << "\n";
    std::cout << "[Период]   2L = 2*pi\n";
    std::cout << "[Гармоник] N  = " << N << "\n";
    std::cout << "[FFT точек]   = " << M << " (округлено до степени 2)\n";

    FourierCoeffs fc = computeFourier(f, L, N);
    std::vector<cd> spec = computeFFT(f, -L, L, M);

    plotASCII(f, fc, spec, L, 72, 26);
    printSpectrum(spec, 12);
    printCoeffs(fc, N);

    std::vector<cd> restored = ifft(spec);
    double maxErr = 0.0;
    for (int i = 0; i < M; i++) {
        double x = -L + 2.0 * L * i / M;
        maxErr = std::max(maxErr, std::abs(restored[i].real() - f(x)));
    }
    std::cout << "\n[IFFT погрешность] max|f - IFFT(FFT(f))| = "
              << std::scientific << std::setprecision(3) << maxErr << "\n";

    // Экспорт
    std::cout << "\n--- Экспорт данных ---\n";
    exportCSV(f, fc, spec, L, N, fname);
    std::cout << "Запустите plot_fourier_fft.py для построения графика.\n";

    std::cout << "\nГотово!\n";
    return 0;
}