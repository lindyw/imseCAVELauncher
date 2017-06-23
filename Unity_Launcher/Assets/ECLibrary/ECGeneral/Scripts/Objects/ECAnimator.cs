using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECAnimator : MonoBehaviour
{
    /* --- Please add the Float Parameter "Speed" to the animator and Tick the Speed Multiplier of all states --- */
    public struct Order
    {
        public string stateName;
        public float speed;
        public float delay;
        public float target;
        public Order(string stateName, float speed, float delay, float target)
        {
            this.stateName = stateName;
            this.speed = speed;
            this.delay = delay;
            this.target = target;
        }
    }

    public Animator animator;

    public string stateName = "";
    [Range(0.01f, 999)]
    public float speed = 1;
    [Range(1, Mathf.Infinity)]
    public float skipSpeed = 99;
    public bool playAtStart = false;
    public bool loop = false;
    public bool isAnimating = false;
    public bool isPaused = false;
    public bool needCrossFade = false;
    public float transitionDuration = 0.1f;
    string prevState;
    float fadeSpeed;

    List<Order> orders = new List<Order>();
    Order current = new Order("None", 0, 0, 0);
    float timer = 0;
    public float currTime = 0;
    public float percentage = 0;

    Dictionary<string, float> aniState = new Dictionary<string, float>();
    Dictionary<int, string> aniIndex = new Dictionary<int, string>();
    AnimationClip[] acs;

    float tmpDuration = 0;
    // Use this for initialization
    void Start()
    {
        tmpDuration = transitionDuration;
        transitionDuration = 0;
        animator = GetComponent<Animator>();
        if (!animator) Destroy(GetComponent<ECAnimator>());

        acs = animator.runtimeAnimatorController.animationClips;
        for (int i = 0; i < acs.Length; i++) {
            aniState.Add(acs[i].name, acs[i].length);
            aniIndex.Add(i, acs[i].name);
        }
        if (acs.Length > 0)
        {
            if (stateName == "") stateName = acs[0].name;
            fadeSpeed = 0;
            animator.SetFloat("Speed", 0);
            animator.Play(stateName, 0, 0);
        }
        prevState = stateName;
        if (playAtStart) Play();
    }

    // Update is called once per frame
    public void Update()
    {
        if (timer > 0)
        {
            if (!isPaused) timer -= Time.deltaTime;
        }
        else if (timer <= 0 && current.stateName != "None")
        {
            if(stateName != current.stateName)
            {
                stateName = current.stateName;
                currTime = 0;
            }
            timer = 0;
            StopAllCoroutines();
            StartCoroutine(StartAnimating(current.stateName, current.speed, current.target, current.delay));
            current.stateName = "None";
        }
        else if (orders.Count > 0 && timer <= 0)
        {
            timer = orders[0].delay;
            current = orders[0];
            orders.RemoveAt(0);
        }
        if (transitionDuration > 0) transitionDuration -= Time.deltaTime;
    }

    public void PlayTo(string stateName, float speed, float delay, float target)
    {
        orders.Add(new Order(stateName, speed, delay, target));
    }

    public void PlayTo(string stateName, float delay, float target)
    {
        PlayTo(stateName, speed, delay, target);
    }
    public void PlayTo(int stateIndex, float speed, float delay, float target)
    {
        PlayTo(aniIndex[stateIndex], speed, delay, target);
    }
    public void PlayTo(int stateIndex, float delay, float target)
    {
        PlayTo(aniIndex[stateIndex], speed, delay, target);
    }
    public void PlayTo(float delay, float target)
    {
        PlayTo(stateName, speed, delay, target);
    }
    public void PlayTo(float target)
    {
        PlayTo(stateName, speed, 0, target);
    }

    public void Play(string stateName, float speed, float delay)
    {
        PlayTo(stateName, speed, delay, Length(stateName));
    }
    public void Play(string stateName, float delay)
    {
        Play(stateName, speed, delay);
    }
    public void Play(int stateIndex, float speed, float delay)
    {
        PlayTo(stateIndex, speed, delay, Length(stateName));
    }
    public void Play(int stateIndex, float delay)
    {
        Play(stateIndex, speed, delay);
    }
    public void Play(float delay)
    {
        Play(stateName, speed, delay);
    }
    public void Play()
    {
        Play(stateName, speed, 0);
    }

    public void ReverseTo(string stateName, float speed, float delay, float target)
    {
        PlayTo(stateName, speed, delay, Length(stateName) - target);
    }
    public void ReverseTo(string stateName, float delay, float target)
    {
        PlayTo(stateName, speed, delay, Length(stateName) - target);
    }
    public void ReverseTo(int stateIndex, float speed, float delay, float target)
    {
        PlayTo(stateIndex, speed, delay, Length(stateName) - target);
    }
    public void ReverseTo(int stateIndex, float delay, float target)
    {
        PlayTo(stateIndex, speed, delay, Length(stateName) - target);
    }
    public void ReverseTo(float delay, float target)
    {
        PlayTo(stateName, speed, delay, Length() - target);
    }
    public void ReverseTo(float target)
    {
        PlayTo(stateName, speed, 0, Length() - target);
    }

    public void Reverse(string stateName, float speed, float delay)
    {
        PlayTo(stateName, speed, delay, 0);
    }
    public void Reverse(string stateName, float delay)
    {
        Reverse(stateName, speed, delay);
    }
    public void Reverse(int stateIndex, float speed, float delay)
    {
        PlayTo(stateIndex, speed, delay, 0);
    }
    public void Reverse(int stateIndex, float delay)
    {
        Reverse(stateIndex, speed, delay);
    }
    public void Reverse(float delay)
    {
        Reverse(stateName, speed, delay);
    }
    public void Reverse()
    {
        Reverse(stateName, speed, 0);
    }

    public void SkipPlayTo(string stateName, float target)
    {
        Clear();
        PlayTo(stateName, skipSpeed, 0, target);
    }
    public void SkipPlayTo(int stateIndex, float target)
    {
        Clear();
        PlayTo(stateIndex, skipSpeed, 0, target);
    }
    public void SkipPlayTo(float target)
    {
        Clear();
        PlayTo(stateName, skipSpeed, 0, target);
    }

    public void SkipPlay(string stateName)
    {
        Clear();
        Play(stateName, skipSpeed, 0);
    }
    public void SkipPlay(int stateIndex)
    {
        Clear();
        Play(stateIndex, skipSpeed, 0);
    }
    public void SkipPlay()
    {
        Clear();
        Play(stateName, skipSpeed, 0);
    }

    public void SkipReverseTo(string stateName, float target)
    {
        Clear();
        PlayTo(stateName, skipSpeed, 0, Length(stateName) - target);
    }
    public void SkipReverseTo(int stateIndex, float target)
    {
        Clear();
        PlayTo(stateIndex, skipSpeed, 0, Length(stateName) - target);
    }
    public void SkipReverseTo(float target)
    {
        Clear();
        PlayTo(stateName, skipSpeed, 0, Length() - target);
    }

    public void SkipReverse(string stateName)
    {
        Clear();
        Reverse(stateName, skipSpeed, 0);
    }
    public void SkipReverse(int stateIndex)
    {
        Clear();
        Reverse(stateIndex, skipSpeed, 0);
    }
    public void SkipReverse()
    {
        Clear();
        Reverse(stateName, skipSpeed, 0);
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
        if (isAnimating)
        {
            StopAllCoroutines();
            StartCoroutine(StartAnimating(stateName, skipSpeed, 0, 0));
        }
        else if (timer > 0 || current.stateName != "None")
        {
            timer = 0;
            current.stateName = "None";
        }
        else if (orders.Count > 0)
        {
            orders.RemoveAt(0);
        }
        isAnimating = false;
    }
    public void Clear()
    {
        StopAllCoroutines();
        isAnimating = false;
        isPaused = false;
        orders.Clear();
        timer = 0;
        current.stateName = "None";
    }
    public void StopAll()
    {
        Clear();
        StartCoroutine(StartAnimating(stateName, skipSpeed, 0, 0));
    }

    IEnumerator StartAnimating(string stateName, float speed, float target, float delay)
    {
        if (animator)
        {
            isPaused = false;
            isAnimating = true;
            fadeSpeed = speed;

            float length = Length(stateName);
            if (currTime > length) currTime = length;
            float origin = currTime;
            length = 1 / length;
            percentage = currTime * length;

            float maxS = Mathf.Abs(target - origin);
            float maxV = speed;
            float maxT = maxS / maxV;
            float dt = 0;
            float ds = 0;
            if (maxS > 0)
            {
                while (isAnimating)
                {
                    if (isAnimating && !isPaused)
                    {
                        dt += Time.deltaTime;
                        if (dt >= maxT) ds = maxS;
                        else ds = maxV * dt;
                        if (ds == maxS) break;
                        else SetPersent(stateName, Mathf.Lerp(origin, target, ds / maxS), length);
                    }
                    yield return 0;
                }
            }
            if (loop)
            {
                SetPersent(stateName, origin, length);
                PlayTo(stateName, speed, delay, target);
            }
            else
            {
                SetPersent(stateName, target, length);
                isAnimating = false;
            }
        }
    }

    public float Length(string stateName)
    {
        if (stateName == "" && acs.Length > 0) return acs[0].length;
        return aniState[stateName];
    }
    public float Length(int stateIndex)
    {
        return aniState[aniIndex[stateIndex]];
    }
    public float Length()
    {
        return Length(stateName);
    }



    void SetPersent(string stateName, float time, float length)
    {
        currTime = time;
        percentage = currTime * length;
        if (transitionDuration <= 0)
        {
            if (needCrossFade && prevState != stateName)
            {
                animator.CrossFade(stateName, tmpDuration / fadeSpeed / Length(prevState), -1, percentage);
                transitionDuration = tmpDuration / fadeSpeed;
                prevState = stateName;
                animator.SetFloat("Speed", fadeSpeed);
            }
            else
            {
                if(fadeSpeed != 0)
                {
                    fadeSpeed = 0;
                    animator.SetFloat("Speed", 0);
                }
                animator.Play(stateName, -1, percentage);
            }
        }
    }
}
