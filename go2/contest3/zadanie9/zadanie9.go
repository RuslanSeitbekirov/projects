package main
import "fmt"
import "sort"

func main(){
	var count_num, rez_num int
	var num int
    number := make([]int, count_num)
	fmt.Scan(&count_num, &rez_num)
	for i := 0; i < count_num; i++ {
	    fmt.Scan(&num)
        number = append(number, num)
	}
	sort.Ints(number)
    seen := make(map[int]bool)
    var found bool
    var num1, num2 int
    
    for _, num := range number {
        complement := rez_num - num
        if seen[complement] {
            num1, num2 = complement, num
            found = true
            break
        }
        seen[num] = true
    }
    
    if found {
        fmt.Printf("%d %d", num1, num2)
    } else {
        fmt.Println("0", "0")
    }
}