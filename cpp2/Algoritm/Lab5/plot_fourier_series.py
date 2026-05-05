"""
Визуализация данных из fourier_series.cpp
Читает: fourier_series_signal.csv, fourier_series_coeffs.csv
"""

import sys
import numpy as np
import matplotlib.pyplot as plt
import matplotlib.gridspec as gridspec

# ── Цвета ────────────────────────────────────────────────────
BG      = '#0f1117'
PANEL   = '#1a1d27'
WHITE   = '#ffffff'
RED     = '#ff6655'
BLUE    = '#5588ff'
GREEN   = '#44cc88'
GRAY    = '#888899'
GRID    = '#2a2d3a'

def styled_ax(ax):
    ax.set_facecolor(PANEL)
    ax.tick_params(colors=GRAY, labelsize=9)
    for s in ax.spines.values(): s.set_edgecolor(GRID)
    ax.grid(color=GRID, linewidth=0.6, alpha=0.8)

# ── Загрузка данных ───────────────────────────────────────────
def load_signal(path='fourier_series_signal.csv'):
    meta = {}
    with open(path) as fh:
        for line in fh:
            if line.startswith('#'): 
                for part in line[1:].split():
                    if '=' in part:
                        k, v = part.split('=', 1)
                        meta[k] = v
            else:
                break
    data = np.loadtxt(path, delimiter=',', comments='#', skiprows=2)
    return meta, data   # columns: x, f_x, S_N_x, error

def load_coeffs(path='fourier_series_coeffs.csv'):
    data = np.loadtxt(path, delimiter=',', comments='#', skiprows=2)
    return data         # columns: n, an, bn, amplitude

# ── Построение ───────────────────────────────────────────────
def plot(signal_file='fourier_series_signal.csv',
         coeffs_file='fourier_series_coeffs.csv'):

    meta, sig = load_signal(signal_file)
    cof       = load_coeffs(coeffs_file)

    fname = meta.get('function', '?')
    N     = meta.get('N', '?')

    x   = sig[:, 0]
    fx  = sig[:, 1]
    sn  = sig[:, 2]
    err = sig[:, 3]

    ns  = cof[:, 0].astype(int)
    an  = cof[:, 1]
    bn  = cof[:, 2]
    amp = cof[:, 3]

    fig = plt.figure(figsize=(13, 9), facecolor=BG)
    gs  = gridspec.GridSpec(2, 2, figure=fig, hspace=0.42, wspace=0.32)

    # ── 1. Основной: f(x) vs S_N(x) ─────────────────────────
    ax1 = fig.add_subplot(gs[0, :])
    styled_ax(ax1)
    ax1.plot(x, fx,  color=WHITE, lw=2,   label='f(x)')
    ax1.plot(x, sn,  color=RED,   lw=2,   label=f'$S_{{N}}(x)$, N={N}', ls='--')
    ax1.axhline(0, color=GRAY, lw=0.5)
    ax1.axvline(0, color=GRAY, lw=0.5)
    ax1.set_title(f'Ряд Фурье: {fname}  (N={N})', color=WHITE, fontsize=13, pad=10)
    ax1.set_xlabel('x', color=GRAY)
    ax1.set_ylabel('y', color=GRAY)
    ax1.legend(facecolor=PANEL, edgecolor=GRID, labelcolor=WHITE, fontsize=11)

    # ── 2. Погрешность ───────────────────────────────────────
    ax2 = fig.add_subplot(gs[1, 0])
    styled_ax(ax2)
    ax2.fill_between(x, err, color=RED, alpha=0.35)
    ax2.plot(x, err, color=RED, lw=1.2)
    ax2.set_title('Погрешность |f(x) − S_N(x)|', color=WHITE, fontsize=11)
    ax2.set_xlabel('x', color=GRAY)
    ax2.set_ylabel('|ошибка|', color=GRAY)

    # ── 3. Спектр коэффициентов ──────────────────────────────
    ax3 = fig.add_subplot(gs[1, 1])
    styled_ax(ax3)
    w = 0.3
    mask = ns > 0   # без n=0
    ax3.bar(ns[mask] - w/2, np.abs(an[mask]), w, color=BLUE,  alpha=0.85, label='|aₙ|')
    ax3.bar(ns[mask] + w/2, np.abs(bn[mask]), w, color=GREEN, alpha=0.85, label='|bₙ|')
    ax3.set_title('Спектр коэффициентов Фурье', color=WHITE, fontsize=11)
    ax3.set_xlabel('n', color=GRAY)
    ax3.set_ylabel('Амплитуда', color=GRAY)
    ax3.legend(facecolor=PANEL, edgecolor=GRID, labelcolor=WHITE, fontsize=9)

    # ── Подпись ──────────────────────────────────────────────
    max_err = err.max()
    fig.text(0.5, 0.01,
             f'Макс. погрешность = {max_err:.5f}   '
             f'Метод: Симпсон   Данные: {signal_file}',
             ha='center', color=GRAY, fontsize=9)

    out = 'plot_fourier_series.png'
    plt.savefig(out, dpi=150, bbox_inches='tight', facecolor=BG)
    plt.show()
    print(f'Сохранено: {out}')

# ── Точка входа ───────────────────────────────────────────────
if __name__ == '__main__':
    args = [a for a in sys.argv[1:] if not a.startswith('-')]
    sig = args[0] if len(args) > 0 else 'csv/fourier_series_signal.csv'
    cof = args[1] if len(args) > 1 else 'csv/fourier_series_coeffs.csv'
    plot(sig, cof)