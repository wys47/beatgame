using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMaker : MonoBehaviour
{
    public MapCtrl mapCtrl;

    public float timingAdjustments;

    private float[] nodes = new float[1000];
    private int nodeCnt;

    void OnEnable()
    {
        nodeCnt = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            float arc = mapCtrl.beatSensor.eulerAngles.z;
            float plus = 0;
            switch (mapCtrl.beatCnt % 1)
            {
                case 0:
                    if (arc > 90 && arc < 180) plus = 0.5f;
                    break;
                case 0.5f:
                    if (arc > 270) plus = 0.5f;
                    break;
            }

            nodes[nodeCnt++] = mapCtrl.beatCnt + plus + timingAdjustments;
        }

        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            string str = "";
            for (int i = 1; i <= nodeCnt - 1; ++i)
            {
                str += nodes[i] + "f,";
            }
        }
    }
}
