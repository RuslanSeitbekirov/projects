// #include <iostream>
// #include <vector>
// #include <functional>
// #include <cmath>

// class IComparator{
// public:
//     virtual bool compare(int a, int b) const = 0;
// };


// bool less(int a, int b){
//     return a < b;
// }

// void greater(int a, int b, bool& verdict){
//     verdict = a > b;
// }

// double key_func(int value){
//     return abs(value);
// }

// class LessAdapter: public IComparator{
//     using func = bool (*)(int a, int b);
//     func cmp;
// public:
//     LessAdapter(func cmp): cmp(cmp){
//     }
    
//     bool compare(int a, int b) const override {
//         return cmp(a, b);
//     }
// };

// // Ваш код здесь

// // Адаптер для функции greater
// class GreaterAdapter: public IComparator{
//     using func = void (*)(int a, int b, bool& verdict);
//     func cmp;
// public:
//     GreaterAdapter(func cmp): cmp(cmp){
//     }
    
//     bool compare(int a, int b) const override {
//         bool result;
//         cmp(a, b, result);
//         return result;
//     }
// };

// // Адаптер для функций-ключей
// class KeyFuncLessAdapter: public IComparator{
//     // Используем только std::function, он может хранить и указатели на функции, и лямбды
//     std::function<double(int)> key_func;
    
// public:
//     // Конструктор для указателей на функцию и лямбд
//     KeyFuncLessAdapter(const std::function<double(int)>& func): key_func(func) {
//     }
    
//     // Явный конструктор для указателей на функцию
//     KeyFuncLessAdapter(double (*func)(int)): key_func(func) {
//     }
    
//     bool compare(int a, int b) const override {
//         return key_func(a) < key_func(b);
//     }
// };

// void bubbleSort(std::vector<int>& arr, const IComparator& comp) {
//     for (int i = 0; i < arr.size() - 1; i++)
//         for (int j = 0; j < arr.size() - i - 1; j++)
//             if (comp.compare(arr[j], arr[j + 1]))
//                 std::swap(arr[j], arr[j+1]);
// }

// int main(){
//     int size;
//     std::cin >> size;
//     std::vector<int> arr(size);
//     for(auto& i : arr) std::cin >> i;
    
//     bubbleSort(arr, LessAdapter(less));
//     for(auto i : arr) std::cout << i << ' ';
//     std::cout << std::endl;
    
//     bubbleSort(arr, GreaterAdapter(greater));
//     for(auto i : arr) std::cout << i << ' ';
//     std::cout << std::endl;
    
//     bubbleSort(arr, KeyFuncLessAdapter(key_func));
//     for(auto i : arr) std::cout << i << ' ';
//     std::cout << std::endl;
    
//     bubbleSort(arr, KeyFuncLessAdapter([](int value)->double{
//         return abs(value) % 2;
//     }));
//     for(auto i : arr) std::cout << i << ' ';
// }