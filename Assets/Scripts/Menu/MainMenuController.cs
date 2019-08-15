using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject onlinePanel, localPanel, mainMenuPanel, optionsPanel;
    [SerializeField] private TextMeshProUGUI subTitle;
    [SerializeField] private GameObject statusText;

    [SerializeField] private string onlineText, localText, mainMenuText, optionsText;

    [SerializeField] private Transform parentMainMenu, parentOnlineMenu, parentLocalMenu, parentOptionMenu;

    [SerializeField] private MenuController menuController;

    private PANEL currentPanel;

    enum PANEL {
        MAIN_MENU,
        OPTION,
        LOCAL,
        ONLINE
    }

    private void Start() {
        currentPanel = PANEL.MAIN_MENU;
        menuController.SetupMenuBtns(parentMainMenu);

        if (PhotonNetwork.IsConnected)
            PhotonNetwork.Disconnect();

    }
    private void Update() {
        if(currentPanel == PANEL.ONLINE)
            statusText.SetActive(true);
        else
            statusText.SetActive(false);
    }

    public void GotoOnline() {
        currentPanel = PANEL.ONLINE;
        menuController.SetupMenuBtns(parentOnlineMenu);

        onlinePanel.SetActive(true);    
        localPanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(false);

        subTitle.text = onlineText;
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = PlayerPrefs.GetString("Region");

        //PhotonNetwork.ConnectToRegion(PlayerPrefs.GetString("Region"));
        PhotonNetwork.ConnectUsingSettings();

    }

    public void GotoLocal() {
        currentPanel = PANEL.LOCAL;
        menuController.SetupMenuBtns(parentLocalMenu);

        onlinePanel.SetActive(false);
        localPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(false);

        subTitle.text = localText;


        SceneManager.LoadScene("LocalArena");
    }

    public void GotoMainMenu() {
        currentPanel = PANEL.MAIN_MENU;
        menuController.SetupMenuBtns(parentMainMenu);

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
        currentPanel = PANEL.OPTION;
        menuController.SetupMenuBtns(parentOptionMenu);

        onlinePanel.SetActive(false);
        localPanel.SetActive(false);
        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);

        subTitle.text = optionsText;

    }

    public void Quit() {
        Application.Quit();
    }
}
