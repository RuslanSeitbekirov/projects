package main

import (
    "encoding/json"
    "fmt"
    "os"
)

// Структуры для парсинга JSON
type Task struct {
    UserID    int    `json:"user_id"`
    ID        int    `json:"id"`
    Title     string `json:"title"`
    Completed bool   `json:"completed"`
}

type Project struct {
    ProjectID int    `json:"project_id"`
    Tasks     []Task `json:"tasks"`
}

func main() {
    // Чтение файла
    data, err := os.ReadFile("data.json") //в err записывается считался ли файл
    if err != nil { // надо чтобы err для чего то использовалось (иначе консоль ругается, Без иф ошибка: this value of err is never used (SA4006)) 
        fmt.Printf("Ошибка чтения файла: %v\n", err)
        return
    }

    // Парсинг JSON
    var projects []Project
    json.Unmarshal(data, &projects)
    // err = json.Unmarshal(data, &projects) // если надо
    // if err != nil { // Без иф ошибка: this value of err is never used (SA4006)
    //     fmt.Printf("Ошибка парсинга JSON: %v\n", err)
    //     return
    // }
    // Ввод ID пользователя
    var userID int
    fmt.Scan(&userID)

    // Подсчет завершенных задач
    completedCount := 0
    for _, project := range projects {
        for _, task := range project.Tasks {
            if task.UserID == userID && task.Completed {
                completedCount++
            }
        }
    }

    // Вывод результата
    fmt.Printf("%d", completedCount)
}