using UnityEngine;
using System.Collections;

public class ECMotion : MonoBehaviour {
    [System.Serializable]
    public enum MotionBase
    {
        VELOCITY = 0,
        TIME = 1
    }
    public float[] motionNumbers;

    public MotionBase motionBase = MotionBase.VELOCITY;
    public float motionFactor = 3;
    public float motionAccel = 1;
    public float motionDecel = 1;
    public float error = 0.001f;
    
    public bool isMoving = false;
    public bool isPaused = false;

    float accV;
    float decV;
    float maxV;
    float accT;
    float decT;
    float maxT;
    float dt;
    float ds;
    float maxS;
    float avgT;
    float accS;
    float decS;
    float avgS;
    public float rate;
    // Use this for initialization
    void Awake()
    {
        if (motionFactor < 0) motionFactor = 0;
        if (motionAccel < 0) motionAccel = 0;
        if (motionDecel < 0) motionDecel = 0;
        if (error < 0) error = 0;
    }
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void GetMotionParatmers(MotionBase method, float factor, float accel, float decel, float distance)
    {
        accV = 0;
        decV = 0;
        maxV = 0;
        accT = 0;
        decT = 0;
        maxT = 0;
        dt = 0;
        ds = 0;
        maxS = distance;
        rate = 0;
        if (factor <= 0 && accel <= 0 && decel <= 0) maxT = 0;
        else {
            switch (method)
            {
                case MotionBase.VELOCITY:
                    accV = accel > 0 ? accel : 0;
                    decV = decel > 0 ? decel : 0;
                    maxV = factor > 0 ? factor : 0;
                    accT = accV > 0 ? maxV / accV : 0;
                    decT = decV > 0 ? maxV / decV : 0;
                    avgT = maxV > 0 ? maxS / maxV - (accT + decT) * 0.5f : 0;
                    if (avgT < 0)
                    {
                        accT = accV > 0 ? decV > 0 ? Mathf.Sqrt(2 * decV * maxS / (accV * (accV + decV))) : Mathf.Sqrt(maxS / accV) : 0;
                        decT = decV > 0 ? accV > 0 ? Mathf.Sqrt(2 * accV * maxS / (decV * (accV + decV))) : Mathf.Sqrt(maxS / decV) : 0;
                        avgT = 0;
                        maxV = accT > 0 ? accV * accT : decV * decT;
                    }
                    break;
                case MotionBase.TIME:
                    accT = accel > 0 ? accel : 0;
                    decT = decel > 0 ? decel : 0;
                    avgT = factor > 0 ? factor : 0;
                    maxT = avgT + (accT + decT) * 0.5f;
                    maxV = maxS / maxT;
                    accV = accT > 0 ? maxV / accT : 0;
                    decV = decT > 0 ? maxV / decT : 0;
                    break;
            }
            maxT = accT + avgT + decT;
        }
        accS = 0.5f * maxV * accT;
        avgS = maxV * avgT;
        decS = 0.5f * maxV * decT;
        //Debug.Log("accV: " + accV + ", maxV: " + maxV + ", decV: " + decV);
        //Debug.Log("accT: " + accT + ", avgT: " + avgT + ", decT: " + decT + ", maxT: " + maxT);
        //Debug.Log("accS: " + accS + ", avgS: " + avgS + ", decS: " + decS + ", maxS: " + maxS);
        //Debug.Log("GetMotionParatmers: " + dt + ", " + maxT + " | " + ds + " , " + maxS + " | " + factor + " . " + accel + " . " + decel + " . " + distance);
    }
    public void Rate()
    {
        dt += Time.deltaTime;
        if (dt >= maxT) ds = maxS;
        else if (dt < accT) ds = 0.5f * accV * dt * dt; // s = 0.5at^2
        else if (dt >= accT + avgT)                     // s = accS + avgS + ut - 0.5at^2
        {
            float t = dt - accT - avgT;
            ds = accS + avgS + maxV * t - 0.5f * decV * t * t;
        }
        else
        {                                               // s = accS + vt
            float t = dt - accT;
            ds = accS + maxV * t;
        }
        if (ds >= maxS) rate = 1;
        else rate = ds / maxS;
        //Debug.Log("Rate: " + dt + " , " + maxT + " | " + ds + " , " + maxS + " , " + rate);
    }
    public void Pause()
    {
        isPaused = true;
    }
    public void UnPause()
    {
        isPaused = false;
    }
    public void Stop()
    {
        isPaused = false;
        isMoving = false;
        StopAllCoroutines();
    }

    public void Value(MotionBase method, float factor, float accel, float decel, float delay, params float[] target)
    {
        StopAllCoroutines();
        StartCoroutine(StartMotion(method, factor, accel, decel, delay, target));
    }
    public void Value(float delay, params float[] target)
    {
        Value(motionBase, motionFactor, motionAccel, motionDecel, delay, target);
    }
    IEnumerator StartMotion(MotionBase method, float factor, float accel, float decel, float delay, params float[] target)
    {
        isPaused = false;
        isMoving = true;
        float[] origin = new float[Mathf.Min(target.Length, motionNumbers.Length)];
        float[] distance = new float[origin.Length];
        for (int i = 0; i < origin.Length; i++)
        {
            origin[i] = motionNumbers[i];
            distance[i] = Mathf.Abs(origin[i] - target[i]);
        }
        float dist = Mathf.Max(distance);
        GetMotionParatmers(method, factor, accel, decel, dist);
        if (dist > error && factor >= 0 && accel >= 0 && decel >= 0)
        {            
            while (isMoving && rate != 1)
            {
                if (!isPaused)
                {
                    Rate();
                    for (int i = 0; i < origin.Length; i++) motionNumbers[i] = Mathf.Lerp(origin[i], target[i], rate);
                }
                yield return 0;
            }
        }
        for (int i = 0; i < origin.Length; i++) motionNumbers[i] = target[i];
        isMoving = false;
    }
}
