#include <iostream>
#include <vector>
#include <iomanip>
using namespace std;

// Функция для вычисления интерполяционного полинома Лагранжа в точке x
double lagrange(const vector<double>& x, const vector<double>& y, double x0) {
    int n = x.size();
    double result = 0.0;
    for (int i = 0; i < n; ++i) {
        double term = y[i];
        for (int j = 0; j < n; ++j) {
            if (j != i) {
                term *= (x0 - x[j]) / (x[i] - x[j]);
            }
        }
        result += term;
    }
    return result;
}

int main() {
    // Данные из таблицы
    vector<double> x = {1, 2, 3, 4, 5};
    vector<double> y = {7.5, 6.2, 5.5, 3.5, 3.0};

    cout << "Интерполяционный полином Лагранжа для заданных точек:\n";
    cout << "x: 1   2   3   4   5\n";
    cout << "y: 7.5 6.2 5.5 3.5 3.0\n\n";

    // Также можно вычислить во всех точках для проверки
    cout << "\nПроверка в узлах интерполяции:\n";
    for (int i = 0; i < x.size(); ++i) {
        double val = lagrange(x, y, x[i]); //x[i] == x0
        cout << "x = " << x[i] << ", исходное y = " << y[i] << ", полином = " << val << endl;
    }

    return 0;
}
