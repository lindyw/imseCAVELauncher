using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECHighlight : MonoBehaviour
{
    public struct Order
    {
        public Type type;
        public Color color;
        public float delay;
        public float duration;
        public string pattern;
        public bool reset;
        public Order(Type type, Color color, float delay, float duration, string pattern, bool reset)
        {
            this.type = type;
            this.color = color;
            this.delay = delay;
            this.duration = duration;
            this.pattern = pattern;
            this.reset = reset;
        }
    }
    [System.Serializable]
    public enum Type
    {
        NONE = 0,
        ANALOG = 1,
        DIGITAL = 2,
        HOLD = 3
    }
    /* --- Please Set at least one of the emission brightness value of shader be > 0 first --- */
    List<Renderer> renderers = new List<Renderer>();
    public List<Material> materials = new List<Material>();
    public string shaderColor = "_EmissionColor";
    public Color color = new Color(1, 0.6f, 0.1f, 0.5f);
    public float speed = 2;
    float emission = 0;
    public Type type = Type.ANALOG;
    public bool playAtStart = false;

    public bool isHighlighting = false;
    public bool isPaused = false;

    List<Order> orders = new List<Order>();
    Order current = new Order(Type.NONE, Color.clear, 0, 0, "", true);
    float timer = 0;
    // Use this for initialization
    void Start()
    {
        GetRenderers();
        if (playAtStart) Highlight(type, color, 0, -999, "", true);
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            if (!isPaused) timer -= Time.deltaTime;
        }
        else if (timer <= 0 && current.type != Type.NONE)
        {
            timer = 0;
            StopAllCoroutines();
            StartCoroutine(StartHighlighting(current.type, current.color, current.duration, current.pattern, current.reset));
            current.type = Type.NONE;
        }
        else if (orders.Count > 0 && timer <= 0)
        {
            timer = orders[0].delay;
            current = orders[0];
            orders.RemoveAt(0);
        }
    }

    void GetRenderers()
    {
        renderers.Clear();
        renderers.AddRange(gameObject.GetComponentsInChildren<Renderer>());
        for (int i = 0; i < renderers.Count; i++)
        {
            materials.AddRange(renderers[i].materials);
        }
        Color c = EmissionColor(0, color);
        for (int i = 0; i < materials.Count; i++)
        {
            materials[i].EnableKeyword("_EMISSION");
            materials[i].SetColor(shaderColor, c);
        }
    }

    public void Highlight(Type type, Color color, float delay, float duration, string pattern, bool reset)
    {
        orders.Add(new Order(type, color, delay, duration, pattern, reset));
    }

    public void Highlight(float delay, float duration)
    {
        Highlight(Type.ANALOG, color, delay, duration, "", true);
    }
    public void Highlight(float delay, string pattern)
    {
        Highlight(Type.ANALOG, color, delay, -1, pattern, true);
    }
    public void Highlight(float delay)
    {
        Highlight(Type.ANALOG, color, delay, -999, "", true);
    }
    public void Highlight()
    {
        Highlight(Type.ANALOG, color, 0, -999, "", true);
    }

    public void Flash(float delay, float duration)
    {
        Highlight(Type.DIGITAL, color, delay, duration, "", true);
    }
    public void Flash(float delay, string pattern)
    {
        Highlight(Type.DIGITAL, color, delay, -1, pattern, true);
    }
    public void Flash(float delay)
    {
        Highlight(Type.DIGITAL, color, delay, -999, "", true);
    }
    public void Flash()
    {
        Highlight(Type.DIGITAL, color, 0, -999, "", true);
    }

    public void Hold(float delay, float duration)
    {
        Highlight(Type.HOLD, color, delay, duration, "", true);
    }
    public void Hold(float delay)
    {
        Highlight(Type.HOLD, color, delay, -999, "", true);
    }
    public void Hold()
    {
        Highlight(Type.HOLD, color, 0, -999, "", true);
    }

    void Dishighlight(Type type, Color color)
    {
        isPaused = false;
        if (isHighlighting)
        {
            StopAllCoroutines();
            StartCoroutine(StartDishighlighting(type, color));
        }
        else if (timer > 0 || current.type != Type.NONE)
        {
            timer = 0;
            current.type = Type.NONE;
        }
        else if (orders.Count > 0)
        {
            orders.RemoveAt(0);
        }
        isHighlighting = false;
    }
    public void Dishighlight()
    {
        Dishighlight(type, color);
    }
    public void DishighlightAll()
    {
        isHighlighting = false;
        isPaused = false;
        orders.Clear();
        timer = 0;
        current.type = Type.NONE;
        StopAllCoroutines();
        StartCoroutine(StartDishighlighting(type, color));
    }

    public void Pause()
    {
        isPaused = true;
    }
    public void UnPause()
    {
        isPaused = false;
    }

    IEnumerator StartHighlighting(Type type, Color color, float duration, string pattern, bool reset)
    {
        if (renderers.Count > 0)
        {
            float speed = this.speed;
            this.type = type;

            //Reset
            if (reset)
            {
                StartCoroutine(StartDishighlighting(type, color));
                do { yield return 0; } while (emission > 0);
            }

            //Highlight
            isHighlighting = true;
            isPaused = false;
            int digital = 1;
            int pIndex = 0;
            List<float> speeds = new List<float>();
            if (pattern.Length > 0)
            {
                for (int i = 0; i < pattern.Length; i++)
                {
                    float s = float.Parse(pattern[i].ToString());
                    for (int j = 0; j < s; j++)
                    {
                        speeds.Add(s);
                    }
                    if (s == 0) speeds.Add(0);
                }
                speed = speeds[0];
            }
            while (duration <= -999 || duration > 0 || pIndex < speeds.Count)
            {
                if (!isPaused)
                {
                    if (duration > 0) duration -= Time.deltaTime;
                    switch (type)
                    {
                        case Type.DIGITAL:
                            if (emission > 1)
                            {
                                emission -= 1;
                                if (speeds.Count <= 0 || speeds[pIndex] != 0) SetColor(EmissionColor(Mathf.PingPong(digital, 1) * color.a, color));
                                digital = 1 - digital;
                                if (digital == 1 && pIndex < speeds.Count)
                                {
                                    pIndex++;
                                    if (speeds[pIndex] != 0) speed = speeds[pIndex];
                                    else speed = 1;
                                }
                            }
                            break;
                        case Type.HOLD:
                            if (digital == 1)
                            {
                                SetColor(EmissionColor(color.a, color));
                                digital = 0;
                            }
                            break;
                        default:
                            if (speeds.Count <= 0 || speeds[pIndex] != 0) SetColor(EmissionColor(Mathf.PingPong(emission, 1) * color.a, color));
                            if (emission > 2)
                            {
                                emission -= 2;
                                if (pIndex < speeds.Count)
                                {
                                    pIndex++;
                                    if (speeds[pIndex] != 0) speed = speeds[pIndex];
                                    else speed = 1;
                                }
                            }
                            break;
                    }
                    emission += Time.deltaTime * speed;
                }
                yield return 0;
            }
            speeds.Clear();

            //Dishighlight
            Dishighlight(type, color);
        }
    }

    IEnumerator StartDishighlighting(Type type, Color color)
    {
        if (renderers.Count > 0)
        {
            isHighlighting = false;
            if (type == Type.ANALOG)
            {
                while (emission > 0 && emission < 2)
                {
                    if (!isPaused)
                    {
                        SetColor(EmissionColor(Mathf.PingPong(emission, 1) * color.a, color));
                        if (emission >= 1) emission += Time.deltaTime * speed;
                        else emission -= Time.deltaTime * speed;
                    }
                    yield return 0;
                }
            }
            SetColor(EmissionColor(0, color));
            emission = 0;
            isPaused = false;
        }
    }

    void SetColor(Color color)
    {
        for (int i = 0; i < materials.Count; i++)
        {
            materials[i].SetColor(shaderColor, color);
        }
    }
    Color EmissionColor(float factor, Color color)
    {
        return new Color(color.r * factor, color.g * factor, color.b * factor, color.a * factor);
    }
}
