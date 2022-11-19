using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using Random = UnityEngine.Random;

public class MetaManager : MonoBehaviour
{
    #region Singleton
    public static MetaManager Instance;
    void Awake() {
        Instance = this;

        PhotonNetwork.SendRate = 10;
        
    }
    #endregion

    public Transform[] playerPoz;
    public Transform[] carPoz;

    #region NPC Management

    [SerializeField] GameObject NPC_prefab;
    [SerializeField] int total_npc;
    public TransformGroup[] footpath_poz;
    [System.Serializable]
    public class TransformGroup
    {
        public Transform[] pozs;
    }

    IEnumerator Start()
    {
        for (int i = 0; i < total_npc; i++)
        {
            int groupId = Random.Range(0, footpath_poz.Length);
            NPC_AI npc = Instantiate(NPC_prefab, footpath_poz[groupId].pozs[Random.Range(0, footpath_poz[groupId].pozs.Length)].position, Quaternion.identity).GetComponent<NPC_AI>();
            yield return new WaitForEndOfFrame();

            npc.SetGroupIndex(groupId);
        }
            
    }

    #endregion

    public GameObject mainCamera;
    public GameObject myPlayer;
    public ThirdPersonController myPlayerController;
    public PrometeoCarController myCarController;
    public InputManager inputManager;
    public static bool carNearby;
    public PrometeoCarController nearby_car_controller;
    public CinemachineVirtualCamera TPS_camera;
    public CinemachineFreeLook CAR_camera;
    public CinemachineFreeLook CAR_Back_camera;
    public FollowCarCamera topCameraFollow;


    
    public bool inRace=false;
    public string _fighterid;
    public Player inChallengePlayer;
    public void SetMyPlayer(ThirdPersonController _controller,PrometeoCarController _car)
    {
        myPlayerController = _controller;
        myPlayer = _controller.gameObject;
        myCarController = _car;

        TPS_camera.Follow = _controller.CinemachineCameraTarget.transform;
        CAR_camera.Follow = myCarController.transform;
        CAR_camera.LookAt = myCarController.transform;

        CAR_Back_camera.Follow = myCarController.transform;
        CAR_Back_camera.LookAt = myCarController.transform;

        topCameraFollow.EnableCamera(myCarController.transform,myPlayer.transform);
    }


  

    #region Event Callbacks Management
    private void OnEnable()
    {
        ThirdPersonController.OnPlayerTriggerCar += TriggeredCar;
    }

    private void TriggeredCar(bool entererd,PrometeoCarController carController)
    {
        
        carNearby = entererd;

        if (entererd)
        {
            nearby_car_controller = carController;
        }
        else
        {
            nearby_car_controller = null;
        }

    }

    private void OnDisable()
    {
        ThirdPersonController.OnPlayerTriggerCar -= TriggeredCar;
    }
    #endregion


    #region Camera Management
    public void ChangeCamera(CameraType type)
    {
        
        switch (type)
        {
            case CameraType.TP:
                {
                   
                   
                    TPS_camera.Priority = 10;
                    CAR_camera.Priority = 0;
                    CAR_Back_camera.Priority = 0;
                    myCarController.canDrive = false;
                    break;
                }
              
            case CameraType.CAR:
                {
                    if (!MetaManager.Instance.inRace)
                    {
                        myCarController.canDrive = true;
                    }

                   
                    TPS_camera.Priority = 0;
                    CAR_camera.Priority = 10;
                    CAR_Back_camera.Priority = 0;
                    break;
                }
            case CameraType.BACKCAR:
                {

                    

                    TPS_camera.Priority = 0;
                    CAR_camera.Priority = 10;
                    CAR_Back_camera.Priority = 20;
                    break;
                }
                
        }
    }
    #endregion


    public void UpdatePlayerWorldProperties(bool isRacing)
    { 
        var hash = PhotonNetwork.LocalPlayer.CustomProperties;
        hash["isRacing"] = isRacing;
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }


    

   
}
public enum CameraType
{
    TP,CAR,BACKCAR
}