using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ECUIController : ECMotion
{
    [System.Serializable]
    public enum Hide
    {
        NONE = 0,
        DISABLE = 1,
        RAYCAST = 2
    }
    [System.Serializable]
    public struct Transition
    {
        public float speed;
        public Vector3 movement;
        public Vector3 rotation;
        public Vector3 scaling;
        public Color fading;
        public float canvasAlpha;

        public Transition(float speed)
        {
            this.speed = speed;
            movement = Vector3.zero;
            rotation = Vector3.zero;
            scaling = Vector3.one;
            fading = Color.white;
            canvasAlpha = 1;
        }
    }
    public enum State
    {
        IDLE = 0,
        FADEIN = 1,
        FADEOUT = 2,
        ENABLED = 3,
        DISABLED = 4
    }

    public Hide hideMethod = Hide.DISABLE;
    public float iniDelay = 0;
    public int layer;
    public int page;
    public Transition fadeIn = new Transition(1);
    public Transition fadeOut = new Transition(1);
    public Transition enabling = new Transition(1);
    public Transition disabling = new Transition(1);
    public bool canDrag = false;
    public bool canTop = false;
    public bool functional = false;
    public bool transitionLock = false;
    public bool colorMask = false;
    public bool childIndividual = false;

    ECUIEvent events;
    int uiType = 0;
    [HideInInspector]
    public CanvasGroup canvasGroup;
    [HideInInspector]
    public List<Graphic> graphics;

    Vector3 position;
    Vector3 eulerAngles;
    Vector3 scale;
    Color color;
    float alpha;
    RectTransform rt;
    Vector3 diffPos;
    Vector3 displacement;
    int sibling;

    bool ini = false;
    public State state = State.IDLE;
    public bool faded = false;
    public bool disabled = false;

    [HideInInspector]
    public List<ECUIController> children;

    // Use this for initialization
    public virtual void Start()
    {
        motionNumbers = new float[4];
        motionNumbers[0] = 0;
        motionNumbers[1] = 0;
        motionNumbers[2] = 1;
        motionNumbers[3] = 1;
        canvasGroup = GetComponent<CanvasGroup>();
        rt = ECCommons.SetComponent<RectTransform>(gameObject);
        events = ECCommons.SetComponent<ECUIEvent>(gameObject);
        if (childIndividual)
        {
            children.AddRange(GetComponentsInChildren<ECUIController>());
            children.RemoveAt(0);
            if (children.Count < 1) childIndividual = false;
        }
        if (colorMask) graphics.AddRange(GetComponentsInChildren<Graphic>());
        else if (GetComponent<Graphic>()) graphics.Add(GetComponent<Graphic>());
        Invoke("Initialize", Time.deltaTime * iniDelay);
    }

    public void Initialize()
    {
        position = rt.localPosition;
        eulerAngles = rt.localEulerAngles;
        scale = rt.localScale;
        if (graphics.Count > 0) color = graphics[0].color;
        ini = true;
    }

    // Update is called once per frame
    public virtual void Update()
    {
        if (ini)
        {
            if (functional)
            {
                if (events.isPressing == gameObject) { if (state != State.FADEOUT && motionNumbers[2] != 0) FadeOut(); }
                else if (events.isOverlapping == gameObject) { if (state != State.FADEIN && motionNumbers[2] != 1) FadeIn(); }
                else if (state != State.FADEOUT && motionNumbers[2] != 0) FadeOut();
            }
            if (events.isDragging) Drag();
            if (events.isClicked)
            {
                if (diffPos == Vector3.zero) diffPos = transform.InverseTransformPoint(Input.mousePosition);
                Top();
            }
            //else if (!events.isPressing) diffPos = Vector3.zero;
        }
    }
    public virtual void Drag()
    {
        if (canDrag)
        {
            transform.localPosition = transform.localPosition + transform.InverseTransformPoint(Input.mousePosition) - diffPos;
            displacement = transform.localPosition - position;
        }
    }
    public virtual void Top()
    {
        if (canTop)
        {
            transform.SetAsLastSibling();
        }
    }
    public virtual void ResetUI()
    {
        transform.localPosition = position;
        transform.localEulerAngles = eulerAngles;
        transform.localScale = scale;
        transform.SetSiblingIndex(sibling);
        diffPos = Vector3.zero;
        displacement = Vector3.zero;
        if (canvasGroup)
        {
            canvasGroup.alpha = alpha;
            //if (!isVisible) canvasGroup.blocksRaycasts = false;
            //else canvasGroup.blocksRaycasts = true;
        }
    }

    public void FadeIn()
    {
        Transformation(fadeIn, State.FADEIN, 0);
        if (childIndividual) foreach (ECUIController c in children) c.FadeIn();
    }
    public void FadeOut()
    {
        Transformation(fadeOut, State.FADEOUT, 0);
        if (childIndividual) foreach (ECUIController c in children) c.FadeOut();
    }
    public void Enable()
    {
        Transformation(enabling, State.ENABLED, 0);
        if (childIndividual) foreach (ECUIController c in children) c.Enable();
    }
    public void Disable()
    {
        Transformation(disabling, State.DISABLED, 0);
        if (childIndividual) foreach (ECUIController c in children) c.Disable();
    }
    public void FadeIn(float speed)
    {
        Transformation(fadeIn, speed, State.FADEIN, 0);
        if (childIndividual) foreach (ECUIController c in children) c.FadeIn(speed);
    }
    public void FadeOut(float speed)
    {
        Transformation(fadeOut, speed, State.FADEOUT, 0);
        if (childIndividual) foreach (ECUIController c in children) c.FadeOut(speed);
    }
    public void Enable(float speed)
    {
        Transformation(enabling, speed, State.ENABLED, 0);
        if (childIndividual) foreach (ECUIController c in children) c.Enable(speed);
    }
    public void Disable(float speed)
    {
        Transformation(disabling, speed, State.DISABLED, 0);
        if (childIndividual) foreach (ECUIController c in children) c.Disable(speed);
    }

    public void Transformation(Transition transition, State state, float delay)
    {
        if (transition.speed != 0 && motionBase == MotionBase.TIME) transition.speed = 1 / transition.speed;
        Transformation(motionBase, motionFactor * transition.speed, motionAccel * transition.speed, motionDecel * transition.speed, transition.movement, transition.rotation, transition.scaling, transition.fading, transition.canvasAlpha, state, delay);
    }
    public void Transformation(Transition transition, float speed, State state, float delay)
    {
        if (speed != 0 && motionBase == MotionBase.TIME) speed = 1 / speed;
        Transformation(motionBase, motionFactor * speed, motionAccel * speed, motionDecel * speed, transition.movement, transition.rotation, transition.scaling, transition.fading, transition.canvasAlpha, state, delay);
    }
    public void Transformation(MotionBase mBase, float factor, float accel, float decel, Vector3 movement, Vector3 rotation, Vector3 scaling, Color fading, float canvasAlpha, State state, float delay)
    {
        switch(hideMethod)
        {
            case Hide.DISABLE: gameObject.SetActive(true); break;
            case Hide.RAYCAST: canvasGroup.blocksRaycasts = true; break;
        }
        StopAllCoroutines();
        StartCoroutine(StartTransition(mBase, factor, accel, decel, position + displacement + movement, eulerAngles + rotation, new Vector3(scale.x * scaling.x, scale.y * scaling.y, scale.z * scaling.z), fading, canvasAlpha, state, delay));
    }

    IEnumerator StartTransition(MotionBase mBase, float factor, float accel, float decel, Vector3 position, Vector3 eulerAngles, Vector3 scale, Color color, float canvasAlpha, State state, float delay)
    {
        isPaused = false;
        isMoving = true;
        bool rayLock = ECUIEvent.raycaster && transitionLock;
        this.state = state;
        switch (state)
        {
            case State.FADEIN: motionNumbers[2] = 1; motionNumbers[3] = 0; break;
            case State.FADEOUT: motionNumbers[2] = 0; motionNumbers[3] = 0; break;
            case State.ENABLED: motionNumbers[2] = 0; motionNumbers[3] = 0; break;
            case State.DISABLED: motionNumbers[2] = 0; motionNumbers[3] = 1; break;
        }
        if (rayLock) ECUIEvent.raycaster.enabled = false;

        float originFade = motionNumbers[0];
        float originAble = motionNumbers[1];
        Vector3 originPos = transform.localPosition;
        Vector3 originAngle = transform.localEulerAngles;
        Vector3 originScale = transform.localScale;
        Color originColor = graphics.Count > 0 ? graphics[0].color : color;
        float originAlpha = canvasGroup ? canvasGroup.alpha : 1;
        float dist = Mathf.Max(Mathf.Abs(motionNumbers[2] - originFade), Mathf.Abs(motionNumbers[3] - originAble));
        GetMotionParatmers(mBase, factor, accel, decel, dist);
        if (dist > error && factor >= 0 && accel >= 0 && decel >= 0)
        {
            while (isMoving && rate != 1)
            {
                if (!isPaused)
                {
                    Rate();
                    motionNumbers[0] = Mathf.Lerp(originFade, motionNumbers[2], rate);
                    motionNumbers[1] = Mathf.Lerp(originAble, motionNumbers[3], rate);
                    transform.localPosition = Vector3.Lerp(originPos, position, rate);
                    transform.localEulerAngles = Vector3.Lerp(originAngle, eulerAngles, rate);
                    transform.localScale = Vector3.Lerp(originScale, scale, rate);
                    if (canvasGroup) canvasGroup.alpha = Mathf.Lerp(originAlpha, canvasAlpha, rate);
                    foreach (Graphic g in graphics) g.color = Color.Lerp(originColor, color, rate);
                }
                if (rayLock && !ECUIEvent.raycaster.enabled) ECUIEvent.raycaster.enabled = false;
                yield return 0;
            }
        }
        motionNumbers[0] = motionNumbers[2];
        motionNumbers[1] = motionNumbers[3];
        transform.localPosition = position;
        transform.localEulerAngles = eulerAngles;
        transform.localScale = scale;
        if (canvasGroup) canvasGroup.alpha = canvasAlpha;
        foreach (Graphic g in graphics) g.color = color;
        faded = motionNumbers[2] == 1;
        disabled = motionNumbers[3] == 1;
        this.state = State.IDLE;
        isMoving = false;
        if (!isVisible)
        {
            switch (hideMethod)
            {
                case Hide.DISABLE: gameObject.SetActive(false); break;
                case Hide.RAYCAST: canvasGroup.blocksRaycasts = false; break;
            }
        }
        if (ECUIEvent.raycaster) ECUIEvent.raycaster.enabled = true;
    }
    public bool isVisible
    {
        get { return (canvasGroup && canvasGroup.alpha != 0 && transform.localScale != Vector3.zero && IsInCanvas()); }
    }
    bool isHidden = false;
    public bool IsInCanvas()
    {
        RectTransform pRectTrans = GetComponent<RectTransform>();
        RectTransform cRectTrans = ECUIEvent.raycaster.GetComponent<RectTransform>();
        Vector2 pSize = pRectTrans.sizeDelta;
        Vector2 cSize = cRectTrans.sizeDelta;
        Vector2 pPos = (Vector2)pRectTrans.position - pSize / 2;
        Vector2 cPos = (Vector2)cRectTrans.position - cSize / 2;
        Rect pRect = new Rect(pPos, pSize);
        Rect cRect = new Rect(cPos, cSize);
        return cRect.Overlaps(pRect);
    }
}
