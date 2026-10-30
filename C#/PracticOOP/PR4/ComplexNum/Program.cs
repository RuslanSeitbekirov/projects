using System.Text;
using ComplexNumbers;

Console.OutputEncoding = Encoding.UTF8;   // чтобы корректно выводились знаки · и ·

var z1 = new Complex(2, 1);    // 2 + i
var z2 = new Complex(3, -1);   // 3 - i

Console.WriteLine($"z1 = {z1},  |z1| = {z1.Modulus:F4}");
Console.WriteLine($"z2 = {z2},  |z2| = {z2.Modulus:F4}");
Console.WriteLine();
 
// ---- Арифметика в прямоугольной форме ----
Console.WriteLine($"z1 + z2 = {z1 + z2}");
Console.WriteLine($"z1 - z2 = {z1 - z2}");
Console.WriteLine($"z1 * z2 = {z1 * z2}");
Console.WriteLine($"z1 / z2 = {z1 / z2}");
Console.WriteLine($"z1 == z2: {z1 == z2},  z1 != z2: {z1 != z2}");
Console.WriteLine();

// ---- Проверка: (2+i)*(3-i) через экспоненциальную форму ----
// Умножение в экспоненциальной форме: модули перемножаются, аргументы складываются.
var (r1, phi1) = z1.ToExponential();
var (r2, phi2) = z2.ToExponential();

Console.WriteLine($"z1 = {z1.ToExponentialString()}");
Console.WriteLine($"z2 = {z2.ToExponentialString()}");

double r = r1 * r2;
double phi = phi1 + phi2;
Console.WriteLine($"z1*z2 (эксп.) = {r:0.####}·e^(i·{phi:0.####})");

Complex product = Complex.FromExponential(r, phi);   // обратно в прямоугольную форму
Console.WriteLine($"z1*z2 (обратно) = {product}");
Console.WriteLine($"Совпадает с прямым умножением: {product.ApproxEquals(z1 * z2)}");
Console.WriteLine($"Точное сравнение (==) из-за погрешности double: {product == z1 * z2}");