#include <iostream>
#include <vector>
#include <stack>

using namespace std;

int main() {
    int len_a;
    int len_b;
    stack<int> s1, s2;
    vector<int> mass_a;
    vector<int> mass_b;
    vector<int> result;
    
    // Читаем длины массивов
    cin >> len_a;
    
    // Читаем элементы первого массива
    for (int i = 0; i < len_a; i++) {
        int num;
        cin >> num;
        mass_a.push_back(num);
    }
    
    cin >> len_b;

    // Читаем элементы второго массива
    for (int i = 0; i < len_b; i++) {
        int num;
        cin >> num;
        mass_b.push_back(num);
    }
    
    for (int i = len_a - 1; i >= 0; i--){
        s1.push(mass_a[i]);
    }

    for (int i = len_b - 1; i >= 0; i--){
        s2.push(mass_b[i]);
    }
    
    while (!s1.empty() && !s2.empty()){
        if (s1.top() <= s2.top()){
            result.push_back(s1.top());
            s1.pop();  // УДАЛЯЕМ элемент из стека
        } else {
            result.push_back(s2.top());
            s2.pop();  // УДАЛЯЕМ элемент из стека
        }
    }
    
    // Добавляем оставшиеся элементы из первого стека
    while (!s1.empty()) {
        result.push_back(s1.top());
        s1.pop();
    }
    
    // Добавляем оставшиеся элементы из второго стека
    while (!s2.empty()) {
        result.push_back(s2.top());
        s2.pop();
    }
    
    for (int i = 0; i < result.size(); i++){
        cout << result[i] << " ";
    }
    
    return 0;
}
    // cout << len_a << " ";
    // for (int num : mass_a) cout << num << " ";
    // cout << endl << len_b << " ";
    // for (int num : mass_b) cout << num << " ";
    // cout << endl;
    /*
push() — добавляет элемент в стек;
pop() — удаляет элемент из стека;
top() — позволяет получить значение вершины стека без её удаления;
size() — возвращает количество элементов в стеке;
empty() — возвращает true, если стек пуст, и false в противном случае.
    
    */