"""
Визуализация данных из fourier_fft.cpp
Читает: fourier_fft_signal.csv, fourier_fft_spectrum.csv, fourier_fft_coeffs.csv
"""

import sys
import numpy as np
import matplotlib.pyplot as plt
import matplotlib.gridspec as gridspec

# ── Цвета ────────────────────────────────────────────────────
BG    = '#0f1117'
PANEL = '#1a1d27'
WHITE = '#ffffff'
RED   = '#ff6655'
BLUE  = '#5588ff'
GREEN = '#44cc88'
GOLD  = '#ffbb44'
GRAY  = '#888899'
GRID  = '#2a2d3a'

def styled_ax(ax):
    ax.set_facecolor(PANEL)
    ax.tick_params(colors=GRAY, labelsize=9)
    for s in ax.spines.values(): s.set_edgecolor(GRID)
    ax.grid(color=GRID, linewidth=0.6, alpha=0.8)

# ── Загрузка данных ───────────────────────────────────────────
def load_signal(path='fourier_fft_signal.csv'):
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
    return meta, data   # x, f_x, fourier_sum, ifft_restored, error_fourier, error_ifft

def load_spectrum(path='fourier_fft_spectrum.csv'):
    return np.loadtxt(path, delimiter=',', comments='#', skiprows=2)
    # k, re, im, amplitude

def load_coeffs(path='fourier_fft_coeffs.csv'):
    return np.loadtxt(path, delimiter=',', comments='#', skiprows=2)
    # n, an, bn, amplitude

# ── Построение ───────────────────────────────────────────────
def plot(signal_file='fourier_fft_signal.csv',
         spectrum_file='fourier_fft_spectrum.csv',
         coeffs_file='fourier_fft_coeffs.csv'):

    meta, sig = load_signal(signal_file)
    sp        = load_spectrum(spectrum_file)
    cof       = load_coeffs(coeffs_file)

    fname = meta.get('function', '?')
    N     = meta.get('N', '?')
    M     = meta.get('FFT_points', '?')

    x       = sig[:, 0]
    fx      = sig[:, 1]
    sn      = sig[:, 2]
    ifft_r  = sig[:, 3]
    err_sn  = sig[:, 4]
    err_ift = sig[:, 5]

    k_fft   = sp[:, 0].astype(int)
    amp_fft = sp[:, 3]

    ns  = cof[:, 0].astype(int)
    an  = cof[:, 1]
    bn  = cof[:, 2]

    fig = plt.figure(figsize=(14, 10), facecolor=BG)
    gs  = gridspec.GridSpec(3, 2, figure=fig, hspace=0.48, wspace=0.32)

    # ── 1. Основной: три кривые ──────────────────────────────
    ax1 = fig.add_subplot(gs[0, :])
    styled_ax(ax1)
    ax1.plot(x, fx,     color=WHITE, lw=2,   label='f(x)',             zorder=4)
    ax1.plot(x, sn,     color=RED,   lw=1.8, label=f'Ряд Фурье N={N}', ls='--', zorder=3)
    ax1.plot(x, ifft_r, color=GREEN, lw=1.5, label='IFFT(FFT(f))',     ls=':',  zorder=3)
    ax1.axhline(0, color=GRAY, lw=0.5)
    ax1.axvline(0, color=GRAY, lw=0.5)
    ax1.set_title(f'БПФ + Ряд Фурье: {fname}', color=WHITE, fontsize=13, pad=10)
    ax1.set_xlabel('x', color=GRAY)
    ax1.set_ylabel('y', color=GRAY)
    ax1.legend(facecolor=PANEL, edgecolor=GRID, labelcolor=WHITE, fontsize=10)

    # ── 2. Погрешность ───────────────────────────────────────
    ax2 = fig.add_subplot(gs[1, 0])
    styled_ax(ax2)
    ax2.fill_between(x, err_sn,  color=RED,   alpha=0.3, label='ряд Фурье')
    ax2.fill_between(x, err_ift, color=GREEN, alpha=0.3, label='IFFT')
    ax2.plot(x, err_sn,  color=RED,   lw=1.2)
    ax2.plot(x, err_ift, color=GREEN, lw=1.2)
    ax2.set_title('Погрешность приближения', color=WHITE, fontsize=11)
    ax2.set_xlabel('x', color=GRAY); ax2.set_ylabel('|ошибка|', color=GRAY)
    ax2.legend(facecolor=PANEL, edgecolor=GRID, labelcolor=WHITE, fontsize=9)

    # ── 3. Амплитудный спектр FFT ────────────────────────────
    ax3 = fig.add_subplot(gs[1, 1])
    styled_ax(ax3)
    show = min(40, len(k_fft))
    ax3.bar(k_fft[:show], amp_fft[:show], color=BLUE, alpha=0.85, width=0.7)
    ax3.set_title(f'Амплитудный спектр FFT (первые {show} частот)', color=WHITE, fontsize=11)
    ax3.set_xlabel('k (номер частоты)', color=GRAY)
    ax3.set_ylabel('|X[k]| / N', color=GRAY)

    # ── 4. Коэффициенты ряда Фурье ───────────────────────────
    ax4 = fig.add_subplot(gs[2, 0])
    styled_ax(ax4)
    mask = ns > 0
    w = 0.3
    ax4.bar(ns[mask] - w/2, np.abs(an[mask]), w, color=BLUE,  alpha=0.85, label='|aₙ|')
    ax4.bar(ns[mask] + w/2, np.abs(bn[mask]), w, color=RED,   alpha=0.85, label='|bₙ|')
    ax4.set_title('Коэффициенты ряда Фурье', color=WHITE, fontsize=11)
    ax4.set_xlabel('n', color=GRAY); ax4.set_ylabel('Амплитуда', color=GRAY)
    ax4.legend(facecolor=PANEL, edgecolor=GRID, labelcolor=WHITE, fontsize=9)

    # ── 5. Логарифмический спектр ────────────────────────────
    ax5 = fig.add_subplot(gs[2, 1])
    styled_ax(ax5)
    pos_mask = amp_fft[:show] > 0
    ax5.semilogy(k_fft[:show][pos_mask], amp_fft[:show][pos_mask],
                 color=GOLD, lw=1.5, marker='o', ms=3)
    ax5.set_title('Спектр FFT (лог. шкала)', color=WHITE, fontsize=11)
    ax5.set_xlabel('k', color=GRAY); ax5.set_ylabel('|X[k]|/N  (log)', color=GRAY)
    ax5.yaxis.label.set_color(GRAY)

    # ── Подпись ──────────────────────────────────────────────
    fig.text(0.5, 0.005,
             f'Макс. погрешность ряда={err_sn.max():.2e}   '
             f'Макс. погрешность IFFT={err_ift.max():.2e}   '
             f'FFT точек={M}',
             ha='center', color=GRAY, fontsize=9)

    out = 'plot_fourier_fft.png'
    plt.savefig(out, dpi=150, bbox_inches='tight', facecolor=BG)
    plt.show()
    print(f'Сохранено: {out}')

# ── Точка входа ───────────────────────────────────────────────
if __name__ == '__main__':
    args = [a for a in sys.argv[1:] if not a.startswith('-')]
    sig = args[0] if len(args) > 0 else 'csv/fourier_fft_signal.csv'
    sp  = args[1] if len(args) > 1 else 'csv/fourier_fft_spectrum.csv'
    cof = args[2] if len(args) > 2 else 'csv/fourier_fft_coeffs.csv'
    plot(sig, sp, cof)