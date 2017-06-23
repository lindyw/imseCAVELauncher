using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECAudioController : MonoBehaviour
{
    public string directory;
    public List<AudioSource> audios = new List<AudioSource>();
    public List<string> names = new List<string>();
    public Dictionary<string, int> nameIndex = new Dictionary<string, int>();

    [Range(0, 1)]
    public float blend = 0;
    [Range(0, 1)]
    public float volume = 1;
    public bool loop = false;
    public bool playOnAwake = false;

    bool[] isPaused;

    List<int> audioList = new List<int>();
    // Use this for initialization
    void Start()
    {
        if (audios.Count <= 0)
        {
            LoadWav();
        }
        else
        {
            for (int i = 0; i < names.Count; i++)
            {
                nameIndex.Add(names[i], i);
            }
        }
        isPaused = new bool[audios.Count];
        for (int i = 0; i < isPaused.Length; i++)
        {
            isPaused[i] = false;
            audios[i].spatialBlend = blend;
            audios[i].volume = volume;
            audios[i].loop = loop;
            audios[i].playOnAwake = playOnAwake;
            if (playOnAwake) Play(i);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void LoadWav()
    {
        string[] files = ECFile.FileCounts(directory);
        for (int i = 0; i < files.Length; i++)
        {
            audios.Add(gameObject.AddComponent<AudioSource>());
            StartCoroutine(ECFile.LoadWAV(audios[i], ECFile.Path(files[i])));
            if (i >= names.Count)
            {
                nameIndex.Add(ECFile.FileName(files[i]), i);
            }
            else
            {
                nameIndex.Add(names[i], i);
            }
        }
    }

    IEnumerator StartPlaying(int index, float delay, float start, float end, float loopLength)
    {
        if (delay <= 0)
        {
            Stop(index);
            yield return 0;
        }
        audioList.Add(index);
        isPaused[index] = false;
        while (delay > 0)
        {
            yield return 0;
            if (!audioList.Contains(index)) break;
            else if (!isPaused[index]) delay -= Time.deltaTime;
        }
        if (audioList.Contains(index))
        {
            AudioSource curAudio = audios[index];
            AudioClip clip = curAudio.clip;
            if (start < 0) start = 0;
            if (start < clip.length)
            {
                float timer = start;
                if (end < 0)
                    if (!curAudio.loop && clip) end = clip.length;
                    else if (curAudio.loop) timer = end - 1;
                if (loopLength < 0) loopLength = 0;
                else if (loopLength > clip.length - start) loopLength = clip.length - start;
                curAudio.Play();
                curAudio.time = start;
                while (curAudio.isPlaying || isPaused[index])
                {
                    yield return 0;
                    if (!audioList.Contains(index) || timer > end) break;
                    else if (isPaused[index]) curAudio.Pause();
                    else
                    {
                        curAudio.UnPause();
                        if (timer >= 0) timer += Time.deltaTime;
                        if (curAudio.loop && curAudio.time > start + loopLength) curAudio.time -= loopLength;
                    }
                }
            }
            curAudio.Stop();
        }
        if (audioList.Contains(index)) audioList.Remove(index);
    }

    public void Play(int index, float delay, float start, float end, float loopLength)
    {
        if (index < audios.Count)
        {
            StartCoroutine(StartPlaying(index, delay, start, end, loopLength));
        }
    }
    public void Play(int index, float delay, float start, float end)
    {
        Play(index, delay, start, end, 0);
    }
    public void Play(int index, float delay, float length)
    {
        Play(index, delay, 0, length, 0);
    }
    public void Play(int index, float delay)
    {
        Play(index, delay, -1);
    }

    public void Play(int index)
    {
        Play(index, 0, -1);
    }
    public void Play(string name, float delay)
    {
        if (nameIndex.ContainsKey(name)) Play(nameIndex[name], delay, -1);
    }
    public void Play(string name)
    {
        if (nameIndex.ContainsKey(name)) Play(nameIndex[name], 0, -1);
    }

    public void Stop()
    {
        audioList.Clear();
    }
    public void Stop(int index)
    {
        while (audioList.Contains(index)) audioList.Remove(index);
    }
    public void Stop(string name)
    {
        if (nameIndex.ContainsKey(name)) Stop(nameIndex[name]);
    }

    public void Pause()
    {
        for (int i = 0; i < isPaused.Length; i++) isPaused[i] = true;
    }
    public void Pause(int index)
    {
        if (index < audios.Count) isPaused[index] = true;
    }
    public void Pause(string name)
    {
        if (nameIndex.ContainsKey(name)) Pause(nameIndex[name]);
    }

    public void UnPause()
    {
        for (int i = 0; i < isPaused.Length; i++) isPaused[i] = false;
    }
    public void UnPause(int index)
    {
        if (index < audios.Count) isPaused[index] = false;
    }
    public void UnPause(string name)
    {
        if (nameIndex.ContainsKey(name)) UnPause(nameIndex[name]);
    }

    public float Duration(int index)
    {
        if (index < audios.Count) return audios[index].clip.length;
        return 0;
    }
    public float Duration(string name)
    {
        if (nameIndex.ContainsKey(name)) return Duration(nameIndex[name]);
        return -1;
    }

    public float Current(int index)
    {
        if (index < audios.Count) return audios[index].time;
        return 0;
    }
    public float Current(string name)
    {
        if (nameIndex.ContainsKey(name)) return Current(nameIndex[name]);
        return -1;
    }

    public bool IsPlaying(int index)
    {
        if (index < audios.Count) return audios[index].isPlaying;
        return false;
    }
    public bool IsPlaying(string name)
    {
        if (nameIndex.ContainsKey(name)) return IsPlaying(nameIndex[name]);
        return false;
    }


    public void SetVolume(int index, float volume)
    {
        if (index < audios.Count) audios[index].volume = volume;
    }
    public void SetVolume(string name, float volume)
    {
        if (nameIndex.ContainsKey(name)) SetVolume(nameIndex[name], volume);
    }
    public void SetPitch(int index, float pitch)
    {
        if (index < audios.Count) audios[index].pitch = pitch;
    }
    public void SetPitch(string name, float pitch)
    {
        if (nameIndex.ContainsKey(name)) SetPitch(nameIndex[name], pitch);
    }
    public void SetLoop(int index, bool loop)
    {
        if (index < audios.Count) audios[index].loop = loop;
    }
    public void SetLoop(string name, bool loop)
    {
        if (nameIndex.ContainsKey(name)) SetLoop(nameIndex[name], loop);
    }

    public float GetVolume(int index)
    {
        if (index < audios.Count) return audios[index].volume;
        return 0;
    }
    public float GetVolume(string name)
    {
        if (nameIndex.ContainsKey(name)) return GetVolume(nameIndex[name]);
        return -1;
    }
    public float GetPitch(int index)
    {
        if (index < audios.Count) return audios[index].pitch;
        return 1;
    }
    public float GetPitch(string name)
    {
        if (nameIndex.ContainsKey(name)) return GetPitch(nameIndex[name]);
        return -1;
    }
    public bool GetLoop(int index)
    {
        if (index < audios.Count) return audios[index].loop;
        return false;
    }
    public bool GetLoop(string name)
    {
        if (nameIndex.ContainsKey(name)) return GetLoop(nameIndex[name]);
        return false;
    }
}
