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
            int pairDir = 0;

            while (true)
            {
                loop = false;
                dir = Random.Range(1, 5);

                if (dir == 1)
                {
                    targetTileNum = Random.Range(maxMapSize * (diff - 1) + maxMapSize - diff + 1, maxMapSize * (diff - 1) + (int)(maxMapSize * 0.5f) + 1);
                    int line = (targetTileNum - 1) % maxMapSize + 1;

                    if (mapCtrl.tileCS[line].tileColor[dir] != 0) loop = true;
                    for (int i = 1; i <= maxMapSize; ++i)
                    {
                        if (mapCtrl.tileCS[maxMapSize * (i - 1) + line].tileColor[2] != 0 && mapCtrl.tileCS[maxMapSize * (i - 1) + line].tileColor[0] != 0)
                        {
                            loop = true;
                            break;
                        }
                    }

                    pairActiveTiming = ActivatedTiming + ((targetTileNum - 1) / maxMapSize - (maxMapSize - line)) * PlusBeatInOneUpdate;
                    pairDir = 3;
                    mapCtrl.tileCS[line].changeTileColorAndInfo(dir, false, color, targetTileNum);
                }
                else if (dir == 2)
                {
                    targetTileNum = Random.Range(maxMapSize * (maxMapSize - diff) + maxMapSize - diff + 1, maxMapSize * (maxMapSize - diff) + diff);
                    int line = (targetTileNum - 1) % maxMapSize + 1;

                    if (mapCtrl.tileCS[maxMapSize * (maxMapSize - 1) + line].tileColor[dir] != 0) loop = true;
                    for (int i = 1; i <= maxMapSize; ++i)
                    {
                        if (mapCtrl.tileCS[maxMapSize * (i - 1) + line].tileColor[1] != 0 && mapCtrl.tileCS[maxMapSize * (i - 1) + line].tileColor[0] != 0)
                        {
                            loop = true;
                            break;
                        }
                    }

                    if (line <= maxMapSize * 0.5f)
                    {
                        pairActiveTiming = ActivatedTiming + (maxMapSize - (targetTileNum - 1) / maxMapSize - (maxMapSize - line + 1)) * PlusBeatInOneUpdate;
                        pairDir = 3;
                    }
                    else
                    {
                        pairActiveTiming = ActivatedTiming + (maxMapSize - (targetTileNum - 1) / maxMapSize - line) * PlusBeatInOneUpdate;
                        pairDir = 4;
                    }

                    mapCtrl.tileCS[maxMapSize * (maxMapSize - 1) + line].changeTileColorAndInfo(dir, false, color, targetTileNum);
                }
                else if (dir == 3)
                {
                    targetTileNum = maxMapSize * Random.Range(maxMapSize - diff, diff - 1) + maxMapSize - diff + 1;
                    int line = (targetTileNum - 1) / maxMapSize;

                    if (mapCtrl.tileCS[line * maxMapSize + maxMapSize].tileColor[dir] != 0) loop = true;
                    for (int i = 1; i <= maxMapSize; ++i)
                    {
                        if (mapCtrl.tileCS[line * maxMapSize + i].tileColor[4] != 0 && mapCtrl.tileCS[line * maxMapSize + i].tileColor[0] != 0)
                        {
                            loop = true;
                            break;
                        }
                    }

                    if (line + 1 <= maxMapSize * 0.5f)
                    {
                        pairActiveTiming = ActivatedTiming + (maxMapSize - ((targetTileNum - 1) % maxMapSize) - (maxMapSize - line)) * PlusBeatInOneUpdate;
                        pairDir = 2;
                    }
                    else
                    {
                        pairActiveTiming = ActivatedTiming + (maxMapSize - ((targetTileNum - 1) % maxMapSize) - (line + 1)) * PlusBeatInOneUpdate;
                        pairDir = 1;
                    }

                    mapCtrl.tileCS[line * maxMapSize + maxMapSize].changeTileColorAndInfo(dir, false, color, targetTileNum);
                }
                else if (dir == 4)
                {
                    targetTileNum = maxMapSize * Random.Range(maxMapSize - diff, (int)(maxMapSize * 0.5f)) + diff;
                    int line = (targetTileNum - 1) / maxMapSize;

                    if (mapCtrl.tileCS[line * maxMapSize + 1].tileColor[dir] != 0) loop = true;
                    for (int i = 1; i <= maxMapSize; ++i)
                    {
                        if (mapCtrl.tileCS[line * maxMapSize + i].tileColor[3] != 0 && mapCtrl.tileCS[line * maxMapSize + i].tileColor[0] != 0)
                        {
                            loop = true;
                            break;
                        }
                    }

                    pairActiveTiming = ActivatedTiming + ((targetTileNum - 1) % maxMapSize - (maxMapSize - line - 1)) * PlusBeatInOneUpdate;
                    pairDir = 2;

                    mapCtrl.tileCS[line * maxMapSize + 1].changeTileColorAndInfo(dir, false, color, targetTileNum);
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

                    if (mapCtrl.tileCS[line].tileColor[dir] != 0) dir = 0;
                    for (int i = 1; i <= maxMapSize; ++i)
                    {
                        if (mapCtrl.tileCS[maxMapSize * (i - 1) + line].tileColor[2] != 0)
                        {
                            dir = 0;
                            break;
                        }
                    }

                    int genTileNum = 0;
                    if (dir != 0) genTileNum = line;
                    else genTileNum = targetTileNum;
                    mapCtrl.tileCS[genTileNum].changeTileColorAndInfo(dir, false, color, 0);
                }
                else if (dir == 2)
                {
                    int line = (targetTileNum - 1) % maxMapSize + 1;

                    if (mapCtrl.tileCS[maxMapSize * (maxMapSize - 1) + line].tileColor[dir] != 0) dir = 0;
                    for (int i = 1; i <= maxMapSize; ++i)
                    {
                        if (mapCtrl.tileCS[maxMapSize * (i - 1) + line].tileColor[1] != 0)
                        {
                            dir = 0;
                            break;
                        }
                    }

                    int genTileNum = 0;
                    if (dir != 0) genTileNum = maxMapSize * (maxMapSize - 1) + line;
                    else genTileNum = targetTileNum;
                    mapCtrl.tileCS[genTileNum].changeTileColorAndInfo(dir, false, color, 0);
                }
                else if (dir == 3)
                {
                    int line = (targetTileNum - 1) / maxMapSize;

                    if (mapCtrl.tileCS[line * maxMapSize + maxMapSize].tileColor[dir] != 0) dir = 0;
                    for (int i = 1; i <= maxMapSize; ++i)
                    {
                        if (mapCtrl.tileCS[line * maxMapSize + i].tileColor[4] != 0)
                        {
                            dir = 0;
                            break;
                        }
                    }

                    int genTileNum = 0;
                    if (dir != 0) genTileNum = line * maxMapSize + maxMapSize;
                    else genTileNum = targetTileNum;
                    mapCtrl.tileCS[genTileNum].changeTileColorAndInfo(dir, false, color, 0);
                }
                else if (dir == 4)
                {
                    int line = (targetTileNum - 1) / maxMapSize;

                    if (mapCtrl.tileCS[line * maxMapSize + 1].tileColor[dir] != 0) dir = 0;
                    for (int i = 1; i <= maxMapSize; ++i)
                    {
                        if (mapCtrl.tileCS[line * maxMapSize + i].tileColor[3] != 0)
                        {
                            dir = 0;
                            break;
                        }
                    }

                    int genTileNum = 0;
                    if (dir != 0) genTileNum = line * maxMapSize + 1;
                    else genTileNum = targetTileNum;
                    mapCtrl.tileCS[genTileNum].changeTileColorAndInfo(dir, false, color, 0);
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