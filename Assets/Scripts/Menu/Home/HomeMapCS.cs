using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HomeMapCS : MonoBehaviour
{
    public TMP_Text nameText;
    public TMP_Text detailText;

    public Image background;
    public Sprite[] backgroundSprites;

    public void changeText(int destinationNum)
    {
        string name;
        string detail;

        switch (destinationNum)
        {
            case 1:
                name = "";
                break;
        }
    }
}
