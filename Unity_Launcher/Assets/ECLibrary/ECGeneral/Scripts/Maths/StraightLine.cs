using UnityEngine;
using System.Collections;

[System.Serializable]
public struct StraightLine{
    public float A;
    public float B;
    public float C;
    public StraightLine(float A, float B, float C)
    {
        this.A = A;
        this.B = B;
        this.C = C;
    }
    public StraightLine(Vector2 start, Vector2 end)
    {
        A = start.y - end.y;
        B = end.x - start.x;
        C = start.x * end.y - end.x * start.y;
    }
    public StraightLine(Vector2 point, float slope)
    {
        A = slope;
        B = -1;
        C = point.y - A * point.x;
    }
    public float slope
    {
        get { return -A / B; }
    }

    public bool Contains(Vector2 point)
    {
        return A * point.x + B * point.y + C == 0;
    }
    public Vector2 Intersection(StraightLine other)
    {
        if (slope == other.slope) return new Vector2();
        float X = B * other.C - other.B * C;
        float Y = A * other.C - other.A * C;
        float Z = B * other.A - A * other.B;
        return new Vector2(X / Z, Y / Z);
    }
    public Vector2[] Intersection(Circle circle)
    {
        Vector2[] points = new Vector2[2];
        if (Distance(circle.center) > circle.r) return points;
        float y = slope;
        float d = -C / B;
        float m = 1 + y * y;
        float x = circle.x;
        float b = circle.y;
        float a = circle.r;
        a = d * d - 2 * b * d + x * x + b * b - a * a;
        x = 2 * b * y + 2 * x - 2 * d * y;
        a = (Mathf.Sqrt(x * x - 4 * a * m) + x) / (2 * m);
        b = d + a * y;
        x = x / m - a;
        y = d + x * y;        
        points[0] = new Vector2(a, b);               
        points[1] = new Vector2(x, y);
        return points;
    }
    public float Distance(Vector2 target)
    {
        return Mathf.Abs(A * target.x + B * target.y + C) / Mathf.Sqrt(A * A + B * B);
    }
    /// <summary>
    /// Calculate the shortest distance between a point and a finite stright line.
    /// <para> Example: Distance(new Vector2(1, 1), new Vector2(0, 0), new Vector2(0, 2)) => 1 </para>
    /// </summary>
    /// <param name="target"> The target point </param>
    /// <param name="start"> The start point of stright line </param>
    /// <param name="end"> The end point of stright line </param>
    public static float Distance(Vector2 target, Vector2 start, Vector2 end)
    {
        float a = start.y - end.y;
        float b = end.x - start.x;
        float c = start.x * end.y - end.x * start.y;
        float d = Mathf.Abs(a * target.x + b * target.y + c) / Mathf.Sqrt(a * a + b * b);
        float s1 = Vector2.Distance(target, start);
        float s2 = Vector2.Distance(target, end);
        float a1 = Mathf.Sqrt(s1 * s1 - d * d);
        float a2 = Mathf.Sqrt(s2 * s2 - d * d);
        float l = Vector2.Distance(start, end);
        if (a1 > l && a1 > l) d = s1 < s2 ? s1 : s2;
        else if (a1 > l) d = s2;
        else if (a2 > l) d = s1;
        return d;
    }
    /// <summary>
    /// Calculate the shortest distance between a point and a infinite stright line.
    /// <para> Example: Distance(new Vector2(1, 1), new Vector2(0, 0), 0) => 1 </para>
    /// </summary>
    /// <param name="target"> The target point </param>
    /// <param name="point"> A point on the stright line </param>
    /// <param name="slope"> The slope of the stright line </param>
    public static float Distance(Vector2 target, Vector2 point, float slope)
    {
        float a = slope;
        float b = -1;
        float c = point.y - a * point.x;
        return Mathf.Abs(a * target.x + b * target.y + c) / Mathf.Sqrt(a * a + b * b);
    }

    public override string ToString()
    {
        return string.Format("[{0}, {1}, {2}]", A, B, C);
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
