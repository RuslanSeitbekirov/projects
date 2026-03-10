#include <iostream>
#include <string>
#include <vector>
using namespace std;

template <typename T> T sum(const vector<T>& arr) {
    T result = T();
    for (const auto& item : arr) {
        result = result + item;
    }
    return result;
}

int main()
{
    int n;
    std::cin >> n;
    
    std::vector<int> vi(n);
    std::vector<double> vd(n);
    std::vector<std::string> vs(n);
    for (int i=0; i<n; i++){
        int item;
        std::cin >> item;
        vi[i] = item;
        vd[i] = item*1.5;
        vs[i] = std::to_string(item);
    }
    
    std::cout << sum(vi) << '\n'
              << sum(vd) << '\n'
              << sum(vs);
}