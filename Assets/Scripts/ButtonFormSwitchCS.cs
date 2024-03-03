using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonFormSwitchCS : MonoBehaviour
{
    public Image image;

    public Sprite defealtImage;
    public Sprite enterImage;

    void OnEnable()
    {
        image.sprite = defealtImage;
    }

    public void OnPointerEnter()
    {
        image.sprite = enterImage;
    }

    public void OnPointerExit()
    {
        image.sprite = defealtImage;
    }
}
