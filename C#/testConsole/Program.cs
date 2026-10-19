using System;
    double res = 1/2; 
for(int i = 1; i < 8; i++) 
{

    if(i%2!=0) res = res + Math.Pow(1/2, i); 
}

Console.WriteLine(res); // 1.234568E+08 — последние цифры потер