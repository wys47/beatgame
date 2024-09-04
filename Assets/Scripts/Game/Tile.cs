using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : Variables
{
    public SpriteRenderer tileSprite;
    public Sprite[] tileImage = new Sprite[5];
    public GameObject tapSuccessEffect;
    public Animator anim;
    [SerializeField] private GameObject glowObj;
    public SpriteRenderer glowImageRenderer;

    private WaitForSeconds glowFadeTime = new WaitForSeconds(0.1f);
    private WaitForSeconds waitForSec = new WaitForSeconds(0.01f);
    private WaitUntil waitUntilAnimEnd;

    [HideInInspector] public int[] tileColor = new int[maxNodeDir];
    [HideInInspector] public int[] tileNodeTargetTileNum = new int[maxNodeDir];
    private bool[] tileEndNodeOnTargetTile = new bool[maxNodeDir];
    private int[] tempColor = new int[maxNodeDir];
    private int[] tempNodeTargetTileNum = new int[maxNodeDir];
    private bool[] tempEndNodeOnTargetTile = new bool[maxNodeDir];

    private float[] colorRGB = new float[4];
    private int fadeCnt;

    private TilePrefabCS tilePrefabCS;

    private void Awake()
    {
        waitUntilAnimEnd = new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
        tilePrefabCS = GameObject.Find("Tiles").GetComponent<TilePrefabCS>();
    }

    public void OnEnable()
    {
        for (int i = 0; i < maxNodeDir; ++i)
        {
            tileColorAndInfoToDefualt(i, false);
            tileColorAndInfoToDefualt(i, true);
        }

        fadeCnt = -1;
    }

    public void onGenerate(int tileNum)
    {
        gameObject.name = tileNum.ToString();

        tileSprite.sprite = tilePrefabCS.tileSprite[DeviceSettingCS.moduleSelected[1]];
        if (tileNum == 28)
        {
            tileSprite.sprite = tilePrefabCS.tileSpriteS[DeviceSettingCS.moduleSelected[1]];
        }
        else if (tileNum == 29)
        {
            tileSprite.sprite = tilePrefabCS.tileSpriteD[DeviceSettingCS.moduleSelected[1]];
        }
        else if (tileNum == 36)
        {
            tileSprite.sprite = tilePrefabCS.tileSpriteA[DeviceSettingCS.moduleSelected[1]];
        }
        else if (tileNum == 37)
        {
            tileSprite.sprite = tilePrefabCS.tileSpriteW[DeviceSettingCS.moduleSelected[1]];
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
                    if (tileNum + maxMapSize <= maxMapSize * maxMapSize) MapCtrl.tileCS[tileNum + maxMapSize].changeTileColorAndInfo(i, true, tileColor[i], tileNodeTargetTileNum[i]);
                    tileColorAndInfoToDefualt(i, false);
                }
                else if (i == 2)
                {
                    if (tileNum - maxMapSize >= 1) MapCtrl.tileCS[tileNum - maxMapSize].changeTileColorAndInfo(i, true, tileColor[i], tileNodeTargetTileNum[i]);
                    tileColorAndInfoToDefualt(i, false);
                }
                else if (i == 3)
                {
                    if ((tileNum - 1) % maxMapSize - 1 >= 0) MapCtrl.tileCS[tileNum - 1].changeTileColorAndInfo(i, true, tileColor[i], tileNodeTargetTileNum[i]);
                    tileColorAndInfoToDefualt(i, false);
                }
                else if (i == 4)
                {
                    if ((tileNum - 1) % maxMapSize + 1 <= maxMapSize - 1) MapCtrl.tileCS[tileNum + 1].changeTileColorAndInfo(i, true, tileColor[i], tileNodeTargetTileNum[i]);
                    tileColorAndInfoToDefualt(i, false);
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
                giveTileColorAndInfo(i, true, this, false, this);
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
                colorRGB[1] += MapCtrl.ColorByCode[tileColor[i]].r;
                colorRGB[2] += MapCtrl.ColorByCode[tileColor[i]].g;
                colorRGB[3] += MapCtrl.ColorByCode[tileColor[i]].b;

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
            if (fadeCnt < maxFadeCnt)
            {
                tileSprite.color = new Color(tileSprite.color.r * 0.7f, tileSprite.color.g * 0.7f, tileSprite.color.b * 0.7f, 1);
                if (glowObj.activeSelf) glowImageRenderer.color = tileSprite.color + Color.black * (glowImageRenderer.color.a * 0.8f - 1);
                ++fadeCnt;
            }
            else
            {
                tileSprite.color = MapCtrl.ColorByCode[0];
                StartCoroutine(glowImageSwitch(false));
                fadeCnt = -1;
            }
        }
    }

    public void tileColorAndInfoToDefualt(int dir, bool isTemp, bool updateColor = false)
    {
        if (!isTemp)
        {
            tileColor[dir] = 0;
            tileNodeTargetTileNum[dir] = 0;
            tileEndNodeOnTargetTile[dir] = false;
        }
        else
        {
            tempColor[dir] = 0;
            tempNodeTargetTileNum[dir] = 0;
            tempEndNodeOnTargetTile[dir] = false;
        }

        if (updateColor) updateTileColor();
    }
    public void giveTileColorAndInfo(int dir, bool isFromTemp, Tile from, bool isToTemp, Tile to, bool updateColor = false)
    {
        int color = isFromTemp ? from.tempColor[dir] : from.tileColor[dir];
        int targetTileNumber = isFromTemp ? from.tempNodeTargetTileNum[dir] : from.tileNodeTargetTileNum[dir];
        bool endNodeOnTargetTile = isFromTemp ? from.tempEndNodeOnTargetTile[dir] : from.tileEndNodeOnTargetTile[dir];

        to.changeTileColorAndInfo(dir, isToTemp, color, targetTileNumber, endNodeOnTargetTile ? 1 : 0);
        from.tileColorAndInfoToDefualt(dir, isFromTemp);

        if (updateColor) updateTileColor();
    }
    public void changeTileColorAndInfo(int dir, bool isTemp, int color = -1, int targetTileNumber = -1, int endNodeOnTargetTile = -1, bool updateColor = false)
    {
        if (!isTemp)
        {
            if (color != -1) tileColor[dir] = color;
            if (targetTileNumber != -1) tileNodeTargetTileNum[dir] = targetTileNumber;
            if (endNodeOnTargetTile != -1) tileEndNodeOnTargetTile[dir] = endNodeOnTargetTile == 1 ? true : false;
        }
        else
        {
            if (color != -1) tempColor[dir] = color;
            if (targetTileNumber != -1) tempNodeTargetTileNum[dir] = targetTileNumber;
            if (endNodeOnTargetTile != -1) tempEndNodeOnTargetTile[dir] = endNodeOnTargetTile == 1 ? true : false;
        }
        if (updateColor) updateTileColor();
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
        else glowImageRenderer.color = MapCtrl.ColorByCode[0];
    }
}
