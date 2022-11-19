using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChallengeRequestManager : MonoBehaviour, IOnEventCallback
{
    [SerializeField] PhotonView pv;

    private void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }
    private void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    private const byte FightEventCode = 1;

    public void RequestFight()
    {
        if (MetaManager.Instance.inRace) { return; }

        Debug.Log("RequestFight" + pv.Owner.NickName + " uId: "+ pv.Owner.UserId);
        

        MetaManager.Instance._fighterid = pv.Owner.UserId;
        //AudioManager.Instance.playSound(2);
        // MetaManager.insta.myPlayer.GetComponent<MyCharacter>().pview.RPC("RequestFightRPC", RpcTarget.All, pview.Owner.UserId);
        pv.RPC("RequestFightRPC", RpcTarget.All, pv.Owner.UserId);

        Debug.Log("RequestFight My " + MetaManager.Instance.myPlayer.GetComponent<PhotonView>().Owner.UserId + " | figher " + MetaManager.Instance._fighterid);


        UIManager.Instance.UpdateStatus("Fight request sent to\n" + pv.Owner.NickName);
        MetaManager.Instance.inChallengePlayer = pv.Owner;
    }


    public System.Action<int,int,int> OnPlayerAcceptedChallenge;
    
    [PunRPC]
    void RequestFightRPC(string _uid, PhotonMessageInfo info)
    {
        
        if (pv.IsMine)
        {

          
            if (pv.Owner.UserId.Equals(_uid))
            {
                if (MetaManager.Instance.inChallengePlayer != null) return;

               

                MetaManager.Instance._fighterid = info.Sender.UserId;                
                MetaManager.Instance.inChallengePlayer = info.Sender;
                //MetaManager.fighterView = info.photonView;
                //MetaManager.fightPlayer = info.photonView.gameObject;
                UIManager.Instance.FightReq(info.Sender.ToString());
               // AudioManager.Instance.playSound(3);

                Debug.Log("RequestFightRPC My " + MetaManager.Instance.myPlayer.GetComponent<PhotonView>().Owner.UserId + " | figher " + MetaManager.Instance._fighterid);

            }
        }
    }
    public void PlaySoloRace()
    {

        int race_start_index = RaceManager.Instance.GetRacePositionsIndex();
        int race_path_index = RaceManager.Instance.GetPathIndex();
        //int race_env_poz_index = RaceManager.Instance.GetRaceEnvironMentPoistionIndex();
        Debug.Log("Race Can Start On Position " + race_start_index + " And On Path " + race_path_index);
        MetaManager.Instance.inChallengePlayer = null;
        MetaManager.Instance.inRace = true;        
        //TODO ENABLE RACE CAR 
        OnPlayerAcceptedChallenge?.Invoke(race_start_index, race_path_index, Random.Range(0,2));
       
    }

    public void RequestFightAction(bool _action)
    {


        //float randomPos = UnityEngine.Random.Range(0, 1000);


        //RESET PLAYER AND ENABLE CAR AND ALLOCATE RACE AREA

        if ((bool)MetaManager.Instance.inChallengePlayer.CustomProperties["isRacing"])
        {
            
            UIManager.Instance.ShowInformationMsg("Other Player is not available! Try Challenge Others", 2f);
            MetaManager.Instance.inChallengePlayer = null;
            return;
        }

        //Data
        int race_start_index = RaceManager.Instance.GetRacePositionsIndex();
        int race_path_index = RaceManager.Instance.GetPathIndex();
        //int race_env_poz_index = RaceManager.Instance.GetRaceEnvironMentPoistionIndex();
        Debug.Log("Race Can Start On Position " + race_start_index + " And On Path "+ race_path_index);


        if (_action)
        {

            MetaManager.Instance.inRace = true;
            //TODO ENABLE RACE CAR 
            OnPlayerAcceptedChallenge?.Invoke(race_start_index,race_path_index ,0);
        }

        /*RaceObjectPool.Instance.offsetsOfGeneration = offsets;*/
        Debug.Log(MetaManager.Instance.inChallengePlayer.UserId);
        SendFightAction(_action, race_start_index,race_path_index, MetaManager.Instance.inChallengePlayer.UserId, PhotonNetwork.LocalPlayer.UserId);

    }

    private void SendFightAction(bool _action, int racePozIndex, int racePathIndex,  string _p1uid, string _p2uid)
    {
      
        object[] content = new object[] { _action, racePozIndex, racePathIndex, _p1uid, _p2uid }; // Array contains the target position and the IDs of the selected units
        RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others }; // You would have to set the Receivers to All in order to receive this event on the local client as well

        PhotonNetwork.RaiseEvent(FightEventCode, content, raiseEventOptions, SendOptions.SendReliable);
    }

    public void OnEvent(EventData photonEvent)
    {

        byte eventCode = photonEvent.Code;
        if (eventCode == FightEventCode)
        {
            if ((bool)pv.Owner.CustomProperties["isRacing"]) return;

            object[] data = (object[])photonEvent.CustomData;
            bool _action = (bool)data[0];
            int racePozIndex  = (int)data[1];
            int racePathIndex = (int)data[2];
            

            /*RaceObjectPool.Instance.offsetsOfGeneration =Newtonsoft.Json.JsonConvert.DeserializeObject<float[]>((string)data[2]);*/

            for (int i = 3; i < data.Length; i++)
            {
                if (pv.Owner.UserId.Equals((string)data[i]))
                {
                    if (_action)
                    {
                      
                        UIManager.Instance.UpdateStatus(PhotonNetwork.CurrentRoom.Players[photonEvent.Sender].NickName + " is ready to fight");
                        if (pv.IsMine)
                        {
                           // can_move = false;
                            MetaManager.Instance.inRace = true;
                            /*   var hash = PhotonNetwork.LocalPlayer.CustomProperties;
                               hash["isfighting"] = true;
                               PhotonNetwork.LocalPlayer.SetCustomProperties(hash);*/

                            //TODO ENABLE RACE CAR 
                            //StartCoroutine(GoToBuildPos(1, yPos));
                            Debug.Log("Race Can Start On Position " + racePozIndex + " And On Path " + racePathIndex);

                            OnPlayerAcceptedChallenge?.Invoke(racePozIndex,racePathIndex, 1);

                        }
                        else
                        {
                            MetaManager.Instance.inChallengePlayer = pv.Owner;
                        }

                        //SelectWeapon();
                        //AudioManager.insta.playSound(4);
                    }
                    else
                    {
                        //Debug.Log(info.Sender + " rejected fight");
                       // AudioManager.insta.playSound(5);
                        UIManager.Instance.UpdateStatus(PhotonNetwork.CurrentRoom.Players[photonEvent.Sender].NickName + " rejected fight");
                    }
                }
            }
        }
    }
}
