#include <iostream>
#include <vector>
#include <string>
#include <map>
using namespace std;

int main() {

    vector<string> result;
    map<string, string> Balans;
    
    int len_map;
    
    int len_result;
    string str_m2;
    
    cin >> len_map;
    cin.ignore(); 
    
    for (int i = 0; i < len_map; i++) {
        string line;
        getline(cin, line);
        
        // Разделяем строку по точке с запятой
        size_t pos = line.find(';'); // позиция ";"
        if (pos != string::npos) {
            string key = line.substr(0, pos); // делит строку от 0 до pos
            string value = line.substr(pos + 1); // делит строку от pos до конца
            Balans[key] = value;
        }
    }
    
    cin >> len_result;
    
    for (int i = 0; i < len_result; i++) {
        cin >> str_m2;
        result.push_back(str_m2);
    }
    
    for (int i = 0; i < len_result; i++){
        auto it = Balans.find(result[i]);
        if (it != Balans.end()) {
            cout << it->second << " "; // it->second выводит значение под ключём it; 
                                       // it->first выводит имя ключа;
        }
            
    }


}