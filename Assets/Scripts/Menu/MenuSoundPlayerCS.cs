using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSoundPlayerCS : MonoBehaviour
{
    public AudioClip[] button = new AudioClip[4];
    public AudioSource audio;
    
    public void playButton(int n)
    {
        audio.clip = button[n];
        audio.Play();
    }
}
