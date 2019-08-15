using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    [SerializeField] private GameObject confirmationUILeave, confirmationUIQuit;
    [SerializeField] private GameObject pauseMenu;

    [SerializeField] private Transform parentConfirmationQuitMenu, parentConfirmationLeaveMenu, parentPauseMenu;
    [SerializeField] private MenuController menuController;

    private bool menuOpen;
    private bool canOpenMenu = true;

    public void Continue() {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void QuitConfirm() {
        menuController.SetupMenuBtns(parentConfirmationQuitMenu);
        confirmationUIQuit.SetActive(true);
    }

    public void QuitCancel() {
        menuController.SetupMenuBtns(parentPauseMenu);
        confirmationUIQuit.SetActive(false);
    }

    public void Quit() {
        Application.Quit();
    }

    public void LeaveConfirm() {
        menuController.SetupMenuBtns(parentConfirmationLeaveMenu);
        confirmationUILeave.SetActive(true);
    }

    public void LeaveCancel() {
        menuController.SetupMenuBtns(parentPauseMenu);
        confirmationUILeave.SetActive(false);
    }

    public void Leave() {
        if (PhotonNetwork.IsConnected) {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
        }
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void Rematch() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void Update() {
        if (GameInput.GetInputDown(GameInput.InputType.PAUSE)  && canOpenMenu) {
            StartCoroutine("WaitTimeMenu");
            if (menuOpen) {
                if (SceneManager.GetActiveScene().name == "LocalArena") {
                    Time.timeScale = 1;
                }
                confirmationUILeave.SetActive(false);
                confirmationUIQuit.SetActive(false);
                pauseMenu.SetActive(false);

                menuController.SetupMenuBtns(null);
                menuOpen = false;
                menuController.inMenu = false;
            }
            else {
                if (SceneManager.GetActiveScene().name == "LocalArena") {
                    Time.timeScale = 0;
                }
                pauseMenu.SetActive(true);

                menuController.SetupMenuBtns(parentPauseMenu);
                menuOpen = true;
                menuController.inMenu = true;
            }
        }
    }

    IEnumerator WaitTimeMenu() {
        canOpenMenu = false;
        yield return new WaitForSecondsRealtime(0.2f);
        canOpenMenu = true;
    }
}
