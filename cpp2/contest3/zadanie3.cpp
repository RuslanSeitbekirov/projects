#include <iostream>
#include <string>
#include <stack>
#include <vector>
#include <map>

using namespace std;


bool is_number(char element) {
    return (element >= '0' && element <= '9');
}


bool is_operator(char element) {
    return (element == '+' || element == '-' || element == '*' || 
            element == '/' || element == '%' || element == '^');
}


int get_priority(char op) {
    if (op == '^') return 3;
    if (op == '*' || op == '/' || op == '%') return 2;
    if (op == '+' || op == '-') return 1;
    return 0;
}


bool is_right_associative(char op) {
    return (op == '^');
}

int main() {
    string input;
    stack<char> S;
    string result = "";
    
    getline(cin, input);
    
    for (int i = 0; i < input.length(); i++) {
        char c = input[i];
        
        if (is_number(c)) {

            while (i < input.length() && is_number(input[i])) {
                result += input[i];
                i++;
            }
            result += ' ';
            i--; 
        }
        else if (is_operator(c)) {

            while (!S.empty() && S.top() != '(' && 
                   ((get_priority(S.top()) > get_priority(c)) ||
                    (get_priority(S.top()) == get_priority(c) && !is_right_associative(c)))) {
                result += S.top();
                result += ' ';
                S.pop();
            }
            S.push(c);
        }
        else if (c == '(') {
            S.push(c);
        }
        else if (c == ')') {

            while (!S.empty() && S.top() != '(') {
                result += S.top();
                result += ' ';
                S.pop();
            }
            if (!S.empty() && S.top() == '(') {
                S.pop(); 
            }
        }
    }
    

    while (!S.empty()) {
        result += S.top();
        result += ' ';
        S.pop();
    }

    if (!result.empty() && result.back() == ' ') {
        result.pop_back();
    }
    
    cout << result << endl;
    
    return 0;
}