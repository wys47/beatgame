using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodesTimingCS : Variables
{
    public SettingCS settingCS;

    private static int maxSong = 1;

    private string[] nodeInString = new string[2]
    {
        "",

        "4x,6x,8x,10c,12c,14c,15c,19z,20z,22z,24s,25f,26.5f,29i,30q,31q,32w,34f,36f,38f,39f,40f,42f,44f,45f,47f,49f,51f,52f,53f,55f,57f,59f,60f,62f,"
    };
    public struct nodesTimingDef
    {
        public int maxTiming;
        public float timing;
        public int eventNum;
    }
    [HideInInspector] public nodesTimingDef[,] nodesTiming = new nodesTimingDef[maxSong + 1, maxTiming];//0.�ִ�Ÿ�̹�,1.Ÿ�̹�,2.�̺�Ʈ

    private void Start()
    {
        int readMode = 0;
        string num = "";
        int nodeCnt = 0;
        for (int i = 1; i < nodeInString.Length; ++i)
        {
            nodeCnt = 1;
            for (int k = 0; k < nodeInString[i].Length; ++k)
            {
                char ch = nodeInString[i][k];
                if (readMode == 0)
                {
                    if (char.IsDigit(ch)) readMode = 1;
                }
                if (readMode == 1)
                {
                    if (!char.IsLetter(ch))
                    {
                        num += ch;
                    }
                    else
                    {
                        if (ch == 'f') nodesTiming[i, nodeCnt].eventNum = 0;
                        else
                        {
                            switch (ch)
                            {
                                case 'q': nodesTiming[i, nodeCnt].eventNum = eventNumClickNodeGenerate; break;//Ŭ�� ��� ����
                                case 'w': nodesTiming[i, nodeCnt].eventNum = eventNumClickNodeAnim; break;//Ŭ�� ��� �ִϸ��̼�

                                case 'z': nodesTiming[i, nodeCnt].eventNum = eventNumPositionedNodeRange[0]; break;//��ġ ���� ��� ����
                                case 'x': nodesTiming[i, nodeCnt].eventNum = eventNumPositionedNodeRange[0] + 1; break;//��ġ ���� ��� ����
                                case 'c': nodesTiming[i, nodeCnt].eventNum = eventNumPositionedNodeRange[0] + 2; break;//��ġ ���� ��� ����

                                case 'p': nodesTiming[i, nodeCnt].eventNum = eventNumChainNodeRange[0]; break;//���� ��� ����
                                case 'o': nodesTiming[i, nodeCnt].eventNum = eventNumChainNodeRange[0] + 1; break;//���� ��� ����
                                case 'i': nodesTiming[i, nodeCnt].eventNum = eventNumChainNodeRange[0] + 2; break;//���� ��� ����

                                case 'a': nodesTiming[i, nodeCnt].eventNum = eventNumGlobalNodeRange[0]; break;//�� ��ü �ִϸ��̼�
                                case 's': nodesTiming[i, nodeCnt].eventNum = eventNumGlobalNodeRange[0] + 1; break;//�� ��ü �ִϸ��̼�
                            }
                        }

                        nodesTiming[i, nodeCnt++].timing = float.Parse(num) + settingCS.sinc;
                        num = "";
                        readMode = 0;
                    }
                }
            }

            nodesTiming[i, 0].maxTiming = nodeCnt - 1;
        }
    }
}
