using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    [SerializeField] private GameObject confirmationUILeave, confirmationUIQuit;
    [SerializeField] private GameObject pauseMenu;

    private bool menuOpen;
    private bool canOpenMenu = true;

    public void Continue() {
        pauseMenu.SetActive(false);
    }

    public void QuitConfirm() {
        confirmationUIQuit.SetActive(true);
    }

    public void QuitCancel() {
        confirmationUIQuit.SetActive(false);
    }

    public void Quit() {
        Application.Quit();
    }

    public void LeaveConfirm() {
        confirmationUILeave.SetActive(true);
    }

    public void LeaveCancel() {
        confirmationUILeave.SetActive(false);
    }

    public void Leave() {
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.Disconnect();
        SceneManager.LoadScene(0);
    }

    private void Update() {
        if (Input.GetKey(KeyCode.Escape)  && canOpenMenu) {
            StartCoroutine("WaitTimeMenu");
            if (menuOpen) {
                if (SceneManager.GetActiveScene().name == "LocalArena") {
                    Time.timeScale = 1;
                }
                confirmationUILeave.SetActive(false);
                confirmationUIQuit.SetActive(false);
                pauseMenu.SetActive(false);
                menuOpen = false;
            }
            else {
                if (SceneManager.GetActiveScene().name == "LocalArena") {
                    Time.timeScale = 0;
                }
                pauseMenu.SetActive(true);
                menuOpen = true;
            }
        }
    }

    IEnumerator WaitTimeMenu() {
        canOpenMenu = false;
        yield return new WaitForSecondsRealtime(0.2f);
        canOpenMenu = true;
    }
}
