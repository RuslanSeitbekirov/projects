#include <iostream>
#include <vector>
#include <string>
#include <map>
using namespace std;

int main() {

    vector<string> words;
    map<string, int> wordCount;
    
    int len_str;
    string str_m;
    
    cin >> len_str;
    
    for (int i = 0; i < len_str; i++) {
        cin >> str_m;
        words.push_back(str_m);
        wordCount[str_m]++;
    }
    
    // Находим максимальную частоту
    int maxCount = 0;
    for (const auto& [word, count] : wordCount) {
        if (count > maxCount) {
            maxCount = count;
        }
    }
    
    // Собираем все слова с максимальной частотой
    vector<string> mostFrequent;
    for (const auto& [word, count] : wordCount) {
        if (count == maxCount) {
            mostFrequent.push_back(word);
        }
    }

    for (size_t i = 0; i < mostFrequent.size(); i++) {
        cout << mostFrequent[i];
        if (i != mostFrequent.size() - 1) {
            cout << " ";
        }
    }
    // for (const auto& [word, count] : wordCount) {
    //     cout << word << ": " << count << endl;
}
