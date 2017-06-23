using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECTranslation : ECMotion
{
    public struct Order
    {
        public Method method;
        public MotionBase mBase;
        public float factor;
        public float accel;
        public float decel;
        public Vector3 position;
        public Vector3 eulerAngles;
        public Vector3 scale;
        public float delay;
        public Order(Method method, MotionBase mBase, float factor, float accel, float decel, Vector3 position, Vector3 eulerAngles, Vector3 scale, float delay)
        {
            this.method = method;
            this.mBase = mBase;
            this.factor = factor;
            this.accel = accel;
            this.decel = decel;
            this.position = position;
            this.eulerAngles = eulerAngles;
            this.scale = scale;
            this.delay = delay;
        }
    }
    public enum Method
    {
        NONE = 0,
        TO = 1,
        BY = 2,
        FROM = 3
    }
    public enum Loop
    {
        NONE = 0,
        RESTART = 1,
        REVERSE = 2,
        CONTINUE = 3
    }

    [Range(1, 360)]
    public float angularRatio = 45;     //The angle rotates at the same time compare with 1 distance
    public float scalingRatio = 0.5f;   //The size scales at the same time compare with 1 distance

    public bool playAtStart = false;
    public Loop loop = Loop.NONE;

    public Transform target;

    List<Order> orders = new List<Order>();
    Order current = new Order(Method.NONE, MotionBase.VELOCITY, 0, 0, 0, Vector3.zero, Vector3.zero, Vector3.zero, 0);
    float timer = 0;
    // Use this for initialization
    void Start()
    {
        if (scalingRatio < 0.01f) scalingRatio = 0.01f;

        angularRatio = 1 / angularRatio;
        scalingRatio = 1 / scalingRatio;

        if (playAtStart && target) TransformTo(target);
    }

    // Update is called once per frame
    void Update()
    {
        if (timer > 0)
        {
            if (!isPaused) timer -= Time.deltaTime;
        }
        else if (timer <= 0 && current.method != Method.NONE)
        {
            timer = 0;
            StopAllCoroutines();
            if (current.method == Method.BY)
            {
                current.position += transform.position;
                current.eulerAngles += transform.localEulerAngles;
                Vector3 s = transform.localScale;
                current.scale = new Vector3(s.x * current.scale.x, s.y * current.scale.y, s.z * current.scale.z);
            }
            StartCoroutine(StartTranslating(current.mBase, current.factor, current.accel, current.decel, current.position, current.eulerAngles, current.scale, current.delay));
            current.method = Method.NONE;
        }
        else if (orders.Count > 0 && timer <= 0)
        {
            timer = orders[0].delay;
            current = orders[0];
            orders.RemoveAt(0);
        }
    }

    void Transformation(Method method, MotionBase mBase, float factor, float accel, float decel, Vector3 position, Vector3 eulerAngles, Vector3 scale, float delay)
    {
        orders.Add(new Order(method, mBase, factor, accel, decel, position, eulerAngles, scale, delay));
    }


    public void TransformTo(MotionBase mBase, float factor, float accel, float decel, Vector3 position, Vector3 eulerAngles, Vector3 scale, float delay)
    {
        Transformation(Method.TO, mBase, factor, accel, decel, position, eulerAngles, scale, delay);
    }
    public void TransformTo(MotionBase mBase, float factor, float accel, float decel, Transform target, float delay)
    {
        TransformTo(mBase, factor, accel, decel, target.position, target.eulerAngles, target.localScale, delay);
    }
    public void TransformTo(Vector3 position, Vector3 eulerAngles, Vector3 scale, float delay)
    {
        TransformTo(motionBase, motionFactor, motionAccel, motionDecel, position, eulerAngles, scale, delay);
    }
    public void TransformTo(Transform target, float delay)
    {
        TransformTo(motionBase, motionFactor, motionAccel, motionDecel, target.position, target.eulerAngles, target.localScale, delay);
    }
    public void TransformTo(MotionBase mBase, float factor, float accel, float decel, Vector3 position, Vector3 eulerAngles, Vector3 scale)
    {
        TransformTo(mBase, factor, accel, decel, position, eulerAngles, scale, 0);
    }
    public void TransformTo(MotionBase mBase, float factor, float accel, float decel, Transform target)
    {
        TransformTo(mBase, factor, accel, decel, target.position, target.eulerAngles, target.localScale, 0);
    }
    public void TransformTo(Vector3 position, Vector3 eulerAngles, Vector3 scale)
    {
        TransformTo(motionBase, motionFactor, motionAccel, motionDecel, position, eulerAngles, scale, 0);
    }
    public void TransformTo(Transform target)
    {
        TransformTo(motionBase, motionFactor, motionAccel, motionDecel, target.position, target.eulerAngles, target.localScale, 0);
    }


    public void TranslateTo(MotionBase mBase, float factor, float accel, float decel, Vector3 position, Vector3 eulerAngles, float delay)
    {
        TransformTo(mBase, factor, accel, decel, position, eulerAngles, transform.localScale, delay);
    }
    public void TranslateTo(MotionBase mBase, float factor, float accel, float decel, Transform target, float delay)
    {
        TranslateTo(mBase, factor, accel, decel, target.position, target.eulerAngles, delay);
    }
    public void TranslateTo(Vector3 position, Vector3 eulerAngles, float delay)
    {
        TranslateTo(motionBase, motionFactor, motionAccel, motionDecel, position, eulerAngles, delay);
    }
    public void TranslateTo(Transform target, float delay)
    {
        TranslateTo(motionBase, motionFactor, motionAccel, motionDecel, target, delay);
    }
    public void TranslateTo(MotionBase mBase, float factor, float accel, float decel, Vector3 position, Vector3 eulerAngles)
    {
        TranslateTo(mBase, factor, accel, decel, position, eulerAngles, 0);
    }
    public void TranslateTo(MotionBase mBase, float factor, float accel, float decel, Transform target)
    {
        TranslateTo(mBase, factor, accel, decel, target.position, target.eulerAngles, 0);
    }
    public void TranslateTo(Vector3 position, Vector3 eulerAngles)
    {
        TranslateTo(motionBase, motionFactor, motionAccel, motionDecel, position, eulerAngles, 0);
    }
    public void TranslateTo(Transform target)
    {
        TranslateTo(motionBase, motionFactor, motionAccel, motionDecel, target, 0);
    }

    public void Translate(MotionBase mBase, float factor, float accel, float decel, Vector3 distance, Vector3 degree, float delay)
    {
        Transformation(Method.BY, mBase, factor, accel, decel, distance, degree, Vector3.one, delay);
    }
    public void Translate(Vector3 position, Vector3 eulerAngles, float delay)
    {
        Translate(motionBase, motionFactor, motionAccel, motionDecel, position, eulerAngles, delay);
    }
    public void Translate(MotionBase mBase, float factor, float accel, float decel, Vector3 distance, Vector3 degree)
    {
        Translate(mBase, factor, accel, decel, distance, degree, 0);
    }
    public void Translate(Vector3 position, Vector3 eulerAngles)
    {
        Translate(motionBase, motionFactor, motionAccel, motionDecel, position, eulerAngles, 0);
    }


    public void MoveTo(MotionBase mBase, float factor, float accel, float decel, Vector3 position, float delay)
    {
        TransformTo(mBase, factor, accel, decel, position, transform.localEulerAngles, transform.localScale, delay);
    }
    public void RotateTo(MotionBase mBase, float factor, float accel, float decel, Vector3 eulerAngles, float delay)
    {
        TransformTo(mBase, factor, accel, decel, transform.position, eulerAngles, transform.localScale, delay);
    }
    public void ScaleTo(MotionBase mBase, float factor, float accel, float decel, Vector3 scale, float delay)
    {
        TransformTo(mBase, factor, accel, decel, transform.position, transform.localEulerAngles, scale, delay);
    }
    public void ScaleTo(MotionBase mBase, float factor, float accel, float decel, float scale, float delay)
    {
        ScaleTo(mBase, factor, accel, decel, Vector3.one * scale, delay);
    }
    public void MoveTo(MotionBase mBase, float factor, float accel, float decel, Vector3 position)
    {
        MoveTo(mBase, factor, accel, decel, position, 0);
    }
    public void RotateTo(MotionBase mBase, float factor, float accel, float decel, Vector3 eulerAngles)
    {
        RotateTo(mBase, factor, accel, decel, eulerAngles, 0);
    }
    public void ScaleTo(MotionBase mBase, float factor, float accel, float decel, Vector3 scale)
    {
        ScaleTo(mBase, factor, accel, decel, scale, 0);
    }
    public void ScaleTo(MotionBase mBase, float factor, float accel, float decel, float scale)
    {
        ScaleTo(mBase, factor, accel, decel, Vector3.one * scale, 0);
    }

    public void MoveTo(Vector3 position, float delay)
    {
        MoveTo(motionBase, motionFactor, motionAccel, motionDecel, position, delay);
    }
    public void RotateTo(Vector3 eulerAngles, float delay)
    {
        RotateTo(motionBase, motionFactor, motionAccel, motionDecel, eulerAngles, delay);
    }
    public void ScaleTo(Vector3 scale, float delay)
    {
        ScaleTo(motionBase, motionFactor, motionAccel, motionDecel, scale, delay);
    }
    public void ScaleTo(float scale, float delay)
    {
        ScaleTo(Vector3.one * scale, delay);
    }
    public void MoveTo(Vector3 position)
    {
        MoveTo(motionBase, motionFactor, motionAccel, motionDecel, position, 0);
    }
    public void RotateTo(Vector3 eulerAngles)
    {
        RotateTo(motionBase, motionFactor, motionAccel, motionDecel, eulerAngles, 0);
    }
    public void ScaleTo(Vector3 scale)
    {
        ScaleTo(motionBase, motionFactor, motionAccel, motionDecel, scale, 0);
    }
    public void ScaleTo(float scale)
    {
        ScaleTo(Vector3.one * scale, 0);
    }


    public void Move(MotionBase mBase, float factor, float accel, float decel, Vector3 distance, float delay)
    {
        Transformation(Method.BY, mBase, factor, accel, decel, distance, Vector3.zero, Vector3.one, delay);
    }
    public void Rotate(MotionBase mBase, float factor, float accel, float decel, Vector3 degree, float delay)
    {
        Transformation(Method.BY, mBase, factor, accel, decel, Vector3.zero, degree, Vector3.one, delay);
    }
    public void Scale(MotionBase mBase, float factor, float accel, float decel, Vector3 magnification, float delay)
    {
        Transformation(Method.BY, mBase, factor, accel, decel, Vector3.zero, Vector3.zero, magnification, delay);
    }
    public void Scale(MotionBase mBase, float factor, float accel, float decel, float magnification, float delay)
    {
        Scale(mBase, factor, accel, decel, Vector3.one * magnification, delay);
    }
    public void Move(MotionBase mBase, float factor, float accel, float decel, Vector3 distance)
    {
        Move(mBase, factor, accel, decel, distance, 0);
    }
    public void Rotate(MotionBase mBase, float factor, float accel, float decel, Vector3 degree)
    {
        Rotate(mBase, factor, accel, decel, degree, 0);
    }
    public void Scale(MotionBase mBase, float factor, float accel, float decel, Vector3 magnification)
    {
        Scale(mBase, factor, accel, decel, magnification, 0);
    }
    public void Scale(MotionBase mBase, float factor, float accel, float decel, float magnification)
    {
        Scale(mBase, factor, accel, decel, Vector3.one * magnification, 0);
    }

    public void Move(Vector3 distance, float delay)
    {
        Move(motionBase, motionFactor, motionAccel, motionDecel, distance, delay);
    }
    public void Rotate(Vector3 degree, float delay)
    {
        Rotate(motionBase, motionFactor, motionAccel, motionDecel, degree, delay);
    }
    public void Scale(Vector3 magnification, float delay)
    {
        Scale(motionBase, motionFactor, motionAccel, motionDecel, magnification, delay);
    }
    public void Scale(float magnification, float delay)
    {
        Scale(motionBase, motionFactor, motionAccel, motionDecel, magnification, delay);
    }
    public void Move(Vector3 distance)
    {
        Move(motionBase, motionFactor, motionAccel, motionDecel, distance, 0);
    }
    public void Rotate(Vector3 degree)
    {
        Rotate(motionBase, motionFactor, motionAccel, motionDecel, degree, 0);
    }
    public void Scale(Vector3 magnification)
    {
        Scale(motionBase, motionFactor, motionAccel, motionDecel, magnification, 0);
    }
    public void Scale(float magnification)
    {
        Scale(motionBase, motionFactor, motionAccel, motionDecel, magnification, 0);
    }

    public new void Stop()
    {
        isPaused = false;
        if (isMoving)
        {
            StopAllCoroutines();
        }
        else if (timer > 0 || current.method != Method.NONE)
        {
            timer = 0;
            current.method = Method.NONE;
        }
        else if (orders.Count > 0)
        {
            orders.RemoveAt(0);
        }
        isMoving = false;
    }
    public void StopAll()
    {
        StopAllCoroutines();
        isMoving = false;
        isPaused = false;
        orders.Clear();
        timer = 0;
        current.method = Method.NONE;
    }

    IEnumerator StartTranslating(MotionBase mBase, float factor, float accel, float decel, Vector3 position, Vector3 eulerAngles, Vector3 scale, float delay)
    {
        isPaused = false;
        isMoving = true;

        Vector3 originPos = transform.position;
        Vector3 originAngle = transform.localEulerAngles;
        Vector3 originScale = transform.localScale;
        float distance = Vector3.Distance(originPos, position);
        float angle3D = ECCommons.Angle3D(originAngle, eulerAngles) * angularRatio;
        float scale3D = Vector3.Distance(originScale, scale) * scalingRatio;
        float dist = Mathf.Max(distance, angle3D, scale3D);
        GetMotionParatmers(mBase, factor, accel, decel, dist);
        if (dist > error && factor >= 0 && accel >= 0 && decel >= 0)
        {
            while (isMoving && rate != 1)
            {
                if (!isPaused)
                {
                    Rate();
                    transform.position = Vector3.Lerp(originPos, position, rate);
                    transform.localEulerAngles = Vector3.Lerp(originAngle, eulerAngles, rate);
                    transform.localScale = Vector3.Lerp(originScale, scale, rate);
                }
                yield return 0;
            }
        }
        switch (loop)
        {
            case Loop.RESTART:
                transform.position = originPos;
                transform.localEulerAngles = originAngle;
                transform.localScale = originScale;
                Transformation(Method.TO, mBase, factor, accel, decel, position, eulerAngles, scale, delay);
                break;
            case Loop.REVERSE:
                Transformation(Method.TO, mBase, factor, accel, decel, originPos, originAngle, originScale, delay);
                break;
            case Loop.CONTINUE:
                Transformation(Method.BY, mBase, factor, accel, decel, position, eulerAngles, scale, delay);
                break;
            default:
                transform.position = position;
                transform.localEulerAngles = eulerAngles;
                transform.localScale = scale;
                isMoving = false;
                break;
        }
    }
}
