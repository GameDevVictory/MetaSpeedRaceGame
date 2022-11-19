using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;
public class MPNetworkManager : MonoBehaviourPunCallbacks
{
    public static MPNetworkManager insta;


    [SerializeField] MPNetworkPlayerSpawner NPSpawner;
    [SerializeField] int MaxPlayers;

    //public static List<GameObject> _characters = new List<GameObject>();

    [SerializeField] FrostweepGames.WebGLPUNVoice.Recorder rec;
    [SerializeField] GameObject VoiceObj;

    private void Awake()
    {
        insta = this;
    }

    private void Start()
    {
        // OnConnectedToServer();
        VoiceObj.SetActive(true);
        rec.RefreshMicrophones();
        rec.reliableTransmission = false;
    }

    #region CommonStuff
    public void OnConnectedToServer()
    {

        if (PhotonNetwork.IsConnectedAndReady)
        {
            JoinRandomRoom();
            Debug.Log("OnConnectedToServer bypass");
            return;
        }
        Debug.Log("OnConnectedToServer");
        PhotonNetwork.ConnectUsingSettings();
        UIManager.Instance.UpdateUserName("Connecting to server");
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        Debug.Log("OnDisconnected " + cause);

    }


    public override void OnConnectedToMaster()
    {
        Debug.Log("OnConnectedToMaster");
        base.OnConnectedToMaster();
        UIManager.Instance.UpdateUserName("Connected to server");
        //int KillScore = (int)PhotonNetwork.LocalPlayer.CustomProperties["role"];
        // KillScore++;
        // Hashtable hash = new Hashtable();
        //hash.Add("playeravatar", 0);
        //PhotonNetwork.LocalPlayer.SetCustomProperties(hash);


        //PhotonNetwork.JoinOrCreateRoom("private", myRoom, TypedLobby.Default);
        //PhotonNetwork.NickName = DatabaseManager.Instance.GetLocalData().name;
        JoinRandomRoom();
    }


    /*Hashtable CreateRoomProperties()
    {
        return new Hashtable
        {
            {"levelIndex", 0}
        };
    }*/

    string[] CreateRoomPropertiesForLobby()
    {
        return new string[]
        {
            "sectorIndex",
            "roomIndex"
        };
    }
    #endregion

    public int sectorIndex = 0;
    public int roomIndex = 0;

    #region RandomRoom
    private void JoinRandomRoom()
    {
        UIManager.Instance.UpdateUserName("Entering to Meta Race");
        //int temoNo = Random.Range(0, 2);
        //Debug.Log("PlayerNo " + temoNo);
        PhotonNetwork.NickName = DatabaseManager.Instance.GetLocalData().name;

        Hashtable hash = new Hashtable();
        hash.Add("char_no", DatabaseManager.Instance.GetLocalData().char_id);
        hash.Add("isRacing", false);        
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);

        //roomIndex = PlayerPrefs.GetInt("musePoz", 0); //room index on network
        Hashtable expectedCustomRoomProperties = new Hashtable
        {
            {"sectorIndex", sectorIndex}            
        };
        // PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties , 0);
        PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, (byte)MaxPlayers, MatchmakingMode.FillRoom, null, null);

    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("OnJoinRandomFailed");
        Debug.LogFormat("Join Random Failed with error code {0} and error message {1}", returnCode, message);
        // here usually you create a new room
        CreateRoom();

    }
    #endregion

    #region NormalRoomOperations
    private void CreateRoom()
    {
        Debug.Log("CreateRoom");
        roomIndex = PlayerPrefs.GetInt("musePoz", 0); //room index on network
        Hashtable expectedCustomRoomProperties = new Hashtable
        {
            {"sectorIndex", sectorIndex}
        };

        RoomOptions myRoom = new RoomOptions();
        myRoom.MaxPlayers = (byte)MaxPlayers;
        myRoom.IsVisible = true;
        myRoom.IsOpen = true;
        myRoom.PublishUserId = true;
        myRoom.CleanupCacheOnLeave = true;
        
        myRoom.CustomRoomProperties = expectedCustomRoomProperties;
        myRoom.CustomRoomPropertiesForLobby = CreateRoomPropertiesForLobby();
        PhotonNetwork.CreateRoom(null, myRoom, null);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogErrorFormat("Room creation failed with error code {0} and error message {1}", returnCode, message);
    }

    public override void OnCreatedRoom()
    {
        
        Debug.Log("OnCreatedRoom");
    }

    public override void OnJoinedRoom()
    {
        // joined a room successfully, CreateRoom leads here on success
        UIManager.Instance.UpdateUserName("Ready to play !");
        Debug.Log("OnJoinedRoom");

        //rec.RefreshMicrophones();
        StartGameCall();
    }

    void StartGameCall()
    {
        NPSpawner.GeneratePlayer();
        rec.RefreshMicrophones();
        UIManager.Instance.ToggleStartUI(false);
       // rec.StartRecord();
        //UIManager.insta.MuteUnmute();
        MetaManager.Instance.UpdatePlayerWorldProperties(false);
        UIManager.Instance.ToggleGameplayUI(true);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        //base.OnJoinRoomFailed(returnCode, message);
        UIManager.Instance.UpdateUserName("Server error, unble to join");
    }
    #endregion


    #region PlayerOperetaions
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("OnPlayerEnteredRoom");
        base.OnPlayerEnteredRoom(newPlayer);

        UIManager.Instance.UpdateStatus(newPlayer.NickName + " Joined");

    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log("OnPlayerLeftRoom");
        base.OnPlayerLeftRoom(otherPlayer);
        UIManager.Instance.UpdateStatus(otherPlayer.NickName + " Left");
    }


    #endregion


}
