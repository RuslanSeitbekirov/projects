// #include <iostream>
// #include <vector>
// #include <cmath>
// #include <iomanip>

// using namespace std;

// // Решение СЛАУ методом Гаусса-Жордана с частичным выбором ведущего элемента
// vector<double> gaussJordan(vector<vector<double>> A, vector<double> b) {
//     int n = A.size();
//     // Расширенная матрица [A | b]
//     vector<vector<double>> aug(n, vector<double>(n + 1));
//     for (int i = 0; i < n; ++i) {
//         for (int j = 0; j < n; ++j) {
//             aug[i][j] = A[i][j];
//         }
//         aug[i][n] = b[i];
//     }

//     for (int col = 0; col < n; ++col) {
//         // Поиск строки с максимальным по модулю элементом в текущем столбце
//         int maxRow = col;
//         double maxVal = fabs(aug[col][col]);
//         for (int row = col + 1; row < n; ++row) {
//             if (fabs(aug[row][col]) > maxVal) {
//                 maxVal = fabs(aug[row][col]);
//                 maxRow = row;
//             }
//         }
//         // Проверка на вырожденность
//         if (maxVal < 1e-12) {
//             throw runtime_error("Матрица вырождена");
//         }
//         // Перестановка строк
//         swap(aug[col], aug[maxRow]);

//         // Нормализация текущей строки (деление на ведущий элемент)
//         double pivot = aug[col][col];
//         for (int j = col; j <= n; ++j) {
//             aug[col][j] /= pivot;
//         }

//         // Обнуление всех остальных строк в текущем столбце
//         for (int row = 0; row < n; ++row) {
//             if (row != col) {
//                 double factor = aug[row][col];
//                 for (int j = col; j <= n; ++j) {
//                     aug[row][j] -= factor * aug[col][j];
//                 }
//             }
//         }
//     }

//     // Извлечение решения из последнего столбца
//     vector<double> x(n);
//     for (int i = 0; i < n; ++i) {
//         x[i] = aug[i][n];
//     }
//     return x;
// }

// int main() {
//     // Исходные данные
//     vector<vector<double>> A = {
//         {3, 1, -1},
//         {-2, 4, 1},
//         {1, 1, 3}
//     };
//     vector<double> b = {-1, 5, -3};

//     try {
//         vector<double> x = gaussJordan(A, b);
//         cout << "Решение системы:" << endl;
//         cout << fixed << setprecision(6);
//         for (size_t i = 0; i < x.size(); ++i) {
//             cout << "x" << i + 1 << " = " << x[i] << endl;
//         }
//     } catch (const exception& e) {
//         cerr << "Ошибка: " << e.what() << endl;
//     }

//     return 0;
// }



#include <iostream>
#include <vector>
#include <cmath>
#include <iomanip>

using namespace std;

// Вспомогательная функция для вывода расширенной матрицы
void printMatrix(const vector<vector<double>>& mat, const string& caption) {
    cout << caption << ":\n";
    int n = mat.size();
    int m = mat[0].size(); // количество столбцов (n+1)
    for (int i = 0; i < n; ++i) {
        for (int j = 0; j < m; ++j) {
            cout << setw(12) << fixed << setprecision(6) << mat[i][j];
        }
        cout << endl;
    }
    cout << endl;
}

// Решение СЛАУ методом Гаусса-Жордана с частичным выбором ведущего элемента
vector<double> gaussJordan(vector<vector<double>> A, vector<double> b) {
    int n = A.size();
    // Расширенная матрица [A | b]
    vector<vector<double>> aug(n, vector<double>(n + 1));
    for (int i = 0; i < n; ++i) {
        for (int j = 0; j < n; ++j) {
            aug[i][j] = A[i][j];
        }
        aug[i][n] = b[i];
    }

    // Вывод исходной матрицы
    printMatrix(aug, "Исходная расширенная матрица");

    for (int col = 0; col < n; ++col) {
        // Поиск строки с максимальным по модулю элементом в текущем столбце
        int maxRow = col;
        double maxVal = fabs(aug[col][col]);
        for (int row = col + 1; row < n; ++row) {
            if (fabs(aug[row][col]) > maxVal) {
                maxVal = fabs(aug[row][col]);
                maxRow = row;
            }
        }
        // Проверка на вырожденность
        if (maxVal < 1e-12) {
            throw runtime_error("Матрица вырождена");
        }
        // Перестановка строк (если нужно)
        if (maxRow != col) {
            swap(aug[col], aug[maxRow]);
            cout << "Перестановка строк " << col+1 << " и " << maxRow+1 << endl;
        }

        // Нормализация текущей строки (деление на ведущий элемент)
        double pivot = aug[col][col];
        for (int j = col; j <= n; ++j) {
            aug[col][j] /= pivot;
        }

        // Обнуление всех остальных строк в текущем столбце
        for (int row = 0; row < n; ++row) {
            if (row != col) {
                double factor = aug[row][col];
                for (int j = col; j <= n; ++j) {
                    aug[row][j] = aug[row][j] - factor * aug[col][j];
                }
            }
        }

        // Вывод матрицы после обработки текущего столбца
        printMatrix(aug, "После шага " + to_string(col+1));
    }

    // Извлечение решения из последнего столбца
    vector<double> x(n);
    for (int i = 0; i < n; ++i) {
        x[i] = aug[i][n];
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
    vector<double> b = {-5, 5, 1};
 
    try {
        vector<double> x = gaussJordan(A, b);
        cout << "Решение системы:" << endl;
        cout << fixed << setprecision(6);
        for (size_t i = 0; i < x.size(); ++i) {
            cout << "x" << i + 1 << " = " << x[i] << endl;
        }
    } catch (const exception& e) {
        cerr << "Ошибка: " << e.what() << endl;
    }

    return 0;
}