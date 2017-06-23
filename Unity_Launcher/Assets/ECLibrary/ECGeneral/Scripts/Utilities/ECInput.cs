using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ECInput : MonoBehaviour
{
    public enum State
    {
        IDLE = 0,
        DOWN = 1,
        PRESS = 2,
        HOLD = 3,
        LOCK = 4,
        UP = 5
    }

    public float downBuffer = 0.25f;
    public float pressBuffer = 1.5f;
    public string separator = " | ";
    public uint numberOfKeys = 1;
    public bool detectAnyKey = false;
    public KeyCode specificKey = KeyCode.None;

    public static bool isLocked = false;
    static string tmpSep = " | ";

    public static State[] state;
    public static string[] value;
    static float[] timer;
    static string[] current;// = "";
    static string[] stateCode;// = "";

    static List<string>[] keys;

    public string[] axisName;
    static bool[] axisExist;
    static string[] axisStr;

    // Use this for initialization
    void Start()
    {
        if (numberOfKeys < 1) numberOfKeys = 1;
        AxisSetup();
        state = new State[numberOfKeys];
        value = new string[numberOfKeys];
        timer = new float[numberOfKeys];
        current = new string[numberOfKeys];
        stateCode = new string[numberOfKeys];
        keys = new List<string>[numberOfKeys];
        ResetInputs();
    }

    public static void ResetInputs()
    {
        for(int i = 0; i < state.Length; i++)
        {
            state[i] = State.IDLE;
            value[i] = "";
            timer[i] = 0;
            current[i] = "";
            stateCode[i] = "";
            keys[i] = new List<string>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (tmpSep != separator) tmpSep = separator;

        for (uint i = 0; i < numberOfKeys; i++)
        {
            switch (state[i])
            {
                case State.DOWN:
                    timer[i] = 0;
                    state[i] = State.PRESS;
                    stateCode[i] = ".";
                    break;
                case State.PRESS:
                    if (timer[i] > downBuffer)
                    {
                        state[i] = State.HOLD;
                        stateCode[i] = "..";
                    }
                    break;
                case State.HOLD:
                    if (timer[i] > pressBuffer)
                    {
                        state[i] = State.LOCK;
                        stateCode[i] = "_";
                        keys[i].Add(current[i] + stateCode[i]);
                        Submit(i);
                    }
                    break;
                case State.LOCK:
                    break;
                case State.UP:
                    keys[i].Add(current[i] + stateCode[i]);
                    state[i] = State.IDLE;
                    break;
                default:
                    if (timer[i] > downBuffer)
                    {
                        Submit(i);
                    }
                    break;
            }

            if (stateCode[i] != "") timer[i] += Time.deltaTime;

            if (i == 0)
            {
                if (detectAnyKey)
                {
                    if (Input.anyKeyDown) KeyDown(i);
                    else if (!Input.anyKey) KeyUp(i);
                }
                else if (specificKey != KeyCode.None)
                {
                    if (Input.GetKeyDown(specificKey)) KeyDown(i);
                    else if (Input.GetKeyUp(specificKey)) KeyUp(i);
                }
            }
        }
    }

    void Submit(uint index)
    {
        value[index] = ECCommons.ListToString(separator, keys[index]) + separator;
        current[index] = "";
        stateCode[index] = "";
        timer[index] = 0;
        keys[index].Clear();
    }

    public static bool KeyCount(uint index, string key, int target)
    {
        int c = value[index].Contains(key) ? ECCommons.Separate(key, value[index]).Length : 0;
        if (c < target) return false;
        return true;
    }

    public static bool KeyCount(uint index, int target)
    {
        return KeyCount(index, tmpSep, target);
    }
    public static bool KeyCount(uint index)
    {
        return KeyCount(index, tmpSep, 1);
    }

    public static bool ClickCount(uint index, int target)
    {
        return KeyCount(index, "." + tmpSep, target);
    }
    public static bool ClickCount(uint index)
    {
        return KeyCount(index, "." + tmpSep, 1);
    }

    public static bool KeyHold(uint index)
    {
        return KeyCount(index, "_" + tmpSep, 1);
    }
    public static bool KeyIdle(uint index)
    {
        return state[index] == State.IDLE;
    }

    public static void Erase()
    {
        for(uint i = 0; i < value.Length; i++) value[i] = "";
    }

    public static void KeyDown(uint index, string button)
    {
        if (state[index] == State.IDLE) state[index] = State.DOWN;
        current[index] = button;
    }
    public static void KeyDown(uint index)
    {
        KeyDown(index, "Any");
    }
    public static void KeyUp(uint index)
    {
        if (state[index] == State.LOCK) state[index] = State.IDLE;
        else if (state[index] != State.IDLE) state[index] = State.UP;
    }

    /* --- Number == 1 --- */
    public static bool KeyCount(string key, int target)
    {
        return KeyCount(0, key, target);
    }

    public static bool KeyCount(int target)
    {
        return KeyCount(0, tmpSep, target);
    }
    public static bool KeyCount()
    {
        return KeyCount(0, tmpSep, 1);
    }

    public static bool ClickCount(int target)
    {
        return KeyCount(0, "." + tmpSep, target);
    }
    public static bool ClickCount()
    {
        return KeyCount(0, "." + tmpSep, 1);
    }

    public static bool KeyHold()
    {
        return KeyCount(0, "_" + tmpSep, 1);
    }
    public static bool KeyIdle()
    {
        return state[0] == State.IDLE;
    }

    public static void KeyDown(string button)
    {
        KeyDown(0, button);
    }
    public static void KeyDown()
    {
        KeyDown(0);
    }
    public static void KeyUp()
    {
        KeyUp(0);
    }

    /* --- Axis Setup --- */
    void AxisSetup()
    {
        axisExist = new bool[axisName.Length];
        axisStr = new string[axisName.Length];
        for (int i = 0; i < axisName.Length; i++)
        {
            axisStr[i] = axisName[i];
            StartCoroutine(CheckAxis(axisName[i], i));
        }
    }
    public static float AxisValue(uint index)
    {
        if (index >= axisExist.Length || !axisExist[index]) return 0;
        return Input.GetAxis(axisStr[index]);
    }

    IEnumerator CheckAxis(string name, int index)
    {
        axisExist[index] = false;
        yield return 0;
        Input.GetAxis(name);
        axisExist[index] = true;
    }
}
