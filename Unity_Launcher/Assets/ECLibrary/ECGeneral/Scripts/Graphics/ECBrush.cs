using UnityEngine;
using System.Collections;

[System.Serializable]
public struct ECBrush
{
    public enum Tool
    {
        Pen = 0,
        Eraser = 1,
        Line = 2,
        Rectangle = 3,
        Ellipse = 4,
        Stamp = 5
    }
    public Tool tool;
    [Range(1, 100)]
    public int size;
    [Range(0, 1)]
    public float hardness;
    public Color color;
    public ECPainting stamp;

    public int width
    {
        get { return stamp.width; }
    }
    public int height
    {
        get { return stamp.height; }
    }
    public Texture2D texture
    {
        get { return stamp.texture; }
    }
    public Texture2D outline
    {
        get { return stamp.preview; }
    }

    public ECBrush(ECPainting stamp, int size, float hardness, Color color)
    {
        this.tool = Tool.Stamp;
        this.size = size;
        this.hardness = hardness;
        this.color = color;
        this.stamp = stamp;
        ColorChange(color);
        Outline();
    }
    public ECBrush(Texture2D texture, int size, float hardness, Color color)
    {
        this.tool = Tool.Stamp;
        this.size = size;
        this.hardness = hardness;
        this.color = color;
        ECPainting stamp = new ECPainting(texture);
        this.stamp = stamp;
        ColorChange(color);
        Outline();
    }
    public ECBrush(Tool tool, int size, float hardness, Color color)
    {
        this.tool = tool;
        this.size = size;
        this.hardness = hardness;
        this.color = color;
        stamp = new ECPainting(Color.clear, size * 2 + 17, size * 2 + 17);
        Outline();
    }
    public void Outline()
    {
        if (tool == Tool.Line || tool == Tool.Rectangle || tool == Tool.Ellipse)
        {
            stamp.Line(new Vector2(size + 8, size), new Vector2(size + 8, size + 16), Color.black);
            stamp.Line(new Vector2(size, size + 8), new Vector2(size + 16, size + 8), Color.black);
        }
        else if (tool == Tool.Stamp)
        {
            stamp.preview.SetPixels(stamp.texture.GetPixels());
        }
        else
        {
            stamp.StrokeEllipse(new Circle(size + 8, size + 8, size).ellipse, Color.black);
        }
        stamp.Preview();
    }
    public void ColorChange(Color color)
    {
        this.color = color;
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                stamp.texture.SetPixel(j, i, stamp.texture.GetPixel(j, i) * color);
            }
        }
        stamp.texture.Apply();
    }
}
