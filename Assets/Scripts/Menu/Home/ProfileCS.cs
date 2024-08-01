using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProfileCS : MonoBehaviour
{
    public Image profileImage;
    public Sprite[] profileSprites;
    [HideInInspector] public int profileSpriteNumber = 1;

    private const int videosCnt = 2;
    private const int videoCntInHolder = 4;
    public RectTransform[] videosArrowTr;
    public GameObject[] videosHolder;
    public RectTransform[] videosTr;
    private Image a;
    private Image[,] videosImages = new Image[videosCnt + 1, videoCntInHolder + 1];
    private TMP_Text[,] videosTitles = new TMP_Text[videosCnt + 1, videoCntInHolder + 1];
    private TMP_Text[,] videosInfos = new TMP_Text[videosCnt + 1, videoCntInHolder + 1];
    public Animator[] screenAnim;

    private void Awake()
    {
        profileImage.sprite = profileSprites[profileSpriteNumber];

        for (int i = 1; i <= videosCnt; ++i)
        {
            videosArrowTr[i].rotation = Quaternion.Euler(0, 0, 0);
            videosHolder[i].SetActive(false);

            string name = "";
            switch (i)
            {
                case 1:
                    name = "UploadedVideos";
                    break;
                case 2:
                    name = "ToBeUploaded";
                    break;
            }

            for (int j = 1; j <= videoCntInHolder; ++j)
            {
                videosImages[i, j] = GameObject.Find(name).transform.Find("Videos").GetChild(j - 1).Find("Image").gameObject.GetComponent<Image>();
                videosImages[i, j].sprite = profileSprites[profileSpriteNumber];

                videosTitles[i, j] = GameObject.Find(name).transform.Find("Videos").GetChild(j - 1).Find("Title").gameObject.GetComponent<TMP_Text>();
                videosInfos[i, j] = GameObject.Find(name).transform.Find("Videos").GetChild(j - 1).Find("Info").gameObject.GetComponent<TMP_Text>();
            }
        } 
    }

    public void VideosArrowUp(int pushedButtonNum)
    {
        if (!videosHolder[pushedButtonNum].activeSelf)
        {
            for (int i = videosCnt; i >= 1; --i)
            {
                videosArrowTr[i].rotation = Quaternion.Euler(0, 0, 0);
                videosHolder[i].SetActive(false);
                if (i > 1 && videosHolder[i - 1].activeSelf) videosTr[i].anchoredPosition += Vector2.up * 400;
            }

            videosArrowTr[pushedButtonNum].rotation = Quaternion.Euler(0, 0, 90);
            videosHolder[pushedButtonNum].SetActive(true);

            for (int i = pushedButtonNum + 1; i <= videosCnt; ++i)
            {
                videosTr[i].anchoredPosition -= Vector2.up * 400;
            }
        }
    }
}
