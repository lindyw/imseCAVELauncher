using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class ECPainting {
    public Texture2D texture;
    public Texture2D preview;
    public int total = -1;
    List<Color[]> pixels = new List<Color[]>();

    public int width
    {
        get { return texture.width; }
    }
    public int height
    {
        get { return texture.height; }
    }
    public int current
    {
        get;
        private set;
    }
    public int steps
    {
        get { return pixels.Count; }
    }
    public Color[] colors
    {
        get { return pixels[current]; }
    }
    public Color[] origin
    {
        get { return pixels[0]; }
    }

    public ECPainting(float width, float height)
    {
        Initializing(width, height);
        Color[] colors = Cover(Color.white, width, height);
        Apply(new Rect(0, 0, width, height), colors);
        Initialized();        
    }
    public ECPainting(Color color, float width, float height)
    {
        Initializing(width, height);
        Color[] colors = Cover(color, width, height);
        Apply(new Rect(0, 0, width, height), colors);
        Initialized();
    }
    public ECPainting(Color[] colors, float width, float height)
    {
        Initializing(width, height);
        Apply(new Rect(0, 0, width, height), colors);
        Initialized();
    }
    public ECPainting(Texture2D source, float width, float height)
    {
        Initializing(width, height);
        LoadTexture(source);
        Initialized();
    }
    public ECPainting(Texture2D source, Rect sourRect, Rect desRect)
    {
        Initializing(desRect.width, desRect.height);
        LoadTexture(source, sourRect, desRect);
        Initialized();
    }
    public ECPainting(Texture2D source, Rect desRect)
    {
        Initializing(desRect.width, desRect.height);
        LoadTexture(source, desRect);
        Initialized();
    }
    public ECPainting(Texture2D source)
    {
        Initializing(source.width, source.height);
        LoadTexture(source);
        Initialized();
    }

    /* ============================== Initialization ============================== */
    public void Initializing(float width, float height)
    {
        texture = new Texture2D((int)width, (int)height);
        preview = new Texture2D((int)width, (int)height);        
    }
    public void Initialized()
    {
        preview.SetPixels(Cover(Color.clear));
        current = 0;
        pixels.Clear();
        pixels.Add(texture.GetPixels());
        Done();
    }

    /* ============================== Get Rects ============================== */
    public static Rect DestRect(Rect destRect, Rect baseRect)
    {
        if (destRect.x < baseRect.x)
        {
            destRect.width -= baseRect.x - destRect.x;
            destRect.x = baseRect.x;
        }
        if (destRect.y < baseRect.y)
        {
            destRect.height -= baseRect.y - destRect.y;
            destRect.y = baseRect.y;
        }
        if (destRect.xMax > baseRect.xMax)
        {
            destRect.width -= destRect.xMax - baseRect.xMax;
        }
        if (destRect.yMax > baseRect.yMax)
        {
            destRect.height -= destRect.yMax - baseRect.yMax;
        }
        return destRect;
    }
    public static Rect DestRect(Rect destRect, Texture2D source)
    {
        return DestRect(destRect, new Rect(0, 0, source.width, source.height));
    }
    public static Rect SourRect(Rect sourRect, Rect destRect)
    {
        sourRect.width = destRect.width;
        sourRect.height = destRect.height;
        sourRect.x = destRect.x - sourRect.x;
        sourRect.y = destRect.y - sourRect.y;
        return sourRect;
    }

    /* ============================== Load Texture ============================== */
    public void LoadTexture(Texture2D source, Rect destRect, Rect sourRect)
    {
        destRect = DestRect(destRect, texture);
        sourRect = DestRect(sourRect, source);
        if (destRect.width > sourRect.width) destRect.width = sourRect.width;
        else sourRect.width = destRect.width;
        if (destRect.height > sourRect.height) destRect.height = sourRect.height;
        else sourRect.height = destRect.height;
        Color[] colors = source.GetPixels((int)sourRect.x, (int)sourRect.y, (int)sourRect.width, (int)sourRect.height);
        Apply(destRect, colors);
    }
    public void LoadTexture(Texture2D source, Rect desRect)
    {
        Rect sourRect = new Rect(0, 0, source.width, source.height);
        LoadTexture(source, sourRect, desRect);
    }
    public void LoadTexture(Texture2D source)
    {
        Rect sourRect = new Rect(0, 0, source.width, source.height);
        Rect desRect = new Rect(0, 0, width, height);
        LoadTexture(source, sourRect, desRect);
    }

    /* ============================== Save Texture ============================== */
    public void SaveTexture(string path)
    {
        ECFile file = new ECFile(path);
        if (!file.DirectoryExists()) file.CreateDirectory();
        byte[] pngFile = texture.EncodeToPNG();
        File.WriteAllBytes(file.Path(), pngFile);
    }
    public Texture2D LoadTexture(string path)
    {
        byte[] bytes = ECFile.ReadBytes(path);
        Texture2D target = new Texture2D(2, 2, TextureFormat.BGRA32, false);
        target.LoadImage(bytes);
        return target;
    }

    /* ============================== Scale Texture ============================== */
    public static Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
    {
        Texture2D result = new Texture2D(targetWidth, targetHeight, source.format, true);
        Color[] rpixels = result.GetPixels(0);
        float incX = (1.0f / (float)targetWidth);
        float incY = (1.0f / (float)targetHeight);
        for (int px = 0; px < rpixels.Length; px++)
        {
            rpixels[px] = source.GetPixelBilinear(incX * ((float)px % targetWidth), incY * ((float)Mathf.Floor(px / targetWidth)));
        }
        result.SetPixels(rpixels, 0);
        result.Apply();
        return result;
    }
    public static Texture2D ScaleTexture(Texture2D source, int percentage)
    {
        int width = (int)(source.width * 0.01f * percentage);
        int height = (int)(source.height * 0.01f * percentage);
        return ScaleTexture(source, width, height);
    }

    /* ============================== Set Colors ============================== */
    public static Color[] Cover(Color color, float width, float height)
    {
        Color[] colors = new Color[(int)(width * height)];
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = color;
        }
        return colors;
    }
    public Color[] Cover(Color color)
    {
        return Cover(color, width, height);
    }
    public Color[] Cover()
    {
        return Cover(Color.white, width, height);
    }
    public static Color Reverse(Color color)
    {
        Color c = Color.white - color;
        c.a = color.a;
        return c;
    }

    /* ============================== Apply ============================== */
    public void Apply(Rect destRect, Color[] colors)
    {
        texture.SetPixels((int)destRect.x, (int)destRect.y, (int)destRect.width, (int)destRect.height, colors);
        texture.Apply();
        Clean();
    }
    public void Apply(Color[] colors)
    {
        texture.SetPixels(0, 0, width, height, colors);
        texture.Apply();
        Clean();
    }
    public void Apply()
    {
        texture.Apply();
        Clean();
    }
    public void Preview()
    {
        preview.Apply();
    }
    public void Clean()
    {
        preview.SetPixels(Cover(Color.clear));
        preview.Apply();
    }
    public void Done()
    {
        if (current < pixels.Count - 1) pixels.RemoveRange(current + 1, pixels.Count - current - 1);
        if (total >= 0 && current > total)
        {
            pixels.RemoveRange(1, pixels.Count - total - 1);
            current = total;
        }
        current++;
        pixels.Add(texture.GetPixels());
    }

    /* ============================== Map ============================== */
    public void Map(int rx, int ry, int rw, int rh, Color[] colors, bool isCover)
    {
        if (rx < 0 || ry < 0 || rx + rw > width || ry + rh > height) return;
        if (!isCover) preview.SetPixels(rx, ry, rw, rh, colors);
        for (int y = ry; y < ry + rh; y++)
        {
            for (int x = rx; x < rx + rw; x++)
            {
                if (x + y * width < 0 || x + y * width >= this.colors.Length) continue;
                int i = (x - rx) + (y - ry) * rw;
                Color color = colors[i];
                Color bg;
                if (!isCover) bg = this.colors[x + y * width];
                else bg = texture.GetPixel(x, y);
                float r = color.r * color.a + bg.r * bg.a * (1 - color.a);
                float g = color.g * color.a + bg.g * bg.a * (1 - color.a);
                float b = color.b * color.a + bg.b * bg.a * (1 - color.a);
                float a = color.a + bg.a * (1 - color.a);
                colors[i] = new Color(r, g, b, a);
            }
        } 
        if (isCover) preview.SetPixels(rx, ry, rw, rh, colors);
        texture.SetPixels(rx, ry, rw, rh, colors);
    }
    public void Map(float rx, float ry, float rw, float rh, Color[] colors, bool isCover)
    {
        Map((int)rx, (int)ry, (int)rw, (int)rh, colors, isCover);
    }
    public void Map(int rx, int ry, int rw, int rh, Color color, bool isCover)
    {
        Map(rx, ry, rw, rh, Cover(color, rw, rh), isCover);
    }
    public void Map(float rx, float ry, float rw, float rh, Color color, bool isCover)
    {
        Map((int)rx, (int)ry, (int)rw, (int)rh, Cover(color, rw, rh), isCover);
    }
    public void Map(int rx, int ry, int rw, int rh, Color[] colors)
    {
        Map((int)rx, (int)ry, (int)rw, (int)rh, colors, false);
    }
    public void Map(float rx, float ry, float rw, float rh, Color[] colors)
    {
        Map((int)rx, (int)ry, (int)rw, (int)rh, colors, false);
    }
    public void Map(int rx, int ry, int rw, int rh, Color color)
    {
        Map(rx, ry, rw, rh, Cover(color, rw, rh), false);
    }
    public void Map(float rx, float ry, float rw, float rh, Color color)
    {
        Map((int)rx, (int)ry, (int)rw, (int)rh, Cover(color, rw, rh), false);
    }
    public void Map(int x, int y, Color color, bool isCover)
    {
        if (x + y * width < 0 || x + y * width >= colors.Length) return;
        if (!isCover) preview.SetPixel(x, y, color);
        Color bg;
        if (!isCover) bg = this.colors[x + y * width];
        else bg = texture.GetPixel(x, y);
        float r = color.r * color.a + bg.r * bg.a * (1 - color.a);
        float g = color.g * color.a + bg.g * bg.a * (1 - color.a);
        float b = color.b * color.a + bg.b * bg.a * (1 - color.a);
        float a = color.a + bg.a * (1 - color.a);
        color = new Color(r, g, b, a);
        if(isCover) preview.SetPixel(x, y, color);
        texture.SetPixel(x, y, color);
    }
    public void Map(float x, float y, Color color, bool isCover)
    {
        Map((int)x, (int)y, color, isCover);
    }
    public void Map(int x, int y, Color color)
    {
        Map(x, y, color, false);
    }
    public void Map(float x, float y, Color color)
    {
        Map((int)x, (int)y, color, false);
    }

    /* ============================== Action ============================== */
    public void Next()
    {
        if (current < pixels.Count - 1)
        {
            current++;
            Apply(colors);
        }
    }
    public void Previous()
    {
        if (current > 1)
        {
            current--;
            Apply(colors);
        }
    }
    public void Delete(int i)
    {
        if (pixels.Count > i)
        {
            pixels.RemoveAt(i);
            current--;
        }
    }
    public void Clear(int i)
    {
        if (pixels.Count > i) pixels.RemoveRange(i + 1, pixels.Count - i - 1);
        current = i;
        Apply(colors);
        Done();
    }
    public void Clear()
    {
        Clear(0);
    }
    /* ============================== Shapes ============================== */
    public static Vector2 LockPoint(Vector2 start, Vector2 end)
    {
        float degree = Mathf.Atan2(Mathf.Abs(start.y - end.y), Mathf.Abs((start.x - end.x))) * Mathf.Rad2Deg;
        float w = Mathf.Abs(start.x - end.x);
        float h = Mathf.Abs(start.y - end.y);
        if (w < h)
        {
            h = start.y > end.y ? -w : w;
            w = start.x > end.x ? -w : w;
        }
        else
        {
            w = start.x > end.x ? -h : h;
            h = start.y > end.y ? -h : h;
        }
        if (degree < 30) return new Vector2(end.x, start.y);
        else if (degree > 60) return new Vector2(start.x, end.y);
        return start + new Vector2(w, h);
    }
    /// <summary>
    /// Return a rectangle created by two points.
    /// <para> Example: TwoPointRect(new Vector2(10, 0), new Vector2(0, 8)) => new Rect(0, 0, 10, 8) </para>
    /// </summary>
    /// <param name="a"> Point A </param>
    /// <param name="b"> Point B </param>
    public static Rect TwoPointRect(Vector2 start, Vector2 end)
    {
        float x = Mathf.Min(start.x, end.x);
        float y = Mathf.Min(start.y, end.y);
        float w = Mathf.Abs(start.x - end.x);
        float h = Mathf.Abs(start.y - end.y);
        return new Rect(x, y, w, h);
    }
    /// <summary>
    /// Return a rectangle created by two points.
    /// <para> Example: TwoPointRect(new Vector2(10, 0), new Vector2(0, 8)) => new Rect(0, 0, 10, 8) </para>
    /// </summary>
    /// <param name="a"> Point A </param>
    /// <param name="b"> Point B </param>
    public static Rect TwoPointSquare(Vector2 start, Vector2 end)
    {
        float w = Mathf.Abs(start.x - end.x);
        float h = Mathf.Abs(start.y - end.y);
        if (w < h) h = w;
        else w = h;
        if (start.x > end.x) start.x -= w;
        if (start.y > end.y) start.y -= h;
        return new Rect(start.x, start.y, w, h);
    }
    /// <summary>
    /// Return an ellipse created by two points.
    /// <para> Example: TwoPointEllipse(new Vector2(10, 0), new Vector2(0, 8)) => new Ellipse(5, 4, 5, 4) </para>
    /// </summary>
    /// <param name="a"> Point A </param>
    /// <param name="b"> Point B </param>
    public static Ellipse TwoPointEllipse(Vector2 start, Vector2 end)
    {
        float w = Mathf.Abs(start.x - end.x) / 2.0f;
        float h = Mathf.Abs(start.y - end.y) / 2.0f;
        Vector2 m = (start + end) / 2;
        return new Ellipse(m.x, m.y, w, h);
    }
    /// <summary>
    /// Return a cricle created by two points.
    /// <para> Example: TwoPointCircle(new Vector2(10, 0), new Vector2(0, 8)) => new Circle(6, 4, 4) </para>
    /// </summary>
    /// <param name="a"> Point A </param>
    /// <param name="b"> Point B </param>
    public static Circle TwoPointCircle(Vector2 start, Vector2 end)
    {
        float w = Mathf.Abs(start.x - end.x) / 2.0f;
        float h = Mathf.Abs(start.y - end.y) / 2.0f;
        float r;
        if (w < h)
        {
            r = w;
            h = start.y > end.y ? -w : w;
            w = start.x > end.x ? -w : w;
        }
        else
        {
            r = h;
            w = start.x > end.x ? -h : h;
            h = start.y > end.y ? -h : h;
        }
        Vector2 m = start + new Vector2(w, h);        
        return new Circle(m.x, m.y, r);
    }

    /* ============================== Draw ============================== */
    public void Draw(Vector2 start, Vector2 end, ECBrush brush)
    {
        if (brush.size <= 1)
        {
            Line(start, end, brush.color);
            return;
        }
        brush.size += 1;
        if (end == start) end.y++;
        Rect rect = new Rect(Mathf.Min(start.x, end.x) - brush.size, Mathf.Min(start.y, end.y) - brush.size, Mathf.Abs(start.x - end.x) + brush.size * 2, Mathf.Abs(start.y - end.y) + brush.size * 2);
        rect = DestRect(rect, texture);
        int rx = (int)rect.x;
        int ry = (int)rect.y;
        int rw = (int)rect.width;
        int rh = (int)rect.height;
        if (rx < 0 || ry < 0 || rw < 0 || rh < 0)   return;
        //Color[] colors = preview.GetPixels(rx, ry, rw, rh);
        //Color[] colors = new Color[rw * rh];
        //for (int i = 0; i < colors.Length; i++) colors[i] = Color.clear;
        for (int y = ry; y < ry + rh; y++)
        {
            for (int x = rx; x < rx + rw; x++)
            {
                Vector2 p = new Vector2(x, y);
                float d = StraightLine.Distance(p, start, end);
                if (d > brush.size)
                {
                    continue;
                }
                else if (d <= brush.size)
                {
                    int i = (x - rx) + (y - ry) * rw;
                    float a = (brush.size - d) / (brush.size * (1 - brush.hardness));
                    a = d / (float)brush.size <= brush.hardness ? brush.color.a : a * brush.color.a;
                    if (preview.GetPixel(x, y).a < a) //colors[i].a < a)
                    {
                        Color c = brush.color;
                        if (brush.tool == ECBrush.Tool.Eraser) c = origin[x + y * width];
                        if (d >= brush.size - 1) c.a = 0;
                        else c.a = a;
                        Map(x, y, c);
                        //colors[i] = c;
                        //Map(x, y, colors[i]);
                    }
                }
            }
        }
        //Map(rx, ry, rw, rh, colors);
    }
    public void Draw(Vector2 point, ECBrush brush)
    {
        Draw(point, point + new Vector2(0, 1), brush);
    }
    public void Draw(float xs, float ys, float xe, float ye, ECBrush brush)
    {
        Draw(new Vector2(xs, ys), new Vector2(xe, ye), brush);
    }
    public void Draw(float x, float y, ECBrush brush)
    {
        Draw(new Vector2(x, y), new Vector2(x, y), brush);
    }
    public void Draw(float x, float y, Color color)
    {
        Line(new Vector2(x, y), new Vector2(x, y), color);
    }

    /* ============================== Line ============================== */
    public void Line(Vector2 start, Vector2 end, ECBrush brush)
    {
        if (start != end)
        Draw(start, end, brush);
    }
    public void Line(float xs, float ys, float xe, float ye, ECBrush brush)
    {
        Line(new Vector2(xs, ys), new Vector2(xe, ye), brush);
    }

    public void Line(Vector2 start, Vector2 end, Color color, float dotDist, bool reverse)
    {
        Rect rect = new Rect(Mathf.Min(start.x, end.x), Mathf.Min(start.y, end.y), Mathf.Abs(start.x - end.x), Mathf.Abs(start.y - end.y));
        rect = DestRect(rect, texture);
        int rx = (int)rect.x;
        int ry = (int)rect.y;
        int rw = (int)rect.width;
        int rh = (int)rect.height;
        if (rx < 0 || ry < 0 || rw < 0 || rh < 0) return;
        float dx = (float)Mathf.Abs(end.x - start.x);
        float dy = (float)Mathf.Abs(end.y - start.y);
        float movement = 0;
        float tmp = 0;
        int i = dx >= dy ? 0 : 1;
        int[] c = {(int)start.x, (int)start.y };
        if (dx >= dy && dy > 0) movement = dx / dy;
        else if (dx > 0) movement = dy / dx;
        while (c[i] != end[i])
        {
            if (c[0] >= rx && c[0] <= rx + rw && c[1] >= ry && c[1] <= ry + rh && 
                (dotDist <= 0 || dotDist > 0 && (int)(Vector2.Distance(start, new Vector2(c[0], c[1])) / dotDist) % 2 == 0))
            {
                if(!reverse) Map(c[0], c[1], color);
                else Map(c[0], c[1], Reverse(texture.GetPixel(c[0], c[1])));
            }
            c[i] += c[i] < end[i] ? 1 : -1;
            if (movement > 0)
            {
                tmp += c[1 - i] < end[1 - i] ? 1 : -1;
                c[1 - i] = (int)start[1 - i] + (int)(tmp / movement);
            }
        }
        if (!reverse) Map(end.x, end.y, color);
        else Map(end.x, end.y, Reverse(texture.GetPixel((int)end.x, (int)end.y)));
    }
    public void Line(Vector2 start, Vector2 end, Color color, bool reverse)
    {
        Line(start, end, color, 0, reverse);
    }
    public void Line(float xs, float ys, float xe, float ye, Color color, float dotDist, bool reverse)
    {
        Line(new Vector2(xs, ys), new Vector2(xe, ye), color, dotDist, reverse);
    }
    public void Line(float xs, float ys, float xe, float ye, Color color, bool reverse)
    {
        Line(new Vector2(xs, ys), new Vector2(xe, ye), color, 0, reverse);
    }

    public void Line(Vector2 start, Vector2 end, Color color, float dotDist)
    {
        Line(start, end, color, dotDist, false);
    }
    public void Line(Vector2 start, Vector2 end, Color color)
    {
        Line(start, end, color, 0, false);
    }
    public void Line(float xs, float ys, float xe, float ye, Color color, float dotDist)
    {
        Line(new Vector2(xs, ys), new Vector2(xe, ye), color, dotDist, false);
    }
    public void Line(float xs, float ys, float xe, float ye, Color color)
    {
        Line(new Vector2(xs, ys), new Vector2(xe, ye), color, 0, false);
    }

    public void Line(Vector2 start, Vector2 end, float dotDist)
    {
        Line(start, end, Color.clear, dotDist, true);
    }
    public void Line(Vector2 start, Vector2 end)
    {
        Line(start, end, Color.clear, 0, true);
    }
    public void Line(float xs, float ys, float xe, float ye, float dotDist)
    {
        Line(new Vector2(xs, ys), new Vector2(xe, ye), Color.clear, dotDist, true);
    }
    public void Line(float xs, float ys, float xe, float ye)
    {
        Line(new Vector2(xs, ys), new Vector2(xe, ye), Color.clear, 0, true);
    }

    /* ============================== Rect ============================== */
    public void StrokeRect(Rect rect, ECBrush brush)
    {
        if (brush.size <= 1)
        {
            StrokeRect(rect, brush.color);
            return;
        }
        int rx = (int)rect.x;
        int ry = (int)rect.y;
        int rw = (int)rect.width;
        int rh = (int)rect.height;
        if (rw < 1 || rh < 1) return;
        Draw(rx, ry, rx + rw, ry, brush);
        Draw(rx, ry + rh, rx + rw, ry + rh, brush);
        Draw(rx, ry, rx, ry + rh, brush);
        Draw(rx + rw, ry, rx + rw, ry + rh, brush);
    }
    public void StrokeRect(Vector2 point, int size, ECBrush brush)
    {
        Rect rect = new Rect(point.x - size, point.y - size, size * 2, size * 2);
        StrokeRect(rect, brush);
    }
    public void StrokeRect(Vector2 start, Vector2 end, ECBrush brush)
    {
        Rect rect = TwoPointRect(start, end);
        StrokeRect(rect, brush);
    }

    public void StrokeRect(Rect rect, Color color, float dotDist, bool reverse)
    {
        int rx = (int)rect.x;
        int ry = (int)rect.y;
        int rw = (int)rect.width;
        int rh = (int)rect.height;
        if (rw < 1 || rh < 1) return;
        if (rect.y >= 0) Line(rx, ry, rx + rw, ry, color, dotDist, reverse);    //Map(rx, ry, rw, 1, color);
        if (rect.x >= 0) Line(rx, ry, rx, ry + rh, color, dotDist, reverse);    //Map(rx, ry, 1, rh, color);
        if (rect.yMax < height) Line(rx, ry + rh, rx + rw, ry + rh, color, dotDist, reverse);   //Map(rx, ry + rh - 1, rw, 1, color);
        if (rect.xMax < width) Line(rx + rw, ry, rx + rw, ry + rh, color, dotDist, reverse);    //Map(rx + rw - 1, ry, 1, rh, color);
    }
    public void StrokeRect(Rect rect, Color color, bool reverse)
    {
        StrokeRect(rect, color, 0, reverse);
    }
    public void StrokeRect(Vector2 point, int size, Color color, float dotDist, bool reverse)
    {
        Rect rect = new Rect(point.x - size, point.y - size, size * 2, size * 2);
        StrokeRect(rect, color, dotDist, reverse);
    }
    public void StrokeRect(Vector2 point, int size, Color color, bool reverse)
    {
        Rect rect = new Rect(point.x - size, point.y - size, size * 2, size * 2);
        StrokeRect(rect, color, 0, reverse);
    }
    public void StrokeRect(Vector2 start, Vector2 end, Color color, float dotDist, bool reverse)
    {
        Rect rect = TwoPointRect(start, end);
        StrokeRect(rect, color, dotDist, reverse);
    }
    public void StrokeRect(Vector2 start, Vector2 end, Color color, bool reverse)
    {
        Rect rect = TwoPointRect(start, end);
        StrokeRect(rect, color, 0, reverse);
    }

    public void StrokeRect(Rect rect, Color color, float dotDist)
    {
        StrokeRect(rect, color, dotDist, false);
    }
    public void StrokeRect(Rect rect, Color color)
    {
        StrokeRect(rect, color, 0, false);
    }
    public void StrokeRect(Vector2 point, int size, Color color, float dotDist)
    {
        Rect rect = new Rect(point.x - size, point.y - size, size * 2, size * 2);
        StrokeRect(rect, color, dotDist, false);
    }
    public void StrokeRect(Vector2 point, int size, Color color)
    {
        Rect rect = new Rect(point.x - size, point.y - size, size * 2, size * 2);
        StrokeRect(rect, color, 0, false);
    }
    public void StrokeRect(Vector2 start, Vector2 end, Color color, float dotDist)
    {
        Rect rect = TwoPointRect(start, end);
        StrokeRect(rect, color, dotDist, false);
    }
    public void StrokeRect(Vector2 start, Vector2 end, Color color)
    {
        Rect rect = TwoPointRect(start, end);
        StrokeRect(rect, color, 0, false);
    }

    public void StrokeRect(Rect rect, float dotDist)
    {
        StrokeRect(rect, Color.clear, dotDist, true);
    }
    public void StrokeRect(Rect rect)
    {
        StrokeRect(rect, Color.clear, 0, true);
    }
    public void StrokeRect(Vector2 point, int size, float dotDist)
    {
        Rect rect = new Rect(point.x - size, point.y - size, size * 2, size * 2);
        StrokeRect(rect, Color.clear, dotDist, true);
    }
    public void StrokeRect(Vector2 point, int size)
    {
        Rect rect = new Rect(point.x - size, point.y - size, size * 2, size * 2);
        StrokeRect(rect, Color.clear, 0, true);
    }
    public void StrokeRect(Vector2 start, Vector2 end, float dotDist)
    {
        Rect rect = TwoPointRect(start, end);
        StrokeRect(rect, Color.clear, dotDist, true);
    }
    public void StrokeRect(Vector2 start, Vector2 end)
    {
        Rect rect = TwoPointRect(start, end);
        StrokeRect(rect, Color.clear, 0, true);
    }

    public void FillRect(Rect rect, Color color)
    {
        Rect desRect = DestRect(rect, texture);
        int rx = (int)desRect.x;
        int ry = (int)desRect.y;
        int rw = (int)desRect.width;
        int rh = (int)desRect.height;
        if (rw < 1 || rh < 1) return;
        Map(rx, ry, rw, rh, color);
    }
    public void FillRect(Vector2 point, int size, Color color)
    {
        Rect rect = new Rect(point.x - size, point.y - size, size * 2, size * 2);
        FillRect(rect, color);
    }
    public void FillRect(Vector2 start, Vector2 end, Color color)
    {
        Rect rect = TwoPointRect(start, end);
        FillRect(rect, color);
    }

    /* ============================== Ellipse ============================== */
    //public void StrokeEllipse(Vector2 center, int width, int height, ECBrush brush)
    //{
    //    brush.size += 1;
    //    Rect rect = new Rect(center.x - width - brush.size, center.y - height - brush.size, width * 2 + brush.size * 2, height * 2 + brush.size * 2);

    //    int rx = (int)rect.x;
    //    int ry = (int)rect.y;
    //    int rw = (int)rect.width;
    //    int rh = (int)rect.height;
    //    int rm = Mathf.Max(rw, rh);
    //    int am = Mathf.Max(width, height);
    //    Vector2 rc = center - new Vector2(rw - rm, rh - rm) * 0.5f;
    //    if (rx < 0 || ry < 0 || rw < 0 || rh < 0) return;
    //    Color[] colors = texture.GetPixels(rx, ry, rm, rm);
    //    Color[] colors = new Color[rm * rm];
    //    for (int i = 0; i < colors.Length; i++) colors[i] = Color.clear;
    //    for (int y = ry; y < ry + rm; y++)
    //    {
    //        for (int x = rx; x < rx + rm; x++)
    //        {
    //            Vector2 p = new Vector2(x, y);
    //            float d = Mathf.Abs(Vector2.Distance(p, rc) - am);
    //            else d = ECUtility.MinDistance(p, ECUtility.VerticalPoints(p, center, width, height));
    //            if (d > brush.size)
    //            {
    //                continue;
    //            }
    //            else if (d <= brush.size)
    //            {
    //                int i = (x - rx) + (y - ry) * rm;
    //                float a = (brush.size - d) / (brush.size * (1 - brush.hardness));
    //                a = d / (float)brush.size <= brush.hardness ? brush.color.a : a * brush.color.a;
    //                if (colors[i].a < a)
    //                {
    //                    Color c = brush.color;
    //                    if (d >= brush.size - 1) c.a = 0;
    //                    else c.a = a;
    //                    colors[i] = c;
    //                }
    //            }
    //        }
    //    }
    //    Texture2D tmp = new Texture2D(rm, rm);
    //    tmp.SetPixels(colors);
    //    tmp.Apply();
    //    tmp = Painting.ResizeTexture(rw, rh, tmp);
    //    center.x -= rw / 2;
    //    center.y -= rh / 2;
    //    Rect desRect = new Rect(center.x, center.y, rw, rh);
    //    Rect sourRect = new Rect(center.x, center.y, rw, rh);
        
    //    desRect = DestRect(desRect, texture);
    //    sourRect = SourRect(sourRect, desRect);
    //    colors = tmp.GetPixels((int)sourRect.x, (int)sourRect.y, (int)sourRect.width, (int)sourRect.height);
    //    Map(desRect.x, desRect.y, desRect.width, desRect.height, colors);
    //}
    public void StrokeEllipse(Vector2 point, int width, int height, ECBrush brush)
    {
        if (width < 1 || height < 1) return;
        if (brush.size <= 1)
        {
            StrokeEllipse(point, width, height, brush.color);
            return;
        }
        int xc = (int)point.x;
        int yc = (int)point.y;
        int a = width;
        int b = height;
        if (a == 0) a = 1;
        if (b == 0) b = 1;
        int a2 = 2 * a * a;
        int b2 = 2 * b * b;
        int error = a * a * b;
        int x = 0;
        int y = b;
        int stopy = 0;
        int stopx = a2 * b;
        while (stopy <= stopx)
        {
            Draw(xc + x, yc + y, brush);
            Draw(xc - x, yc + y, brush);
            Draw(xc + x, yc - y, brush);
            Draw(xc - x, yc - y, brush);
            ++x;
            error -= b2 * (x - 1);
            stopy += b2;
            if (error <= 0)
            {
                error += a2 * (y - 1);
                --y;
                stopx -= a2;
            }
        }
        error = b * b * a;
        x = a;
        y = 0;
        stopy = b2 * a;
        stopx = 0;
        while (stopy >= stopx)
        {
            Draw(xc + x, yc + y, brush);
            Draw(xc - x, yc + y, brush);
            Draw(xc + x, yc - y, brush);
            Draw(xc - x, yc - y, brush);
            ++y;
            error -= a2 * (y - 1);
            stopx += a2;
            if (error < 0)
            {
                error += b2 * (x - 1);
                --x;
                stopy -= b2;
            }
        }
    }
    public void StrokeEllipse(Vector2 start, Vector2 end, ECBrush brush)
    {
        Ellipse e = TwoPointEllipse(start, end);
        StrokeEllipse(e.center, (int)e.w, (int)e.h, brush);
    }
    public void StrokeEllipse(Ellipse ellipse, ECBrush brush)
    {
        StrokeEllipse(ellipse.center, (int)ellipse.w, (int)ellipse.h, brush);
    }

    public void StrokeEllipse(Vector2 point, int width, int height, Color color, float dotDist, bool reverse)
    {
        if (width < 1 || height < 1) return;
        int xc = (int)point.x;
        int yc = (int)point.y;
        int a = width;
        int b = height;
        if (a == 0) a = 1;
        if (b == 0) b = 1;
		int a2 = 2*a * a;
		int b2 = 2*b * b;
		int error = a*a*b;
		int x = 0;
		int y = b;
		int stopy = 0;
		int stopx = a2 * b ;
		while (stopy <= stopx) {
            if (xc + x >= 0 && xc + x < this.width && yc + y >= 0 && yc + y < this.height && (int)(Mathf.Abs(x) / dotDist) % 2 == 0) Line(xc + x, yc + y, xc + x, yc + y, color, 0, reverse);
            if (x != 0 && xc - x >= 0 && xc - x < this.width && yc + y >= 0 && yc + y < this.height && (int)(Mathf.Abs(x) / dotDist) % 2 == 0) Line(xc - x, yc + y, xc - x, yc + y, color, 0, reverse);
            if (xc + x >= 0 && xc + x < this.width && yc - y >= 0 && yc - y < this.height && (int)(Mathf.Abs(x) / dotDist) % 2 == 0) Line(xc + x, yc - y, xc + x, yc - y, color, 0, reverse);
            if (x != 0 && xc - x >= 0 && xc - x < this.width && yc - y >= 0 && yc - y < this.height && (int)(Mathf.Abs(x) / dotDist) % 2 == 0) Line(xc - x, yc - y, xc - x, yc - y, color, 0, reverse);
			++x;
			error -= b2 * (x - 1);
			stopy += b2;
			if (error <= 0) {
				error += a2 * (y - 1);
				--y;
				stopx -= a2;
			}
		}
			
		error = b*b*a;
		x = a;
		y = 0;
		stopy = b2*a;
		stopx = 0;
		while (stopy >= stopx) {
            if (a2 * y < b2 * x && xc + x >= 0 && xc + x < this.width && yc + y >= 0 && yc + y < this.height && (int)(Mathf.Abs(y) / dotDist) % 2 == 0) Line(xc + x, yc + y, xc + x, yc + y, color, 0, reverse);
            if (a2 * y < b2 * x && xc - x >= 0 && xc - x < this.width && yc + y >= 0 && yc + y < this.height && (int)(Mathf.Abs(y) / dotDist) % 2 == 0) Line(xc - x, yc + y, xc - x, yc + y, color, 0, reverse);
            if (a2 * y < b2 * x && y != 0 && xc + x >= 0 && xc + x < this.width && yc - y >= 0 && yc - y < this.height && (int)(Mathf.Abs(y) / dotDist) % 2 == 0) Line(xc + x, yc - y, xc + x, yc - y, color, 0, reverse);
            if (a2 * y < b2 * x && y != 0 && xc - x >= 0 && xc - x < this.width && yc - y >= 0 && yc - y < this.height && (int)(Mathf.Abs(y) / dotDist) % 2 == 0) Line(xc - x, yc - y, xc - x, yc - y, color, 0, reverse);
			++y;
			error -= a2 * (y - 1);
			stopx += a2;
			if (error < 0) {
				error += b2 * (x - 1);
				--x;
				stopy -= b2;
			}
		}
        //int xc = (int)point.x;
        //int yc = (int)point.y;
        //if (width < 1) width = 1;
        //if (height < 1) height = 1;
        //int a2 = width * width;
        //int b2 = height * height;
        //int fa2 = 4 * a2, fb2 = 4 * b2;
        //int x, y, sigma;
        ///* first half */
        //for (x = 0, y = height, sigma = 2 * b2 + a2 * (1 - 2 * height); b2 * x <= a2 * y; x++)
        //{
        //    if (xc + x >= 0 && xc + x < this.width && yc + y >= 0 && yc + y < this.height && (int)(Mathf.Abs(x) / dotDist) % 2 == 0) Line(xc + x, yc + y, xc + x, yc + y, color, 0, reverse);
        //    if (x != 0 && xc - x >= 0 && xc - x < this.width && yc + y >= 0 && yc + y < this.height && (int)(Mathf.Abs(x) / dotDist) % 2 == 0) Line(xc - x, yc + y, xc - x, yc + y, color, 0, reverse);
        //    if (xc + x >= 0 && xc + x < this.width && yc - y >= 0 && yc - y < this.height && (int)(Mathf.Abs(x) / dotDist) % 2 == 0) Line(xc + x, yc - y, xc + x, yc - y, color, 0, reverse);
        //    if (x != 0 && xc - x >= 0 && xc - x < this.width && yc - y >= 0 && yc - y < this.height && (int)(Mathf.Abs(x) / dotDist) % 2 == 0) Line(xc - x, yc - y, xc - x, yc - y, color, 0, reverse);
        //    if (sigma >= 0)
        //    {
        //        sigma += fa2 * (1 - y);
        //        y--;
        //    }
        //    sigma += b2 * ((4 * x) + 6);
        //}
        ///* second half */
        //for (x = width, y = 0, sigma = 2 * a2 + b2 * (1 - 2 * width); a2 * y <= b2 * x; y++)
        //{
        //    if (a2 * y < b2 * x && xc + x >= 0 && xc + x < this.width && yc + y >= 0 && yc + y < this.height && (int)(Mathf.Abs(y) / dotDist) % 2 == 0) Line(xc + x, yc + y, xc + x, yc + y, color, 0, reverse);
        //    if (a2 * y < b2 * x && xc - x >= 0 && xc - x < this.width && yc + y >= 0 && yc + y < this.height && (int)(Mathf.Abs(y) / dotDist) % 2 == 0) Line(xc - x, yc + y, xc - x, yc + y, color, 0, reverse);
        //    if (a2 * y < b2 * x && y != 0 && xc + x >= 0 && xc + x < this.width && yc - y >= 0 && yc - y < this.height && (int)(Mathf.Abs(y) / dotDist) % 2 == 0) Line(xc + x, yc - y, xc + x, yc - y, color, 0, reverse);
        //    if (a2 * y < b2 * x && y != 0 && xc - x >= 0 && xc - x < this.width && yc - y >= 0 && yc - y < this.height && (int)(Mathf.Abs(y) / dotDist) % 2 == 0) Line(xc - x, yc - y, xc - x, yc - y, color, 0, reverse);
        //    if (sigma >= 0)
        //    {
        //        sigma += fb2 * (1 - x);
        //        x--;
        //    }
        //    sigma += a2 * ((4 * y) + 6);
        //}
    }
    public void StrokeEllipse(Vector2 point, int width, int height, Color color, bool reverse)
    {
        StrokeEllipse(point, width, height, color, 0, reverse);
    }
    public void StrokeEllipse(Vector2 start, Vector2 end, Color color, float dotDist, bool reverse)
    {
        Ellipse e = TwoPointEllipse(start, end);
        StrokeEllipse(e.center, (int)e.w, (int)e.h, color, dotDist, reverse);
    }
    public void StrokeEllipse(Vector2 start, Vector2 end, Color color, bool reverse)
    {
        Ellipse e = TwoPointEllipse(start, end);
        StrokeEllipse(e.center, (int)e.w, (int)e.h, color, 0, reverse);
    }
    public void StrokeEllipse(Ellipse ellipse, Color color, float dotDist, bool reverse)
    {
        StrokeEllipse(ellipse.center, (int)ellipse.w, (int)ellipse.h, color, dotDist, reverse);
    }
    public void StrokeEllipse(Ellipse ellipse, Color color, bool reverse)
    {
        StrokeEllipse(ellipse.center, (int)ellipse.w, (int)ellipse.h, color, 0, reverse);
    }

    public void StrokeEllipse(Vector2 point, int width, int height, Color color, float dotDist)
    {
        StrokeEllipse(point, width, height, color, dotDist, false);
    }
    public void StrokeEllipse(Vector2 point, int width, int height, Color color)
    {
        StrokeEllipse(point, width, height, color, 0, false);
    }
    public void StrokeEllipse(Vector2 start, Vector2 end, Color color, float dotDist)
    {
        Ellipse e = TwoPointEllipse(start, end);
        StrokeEllipse(e.center, (int)e.w, (int)e.h, color, dotDist, false);
    }
    public void StrokeEllipse(Vector2 start, Vector2 end, Color color)
    {
        Ellipse e = TwoPointEllipse(start, end);
        StrokeEllipse(e.center, (int)e.w, (int)e.h, color, 0, false);
    }
    public void StrokeEllipse(Ellipse ellipse, Color color, float dotDist)
    {
        StrokeEllipse(ellipse.center, (int)ellipse.w, (int)ellipse.h, color, dotDist, false);
    }
    public void StrokeEllipse(Ellipse ellipse, Color color)
    {
        StrokeEllipse(ellipse.center, (int)ellipse.w, (int)ellipse.h, color, 0, false);
    }

    public void StrokeEllipse(Vector2 point, int width, int height, float dotDist)
    {
        StrokeEllipse(point, width, height, Color.clear, dotDist, true);
    }
    public void StrokeEllipse(Vector2 point, int width, int height)
    {
        StrokeEllipse(point, width, height, Color.clear, 0, true);
    }
    public void StrokeEllipse(Vector2 start, Vector2 end, float dotDist)
    {
        Ellipse e = TwoPointEllipse(start, end);
        StrokeEllipse(e.center, (int)e.w, (int)e.h, Color.clear, dotDist, true);
    }
    public void StrokeEllipse(Vector2 start, Vector2 end)
    {
        Ellipse e = TwoPointEllipse(start, end);
        StrokeEllipse(e.center, (int)e.w, (int)e.h, Color.clear, 0, true);
    }
    public void StrokeEllipse(Ellipse ellipse, float dotDist)
    {
        StrokeEllipse(ellipse.center, (int)ellipse.w, (int)ellipse.h, Color.clear, dotDist, true);
    }
    public void StrokeEllipse(Ellipse ellipse)
    {
        StrokeEllipse(ellipse.center, (int)ellipse.w, (int)ellipse.h, Color.clear, 0, true);
    }

    public void FillEllipse(Vector2 point, int width, int height, Color color)
    {
        if (width < 1 || height < 1) return;
        int xc = (int)point.x;
        int yc = (int)point.y;
        int a = width;
        int b = height;
        if (a == 0) a = 1;
        if (b == 0) b = 1;
        int a2 = 2 * a * a;
        int b2 = 2 * b * b;
        int error = a * a * b;
        int x = 0;
        int y = b;
        int stopy = 0;
        int stopx = a2 * b;
        while (stopy <= stopx)
        {
            if (xc + x >= 0 && xc + x < this.width && yc + y >= 0 && yc + y < this.height) Line(xc + x, yc + y, xc + x, yc - y, color);
            if (xc - x >= 0 && xc - x < this.width && yc + y >= 0 && yc + y < this.height) Line(xc - x, yc + y, xc - x, yc - y, color);
            ++x;
            error -= b2 * (x - 1);
            stopy += b2;
            if (error <= 0)
            {
                error += a2 * (y - 1);
                --y;
                stopx -= a2;
            }
        }

        error = b * b * a;
        x = a;
        y = 0;
        stopy = b2 * a;
        stopx = 0;
        while (stopy >= stopx)
        {
            if (xc + x >= 0 && xc + x < this.width && yc + y >= 0 && yc + y < this.height) Line(xc + x, yc + y, xc + x, yc - y, color);
            if (xc - x >= 0 && xc - x < this.width && yc + y >= 0 && yc + y < this.height) Line(xc - x, yc + y, xc - x, yc - y, color);
            ++y;
            error -= a2 * (y - 1);
            stopx += a2;
            if (error < 0)
            {
                error += b2 * (x - 1);
                --x;
                stopy -= b2;
            }
        }
    }
    public void FillEllipse(Vector2 start, Vector2 end, Color color)
    {
        Ellipse e = TwoPointEllipse(start, end);
        FillEllipse(e.center, (int)e.w, (int)e.h, color);
    }
    public void FillEllipse(Ellipse ellpise, Color color)
    {
        FillEllipse(ellpise.center, (int)ellpise.w, (int)ellpise.h, color);
    }

    /* ============================== Circle ============================== */
    public void StrokeCircle(Circle circle, ECBrush brush)
    {
        if (circle.r < 1) return;
        if (brush.size <= 1)
        {
            StrokeEllipse(circle.ellipse, brush.color);
            return;
        }
        brush.size += 1;
        //if (end == start) end.y++;
        Rect rect = new Rect(circle.x - circle.r - brush.size, circle.y - circle.r - brush.size, circle.r*2 + brush.size * 2, circle.r*2 + brush.size * 2);
        rect = DestRect(rect, texture);
        int rx = (int)rect.x;
        int ry = (int)rect.y;
        int rw = (int)rect.width;
        int rh = (int)rect.height;
        if (rx < 0 || ry < 0 || rw < 0 || rh < 0) return;
        //Color[] colors = preview.GetPixels(rx, ry, rw, rh);
        //Color[] colors = new Color[rw * rh];
        //for (int i = 0; i < colors.Length; i++) colors[i] = Color.clear;
        for (int y = ry; y < ry + rh; y++)
        {
            for (int x = rx; x < rx + rw; x++)
            {
                Vector2 p = new Vector2(x, y);
                float d = Circle.Distance(circle, p);
                if (d > brush.size)
                {
                    continue;
                }
                else if (d <= brush.size)
                {
                    int i = (x - rx) + (y - ry) * rw;
                    float a = (brush.size - d) / (brush.size * (1 - brush.hardness));
                    a = d / (float)brush.size <= brush.hardness ? brush.color.a : a * brush.color.a;
                    if (preview.GetPixel(x, y).a < a) //colors[i].a < a)
                    {
                        Color c = brush.color;
                        if (d >= brush.size - 1) c.a = 0;
                        else c.a = a;
                        Map(x, y, c);
                        //colors[i] = c;
                        //Map(x, y, colors[i]);
                    }
                }
            }
        }
        //Map(rx, ry, rw, rh, colors);
    }
    public void StrokeCircle(Vector2 start, Vector2 end, ECBrush brush)
    {
        Circle c = TwoPointCircle(start, end);
        StrokeCircle(c, brush);
    }
    public void StrokeCircle(Vector2 point, int size, ECBrush brush)
    {
        Circle c = new Circle(point.x, point.y, size);
        StrokeCircle(c, brush);
    }

    /* ============================== Stamp ============================== */
    public void FillStamp(Vector2 point, ECBrush brush)
    {
        if (point.x < 0 || point.y < 0 || point.x > width || point.y > height) return;
        point.x -= brush.width / 2;
        point.y -= brush.height / 2;
        Rect desRect = new Rect(point.x, point.y, brush.width, brush.height);
        Rect sourRect = desRect;
        desRect = DestRect(desRect, texture);
        sourRect = SourRect(sourRect, desRect);

        Color[] colors = brush.stamp.texture.GetPixels((int)sourRect.x, (int)sourRect.y, (int)sourRect.width, (int)sourRect.height);
        Map(desRect.x, desRect.y, desRect.width, desRect.height, colors, true);
    }
}
