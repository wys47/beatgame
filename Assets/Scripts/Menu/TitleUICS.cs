using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleUICS : MonoBehaviour
{
    public void onDisableAnimEnd()
    {
        gameObject.SetActive(false);
    }
}
