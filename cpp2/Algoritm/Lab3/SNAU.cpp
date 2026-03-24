#include <iostream>
#include <vector>
#include <cmath>
#include <iomanip>

using namespace std;

// Решение линейной системы 2x2 методом Гаусса; А - Якобиан
vector<double> solve_linear_2x2(const vector<vector<double>>& A, const vector<double>& b) {
    double a11 = A[0][0], a12 = A[0][1];
    double a21 = A[1][0], a22 = A[1][1];
    double det = a11 * a22 - a12 * a21; // определитель
    if (fabs(det) < 1e-12) throw runtime_error("Якобиан вырожден");
    double x = (b[0] * a22 - a12 * b[1]) / det;
    double y = (a11 * b[1] - b[0] * a21) / det;
    return {x, y};
}

// Вектор-функция F(x,y)
vector<double> F(const vector<double>& x) {
    double X = x[0], Y = x[1];
    return {
        cos(X) + Y - 1.5,
        2.0 * X - sin(Y - 0.5) - 1.0
    };
}

// Якобиан
vector<vector<double>> J(const vector<double>& x) {
    double X = x[0], Y = x[1];
    return {
        {-sin(X), 1.0},
        {2.0, -cos(Y - 0.5)}
    };
}

// Метод Ньютона-Рафсона. Ищем такое Δ(попрака/изменение/delta) чтобы F(x+Δ) стало нулём: F(x)+J(x)⋅Δ=0. => J(x)⋅Δ=−F(x).
vector<double> newton_raphson(const vector<double>& x0, double eps = 1e-10, int max_it = 100) {
    vector<double> x = x0; // Текущие приближения
    int n = x.size(); // Размерность
    for (int iter = 0; iter < max_it; ++iter) {
        vector<double> Fx = F(x); // хранит два числа
        double maxF = 0.0;
        for (double v : Fx) maxF = max(maxF, fabs(v));
        cout << "Итерация " << iter << ": x = (" << x[0] << ", " << x[1] 
             << "), F = (" << Fx[0] << ", " << Fx[1] << "), max|F| = " << maxF << endl;
        if (maxF < eps) {
            cout << "Сошлось за " << iter << " итераций." << endl;
            return x;
        }

        vector<vector<double>> Jx = J(x);
        vector<double> minusF = { -Fx[0], -Fx[1] };
        vector<double> delta = solve_linear_2x2(Jx, minusF);

        for (int i = 0; i < n; ++i) x[i] += delta[i];
    }
    throw runtime_error("Не сошлось за максимальное число итераций");
}

int main() {
    try {
        // Начальное приближение (можно подобрать, например, (0.5, 0.5) или (1, 1))
        vector<double> x0 = {0.5, 0.5};
        vector<double> sol = newton_raphson(x0);

        cout << fixed << setprecision(12);
        cout << "\nРешение системы:\n";
        cout << "x = " << sol[0] << "\n";
        cout << "y = " << sol[1] << "\n";

        // Проверка
        vector<double> Fsol = F(sol);
        cout << "Невязка F = (" << Fsol[0] << ", " << Fsol[1] << ")\n";
    } catch (const exception& e) {
        cerr << "Ошибка: " << e.what() << endl;
    }
    return 0;
}