using UnityEngine;
using System.Collections;

public enum Alignment
{
    UP = 0, 
    DOWN = 1, 
    LEFT = 2, 
    RIGHT = 3, 
    CENTER = 4, 
    UPPER_LEFT = 5, 
    UPPER_RIGHT = 6, 
    LOWER_LEFT = 7, 
    LOWER_RIGHT = 8
}
public enum ConnectionState
{
    DISCONNECTED = 0, 
    CONNECTING = 1, 
    CONNECTED = 2, 
    FULL = 3, 
    ERROR = 4
}
