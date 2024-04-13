using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : Variables
{
    [HideInInspector] public MapCtrl mapCtrl;

    public SpriteRenderer tileSprite;
    public Sprite[] tileImage = new Sprite[5];
    public GameObject tapSuccessEffect;
    public Animator anim;
    public GameObject glowObj;
    public SpriteRenderer glowImageRenderer;

    private WaitForSeconds glowFadeTime = new WaitForSeconds(0.1f);
    private WaitForSeconds waitForSec = new WaitForSeconds(0.01f);
    private WaitUntil waitUntilAnimEnd;

    [HideInInspector] public int[] tileColor = new int[maxNodeDir];
    [HideInInspector] public int[] tileNodeTargetTileNum = new int[maxNodeDir];
    private int[] tempColor = new int[maxNodeDir];
    private int[] tempNodeTargetTileNum = new int[maxNodeDir];

    private float[] colorRGB = new float[4];
    private int fadeCnt;

    private void Awake()
    {
        waitUntilAnimEnd = new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
    }

    public void OnEnable()
    {
        for (int i = 0; i < maxNodeDir; ++i)
        {
            changeTileColorAndInfo(i, false, 0, 0);
            changeTileColorAndInfo(i, true, 0, 0);
        }

        fadeCnt = -1;
    }

    public void OnGenerate(int tileNum)
    {
        gameObject.name = tileNum.ToString();
        if (tileNum == 28)
        {
            tileSprite.sprite = tileImage[1];
            transform.Translate(new Vector2(-0.03f, -0.03f));
        }
        else if (tileNum == 29)
        {
            tileSprite.sprite = tileImage[2];
            transform.Translate(new Vector2(0.03f, -0.03f));
        }
        else if (tileNum == 36)
        {
            tileSprite.sprite = tileImage[3];
            transform.Translate(new Vector2(-0.03f, 0.03f));
        }
        else if (tileNum == 37)
        {
            tileSprite.sprite = tileImage[4];
            transform.Translate(new Vector2(0.03f, 0.03f));
        }
    }

    public void changeTempTileColor(int tileNum)
    {
        for (int i = 0; i < maxNodeDir; ++i)
        {
            if (tileColor[i] != 0)
            {
                if (i == 1)
                {
                    if (tileNum + maxMapSize <= maxMapSize * maxMapSize) mapCtrl.tileCS[tileNum + maxMapSize].changeTileColorAndInfo(i, true, tileColor[i], tileNodeTargetTileNum[i]);
                    changeTileColorAndInfo(i, false, 0, 0);
                }
                else if (i == 2)
                {
                    if (tileNum - maxMapSize >= 1) mapCtrl.tileCS[tileNum - maxMapSize].changeTileColorAndInfo(i, true, tileColor[i], tileNodeTargetTileNum[i]);
                    changeTileColorAndInfo(i, false, 0, 0);
                }
                else if (i == 3)
                {
                    if ((tileNum - 1) % maxMapSize - 1 >= 0) mapCtrl.tileCS[tileNum - 1].changeTileColorAndInfo(i, true, tileColor[i], tileNodeTargetTileNum[i]);
                    changeTileColorAndInfo(i, false, 0, 0);
                }
                else if (i == 4)
                {
                    if ((tileNum - 1) % maxMapSize + 1 <= maxMapSize - 1) mapCtrl.tileCS[tileNum + 1].changeTileColorAndInfo(i, true, tileColor[i], tileNodeTargetTileNum[i]);
                    changeTileColorAndInfo(i, false, 0, 0);
                }
            }
        }
    }
    public void changeTileColorFromTemp()
    {
        for (int i = 1; i < maxNodeDir; ++i)
        {
            if (tempColor[i] != 0)
            {
                changeTileColorAndInfo(i, false, tempColor[i], tempNodeTargetTileNum[i]);
                changeTileColorAndInfo(i, true, 0, 0);
            }
        }
    }

    public void updateTileColor()
    {
        int cnt = 0;
        for (int i = 1; i <= 3; ++i) colorRGB[i] = 0;

        for (int i = 0; i < maxNodeDir; ++i)
        {
            if (tileColor[i] != 0)
            {
                colorRGB[1] += mapCtrl.ColorByCode[tileColor[i]].r;
                colorRGB[2] += mapCtrl.ColorByCode[tileColor[i]].g;
                colorRGB[3] += mapCtrl.ColorByCode[tileColor[i]].b;

                ++cnt;
            }
        }

        if (cnt != 0)
        {
            tileSprite.color = new Color(colorRGB[1] / cnt, colorRGB[2] / cnt, colorRGB[3] / cnt);
            fadeCnt = 0;
        }
        else if (fadeCnt != -1)
        {
            if (fadeCnt < 5)
            {
                tileSprite.color = new Color(tileSprite.color.r * 0.7f, tileSprite.color.g * 0.7f, tileSprite.color.b * 0.7f, 1);
                if (glowObj.activeSelf) glowImageRenderer.color = tileSprite.color + Color.black * (glowImageRenderer.color.a * 0.8f - 1);
                ++fadeCnt;
            }
            else
            {
                tileSprite.color = mapCtrl.ColorByCode[0];
                StartCoroutine(glowImageSwitch(false));
                fadeCnt = -1;
            }
        }
    }

    public void changeTileColorAndInfo(int dir, bool isTemp, int color = -1, int targetTileNumber = -1)
    {
        if (!isTemp)
        {
            if (color != -1) tileColor[dir] = color;
            if (targetTileNumber != -1) tileNodeTargetTileNum[dir] = targetTileNumber;
        }
        else
        {
            if (color != -1) tempColor[dir] = color;
            if (targetTileNumber != -1) tempNodeTargetTileNum[dir] = targetTileNumber;
        }
    }

    public IEnumerator ActivateTapSuccessEffect()
    {
        tapSuccessEffect.SetActive(true);
        anim.Play("tap_success_anim", -1, 0f);
        yield return waitUntilAnimEnd;
        tapSuccessEffect.SetActive(false);
    }

    public IEnumerator glowImageSwitch(bool onOff, bool fade = false)
    {
        glowObj.SetActive(onOff);
        if (onOff == true)
        {
            glowImageRenderer.color = tileSprite.color;
            if (fade)
            {
                for (int i = 1; i <= 5; ++i)
                {
                    yield return glowFadeTime;
                    glowImageRenderer.color = tileSprite.color + Color.black * (glowImageRenderer.color.a * 0.8f - 1);
                }
                yield return glowFadeTime;
                StartCoroutine(glowImageSwitch(false));
            }
        }
        else glowImageRenderer.color = mapCtrl.ColorByCode[0];
    }
}
