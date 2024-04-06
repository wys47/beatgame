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
    private SpriteRenderer[] zoneSprites = new SpriteRenderer[6];
    public struct zoneActiveInfoDef
    {
        public int colorCode;
        public float timing;
    } 
    [HideInInspector] public zoneActiveInfoDef[] zoneActiveInfo = new zoneActiveInfoDef[6];
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

    private struct nodeActiveInfoDef
    {
        public float activeTiming;
        public float timing;
    }
    private nodeActiveInfoDef[] nodeActiveInfo = new nodeActiveInfoDef[maxTiming];
    private int nodeCntActive;
    private int nodeCycle;

    private int[] palette = new int[maxColor + 1];
    private int useColorCnt;

    private struct collodeDef
    {
        public int tileNum;
        public int primeNodeRank;
        public int dir;
    }
    private collodeDef[] collide = new collodeDef[4];

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
        for (int i = 0; i <= 5; ++i)
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

                if (collide[0].tileNum != 0)
                {
                    gameUIManager.scoreBoardCS.onClickTiming(false);
                    collide[0].tileNum = 0;
                }

                for (int i = 1; i <= 3; ++i)
                {
                    collide[i].tileNum = 0;

                    if (collide[i].primeNodeRank == 2) gameUIManager.scoreBoardCS.onTapTiming(false);
                    collide[i].primeNodeRank = 0;

                    collide[i].dir = 0;
                }

                for (int i = 1; i <= maxMapSize * maxMapSize; ++i)
                {
                    for (int k = 1; k < maxNodeDir; ++k)
                    {
                        if (tileCS[i].tileColor[0] == tileCS[i].tileColor[k]) tileCS[i].changeTileColorAndInfo(0, false, 0, 0);

                        int targetTileNum = tileCS[i].tileNodeTargetTileNum[k];
                        if (targetTileNum == 0) continue;
                        int zone = zoneByTileNum(targetTileNum);

                        if (zone != 0)
                        {
                            if (k == 1)
                            {
                                int diff = targetTileNum - i;

                                if (diff == maxMapSize)
                                {
                                    if (collide[zone].primeNodeRank == 0)
                                    {
                                        collide[zone].tileNum = i;
                                        collide[zone].primeNodeRank = 1;
                                        collide[zone].dir = k;
                                    }
                                }
                                else if (diff == 0)
                                {
                                    collide[zone].tileNum = i;
                                    collide[zone].primeNodeRank = 2;
                                    collide[zone].dir = k;
                                }
                            }
                            else if (k == 2)
                            {
                                int diff = i - targetTileNum;

                                if (diff == maxMapSize)
                                {
                                    if (collide[zone].primeNodeRank == 0)
                                    {
                                        collide[zone].tileNum = i;
                                        collide[zone].primeNodeRank = 1;
                                        collide[zone].dir = k;
                                    }
                                }
                                else if (diff == 0)
                                {
                                    collide[zone].tileNum = i;
                                    collide[zone].primeNodeRank = 2;
                                    collide[zone].dir = k;
                                }
                            }
                            else if (k == 3)
                            {
                                int diff = i - targetTileNum;

                                if (diff == 1)
                                {
                                    if (collide[zone].primeNodeRank == 0)
                                    {
                                        collide[zone].tileNum = i;
                                        collide[zone].primeNodeRank = 1;
                                        collide[zone].dir = k;
                                    }
                                }
                                else if (diff == 0)
                                {
                                    collide[zone].tileNum = i;
                                    collide[zone].primeNodeRank = 2;
                                    collide[zone].dir = k;
                                }
                            }
                            else if (k == 4)
                            {
                                int diff = targetTileNum - i;

                                if (diff == 1)
                                {
                                    if (collide[zone].primeNodeRank == 0)
                                    {
                                        collide[zone].tileNum = i;
                                        collide[zone].primeNodeRank = 1;
                                        collide[zone].dir = k;
                                    }
                                }
                                else if (diff == 0)
                                {
                                    collide[zone].tileNum = i;
                                    collide[zone].primeNodeRank = 2;
                                    collide[zone].dir = k;
                                }
                            }
                        }
                        else if (targetTileNum == i) collide[zone].tileNum = i;
                    }
                }

                for (int i = 1; i <= 5; ++i)
                {
                    if (zoneActiveInfo[i].colorCode != 0)
                    {
                        if (zoneActiveInfo[i].timing != beatCnt)
                        {
                            if (!zoneObj[i].activeSelf) StartCoroutine(zoneFade(i, zoneActiveInfo[i].colorCode, true));
                        }
                        else
                        {
                            if (zoneObj[i].activeSelf) StartCoroutine(zoneFade(i, zoneActiveInfo[i].colorCode, false));
                            zoneActiveInfo[i].colorCode = 0;
                        }
                    }
                }

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

                    if (nodeActiveInfo[nodeCntActive].activeTiming == beatCnt) onNodeActiveTiming();

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
                        if (nodesTimingCS.nodesTiming[currentPlayingTrack, nodeCntActive].timing == beatCnt)
                        {
                            playGameSound(gameSound[1]);
                            if (nodesTimingCS.nodesTiming[currentPlayingTrack, nodeCntActive].eventNum != 0)
                            {
                                if (nodeCntActive >= nodesTimingCS.nodesTiming[currentPlayingTrack, 0].maxTiming || nodesTimingCS.nodesTiming[currentPlayingTrack, nodeCntActive + 1].timing - beatCnt > PlusBeatInOneUpdate * maxMapSize + maxMapAnimationPlayBeat) StartCoroutine(MapAnimation(nodesTimingCS.nodesTiming[currentPlayingTrack, nodeCntActive].eventNum));
                            }

                            ++nodeCntActive;
                        }
                    }
                }
            }

            int clickTileNum = collide[0].tileNum;
            if (clickTileNum != 0)
            {
                if (Input.GetMouseButton(0))
                {
                    RaycastHit2D raycastHit2D = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition) - Vector3.forward, Vector3.forward, 2);
                    if (raycastHit2D && int.Parse(raycastHit2D.collider.name) == clickTileNum)
                    {
                        StartCoroutine(tileCS[clickTileNum].glowImageSwitch(true, true));
                        gameUIManager.scoreBoardCS.onClickTiming(true);

                        int eventNum = nodesTimingCS.nodesTiming[currentPlayingTrack, nodeCntActive - 1].eventNum;
                        if (nodesTimingCS.nodesTiming[currentPlayingTrack, nodeCntActive - 1].timing == beatCnt && eventNum == 2)
                        {
                            if (gameUIManager.scoreBoardCS.clickAll) StartCoroutine(MapAnimation(eventNum, clickTileNum));
                            gameUIManager.scoreBoardCS.clickAll = true;
                        }

                        collide[0].tileNum = 0;
                    }
                }
            }

            bool clickSuccess = false;
            if (collide[1].primeNodeRank != 0)
            {
                if (Input.GetKeyDown(KeyCode.A))
                {
                    if (collide[1].primeNodeRank == 2)
                    {
                        StartCoroutine(tileCS[collide[1].tileNum].glowImageSwitch(true));
                        gameUIManager.scoreBoardCS.onTapTiming(true);
                        clickSuccess = true;
                    }
                    else gameUIManager.scoreBoardCS.onPreTap();

                    tileCS[collide[1].tileNum].changeTileColorAndInfo(collide[1].dir, false, -1, 0);
                    collide[1].primeNodeRank = 0;
                }
            }
            if (collide[2].primeNodeRank != 0)
            {
                if (Input.GetKeyDown(KeyCode.S))
                {
                    if (collide[2].primeNodeRank == 2)
                    {
                        StartCoroutine(tileCS[collide[2].tileNum].glowImageSwitch(true));
                        gameUIManager.scoreBoardCS.onTapTiming(true);
                        clickSuccess = true;
                    }
                    else gameUIManager.scoreBoardCS.onPreTap();

                    tileCS[collide[2].tileNum].changeTileColorAndInfo(collide[2].dir, false, -1, 0);
                    collide[2].primeNodeRank = 0;
                }
            }
            if (collide[3].primeNodeRank != 0)
            {
                if (Input.GetKeyDown(KeyCode.D))
                {
                    if (collide[3].primeNodeRank == 2)
                    {
                        StartCoroutine(tileCS[collide[3].tileNum].glowImageSwitch(true));
                        gameUIManager.scoreBoardCS.onTapTiming(true);
                        clickSuccess = true;
                    }
                    else gameUIManager.scoreBoardCS.onPreTap();

                    tileCS[collide[3].tileNum].changeTileColorAndInfo(collide[3].dir, false, -1, 0);
                    collide[3].primeNodeRank = 0;
                }
            }
            if (clickSuccess)
            {
                int eventNum = nodesTimingCS.nodesTiming[currentPlayingTrack, nodeCntActive - 1].eventNum;
                if (nodesTimingCS.nodesTiming[currentPlayingTrack, nodeCntActive - 1].timing == beatCnt && eventNum > 10)
                {
                    if (gameUIManager.scoreBoardCS.tapAll) StartCoroutine(MapAnimation(eventNum));
                    gameUIManager.scoreBoardCS.tapAll = true;
                }
            }

            beatSensor.Rotate(0, 0, beatSensorRotateArc);
        }
    }

    private IEnumerator onEnabledFunc()
    {
        currentPlayingTrack = 1;
        for (int i = 1; i <= 3; ++i) zoneObj[i].SetActive(false);
        for (int i = 1; i <= 3; ++i) zoneActiveInfo[i].colorCode = 0;
        mapGlowObj.SetActive(false);

        beatSensor.rotation = Quaternion.Euler(0, 0, 0);
        gameState = 0;
        beatCnt = -PlusBeatInOneUpdate * maxMapSize - PlusBeatInOneUpdate;

        collide[0].tileNum = 0;

        nodeCycle = 0;
        nodeCntActive = 1;

        itsTime = 0;

        yield return waitForStartFuzz;

        musicPlayer.clip = musicManager.music[currentPlayingTrack];
        musicPlayer.Play();
    }

    private void onFirstUpdate()
    {
        for (int i = 1; i <= nodesTimingCS.nodesTiming[currentPlayingTrack, 0].maxTiming + 1; ++i) nodeActiveInfo[i].activeTiming = PlusBeatInOneUpdate * 0.5f;

        for (int i = 1; i <= nodesTimingCS.nodesTiming[currentPlayingTrack, 0].maxTiming; ++i)
        {
            float timing = nodesTimingCS.nodesTiming[currentPlayingTrack, i].timing + PlusBeatInOneUpdate - (nodesTimingCS.nodesTiming[currentPlayingTrack, i].eventNum != 1 && nodesTimingCS.nodesTiming[currentPlayingTrack, i].eventNum != 2 ? Random.Range((int)(maxMapSize * 0.5f) + 1, maxMapSize) : 8) * PlusBeatInOneUpdate;

            ++gameUIManager.scoreBoardCS.nodeCnt;

            for (int k = 1; k <= nodesTimingCS.nodesTiming[currentPlayingTrack, 0].maxTiming; ++k)
            {
                if (nodeActiveInfo[k].activeTiming == PlusBeatInOneUpdate * 0.5f)
                {
                    nodeActiveInfo[k].activeTiming = timing;
                    nodeActiveInfo[k].timing = nodesTimingCS.nodesTiming[currentPlayingTrack, i].timing;
                    break;
                }
                else if (nodeActiveInfo[k].activeTiming > timing)
                {
                    for (int j = i; j >= k + 1; --j)
                    {
                        nodeActiveInfo[j].activeTiming = nodeActiveInfo[j - 1].activeTiming;
                        nodeActiveInfo[j].timing = nodeActiveInfo[j - 1].timing;
                    }
                    nodeActiveInfo[k].activeTiming = timing;
                    nodeActiveInfo[k].timing = nodesTimingCS.nodesTiming[currentPlayingTrack, i].timing;
                    break;
                }
            }
        }

        animationDelay[0] = new WaitForSeconds(0);
        animationDelay[1] = new WaitForSeconds(60 / musicManager.musicBPM[currentPlayingTrack] / 4 * 0.9f);
        animationDelay[2] = new WaitForSeconds(60 / musicManager.musicBPM[currentPlayingTrack] / 15 * 0.9f);
        animationDelay[3] = new WaitForSeconds(0.05f);
    }

    private void onMusicEnd()
    {
        gameUIManager.onMusicEnd();
        gameObject.SetActive(false);
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
            activateNode(nodeActiveInfo[nodeCntActive].activeTiming, nodeActiveInfo[nodeCntActive++].timing, color, 1);
        }
        while (nodeActiveInfo[nodeCntActive].activeTiming == beatCnt);
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

            if (generateMode == 1)
            {
                int eventNum = nodesTimingCS.nodesTiming[currentPlayingTrack, nodeCntActive - 1].eventNum;

                if (eventNum == 1 || eventNum == 2)
                {
                    int dir_ = 1 + 3 * Random.Range(0, 2);
                    int genTileNum = 0;
                    int targetTileNum_ = 0;
                    if (dir_ == 1)
                    {
                        do genTileNum = Random.Range(5, 9);
                        while (tileCS[genTileNum].tileColor[1] != 0);

                        targetTileNum_ = genTileNum + maxMapSize * (maxMapSize - 1);

                        zoneActiveInfo[4].colorCode = maxColor + 1;
                        zoneActiveInfo[4].timing = beatCnt + 8 * PlusBeatInOneUpdate;
                    }
                    else
                    {
                        do genTileNum = maxMapSize * Random.Range(4, 8) + 1;
                        while (tileCS[genTileNum].tileColor[4] != 0);

                        targetTileNum_ = genTileNum + maxMapSize - 1;

                        zoneActiveInfo[5].colorCode = maxColor + 1;
                        zoneActiveInfo[5].timing = beatCnt + 8 * PlusBeatInOneUpdate;
                    }

                    tileCS[genTileNum].changeTileColorAndInfo(dir_, false, maxColor + 1, targetTileNum_);
                }
                else node.generate();
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
        if (animNum == 2)
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
                for (int k = 0; k < maxNodeDir; ++k) tileCS[i].changeTileColorAndInfo(k, false, 0, 0);
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
                for (int k = 0; k < maxNodeDir; ++k) tileCS[i].changeTileColorAndInfo(k, false, 0, 0);
                tileCS[i].glowObj.SetActive(false);
                tileCS[i].updateTileColor();
            }

            int pointTileNum = 0;
            for (int i = 1; i <= 3; ++i) if (collide[i].tileNum != 0) pointTileNum = collide[i].tileNum;

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
