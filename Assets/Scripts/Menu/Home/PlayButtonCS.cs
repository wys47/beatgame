using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayButtonCS : MonoBehaviour
{
    public Image image;

    public Sprite[] defealtImage;
    public Sprite[] enterImage;

    private int difficulty;

    void OnEnable()
    {
        difficulty = 1;
        image.sprite = defealtImage[difficulty];
    }

    public void OnPointerEnter()
    {
        image.sprite = enterImage[difficulty];
    }

    public void OnPointerExit()
    {
        image.sprite = defealtImage[difficulty];
    }

    public void OnDifficultyUp(int n)
    {
        difficulty = n;
        image.sprite = defealtImage[difficulty];
    }
}
