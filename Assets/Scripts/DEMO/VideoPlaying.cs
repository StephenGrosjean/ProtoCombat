using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class VideoPlaying : MonoBehaviour
{

    [SerializeField] private bool isInput;
    [SerializeField] private float timeBeforeVideo;
    private bool isPlaying, checkStarted;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(GameInput.GetInput(GameInput.InputType.ACTION_BACK) ||
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

            isInput = true;

        }

        if(!isPlaying && !checkStarted) {
            checkStarted = true;
            StartCoroutine("WaitToPlay");
        }
    }

    IEnumerator WaitToPlay() {
        yield return new WaitForSeconds(timeBeforeVideo);
        if (!isInput && !isPlaying) {
            SceneManager.LoadScene("VideoPlayer");
        }

    }
}
