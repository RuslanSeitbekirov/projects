package main

import "fmt"

type Water struct{
	temperature int
}

type Teapot struct{
	water *Water
}

func NewWater(temp int) *Water{
	return &Water{temperature: temp}
}

func (w *Water) Temperature() int{
	return w.temperature
}

func (w *Water) heat_up(degrees int){
	 w.temperature += degrees
}

func (w *Water) is_boiling() bool{
	return w.temperature >= 100
}

func NewTeapot(water *Water) *Teapot{
	return &Teapot{water: water}
}

func (t *Teapot) heat_up(degrees int){
	t.water.heat_up(degrees)
}

func (t *Teapot) is_boiling() bool{
	return t.water.is_boiling()
}

func main(){
    var temperature, count int
    fmt.Scan(&temperature, &count)

    heat := NewQueue()
    for i := 0; i < count; i++ {
        var t int
        fmt.Scan(&t)
        heat.Push(t)
    }

    water := NewWater(temperature)
    teapot := NewTeapot(water)
    
    for !teapot.is_boiling() {
        teapot.heat_up(heat.Pop())
    }
    
    for !heat.IsEmpty() {
        fmt.Print(heat.Pop(), " ")
    }
}