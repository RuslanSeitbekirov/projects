import math
import numpy as np
import sympy as sp
from sympy.stats import Binomial, P as prob
import matplotlib
matplotlib.use("Agg")                    # сохраняем график в файл
import matplotlib.pyplot as plt 

# ---------- ЭТАП 0. Входные параметры (меняются пользователем) ----------
n = 50      # число испытаний (выстрелов)
m = 40     # наблюдаемое число промахов
p0 = 0.3     # вероятность промаха по H0 /
p1 = 0.5     # вероятность промаха по H1
alpha = 0.05    # уровень значимости
target_power = 0.8     # желаемая мощность

# ---------- Вспомогательные функции (numpy) ----------
def pmf(n, p):
    """Вектор P(X=k), k=0..n, для Bin(n,p)."""
    k = np.arange(n + 1) #создает массив целых чисел [0, 1, 2, ..., n].
    comb = np.array([math.comb(n, int(i)) for i in k], dtype=float)
    #math.comb(n, int(i)): вычисляет биномиальный коэффициент​ (число сочетаний).
    #np.array([...], dtype=float): преобразует список сочетаний в массив чисел с плавающей точкой.
    return comb * p**k * (1 - p)**(n - k)
    #comb * p**k * (1 - p)**(n - k): векторизованная формула Бернулли. NumPy автоматически применяет формулу к каждому элементу массива, возвращая массив вероятностей.

#ищет индексы критических границ (краснный пунктир на графике)
def critical(n, p0, alpha):
    """Границы двусторонней критической области: m<=k_low или m>=k_high."""
    P = pmf(n, p0)
    cdf_ = np.cumsum(P)                   # P(X<=k) - накопленная сумма слева
    sf_  = np.cumsum(P[::-1])[::-1]       # P(X>=k) - накопленная сумма справа
    low  = np.where(cdf_ <= alpha / 2)[0] # Индексы, где левый хвост <= alpha/2
    high = np.where(sf_  <= alpha / 2)[0] # Индексы, где правый хвост <= alpha/2
    k_low  = int(low[-1]) if low.size else None     # Берем самый правый индекс из левой зоны
    k_high = int(high[0]) if high.size else None    # Берем самый левый индекс из правой зоны
    return k_low, k_high

#Мощность — это вероятность того, что мы правильно отвергнем H0​, когда верна H1​. Мы берем границы, найденные при H0​,
# и смотрим, какая доля вероятности при H1​ попадает в эти границы.
def power(n, p0, p1, alpha):
    """Мощность = P(попасть в крит. область | H1)."""
    k_low, k_high = critical(n, p0, alpha)
    P1 = pmf(n, p1)
    w = 0.0
    if k_low is not None:  w += P1[:k_low + 1].sum()
    if k_high is not None: w += P1[k_high:].sum()
    return w

# ---------- ЭТАП 1. Символьная запись модели (sympy) ----------
print("=== ЭТАП 1. Модель ===")
N, M, p = sp.symbols("n m p")
pmf_sym = sp.binomial(N, M) * p**M * (1 - p)**(N - M)
print("Закон распределения числа промахов: P(X=m) =", pmf_sym)
print("H0: p =", p0, "| H1: p =", p1, "| alpha =", alpha, "| n =", n, "| m =", m)
print("Критерий: H0 отвергается, если m <= k_low или m >= k_high,")
print("          где P(X<=k_low|H0) <= alpha/2 и P(X>=k_high|H0) <= alpha/2")
print("Мощность: 1 - beta = P(X<=k_low|H1) + P(X>=k_high|H1)")

# ---------- ЭТАП 2. Критические значения ----------
print("\n=== ЭТАП 2. Предельное число промахов ===")
k_low, k_high = critical(n, p0, alpha)
print(f"Нижняя граница: m <= {k_low}   (хвост при H0 = {pmf(n,p0)[:k_low+1].sum():.4f})" if k_low is not None
      else "Нижней границы нет (даже P(X=0) > alpha/2)")
