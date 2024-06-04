using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildUIAlphaCS : MonoBehaviour
{
    public Color color;

    public void changeAlpha(float alpha)
    {
        color.a = alpha;
    }
}
