using UnityEngine;
using System.Collections;

[System.Serializable]
public struct Circle
{
    public float x;
    public float y;
    public float r;
    public Circle(float A, float B, float D, float E, float F)
    {
        if (A * B != 0 && A * B != 1)
        {
            D /= A * B;
            E /= A * B;
            F /= A * B;
        }
        this.x = -0.5f * D;
        this.y = -0.5f * E;
        this.r = Mathf.Sqrt(x * x + y * y - F);
    }
    public Circle(float x, float y, float r)
    {
        this.x = x;
        this.y = y;
        this.r = r;
    }
    public Circle(float r)
    {
        x = 0;
        y = 0;
        this.r = r;
    }
    public Circle(float x, float y)
    {
        this.x = x;
        this.y = y;
        r = 1;
    }
    public float area
    {
        get
        {
            return Mathf.PI * r * r;
        }
    }
    public float perimeter
    {
        get
        {
            return 2 * Mathf.PI * r;
        }
    }
    public Vector2 center
    {
        get
        {
            return new Vector2(x, y);
        }
    }
    public Ellipse ellipse
    {
        get
        {
            return new Ellipse(x, y, r);
        }
    }
    public static bool operator ==(Circle lhs, Circle rhs){
        return lhs.x == rhs.x && lhs.y == rhs.y && lhs.r == rhs.r;
    }
    public static bool operator !=(Circle lhs, Circle rhs)
    {
        return !(lhs == rhs);
    }
    public static bool operator >(Circle lhs, Circle rhs)
    {
        return lhs.r > rhs.r;
    }
    public static bool operator <(Circle lhs, Circle rhs)
    {
        return lhs.r < rhs.r;
    }
    public static bool operator >=(Circle lhs, Circle rhs)
    {
        return lhs.r >= rhs.r;
    }
    public static bool operator <=(Circle lhs, Circle rhs)
    {
        return lhs.r <= rhs.r;
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
        return string.Format("({0}, {1}, {2})", x, y, r);
    }

    public bool Contains(Vector2 point)
    {
        return Vector2.Distance(point, center) <= r;
    }
    public bool Overlap(Circle other)
    {
        return Vector2.Distance(other.center, center) <= other.r + r;
    }

    public static float Distance(Circle circle, Vector2 point)
    {
        return Mathf.Abs(Vector2.Distance(point, circle.center) - circle.r);
    }
    public float Distance(Vector2 point)
    {
        return Mathf.Abs(Vector2.Distance(point, center) - r);
    }
}
