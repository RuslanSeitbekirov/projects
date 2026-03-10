package main

import (
    "fmt"
    "math/rand"
    "time"
)

// В Go нет классов, только структуры с методами
// Структура Cat соответствует классу Cat в C++
type Cat struct {
    // В Go поля структуры public, если начинаются с заглавной буквы
    //приватные с маленькой (со строчной буквы)
    aliveState bool
}

// Конструктор как функция, возвращающая Cat
// В Go нет конструкторов как в C++, но принято использовать функции-конструкторы
func NewCat(isAlive bool) Cat {
    return Cat{
        aliveState: isAlive, // Инициализируем поле структуры
    }
}

// Метод для структуры Cat
// В Go методы объявляются отдельно от структуры
// (c Cat) - это "receiver", указывает к какой структуре принадлежит метод
func (c Cat) is_alive() bool {
    return c.aliveState // Просто возвращаем сохраненное состояние
}

// Структура Box соответствует классу Box в C++
type Box struct {
    // В Go нет полей по умолчанию, как в C++
    // Инициализацию делаем в конструкторе
}

// Конструктор для Box
func NewBox() Box {
    // Инициализируем генератор случайных чисел
    // В Go это делается один раз на уровне пакета, но для примера покажем здесь
	rand.Seed(time.Now().UnixNano())
    return Box{}
}

// Метод Open для структуры Box
func (b Box) open() Cat {
    // Генерируем случайное число от 0 до 1
    // В Go rand.Intn(n) генерирует число от 0 до n-1
    randomNumber := rand.Intn(2)
    
    // Определяем состояние кота
    catIsAlive := (randomNumber == 0)
    
    // Создаем и возвращаем кота
    return NewCat(catIsAlive)
}

func main() {
    // Создаем коробку
    var box Box
    
    // Открываем коробку и получаем кота
    cat := box.open()
    
    // Проверяем состояние кота
    if cat.is_alive() {
        fmt.Println("alive")
    } else {
        fmt.Println("dead")
    }
}