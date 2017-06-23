using UnityEngine;
using System.Collections;
using System.Reflection;

[System.Serializable]
public struct Complex {
    public double r;
    public double i;

    public static Complex zero = new Complex(0, 0);

    public Complex(double r, double i)
    {
        this.r = r;
        this.i = i;
    }
    public Complex(double r)
    {
        this.r = r;
        this.i = 0;
    }
    /* ============================== + ============================== */
    public static Complex operator +(Complex a, Complex b)
    {
        return new Complex(a.r + b.r, a.i + b.i);
    }
    public static Complex operator +(double a, Complex b)
    {
        return new Complex(a + b.r, b.i);
    }
    public static Complex operator +(Complex a, double b)
    {
        return new Complex(a.r + b, a.i);
    }
    /* ============================== - ============================== */
    public static Complex operator -(Complex a, Complex b)
    {
        return new Complex(a.r - b.r, a.i - b.i);
    }
    public static Complex operator -(double a, Complex b)
    {
        return new Complex(a - b.r, -b.i);
    }
    public static Complex operator -(Complex a, double b)
    {
        return new Complex(a.r - b, a.i);
    }
    /* ============================== * ============================== */
    public static Complex operator *(Complex a, Complex b)
    {
        return new Complex(a.r * b.r - a.i * b.i, a.r * b.i + a.i * b.r);
    }
    public static Complex operator *(double a, Complex b)
    {
        return new Complex(a * b.r, a * b.i);
    }
    public static Complex operator *(Complex a, double b)
    {
        return new Complex(a.r * b, a.i * b);
    }
    /* ============================== / ============================== */
    public static Complex operator /(Complex a, Complex b)
    {
        double m = b.r * b.r + b.i * b.i;
        if (m == 0) return Complex.zero;
        return new Complex((a.r * b.r + a.i * b.i) / m, (a.i * b.r - a.r * b.i) / m);
    }
    public static Complex operator /(double a, Complex b)
    {
        double m = b.r * b.r + b.i * b.i;
        if (m == 0) return Complex.zero;
        return new Complex(a * b.r / m, a * b.i / m);
    }
    public static Complex operator /(Complex a, double b)
    {
        if (b == 0) return Complex.zero;
        return new Complex(a.r / b, a.i / b);
    }
    /* ============================== Compare ============================== */
    public static bool operator ==(Complex a, Complex b)
    {
        return a.r == b.r && a.i == b.i;
    }
    public static bool operator ==(double a, Complex b)
    {
        return a == b.r && 0 == b.i;
    }
    public static bool operator ==(Complex a, double b)
    {
        return a.r == b && a.i == 0;
    }
    public static bool operator !=(Complex a, Complex b)
    {
        return !(a == b);
    }
    public static bool operator !=(double a, Complex b)
    {
        return !(a == b);
    }
    public static bool operator !=(Complex a, double b)
    {
        return !(a == b);
    }
    /* ============================== Math ============================== */
    public static Complex[] Sqrt(Complex c)
    {
        Complex[] result = new Complex[2];
        double p = System.Math.Sqrt(c.r * c.r + c.i * c.i);
        double r = System.Math.Sqrt(0.5f * (p + c.r));
        double i = System.Math.Sqrt(0.5f * (p - c.r));
        result[0] = new Complex(r, i);
        result[1] = new Complex(r, -i);
        return result;
    }
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
    public override string ToString()
    {
        if (r == 0) return string.Format("{0}i", i);
        else if (i == 0) return string.Format("{0}", r);
        else if (i < 0) return string.Format("{0} - {1}i", r, -i);
        return string.Format("{0} + {1}i", r, i);
    }
}
