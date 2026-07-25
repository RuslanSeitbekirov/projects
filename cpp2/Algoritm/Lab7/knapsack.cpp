/*
 * ====================================================================
 *  ЗАДАЧА О РЮКЗАКЕ (0/1 Knapsack Problem)
 *  Метод: динамическое программирование
 *
 *  Условие задачи:
 *    Дано N предметов, каждый из которых имеет вес w[i] и ценность v[i].
 *    Дан рюкзак вместимостью W.
 *    Нужно выбрать подмножество предметов с максимальной суммарной
 *    ценностью, не превышая вместимость рюкзака.
 *    Каждый предмет берётся не более одного раза (вариант 0/1).
 *
 *  Алгоритм:
 *    dp[i][w] = максимальная ценность при рассмотрении первых i предметов
 *               и ограничении на вес w.
 *    Рекуррентность:
 *      dp[i][w] = dp[i-1][w],                       если w[i] > w
 *      dp[i][w] = max(dp[i-1][w],
 *                     dp[i-1][w-w[i]] + v[i]),       если w[i] <= w
 *    Ответ: dp[N][W]
 *    Обратный ход: восстановление состава рюкзака
 *
 *  Сложность: O(N*W) по времени и памяти.
 *
 *  Вывод:
 *    · Полная DP-таблица в терминале
 *    · ASCII-гистограмма предметов (ценность и вес)
 *    · Визуализация содержимого рюкзака
 *    · Экспорт DP-таблицы и результата в CSV
 * ====================================================================
 */

#include <iostream>
#include <iomanip>
#include <vector>
#include <string>
#include <algorithm>
#include <fstream>
#include <sstream>
#include <cmath>
#include <set>

using namespace std;

// ====================================================================
//  Структура предмета
// ====================================================================
struct Item {
    string name;
    int    weight;
    int    value;
};

// ====================================================================
//  Основной алгоритм: динамическое программирование
//
//  Возвращает полную DP-таблицу размером (n+1) x (W+1)
// ====================================================================
vector<vector<int>> solveKnapsack(const vector<Item>& items, int W) {
    int n = (int)items.size();
    // Инициализируем таблицу нулями
    vector<vector<int>> dp(n + 1, vector<int>(W + 1, 0));

    for (int i = 1; i <= n; i++) {
        for (int w = 0; w <= W; w++) {
            // Вариант 1: не берём предмет i
            dp[i][w] = dp[i-1][w];
            // Вариант 2: берём предмет i (если помещается)
            if (items[i-1].weight <= w) {
                int withItem = dp[i-1][w - items[i-1].weight] + items[i-1].value;
                dp[i][w] = max(dp[i][w], withItem);
            }
        }
    }
    return dp;
}

// ====================================================================
//  Обратный ход: восстановление состава рюкзака
//  Возвращает индексы выбранных предметов (0-based)
// ====================================================================
vector<int> traceback(const vector<vector<int>>& dp,
                      const vector<Item>& items, int W) {
    int n = (int)items.size();
    vector<int> chosen;
    int w = W;
    for (int i = n; i > 0; i--) {
        // Если значение изменилось по сравнению с предыдущей строкой —
        // значит предмет i был взят
        if (dp[i][w] != dp[i-1][w]) {
            chosen.push_back(i - 1);   // 0-based индекс
            w -= items[i-1].weight;
        } 
    }
    reverse(chosen.begin(), chosen.end());
    return chosen;
}

