#include <iostream>
#include <vector>
#include <cmath>
#include <iomanip>

using namespace std; 

// Метод Зейделя для решения СЛАУ Ax = b
vector<double> seidel(const vector<vector<double>>& A, const vector<double>& b, double eps = 1e-4, int maxIter = 200000) {
    int n = A.size();
    vector<double> x(n, 0.0); // начальное приближение
    vector<double> x_new(n, 0.0);

    for (int iter = 0; iter < maxIter; ++iter) {
        // Одновременно обновляем компоненты, используя уже новые значения
        for (int i = 0; i < n; ++i) {
            double sum = b[i];
            for (int j = 0; j < n; ++j) {
                if (j != i) {
                    sum -= A[i][j] * x_new[j]; // используем уже обновлённые x[j] для j < i
                }
            }
            x_new[i] = sum / A[i][i];
        }

        // Проверка сходимости
        double error = 0.0;
        cout << iter << "df" << error << endl;
        for (int i = 0; i < n; ++i) {
            error += fabs(x_new[i] - x[i]);
        }
        if (error < eps) {
            return x_new;
        }
        x = x_new;
    }
    throw runtime_error("Не сошлось за максимальное число итераций");
}

int main() {
    // Исходные данные
    vector<vector<double>> A = {
        {5, 1, -1},
        {-1, 3, -1},
        {1, -2, 4}
    };
    vector<double> b = {-5, 5, 1};

    try {
        vector<double> x = seidel(A, b);
        cout << "Решение системы методом Зейделя:" << endl;
        cout << fixed << setprecision(10);
        for (size_t i = 0; i < x.size(); ++i) {
            cout << "x" << i + 1 << " = " << x[i] << endl;
        }
    } catch (const exception& e) {
        cerr << "Ошибка: " << e.what() << endl;
    }

    return 0;
}