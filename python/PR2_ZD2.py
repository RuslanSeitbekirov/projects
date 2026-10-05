import math

# ============ ЗАДАНИЕ 2: экспоненциальная аппроксимация y = c + a*exp(b*x) ============
y = [1.7,-5.4,-4.0,-5.9,-1.6,0.0,0.6,2.1,0.1,-4.9,-3.5,5.9,8.5,9.9,13.3,11.1,14.4,16.2]
x = list(range(1, len(y)+1))

def linfit(u, v):                       # v = c + a*u (МНК)
    n = len(u); mu = sum(u)/n; mv = sum(v)/n
    suu = sum((ui-mu)**2 for ui in u)
    a = sum((ui-mu)*(vi-mv) for ui, vi in zip(u, v)) / suu
    return mv - a*mu, a

def sse_for_b(b):
    u = [math.exp(b*xi) for xi in x]
    c, a = linfit(u, y)
    return sum((yi - c - a*ui)**2 for yi, ui in zip(y, u)), c, a

def fit_exp():
    grid = [i/1000 for i in range(-1000, 1001) if abs(i) >= 5]
    b = min(grid, key=lambda t: sse_for_b(t)[0])
    lo, hi = b-0.001, b+0.001           # золотое сечение
    g = (math.sqrt(5)-1)/2
    for _ in range(80):
        m1, m2 = hi-g*(hi-lo), lo+g*(hi-lo)
        if sse_for_b(m1)[0] < sse_for_b(m2)[0]: hi = m2
        else: lo = m1
    b = (lo+hi)/2
    sse, c, a = sse_for_b(b)
    return c, a, b, sse

# F-распределение: p-value через неполную бета-функцию
def betacf(a, b, x):
    tiny = 1e-300; qab, qap, qam = a+b, a+1, a-1
    c = 1.0; d = 1-qab*x/qap; d = tiny if abs(d) < tiny else d; d = 1/d; h = d
    for m in range(1, 300):
        m2 = 2*m
        aa = m*(b-m)*x/((qam+m2)*(a+m2))
        d = 1+aa*d; d = tiny if abs(d) < tiny else d
        c = 1+aa/c; c = tiny if abs(c) < tiny else c
        d = 1/d; h *= d*c
        aa = -(a+m)*(qab+m)*x/((a+m2)*(qap+m2))
        d = 1+aa*d; d = tiny if abs(d) < tiny else d
        c = 1+aa/c; c = tiny if abs(c) < tiny else c
        d = 1/d; de = d*c; h *= de
        if abs(de-1) < 1e-14: break
    return h

def betai(a, b, x):
    if x <= 0: return 0.0
    if x >= 1: return 1.0
    bt = math.exp(math.lgamma(a+b)-math.lgamma(a)-math.lgamma(b)+a*math.log(x)+b*math.log(1-x))
    if x < (a+1)/(a+b+2): return bt*betacf(a, b, x)/a
    return 1 - bt*betacf(b, a, 1-x)/b

def f_pvalue(F, d1, d2):                # P(F_{d1,d2} > F)
    return betai(d2/2, d1/2, d2/(d2+d1*F))

def f_critical(alpha, d1, d2):          # бисекция
    lo, hi = 0.0, 1000.0
    for _ in range(200):
        mid = (lo+hi)/2
        if f_pvalue(mid, d1, d2) > alpha: lo = mid
        else: hi = mid
    return (lo+hi)/2

def approx_test(alpha=0.05):
    n = len(y); my = sum(y)/n
    c, a, b, sse = fit_exp()
    sst = sum((v-my)**2 for v in y)
    k = 3                                # c, a, b
    d1, d2 = k-1, n-k
    F = ((sst-sse)/d1) / (sse/d2)
    Fcr, p = f_critical(alpha, d1, d2), f_pvalue(F, d1, d2)
    print(f"\nМодель: y = {c:.4f} + {a:.5f}*exp({b:.4f}*x)")
    print(f"SSE={sse:.3f}, SST={sst:.3f}, R^2={1-sse/sst:.4f}")
    print(f"F={F:.3f}, F_крит(alpha={alpha}; {d1},{d2})={Fcr:.3f}, p={p:.2e}")
    print("Вывод:", "гипотеза не отвергается (экспонента значимо описывает ряд)" if F > Fcr
          else "гипотеза отвергается")
    # сравнение с линейной моделью
    c1, a1 = linfit(x, y)
    sse1 = sum((yi-c1-a1*xi)**2 for xi, yi in zip(x, y))
    print(f"Для сравнения линейная: SSE={sse1:.3f}, R^2={1-sse1/sst:.4f}")
    # критерий серий по знакам остатков (случайность остатков)
    res = [yi-c-a*math.exp(b*xi) for xi, yi in zip(x, y)]
    s = [r > 0 for r in res]
    runs = 1+sum(s[i] != s[i-1] for i in range(1, n))
    print("Число серий знаков остатков:", runs, "из", n)

if __name__ == "__main__":
    approx_test(0.05)