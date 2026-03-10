package main

import (
	"fmt"
	"log"
	"os"

	"github.com/joho/godotenv"
)

func main() {
	err := godotenv.Load()
	if err != nil {
		log.Fatal("Error loading .env file")
	}

	apiURL := os.Getenv("API_URL")
	apiKey := os.Getenv("API_KEY")
	fmt.Println(apiURL)
	fmt.Println(apiKey)
}