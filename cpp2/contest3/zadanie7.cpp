#include <fstream>
#include <iostream>
#include <string>
#include <vector>
#include <map>
#include <algorithm>
using namespace std;

int main() {
    ifstream file("data.txt");
    string target;
    cin >> target;

    vector<string> words;
    string word;
    map<string, int> wordCount;

    // Читаем все слова из файла
    while (file >> word && word != "stopword") {
        words.push_back(word);
    }

    // Ищем слова, идущие после target
    for (size_t i = 0; i + 1 < words.size(); i++) {
        if (words[i] == target) {
            wordCount[words[i + 1]]++;
        }
    }

    // Если нет подходящих слов
    if (wordCount.empty()) {
        cout << '-';
        return 0;
    }

    // Преобразуем map в вектор пар для сортировки
    vector<pair<string, int>> wordPairs;
    for (const auto& [w, count] : wordCount) {
        wordPairs.push_back({ w, count });
    }

    // Сортируем по частоте (убывание) и лексикографически (возрастание)
    sort(wordPairs.begin(), wordPairs.end(),
        [](const pair<string, int>& a, const pair<string, int>& b) {
            if (a.second != b.second) {
                return a.second > b.second; // сначала по частоте (убывание)
            }
            return a.first < b.first; // затем лексикографически (возрастание)
        });

    // Выводим до 3 слов
    int count = 0;
    for (size_t i = 0; i < wordPairs.size() && count < 3; i++) {
        if (count > 0) cout << " ";
        cout << wordPairs[i].first;
        count++;
    }

}