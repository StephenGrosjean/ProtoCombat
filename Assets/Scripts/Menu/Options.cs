using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class Options : MonoBehaviour
{
    [Header("Graphics Settings")]
    [SerializeField] private TMP_Dropdown qualityDropdown;
    [SerializeField] private int quality;

    [Header("Volume Settings")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider vfxVolumeSlider;
    [SerializeField] private TextMeshProUGUI musicVolumeText;
    [SerializeField] private TextMeshProUGUI vfxVolumeText;
    [SerializeField] private float musicVolume = 50;
    [SerializeField] private float vfxVolume = 50;

    [Header("Network Settings")]
    [SerializeField] private TMP_Dropdown regionDropdown;
    [SerializeField] private string region = "eu";
    //Regions in order: eu, us, usw, cae, jp, in, ru, rue, cn, kr

    void Start()
    {
        LoadSettings();
    }


    public void SetQuality() {
        QualitySettings.SetQualityLevel(qualityDropdown.value);
        quality = qualityDropdown.value;
        SaveSettings();
    }

    public void Music_SetVolume() {
        musicVolume = musicVolumeSlider.value;
        musicVolumeText.text = musicVolume.ToString();
        SaveSettings();
    }

    public void VFX_SetVolume() {
        vfxVolume = vfxVolumeSlider.value;
        vfxVolumeText.text = vfxVolume.ToString();
        SaveSettings();
    }

    public void Network_SetRegion() {
        switch(regionDropdown.value){
            case 0:
                region = "eu";
                break;
            case 1:
                region = "us";
                break;
            case 2:
                region = "usw";
                break;
            case 3:
                region = "cae";
                break;
            case 4:
                region = "jp";
                break;
            case 5:
                region = "in";
                break;
            case 6:
                region = "ru";
                break;
            case 7:
                region = "rue";
                break;
            case 8:
                region = "cn";
                break;
            case 9:
                region = "kr";
                break;
        }
        SaveSettings();
    }

    void SaveSettings() {
        PlayerPrefs.SetInt("Quality", quality);
        PlayerPrefs.SetFloat("VFX_Volume", vfxVolume);
        PlayerPrefs.SetFloat("Music_Volume", musicVolume);
        PlayerPrefs.SetString("Region", region);
        PlayerPrefs.Save();
    }

    void LoadSettings() {
        if (PlayerPrefs.HasKey("Quality")) {
            quality = PlayerPrefs.GetInt("Quality");
        }
        else {
            quality = 3;
        }

        if (PlayerPrefs.HasKey("VFX_Volume")) {
            vfxVolume = PlayerPrefs.GetFloat("VFX_Volume");
        }
        else {
            vfxVolume = 50;
        }

        if (PlayerPrefs.HasKey("Music_Volume")) {
            musicVolume = PlayerPrefs.GetFloat("Music_Volume");
        }
        else {
            musicVolume = 50;
        }

        if (PlayerPrefs.HasKey("Region")) {
            region = PlayerPrefs.GetString("Region");
        }
        else {
            region = "eu";
        }

        //Show Settings
        qualityDropdown.value = quality;
        musicVolumeSlider.value = musicVolume;
        vfxVolumeSlider.value = vfxVolume;
        regionDropdown.value = FindRegionIDByName();

        SaveSettings();
    }

    int FindRegionIDByName() {
        int regionToReturn = 0;
        switch (region) {
            case "eu":
                regionToReturn = 0;
                break;
            case "us":
                regionToReturn = 1;
                break;
            case "usw":
                regionToReturn = 2;
                break;
            case "cae":
                regionToReturn = 3;
                break;
            case "jp":
                regionToReturn = 4;
                break;
            case "in":
                regionToReturn = 5;
                break;
            case "ru":
                regionToReturn = 6;
                break;
            case "rue":
                regionToReturn = 7;
                break;
            case "cn":
                regionToReturn = 8;
                break;
            case "kr":
                regionToReturn = 9;
                break;
        }
        return regionToReturn;
    }
}
