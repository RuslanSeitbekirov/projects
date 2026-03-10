package main

import (
	"flag"
	"fmt"
	"os"
)

func main() {
	action := flag.String("action", "set", "")
	flag.Parse()
	name := flag.Arg(0)

	switch *action {
	case "look":
		{
			val, exists := os.LookupEnv(name)
			if !exists {
				fmt.Printf("%s not exist\n", name)
				return
			}
			fmt.Printf("%s = %s\n", name, val)
		}
	case "get":
		{
			fmt.Printf("%s = %s\n", name, os.Getenv(name))
		}
	case "set":
		{
			os.Setenv(name, flag.Arg(1))
			fmt.Printf("%s = %s\n", name, os.Getenv(name))
		}

	case "unset":
		{
			os.Unsetenv(name)
			val, exists := os.LookupEnv(name)
			if !exists {
				fmt.Printf("%s not exist\n", name)
				return
			}
			fmt.Printf("%s = %s\n", name, val)
		}
	default:
		{
			fmt.Println("action not in [look, get, set, unset]")
		}
	}
}