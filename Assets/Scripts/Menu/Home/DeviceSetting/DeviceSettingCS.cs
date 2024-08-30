using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceSettingCS : MonoBehaviour
{
    public GameObject settingDialog;
    private bool settingDialogOpen;

    public GameObject[] moduleLists;
    [HideInInspector] public static int[] moduleSelected = {0, 1, 1, 1 };

    [HideInInspector] public static DeviceModuleCS[,] deviceModuleCS = new DeviceModuleCS[4, 10];

    private void OnEnable()
    {
        settingDialog.SetActive(false);
        settingDialogOpen = false;

        for (int i = 1; i <= 3; ++i) for (int j = 1; deviceModuleCS[i, j] != null; ++j) deviceModuleCS[i, j].onHomeEnabled();
    }

    public void onSettingDialogButton(bool openClose)
    {
        if (openClose) for (int i = 1; i <= 3; ++i) moduleLists[i].SetActive(false);
        if (openClose != settingDialogOpen)
        {
            settingDialog.SetActive(openClose);
            settingDialogOpen = openClose;
        }
    }

    public void onModuleListOpenUp(int module)
    {
        moduleLists[module].SetActive(!moduleLists[module].activeSelf);
    }

    public static void onModuleSelect(int module, int n)
    {
        moduleSelected[module] = n;
        for (int i = 1; deviceModuleCS[module, i] != null; ++i) deviceModuleCS[module, i].selectedDeActivate();
    }
}
