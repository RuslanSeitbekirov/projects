// Метод сопряженных градиентов (Флетчера-Ривса) для решения СЛАУ с симметричной положительно определённой матрицей
#include <iostream>
#include <vector>
#include <cmath>
#include <iomanip>
using namespace std;

// Умножение матрицы на вектор
vector<double> matVec(const vector<vector<double>>& A, const vector<double>& x) {
    int n = A.size();
    vector<double> res(n, 0.0);
    for (int i = 0; i < n; ++i)
        for (int j = 0; j < n; ++j)
            res[i] += A[i][j] * x[j];
    return res;
}

// Скалярное произведение
double dot(const vector<double>& a, const vector<double>& b) {
    double res = 0.0;
    for (size_t i = 0; i < a.size(); ++i)
        res += a[i] * b[i];
    return res;
}

// Разность векторов
vector<double> sub(const vector<double>& a, const vector<double>& b) {
    vector<double> res(a.size());
    for (size_t i = 0; i < a.size(); ++i)
        res[i] = a[i] - b[i];
    return res;
}

// Сумма векторов
vector<double> add(const vector<double>& a, const vector<double>& b) {
    vector<double> res(a.size());
    for (size_t i = 0; i < a.size(); ++i)
        res[i] = a[i] + b[i];
    return res;
}

// Умножение вектора на скаляр
vector<double> mul(double alpha, const vector<double>& v) {
    vector<double> res(v.size());
    for (size_t i = 0; i < v.size(); ++i)
        res[i] = alpha * v[i];
    return res;
}

// Евклидова норма вектора
double norm(const vector<double>& v) {
    return sqrt(dot(v, v));
}

// Метод сопряженных градиентов
vector<double> conjugateGradient(const vector<vector<double>>& A, const vector<double>& b, 
                                 double eps = 1e-10, int maxIter = 1000) {
    int n = A.size();
    vector<double> x(n, 0.0);          // начальное приближение
    vector<double> r = sub(b, matVec(A, x));
    vector<double> p = r;
    double rsold = dot(r, r);

    for (int iter = 0; iter < maxIter; ++iter) {
        vector<double> Ap = matVec(A, p);
        double alpha = rsold / dot(p, Ap);
        x = add(x, mul(alpha, p));
        r = sub(r, mul(alpha, Ap));
        double rsnew = dot(r, r);
        if (sqrt(rsnew) < eps) break;
        double beta = rsnew / rsold;
        p = add(r, mul(beta, p));
        rsold = rsnew;
    }
    return x;
}

int main() {
    // Исходные данные
    vector<vector<double>> A = {
        {5, 1, -1},
        {-1, 3, -1},
        {1, -2, 4}
    };
    vector<double> b = {-5, 5, -1};

    try {
        vector<double> x = conjugateGradient(A, b);
        cout << "Решение методом сопряженных градиентов (Флетчера–Ривса):\n";
        cout << fixed << setprecision(10);
        for (size_t i = 0; i < x.size(); ++i)
            cout << "x" << i + 1 << " = " << x[i] << endl;

        // Проверка невязки
        vector<double> resid = sub(matVec(A, x), b);
        cout << "Норма невязки: " << norm(resid) << endl;
    } catch (const exception& e) {
        cerr << "Ошибка: " << e.what() << endl;
    }
    return 0;
}