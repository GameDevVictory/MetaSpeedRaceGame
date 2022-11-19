using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Singleton
    public static UIManager Instance;
    void Awake()
    {
        Instance = this;
    }
    #endregion

    public string username;
    public int user_char;

    [SerializeField] GameObject PressF_UI;
    [SerializeField] GameObject OpenCars_BTN;
    [SerializeField] GameObject soloRace_BTN;

    #region Event Callbacks Management
    private void OnEnable()
    {
        ThirdPersonController.OnPlayerTriggerCar += PlayerTriggeredCar;
        ThirdPersonController.OnPlayerEnteredCar += PlayerEnteredCar;
        PrometeoCarController.OnBoosterValueChange += HandleBoostChange;
    }



    private void OnDisable()
    {
        ThirdPersonController.OnPlayerTriggerCar -= PlayerTriggeredCar;
        PrometeoCarController.OnBoosterValueChange -= HandleBoostChange;
        ThirdPersonController.OnPlayerEnteredCar -= PlayerEnteredCar;
    }

    

    private void PlayerTriggeredCar(bool _enter,PrometeoCarController carController)
    {
        ToggleInteractUI(_enter);
    }
    private void PlayerEnteredCar()
    {
        ToggleInteractUI(false);
        HandleBoostChange(MetaManager.Instance.myCarController);
    }



    #endregion

    #region UI Methods
    public void ToggleInteractUI(bool value)
    {
        PressF_UI.SetActive(value);
    }
    #endregion


    #region Information Popups
    [Header("Informaion (InGame)")]
    [SerializeField] GameObject information_box;
    [SerializeField] TMP_Text information_text;
    [SerializeField] Image information_image;
    Coroutine info_coroutine;

    [Space(20f)]
    [Header("Informaion (Login)")]
    [SerializeField] TMP_Text usernameText;
    [SerializeField] TMP_Text statusText;


    public void ShowInformationMsg(string msg,float time,Sprite image=null)
    {
        if (image != null)
        {
            information_image.sprite = image;
            information_image.gameObject.SetActive(true);
        }
        else
        {
            information_image.gameObject.SetActive(false);
        }

        information_text.text = msg;

        if (info_coroutine != null)
        {
            StopCoroutine(info_coroutine);
        }
        info_coroutine = StartCoroutine(disableInformationMsg(time));
    }
    IEnumerator disableInformationMsg(float time)
    {
        LeanTween.cancel(information_box);

        information_box.SetActive(true);
        LeanTween.scaleY(information_box, 1, 0.15f).setFrom(0);
        AudioManager.Instance.playSound(0);

        yield return new WaitForSeconds(time);

        LeanTween.scaleY(information_box,0,0.15f).setOnComplete(()=> {
            information_box.SetActive(false);
        });


    }

    private void Update()
    {
        if (MetaManager.Instance.myPlayerController != null)
        {
            if(MetaManager.Instance.myPlayerController._pState==PlayerState.RACE ){
                if (PressF_UI.activeSelf)
                {
                    PressF_UI.SetActive(false);
                }
                if (OpenCars_BTN.activeSelf)
                {
                    OpenCars_BTN.SetActive(false);
                }
                if (soloRace_BTN.activeSelf)
                {
                    soloRace_BTN.SetActive(false);
                }
            }
            else
            {
                OpenCars_BTN.SetActive(true);
                soloRace_BTN.SetActive(true);
            }
        }
    }
    public void UpdateUserName(string _name, string _ethad = null)
    {
        if (_ethad != null)
        {
            usernameText.text = "Hi, " + _name + "\n  Your crypto address is : " + _ethad;
            username = _name;
        }
        else usernameText.text = _name;
    }

    public void UpdateStatus(string _msg)
    {
        statusText.text = _msg;
        StartCoroutine(ResetUpdateText());
    }

    IEnumerator ResetUpdateText()
    {
        yield return new WaitForSeconds(2);
        statusText.text = "";
    }
    #endregion


    #region Edit Profile Section
    [SerializeField] Toggle[] char_toggles;
    [SerializeField] TMP_InputField name_input;
    public void OpenEditProfile()
    {
        LocalData data = DatabaseManager.Instance.GetLocalData();

        name_input.text = data.name;
        for (int i = 0; i < char_toggles.Length; i++)
        {
            if (i == data.char_id)
            {
                char_toggles[data.char_id].isOn = true;
                break;
            }           
        }

        start_ui_btns.SetActive(false);
        editprofile_ui.SetActive(true);
    }
    public void SetProfile()
    {
        if (string.IsNullOrEmpty(name_input.text)) return;

        LocalData data = DatabaseManager.Instance.GetLocalData();

        data.name = name_input.text;
        for (int i = 0; i < char_toggles.Length; i++)
        {
            if (char_toggles[i].isOn)
            {
                data.char_id = i;
                break;
            }           
        }
        DatabaseManager.Instance.UpdateData(data);


        start_ui_btns.SetActive(true);
        editprofile_ui.SetActive(false);
        UpdateUserName(data.name, SingletonDataManager.userethAdd);
    }
    #endregion

   
    public void UpdatePlayerUIData(bool _show, LocalData data, bool _init = false)
    {
        if (_show)
        {
           

            //scoreTxt.text = data.coins.ToString();


            // if (PhotonNetwork.LocalPlayer.CustomProperties["health"] != null) healthSlider.value = float.Parse(PhotonNetwork.LocalPlayer.CustomProperties["health"].ToString());
        }        
    }

    #region Panel Management
    [Space(20f)]
    [Header("Panels")]
    [SerializeField] GameObject login_ui;
    [SerializeField] GameObject start_ui;
    [SerializeField] GameObject gameplay_ui;
    [SerializeField] GameObject start_ui_btns;
    [SerializeField] GameObject editprofile_ui;

    [Header("Buttons")]
    [SerializeField] GameObject loginui_btns;

    public void StartGame()
    {
        //start_ui.SetActive(false);
        //StartUI.SetActive(false);
         if (PlayerPrefs.GetInt("tutorial", 0) == 0)
         {
             ShowTutorial();
         }
         else
         {
             MPNetworkManager.insta.OnConnectedToServer();
         }
        //MPNetworkManager.insta.OnConnectedToServer();
    }

    #region Tutorial
    [Header("Tutorial")]
    [SerializeField] GameObject TutorialUI;
    [SerializeField] GameObject[] tutorialObjects;
    int currentTutorial=0;
    public void ShowTutorial()
    {
        TutorialUI.SetActive(true);
        for (int i = 0; i < tutorialObjects.Length; i++)
        {
            tutorialObjects[i].SetActive(false);
        }
        tutorialObjects[currentTutorial].SetActive(true);
    }
    public void NextTutorial()
    {
        tutorialObjects[currentTutorial].SetActive(false);
        currentTutorial++;
        if (currentTutorial >= tutorialObjects.Length)
        {
            SkipTutorial();
            return;
        }
        tutorialObjects[currentTutorial].SetActive(true);
    }
    public void SkipTutorial()
    {
        PlayerPrefs.SetInt("tutorial", 1);
        TutorialUI.SetActive(false);
        StartGame();
        //MPNetworkManager.insta.OnConnectedToServer();
    }


    #endregion
    public void ToggleGameplayUI(bool enabled)
    {
        gameplay_ui.SetActive(enabled);
    }
    public void ToggleStartUI(bool enabled)
    {
        start_ui.SetActive(enabled);
    }
    #endregion

    #region VoiceChat

    [Header("VoiceChat")]
    [SerializeField] FrostweepGames.WebGLPUNVoice.Recorder recorder;
    [SerializeField] FrostweepGames.WebGLPUNVoice.Listener lister;    
    [SerializeField] Image recorderImg;
    [SerializeField] Image listenerImg;
    [SerializeField] Sprite[] recorderSprites; //0 on 1 off
    [SerializeField] Sprite[] listenerSprites; //0 on 1 off
    public void MuteUnmute()
    {
        if (recorder.recording)
        {
            recorder.recording = false;
            recorderImg.sprite = recorderSprites[1];
            recorder.StopRecord();
        }
        else
        {
            recorder.RefreshMicrophones();
            recorder.recording = true;
            recorder.StartRecord();
            recorderImg.sprite = recorderSprites[0];
        }
    }

    public void MuteUnmuteListner()
    {
        if (lister._listening)
        {
            lister._listening = false;
            listenerImg.sprite = listenerSprites[1];
        }
        else
        {
            lister._listening = true;
            listenerImg.sprite = listenerSprites[0];
        }
    }

    #endregion

    #region Fight Request
    [Space(20f)]
    [Header("Fight Request")]
    [SerializeField] GameObject FightRequestUI;
    [SerializeField] TMP_Text fightRequestText;

    public void FightReq(string _userdata)
    {
        FightRequestUI.SetActive(true);
        fightRequestText.text = _userdata + " want to fight with you !";
    }

    public void FightReqAcion(bool _accept)
    {
     
        MetaManager.Instance.myPlayer.GetComponent<ChallengeRequestManager>().RequestFightAction(_accept);
        FightRequestUI.SetActive(false);
        Debug.Log("Fight Action " + _accept);
        // PhotonView photonView = PhotonView.Get(this);
        // photonView.RPC("UpdateHealthMe", RpcTarget.All, PhotonNetwork.LocalPlayer.UserId);
    }
    #endregion

    #region Solo Play Management
    public void PlaySoloRace()
    {
        MetaManager.Instance.myPlayer.GetComponent<ChallengeRequestManager>().PlaySoloRace();
    }
    #endregion


    #region Car Info Management
    [Header("Car Data")]
    [SerializeField] Image boostFillAmount;
    [SerializeField] GameObject carBoost_UI;
    [SerializeField] GameObject Speedometer;
    [SerializeField] Transform speed_kanto;
    [SerializeField] TMP_Text speed_text;
    [SerializeField] float max_kanto_rotation;

    public void ToggleBoostUI(bool enabled)
    {
        carBoost_UI.SetActive(enabled);
        Speedometer.SetActive(true);
        HandleBoostChange(MetaManager.Instance.myCarController);
        SetSpeedometer(0);

    }
    public void SetSpeedometer(float currentSpeed,float max_speed=280f)
    {
        speed_kanto.transform.rotation = Quaternion.Euler(0,180,(currentSpeed/ max_speed) * max_kanto_rotation);
        speed_text.text = ((int)currentSpeed).ToString();
    }
    private void HandleBoostChange(PrometeoCarController carController)
    {
        if (carController.nitroTimer <= 0)
        {
            boostFillAmount.fillAmount = 0;
            return;
        }
        boostFillAmount.fillAmount = carController.current_nitroTimer / carController.nitroTimer;                
    }



    #endregion

    #region Game Complete UI & Token UI
    [SerializeField] GameObject gamecomplete_ui;
    [SerializeField] TMP_Text game_status_text;
    [SerializeField] TMP_Text reward_got_text;
    [SerializeField] GameObject TokenUI;
    public void ShowGameCompleteUI(bool won, bool isSoloRace,bool showToken=false)
    {
        gamecomplete_ui.SetActive(true);
        game_status_text.text = won ? "Awesome Driving Skills!" : "Improve Your Driving!";
        
        int coins = won?500:250;

        if (isSoloRace)
        {
            coins = 250;
        }

        if (won)
        {
            reward_got_text.text = "Coins earned: " + coins.ToString();
            LocalData data = DatabaseManager.Instance.GetLocalData();
            data.coins += coins;
            DatabaseManager.Instance.UpdateData(data);
        }
        else
        {
            reward_got_text.text = "Coins earned: " + coins.ToString();
        }


        LeanTween.scale(gamecomplete_ui.transform.GetChild(0).gameObject, Vector3.one, 0.3f).setFrom(Vector3.zero).setEaseInQuad();

        Debug.Log("showToken : " + showToken);
        if (showToken)
        {
            ShowTokenUI();
        }


        SetCoinText();

    }

    private void ShowTokenUI()
    {
        TokenUI.SetActive(true);
    }
    public void ClaimToken()
    {
        BlockchainManager.Instance.GetTokenReward();
        TokenUI.SetActive(false);
    }


    public void CloseGameCompleteUI()
    {        
        LeanTween.scale(gamecomplete_ui.transform.GetChild(0).gameObject, Vector3.zero, 0.3f).setFrom(Vector3.one).setEaseOutQuad().setOnComplete(()=> {
            gamecomplete_ui.SetActive(false);
        });
       // StartCoroutine(MetaManager.Instance.myPlayerController.ResetPlayerPozRot());
    }
    #endregion

    #region Coin & Token Texts
    [SerializeField] TMP_Text[] coin_texts;
    [SerializeField] TMP_Text[] token_texts;
    public void SetCoinText()
    {
        int coins = DatabaseManager.Instance.GetLocalData().coins;
        for (int i = 0; i < coin_texts.Length; i++)
        {
            coin_texts[i].text = coins.ToString();
        }
    }

    public void SetTokenBalanceText()
    {       
        for (int i = 0; i < token_texts.Length; i++)
        {
            token_texts[i].text = SingletonDataManager.userTokenBalance;
        }
    }
    #endregion

    #region Emote UI
    [SerializeField] GameObject emote_ui;
    public void ToggleEmoteUI(bool enabled)
    {
        emote_ui.SetActive(enabled);
    }

   
    #endregion


}

