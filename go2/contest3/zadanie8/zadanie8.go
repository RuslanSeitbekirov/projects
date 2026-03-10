package main

import "fmt"

func main() {
	var lenMap int
	fmt.Scan(&lenMap)

	line := make([]string, lenMap)
	
	// Читаем слова в массив
	for i := 0; i < lenMap; i++ {
		fmt.Scan(&line[i])
	}

	// Создаем карту для подсчета количества слов
	unNum := make(map[string]int)
	
	// Добавляем слова в карту и считаем количество
	for _, number := range line {
		unNum[number]++ // Автоматически создает ключ со значением 0, если его нет
	}

	// Ищем первое слово с нечетным количеством повторений
	for i := 0; i < lenMap; i++ { // count значение по ключу, exists хранит bool есть ключь или нет, line[i] ключ
		if count, exists := unNum[line[i]]; exists && count % 2 != 0 {
			fmt.Print(line[i], " ")
			break
		}
	}
}