using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using XInputDotNetPure;
using TMPro;


public class MultiControllerManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private List<Transform> spawnPositions;
    [Header("UI")]
    [SerializeField] private List<GameObject> playersReadyUI = new List<GameObject>();
    [SerializeField] private List<GameObject> playersNotReadyUI = new List<GameObject>();
    [SerializeField] private List<GameObject> readyUI = new List<GameObject>();
    [SerializeField] private TextMeshProUGUI countDown;
    [SerializeField] private int maxCountDown;
    [SerializeField] private GameObject countDownUI;
    private int currentCountDown;

    public struct ControllerToPlayer
    {
        public InputDevice device;
        public int playerId;
        public ControllerState controllerState;
        public GameObject player;
        public GameState gameState;
        public bool isReady;
    }

    private List<ControllerToPlayer> listOfControllers = new List<ControllerToPlayer>();

    public enum ControllerState
    {
        ATTACHED,
        DETACHED
    }

    public enum GameState
    {
        NOT_PLAYING,
        PLAYING
    }

    public int NumController = 0;

    private bool isCountdown;

    // Start is called before the first frame update
    void Awake()
    {
        InputManager.OnDeviceAttached += AttachDevice;
        InputManager.OnDeviceDetached += DetachDevice;
    }

    private void Update() {
        if (listOfControllers.Count > 0) {

            for (int i = 0; i < listOfControllers.Count; i++) {
                if (GameInput.GetInputDown(GameInput.InputType.SHOOT, listOfControllers[i].device)) {
                    ControllerToPlayer ctPlayer = listOfControllers[i];
                    ctPlayer.isReady = true;
                    listOfControllers[i] = ctPlayer;
                    
                }

                playersReadyUI[i].SetActive(listOfControllers[i].isReady);
                playersNotReadyUI[i].SetActive(!listOfControllers[i].isReady);
            }

            bool playersReady = true;
            foreach (ControllerToPlayer ct in listOfControllers) {
                if (!ct.isReady) {
                    playersReady = false;
                }
            }
            if (playersReady) {
               
                if (!isCountdown) {
                    isCountdown = true;
                     StartCoroutine("CountDown");
                }
            }
        }
    }

    void AttachDevice(InputDevice device)
    {
        ControllerToPlayer newPlayer;

        //Check if we've found again the controller
        bool reFoundController = false;
        for (int i = 0; i < listOfControllers.Count; i++)
        {
            if (listOfControllers[i].device.Equals(device))
            {
                Debug.Log("Reconnected controller : " + listOfControllers[i].playerId);
                reFoundController = true;

                ControllerToPlayer ctPlayer = listOfControllers[i];
                ctPlayer.controllerState = ControllerState.ATTACHED;
                ctPlayer.gameState = GameState.PLAYING;

                listOfControllers[i] = ctPlayer;
                break;
            }
        }

        //Else, register the new controller
        if (!reFoundController)
        {
            Debug.Log("New Connected controller : " + listOfControllers.Count);
            ControllerToPlayer ctPlayer = new ControllerToPlayer();
            ctPlayer.playerId = listOfControllers.Count;
            ctPlayer.device = device;
            ctPlayer.controllerState = ControllerState.ATTACHED;
            ctPlayer.gameState = GameState.PLAYING;

            if (playerPrefab != null)
            {

                Vector3 spawnPosition = Vector3.zero;
                if (spawnPositions.Count >= listOfControllers.Count)
                    spawnPosition = spawnPositions[listOfControllers.Count].position;

                GameObject newPlayerObj = Instantiate(playerPrefab, spawnPosition, spawnPositions[listOfControllers.Count].rotation);
                newPlayerObj.name = "Player "+(ctPlayer.playerId+1).ToString();
                newPlayerObj.GetComponent<TankControl>().SetupPlayer(ctPlayer.playerId, false, device);
                ctPlayer.player = newPlayerObj;
            }

            listOfControllers.Add(ctPlayer);
            NumController++;
        }
    }

    void DetachDevice(InputDevice device)
    {
        for (int i = 0; i < listOfControllers.Count; i++)
        {
            if (listOfControllers[i].device.Equals(device))
            {
                Debug.Log("Detached Controller : ");
                ControllerToPlayer ctPlayer = listOfControllers[i];
                ctPlayer.controllerState = ControllerState.DETACHED;
                ctPlayer.gameState = GameState.NOT_PLAYING;
                ctPlayer.player.GetComponent<TankControl>().canControl = false;
                break;
            }
        }
    }

    IEnumerator CountDown() {
        yield return new WaitForSeconds(1);

        foreach (GameObject ui in readyUI) {
            ui.SetActive(false);
        }
        countDownUI.SetActive(true);
        
        currentCountDown = maxCountDown;
        while (currentCountDown > 0) {
            countDown.text = currentCountDown.ToString();
            yield return new WaitForSeconds(1);
            currentCountDown--;
        }
        countDown.text = "FIGHT !";
        yield return new WaitForSeconds(1);
        countDownUI.SetActive(false);
        foreach (ControllerToPlayer ct in listOfControllers) {
            ct.player.GetComponent<TankControl>().canControl = true;
        }


    }
}
