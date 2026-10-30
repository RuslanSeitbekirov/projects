using System.Text;
using Vectors3D;

Console.OutputEncoding = Encoding.UTF8;

// ===== Проверка операций =====
var a = new Vector3D(1, 2, 3);
var b = new Vector3D(4, 5, 6);

Console.WriteLine($"a = {a}, |a| = {a.Length:F4}");
Console.WriteLine($"b = {b}, |b| = {b.Length:F4}");
Console.WriteLine($"a + b = {a + b}");
Console.WriteLine($"a - b = {a - b}");
Console.WriteLine($"a * b (скалярное) = {a * b}");
Console.WriteLine($"a / b (покомпонентно) = {a / b}");
Console.WriteLine($"a * 2 = {a * 2},  2 * a = {2 * a}");
Console.WriteLine($"a / 2 = {a / 2}");
Console.WriteLine($"a × b (векторное) = {Vector3D.Cross(a, b)}");
Console.WriteLine($"a == b: {a == b},  a == a: {a == new Vector3D(1, 2, 3)}");
Console.WriteLine();

// ===== Сила Кориолиса: студент 70 кг бежит на юг со скоростью 7 м/с в Симферополе =====
// Локальные оси: X — восток, Y — север, Z — вверх.
const double omegaEarth = 7.2921159e-5;            // угловая скорость Земли, рад/с
double latitude = 44.95 * Math.PI / 180;           // широта Симферополя, рад
double mass = 70;                                  // масса, кг

// Вектор угловой скорости Земли в локальных осях: Ω = Ω·(0, cos φ, sin φ)
var omega = new Vector3D(0, omegaEarth * Math.Cos(latitude), omegaEarth * Math.Sin(latitude));

// Скорость: на юг = в направлении, противоположном оси Y
var velocity = new Vector3D(0, -7, 0);

// F = -2m (Ω × v)
Vector3D force = -2 * mass * Vector3D.Cross(omega, velocity);

Console.WriteLine("Сила Кориолиса (Симферополь, 44,95° с. ш.):");
Console.WriteLine($"Ω = {omega} рад/с");
Console.WriteLine($"v = {velocity} м/с");
Console.WriteLine($"F = {force} Н");
Console.WriteLine($"|F| = {force.Length:F4} Н");
Console.WriteLine(force.X < 0 ? "Направление: на запад (вправо по ходу движения)" : "Направление: на восток");