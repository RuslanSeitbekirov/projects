// package main

// import (
// 	"flag"
// 	"fmt"
// 	"os"
// 	"strconv"
// )

// func main() {
// 	flag.Usage = func() {
// 		fmt.Fprintln(os.Stderr, "Usage: sum [options] -- VALUE...")
// 		fmt.Fprintln(os.Stderr, "Print sum of all passed integer VALUEs.\n")
// 		flag.PrintDefaults()
// 		fmt.Fprintln(os.Stderr, "  -h, --help\n        display this help and exit")
// 	}

// 	flag.Parse()

// 	// Получаем все позиционные аргументы в видей слайса
// 	posArgs := flag.Args()

// 	if len(posArgs) > 0{
// 		sum := 0
// 		for _, arg := range posArgs {
// 			val, _ := strconv.Atoi(arg)
// 			sum += val

// 		}
// 		fmt.Println(sum)
// 	}

// }
package main
import (
	"flag"
	"fmt"
	"bufio"
	"os"
	"strconv"
	"strings"
)

// Свой тип
type nums []string

func (n *nums) String() string {
	return fmt.Sprintf("%s", *n)
}

func (n *nums) Set(value string) error {
    // Валидируем значениеы
	_, err := strconv.Atoi(value)
	if err != nil {
		return fmt.Errorf("'%s' not a number", value)
	}

	*n = append(*n, value)
	return nil
}

func main() {
	flag.Usage = func() {
		fmt.Fprintln(os.Stderr, "Usage: sum [options] -- [VALUE...]")
		fmt.Fprintln(os.Stderr, "Print sum of all passed integer VALUEs.\n")
		flag.PrintDefaults()
		fmt.Fprintln(os.Stderr, "  -h, --help\n        display this help and exit")
	}

	s := flag.Bool("s", false, "Put data via std input")
	std := flag.Bool("std", false, "Put data via std input")

	var data nums
	flag.Var(&data, "num", "The integer number")

	flag.Parse()

	if *s || *std {
		// Заполняем data со стандартного ввода
		scanner := bufio.NewScanner(os.Stdin)
		for scanner.Scan() {
			line := scanner.Text()
			// Разбиваем строку на отдельные числа
			numbers := strings.Fields(line)
			for _, num := range numbers {
				// Валидируем и добавляем каждое число
				_, err := strconv.Atoi(num)
				if err != nil {
					fmt.Fprintf(os.Stderr, "Error: '%s' not a number\n", num)
					os.Exit(1)
				}
				data = append(data, num)
			}
		}
		if err := scanner.Err(); err != nil {
			fmt.Fprintf(os.Stderr, "Error reading from stdin: %v\n", err)
			os.Exit(1)
		}
	} else {
		// Добавляем позиционные аргументы
		data = append(data, flag.Args()...)
	}

	// Проверяем, что есть данные для обработки
	if len(data) == 0 {
		fmt.Fprintln(os.Stderr, "Error: no values provided")
		flag.Usage()
		os.Exit(1)
	}

	// Основной код сложения чисел
	sum := 0
	for _, numStr := range data {
		num, _ := strconv.Atoi(numStr)
		sum += num
	}

	fmt.Println(sum)
}