// ====================================================================
//  ВЫВОД DP-ТАБЛИЦЫ
// ====================================================================
void printDPTable(const vector<vector<int>>& dp,
                  const vector<Item>& items,
                  const vector<int>& chosen, int W) {
    int n = (int)items.size();

    // Собираем множество ячеек обратного хода
    set<pair<int,int>> traceSet;
    int tw = W;
    for (int i = n; i > 0; i--) {
        if (dp[i][tw] != dp[i-1][tw]) {
            traceSet.insert({i, tw});
            tw -= items[i-1].weight;
        }
    }

    // Ширина столбца
    const int COLW = 6;
    // Ограничиваем вывод таблицы: не более 15 столбцов
    int dispW = min(W, 15);

    cout << "\n" << string(70, '=') << "\n";
    cout << "  DP-ТАБЛИЦА  dp[i][w]\n";
    cout << "  '#' = ячейка обратного хода (путь к ответу)\n";
    cout << "  '*' = оптимальный ответ dp[N][W]\n";
    if (W > dispW)
        cout << "  (показаны столбцы w=0.." << dispW << " и w=" << W << ")\n";
    cout << string(70, '-') << "\n";

    // Заголовок
    cout << setw(16) << left << "Предмет \\ w";
    for (int w = 0; w <= dispW; w++)
        cout << setw(COLW) << right << w;
    if (W > dispW) cout << setw(COLW) << right << (".."+to_string(W));
    cout << "\n" << string(70, '-') << "\n";

    for (int i = 0; i <= n; i++) {
        // Метка строки
        if (i == 0)
            cout << setw(16) << left << "i=0 (нет пред.)";
        else
            cout << setw(16) << left << ("i="+to_string(i)+" "+items[i-1].name.substr(0,8));

        for (int w = 0; w <= dispW; w++) {
            bool isTrace  = traceSet.count({i, w}) > 0;
            bool isAnswer = (i == n && w == W);
            string cell   = to_string(dp[i][w]);
            if      (isAnswer) cell += "*";
            else if (isTrace)  cell += "#";
            cout << setw(COLW) << right << cell;
        }
        if (W > dispW) {
            bool isAnswer = (i == n);
            string cell   = to_string(dp[i][W]);
            if (isAnswer) cell += "*";
            cout << setw(COLW) << right << cell;
        }
        cout << "\n";
    }
    cout << string(70, '-') << "\n";
}

// ====================================================================
//  ASCII-ГИСТОГРАММА предметов
// ====================================================================
void printItemsChart(const vector<Item>& items,
                     const vector<int>& chosen) {
    if (items.empty()) return;

    int maxV = (*max_element(items.begin(), items.end(),
                [](const Item& a, const Item& b){ return a.value < b.value; })).value;
    int maxW = (*max_element(items.begin(), items.end(),
                [](const Item& a, const Item& b){ return a.weight < b.weight; })).weight;

    const int BAR_W = 30;
    set<int> chosenSet(chosen.begin(), chosen.end());

    cout << "\n" << string(70, '=') << "\n";
    cout << "  ГИСТОГРАММА ПРЕДМЕТОВ  (█ = в рюкзаке, ░ = не взят)\n";
    cout << string(70, '-') << "\n";

    for (int i = 0; i < (int)items.size(); i++) {
        const auto& it = items[i];
        bool sel = chosenSet.count(i);

        // Строка с именем предмета
        string nameLabel = (sel ? "  [+] " : "  [ ] ") + it.name;
        cout << setw(20) << left << nameLabel;
        cout << setw(6)  << right << to_string(it.value) + " ₽";

        int barLen = (int)round((double)it.value / maxV * BAR_W);
        string bar(barLen, sel ? '#' : '.');
        cout << " |" << bar << "\n";

        // Строка с весом
        cout << setw(20) << left << string(6, ' ');
        cout << setw(6)  << right << to_string(it.weight) + " кг";
        int wLen = (int)round((double)it.weight / maxW * BAR_W);
        string wbar(wLen, sel ? '=' : '-');
        cout << " |" << wbar << "\n\n";
    }
    cout << "  Легенда: # = ценность в рюкзаке,  . = ценность вне рюкзака\n";
    cout << "           = = вес в рюкзаке,        - = вес вне рюкзака\n";
}

// ====================================================================
//  ВИЗУАЛИЗАЦИЯ РЮКЗАКА (ASCII)
// ====================================================================
void printKnapsack(const vector<Item>& items,
                   const vector<int>& chosen, int W) {
    int totalV = 0, totalW = 0;
    for (int idx : chosen) {
        totalV += items[idx].value;
        totalW += items[idx].weight;
    }

    // Прогресс-бар заполненности
    int barTotal = 40;
    int filled   = (W > 0) ? (int)round((double)totalW / W * barTotal) : 0;

    cout << "\n" << string(70, '=') << "\n";
    cout << "  СОДЕРЖИМОЕ РЮКЗАКА\n";
    cout << string(70, '-') << "\n";

    // Рисуем рюкзак
    cout << "  +" << string(50, '-') << "+\n";
    for (int idx : chosen) {
        const auto& it = items[idx];
        string line = "  " + it.name + "  [" +
                      to_string(it.weight) + " кг, " +
                      to_string(it.value) + " ₽]";
        cout << "  |  " << left << setw(46) << line << "|\n";
    }
    if (chosen.empty()) {
        cout << "  |  " << left << setw(46) << "(рюкзак пуст)" << "|\n";
    }
    cout << "  +" << string(50, '-') << "+\n";

    // Статистика
    cout << "\n  Итого предметов : " << chosen.size() << "\n";
    cout << "  Суммарный вес   : " << totalW << " кг  из " << W << " кг\n";
    cout << "  Суммарная цен.  : " << totalV << " ₽\n\n";

    // Прогресс-бар заполненности
    cout << "  Заполнение рюкзака:\n  [";
    cout << string(filled, '#') << string(barTotal - filled, '.');
    cout << "]  " << totalW << "/" << W << " кг  ("
         << (W > 0 ? (int)round(100.0 * totalW / W) : 0) << "%)\n";

    cout << string(70, '-') << "\n";
    cout << "  Максимальная ценность (ответ): " << totalV << " ₽\n";
}

