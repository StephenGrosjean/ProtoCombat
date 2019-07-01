using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject onlinePanel, localPanel, mainMenuPanel, optionsPanel;
    [SerializeField] private TextMeshProUGUI subTitle;

    [SerializeField] private string onlineText, localText, mainMenuText, optionsText;

    public void GotoOnline() {
        onlinePanel.SetActive(true);
        localPanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(false);

        subTitle.text = onlineText;
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = PlayerPrefs.GetString("Region");

        //        PhotonNetwork.ConnectToRegion(PlayerPrefs.GetString("Region"));
        PhotonNetwork.ConnectUsingSettings();

    }

    public void GotoLocal() {
        onlinePanel.SetActive(false);
        localPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(false);

        subTitle.text = localText;
    }

    public void GotoMainMenu() {
        onlinePanel.SetActive(false);
        localPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
        optionsPanel.SetActive(false);

        subTitle.text = mainMenuText;

        if (PhotonNetwork.IsConnectedAndReady) {
            PhotonNetwork.Disconnect();
        }
    }

    public void GotoOptions() {
        onlinePanel.SetActive(false);
        localPanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);

        subTitle.text = optionsText;

    }
}
