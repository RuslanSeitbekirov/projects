#include <iostream>
#include <vector>
#include <string>
#include <algorithm>
#include <sstream>
#include <set>

using namespace std;

int main() {
    int len_a, len_b;
    vector<string> S;
    
    // Читаем длины массивов
    cin >> len_a;
    cin.ignore(); // игнорируем символ новой строки после числа
    
    // Читаем первую строку со словами
    string line_a;
    getline(cin, line_a);
    
    cin >> len_b;
    cin.ignore(); // игнорируем символ новой строки после числа
    
    // Читаем вторую строку со словами
    string line_b;
    getline(cin, line_b);
    
    // Разбиваем строки на слова
    vector<string> mass_a;
    vector<string> mass_b;
    
    stringstream ss_a(line_a);
    string word;
    while (ss_a >> word) {
        mass_a.push_back(word);
    }
    
    stringstream ss_b(line_b);
    while (ss_b >> word) {
        mass_b.push_back(word);
    }
    
    // Используем set для автоматической сортировки и избежания дубликатов
    set<string> common_words;
    
    // Для эффективности используем set для быстрого поиска
    set<string> set_a(mass_a.begin(), mass_a.end());
    
    for (const string& w : mass_b) {
        if (set_a.count(w)) {
            common_words.insert(w);
        }
    }
    
    // Преобразуем set в vector для вывода
    S.assign(common_words.begin(), common_words.end());
    
    if (!S.empty()) {
        for (int i = 0; i < S.size(); i++) {
            cout << S[i];
            if (i != S.size()-1) cout << " ";
        }
        cout << endl;
    } else {
        cout << "-1" << endl; 
    }
    
    return 0;
}