// ====================================================================
//  ЭКСПОРТ В CSV
// ====================================================================
void exportCSV(const vector<vector<int>>& dp,
               const vector<Item>& items,
               const vector<int>& chosen, int W) {
    int n = (int)items.size();

    // CSV 1: DP-таблица
    ofstream f1("knapsack_dp.csv");
    f1 << "i,name,w=0";
    for (int w = 1; w <= W; w++) f1 << "," << w;
    f1 << "\n";
    for (int i = 0; i <= n; i++) {
        f1 << i << "," << (i == 0 ? "" : items[i-1].name);
        for (int w = 0; w <= W; w++) f1 << "," << dp[i][w];
        f1 << "\n";
    }
    f1.close();
    cout << "  -> knapsack_dp.csv\n";

    // CSV 2: выбранные предметы
    ofstream f2("knapsack_result.csv");
    f2 << "chosen,name,weight,value\n";
    for (int i = 0; i < n; i++) {
        bool sel = find(chosen.begin(), chosen.end(), i) != chosen.end();
        f2 << (sel ? 1 : 0) << ","
           << items[i].name << ","
           << items[i].weight << ","
           << items[i].value << "\n";
    }
    f2.close();
    cout << "  -> knapsack_result.csv\n";
}

// ====================================================================
//  MAIN
// ====================================================================
int main() {
    cout << "======================================================\n";
    cout << "  ЗАДАЧА О РЮКЗАКЕ — динамическое программирование\n";
    cout << "======================================================\n\n";

    // ── Данные задачи ──────────────────────────────────────────────
    //    Чтобы изменить задачу — отредактируйте этот список
    //    и вместимость W ниже.
    vector<Item> items = {
        {"Ноутбук",    4, 1200},
        {"Камера",     3,  800},
        {"Книги",      5,  400},
        {"Телефон",    1,  900},
        {"Зарядка",    2,  300},
        {"Часы",       1,  600},
        {"Планшет",    3,  750},
        {"Наушники",   1,  500},
    };
    int W = 10;   // вместимость рюкзака (кг)

    // ── Вывод условия задачи ───────────────────────────────────────
    cout << "Вместимость рюкзака W = " << W << " кг\n\n";
    cout << left;
    cout << setw(4)  << "№"
         << setw(14) << "Предмет"
         << setw(10) << "Вес, кг"
         << setw(12) << "Ценность, ₽" << "\n";
    cout << string(40, '-') << "\n";
    for (int i = 0; i < (int)items.size(); i++)
        cout << setw(4)  << (i+1)
             << setw(14) << items[i].name
             << setw(10) << items[i].weight
             << setw(12) << items[i].value << "\n";

    // ── Решение ────────────────────────────────────────────────────
    cout << "\n[1] Заполняем DP-таблицу...\n";
    auto dp = solveKnapsack(items, W);

    cout << "[2] Обратный ход — восстанавливаем состав рюкзака...\n";
    auto chosen = traceback(dp, items, W);

    // ── Графический вывод ─────────────────────────────────────────
    printDPTable(dp, items, chosen, W);
    printItemsChart(items, chosen);
    printKnapsack(items, chosen, W);

    // ── Экспорт ────────────────────────────────────────────────────
    cout << "\n[3] Экспорт данных в CSV:\n";
    exportCSV(dp, items, chosen, W);

    cout << "\nГотово!\n";
    return 0;
}
