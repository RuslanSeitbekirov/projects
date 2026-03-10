package main

import (
	"fmt"
	"os"
)

func main() {
	envs := os.Environ() // чтобы получить слайс с переменными среды.
	for _, e := range envs {
		fmt.Println(e)
	}
}
