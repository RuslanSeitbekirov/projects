package main
import "fmt"

type Matrix struct{
    data [][]int
}

// Ваш код здесь
func NewMatrix(rows, cols, value int) Matrix {
    data := make([][]int, rows)
    for i := range data {
        data[i] = make([]int, cols)
        for j := range data[i] {
            data[i][j] = value
        }
    }
    return Matrix{data: data}
}

func (m *Matrix) Rows() int {
    return len(m.data)
}

func (m *Matrix) Cols() int {
    if m.Rows() == 0 {
        return 0
    }
    return len(m.data[0])
}

func (m *Matrix) Set(row, col, value int) {
    if row >= 0 && row < m.Rows() && col >= 0 && col < m.Cols() {
        m.data[row][col] = value
    }
}

func (m *Matrix) Get(row, col int) int {
    if row >= 0 && row < m.Rows() && col >= 0 && col < m.Cols() {
        return m.data[row][col]
    }
    return 0
}

func (m *Matrix) Convolution(core *Matrix) Matrix {
    rows := m.Rows() - core.Rows() + 1
    cols := m.Cols() - core.Cols() + 1
    
    if rows <= 0 || cols <= 0 {
        return NewMatrix(0, 0, 0)
    }
    
    result := NewMatrix(rows, cols, 0)
    
    for i := 0; i < rows; i++ {
        for j := 0; j < cols; j++ {
            sum := 0
            for ki := 0; ki < core.Rows(); ki++ {
                for kj := 0; kj < core.Cols(); kj++ {
                    sum += m.Get(i+ki, j+kj) * core.Get(ki, kj)
                }
            }
            result.Set(i, j, sum)
        }
    }
    
    return result
}

func (a *Matrix) Multiplication(b *Matrix) *Matrix {
    if a.Cols() != b.Rows() {
        return &Matrix{data: [][]int{}}
    }
    
    rows := a.Rows()
    cols := b.Cols()
    result := NewMatrix(rows, cols, 0)
    
    for i := 0; i < rows; i++ {
        for j := 0; j < cols; j++ {
            sum := 0
            for k := 0; k < a.Cols(); k++ {
                sum += a.Get(i, k) * b.Get(k, j)
            }
            result.Set(i, j, sum)
        }
    }
    
    return &result
}

func (m *Matrix) AddRow(value int) {
    newRow := make([]int, m.Cols())
    for i := range newRow {
        newRow[i] = value
    }
    m.data = append(m.data, newRow)
}

func (m *Matrix) AddCol(value int) {
    for i := range m.data {
        m.data[i] = append(m.data[i], value)
    }
}

func (m *Matrix) String() string {
    var result string
    for i := 0; i < m.Rows(); i++ {
        for j := 0; j < m.Cols(); j++ {
            result += fmt.Sprintf("%d", m.data[i][j])
            if j < m.Cols()-1 {
                result += " "
            }
        }
        if i < m.Rows()-1 {
            result += "\n"
        }
    }
    return result
}


func main() {
    var rowsFirst, colsFirst, firstInitVal,
        rowsCore, colsCore int
    
    fmt.Scan(&rowsFirst, &colsFirst, &firstInitVal)
    first := NewMatrix(rowsFirst, colsFirst, firstInitVal)
    for i := 0; i < first.Rows(); i++ {
        for j := 0; j < first.Cols(); j++ {
            var t int
            fmt.Scan(&t)
            first.Set(i, j, t)
        }
    }

    fmt.Scan(&rowsCore, &colsCore)
    core := NewMatrix(rowsCore, colsCore, 0)
    for i := 0; i < core.Rows(); i++ {
        for j := 0; j < core.Cols(); j++ {
            var t int
            fmt.Scan(&t)
            core.Set(i, j, t)
        }
    }
    
    conv := first.Convolution(&core)
    
    conv.AddRow(1)
    conv.AddCol(1)
    conv.AddCol(1)
    
    mult := conv.Multiplication(&first)
    
    fmt.Print(mult)
}