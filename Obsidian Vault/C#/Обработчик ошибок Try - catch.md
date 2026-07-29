
Нужно чтобы делать защиту от дебилов.
Структура:

```
try
{
	int num = Convert.ToInt32(Console.ReadLine());
	System.Console.WriteLine(num);
}
catch (<При какой ошибке выполняется>)
{
	System.Console.WriteLine("Uncorrect format!");
}
finally // выполнится в любом случае
{
	System.Console.WriteLine("I go kakat!");
}
```

Имя или код ошибки для catch можно брать из терминала при воспроизведении ошибки.


Пример кода:

```
try
{
	int a = 5, b = 0, res;
	Console.Write("Write first num:");
	a = Convert.ToInt32(Console.ReadLine());
	System.Console.Write("Write second num:");
	b = Convert.ToInt32(Console.ReadLine());
	res = a/b;
	System.Console.WriteLine("Result: " + res);
}
catch (System.DivideByZeroException) // ловит ошибку деления на ноль
{
	System.Console.WriteLine("You can't divide num on zero!!");
}
catch (FormatException) // ловит ошибку неверного формата ввода
{
	System.Console.WriteLine("Uncorrect format!");
}
finally
{
	System.Console.WriteLine("I go kakat!");
}
```