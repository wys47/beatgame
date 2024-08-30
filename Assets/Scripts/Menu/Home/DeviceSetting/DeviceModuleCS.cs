using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeviceModuleCS : MonoBehaviour
{
    public GameObject download;
    public GameObject selected;

    [HideInInspector] public bool purchased = false;

    public int module;
    public int n;

    private void Awake()
    {
        DeviceSettingCS.deviceModuleCS[module, n] = this;
    }

    public void onHomeEnabled()
    {
        if (DeviceSettingCS.moduleSelected[module] == n)
        {
            purchased = true;
            selected.SetActive(true);
        }
        else selected.SetActive(false);
        download.SetActive(!purchased);
    }

    public void onDownloadButtonUp()
    {
        purchased = true;
        download.SetActive(!purchased);
    }

    public void onSelectClickUp()
    {
        if (purchased)
        {
            DeviceSettingCS.onModuleSelect(module, n);
            selected.SetActive(true);
        }
    }
    public void selectedDeActivate()
    {
        selected.SetActive(false);
    }
}
