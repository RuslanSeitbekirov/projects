/*
====================================================================
ГЕНЕТИЧЕСКИЙ АЛГОРИТМ для задачи целочисленного ЛП
Вариант 4:
Максимизировать F = 2*x1 + x2
При ограничениях:
 0 <= x1 <= 3
 0 <= x2 <= 5
 x1,x2 in Z
Точный оптимум (перебор): F* = 11
Оптимальная точка: (3,5)
====================================================================
*/
#include <iostream>
#include <iomanip>
#include <vector>
#include <string>
#include <algorithm>
#include <random>
#include <cmath>
#include <set>
#include <numeric>

using namespace std;

// ====================================================================
//  ПАРАМЕТРЫ ЗАДАЧИ (Вариант 4)
// ====================================================================
const int X1_MIN = 0, X1_MAX = 3;
const int X2_MIN = 0, X2_MAX = 5;
const double PENALTY = 20.0;   // штраф за единицу нарушения (остался для совместимости)

// ====================================================================
//  ПАРАМЕТРЫ ГА
// ====================================================================
const int    POP_SIZE    = 24;
const int    GENERATIONS = 40;
const double P_CROSS     = 0.85;
const double P_MUT       = 0.20;
const int    ELITE       = 2;
const int    TOURNAMENT  = 3;
const int    SEED        = 42;

// ====================================================================
//  ЦЕЛЕВАЯ ФУНКЦИЯ И ОГРАНИЧЕНИЯ
// ====================================================================
int objective(int x1, int x2) {
    return 2 * x1 + x2;
}

bool is_feasible(int x1, int x2) {
    return (x1 >= X1_MIN) && (x1 <= X1_MAX) &&
           (x2 >= X2_MIN) && (x2 <= X2_MAX);
}

// Приспособленность: F - штраф за нарушения (максимизируем)
double fitness(int x1, int x2) {
    double val = static_cast<double>(objective(x1, x2));
    double penalty = 0.0;

    // Выход за границы x1
    if (x1 < X1_MIN) penalty += (X1_MIN - x1);
    if (x1 > X1_MAX) penalty += (x1 - X1_MAX);
    // Выход за границы x2
    if (x2 < X2_MIN) penalty += (X2_MIN - x2);
    if (x2 > X2_MAX) penalty += (x2 - X2_MAX);

    return val - PENALTY * penalty;
}

// ====================================================================
//  ОСОБЬ (хромосома)
// ====================================================================
struct Individual {
    int    x1, x2;
    double fit;
    bool   feasible;

    void evaluate() {
        fit      = fitness(x1, x2);
        feasible = is_feasible(x1, x2);
    }

    int fval() const { return objective(x1, x2); }

    string chromStr() const {
        return "[" + to_string(x1) + "," + to_string(x2) + "]";
    }
};

// ====================================================================
//  ВСПОМОГАТЕЛЬНЫЙ ВЫВОД
// ====================================================================
void hline(int w = 78, char c = '-') {
    cout << string(w, c) << "\n";
}

string fbar(double val, double lo, double hi, int width = 22) {
    if (hi <= lo) return string(width, '-');
    int fill = (int)((val - lo) / (hi - lo) * width + 0.5);
    fill = max(0, min(width, fill));
    return string(fill, '#') + string(width - fill, '.');
}

// ====================================================================
//  ВЫВОД ПОПУЛЯЦИИ
// ====================================================================
void printPopulation(const vector<Individual>& pop, const string& title, int gen = -1) {
    hline(78, '=');
    cout << "   " << title << "\n";
    hline(78);
    cout << setw(4)   << " № "
         << setw(10)  << "Хромос. "
         << setw(6)   << "x1 "
         << setw(6)   << "x2 "
         << setw(8)   << "F(x) "
         << setw(10)  << "Fitness "
         << setw(8)   << "Допуст "
         << "  Прогресс\n";
    hline(78);
    double lo = pop.back().fit, hi = pop.front().fit;
    for (size_t i = 0; i < pop.size(); i++) {
        const auto& ind = pop[i];
        cout << setw(4)   << (i + 1)
             << setw(10)  << ind.chromStr()
             << setw(6)   << ind.x1
             << setw(6)   << ind.x2
             << setw(8)   << ind.fval()
             << setw(10)  << fixed << setprecision(1) << ind.fit
             << setw(8)   << (ind.feasible ? "да " : "нет ")
             << "  | " << fbar(ind.fit, lo, hi) << "| ";
        if (i == 0) cout << "   <== BEST ";
        cout << "\n";
    }
    hline(78);
}

