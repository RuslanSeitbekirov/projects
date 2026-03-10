#include <iostream>
#include <algorithm>
#include <string_view>
#include <vector>
#include <cstdlib>

using namespace std;

int main(int argc, char* argv[]){
    // Оборачиваем оргументы в string_view и помещаем в вектор
    vector<string_view> args(argc);
    for(int i=0; i<args.size(); i++) args[i] = argv[i];
    
    // Если среди аргументов есть --help или -h, выводим справку и выходим
    if (find(args.begin(), args.end(), "-h") != args.end() ||
        find(args.begin(), args.end(), "--help") != args.end()){
        cout << "Usage: sum VALUE...\n"
             << "Print sum of all passed integer VALUEs.\n\n"
             << "  -h, --help     display this help and exit" << endl;
        return 0;
    }
    
    int sum = 0;

    if (args.size() == 1){
        int value = 0;
        while (cin >> value) sum += value;
    }else {
        for(int i = 1; i < args.size(); i++){
            sum += atoi(args[i].data());
        }
    }

    cout << sum << endl;
}