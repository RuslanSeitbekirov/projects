<<<<<<< HEAD
a = [35000, 54000,60000, 100000]
b = [437000, 546000, 600000, ]

n = [0.11, 0.1, 0.09, 0.008]

print(100000/(1+0.11)*(1+0.1)*(1+0.09)*(1+0.08)+60000/(1+0.1)*(1+0.09)*(1+0.08)+35000/(1+0.02))
=======
import matplotlib.pyplot as plt
import numpy as np
import math as m

# ============================================================
# 1. АДАПТИВНОЕ ИНТЕГРИРОВАНИЕ
# ============================================================
def simpson_rule(func, a, b):
    h = (b - a) / 2
    return (h / 3) * (func(a) + 4 * func((a + b) / 2) + func(b))

def adaptive_simpson(func, a, b, eps, max_depth=50):
    mid = (a + b) / 2
    whole = simpson_rule(func, a, b)
    left = simpson_rule(func, a, mid)
    right = simpson_rule(func, mid, b)
    
    delta = abs(left + right - whole) / 15
    
    # Улучшение: масштабируем eps по длине отрезка для глобальной точности
    local_eps = eps * (b - a) / (2 * m.pi)
    
    if delta < local_eps or max_depth == 0:
        return left + right + (left + right - whole) / 15
    else:
        return (adaptive_simpson(func, a, mid, eps, max_depth - 1) +
                adaptive_simpson(func, mid, b, eps, max_depth - 1))

# ============================================================
# 2. ВЫЧИСЛЕНИЕ КОЭФФИЦИЕНТОВ (с защитой от осцилляций)
# ============================================================
def compute_fourier_coeffs(func, a, b, N, eps=1e-6):
    T = b - a
    w0 = 2 * m.pi / T
    
    a0 = (2 / T) * adaptive_simpson(func, a, b, eps, max_depth=50)

    an, bn = [], []
    for n in range(1, N + 1):
        safe_depth = max(50, n * 3)
        
        integrand_an = lambda x, n=n: func(x) * m.cos(n * w0 * x)
        integrand_bn = lambda x, n=n: func(x) * m.sin(n * w0 * x)

        a_n = (2 / T) * adaptive_simpson(integrand_an, a, b, eps, max_depth=safe_depth)
        b_n = (2 / T) * adaptive_simpson(integrand_bn, a, b, eps, max_depth=safe_depth)

        an.append(a_n)
        bn.append(b_n)

    return a0, an, bn

# ============================================================
# 3. ВОССТАНОВЛЕНИЕ РЯДА
# ============================================================
def fourier_series_value(x, a0, an, bn, a, b):
    T = b - a
    w0 = 2 * m.pi / T
    res = a0 / 2
    for n, (an_val, bn_val) in enumerate(zip(an, bn), start=1):
        res += an_val * m.cos(n * w0 * x) + bn_val * m.sin(n * w0 * x)
    return res

# ============================================================
# 4. ПРИМЕР
# ============================================================
if __name__ == "__main__":
    def target_func(x):
        return 1.0 / (10.0 + 6.0 * np.sin(x + np.e))

    a, b = -m.pi, m.pi
    N = 40000
    eps = 1e-8

    a0, an, bn = compute_fourier_coeffs(target_func, a, b, N, eps)

    print(f"\na0 = {a0:.8f}")
    print("Коэффициенты an:", [f"{v:.6f}" for v in an])
    print("Коэффициенты bn:", [f"{v:.6f}" for v in bn])

    # Визуализация
    xs = np.linspace(a, b, 1000)
    ys_orig = [target_func(x) for x in xs]
    ys_fourier = [fourier_series_value(x, a0, an, bn, a, b) for x in xs]

    plt.figure(figsize=(9, 5))
    plt.plot(xs, ys_orig, label="Оригинал f(x)", linewidth=2)
    plt.plot(xs, ys_fourier, "--", label=f"Ряд Фурье (N={N})", linewidth=2)
    plt.title("Аппроксимация рядом Фурье\n(устранена ошибка bn для высоких гармоник)")
    plt.legend(); plt.grid(alpha=0.3)
    plt.tight_layout()
    plt.show()
>>>>>>> 6b5e821692a531bff13ec76c8e0f6ea97e2f3e62
