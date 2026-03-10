#include <iostream>
#include <vector>
#include <stack>
#include <string>

using namespace std;

int main() {
    stack<char> S;
    vector<char> line;
    string flag = "YES";
    
    string input;
    getline(cin, input, '!');
    for (char c : input) {
        line.push_back(c);
    }
    
    for (int i = 0; i < line.size(); i++) {
        if (line[i] == '(' || line[i] == '[' || line[i] == '{') {
            S.push(line[i]);
        } else if (line[i] == ')' || line[i] == ']' || line[i] == '}') {
            if (S.empty()) {
                flag = "NO";
                break;
            }
            
            if ((S.top() == '(' && line[i] == ')') ||
                (S.top() == '[' && line[i] == ']') ||
                (S.top() == '{' && line[i] == '}')) {
                S.pop();
            } else {
                flag = "NO";
                break;
            }
        }
    }
    
    if (!S.empty()) {
        flag = "NO";
    }
    
    
    cout << flag << endl;
    
}