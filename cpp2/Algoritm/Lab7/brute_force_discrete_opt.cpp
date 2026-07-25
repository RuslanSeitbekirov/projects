/*
 * ====================================================================
 *  МНОГОМЕРНАЯ ДИСКРЕТНАЯ ОПТИМИЗАЦИЯ
 *  Метод полного перебора (Brute Force Enumeration)
 *
 *  Задача (Вариант 4):
 *    МАКСИМИЗИРОВАТЬ F = 2*x1 + x2
 *
 *    При ограничениях:
 *      7*x1 + 3*x2 <= 21
 *      x1  +   x2  <= 5
 *      0 <= x1 <= 3
 *      0 <= x2 <= 5
 *      x1, x2 — целые (x1, x2 in Z)
 *
 *  Метод перебора: 
 *    Перебираем ВСЕ целые точки из прямоугольника [0..3] x [0..5].
 *    Для каждой точки проверяем выполнение всех ограничений.
 *    Среди допустимых точек ищем максимум F.
 *    Итого кандидатов: 4 * 6 = 24 комбинации.
 *
 *  Вывод:
 *    · Таблица всех переборных комбинаций с проверкой ограничений
 *    · Пошаговый прогресс поиска (текущий максимум)
 *    · Топ-5 лучших допустимых решений
 *    · ASCII-карта значений F(x1,x2) на плоскости
 *    · Гистограмма значений F по допустимой области
 *    · Итоговый результат
 * ====================================================================
 */

#include <iostream>
#include <iomanip>
#include <vector>
#include <string>
#include <algorithm>
#include <cmath>
#include <sstream>
#include <fstream>
#include <climits>

using namespace std;

// ====================================================================
//  ЦЕЛЕВАЯ ФУНКЦИЯ (максимизация)
// ====================================================================
int F(int x1, int x2) {
    return 2*x1 + x2;
}

// ====================================================================
//  ПРОВЕРКА ВСЕХ ОГРАНИЧЕНИЙ
//  Возвращает true, если точка допустима
// ====================================================================
bool isFeasible(int x1, int x2) {
    if (7*x1 + 3*x2 > 21) return false;  // ограничение 1
    if (x1  +   x2  >  5) return false;  // ограничение 2
    if (x1 < 0 || x1 > 3) return false;  // граница x1
    if (x2 < 0 || x2 > 5) return false;  // граница x2
    return true;
}

// ====================================================================
//  Структура одного решения
// ====================================================================
struct Solution {
    int  x1, x2;
    int  fval;
    bool feasible;  // допустимая точка?
    int  index;     // порядковый номер в переборе
};

// ====================================================================
//  Разделительная линия
// ====================================================================
void hline(int w = 74, char c = '-') {
    cout << string(w, c) << "\n";
}

// ====================================================================
//  ВЫВОД ТАБЛИЦЫ ПЕРЕБОРА
// ====================================================================
void printHeader() {
    cout << "\n";
    hline(74, '=');
    cout << "  ТАБЛИЦА ПОЛНОГО ПЕРЕБОРА\n";
    hline(74, '=');
    cout << setw(5)  << "  #"
         << setw(5)  << "x1"
         << setw(5)  << "x2"
         << setw(8)  << "F=2x1+x2"
         << setw(12) << "7x1+3x2"
         << setw(10) << "x1+x2"
         << setw(12) << "Доп-ть"
         << setw(14) << "Тек. max"
         << "\n";
    hline(74);
}

void printRow(const Solution& s, int curMax) {
    int lhs1 = 7*s.x1 + 3*s.x2;
    int lhs2 = s.x1   +   s.x2;

    cout << setw(5)  << s.index
         << setw(5)  << s.x1
         << setw(5)  << s.x2
         << setw(8)  << s.fval;

    // Ограничение 1: 7x1+3x2 <= 21
    cout << setw(9)  << lhs1 << "<=21";
    // Ограничение 2: x1+x2 <= 5
    cout << setw(7)  << lhs2 << "<=5";

    if (s.feasible) {
        cout << setw(8) << "ДА";
        cout << setw(10) << curMax;
        if (s.fval == curMax) cout << "  *** MAX ***";
    } else {
        cout << setw(8) << "НЕТ";
        cout << setw(10) << (curMax == INT_MIN ? 0 : curMax);
        // Объясняем нарушение
        cout << "  [";
        if (lhs1 > 21) cout << "7x1+3x2>21 ";
        if (lhs2 >  5) cout << "x1+x2>5";
        cout << "]";
    }
    cout << "\n";
}

