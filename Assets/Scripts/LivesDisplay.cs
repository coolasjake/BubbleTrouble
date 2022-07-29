using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LivesDisplay : MonoBehaviour
{
    public List<Image> lives = new List<Image>();
    public Sprite active;
    public Sprite disabled;

    // Start is called before the first frame update
    void Start()
    {
        if (lives.Count == 0)
            lives.AddRange(GetComponentsInChildren<Image>());
    }

    public void SetLives(int val)
    {
        int i = 0;
        foreach (Image life in lives)
        {
            if (i < val)
                life.sprite = active;
            else
                life.sprite = disabled;
            ++i;
        }
    }
}
