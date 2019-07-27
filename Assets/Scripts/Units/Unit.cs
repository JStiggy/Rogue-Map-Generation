using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public enum Status { good, poison, paralysis, sleep, pertification, dead}

    public int x; public int y;

    public string name;
    public int MAX_HP = 999;
    public int MAX_MP = 999;
    public int HP = 999;
    public int MP = 999;
    public int STR = 100;
    public int VIT = 100;

    public Status status = Status.good;
}