// ====================================================================
//  ASCII-КАРТА значений F на плоскости (x1, x2)
//  Строки — x2 (сверху = большие), столбцы — x1
// ====================================================================
void printMap(const vector<Solution>& all,
              int x1max, int x2max) {
    cout << "\n";
    hline(74, '=');
    cout << "  КАРТА ЗНАЧЕНИЙ F(x1,x2) НА ПЛОСКОСТИ\n";
    cout << "  '[F]' = допустимая точка  '---' = нарушение ограничений\n";
    hline(74);

    // Заголовок: x1 по горизонтали
    cout << "  x2\\x1 ";
    for (int v1 = 0; v1 <= x1max; v1++)
        cout << setw(7) << v1;
    cout << "\n  ";
    hline(6 + 7*(x1max+1), '-');

    // Строки: x2 сверху вниз (убывает)
    for (int v2 = x2max; v2 >= 0; v2--) {
        cout << setw(5) << v2 << " |";
        for (int v1 = 0; v1 <= x1max; v1++) {
            // Найдём решение в таблице
            const Solution* sp = nullptr;
            for (auto& s : all)
                if (s.x1==v1 && s.x2==v2) { sp=&s; break; }

            if (!sp) { cout << setw(7) << "?"; continue; }

            if (sp->feasible) {
                // Выводим значение F с признаком
                ostringstream oss;
                oss << "[" << sp->fval << "]";
                cout << setw(7) << oss.str();
            } else {
                cout << setw(7) << "---";
            }
        }
        cout << "\n";
    }
    cout << "  ";
    hline(6 + 7*(x1max+1), '-');
    cout << "  Легенда: [F] = F(x1,x2) в допустимой точке,"
            "  --- = недопустимая\n";
}

// ====================================================================
//  ASCII-ГИСТОГРАММА значений F по допустимым точкам
// ====================================================================
void printHistogram(const vector<Solution>& feasibleOnly) {
    if (feasibleOnly.empty()) return;

    int fmin = feasibleOnly.front().fval;
    int fmax = feasibleOnly.back().fval;
    const int BAR_W = 36;

    cout << "\n";
    hline(74, '=');
    cout << "  ГИСТОГРАММА F ПО ДОПУСТИМЫМ ТОЧКАМ\n";
    hline(74);

    // Группируем по значению F
    for (int fv = fmin; fv <= fmax; fv++) {
        int cnt = 0;
        string pts;
        for (auto& s : feasibleOnly)
            if (s.fval == fv) {
                cnt++;
                pts += "(" + to_string(s.x1) + "," + to_string(s.x2) + ") ";
            }
        if (cnt == 0) continue;

        int len = max(1, (int)round((double)cnt / (double)feasibleOnly.size() * BAR_W));
        cout << "  F=" << setw(3) << fv << " |"
             << string(len, '#') << string(BAR_W-len, '.')
             << "| " << cnt << " точки:  " << pts << "\n";
    }
    hline(74);
}