print(f"Верхняя граница: m >= {k_high}  (хвост при H0 = {pmf(n,p0)[k_high:].sum():.4f})")
# перекрёстная проверка средствами sympy.stats
X = Binomial("X", n, sp.Rational(str(p0)))
if k_low is not None:
    print("Проверка sympy: P(X<=k_low|H0) =", float(prob(X <= k_low)))
print("Проверка sympy: P(X>=k_high|H0) =", float(prob(X >= k_high)))

# ---------- ЭТАП 3. Решение по данным ----------
print("\n=== ЭТАП 3. Решение ===")
reject = (k_low is not None and m <= k_low) or (k_high is not None and m >= k_high)
P0 = pmf(n, p0)
p_value = min(1.0, 2 * min(P0[:m + 1].sum(), P0[m:].sum()))
print(f"Наблюдалось m={m} промахов. p-value ~ {p_value:.4f}")
print("Вывод:", "H0 ОТВЕРГАЕТСЯ" if reject else "оснований отвергнуть H0 нет")

# ---------- ЭТАП 4. Мощность ----------
print("\n=== ЭТАП 4. Мощность проверки ===")
w = power(n, p0, p1, alpha)
print(f"Мощность 1-beta = {w:.4f}; ошибка II рода beta = {1-w:.4f}")
n_need = next((nn for nn in range(5, 3000) if power(nn, p0, p1, alpha) >= target_power), None)
print(f"Для мощности >= {target_power} нужно n >= {n_need}")

# ---------- ЭТАП 5. Зависимость от alpha и n ----------
print("\n=== ЭТАП 5. Зависимость границ и мощности от alpha ===")
alphas = [0.2, 0.1, 0.05, 0.01, 0.001]
print("alpha    нижняя  верхняя  мощность")
for a in alphas:
    kl, kh = critical(n, p0, a)
    print(f"{a:<8} {str(kl):<7} {str(kh):<8} {power(n, p0, p1, a):.3f}")
print("Вывод: чем меньше alpha, тем шире область принятия H0 и тем ниже мощность.")
print("\nМощность от числа испытаний (alpha =", alpha, "):")
ns = np.arange(5, 121)
pw_n = np.array([power(int(i), p0, p1, alpha) for i in ns])
for i in (10, 20, 40, 80, 120):
    print(f"n={i:<4} мощность={pw_n[i-5]:.3f}")

# ---------- ЭТАП 6. Графики ----------
fig, ax = plt.subplots(1, 3, figsize=(16, 4.5))
k = np.arange(n + 1)
ax[0].bar(k - 0.2, P0, 0.4, label=f"H0: p={p0}")
ax[0].bar(k + 0.2, pmf(n, p1), 0.4, label=f"H1: p={p1}", alpha=0.7)
if k_low is not None:  ax[0].axvline(k_low + 0.5, color="r", ls="--")
if k_high is not None: ax[0].axvline(k_high - 0.5, color="r", ls="--", label="границы")
ax[0].axvline(m, color="k", ls=":", label=f"наблюдение m={m}")
ax[0].set(title="Распределения числа промахов", xlabel="сделано m промахов", ylabel="P (Вероятность что кандидат промахнётся m раз)"); ax[0].legend()
al_grid = np.logspace(-3, np.log10(0.3), 40)
ax[1].semilogx(al_grid, [power(n, p0, p1, a) for a in al_grid])
ax[1].set(title="Мощность от alpha", xlabel="alpha", ylabel="1-beta (шанс поймать плохого стрелка)"); ax[1].grid(True)
ax[2].plot(ns, pw_n); ax[2].axhline(target_power, color="r", ls="--")
ax[2].set(title="Мощность от n", xlabel="n", ylabel="1-beta (шанс поймать плохого стрелка)"); ax[2].grid(True)
plt.tight_layout(); plt.savefig("task1_plot.png", dpi=120)
print("\nГрафик сохранён: task1_plot.png")