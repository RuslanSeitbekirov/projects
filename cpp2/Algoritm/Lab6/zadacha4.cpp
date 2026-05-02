/*
 * ============================================================
 *  Симплекс-метод (двухэтапный) — Лабораторная работа №6, Вариант 4
 *
 *  Исходная задача:
 *    Минимизировать: Z = x1 + x2 + x3 + x4
 *    x1 — краски    (30 д.е./кор.)
 *    x2 — карандаши (20 д.е./кор.)
 *    x3 — линейки   (12 д.е./шт.)
 *    x4 — блокноты  (10 д.е./шт.)
 *
 *    30x1 + 20x2 + 12x3 + 10x4 >= 300   (бюджет)
 *    x1 >= 3                              (красок не менее 3)
 *    x3 <= 5                              (линеек не более 5)
 *    x4 = x1 + x2                         (блокноты = краски + карандаши)
 *
 *  После подстановки x4 = x1+x2:
 *    Минимизировать: 2x1 + 2x2 + x3
 *    40x1 + 30x2 + 12x3 >= 300
 *    x1 >= 3
 *    x3 <= 5
 *
 *  Стандартная форма:
 *    40x1 + 30x2 + 12x3 - s1 + a1 = 300
 *    x1 - s2 + a2 = 3
 *    x3 + s3 = 5
 *
 *  Переменные (столбцы):  x1 x2 x3 s1 s2 s3 | b
 *                         0  1  2  3  4  5   | 6
 *  Искусственные a1, a2 используются только на Этапе 1.
 * ============================================================
 */

#include <iostream>
#include <iomanip>
#include <vector>
#include <string>
#include <cmath>
#include <limits>
#include <algorithm>
#include <climits>

using namespace std;

const double INF = numeric_limits<double>::infinity();
const double EPS = 1e-9;

// ============================================================
//  Вывод таблицы
// ============================================================
void printTable(const vector<vector<double>>& tab,
                const vector<string>& colNames,
                const vector<string>& rowNames,
                const string& title,
                int pivRow = -1, int pivCol = -1)
{
    const int W = 10;
    int cols = (int)colNames.size();

    cout << "\n" << string(70,'=') << "\n" << title << "\n";
    cout << string(70,'-') << "\n";
    cout << setw(8) << "Базис";
    for (auto& n : colNames) cout << setw(W) << n;
    cout << "\n" << string(8 + W*cols, '-') << "\n";

    for (int r = 0; r < (int)tab.size(); r++) {
        cout << setw(8) << rowNames[r];
        for (int c = 0; c < cols; c++) {
            double v = tab[r][c];
            string s = to_string((int)round(v*1000));
            // форматируем
            char buf[32];
            snprintf(buf, sizeof(buf), "%.3f", v);
            string vs(buf);
            if (r == pivRow && c == pivCol)
                cout << setw(W-1) << vs << "*";
            else
                cout << setw(W) << vs;
        }
        cout << "\n";
    }
}

// ============================================================
//  Жорданово исключение (pivot)
// ============================================================
void pivot(vector<vector<double>>& tab, int pivRow, int pivCol,
           vector<int>& basis)
{
    int R = (int)tab.size(), C = (int)tab[0].size();
    double pe = tab[pivRow][pivCol];
    for (int c = 0; c < C; c++) tab[pivRow][c] /= pe;
    for (int r = 0; r < R; r++) {
        if (r == pivRow) continue;
        double f = tab[r][pivCol];
        if (fabs(f) < EPS) continue;
        for (int c = 0; c < C; c++) tab[r][c] -= f * tab[pivRow][c];
    }
    basis[pivRow] = pivCol;
}