// ====================================================================
//  ИНИЦИАЛИЗАЦИЯ
// ====================================================================
Individual randomInd(mt19937& rng) {
    Individual ind;
    ind.x1 = uniform_int_distribution<int>(X1_MIN, X1_MAX)(rng);
    ind.x2 = uniform_int_distribution<int>(X2_MIN, X2_MAX)(rng);
    ind.evaluate();
    return ind;
}

vector<Individual> initPopulation(mt19937& rng) {
    vector<Individual> pop(POP_SIZE);
    for (auto& ind : pop) ind = randomInd(rng);
    return pop;
}

void sortPop(vector<Individual>& pop) {
    sort(pop.begin(), pop.end(),
         [](const Individual& a, const Individual& b) {
             return a.fit > b.fit;   // убывание: максимизируем
         });
}

// ====================================================================
//  ТУРНИРНАЯ СЕЛЕКЦИЯ
// ====================================================================
const Individual& tournamentSelect(const vector<Individual>& pop, mt19937& rng) {
    uniform_int_distribution<int> d(0, (int)pop.size() - 1);
    int best = d(rng);
    for (int i = 1; i < TOURNAMENT; i++) {
        int cand = d(rng);
        if (pop[cand].fit > pop[best].fit) best = cand;
    }
    return pop[best];
}

// ====================================================================
//  ОДНОТОЧЕЧНЫЙ КРОССОВЕР
// ====================================================================
pair<Individual, Individual> crossover(const Individual& p1, const Individual& p2) {
    Individual c1, c2;
    c1.x1 = p1.x1;  c1.x2 = p2.x2;
    c2.x1 = p2.x1;  c2.x2 = p1.x2;
    c1.evaluate();   c2.evaluate();
    return {c1, c2};
}

// ====================================================================
//  МУТАЦИЯ
// ====================================================================
void mutate(Individual& ind, mt19937& rng) {
    int gene = uniform_int_distribution<int>(0, 1)(rng);
    int type = uniform_int_distribution<int>(0, 2)(rng);
    if (gene == 0) {
        int lo = X1_MIN, hi = X1_MAX;
        if      (type == 0) ind.x1 = uniform_int_distribution<int>(lo, hi)(rng);
        else if (type == 1) ind.x1 = min(hi, ind.x1 + 1);
        else                ind.x1 = max(lo, ind.x1 - 1);
    } else {
        int lo = X2_MIN, hi = X2_MAX;
        if      (type == 0) ind.x2 = uniform_int_distribution<int>(lo, hi)(rng);
        else if (type == 1) ind.x2 = min(hi, ind.x2 + 1);
        else                ind.x2 = max(lo, ind.x2 - 1);
    }
    ind.evaluate();
}

// ====================================================================
//  ОДНО ПОКОЛЕНИЕ ГА
// ====================================================================
struct GenLog {
    vector<string> crossLog;
    vector<string> mutLog;
};

vector<Individual> nextGeneration(const vector<Individual>& pop, mt19937& rng, GenLog& log) {
    uniform_real_distribution<double> prob(0.0, 1.0);
    vector<Individual> newPop;
    newPop.reserve(POP_SIZE);

    for (int i = 0; i < ELITE; i++) newPop.push_back(pop[i]);

    while ((int)newPop.size() < POP_SIZE) {
        const Individual& p1 = tournamentSelect(pop, rng);
        const Individual& p2 = tournamentSelect(pop, rng);
        Individual c1 = p1, c2 = p2;

        if (prob(rng) < P_CROSS) {
            auto [nc1, nc2] = crossover(p1, p2);
            log.crossLog.push_back("  Кросс: " + p1.chromStr() + " x " + p2.chromStr() +
                                   " => " + nc1.chromStr() + ", " + nc2.chromStr());
            c1 = nc1; c2 = nc2;
        }

        if (prob(rng) < P_MUT) {
            string before = c1.chromStr();
            mutate(c1, rng);
            log.mutLog.push_back("  Мутац: " + before + " => " + c1.chromStr() +
                                 " fit= " + to_string((int)c1.fit));
        }
        if (prob(rng) < P_MUT) {
            string before = c2.chromStr();
            mutate(c2, rng);
            log.mutLog.push_back("  Мутац: " + before + " => " + c2.chromStr() +
                                 " fit= " + to_string((int)c2.fit));
        }

        newPop.push_back(c1);
        if ((int)newPop.size() < POP_SIZE) newPop.push_back(c2);
    }
    sortPop(newPop);
    return newPop;
}

