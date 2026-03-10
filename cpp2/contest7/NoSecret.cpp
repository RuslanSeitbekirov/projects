#include <iostream>

class Keeper{
    int trash;
    int secret;
    
public:
    Keeper(){
        std::cin >> secret;
    }
};

int hack_it(Keeper& keeper) {
    return *((int*)(&keeper) + 1);
}

int main(){
    Keeper keeper;
    int secret = hack_it(keeper);
    std::cout << secret;
}

/*
Объяснение:
Расположение в памяти: В классе Keeper поля расположены последовательно:
    trash (смещение 0)
    secret (смещение sizeof(int))
    Приведение типов: &keeper получает адрес объекта, (int*) приводим к указателю на int, + 1 смещаемся на один int (к полю secret)
Разыменование: * получает значение по этому адресу
*/