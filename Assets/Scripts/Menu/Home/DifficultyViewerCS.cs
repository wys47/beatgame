using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DifficultyViewerCS : MonoBehaviour
{
    public Image[] image;
    private Color[,] color = new Color[5, 2]; 

    private int difficulty;

    void Awake()
    {
        color[1, 0] = new Color(0.52f, 0.52f, 0.52f, 1);
        color[1, 1] = new Color(1, 0.77f, 0, 1);

        color[2, 0] = new Color(0.37f, 0.37f, 0.37f, 1);
        color[2, 1] = new Color(1, 0.5f, 0, 1);

        color[3, 0] = new Color(0.29f, 0.29f, 0.29f, 1);
        color[3, 1] = new Color(1, 0, 0, 1);

        color[4, 0] = new Color(0.11f, 0.11f, 0.11f, 1);
        color[4, 1] = new Color(1, 0, 0, 1);
    }

    void OnEnable()
    {
        difficulty = 1;
        ChangeDifficulty();
    }

    private void ChangeDifficulty()
    {
        for (int i = 1; i <= 4; ++i)
        {
            if (i <= difficulty) image[i].color = color[difficulty, 1];
            else image[i].color = color[i, 0];
        }
    }

    public void OnDifficultyUp(int n)
    {
        difficulty = n;
        ChangeDifficulty();
    }
}
