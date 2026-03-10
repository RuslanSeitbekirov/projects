// 14  В массиве из n элементов выбрать без повторений все элементы, встречающиеся более одного раза.
// package main

// import "fmt"

// func main() {
// 	var lenMap int
// 	fmt.Scan(&lenMap)
	
// 	numbers := make([]int, lenMap)
	
// 	// Читаем числа в массив
// 	for i := 0; i < lenMap; i++ {
// 		fmt.Scan(&numbers[i])
// 	}

// 	// Создаем карту для подсчета количества чисел
// 	// Ключи - числа (int), значения - количество повторений (int)
// 	countMap := make(map[int]int)
	
// 	// Добавляем числа в карту и считаем количество
// 	for _, number := range numbers {
// 		countMap[number]++ // Увеличиваем счетчик для этого числа
// 	}

// 	for num, count := range countMap {
// 		if count > 1 {
// 			fmt.Print(num, " ")
// 		}
// 	}
// 	fmt.Println()

// }

//6 В неубывающей последовательности из n элементов найти количество элементов, меньших заданного числа и напечатать их.

// package main
// import "fmt"

// func main() {
// 	var lenMap int
// 	var target int
// 	fmt.Scan(&lenMap)
// 	fmt.Scan(&target)

// 	// Создаем массив ЧИСЕЛ (int), а не строк
// 	numbers := make([]int, lenMap)
	
// 	// Читаем числа в массив
// 	for i := 0; i < lenMap; i++ {
// 		fmt.Scan(&numbers[i])
// 	}

// 	for i:= 0; i < len(numbers); i++{
// 		if target > numbers[i]{
// 			fmt.Println(numbers[i])
// 		}else {
// 			break;
// 		}
// 	}
// }


//18 Ввести одномерный массив из n элементов. Отсортировать массив по неубыванию (невозрастанию) методом прямого выбора.
// package main

// import "fmt"

// func main() {
//     var n int
//     fmt.Print("Введите количество элементов: ")
//     fmt.Scan(&n)
    
//     // Создаем и заполняем массив
//     arr := make([]int, n)
//     fmt.Print("Введите элементы массива: ")
//     for i := 0; i < n; i++ {
//         fmt.Scan(&arr[i])
//     }
    
//     fmt.Println("Исходный массив:", arr)
    
//     // Сортировка методом прямого выбора (по неубыванию)
//     for i := 0; i < n-1; i++ {
//         minIndex := i
//         // Ищем минимальный элемент в оставшейся части
//         for j := i + 1; j < n; j++ {
//             if arr[j] < arr[minIndex] {
//                 minIndex = j
//             }
//         }
//         // Меняем местами текущий элемент с минимальным
//         arr[i], arr[minIndex] = arr[minIndex], arr[i]
// 		fmt.Print(arr)
//     }
    
//     fmt.Println("Отсортированный массив (по неубыванию):", arr)
// }




// Тема 6 задание 6 По массиву X(n) построить матрицу А(n*n) найти максимумальную сумму елементов строк

package main

import (
    "fmt"
    "math"
)

func createMatrix(arr []float64, n int) [][]float64 {
    if n == 0 || len(arr) < n {
        return nil
    }
    
    // Создаем матрицу N×N
    matrix := make([][]float64, n)
    for i := range matrix {
        matrix[i] = make([]float64, n)
    }
    
    // Заполняем матрицу: строка i = arr[i] * каждый элемент массива
    for i := 0; i < n; i++ {
        for j := 0; j < n; j++ {
            matrix[i][j] = math.Abs(arr[i] * arr[j])
        }
    }
    
    return matrix
}

func main() {
    var n int
    fmt.Print("Введите количество элементов: ")
    fmt.Scan(&n)
    
    // Создаем и заполняем массив
    arr := make([]float64, n)
    fmt.Print("Введите элементы массива: ")
    for i := 0; i < n; i++ {
        fmt.Scan(&arr[i])
    }

    matrix := createMatrix(arr, n)
    if len(matrix) > 0{
        for i := range matrix{
            fmt.Println(matrix[i])
        }
    }
    max_sum := 0.0
    sum := 0.0
    for x := 0; x < len(matrix); x++{
        sum = 0
        for y := 0; y < len(matrix[x]); y++{
            sum += matrix[x][y]
        }

        if sum > max_sum {
            max_sum = sum
        }
    }
    
    fmt.Println("Максимальная сумма:", max_sum)


}

//ПР6 задание 18 Найти максимальный (минимальный) элемент среди тех строк матрицы A( n m ) , элементы которых упорядочены по неубыванию(невозрастанию).

// package main

// import (
// 	"fmt"
// 	"math"
// )

// func createMatrixFromArray(arr []float64, rows int) [][]float64 {
// 	if len(arr) == 0 || rows == 0 {
// 		return nil
// 	}

