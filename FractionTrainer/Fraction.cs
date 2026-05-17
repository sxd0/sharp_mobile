using System;

namespace FractionTrainer;

public sealed class Fraction : IEquatable<Fraction>
{
    public int Numerator { get; }
    public int Denominator { get; }

    public Fraction(int numerator, int denominator)
    {
        if (denominator == 0)
        {
            throw new ArgumentException("Знаменатель не может быть нулем.", nameof(denominator));
        }

        var gcd = GreatestCommonDivisor(Math.Abs(numerator), Math.Abs(denominator));
        numerator /= gcd;
        denominator /= gcd;

        if (denominator < 0)
        {
            numerator = -numerator;
            denominator = -denominator;
        }

        Numerator = numerator;
        Denominator = denominator;
    }

    public bool Equals(Fraction? other)
    {
        return other is not null
            && Numerator == other.Numerator
            && Denominator == other.Denominator;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Fraction);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Numerator, Denominator);
    }

    public override string ToString()
    {
        return $"{Numerator}/{Denominator}";
    }

    private static int GreatestCommonDivisor(int left, int right)
    {
        while (right != 0)
        {
            var temp = right;
            right = left % right;
            left = temp;
        }

        return Math.Max(left, 1);
    }
}
