package main

import (
	"bufio"
	"fmt"
	"os"
	"sort"
	"strconv"
	"strings"
)

type PassInfo struct {
	PassengerId int
	Survived    int
	Pclass      int
	Name        string
	Sex         string
	Age         float64
	SibSp       int
	Parch       int
	Ticket      string
	Fare        float64
	Cabin       string
	Embarked    string
}

func make_struct(line string) PassInfo {
	var passenger PassInfo
	var tokens []string
	var token string
	in_quotes := false

	line = strings.TrimSuffix(line, "\r")

	for i := 0; i < len(line); i++ {
		c := line[i]
		if c == '"' {
			if in_quotes && i+1 < len(line) && line[i+1] == '"' {
				token = token + string('"')
				i++
			} else {
				in_quotes = !in_quotes
			}
		} else if c == ',' && !in_quotes {
			tokens = append(tokens, token)
			token = ""
		} else {
			token += string(c)
		}
	}
	tokens = append(tokens, token)

	if len(tokens) >= 12 {
		passenger.PassengerId, _ = strconv.Atoi(tokens[0])
		passenger.Survived, _ = strconv.Atoi(tokens[1])
		passenger.Pclass, _ = strconv.Atoi(tokens[2])
		passenger.Name = tokens[3]
		passenger.Sex = tokens[4]
		if tokens[5] != "" {
			passenger.Age, _ = strconv.ParseFloat(tokens[5], 64)
		}
		passenger.SibSp, _ = strconv.Atoi(tokens[6])
		passenger.Parch, _ = strconv.Atoi(tokens[7])
		passenger.Ticket = tokens[8]
		if tokens[9] != "" {
			passenger.Fare, _ = strconv.ParseFloat(tokens[9], 64)
		}
		passenger.Cabin = tokens[10]
		passenger.Embarked = tokens[11]
	}
	return passenger
}

func comparePassenger(a PassInfo, b PassInfo) bool {
	if a.Age == b.Age {
		return a.Name < b.Name
	}
	return a.Age < b.Age
}

func main() {
	data, _ := os.Open("train.csv")
	defer data.Close()

	scanner := bufio.NewScanner(data)

	var pclass int
	var age float64
	fmt.Scan(&pclass, &age)
	var arr []PassInfo

	// Пропускаем заголовок
	if scanner.Scan() {
		scanner.Text()
	}

	for scanner.Scan() {
		line := scanner.Text()
		passenger := make_struct(line)

		if passenger.Pclass == pclass && passenger.Age > age && passenger.Sex == "female" {
			arr = append(arr, passenger)
            fmt.Println(arr)
		}
	}

	// Сортировка
	sort.Slice(arr, func(i, j int) bool {
		return comparePassenger(arr[i], arr[j]) //Go автоматом передаёт индексы элементов для сравнения(i и j)
	})

	// Вывод результатов
	for _, passenger := range arr {
		fmt.Print(passenger.Name)
	}
}
