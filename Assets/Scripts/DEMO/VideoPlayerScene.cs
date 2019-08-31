using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoPlayerScene : MonoBehaviour {

    private void Start() {
        GetComponent<VideoPlayer>().url = Application.dataPath + "/Video/Gameplay.mp4";
        SoundManager._instance.StopSound(SoundManager._instance.gameObject.GetComponent<AudioSource>());
    }

    void Update() {
        if (GameInput.GetInput(GameInput.InputType.ACTION_BACK) ||
            GameInput.GetInput(GameInput.InputType.ACTION_CONFIRM) ||
            GameInput.GetInput(GameInput.InputType.BIG_SHOOT) ||
            GameInput.GetInput(GameInput.InputType.DASH) ||
            GameInput.GetInput(GameInput.InputType.DEFENSE) ||
            GameInput.GetInput(GameInput.InputType.DOWN) ||
            GameInput.GetInput(GameInput.InputType.LEFT) ||
            GameInput.GetInput(GameInput.InputType.PAUSE) ||
            GameInput.GetInput(GameInput.InputType.RIGHT) ||
            GameInput.GetInput(GameInput.InputType.SHOOT) ||
            GameInput.GetInput(GameInput.InputType.UP)) {

            SceneManager.LoadScene("Lobby");
            SoundManager._instance.PlayMusic(SoundManager.MusicList.MENU);

        }

    }

}
