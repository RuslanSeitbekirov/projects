// Метод регуляризации Тихонова для решения плохо обусловленных СЛАУ
#include <iostream>
#include <vector>
#include <cmath>
#include <iomanip>
using namespace std;

// Умножение матриц
vector<vector<double>> matMul(const vector<vector<double>>& A, const vector<vector<double>>& B) {
    int n = A.size();
    int m = B[0].size();
    int p = B.size(); // = A[0].size()
    vector<vector<double>> C(n, vector<double>(m, 0.0));
    for (int i = 0; i < n; ++i)
        for (int k = 0; k < p; ++k)
            if (A[i][k] != 0.0)
                for (int j = 0; j < m; ++j)
                    C[i][j] += A[i][k] * B[k][j];
    return C;
}

// Транспонирование матрицы
vector<vector<double>> transpose(const vector<vector<double>>& A) {
    int n = A.size();
    int m = A[0].size();
    vector<vector<double>> At(m, vector<double>(n));
    for (int i = 0; i < n; ++i)
        for (int j = 0; j < m; ++j)
            At[j][i] = A[i][j];
    return At;
}

// Умножение матрицы на вектор
vector<double> matVec(const vector<vector<double>>& A, const vector<double>& x) {
    int n = A.size();
    vector<double> res(n, 0.0);
    for (int i = 0; i < n; ++i)
        for (int j = 0; j < n; ++j)
            res[i] += A[i][j] * x[j];
    return res;
}

// Сложение матриц
vector<vector<double>> matAdd(const vector<vector<double>>& A, const vector<vector<double>>& B) {
    int n = A.size();
    int m = A[0].size();
    vector<vector<double>> C(n, vector<double>(m));
    for (int i = 0; i < n; ++i)
        for (int j = 0; j < m; ++j)
            C[i][j] = A[i][j] + B[i][j];
    return C;
}

// Умножение матрицы на скаляр
vector<vector<double>> matScalar(double alpha, const vector<vector<double>>& A) {
    int n = A.size();
    int m = A[0].size();
    vector<vector<double>> C(n, vector<double>(m));
    for (int i = 0; i < n; ++i)
        for (int j = 0; j < m; ++j)
            C[i][j] = alpha * A[i][j];
    return C;
}

// Решение СЛАУ методом Гаусса (для квадратной матрицы)
vector<double> gauss(vector<vector<double>> A, vector<double> b) {
    int n = A.size();
    // Расширенная матрица
    vector<vector<double>> aug(n, vector<double>(n + 1));
    for (int i = 0; i < n; ++i) {
        for (int j = 0; j < n; ++j) aug[i][j] = A[i][j];
        aug[i][n] = b[i];
    }

    for (int col = 0; col < n; ++col) {
        // Поиск главного элемента
        int maxRow = col;
        double maxVal = fabs(aug[col][col]);
        for (int row = col + 1; row < n; ++row)
            if (fabs(aug[row][col]) > maxVal) {
                maxVal = fabs(aug[row][col]);
                maxRow = row;
            }
        if (maxVal < 1e-12)
            throw runtime_error("Матрица вырождена");
        swap(aug[col], aug[maxRow]);

        // Нормализация строки
        double pivot = aug[col][col];
        for (int j = col; j <= n; ++j)
            aug[col][j] /= pivot;

        // Обнуление остальных строк
        for (int row = 0; row < n; ++row) {
            if (row != col) {
                double factor = aug[row][col];
                for (int j = col; j <= n; ++j)
                    aug[row][j] -= factor * aug[col][j];
            }
        }
    }

    vector<double> x(n);
    for (int i = 0; i < n; ++i) x[i] = aug[i][n];
    return x;
}

// Метод регуляризации Тихонова
vector<double> tikhonov(const vector<vector<double>>& A, const vector<double>& b, double alpha) {
    int n = A.size();
    // A^T
    auto At = transpose(A);
    // A^T * A
    auto AtA = matMul(At, A);
    // A^T * b
    auto Atb = matVec(At, b);
    // Единичная матрица
    vector<vector<double>> I(n, vector<double>(n, 0.0));
    for (int i = 0; i < n; ++i) I[i][i] = 1.0;
    // M = A^T A + α I
    auto M = matAdd(AtA, matScalar(alpha, I));
    // Решение системы M x = A^T b
    return gauss(M, Atb);
}

int main() {
    // Исходная система (может быть плохо обусловлена)
    vector<vector<double>> A = {
        {3, 1, -1},
        {-2, 4, 1},
        {1, 1, 3}
    };
    vector<double> b = {-1, 5, -3};
    double alpha = 0.01;   // параметр регуляризации

    try {
        auto x = tikhonov(A, b, alpha);
        cout << "Решение методом регуляризации Тихонова (α = " << alpha << "):\n";
        cout << fixed << setprecision(10);
        for (size_t i = 0; i < x.size(); ++i)
            cout << "x" << i + 1 << " = " << x[i] << endl;

        // Проверка невязки исходной системы
        auto resid = matVec(A, x);
        for (size_t i = 0; i < resid.size(); ++i) resid[i] -= b[i];
        double normResid = 0.0;
        for (double v : resid) normResid += v * v;
        normResid = sqrt(normResid);
        cout << "Норма невязки исходной системы: " << normResid << endl;
    } catch (const exception& e) {
        cerr << "Ошибка: " << e.what() << endl;
    }
    return 0;
}