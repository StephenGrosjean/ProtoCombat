using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;
using XInputDotNetPure;


public class MultiControllerManager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private List<Transform> spawnPositions;

    public struct ControllerToPlayer
    {
        public InputDevice device;
        public int playerId;
        public ControllerState controllerState;
        public GameObject player;
        public GameState gameState;
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

    // Start is called before the first frame update
    void Awake()
    {
        InputManager.OnDeviceAttached += AttachDevice;
        InputManager.OnDeviceDetached += DetachDevice;
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
                ctPlayer.player.GetComponent<TankControl>().canControl = true;
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

                GameObject newPlayerObj = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
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
}
