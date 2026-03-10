package main

import "fmt"

func FindIndex[T comparable](slice []T, target T) []int {
	index := make([] int, 0)
	for i:= 0; i < len(slice); i++{
		if slice[i] == target{
			index = append(index, i)
		}
	}

	return index
}

func main(){
    var count int
    fmt.Scan(&count)

    status  := make([]string, count)
    for i:=0; i<count; i++ {
        fmt.Scan(&status[i])
    }

    balance := make([]int, count)
    for i:=0; i<count; i++ {
        fmt.Scan(&balance[i])
    }    

    badUserIndex := FindIndex(status, "blocked")
    badUserIndex = append(badUserIndex, FindIndex(balance, 0)...)
    
    uniqueIndexes := make(map[int]bool)
    for i:=0; i<len(badUserIndex); i++ {
        uniqueIndexes[badUserIndex[i]] = true
    }

    fmt.Println(len(uniqueIndexes))
}