// ====================================================================
//  ТОП-K ЛУЧШИХ ДОПУСТИМЫХ РЕШЕНИЙ
// ====================================================================
void printTop(const vector<Solution>& sorted, int K = 5) {
    int n = min(K, (int)sorted.size());
    cout << "\n";
    hline(74, '=');
    cout << "  ТОП-" << n << " ЛУЧШИХ ДОПУСТИМЫХ РЕШЕНИЙ\n";
    hline(74);
    cout << setw(6)  << "Место"
         << setw(7)  << "x1"
         << setw(7)  << "x2"
         << setw(12) << "F=2x1+x2"
         << setw(14) << "7x1+3x2<=21"
         << setw(12) << "x1+x2<=5"
         << setw(10) << "№ итер."
         << "\n";
    hline(74);
    for (int i = 0; i < n; i++) {
        const auto& s = sorted[i];
        cout << setw(6)  << (i+1)
             << setw(7)  << s.x1
             << setw(7)  << s.x2
             << setw(12) << s.fval
             << setw(11) << 7*s.x1+3*s.x2 << "<= 21"
             << setw(9)  << s.x1+s.x2 << "<= 5"
             << setw(10) << s.index
             << "\n";
    }
    hline(74);
}

// ====================================================================
//  ПРОВЕРКА ОГРАНИЧЕНИЙ В ОПТИМАЛЬНОЙ ТОЧКЕ (подробно)
// ====================================================================
void printFeasibilityCheck(const Solution& best) {
    cout << "\n";
    hline(74, '=');
    cout << "  ПРОВЕРКА ВСЕХ ОГРАНИЧЕНИЙ В ОПТИМАЛЬНОЙ ТОЧКЕ\n";
    hline(74);
    cout << "  x1* = " << best.x1 << ",  x2* = " << best.x2 << "\n\n";

    auto chk = [](bool ok){ return ok ? "[ВЫПОЛНЕНО]" : "[НАРУШЕНО!]"; };

    int lhs1 = 7*best.x1 + 3*best.x2;
    int lhs2 = best.x1 + best.x2;

    cout << "  1) 7*x1 + 3*x2 <= 21 :  7*" << best.x1
         << " + 3*" << best.x2 << " = " << lhs1
         << " <= 21  " << chk(lhs1 <= 21) << "\n";

    cout << "  2) x1 + x2 <= 5     :  "
         << best.x1 << " + " << best.x2 << " = " << lhs2
         << " <= 5   " << chk(lhs2 <= 5) << "\n";

    cout << "  3) 0 <= x1 <= 3     :  0 <= "
         << best.x1 << " <= 3   " << chk(best.x1>=0 && best.x1<=3) << "\n";

    cout << "  4) 0 <= x2 <= 5     :  0 <= "
         << best.x2 << " <= 5   " << chk(best.x2>=0 && best.x2<=5) << "\n";

    cout << "  5) x1, x2 in Z      :  "
         << best.x1 << ", " << best.x2 << " -- целые  [ВЫПОЛНЕНО]\n";

    hline(74);
}

// ====================================================================
//  ЭКСПОРТ CSV
// ====================================================================
void exportCSV(const vector<Solution>& all) {
    ofstream f("brute_force_all.csv");
    f << "index,x1,x2,F_value,feasible,7x1+3x2,x1+x2\n";
    for (auto& s : all)
        f << s.index << "," << s.x1 << "," << s.x2 << ","
          << s.fval << "," << (s.feasible?1:0) << ","
          << 7*s.x1+3*s.x2 << "," << s.x1+s.x2 << "\n";
    f.close();
    cout << "  -> brute_force_all.csv  (" << all.size() << " строк)\n";
}

