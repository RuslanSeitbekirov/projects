#include <iostream>

class IntSharedPointer{
    int* ptr;
    int* count;

public:

    IntSharedPointer(int* p = nullptr){
        ptr = p;
        count = new int(1);
        if (p == nullptr) {
           *count = 0;
        }   
    }

    IntSharedPointer(const IntSharedPointer& other){
        ptr = other.ptr;
        count = other.count;
        if ( ptr != nullptr){
            (*count)++;
        }
    }

    IntSharedPointer& operator=(IntSharedPointer other) {
        swap(other);
        return *this;
    }

    ~IntSharedPointer() {
        release();
    }

    int& operator*() const{
        return *ptr;
    }

    int* get() const {
        return ptr;
    }
private:
    void swap(IntSharedPointer& i) noexcept{
        std::swap(ptr, i.ptr);
        std::swap(count, i.count);
    }

    void release() {
        if (ptr != nullptr) {
            (*count)--;
            if(*count == 0){
                delete ptr;
                delete count;
            }
        }
    }
};

void print(IntSharedPointer p){
    std::cout << *p << std::endl;
}

void print_with_exception(IntSharedPointer p){
    std::cout << *p << std::endl;
    if (*p % 2) throw std::string("error");
}

int main(){
    IntSharedPointer p(new int);
    std::cin >> *p;
    p = p;
    print(p);

    IntSharedPointer p2(p);
    std::cin >> *p2;
    print(p);
    print(p2);
    
    {
        int value;
        std::cin >> value;
        IntSharedPointer p3(new int(value));
        p2 = p3;
    }
    print(p2);
    
    p = p2;
    try{
        print_with_exception(p);
    }
    catch (const std::string& e) {
        std::cout << e << std::endl;
    }
}