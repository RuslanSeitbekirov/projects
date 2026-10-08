import numpy as np
import sympy as sp
from sympy.stats import FDistribution, cdf
import matplotlib
matplotlib.use("Agg")
import matplotlib.pyplot as plt 

# ---------- ЭТАП 0. Данные и уровень значимости ----------
y = np.array([1.7, -5.4, -4.0, -5.9, -1.6, 0.0, 0.6, 2.1, 0.1, -4.9,
              -3.5, 5.9, 8.5, 9.9, 13.3, 11.1, 14.4, 16.2])
x = np.arange(1, len(y) + 1)
n = len(y)
alpha = 0.05
print("=== ЭТАП 0. Данные ===")
print("y =", y.tolist()); print("n =", n, "| alpha =", alpha)

# ---------- ЭТАП 1. Модель и МНК в символьном виде (sympy) ----------
# Данные содержат отрицательные значения, поэтому берём y = c + a*exp(b*x).
# При фиксированном b модель линейна по (c, a): решаем нормальные уравнения символьно.
print("\n=== ЭТАП 1. Модель и нормальные уравнения ===")
c, a, b, xs = sp.symbols("c a b x")
print("Модель: y(x) =", c + a * sp.exp(b * xs))
n_s, Su, Suu, Sy, Suy = sp.symbols("n Su Suu Sy Suy")   # суммы по u=exp(b*x)
sol = sp.solve([sp.Eq(n_s*c + Su*a, Sy), sp.Eq(Su*c + Suu*a, Suy)], [c, a])
print("Нормальные уравнения: n*c + Su*a = Sy;  Su*c + Suu*a = Suy")
print("Решение: c =", sol[c], ";  a =", sol[a])
f_ca = sp.lambdify((n_s, Su, Suu, Sy, Suy), (sol[c], sol[a]), "numpy")

def fit_for_b(bv):
    """При заданном b возвращает (SSE, c, a)."""
    u = np.exp(bv * x)
    cv, av = f_ca(n, u.sum(), (u**2).sum(), y.sum(), (u * y).sum())
    return np.sum((y - cv - av * u)**2), cv, av

# ---------- ЭТАП 2. Подбор параметра b ----------
print("\n=== ЭТАП 2. Подбор b (сетка с последовательным сужением) ===")
lo, hi = -1.0, 1.0
for it in range(6):
    grid = np.linspace(lo, hi, 2001)
    grid = grid[np.abs(grid) > 1e-3]               # b=0 вырожденный случай
    sse_grid = np.array([fit_for_b(g)[0] for g in grid])
    b_best = grid[sse_grid.argmin()]
    step = (hi - lo) / 2000
    lo, hi = b_best - 2 * step, b_best + 2 * step
    print(f"итерация {it+1}: b = {b_best:.6f}, SSE = {sse_grid.min():.4f}")
sse, c_hat, a_hat = fit_for_b(b_best)
print(f"\nИтоговая модель: y = {c_hat:.4f} + {a_hat:.5f}*exp({b_best:.5f}*x)")

# ---------- ЭТАП 3. Качество аппроксимации ----------
print("\n=== ЭТАП 3. Качество ===")
y_hat = c_hat + a_hat * np.exp(b_best * x)
res = y - y_hat
sst = np.sum((y - y.mean())**2)
print(f"SSE = {sse:.3f}, SST = {sst:.3f}, R^2 = {1 - sse/sst:.4f}")
lin = np.polyfit(x, y, 1); sse_lin = np.sum((y - np.polyval(lin, x))**2)
print(f"Линейная модель для сравнения: SSE = {sse_lin:.3f}, R^2 = {1 - sse_lin/sst:.4f}")

# ---------- ЭТАП 4. F-критерий значимости (sympy.stats для F-распределения) ----------
print("\n=== ЭТАП 4. Проверка гипотезы ===")
print("H0: модель не объясняет изменчивость ряда; H1: экспонента значимо описывает ряд")
k = 3                                             # число параметров (c, a, b)
d1, d2 = k - 1, n - k
F = ((sst - sse) / d1) / (sse / d2)
F_cdf = sp.lambdify(sp.Symbol("t"), cdf(FDistribution("F", d1, d2))(sp.Symbol("t", positive=True)), "mpmath")
p_value = float(1 - F_cdf(F))
lo_, hi_ = 0.0, 1000.0                            # критическое значение бисекцией
for _ in range(100):
    mid = (lo_ + hi_) / 2
    if 1 - F_cdf(mid) > alpha: lo_ = mid
    else: hi_ = mid
F_crit = (lo_ + hi_) / 2
print(f"F = {F:.3f}; F_крит(alpha={alpha}; {d1}, {d2}) = {F_crit:.3f}; p-value = {p_value:.2e}")
print("Вывод:", "гипотеза об экспоненциальной аппроксимации НЕ отвергается" if F > F_crit
      else "гипотеза отвергается")

# ---------- ЭТАП 5. Анализ остатков ----------
print("\n=== ЭТАП 5. Остатки ===")
signs = res > 0
runs = 1 + int(np.sum(signs[1:] != signs[:-1]))
print("Остатки:", np.round(res, 2).tolist())
print(f"Число серий знаков: {runs} из {n} (для случайных остатков ожидается ~{n//2+1})")

# ---------- ЭТАП 6. Графики ----------
fig, ax = plt.subplots(1, 3, figsize=(16, 4.5))
xx = np.linspace(1, n, 200)
ax[0].plot(x, y, "o", label="данные")
ax[0].plot(xx, c_hat + a_hat * np.exp(b_best * xx), "r", label="экспонента")
ax[0].plot(xx, np.polyval(lin, xx), "g--", label="линейная")
ax[0].set(title="Аппроксимация", xlabel="x", ylabel="y"); ax[0].legend(); ax[0].grid(True)
ax[1].stem(x, res); ax[1].axhline(0, color="k")
ax[1].set(title="Остатки", xlabel="x"); ax[1].grid(True)
bb = np.linspace(0.01, 0.5, 200)
ax[2].plot(bb, [fit_for_b(v)[0] for v in bb]); ax[2].axvline(b_best, color="r", ls="--")
ax[2].set(title="SSE(b)", xlabel="b", ylabel="SSE"); ax[2].grid(True)
plt.tight_layout(); plt.savefig("task2_plot.png", dpi=120)
print("\nГрафик сохранён: task2_plot.png")