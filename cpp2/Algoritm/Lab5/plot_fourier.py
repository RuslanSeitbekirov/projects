import matplotlib.pyplot as plt
import numpy as np

# Загрузка данных
data = np.loadtxt('fourier_data.csv', comments='#')
x, f_x, S_x = data[:,0], data[:,1], data[:,2]

# Построение графика
plt.figure(figsize=(10, 6))
plt.plot(x, f_x, 'b-', linewidth=2, label='f(x) - исходная функция')
plt.plot(x, S_x, 'r--', linewidth=1.5, label='S_N(x) - ряд Фурье')
plt.xlabel('x'); plt.ylabel('y'); plt.title('Разложение в ряд Фурье');
plt.grid(True, alpha=0.3); plt.legend(); plt.axhline(0, color='k', linewidth=0.5)
plt.tight_layout(); plt.savefig('fourier_plot.png', dpi=300)
plt.show()
