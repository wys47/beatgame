using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeCS : Variables
{
    private MapCtrl mapCtrl;

    [HideInInspector] public float ActivatedTiming;
    [HideInInspector] public float timing;
    [HideInInspector] public int color;
    [HideInInspector] public int dir;
    [HideInInspector] public int targetZone;
    [HideInInspector] public int targetTileNum;
    [HideInInspector] public int generateMode;

    private void Awake()
    {
        mapCtrl = GameObject.Find("MapCtrlObject").GetComponent<MapCtrl>();
    }

    public void generate()
    {
        if (generateMode == 1)
        {
            bool loop;

            int diff = (int)((timing - (ActivatedTiming - PlusBeatInOneUpdate)) / PlusBeatInOneUpdate);
            float pairActiveTiming = 0;
            int pairDir = Random.Range(1, 11);

            while (true)
            {
                loop = false;
                if (targetZone == 0) dir = Random.Range(1, 5);
                else if (targetZone == 1) dir = 1;
                else if (targetZone == 2) dir = 2;//2번째의 경우만 예외처리함
                else if (targetZone == 3) dir = 4;

                if (dir == 1)
                {
                    targetTileNum = Random.Range(maxMapSize * (diff - 1) + maxMapSize - diff + 1, maxMapSize * (diff - 1) + (int)(maxMapSize * 0.5f) + 1);
                    int line = (targetTileNum - 1) % maxMapSize + 1;

                    if (MapCtrl.tileCS[line].tileColor[dir] != 0) loop = true;
                    for (int i = 1; i <= maxMapSize; ++i)
                    {
                        if (MapCtrl.tileCS[maxMapSize * (i - 1) + line].tileColor[2] != 0 && MapCtrl.tileCS[maxMapSize * (i - 1) + line].tileColor[0] != 0)
                        {
                            loop = true;
                            break;
                        }
                    }

                    pairActiveTiming = ActivatedTiming + ((targetTileNum - 1) / maxMapSize - (maxMapSize - line)) * PlusBeatInOneUpdate;

                    if (pairDir <= unpairTapNodeGeneratePosibility * 10 && MapCtrl.difficulty >= 2) pairDir = 0;
                    else pairDir = 3;

                    MapCtrl.tileCS[line].changeTileColorAndInfo(dir, false, color, targetTileNum);
                }
                else if (dir == 2)
                {
                    targetTileNum = Random.Range(maxMapSize * (maxMapSize - diff) + maxMapSize - diff + 1, maxMapSize * (maxMapSize - diff) + (targetZone == 2 ? (int)(maxMapSize * 0.5f) + 1 : diff));
                    int line = (targetTileNum - 1) % maxMapSize + 1;

                    if (MapCtrl.tileCS[maxMapSize * (maxMapSize - 1) + line].tileColor[dir] != 0) loop = true;
                    for (int i = 1; i <= maxMapSize; ++i)
                    {
                        if (MapCtrl.tileCS[maxMapSize * (i - 1) + line].tileColor[1] != 0 && MapCtrl.tileCS[maxMapSize * (i - 1) + line].tileColor[0] != 0)
                        {
                            loop = true;
                            break;
                        }
                    }

                    if (line <= maxMapSize * 0.5f)
                    {
                        pairActiveTiming = ActivatedTiming + (maxMapSize - (targetTileNum - 1) / maxMapSize - (maxMapSize - line + 1)) * PlusBeatInOneUpdate;
                        if (pairDir <= unpairTapNodeGeneratePosibility * 10 && MapCtrl.difficulty >= 2) pairDir = 0;
                        else pairDir = 3;
                    }
                    else
                    {
                        pairActiveTiming = ActivatedTiming + (maxMapSize - (targetTileNum - 1) / maxMapSize - line) * PlusBeatInOneUpdate;
                        if (pairDir <= unpairTapNodeGeneratePosibility * 10 && MapCtrl.difficulty >= 2) pairDir = 0;
                        else pairDir = 4;
                    }

                    MapCtrl.tileCS[maxMapSize * (maxMapSize - 1) + line].changeTileColorAndInfo(dir, false, color, targetTileNum);
                }
                else if (dir == 3)
                {
                    targetTileNum = maxMapSize * Random.Range(maxMapSize - diff, diff - 1) + maxMapSize - diff + 1;
                    int line = (targetTileNum - 1) / maxMapSize;

                    if (MapCtrl.tileCS[line * maxMapSize + maxMapSize].tileColor[dir] != 0) loop = true;
                    for (int i = 1; i <= maxMapSize; ++i)
                    {
                        if (MapCtrl.tileCS[line * maxMapSize + i].tileColor[4] != 0 && MapCtrl.tileCS[line * maxMapSize + i].tileColor[0] != 0)
                        {
                            loop = true;
                            break;
                        }
                    }

                    if (line + 1 <= maxMapSize * 0.5f)
                    {
                        pairActiveTiming = ActivatedTiming + (maxMapSize - ((targetTileNum - 1) % maxMapSize) - (maxMapSize - line)) * PlusBeatInOneUpdate;
                        if (pairDir <= unpairTapNodeGeneratePosibility * 10 && MapCtrl.difficulty >= 2) pairDir = 0;
                        else pairDir = 2;
                    }
                    else
                    {
                        pairActiveTiming = ActivatedTiming + (maxMapSize - ((targetTileNum - 1) % maxMapSize) - (line + 1)) * PlusBeatInOneUpdate;
                        if (pairDir <= unpairTapNodeGeneratePosibility * 10 && MapCtrl.difficulty >= 2) pairDir = 0;
                        else pairDir = 1;
                    }

                    MapCtrl.tileCS[line * maxMapSize + maxMapSize].changeTileColorAndInfo(dir, false, color, targetTileNum);
                }
                else if (dir == 4)
                {
                    targetTileNum = maxMapSize * Random.Range(maxMapSize - diff, (int)(maxMapSize * 0.5f)) + diff;
                    int line = (targetTileNum - 1) / maxMapSize;

                    if (MapCtrl.tileCS[line * maxMapSize + 1].tileColor[dir] != 0) loop = true;
                    for (int i = 1; i <= maxMapSize; ++i)
                    {
                        if (MapCtrl.tileCS[line * maxMapSize + i].tileColor[3] != 0 && MapCtrl.tileCS[line * maxMapSize + i].tileColor[0] != 0)
                        {
                            loop = true;
                            break;
                        }
                    }

                    pairActiveTiming = ActivatedTiming + ((targetTileNum - 1) % maxMapSize - (maxMapSize - line - 1)) * PlusBeatInOneUpdate;
                    if (pairDir <= unpairTapNodeGeneratePosibility * 10 && MapCtrl.difficulty >= 2) pairDir = 0;
                    else pairDir = 2;

                    MapCtrl.tileCS[line * maxMapSize + 1].changeTileColorAndInfo(dir, false, color, targetTileNum);
                }

                if (!loop) break;
            }

            mapCtrl.zoneActiveInfo[mapCtrl.zoneByTileNum(targetTileNum)].colorCode = color;
            mapCtrl.zoneActiveInfo[mapCtrl.zoneByTileNum(targetTileNum)].timing = timing;

            mapCtrl.activateNode(pairActiveTiming, timing, color, 2, targetTileNum, pairDir);
            deActivate();
        }
    }

    // Update is called once per frame
    public void beatCntUpdated(float BeatCnt)
    {
        if (generateMode == 2)
        {
            if (ActivatedTiming == BeatCnt)
            {
                if (dir == 1)
                {
                    int line = (targetTileNum - 1) % maxMapSize + 1;

                    if (MapCtrl.tileCS[line].tileColor[dir] != 0) dir = 0;
                    for (int i = 1; i <= maxMapSize; ++i)
                    {
                        if (MapCtrl.tileCS[maxMapSize * (i - 1) + line].tileColor[2] != 0)
                        {
                            dir = 0;
                            break;
                        }
                    }

                    int genTileNum;
                    if (dir != 0) genTileNum = line;
                    else genTileNum = targetTileNum;
                    print(genTileNum);
                    MapCtrl.tileCS[genTileNum].changeTileColorAndInfo(dir, false, color, 0);
                }
                else if (dir == 2)
                {
                    int line = (targetTileNum - 1) % maxMapSize + 1;

                    if (MapCtrl.tileCS[maxMapSize * (maxMapSize - 1) + line].tileColor[dir] != 0) dir = 0;
                    for (int i = 1; i <= maxMapSize; ++i)
                    {
                        if (MapCtrl.tileCS[maxMapSize * (i - 1) + line].tileColor[1] != 0)
                        {
                            dir = 0;
                            break;
                        }
                    }

                    int genTileNum;
                    if (dir != 0) genTileNum = maxMapSize * (maxMapSize - 1) + line;
                    else genTileNum = targetTileNum;
                    MapCtrl.tileCS[genTileNum].changeTileColorAndInfo(dir, false, color, 0);
                }
                else if (dir == 3)
                {
                    int line = (targetTileNum - 1) / maxMapSize;

                    if (MapCtrl.tileCS[line * maxMapSize + maxMapSize].tileColor[dir] != 0) dir = 0;
                    for (int i = 1; i <= maxMapSize; ++i)
                    {
                        if (MapCtrl.tileCS[line * maxMapSize + i].tileColor[4] != 0)
                        {
                            dir = 0;
                            break;
                        }
                    }

                    int genTileNum;
                    if (dir != 0) genTileNum = line * maxMapSize + maxMapSize;
                    else genTileNum = targetTileNum;
                    MapCtrl.tileCS[genTileNum].changeTileColorAndInfo(dir, false, color, 0);
                }
                else if (dir == 4)
                {
                    int line = (targetTileNum - 1) / maxMapSize;

                    if (MapCtrl.tileCS[line * maxMapSize + 1].tileColor[dir] != 0) dir = 0;
                    for (int i = 1; i <= maxMapSize; ++i)
                    {
                        if (MapCtrl.tileCS[line * maxMapSize + i].tileColor[3] != 0)
                        {
                            dir = 0;
                            break;
                        }
                    }

                    int genTileNum;
                    if (dir != 0) genTileNum = line * maxMapSize + 1;
                    else genTileNum = targetTileNum;
                    MapCtrl.tileCS[genTileNum].changeTileColorAndInfo(dir, false, color, 0);
                }
                else if (dir == 0)
                {
                    int genTileNum;
                    genTileNum = targetTileNum;
                    MapCtrl.tileCS[genTileNum].changeTileColorAndInfo(dir, false, color, 0);
                }

                deActivate();
            }
        }
    }

    void deActivate()
    {
        gameObject.SetActive(false);
    }
}