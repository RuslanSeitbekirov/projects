using System;


int len;

len = Convert.ToInt16(System.Console.ReadLine());

int[,] sum = new int[len, 4];

int rows = sum.GetUpperBound(0) + 1;    // количество строк
int columns = sum.Length / rows;        // количество столбцов
// или так
// int columns = numbers.GetUpperBound(1) + 1;
 
for (int i = 0; i < rows; i++)
{
    for (int j = 0; j < columns; j++)
    {
        int num = Convert.ToInt16(Console.ReadLine());
        sum[i, j] = num;
    }
    Console.WriteLine();
}

bool[] flag = new bool[len];

for (int i = 0; i < rows; i++)
{
    if (sum[i, 0] == sum[i, 2] && sum[i, 1] == sum[i, 3] && sum[i, 0] == sum[i, 1] )
    {
        flag[i] = true;
    }
    else flag[i] = false;
}


for (int row = 0; row < len; row++)
{
    System.Console.WriteLine($"a = {sum[row, 0]}, b = {sum[row, 1]}, c = {sum[row, 2]}, d = {sum[row, 3]}, Итог = {flag[row]}");
}