// ====================================================================
//  MAIN
// ====================================================================
int main() {
    // ── Описание задачи ──────────────────────────────────────────
    cout << "\n";
    hline(74, '=');
    cout << "  МНОГОМЕРНАЯ ДИСКРЕТНАЯ ОПТИМИЗАЦИЯ  (Вариант 4)\n";
    cout << "  Метод: полный перебор (Brute Force)\n";
    hline(74, '=');

    cout << "\n  Целевая функция (МАКСИМИЗАЦИЯ):\n";
    cout << "    F(x1, x2) = 2*x1 + x2  -->  max\n\n";

    cout << "  Ограничения:\n";
    cout << "    [1]  7*x1 + 3*x2 <= 21\n";
    cout << "    [2]  x1  +   x2  <=  5\n";
    cout << "    [3]  0 <= x1 <= 3\n";
    cout << "    [4]  0 <= x2 <= 5\n";
    cout << "    [5]  x1, x2 in Z  (целые)\n\n";

    // Границы перебора
    const int X1_MIN=0, X1_MAX=3;
    const int X2_MIN=0, X2_MAX=5;

    int total = (X1_MAX-X1_MIN+1) * (X2_MAX-X2_MIN+1);
    cout << "  Полный перебор по прямоугольнику:\n";
    cout << "    x1 in {" << X1_MIN << ".." << X1_MAX << "}  — "
         << (X1_MAX-X1_MIN+1) << " значений\n";
    cout << "    x2 in {" << X2_MIN << ".." << X2_MAX << "}  — "
         << (X2_MAX-X2_MIN+1) << " значений\n";
    cout << "    Всего кандидатов: " << (X1_MAX-X1_MIN+1)
         << " x " << (X2_MAX-X2_MIN+1) << " = " << total << "\n";

    // ── ПОЛНЫЙ ПЕРЕБОР ───────────────────────────────────────────
    printHeader();

    vector<Solution> all;
    all.reserve(total);
    Solution best{0, 0, INT_MIN, false, 0};
    int curMax = INT_MIN;
    int idx    = 0;

    for (int v1 = X1_MIN; v1 <= X1_MAX; v1++) {
        for (int v2 = X2_MIN; v2 <= X2_MAX; v2++) {
            idx++;
            bool ok  = isFeasible(v1, v2);
            int  fv  = F(v1, v2);
            Solution s{v1, v2, fv, ok, idx};
            all.push_back(s);

            if (ok && fv > curMax) {
                curMax = fv;
                best   = s;
            }
            printRow(s, curMax == INT_MIN ? 0 : curMax);
        }
    }

    // ── Допустимые решения — сортируем по убыванию F ─────────────
    vector<Solution> feasible;
    for (auto& s : all) if (s.feasible) feasible.push_back(s);
    sort(feasible.begin(), feasible.end(),
         [](const Solution& a, const Solution& b){ return a.fval > b.fval; });

    int feasCnt = (int)feasible.size();
    int infeasCnt = total - feasCnt;
    cout << "\n  Итого: " << total << " кандидатов,  "
         << "допустимых: " << feasCnt << ",  "
         << "недопустимых: " << infeasCnt << "\n";

    // ── Дополнительный вывод ─────────────────────────────────────
    printTop(feasible, 5);
    printMap(all, X1_MAX, X2_MAX);

    // Для гистограммы: сортируем по F возрастанию
    vector<Solution> feasAsc = feasible;
    sort(feasAsc.begin(), feasAsc.end(),
         [](const Solution& a, const Solution& b){ return a.fval < b.fval; });
    printHistogram(feasAsc);

    // ── Итоговый результат ───────────────────────────────────────
    cout << "\n";
    hline(74, '=');
    cout << "  ИТОГ: ОПТИМАЛЬНОЕ РЕШЕНИЕ\n";
    hline(74, '=');
    cout << "  x1*      = " << best.x1   << "\n";
    cout << "  x2*      = " << best.x2   << "\n";
    cout << "  F* = 2*" << best.x1 << " + " << best.x2
         << " = " << best.fval << "  (МАКСИМУМ)\n";
    cout << "  Найдено на итерации #" << best.index
         << " из " << total << "\n";

    printFeasibilityCheck(best);

    // Статистика
    if (!feasible.empty()) {
        double favg = 0;
        for (auto& s : feasible) favg += s.fval;
        favg /= feasible.size();
        cout << "\n  Статистика по допустимой области (" << feasCnt << " точек):\n";
        cout << "    max F = " << feasible.front().fval << "\n";
        cout << "    min F = " << feasAsc.front().fval  << "\n";
        cout << "    avg F = " << fixed << setprecision(2) << favg << "\n";
    }

    hline(74, '=');

    cout << "\n  Экспорт:\n";
    exportCSV(all);

    cout << "\nГотово!\n";
    return 0;
}
