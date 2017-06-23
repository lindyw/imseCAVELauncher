using UnityEngine;
using System.Collections;

[System.Serializable]
public struct Ellipse
{
    public float x;
    public float y;
    public float w;
    public float h;

    public Ellipse(float x, float y, float r)
    {
        this.x = x;
        this.y = y;
        this.w = r;
        this.h = r;
    }
    public Ellipse(float x, float y, float w, float h)
    {
        this.x = x;
        this.y = y;
        this.w = w;
        this.h = h;
    }
    public Ellipse(float w, float h)
    {
        this.x = 0;
        this.y = 0;
        this.w = w;
        this.h = h;
    }
    public Ellipse(float r)
    {
        this.x = 0;
        this.y = 0;
        this.w = r;
        this.h = r;
    }

    public float area
    {
        get 
        { 
            return Mathf.PI * w * h; 
        }
    }
    public float perimeter
    {
        get
        {
            return 2 * Mathf.PI * Mathf.Max(w, h) + 4 * Mathf.Abs(w - h);
        }
    }
    public Vector2 center
    {
        get
        {
            return new Vector2(x, y);
        }
    }
    public bool isCircle
    {
        get { return w == h; }
    }
    
    public static bool operator ==(Ellipse lhs, Ellipse rhs)
    {
        return lhs.x == rhs.x && lhs.y == rhs.y && lhs.w == rhs.w && lhs.h == rhs.h;
    }
    public static bool operator !=(Ellipse lhs, Ellipse rhs)
    {
        return !(lhs == rhs);
    }
    public static bool operator >(Ellipse lhs, Ellipse rhs)
    {
        return lhs.w * lhs.h > rhs.w * rhs.h;
    }
    public static bool operator <(Ellipse lhs, Ellipse rhs)
    {
        return lhs.w * lhs.h < rhs.w * rhs.h;
    }
    public static bool operator >=(Ellipse lhs, Ellipse rhs)
    {
        return lhs.w * lhs.h >= rhs.w * rhs.h;
    }
    public static bool operator <=(Ellipse lhs, Ellipse rhs)
    {
        return lhs.w * lhs.h <= rhs.w * rhs.h;
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
        return string.Format("({0}, {1}, {2}, {3}", x, y, w, h);
    }

    public bool Contains(Vector2 point)
    {
        return Mathf.Pow((point.x - x) / w, 2) + Mathf.Pow((point.y - y) / h, 2) < 1;
    }
    public Vector2[] Intersection(Vector2 point)
    {
        return Ellipse.Intersection(point, center, w, h);
    }
    public static Vector2[] Intersection(Vector2 point, Vector2 center, float width, float height)
    {
        float a = width;
        float b = height;
        float h = center.x;
        float k = center.y;
        Vector2[] interPoint = new Vector2[2];
        if (point.x == center.x)
        {
            interPoint[0] = new Vector2(h, k + b);
            interPoint[1] = new Vector2(h, k - b);
        }
        else if (point.y == center.y)
        {
            interPoint[0] = new Vector2(h + a, k);
            interPoint[1] = new Vector2(h - b, k);
        }
        else
        {
            float t = (point.y - k) / (point.x - h);
            float s = Mathf.Sqrt(a * a * b * b / (b * b + a * a * t * t));
            interPoint[0] = new Vector2(h + s, k + t * s);
            interPoint[1] = new Vector2(h - s, k - t * s);
        }
        return interPoint;
    }
    public Vector2[] VerticalPoints(Vector2 point)
    {
        return Ellipse.VerticalPoints(point, center, w, h);
    }
    public static Vector2[] VerticalPoints(Vector2 point, Vector2 center, float width, float height)
    {
        Vector2[] points = new Vector2[4];
        float h = center.x;
        float k = center.y;
        float p = point.x - h;
        float q = point.y - k;
        float a = width;
        float b = height;
        //if (point.x == center.x || point.y == center.y)
        //{
        //    points[0] = new Vector2(h, k + b);
        //    points[1] = new Vector2(h + a, k);
        //    points[2] = new Vector2(h - a, k);
        //    points[3] = new Vector2(h, k - b);
        //}
        //else
        //{
        float a2 = a * a;
        float b2 = b * b;
        float b4 = b2 * b2;
        float q2 = q * q;
        float d = a2 - b2;
        float d2 = d * d;
        float B = 2 * b2 * q / d;
        float C = b2 * ((a2 * p * p + b2 * q2) / d2 - 1);
        float D = -2 * b4 * q / d;
        float E = -b4 * b2 * q2 / d2;
        Complex[] ys = ECCommons.QuarticRoots(1, B, C, D, E);
        Complex[] xs = new Complex[ys.Length];
        for (int i = 0; i < xs.Length; i++)
        {
            if (ys[i].i == 0)
            {
                if (ys[i].r > b) ys[i].r = b;
                else if (ys[i].r < -b) ys[i].r = -b;
                //xs[i].r = a2 * p * ys[i].r / (d * ys[i].r + b2 * q);
                xs[i].r = a * System.Math.Sqrt(1 - ys[i].r * ys[i].r / b2);
                if (p < 0) xs[i].r *= -1;
                points[i] = new Vector2((float)xs[i].r + h, (float)ys[i].r + k);
            }
            else if (i > 0)
            {
                points[i] = points[0];
            }
            else if (a > b)
            {
                points[i] = new Vector2(h, k + b);
            }
            else
            {
                points[i] = new Vector2(h + a, k);
            }
        }
        //}
        return points;
    }
}
