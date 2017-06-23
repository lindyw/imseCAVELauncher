using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ECPainter : MonoBehaviour {
    ECPainting paint;
    public Texture2D bg;
    public Texture2D stamp;
    public ECBrush brush;
    int tool = 0;
    Texture2D currentStamp;
    Texture2D currentBG; 
    RectTransform boardSize;
    int totalPaint = 0;
    public string path
    {
        get;
        set;
    }

    public ECBrush[] brushSetting
    {
        get;
        set;
    }

    public RawImage board;
    public RawImage preview;
    public RawImage brushOutline;

    bool isShift = false;
    bool isPainting = false;

    Vector2 start;
    Vector2 end;
    Vector2 press; 
    
    Vector2 mPos;
    Vector2 none = new Vector2(-9999, -9999);

	// Use this for initialization
	void Start () {
        if (path == null) path = "Paints/paing.png";
        start = none;
        end = start;
        press = start;
        currentStamp = stamp;
        boardSize = gameObject.GetComponent<RectTransform>();
        SetBoard();
        brushSetting = new ECBrush[6];
        brushSetting[0] = new ECBrush(ECBrush.Tool.Pen, 6, 0.75f, Color.black);
        brushSetting[1] = new ECBrush(ECBrush.Tool.Eraser, 12, 0.9f, Color.black);
        brushSetting[2] = new ECBrush(ECBrush.Tool.Line, 3, 0.9f, Color.black);
        brushSetting[3] = new ECBrush(ECBrush.Tool.Rectangle, 3, 0.75f, Color.black);
        brushSetting[4] = new ECBrush(ECBrush.Tool.Ellipse, 3, 0.75f, Color.black);
        if(stamp == null) brushSetting[5] = new ECBrush(ECBrush.Tool.Stamp, 6, 0.75f, Color.white);
        else brushSetting[5] = new ECBrush(ECPainting.ScaleTexture(stamp, 10), 10, 0.75f, Color.white);

        SetECBrush();
	}
	
	// Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Save(path);
        }
        //ECBrush
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            brush.tool = ECBrush.Tool.Pen;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            brush.tool = ECBrush.Tool.Eraser;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            brush.tool = ECBrush.Tool.Line;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            brush.tool = ECBrush.Tool.Rectangle;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            brush.tool = ECBrush.Tool.Ellipse;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            brush.tool = ECBrush.Tool.Stamp;
        }
        //Actions
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            paint.Previous();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            paint.Next();
        }
        else if (Input.GetKeyDown(KeyCode.Delete))
        {
            paint.Clear();
        }
        //Mouse
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            Reset();
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            isPainting = true;
        }
        if (tool != (int)brush.tool || brushSetting[tool].size != brush.size || brushSetting[tool].hardness != brush.hardness || brushSetting[tool].color != brush.color || stamp != currentStamp)
        {
            SetECBrush((int)brush.tool);
        }
        if (currentBG != bg)
        {
            SetBoard();
        }
        if (isPainting)
        {
            if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                Paint(true);
                start = none;
                press = start;
                isPainting = false;
            }
            else
            {
                mPos = (Input.mousePosition - transform.position) / transform.lossyScale.x / 1;
                mPos = new Vector2((int)mPos.x, (int)mPos.y);
                end = mPos;
                if (start != end || Input.GetKey(KeyCode.LeftShift) != isShift)
                {
                    if (start == none)
                    {
                        start = end;
                        press = start;
                    }
                    Paint();
                    if (Input.GetKey(KeyCode.LeftShift) != isShift) isShift = Input.GetKey(KeyCode.LeftShift);
                    if (start != end) start = end;
                }
            }
        }
        brushOutline.transform.position = Input.mousePosition;
	}
    public void SetBoard()
    {
        currentBG = bg;
        if (bg != null) paint = new ECPainting(bg, boardSize.sizeDelta.x, boardSize.sizeDelta.y);
        else paint = new ECPainting(boardSize.sizeDelta.x, boardSize.sizeDelta.y);
        board.texture = paint.texture;
        preview.texture = paint.preview;
    }
    public void SetECBrush(int tool)
    {
        if (tool == this.tool) brushSetting[tool] = new ECBrush(brushSetting[tool].tool, brush.size, brush.hardness, brush.color);
        if (stamp != currentStamp)
        {
            currentStamp = stamp;
            brushSetting[5] = new ECBrush(ECPainting.ScaleTexture(stamp, brushSetting[5].size), brushSetting[5].size, brushSetting[5].hardness, brushSetting[5].color);
        }
        if (tool < 0) tool = 0;
        this.tool = tool;
        brush = brushSetting[tool];
        brushOutline.texture = brush.outline;
        brushOutline.GetComponent<RectTransform>().sizeDelta = new Vector2(brush.width, brush.height);
        Reset();
    }
    public void SetECBrush()
    {
        SetECBrush(-1);
    }
    public void Reset()
    {
        isPainting = false;
        paint.Clean();
        paint.texture.SetPixels(paint.colors);
        start = none;
        press = start;
    }
    public void Save(string path)
    {
        ECFile store = new ECFile(path);        
        if (!store.DirectoryExists()) store.CreateDirectory();
        path = store.directory + "/" + store.file + "_";
        totalPaint++;
        while (ECFile.FileExists(path + ECCommons.FixLength(totalPaint, 0, 3) + "." + store.extension)) totalPaint++;
        paint.SaveTexture(path + ECCommons.FixLength(totalPaint, 0, 3) + "." + store.extension);
    }
    public void Paint(bool isECBrush)
    {
        if (brush.tool == ECBrush.Tool.Pen || brush.tool == ECBrush.Tool.Eraser)
        {
            paint.Draw(start, end, brush);
        }
        else if (brush.tool == ECBrush.Tool.Stamp)
        {
            paint.FillStamp(end, brush);
        }
        else if (brush.tool == ECBrush.Tool.Line)
        {
            Vector2[] pos = new Vector2[2];
            pos[0] = ECPainting.LockPoint(press, end);
            pos[1] = ECPainting.LockPoint(press, start);
            if (isShift)
            {
                paint.Line(press, pos[1], Color.clear);
                if (isECBrush) paint.Line(press, pos[0], brush);
                else if (Input.GetKey(KeyCode.LeftShift) == isShift) paint.Line(press, pos[0], 10);
                else paint.Line(press, end, 10);
            }
            else
            {
                paint.Line(press, start, Color.clear);
                if (isECBrush) paint.Line(press, end, brush);
                else if (Input.GetKey(KeyCode.LeftShift) == isShift) paint.Line(press, end, 10);
                else paint.Line(press, pos[0], 10);
            }

        }
        else if (brush.tool == ECBrush.Tool.Rectangle)
        {
            if (isShift)
            {
                paint.StrokeRect(ECPainting.TwoPointSquare(press, start), Color.clear);
                if (isECBrush) paint.StrokeRect(ECPainting.TwoPointSquare(press, end), brush);
                else if (Input.GetKey(KeyCode.LeftShift) == isShift) paint.StrokeRect(ECPainting.TwoPointSquare(press, end), 10);
                else paint.StrokeRect(ECPainting.TwoPointRect(press, end), 10);
            }
            else
            {
                paint.StrokeRect(ECPainting.TwoPointRect(press, start), Color.clear);
                if (isECBrush) paint.StrokeRect(ECPainting.TwoPointRect(press, end), brush);
                else if (Input.GetKey(KeyCode.LeftShift) == isShift) paint.StrokeRect(ECPainting.TwoPointRect(press, end), 10);
                else paint.StrokeRect(ECPainting.TwoPointSquare(press, end), 10);
            }
        }
        else if (brush.tool == ECBrush.Tool.Ellipse)
        {
            if (isShift)
            {
                paint.StrokeEllipse(ECPainting.TwoPointCircle(press, start).ellipse, Color.clear);
                if (isECBrush) paint.StrokeCircle(ECPainting.TwoPointCircle(press, end), brush);
                else if (Input.GetKey(KeyCode.LeftShift) == isShift) paint.StrokeEllipse(ECPainting.TwoPointCircle(press, end).ellipse, 10);
                else paint.StrokeEllipse(ECPainting.TwoPointEllipse(press, end), 10);
            }
            else
            {
                paint.StrokeEllipse(ECPainting.TwoPointEllipse(press, start), Color.clear);
                if (isECBrush) paint.StrokeEllipse(ECPainting.TwoPointEllipse(press, end), brush);
                else if (Input.GetKey(KeyCode.LeftShift) == isShift) paint.StrokeEllipse(ECPainting.TwoPointEllipse(press, end), 10);
                else paint.StrokeEllipse(ECPainting.TwoPointCircle(press, end).ellipse, 10);
            }
        }
        if (!isECBrush) paint.Preview();
        else
        {
            paint.Apply();
            paint.Done();
        }
    }
    public void Paint()
    {
        Paint(false);
    }
}
