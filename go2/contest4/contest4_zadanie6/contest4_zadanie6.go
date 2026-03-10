package main

import (
	"fmt"
)

func sum(slice interface{}) interface{} {
    switch v := slice.(type) {
    case []int:
        total := 0
        for _, num := range v {
            total += num
        }
        return total
    case []string:
        result := ""
        for _, str := range v {
            result += str
        }
        return result
    default:
        return nil
    }
}


func main() {
	var n int
	fmt.Scan(&n)

	nums := make([]int, n)
	strings := make([]string, n)

	for i := range nums {
		fmt.Scan(&nums[i])
		strings[i] = fmt.Sprint(nums[i])
	}

	fmt.Println(sum(nums))
	fmt.Println(sum(strings))
}