// ============================================================
//  Один шаг симплекса
//  objRow  — строка ЦФ
//  allowed — набор допустимых столбцов (индексы переменных)
//  Возврат: 0=продолжать, 1=оптимум, -1=неограничена
// ============================================================
int simplexStep(vector<vector<double>>& tab,
                int objRow,
                const vector<int>& allowed,
                const vector<string>& colNames,
                const vector<string>& rowNames,
                vector<int>& basis,
                int numCons,
                const string& stepLabel,
                int& pivRowOut, int& pivColOut)
{
    int bCol = (int)tab[0].size() - 1; // столбец b

    // Поиск ведущего столбца
    int pivCol = -1; double minCoef = -EPS;
    for (int c : allowed) {
        if (tab[objRow][c] < minCoef) { minCoef = tab[objRow][c]; pivCol = c; }
    }
    if (pivCol == -1) return 1; // оптимум

    // Поиск ведущей строки (правило минотношений)
    int pivRow = -1; double minRatio = INF;
    for (int r = 0; r < numCons; r++) {
        if (tab[r][pivCol] < EPS) continue;
        double ratio = tab[r][bCol] / tab[r][pivCol];
        if (ratio < minRatio - EPS) { minRatio = ratio; pivRow = r; }
    }
    if (pivRow == -1) return -1; // неограничена

    // Вывод таблицы с ведущим элементом
    printTable(tab, colNames, rowNames, stepLabel, pivRow, pivCol);
    cout << "  --> Ввести: " << colNames[pivCol]
         << ", Вывести: "  << rowNames[pivRow]
         << " (мин. отношение = " << fixed << setprecision(3) << minRatio << ")\n";

    pivRowOut = pivRow; pivColOut = pivCol;
    pivot(tab, pivRow, pivCol, basis);
    // ── Целочисленный оптимум (перебор) ──────────────────────
    cout << "\n" << string(70,'=') << "\n";
    cout << "  ЦЕЛОЧИСЛЕННЫЙ ОПТИМУМ (перебор)\n";
    cout << string(70,'=') << "\n";
    cout << "  (Вещественный оптимум x1=7.5 нецелый -> ищем целочисленный)\n\n";

    int best_Z = INT_MAX;
    int bx1=-1, bx2=-1, bx3=-1, bx4=-1;

    // x1 >= 3, x3 in [0,5], x2 >= 0
    // бюджет: 40x1 + 30x2 + 12x3 >= 300
    // минимизируем Z = 2x1 + 2x2 + x3 (= общее число предметов - x4 уже учтён)
    for (int ix1 = 3; ix1 <= 30; ix1++)
    for (int ix2 = 0; ix2 <= 30; ix2++)
    for (int ix3 = 0; ix3 <= 5; ix3++) {
        if (40*ix1 + 30*ix2 + 12*ix3 < 300) continue;
        int ix4 = ix1 + ix2;
        int Ztot = ix1 + ix2 + ix3 + ix4;  // = 2x1+2x2+x3
        if (Ztot < best_Z) {
            best_Z = Ztot; bx1=ix1; bx2=ix2; bx3=ix3; bx4=ix4;
        }
    }

    if (best_Z < INT_MAX) {
        double bcost = 30.0*bx1 + 20.0*bx2 + 12.0*bx3 + 10.0*bx4;
        cout << fixed << setprecision(0);
        cout << "  x1 = " << bx1 << "  (краски,     коробки)\n";
        cout << "  x2 = " << bx2 << "  (карандаши,  коробки)\n";
        cout << "  x3 = " << bx3 << "  (линейки,    штук)\n";
        cout << "  x4 = " << bx4 << "  (блокноты,   штук) = x1+x2\n\n";
        cout << "  Z_min (целочисл.) = " << best_Z << " предметов\n";
        cout << "  Стоимость = " << fixed << setprecision(1) << bcost << " д.е.\n";
        cout << "\n  Проверка:\n";
        cout << "  40*"<<bx1<<"+30*"<<bx2<<"+12*"<<bx3<<" = "
             <<40*bx1+30*bx2+12*bx3<<" >= 300  [";
        cout << (40*bx1+30*bx2+12*bx3 >= 300 ? "OK" : "!") << "]\n";
        cout << "  x1="<<bx1<<" >= 3  [" << (bx1>=3?"OK":"!") << "]\n";
        cout << "  x3="<<bx3<<" <= 5  [" << (bx3<=5?"OK":"!") << "]\n";
    }


    return 0;
}