// ====================================================================
//  ASCII-ГРАФИК сходимости
// ====================================================================
void printConvergence(const vector<double>& bestHist, const vector<double>& avgHist) {
    hline(78, '=');
    cout << "  ДИНАМИКА СХОДИМОСТИ\n";
    hline(78);
    double lo = *min_element(avgHist.begin(), avgHist.end());
    double hi = *max_element(bestHist.begin(), bestHist.end());
    cout << "  Ген  Лучший F  Средний F  График (лучший)\n";
    hline(60, '-');

    for (size_t i = 0; i < bestHist.size(); i++) {
        int barLen = (int)round((bestHist[i] - lo) / max(1.0, hi - lo) * 36);
        barLen = max(0, min(36, barLen));
        cout << setw(4)   << i
             << setw(9)   << fixed << setprecision(1) << bestHist[i]
             << setw(11)  << avgHist[i]
             << "  | " << string(barLen, '#')
             << string(36 - barLen, '.') << "|\n";
    }
    hline(60, '-');
    cout << "  (длиннее полоса = больше F)\n";
}

// ====================================================================
//  РАЗНООБРАЗИЕ ПОПУЛЯЦИИ
// ====================================================================
double diversity(const vector<Individual>& pop) {
    set<pair<int,int>> seen;
    for (auto& ind : pop) seen.insert({ind.x1, ind.x2});
    return 100.0 * seen.size() / pop.size();
}

void printGenLine(int gen, const vector<Individual>& pop, double globalBest, bool improved) {
    double avg = 0;
    int feasCnt = 0;
    for (auto& ind : pop) {
        avg += ind.fit;
        if (ind.feasible) feasCnt++;
    }
    avg /= pop.size();
    cout << setw(4)   << gen
         << setw(10)  << fixed << setprecision(1) << pop[0].fit
         << setw(10)  << avg
         << setw(8)   << feasCnt << "/ " << POP_SIZE
         << setw(7)   << (int)diversity(pop) << "% "
         << "  x=(" << pop[0].x1 << "," << pop[0].x2 << ") "
         << "  F= " << pop[0].fval()
         << (improved ? "  *** НОВЫЙ РЕКОРД *** " : " ")
         << "\n";
}

