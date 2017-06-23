using UnityEngine;
using System.Collections;

public class ECTimer : MonoBehaviour {
    public bool isPlaying = false;
    public bool isPaused = false;

    public float time = 0;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (isPlaying && !isPaused) time += Time.deltaTime;
	}

    public bool TimesUp(float target)
    {
        if (time >= target)
        {
            Stop();
            return true;
        }
        return false;
    }

    public void Reset()
    {
        time = 0;
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
        isPlaying = false;
    }

    public void Count()
    {
        isPaused = false;
        isPlaying = true;
    }
    public void Count(float start)
    {
        time = start;
        Count();
    }
}