// 	cols := len(arr) / rows
// 	if rows*cols != len(arr) {
// 		panic("Количество элементов должно быть кратно количеству строк")
// 	}

// 	matrix := make([][]float64, rows)
// 	k := 0
// 	for i := range matrix {
// 		matrix[i] = make([]float64, cols)
// 		for j := range matrix[i] {
// 			matrix[i][j] = arr[k]
// 			k++
// 		}
// 	}
// 	return matrix
// }

// // Проверяет, упорядочена ли строка по возростанию
// func Increasing(row []float64) bool {
// 	for i := 1; i < len(row); i++ {
// 		if row[i] < row[i-1] {
// 			return false
// 		}
// 	}
// 	return true
// }

// // Проверяет, упорядочена ли строка по убыванию
// func Decreasing(row []float64) bool {
// 	for i := 1; i < len(row); i++ {
// 		if row[i] > row[i-1] {
// 			return false
// 		}
// 	}
// 	return true
// }


// // поиск максимального элемента строки
// func findMaxInOrderedRows(matrix [][]float64) (float64, bool) {
// 	if len(matrix) == 0 || len(matrix[0]) == 0 {
// 		return 0, false
// 	}

// 	maxVal := math.Inf(-1) // отрицательная бесконечность
// 	found := false

// 	for i := 0; i < len(matrix); i++ {
// 		// Проверяем, упорядочена ли строка по неубыванию или невозрастанию
// 		if Decreasing(matrix[i]) || Increasing(matrix[i]) {
// 			// Находим максимальный элемент в упорядоченной строке
// 			for j := 0; j < len(matrix[i]); j++ {
// 				if matrix[i][j] > maxVal {
// 					maxVal = matrix[i][j]
// 					found = true
// 				}
// 			}
// 		}
// 	}

// 	return maxVal, found
// }

// // поиск минимального элемента строки
// func findMinInOrderedRows(matrix [][]float64) (float64, bool) {
// 	if len(matrix) == 0 || len(matrix[0]) == 0 {
// 		return 0, false
// 	}

// 	minVal := math.Inf(1)
// 	found := false

// 	for i := 0; i < len(matrix); i++ {
// 		// Проверяем, упорядочена ли строка по неубыванию или невозрастанию
// 		if Decreasing(matrix[i]) || Increasing(matrix[i]) {
// 			// Находим минимальный элемент в упорядоченной строке
// 			for j := 0; j < len(matrix[i]); j++ {
// 				if matrix[i][j] < minVal {
// 					minVal = matrix[i][j]
// 					found = true
// 				}
// 			}
// 		}
// 	}

// 	return minVal, found
// }

// func main() {
// 	var n, rows int
// 	fmt.Print("Введите общее количество элементов: ")
// 	fmt.Scan(&n)
// 	fmt.Print("Введите количество строк: ")
// 	fmt.Scan(&rows)

// 	arr := make([]float64, n)
// 	fmt.Printf("Введите %d элементов массива: ", n)
// 	for i := 0; i < n; i++ {
// 		fmt.Scan(&arr[i])
// 	}

// 	matrix := createMatrixFromArray(arr, rows)

// 	// Проверяем каждую строку на упорядоченность
// 	fmt.Println("\nАнализ строк:")
// 	for i := 0; i < len(matrix); i++ {
// 		Dec := Decreasing(matrix[i])
// 		Inc := Increasing(matrix[i])
		
// 		var order string
// 		if Dec && Inc {
// 			order = "все элементы равны"
// 		} else if Inc {
// 			order = "неубывающая"
// 		} else if Dec {
// 			order = "невозрастающая"
// 		} else {
// 			order = "неупорядоченная"
// 		}
// 		fmt.Printf("Строка %d: %s\n", i, order)
// 	}

// 	// Находим максимальный элемент среди упорядоченных строк
//     // maxVal минимальное значение, maxFound (true/false) найдено или нет
// 	maxVal, maxFound := findMaxInOrderedRows(matrix)
// 	if maxFound {
// 		fmt.Printf("\nМаксимальный элемент в упорядоченных строках: %.2f\n", maxVal)
// 	} else {
// 		fmt.Println("\nНет упорядоченных строк")
// 	}

// 	// Находим минимальный элемент среди упорядоченных строк
//     // minVal минимальное значение, minFound (true/false) найдено или нет
// 	minVal, minFound := findMinInOrderedRows(matrix)
// 	if minFound {
// 		fmt.Printf("Минимальный элемент в упорядоченных строках: %.2f\n", minVal)
// 	} else {
// 		fmt.Println("Нет упорядоченных строк")
// 	}
// }
// почему вызов функции лучше чем хранение информации об упорядоченности строки? Инфа о строке хранится В ОЗУ => компьютеру не надо выделять на это память + 
// во время расщирения массива он перезаписывается в новом месте что также забивает ОЗУ.