// ====================================================================
//  MAIN
// ====================================================================
int main() {
    mt19937 rng(SEED);

    hline(78, '=');
    cout << "  ГЕНЕТИЧЕСКИЙ АЛГОРИТМ — Вариант 4\n";
    cout << "  Целочисленное линейное программирование\n";
    hline(78, '=');

    cout << "\n  ЗАДАЧА:\n";
    cout << "    Максимизировать: F = 2*x1 + x2\n";
    cout << "    Ограничения:\n";
    cout << "      0 <= x1 <= 3,  0 <= x2 <= 5,  x1,x2 in Z\n";

    cout << "\n  ТОЧНЫЙ ОПТИМУМ (полный перебор): F* = 11\n";
    cout << "  Оптимальная точка: (3,5)\n";

    cout << "\n  КОДИРОВАНИЕ ХРОМОСОМЫ:\n";
    cout << "    [x1, x2] — прямое целочисленное кодирование\n";
    cout << "    x1 in {0..3},  x2 in {0..5}\n";
    cout << "    Нарушения ограничений штрафуются: fitness = F - " << PENALTY << "*нарушение\n";

    cout << "\n  ПАРАМЕТРЫ ГА:\n";
    cout << "    POP_SIZE    = " << POP_SIZE     << "\n";
    cout << "    GENERATIONS = " << GENERATIONS  << "\n";
    cout << "    P_CROSS     = " << P_CROSS      << "  (вероятность кроссовера)\n";
    cout << "    P_MUT       = " << P_MUT        << "  (вероятность мутации)\n";
    cout << "    ELITE       = " << ELITE        << "    (элитизм)\n";
    cout << "    TOURNAMENT  = " << TOURNAMENT   << "    (турнир)\n";
    cout << "    SEED        = " << SEED         << "\n";

    auto pop = initPopulation(rng);
    sortPop(pop);
    printPopulation(pop, "ШАГ 1: НАЧАЛЬНАЯ ПОПУЛЯЦИЯ (поколение 0) ");

    int feasCnt0 = count_if(pop.begin(), pop.end(), [](const Individual& i){ return i.feasible; });
    cout << "  Допустимых особей в начале: " << feasCnt0 << " из " << POP_SIZE << "\n";

    hline(78, '=');
    cout << "  ШАГ 2: ЭВОЛЮЦИЯ — " << GENERATIONS << " ПОКОЛЕНИЙ\n";
    hline(78, '=');
    cout << "\n  Ген  Лучш.fit  Сред.fit  Допуст.  Разнооб.  Лучшая точка\n";
    hline(78);

    Individual globalBest = pop[0];
    vector<double> bestHist, avgHist;
    {
        double avg = 0;
        for (auto& ind : pop) avg += ind.fit;
        avg /= pop.size();
        bestHist.push_back(pop[0].fit);
        avgHist.push_back(avg);
        printGenLine(0, pop, globalBest.fit, false);
    }

    for (int gen = 1; gen <= GENERATIONS; gen++) {
        GenLog log;
        pop = nextGeneration(pop, rng, log);

        bool improved = (pop[0].fit > globalBest.fit);
        if (improved) globalBest = pop[0];

        double avg = 0;
        for (auto& ind : pop) avg += ind.fit;
        avg /= pop.size();
        bestHist.push_back(pop[0].fit);
        avgHist.push_back(avg);

        printGenLine(gen, pop, globalBest.fit, improved);

        if (gen % 10 == 0 || improved) {
            printPopulation(pop, "Популяция — поколение " + to_string(gen) +
                            (improved ? " [НОВЫЙ РЕКОРД] " : " "), gen);

            cout << "\n  --- Операторы поколения " << gen << " ---\n";
            int shown = 0;
            for (auto& s : log.crossLog) if (shown++ < 5) cout << s << "\n";
            if ((int)log.crossLog.size() > 5) cout << "  ... ещё " << log.crossLog.size() - 5 << " кроссоверов\n";
            for (auto& s : log.mutLog) cout << s << "\n";
        }
    }

    printPopulation(pop, "ШАГ 3: ФИНАЛЬНАЯ ПОПУЛЯЦИЯ (поколение " + to_string(GENERATIONS) + ") ");
    printConvergence(bestHist, avgHist);

    hline(78, '=');
    cout << "  ШАГ 4: ОПТИМАЛЬНОЕ РЕШЕНИЕ\n";
    hline(78, '=');
    cout << fixed << setprecision(0);
    cout << "  x1* = " << globalBest.x1 << "\n";
    cout << "  x2* = " << globalBest.x2 << "\n";
    cout << "  F*  = " << globalBest.fval() << "\n";
    cout << "  Допустимая точка: " << (globalBest.feasible ? "ДА ✓" : "НЕТ ✗") << "\n";
    cout << "  Хромосома: " << globalBest.chromStr() << "\n\n";

    hline(78, '=');
    cout << "  СРАВНЕНИЕ С ТОЧНЫМ ОТВЕТОМ (полный перебор)\n";
    hline(78);
    cout << "  Точный оптимум: F* = 11\n";
    cout << "  Оптимальная точка: (3,5)\n\n";

    bool isOptimal = (globalBest.fval() == 11 && globalBest.feasible);
    bool isExactPoint = (globalBest.x1 == 3 && globalBest.x2 == 5);

    cout << "  ГА нашёл: F=" << globalBest.fval()
         << " точка=(" << globalBest.x1 << "," << globalBest.x2 << ")\n";
    cout << "  Значение F: " << (isOptimal ? "ОПТИМАЛЬНО ✓" : "НЕ ОПТИМАЛЬНО ✗") << "\n";
    cout << "  Точка: " << (isExactPoint ? "совпадает с оптимумом ✓" : "не совпадает") << "\n";

    hline(78);
    cout << "  Статистика:\n";
    cout << "    Поколений:        " << GENERATIONS << "\n";
    cout << "    ~Вычислений F():  " << (POP_SIZE * (GENERATIONS + 1)) << "\n";
    cout << "    Пространство:    4 * 6 = 24 точки\n";
    int finalFeasible = count_if(pop.begin(), pop.end(), [](const Individual& i){ return i.feasible; });
    cout << "    Допустимых в финальной популяции: " << finalFeasible << "/" << POP_SIZE << "\n";
    hline(78, '=');

    cout << "\nГотово!\n";
    return 0;
}