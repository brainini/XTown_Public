using Cinemachine;
using Photon.Pun;
using StarterAssets;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnCharacter : MonoBehaviourPunCallbacks
{
    public GameObject[] CharacterPrefabs;
    public Transform SpawnPoint;
    public CinemachineFreeLook FreeLookCam;
    private Transform FollowTarget;
    private GameObject Player;

    public static SpawnCharacter Instance = null;
    // Start is called before the first frame update
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this.gameObject);
        }

        Debug.Log("SpawnCharactery/Awake");
        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();

        SpawnPoint = GameObject.Find("SpawnPoint").transform;
        FreeLookCam = GameObject.Find("CharacterCam").GetComponent<CinemachineFreeLook>();

        // disable currentRoomCanvas
        RoomsCanvases.Instance.CurrentRoomCanvas.Hide();
        InitCharacter();
    }

    public ThirdPersonControllerMulti PlayerControl
    {
        get
        {
            Player = GameObject.FindWithTag("Player");
            if (Player == null) Debug.LogError("Player is null");
            return Player.GetComponent<ThirdPersonControllerMulti>();
        }
        private set
        {
            return;
        }
    }
    private GameObject _prefab;

    // singleton


    /// <summary>
    /// Monobeviour callbacks
    /// </summary>
    void OnEnable()
    {
        Debug.Log("SpawnCharacter/OnEnable");
        int selectedCharacter = PlayerPrefs.GetInt("selectedCharacter");
        _prefab = CharacterPrefabs[selectedCharacter];
    }

    private void OnDisable()
    {
        Debug.Log("SpawnCharacter disabled");
    }


    void Update()
    {
        if (Player == null)
        {
            Player = GameObject.FindWithTag("Player");
            if (Player != null)
            {
                //FollowTarget = Player.transform;
                FollowTarget = GameObject.Find("FollowTarget").transform;
                FreeLookCam.Follow = Player.transform;
                FreeLookCam.LookAt = FollowTarget;
            }
        }
    }

    void InitCharacter()
    {
        Debug.Log("SpawnCharacter/InitCharacter");
        if (!PhotonNetwork.InRoom || !PhotonNetwork.IsConnected)
        {// instantiate locally
            Debug.Log("SpawnCharacter/Instantiating player locally");
            Player = Instantiate(_prefab, SpawnPoint.position, Quaternion.identity);
        }
        else
        {
            // instantiate over the network
            Debug.Log("SpawnCharacter/Instantiating player over the network");
            Player = PhotonNetwork.Instantiate("CharacterPrefab", SpawnPoint.position, Quaternion.identity);
        }

        FollowTarget = Player.transform.Find("FollowTarget");
        FreeLookCam.Follow = Player.transform;
        FreeLookCam.LookAt = FollowTarget;
    }


}
