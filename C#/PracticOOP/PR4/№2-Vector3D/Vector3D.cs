using System.Globalization;

namespace Vectors3D;

/// <summary>Трёхмерный вектор (X, Y, Z).</summary>
public readonly struct Vector3D
{   // из вне поля только для чтения
    public double X { get; }
    public double Y { get; }
    public double Z { get; }

    public Vector3D(double x, double y, double z)
    {
        X = x;
        Y = y;
        Z = z;
    } 

    // ---- Модуль вектора (свойство только для чтения) ----
    public double Length => Math.Sqrt(X * X + Y * Y + Z * Z);

    // ---- Сложение и вычитание ----
    public static Vector3D operator +(Vector3D a, Vector3D b)
        => new(a.X + b.X, a.Y + b.Y, a.Z + b.Z);

    public static Vector3D operator -(Vector3D a, Vector3D b)
        => new(a.X - b.X, a.Y - b.Y, a.Z - b.Z);

    // Унарный минус (нужен для формулы F = -2m(Ω × v))
    public static Vector3D operator -(Vector3D a) => new(-a.X, -a.Y, -a.Z);

    // ---- Умножение ----

    // Вектор * вектор = скалярное произведение (число)
    public static double operator *(Vector3D a, Vector3D b)
        => a.X * b.X + a.Y * b.Y + a.Z * b.Z;

    // Вектор * число (справа) и число * вектор (слева) = Vector3D 
    public static Vector3D operator *(Vector3D a, double k)
        => new(a.X * k, a.Y * k, a.Z * k);

    public static Vector3D operator *(double k, Vector3D a) => a * k; // вызовет предыдущую перегрузку но в другом порядке

    // ---- Деление ----

    // Вектор / число
    public static Vector3D operator /(Vector3D a, double k)
    {
        if (k == 0) throw new DivideByZeroException("Деление вектора на ноль");
        return new(a.X / k, a.Y / k, a.Z / k);
    }

    // Вектор / вектор = покомпонентное деление
    public static Vector3D operator /(Vector3D a, Vector3D b)
    {
        if (b.X == 0 || b.Y == 0 || b.Z == 0)
            throw new DivideByZeroException("Одна из компонент делителя равна нулю");
        return new(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
    }

    // ---- Векторное произведение a × b ----
    public static Vector3D Cross(Vector3D a, Vector3D b)
        => new(a.Y * b.Z - a.Z * b.Y,
               a.Z * b.X - a.X * b.Z,
               a.X * b.Y - a.Y * b.X);

    // ---- Сравнение ----
    public static bool operator ==(Vector3D a, Vector3D b)
        => a.X == b.X && a.Y == b.Y && a.Z == b.Z;

    public static bool operator !=(Vector3D a, Vector3D b) => !(a == b);

    public override bool Equals(object? obj) => obj is Vector3D v && this == v;

    public override int GetHashCode() => HashCode.Combine(X, Y, Z);

    // ---- Вывод ----
    public override string ToString()
        => string.Format(CultureInfo.CurrentCulture, "({0:0.######}; {1:0.######}; {2:0.######})", X, Y, Z);
}