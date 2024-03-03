using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCtrl : Variables //게임 시작이 되면 맵을 생성하고 게임을 진행하는 역할을 한다.
{
    //게임 규모
    private const int maxAudioPlayer = 3;
    private const int maxNode = 40;
    private const float maxMapAnimationPlayBeat = 1;

    //게임 수치 변수
    [HideInInspector]
    public Color[] ColorByCode = new Color[maxColor + 2];
    private const float zoneAlpha = 0.7f;

    //UI
    public GameUIManager gameUIManager;

    //음악
    public MusicManager musicManager;
    private WaitForSeconds waitForStartFuzz = new WaitForSeconds(2f);
    private WaitForSeconds[] wiatForMusicStart = new WaitForSeconds[100];
    private float beatSensorRotateArc;

    public NodesTimingCS nodesTimingCS;

    //맵메이킹
    public int makeMode;

    //오디오클립
    public AudioClip[] gameSound;

    //오디오소스
    public AudioSource musicPlayer;
    public AudioSource[] gameSoundPlayers;

    //게임 컴포넌트
    public Transform tilesHolder;
    public GameObject tileObj;
    [HideInInspector] public Tile[] tileCS = new Tile[maxMapSize * maxMapSize + 1];

    public GameObject[] zoneObj;
    private SpriteRenderer[] zoneSprites = new SpriteRenderer[4];
    [HideInInspector] public float[,] zoneActiveInfo = new float[4, 3];//1.컬러
    private WaitForSeconds zoneFadeTime = new WaitForSeconds(0.05f);

    public GameObject mapGlowObj;
    private SpriteRenderer mapGlowSprite;
    private WaitForSeconds[] animationDelay = new WaitForSeconds[4];

    public Transform nodesHolder;
    public GameObject nodeObj;
    private GameObject[] nodeCopys = new GameObject[maxNode + 1];
    [HideInInspector] public NodeCS[] nodeCS = new NodeCS[maxNode + 1];

    public Transform beatSensor;

    //게임관리용 변수
    private int currentPlayingTrack;

    private int gameState;
    [HideInInspector] public float beatCnt;
    private bool afterBeatCntPlus;

    private float[,] nodeActiveInfo = new float[maxTiming, 2];//0.발동타이밍,1.타이밍
    private int nodeCntActive;
    private int nodeCycle;

    private int genClickNodeCnt;
    private int[] clickNodeGenPos = new int[(int)(maxMapSize * maxMapSize * 0.25f) + 1];//0.총 소환할 클릭노드 수

    private int[] palette = new int[maxColor + 1];
    private int useColorCnt;

    private int[,] collide = new int[4, 3];//0.타일넘버,1.우선 노드 순위,2.방향

    private int itsTime;

    //디버깅용 변수
    private int debug = 0;

    void Awake()
    {
        ColorByCode[0] = new Color(0.2f, 0.2f, 0.2f, 1);
        ColorByCode[1] = new Color(1, 0, 0, 1);
        ColorByCode[2] = new Color(1, 0.5f, 0, 1);
        ColorByCode[3] = new Color(1, 1, 0, 1);
        ColorByCode[4] = new Color(0.65f, 1, 0, 1);
        ColorByCode[5] = new Color(0, 0.8f, 0, 1);
        ColorByCode[6] = new Color(0, 1, 0.8f, 1);
        ColorByCode[7] = new Color(0, 0.96f, 1, 1);
        ColorByCode[8] = new Color(0, 0, 1, 1);
        ColorByCode[9] = new Color(0.67f, 0, 1, 1);
        ColorByCode[10] = new Color(1, 0, 0.7f, 1);
        ColorByCode[maxColor + 1] = new Color(1, 1, 1, 1);

        clickNodeGenPos[1] = 64;
        clickNodeGenPos[2] = 63;
        clickNodeGenPos[3] = 56;
        clickNodeGenPos[4] = 62;
        clickNodeGenPos[5] = 55;
        clickNodeGenPos[6] = 48;
        clickNodeGenPos[7] = 61;
        clickNodeGenPos[8] = 54;
        clickNodeGenPos[9] = 47;
        clickNodeGenPos[10] = 40;
        clickNodeGenPos[11] = 53;
        clickNodeGenPos[12] = 46;
        clickNodeGenPos[13] = 39;
        clickNodeGenPos[14] = 45;
        clickNodeGenPos[15] = 38;
        clickNodeGenPos[16] = 37;

        for (int y = 1; y <= maxMapSize; ++y)
        {
            for (int x = 1; x <= maxMapSize; ++x)
            {
                GameObject tile = Instantiate(tileObj, tilesHolder);
                tile.transform.position = new Vector2(x * 0.7f - 0.25f - maxMapSize * 0.35f, y * 0.7f - 0.25f - maxMapSize * 0.35f);
                tileCS[(y - 1) * maxMapSize + x] = tile.GetComponent<Tile>();
                tileCS[(y - 1) * maxMapSize + x].mapCtrl = gameObject.GetComponent<MapCtrl>();
                tileCS[(y - 1) * maxMapSize + x].OnGenerate((y - 1) * maxMapSize + x);
            }
        }
        for (int i = 0; i <= 3; ++i)
        {
            zoneSprites[i] = zoneObj[i].GetComponent<SpriteRenderer>();
            zoneObj[i].SetActive(false);
        }
        mapGlowSprite = mapGlowObj.GetComponent<SpriteRenderer>();
        tilesHolder.Rotate(0, 0, 45);

        for (int i = 1; i <= musicManager.musicCnt; ++i)
        {
            wiatForMusicStart[i] = new WaitForSeconds(musicManager.musicStartTime[i]);
            beatSensorRotateArc = musicManager.musicBPM[i] / 5f;
        }

        for (int i = 1; i <= maxNode; ++i)
        {
            nodeCopys[i] = Instantiate(nodeObj, nodesHolder);
            nodeCS[i] = nodeCopys[i].GetComponent<NodeCS>();
            nodeCopys[i].SetActive(false);
        }
    }

    void OnEnable()
    {
        StartCoroutine(onEnabledFunc());
    }

    void Update()
    {
        if (gameState == 0)
        {
            onFirstUpdate();
            gameState = 1;
        }
        if (gameState == 1)
        {
            if (itsTime == 0) StartCoroutine(timer(wiatForMusicStart[currentPlayingTrack]));
            if (itsTime == 2)
            {
                gameState = 2;
                itsTime = 0;
            }
        }
        if (gameState == 2)
        {
            if (afterBeatCntPlus)
            {
                for (int i = 1; i <= maxMapSize * maxMapSize; ++i) tileCS[i].updateTileColor();

                for (int i = 1; i <= 3; ++i)
                {
                    collide[i, 0] = 0;
                    collide[i, 1] = 0;
                    collide[i, 2] = 0;
                }

                for (int i = 1; i <= maxMapSize * maxMapSize; ++i)
                {
                    for (int k = 1; k < maxNodeDir; ++k)
                    {
                        int targetTileNum = tileCS[i].tileNodeTargetTileNum[k, 0];
                        if (targetTileNum == 0) continue;
                        int zone = zoneByTileNum(targetTileNum);

                        if (k == 1)
                        {
                            int diff = targetTileNum - i;

                            if (diff == maxMapSize)
                            {
                                if (collide[zone, 1] == 0)
                                {
                                    collide[zone, 0] = i;
                                    collide[zone, 1] = 1;
                                    collide[zone, 2] = k;
                                }
                            }
                            else if (diff == 0)
                            {
                                collide[zone, 0] = i;
                                collide[zone, 1] = 2;
                                collide[zone, 2] = k;

                                if (tileCS[i].tileColor[0] == tileCS[i].tileColor[k]) tileCS[i].changeTileColorAndInfo(0, false, 0, 0, 0);
                            }
                        }
                        else if (k == 2)
                        {
                            int diff = i - targetTileNum;

                            if (diff == maxMapSize)
                            {
                                if (collide[zone, 1] == 0)
                                {
                                    collide[zone, 0] = i;
                                    collide[zone, 1] = 1;
                                    collide[zone, 2] = k;
                                }
                            }
                            else if (diff == 0)
                            {
                                collide[zone, 0] = i;
                                collide[zone, 1] = 2;
                                collide[zone, 2] = k;

                                if (tileCS[i].tileColor[0] == tileCS[i].tileColor[k]) tileCS[i].changeTileColorAndInfo(0, false, 0, 0, 0);
                            }
                        }
                        else if (k == 3)
                        {
                            int diff = i - targetTileNum;

                            if (diff == 1)
                            {
                                if (collide[zone, 1] == 0)
                                {
                                    collide[zone, 0] = i;
                                    collide[zone, 1] = 1;
                                    collide[zone, 2] = k;
                                }
                            }
                            else if (diff == 0)
                            {
                                collide[zone, 0] = i;
                                collide[zone, 1] = 2;
                                collide[zone, 2] = k;

                                if (tileCS[i].tileColor[0] == tileCS[i].tileColor[k]) tileCS[i].changeTileColorAndInfo(0, false, 0, 0, 0);
                            }
                        }
                        else if (k == 4)
                        {
                            int diff = targetTileNum - i;

                            if (diff == 1)
                            {
                                if (collide[zone, 1] == 0)
                                {
                                    collide[zone, 0] = i;
                                    collide[zone, 1] = 1;
                                    collide[zone, 2] = k;
                                }
                            }
                            else if (diff == 0)
                            {
                                collide[zone, 0] = i;
                                collide[zone, 1] = 2;
                                collide[zone, 2] = k;

                                if (tileCS[i].tileColor[0] == tileCS[i].tileColor[k]) tileCS[i].changeTileColorAndInfo(0, false, 0, 0, 0);
                            }
                        }
                    }
                }

                for (int i = 1; i <= 3; ++i)
                {
                    if (zoneActiveInfo[i, 1] != 0)
                    {
                        if (zoneActiveInfo[i, 2] != beatCnt)
                        {
                            if (!zoneObj[i].activeSelf) StartCoroutine(zoneFade(i, (int)zoneActiveInfo[i, 1], true));
                        }
                        else
                        {
                            if (zoneObj[i].activeSelf) StartCoroutine(zoneFade(i, (int)zoneActiveInfo[i, 1], false));
                            zoneActiveInfo[i, 1] = 0;
                        }
                    }
                }

                if (nodesTimingCS.nodesTiming[currentPlayingTrack, nodeCntActive - 1, 2] > 10 && nodesTimingCS.nodesTiming[currentPlayingTrack, nodeCntActive - 1, 1] == beatCnt) StartCoroutine(MapAnimation((int)nodesTimingCS.nodesTiming[currentPlayingTrack, nodeCntActive - 1, 2]));

                afterBeatCntPlus = false;

                if (!musicPlayer.isPlaying) onMusicEnd();
            }

            float arc = beatSensor.eulerAngles.z;
            if (arc > 360 - beatSensorRotateArc * 0.5f || arc <= beatSensorRotateArc * 0.5f || (arc > 180 - beatSensorRotateArc * 0.5f && arc <= 180 + beatSensorRotateArc * 0.5f))
            {
                beatCnt += PlusBeatInOneUpdate;
                afterBeatCntPlus = true;

                if (makeMode == 0)
                {
                    for (int i = 1; i <= maxMapSize * maxMapSize; ++i) tileCS[i].changeTempTileColor(i);
                    for (int i = 1; i <= maxMapSize * maxMapSize; ++i) tileCS[i].changeTileColorFromTemp();

                    if (nodeActiveInfo[nodeCntActive, 0] == beatCnt) onNodeActiveTiming();

                    for (int i = 1; i <= maxNode; ++i)
                    {
                        if (nodeCopys[i].activeSelf) nodeCS[i].beatCntUpdated(beatCnt);
                    }
                }
                else
                {
                    if (makeMode == 1)
                    {
                        if (beatCnt % 1 == 0) playGameSound(gameSound[1]);
                    }
                    else if (makeMode == 2)
                    {
                        if (nodesTimingCS.nodesTiming[currentPlayingTrack, nodeCntActive, 1] == beatCnt)
                        {
                            playGameSound(gameSound[1]);
                            if (nodesTimingCS.nodesTiming[currentPlayingTrack, nodeCntActive, 2] != 0)
                            {
                                if (nodeCntActive >= nodesTimingCS.nodesTiming[currentPlayingTrack, 0, 0] || nodesTimingCS.nodesTiming[currentPlayingTrack, nodeCntActive + 1, 1] - beatCnt > PlusBeatInOneUpdate * maxMapSize + maxMapAnimationPlayBeat) StartCoroutine(MapAnimation((int)nodesTimingCS.nodesTiming[currentPlayingTrack, nodeCntActive, 2]));
                            }

                            ++nodeCntActive;
                        }
                    }
                }
            }

            int ClickTileZone = 0;
            if (collide[1, 1] != 0)
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    if (collide[1, 1] == 2)
                    {
                        StartCoroutine(tileCS[collide[1, 0]].glowImageSwitch(true));
                        gameUIManager.scoreBoardCS.onTapTiming(true);
                    }
                    else gameUIManager.scoreBoardCS.onPreTap();

                    tileCS[collide[1, 0]].changeTileColorAndInfo(collide[1, 2], false, -1, 0);
                }
                else if (collide[1, 1] == 2) gameUIManager.scoreBoardCS.onTapTiming(false);

                if (collide[1, 1] == 2 && tileCS[collide[1, 0]].tileNodeTargetTileNum[collide[1, 2], 1] != 0) ClickTileZone = 1;
            }
            if (collide[2, 1] != 0)
            {
                if (Input.GetKeyDown(KeyCode.S))
                {
                    if (collide[2, 1] == 2)
                    {
                        StartCoroutine(tileCS[collide[2, 0]].glowImageSwitch(true));
                        gameUIManager.scoreBoardCS.onTapTiming(true);
                    }
                    else gameUIManager.scoreBoardCS.onPreTap();

                    tileCS[collide[2, 0]].changeTileColorAndInfo(collide[2, 2], false, -1, 0);
                }
                else if (collide[2, 1] == 2) gameUIManager.scoreBoardCS.onTapTiming(false);

                if (collide[2, 1] == 2 && tileCS[collide[2, 0]].tileNodeTargetTileNum[collide[2, 2], 1] != 0) ClickTileZone = 2;
            }
            if (collide[3, 1] != 0)
            {
                if (Input.GetKeyDown(KeyCode.D))
                {
                    if (collide[3, 1] == 2)
                    {
                        StartCoroutine(tileCS[collide[3, 0]].glowImageSwitch(true));
                        gameUIManager.scoreBoardCS.onTapTiming(true);
                    }
                    else gameUIManager.scoreBoardCS.onPreTap();

                    tileCS[collide[3, 0]].changeTileColorAndInfo(collide[3, 2], false, -1, 0);
                }
                else if (collide[3, 1] == 2) gameUIManager.scoreBoardCS.onTapTiming(false);

                if (collide[3, 1] == 2 && tileCS[collide[3, 0]].tileNodeTargetTileNum[collide[3, 2], 1] != 0) ClickTileZone = 3;
            }

            if (ClickTileZone != 0)
            {
                tileCS[tileCS[collide[ClickTileZone, 0]].tileNodeTargetTileNum[collide[ClickTileZone, 2], 1]].changeTileColorAndInfo(0, false, maxColor + 1, -1, -1);
                tileCS[tileCS[collide[ClickTileZone, 0]].tileNodeTargetTileNum[collide[ClickTileZone, 2], 1]].updateTileColor();

                bool clickSuccess = false;
                if (Input.GetMouseButton(0))
                {
                    RaycastHit2D raycastHit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition) - Vector3.forward, Vector3.forward, 2);
                    if (raycastHit2D && int.Parse(raycastHit2D.collider.name) == tileCS[collide[ClickTileZone, 0]].tileNodeTargetTileNum[collide[ClickTileZone, 2], 1])
                    {
                        StartCoroutine(tileCS[tileCS[collide[ClickTileZone, 0]].tileNodeTargetTileNum[collide[ClickTileZone, 2], 1]].glowImageSwitch(true, true));
                        clickSuccess = true;
                    }
                }
                tileCS[collide[ClickTileZone, 0]].changeTileColorAndInfo(collide[ClickTileZone, 2], false, -1, -1, 0);

                gameUIManager.scoreBoardCS.onClickTiming(clickSuccess);
            }

            beatSensor.Rotate(0, 0, beatSensorRotateArc);
        }
    }

    private IEnumerator onEnabledFunc()
    {
        currentPlayingTrack = 1;
        for (int i = 1; i <= 3; ++i) zoneObj[i].SetActive(false);
        for (int i = 1; i <= 3; ++i) zoneActiveInfo[i, 1] = 0;
        mapGlowObj.SetActive(false);

        beatSensor.rotation = Quaternion.Euler(0, 0, 0);
        gameState = 0;
        beatCnt = -PlusBeatInOneUpdate * maxMapSize - PlusBeatInOneUpdate;

        nodeCycle = 0;
        nodeCntActive = 1;

        genClickNodeCnt = 0;

        itsTime = 0;

        yield return waitForStartFuzz;

        musicPlayer.clip = musicManager.music[currentPlayingTrack];
        musicPlayer.Play();
    }

    private void onFirstUpdate()
    {
        for (int i = 1; i <= nodesTimingCS.nodesTiming[currentPlayingTrack, 0, 0] + 1; ++i) nodeActiveInfo[i, 0] = PlusBeatInOneUpdate * 0.5f;

        for (int i = 1; i <= nodesTimingCS.nodesTiming[currentPlayingTrack, 0, 0]; ++i)
        {
            float timing = nodesTimingCS.nodesTiming[currentPlayingTrack, i, 1] + PlusBeatInOneUpdate - Random.Range((int)(maxMapSize * 0.5f) + 1, maxMapSize) * PlusBeatInOneUpdate;

            for (int k = 1; k <= nodesTimingCS.nodesTiming[currentPlayingTrack, 0, 0]; ++k)
            {
                int nodeEvent = (int)nodesTimingCS.nodesTiming[currentPlayingTrack, i, 2];
                if (nodeEvent >= 1 && nodeEvent <= 3) gameUIManager.scoreBoardCS.nodeCnt += nodeEvent + 1;
                else ++gameUIManager.scoreBoardCS.nodeCnt;

                if (nodeActiveInfo[k, 0] == PlusBeatInOneUpdate * 0.5f)
                {
                    nodeActiveInfo[k, 0] = timing;
                    nodeActiveInfo[k, 1] = nodesTimingCS.nodesTiming[currentPlayingTrack, i, 1];
                    break;
                }
                else if (nodeActiveInfo[k, 0] > timing)
                {
                    for (int j = i; j >= k + 1; --j)
                    {
                        nodeActiveInfo[j, 0] = nodeActiveInfo[j - 1, 0];
                        nodeActiveInfo[j, 1] = nodeActiveInfo[j - 1, 1];
                    }
                    nodeActiveInfo[k, 0] = timing;
                    nodeActiveInfo[k, 1] = nodesTimingCS.nodesTiming[currentPlayingTrack, i, 1];
                    break;
                }
            }
        }

        animationDelay[0] = new WaitForSeconds(0);
        animationDelay[1] = new WaitForSeconds(60 / musicManager.musicBPM[currentPlayingTrack] / 4 * 0.9f);
        animationDelay[2] = new WaitForSeconds(60 / musicManager.musicBPM[currentPlayingTrack] / 15 * 0.9f);
        animationDelay[3] = new WaitForSeconds(0.1f);
    }

    private void onMusicEnd()
    {
        gameUIManager.onMusicEnd();
    }

    private void onNodeActiveTiming()
    {
        useColorCnt = 0;
        for (int i = 1; i <= maxColor; ++i) palette[i] = 1;
        for (int i = 1; i <= maxMapSize * maxMapSize; ++i)
        {
            for (int k = 0; k < maxNodeDir; ++k)
            {
                if (tileCS[i].tileColor[k] != 0 && tileCS[i].tileColor[k] != maxColor + 1 && palette[tileCS[i].tileColor[k]] == 1)
                {
                    palette[tileCS[i].tileColor[k]] = 0;
                    ++useColorCnt;
                }
            }
        }

        for (int i = 1, k = 1; k <= maxColor - useColorCnt; ++i)
        {
            if (palette[i] == 1)
            {
                palette[k++] = i;
            }
        }
        int color = 0;

        do
        {
            color = palette[Random.Range(1, maxColor - useColorCnt + 1)];
            activateNode(nodeActiveInfo[nodeCntActive, 0], nodeActiveInfo[nodeCntActive++, 1], color, 1);
        }
        while (nodeActiveInfo[nodeCntActive, 0] == beatCnt);
    }
    public void activateNode(float activeTiming, float timing, int color, int generateMode, int targetTileNum = 0, int dir = 0)
    {
        if (nodeCycle == maxNode) nodeCycle = 1;
        else ++nodeCycle;

        NodeCS node = nodeCS[nodeCycle];
        if (!nodeCopys[nodeCycle].activeSelf)
        {
            nodeCopys[nodeCycle].SetActive(true);

            node.ActivatedTiming = activeTiming;
            node.timing = timing;
            node.color = color;
            node.generateMode = generateMode;
            node.targetTileNum = targetTileNum;
            node.dir = dir;
            node.clickTileNum = 0;

            if (generateMode == 1)
            {
                if (nodesTimingCS.nodesTiming[currentPlayingTrack, nodeCntActive - 1, 2] > 0 && nodesTimingCS.nodesTiming[currentPlayingTrack, nodeCntActive - 1, 2] <= 10)
                {
                    genClickNodeCnt = (int)nodesTimingCS.nodesTiming[currentPlayingTrack, nodeCntActive - 1, 2];
                    clickNodeGenPos[0] = genClickNodeCnt;
                }

                if (genClickNodeCnt > 0)
                {
                    if (gameUIManager.scoreBoardCS.clickAll)
                    {
                        if (genClickNodeCnt > 1)
                        {
                            int clickTileNum = clickNodeGenPos[clickNodeGenPos[0] - genClickNodeCnt + 1];
                            tileCS[clickTileNum].tileColor[0] = color;
                            node.clickTileNum = clickTileNum;
                            --genClickNodeCnt;
                        }
                        else
                        {
                            genClickNodeCnt = 0;
                            StartCoroutine(MapAnimation(1, clickNodeGenPos[clickNodeGenPos[0] - genClickNodeCnt]));
                        }
                    }
                    else genClickNodeCnt = 0;
                }

                node.generate();
            }
        }
        else activateNode(activeTiming, timing, color, generateMode, targetTileNum, dir);
        return;
    }
    public int zoneByTileNum(int tileNum)
    {
        int zoneNum = 0;
        if ((tileNum - 1) / maxMapSize >= maxMapSize * 0.5f)
        {
            if ((tileNum - 1) % maxMapSize + 1 <= maxMapSize * 0.5f) zoneNum = 1;
            else zoneNum = 0;
        }
        else if ((tileNum - 1) % maxMapSize + 1 <= maxMapSize * 0.5f) zoneNum = 2;
        else zoneNum = 3;

        return zoneNum;
    }
    private IEnumerator zoneFade(int zoneNum, int color, bool onOff)
    {
        if (onOff) zoneObj[zoneNum].SetActive(true);

        for (int i = 1; i <= 5; ++i)
        {
            if (onOff) zoneSprites[zoneNum].color = ColorByCode[color] + Color.black * (zoneAlpha * 0.2f * i - 1);
            else zoneSprites[zoneNum].color = ColorByCode[color] + Color.black * (zoneAlpha - zoneAlpha * 0.2f * i - 1);

            yield return zoneFadeTime;
        }

        if (!onOff) zoneObj[zoneNum].SetActive(false);
    }

    IEnumerator timer(WaitForSeconds sec)
    {
        itsTime = 1;
        yield return sec;
        itsTime = 2;
    }
    IEnumerator MapAnimation(int animNum, int originTileNum = 0)
    {
        if (animNum == 1)
        {
            for (int i = 1; i <= maxMapSize * maxMapSize; ++i)
            {
                if (zoneByTileNum(i) == 0)
                {
                    tileCS[i].changeTileColorAndInfo(0, false, 0);
                    tileCS[i].updateTileColor();
                }
            }

            zoneObj[0].SetActive(true);
            tileCS[originTileNum].changeTileColorAndInfo(0, false, maxColor + 1);
            tileCS[originTileNum].updateTileColor();

            yield return animationDelay[3];

            int a = (originTileNum - 1) % maxMapSize + 1;
            int b = (originTileNum - 1) / maxMapSize + 1;

            for (int n = 1; n <= (maxMapSize * 0.5f - 1) * 2; ++n)
            {
                for (int i = 1; i <= maxMapSize * maxMapSize; ++i)
                {
                    int x = (i - 1) % maxMapSize + 1;
                    int y = (i - 1) / maxMapSize + 1;

                    if (zoneByTileNum(i) == 0 && x >= a - n && x <= a + n && y >= b - n && y <= b + n)
                    {
                        if (y - b == x - a + n || y - b == x - a - n || y - b == -(x - a) + n || y - b == -(x - a) - n)
                        {
                            tileCS[i].changeTileColorAndInfo(0, false, maxColor + 1);
                            tileCS[i].updateTileColor();
                        }
                    }
                }

                yield return animationDelay[3];
            }

            for (int i = 1; i <= maxMapSize * maxMapSize; ++i) if (zoneByTileNum(i) == 0) tileCS[i].changeTileColorAndInfo(0, false, 0);
            zoneObj[0].SetActive(false);
        }
        else if (animNum == 11)
        {
            for (int i = 1; i <= 3; ++i) zoneObj[i].SetActive(false);

            mapGlowObj.SetActive(true);
            for (int i = 1; i <= maxMapSize * maxMapSize; ++i)
            {
                for (int k = 0; k < maxNodeDir; ++k) tileCS[i].changeTileColorAndInfo(k, false, 0, 0, 0);
                tileCS[i].glowObj.SetActive(false);
                tileCS[i].updateTileColor();
            }

            yield return animationDelay[0];

            for (int n = 3; n >= 0; --n)
            {
                for (int i = 1; i <= maxMapSize * maxMapSize; ++i)
                {
                    if ((i - 1) / maxMapSize + 1 == maxMapSize * 0.5f - n || (i - 1) / maxMapSize + 1 == maxMapSize * 0.5f + 1 + n)
                    {
                        if ((i - 1) % maxMapSize + 1 >= maxMapSize * 0.5f - n && (i - 1) % maxMapSize + 1 <= maxMapSize * 0.5f + n + 1) tileCS[i].changeTileColorAndInfo(0, false, maxColor + 1);
                    }
                    else if ((i - 1) % maxMapSize + 1 == maxMapSize * 0.5f - n || (i - 1) % maxMapSize + 1 == maxMapSize * 0.5f + n + 1) tileCS[i].changeTileColorAndInfo(0, false, maxColor + 1);

                    tileCS[i].updateTileColor();
                }

                yield return animationDelay[1];
            }

            for (int i = 1; i <= maxMapSize * maxMapSize; ++i) tileCS[i].changeTileColorAndInfo(0, false, 0);
            mapGlowObj.SetActive(false);
        }
        else if (animNum == 12 && makeMode == 0)
        {
            for (int i = 1; i <= 3; ++i) zoneObj[i].SetActive(false);

            mapGlowObj.SetActive(true);
            for (int i = 1; i <= maxMapSize * maxMapSize; ++i)
            {
                for (int k = 0; k < maxNodeDir; ++k) tileCS[i].changeTileColorAndInfo(k, false, 0, 0, 0);
                tileCS[i].glowObj.SetActive(false);
                tileCS[i].updateTileColor();
            }

            int pointTileNum = 0;
            for (int i = 1; i <= 3; ++i) if (collide[i, 0] != 0) pointTileNum = collide[i, 0];

            print(pointTileNum);
            print(tileCS[pointTileNum]);
            tileCS[pointTileNum].changeTileColorAndInfo(0, false, maxColor + 1);
            tileCS[pointTileNum].updateTileColor();
            yield return animationDelay[2];

            int a = (pointTileNum - 1) % maxMapSize + 1;
            int b = (pointTileNum - 1) / maxMapSize + 1;

            for (int n = 1; n <= (maxMapSize - 1) * 2; ++n)
            {
                for (int i = 1; i <= maxMapSize * maxMapSize; ++i)
                {
                    int x = (i - 1) % maxMapSize + 1;
                    int y = (i - 1) / maxMapSize + 1;

                    if (x >= a - n && x <= a + n && y >= b - n && y <= b + n)
                    {
                        if (y - b == x - a + n || y - b == x - a - n || y - b == -(x - a) + n || y - b == -(x - a) - n)
                        {
                            tileCS[i].changeTileColorAndInfo(0, false, maxColor + 1);
                            tileCS[i].updateTileColor();
                        }
                    }
                }

                yield return animationDelay[2];
            }

            for (int i = 1; i <= maxMapSize * maxMapSize; ++i) tileCS[i].changeTileColorAndInfo(0, false, 0);
            mapGlowObj.SetActive(false);
        }
    }
    void playGameSound(AudioClip clip)
    {
        for (int i = 1; i <= maxAudioPlayer; ++i)
        {
            AudioSource audio = gameSoundPlayers[i];
            if (!audio.isPlaying)
            {
                audio.clip = clip;
                audio.Play();
            }
            else continue;
            return;
        }

        print("오디오 동시재생 수 초과");
    }
}
