using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public float levelTime;
    public List<Ball> balls = new List<Ball>();
}

[System.Serializable]
public struct BallAppearance
{
    public Color col;
    public Sprite sprite;
}
