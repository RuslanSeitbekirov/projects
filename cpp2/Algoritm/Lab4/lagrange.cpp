#include <iostream>
#include <vector>
#include <iomanip>
using namespace std;

class LagrangePolynomial {
private:
    vector<double> x_points;
    vector<double> y_points;
    int n;

public:
    // Конструктор: сохраняем точки интерполяции
    LagrangePolynomial(const vector<double>& x, const vector<double>& y): x_points(x), y_points(y), n(x.size()) {}

    // Оператор () — позволяет использовать объект как функцию: L(x)
    double operator()(double x) const {
        double result = 0.0;
        
        for (int i = 0; i < n; ++i) {
            // Вычисляем базисный полином l_i(x)
            double basis = 1.0;
            for (int j = 0; j < n; ++j) {
                if (i != j) {
                    basis *= (x - x_points[j]) / (x_points[i] - x_points[j]);
                }
            }
            result += y_points[i] * basis;
        }
        return result;
    }
};

int main() {
    vector<double> x = {1, 2, 3, 4, 5};
    vector<double> y = {7.5, 6.2, 5.5, 3.5, 3.0};
    
    // Создаем «функцию» полинома (аналог L = lagrange_polynomial_vectorized(...) в Python)
    LagrangePolynomial L(x, y);
    
    // Настройка вывода как в Python: фиксированный формат, 4 знака после запятой
    cout << fixed << setprecision(4);
    
    // Вычисляем значения
    cout << "L(x) = " << L(4.9) << endl;
    return 0;
}