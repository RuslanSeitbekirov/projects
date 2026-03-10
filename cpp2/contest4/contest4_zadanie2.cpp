#include <iostream>
#include <vector>
using namespace std;

vector<int> range(int start, int stop, int step){
    vector<int> nums;

    if (start == stop || step == 0){
        return nums;
    } 
    
    if ((start < stop && step < 0) || (start > stop && step > 0)) {
        return nums;
    }

    if (step > 0) {
        for (int i = start; i < stop; i += step){
            nums.push_back(i);
        }
    } else {
        for (int i = start; i > stop; i += step){
            nums.push_back(i);
        } 
    }

    return nums;
}

vector<int> range(int start, int stop){
    return range(start, stop, 1);
}

vector<int> range(int stop){
    return range(0, stop, 1);
}

long long sum(const std::vector<int>& nums){
    long long sum = 0;
    for(auto i : nums) sum += i;
    return sum;
}

int main()
{
    int start, stop, step;
    std::cin >> start >> stop >> step;
    
    std::cout << sum(range(start, stop, step)) << ' '
              << sum(range(start, stop)) << ' '
              << sum(range(stop));
}