// ============================================================
//  MAIN
// ============================================================
int main()
{
    cout << "=== Симплекс-метод (двухэтапный) ===\n";
    cout << "    Лабораторная работа №6, Вариант 4\n\n";
    cout << "Постановка задачи:\n";
    cout << "  min Z = x1 + x2 + x3 + x4\n";
    cout << "  30x1 + 20x2 + 12x3 + 10x4 >= 300\n";
    cout << "  x1 >= 3\n";
    cout << "  x3 <= 5\n";
    cout << "  x4 = x1 + x2\n\n";
    cout << "После подстановки x4 = x1+x2:\n";
    cout << "  min 2x1 + 2x2 + x3\n";
    cout << "  40x1 + 30x2 + 12x3 >= 300\n";
    cout << "  x1 >= 3\n";
    cout << "  x3 <= 5\n\n";
    cout << "Стандартная форма:\n";
    cout << "  40x1 + 30x2 + 12x3 - s1 + a1 = 300\n";
    cout << "  x1 - s2 + a2 = 3\n";
    cout << "  x3 + s3 = 5\n";

    // ── Этап 1: расширенная таблица с a1, a2 ─────────────────
    // Столбцы: x1  x2  x3  s1  s2  s3  a1  a2  | b
    //          0   1   2   3   4   5   6   7   | 8
    const int numCons = 3;
    const int numVarsExt = 9; // 8 переменных + b

    vector<vector<double>> tab1 = {
    //  x1    x2    x3    s1    s2    s3    a1    a2    b
      { 40,   30,   12,  -1,    0,    0,    1,    0,  300 }, // R0
      {  1,    0,    0,   0,   -1,    0,    0,    1,    3 }, // R1
      {  0,    0,    1,   0,    0,    1,    0,    0,    5 }, // R2
      {  0,    0,    0,   0,    0,    0,    1,    1,    0 }, // R3: W=a1+a2
    };

    vector<string> colNames1 = {"x1","x2","x3","s1","s2","s3","a1","a2","b"};
    vector<int> basis1 = {6, 7, 5};  // a1, a2, s3

    // Имена строк (basis + objRow)
    auto rowNames1 = [&]() -> vector<string> {
        vector<string> rn;
        for (int b : basis1) rn.push_back(colNames1[b]);
        rn.push_back("W");
        return rn;
    };

    // Пересчитываем строку W: W - (R0+R1), т.к. a1,a2 в базисе
    for (int c = 0; c < numVarsExt; c++)
        tab1[3][c] -= (tab1[0][c] + tab1[1][c]);

    {
        auto rn = rowNames1();
        printTable(tab1, colNames1, rn, "Начальная таблица (Этап 1)");
    }

    cout << "\n" << string(70,'*') << "\n";
    cout << "  ЭТАП 1: минимизируем W = a1 + a2\n";
    cout << string(70,'*') << "\n";

    // Разрешены все столбцы, кроме b (индекс 8)
    vector<int> allowed1 = {0,1,2,3,4,5,6,7};

    int iter = 0;
    while (true) {
        auto rn = rowNames1();
        int pr=-1, pc=-1;
        int res = simplexStep(tab1, 3, allowed1, colNames1, rn,
                              basis1, numCons,
                              "Этап 1, итерация " + to_string(iter), pr, pc);
        iter++;
        if (res != 0) break;
    }

    {
        auto rn = rowNames1();
        printTable(tab1, colNames1, rn, "Этап 1 — оптимальная таблица");
    }

    double Wopt = -tab1[3][8];
    cout << "\n  W* = " << fixed << setprecision(6) << fabs(Wopt) << "\n";
    if (fabs(Wopt) > 1e-6) {
        cout << "  Задача не имеет допустимого решения!\n";
        return 1;
    }
    cout << "  W* = 0 -- допустимое решение найдено.\n";

    // ── Переход к Этапу 2 ─────────────────────────────────────
    // Убираем столбцы a1(6) и a2(7): перестраиваем таблицу
    // Столбцы: x1  x2  x3  s1  s2  s3 | b
    //          0   1   2   3   4   5  | 6
    const int numVars2 = 6;
    const int bCol2 = 6;

    vector<vector<double>> tab2(numCons + 1, vector<double>(bCol2 + 1));
    // Копируем строки 0..2 (ограничения), оставляем столбцы 0..5 и b(8)
    int keep[] = {0,1,2,3,4,5,8};
    for (int r = 0; r < numCons; r++)
        for (int ci = 0; ci <= numVars2; ci++)
            tab2[r][ci] = tab1[r][keep[ci]];

    // Строка ЦФ Z = 2x1 + 2x2 + x3 (до пересчёта)
    tab2[numCons] = {2, 2, 1, 0, 0, 0, 0};

    vector<string> colNames2 = {"x1","x2","x3","s1","s2","s3","b"};
    vector<int> basis2 = basis1; // скопировали из этапа 1

    // Пересчитываем строку Z по текущему базису
    for (int r = 0; r < numCons; r++) {
        int bv = basis2[r];
        if (bv >= numVars2) continue; // если вдруг искусственная (не должна)
        double coef = tab2[numCons][bv];
        if (fabs(coef) < EPS) continue;
        for (int c = 0; c <= numVars2; c++)
            tab2[numCons][c] -= coef * tab2[r][c];
    }

    auto rowNames2 = [&]() -> vector<string> {
        vector<string> rn;
        for (int b : basis2) rn.push_back(colNames2[b]);
        rn.push_back("Z");
        return rn;
    };

    {
        auto rn = rowNames2();
        printTable(tab2, colNames2, rn, "Начало Этапа 2 (после пересчёта Z)");
    }

    cout << "\n" << string(70,'*') << "\n";
    cout << "  ЭТАП 2: минимизируем Z = 2x1 + 2x2 + x3\n";
    cout << string(70,'*') << "\n";

    vector<int> allowed2 = {0,1,2,3,4,5};

    iter = 0;
    while (true) {
        auto rn = rowNames2();
        int pr=-1, pc=-1;
        int res = simplexStep(tab2, numCons, allowed2, colNames2, rn,
                              basis2, numCons,
                              "Этап 2, итерация " + to_string(iter), pr, pc);
        iter++;
        if (res == 1) { cout << "  Оптимум достигнут.\n"; break; }
        if (res == -1) { cout << "  Задача неограничена!\n"; return 1; }
    }

    {
        auto rn = rowNames2();
        printTable(tab2, colNames2, rn, "ОПТИМАЛЬНАЯ ТАБЛИЦА");
    }

    // ── Вывод решения ─────────────────────────────────────────
    vector<double> sol(numVars2, 0.0);
    for (int r = 0; r < numCons; r++)
        if (basis2[r] < numVars2)
            sol[basis2[r]] = tab2[r][bCol2];

    double x1=sol[0], x2=sol[1], x3=sol[2];
    double s1=sol[3], s2=sol[4], s3=sol[5];
    double x4=x1+x2;
    double Z = x1+x2+x3+x4;
    double cost = 30*x1 + 20*x2 + 12*x3 + 10*x4;

    cout << "\n" << string(70,'=') << "\n";
    cout << "  ОПТИМАЛЬНОЕ РЕШЕНИЕ\n";
    cout << string(70,'=') << "\n";
    cout << fixed << setprecision(4);
    cout << "  x1 = " << x1 << "  (краски,     коробки)\n";
    cout << "  x2 = " << x2 << "  (карандаши,  коробки)\n";
    cout << "  x3 = " << x3 << "  (линейки,    штук)\n";
    cout << "  x4 = " << x4 << "  (блокноты,   штук) = x1+x2\n";
    cout << "\n";
    cout << "  s1 = " << s1 << "  (излишек бюджета, д.е.)\n";
    cout << "  s2 = " << s2 << "  (излишек по краскам, кор.)\n";
    cout << "  s3 = " << s3 << "  (остаток по линейкам, шт.)\n";
    cout << "\n";
    cout << "  Z_min (общее кол-во предметов) = " << Z << "\n";
    cout << "  Стоимость = 30*"<<x1<<" + 20*"<<x2
         <<" + 12*"<<x3<<" + 10*"<<x4<<" = "<<cost<<" д.е.\n";

    cout << "\n  Проверка ограничений:\n";
    double lhs1 = 40*x1+30*x2+12*x3;
    cout << "  40x1+30x2+12x3 = " << lhs1 << " >= 300  "
         << (lhs1 >= 300-1e-6 ? "[OK]" : "[НАРУШЕНО]") << "\n";
    cout << "  x1 = " << x1 << " >= 3  "
         << (x1 >= 3-1e-6 ? "[OK]" : "[НАРУШЕНО]") << "\n";
    cout << "  x3 = " << x3 << " <= 5  "
         << (x3 <= 5+1e-6 ? "[OK]" : "[НАРУШЕНО]") << "\n";
    cout << "  x4 = x1+x2 = " << x4 << "  [OK]\n";

    // ── Целочисленный оптимум (перебор) ──────────────────────
    cout << "\n" << string(70,'=') << "\n";
    cout << "  ЦЕЛОЧИСЛЕННЫЙ ОПТИМУМ (перебор)\n";
    cout << string(70,'=') << "\n";
    cout << "  (Вещественный оптимум x1=7.5 нецелый -> ищем целочисленный)\n\n";

    int best_Z = INT_MAX;
    int bx1=-1, bx2=-1, bx3=-1, bx4=-1;

    // x1 >= 3, x3 in [0,5], x2 >= 0
    // бюджет: 40x1 + 30x2 + 12x3 >= 300
    // минимизируем Z = 2x1 + 2x2 + x3 (= общее число предметов - x4 уже учтён)
    for (int ix1 = 3; ix1 <= 30; ix1++)
    for (int ix2 = 0; ix2 <= 30; ix2++)
    for (int ix3 = 0; ix3 <= 5; ix3++) {
        if (40*ix1 + 30*ix2 + 12*ix3 < 300) continue;
        int ix4 = ix1 + ix2;
        int Ztot = ix1 + ix2 + ix3 + ix4;  // = 2x1+2x2+x3
        if (Ztot < best_Z) {
            best_Z = Ztot; bx1=ix1; bx2=ix2; bx3=ix3; bx4=ix4;
        }
    }

    if (best_Z < INT_MAX) {
        double bcost = 30.0*bx1 + 20.0*bx2 + 12.0*bx3 + 10.0*bx4;
        cout << fixed << setprecision(0);
        cout << "  x1 = " << bx1 << "  (краски,     коробки)\n";
        cout << "  x2 = " << bx2 << "  (карандаши,  коробки)\n";
        cout << "  x3 = " << bx3 << "  (линейки,    штук)\n";
        cout << "  x4 = " << bx4 << "  (блокноты,   штук) = x1+x2\n\n";
        cout << "  Z_min (целочисл.) = " << best_Z << " предметов\n";
        cout << "  Стоимость = " << fixed << setprecision(1) << bcost << " д.е.\n";
        cout << "\n  Проверка:\n";
        cout << "  40*"<<bx1<<"+30*"<<bx2<<"+12*"<<bx3<<" = "
             <<40*bx1+30*bx2+12*bx3<<" >= 300  [";
        cout << (40*bx1+30*bx2+12*bx3 >= 300 ? "OK" : "!") << "]\n";
        cout << "  x1="<<bx1<<" >= 3  [" << (bx1>=3?"OK":"!") << "]\n";
        cout << "  x3="<<bx3<<" <= 5  [" << (bx3<=5?"OK":"!") << "]\n";
    }


    return 0;
}


