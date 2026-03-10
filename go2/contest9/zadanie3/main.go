package main

import (
    "fmt"
)

type Stack interface{
    empty() bool
    push(r rune)
    top() rune
    pop() rune
}

// Ваш код здесь
type SimpleStack struct {
    elements []rune
}

func NewSimpleStack() Stack {
    return &SimpleStack{
        elements: make([]rune, 0),
    }
}

func (s *SimpleStack) empty() bool {
    return len(s.elements) == 0
}

func (s *SimpleStack) push(r rune) {
    s.elements = append(s.elements, r)
}

func (s *SimpleStack) top() rune {
    if s.empty() {
        return 0
    }
    return s.elements[len(s.elements)-1]
}

func (s *SimpleStack) pop() rune {
    if s.empty() {
        return 0
    }
    top := s.top()
    s.elements = s.elements[:len(s.elements)-1]
    return top
}


func bracketValidator(stack Stack, str string) bool{
    mapping := map[rune]rune{
        ')': '(',
        ']': '[',
        '}': '{',
    }

    for _, char := range str {
        if char == '(' || char == '[' || char == '{' {
            stack.push(char)
        } else if char == ')' || char == ']' || char == '}' {
            if stack.empty() || stack.top() != mapping[char] {
                return false
            }
            stack.pop()
        }
    }

    return stack.empty()
}

func main() {
    var input string
    fmt.Scan(&input)
    fmt.Println(bracketValidator(NewSimpleStack(), input))
}