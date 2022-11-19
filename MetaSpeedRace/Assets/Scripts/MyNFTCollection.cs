using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MyNFTCollection : MonoBehaviour
{
    public static MyNFTCollection insta;
    

        
    

    int currentSelectedItem = 0;
    [SerializeField] GameObject Canvas3DObject;

    [SerializeField] GameObject LoadingMyCollection;
    [SerializeField] GameObject MyCollectionObject;

    [SerializeField] PlayerCarsInfo playerCarInfo;

    [SerializeField] List<int> available_cars=new List<int>();
    [SerializeField] List<int> all_cars = new List<int>();
    [SerializeField] Button buy_select_btn;
    [SerializeField] TMP_Text car_cost_text;



    public static Action OnCarUpgrade;

    private void Awake()
    {
        insta = this;
        this.gameObject.SetActive(false);
    }

    async private void OnEnable()
    {
        LoadingMyCollection.SetActive(true);
        MyCollectionObject.SetActive(false);

        selectedCarinUI = 0;
        // ClosePurchasePanel();
        all_cars.Clear();
        for (int i = 0; i < PlayerCarsInfo.Instance.all_car_details.Count; i++)
        {
            all_cars.Add(PlayerCarsInfo.Instance.all_car_details[i].car_index);
        }

        await BlockchainManager.Instance.CheckPuzzleList();

        
        SetNewData();
       

        prev_BTN.SetActive(false);
       
        if (available_cars.Count > 0)
        {
            Enable3DCar(available_cars[0]);
            LoadingMyCollection.SetActive(false);
            MyCollectionObject.SetActive(true);
        }

       
        next_BTN.SetActive(true);
        

        Canvas3DObject.SetActive(true);
        UIManager.Instance.SetCoinText();
       
    }

    public void SetNewData()
    {
        List<string> temp_list = new List<string>();



        available_cars.Clear();
        available_cars.Add(0);



        temp_list = BlockchainManager.Instance.nftList;

        if (temp_list.Count > 0)
        {
            for (int i = 0; i < temp_list.Count; i++)
            {
                if (temp_list[i].StartsWith("50") && temp_list[i].Length == 3)
                {
                    available_cars.Add(Int32.Parse(temp_list[i]) - 499);
                    //MyNFTCollection.insta.GenerateItem(Int32.Parse(temp_list[i]));
                }
            }
        }

        LocalData data = DatabaseManager.Instance.GetLocalData();


       

        for (int i = 0; i < available_cars.Count; i++)
        {
            data.carDetails[available_cars[i]].isBought = true;
           Debug.Log("Available : " + available_cars[i]);
        }
        DatabaseManager.Instance.UpdateData(data);
    }

    public int lastSelectedButton = -1;
    public void DisableLastSelectedButton()
    {
        Debug.Log(lastSelectedButton + 1);

        if (!available_cars.Contains(lastSelectedButton))
        {
            available_cars.Add(lastSelectedButton);
                        
            Enable3DCar(lastSelectedButton);
            
        }
    }
    private void OnDisable()
    {
        Canvas3DObject.SetActive(false);
    }
    #region Car Info Management
        
    //0-Accel  1-Max Speed 2-Braking 3-NItroTime  4-Drift Control
    [SerializeField] SliderInfo[] all_slider;
    [SerializeField] GameObject[] all_overlay_cars;
    int selectedCarinUI = 0;
    [SerializeField] GameObject prev_BTN;
    [SerializeField] GameObject next_BTN;
    [SerializeField] TMP_Text carInfo_txt;

    [System.Serializable]
    public class SliderInfo
    {
        public Slider slider;
        public TMP_Text min_value;
        public TMP_Text max_value;
        public Button upgradeBTN;
        public TMP_Text upgrade_text;
    }

    void Enable3DCar(int index)
    {
        for (int i = 0; i < all_overlay_cars.Length; i++)
        {
            all_overlay_cars[i].SetActive(false);
        }

        all_overlay_cars[index].SetActive(true);
        SetCarInfo(index);
    }
    public void NextCar()
    {

        if (selectedCarinUI >= all_cars.Count - 1) return;

        selectedCarinUI++;
        if (selectedCarinUI == all_cars.Count - 1) next_BTN.SetActive(false);


        Enable3DCar(all_cars[selectedCarinUI]);
        prev_BTN.SetActive(true);

       // Debug.LogWarning("Change Car Stats UI HERE");

    }
    public void PreviousCar()
    {
        if (selectedCarinUI == 0) return;

        selectedCarinUI--;

        if (selectedCarinUI == 0) prev_BTN.SetActive(false);

        next_BTN.SetActive(true);
        Enable3DCar(all_cars[selectedCarinUI]);
        //Debug.LogWarning("Change Car Stats UI HERE");
    }


    [SerializeField] TMP_Text txt_mycoins;
    
    

    public bool test = false;
    //0-FireRate 1-Damage  2=Reload Time 3-Accuracy 

    public void ClampDriftToggle()
    {
        CarUpgradeInfo car_details = PlayerCarsInfo.Instance.all_car_details.Find(x => x.car_index == lastSelectedCar);
        CarUpgradePrice car_upgrade_details = PlayerCarsInfo.Instance.car_upgrades.Find(x => x.car_index == lastSelectedCar);

        if (all_slider[4].slider.value > car_upgrade_details.handling.amount[car_details.current_handling_level]) {
            all_slider[4].slider.value = car_upgrade_details.handling.amount[car_details.current_handling_level];
        }
        
        OnCarUpgrade?.Invoke();
        
    }
    public void TogglePointerUp()
    {
        LocalData data = DatabaseManager.Instance.GetLocalData();
        data.carDetails[lastSelectedCar].handling_amount = all_slider[4].slider.value;
        DatabaseManager.Instance.UpdateData(data);
    }
    public void UpgradeStats(int stat_index)
    {
        LocalData data = DatabaseManager.Instance.GetLocalData();

#if UNITY_EDITOR
        if (test)
        {
            data.coins = 999999;
        }
#endif
        CarUpgradeInfo car_details = PlayerCarsInfo.Instance.all_car_details.Find(x => x.car_index == lastSelectedCar);
        CarUpgradePrice car_upgrade_details = PlayerCarsInfo.Instance.car_upgrades.Find(x => x.car_index == lastSelectedCar);


        //0-Accel  1-Max Speed 2-Braking 3-NItroTime  4-Drift Control

        switch (stat_index)
        {
            case 0:
                {
                    //Acceleraion

                    int requiredAmount = car_upgrade_details.acceleratoin.cost[car_details.current_acceleratoin_level];
                    
                    if (data.coins >= requiredAmount )
                    {
                        car_details.current_acceleratoin_level++;                        
                        data.coins -= requiredAmount;
                        data.carDetails = PlayerCarsInfo.Instance.all_car_details;
                        DatabaseManager.Instance.UpdateData(data);
                        AudioManager.Instance.playSound(4);
                        OnCarUpgrade?.Invoke();
                    }
                    else
                    {
                        MessaeBox.insta.showMsg("Not Enough Coins!", true);
                    }
                    break;
                }
            case 1:
                {
                    //Max Speed
                    int requiredAmount = car_upgrade_details.speed.cost[car_details.current_speed_level];

                    if (data.coins >= requiredAmount)
                    {
                        car_details.current_speed_level++;                        
                        data.coins -= requiredAmount;
                        data.carDetails = PlayerCarsInfo.Instance.all_car_details;
                        DatabaseManager.Instance.UpdateData(data);
                        AudioManager.Instance.playSound(4);
                        OnCarUpgrade?.Invoke();
                    }
                    else
                    {
                        MessaeBox.insta.showMsg("Not Enough Coins!", true);
                    }

                    break;
                }
            case 2:
                {
                    //Braking

                    int requiredAmount = car_upgrade_details.braking.cost[car_details.current_braking_level];

                    if (data.coins >= requiredAmount)
                    {
                        car_details.current_braking_level++;                       
                        data.coins -= requiredAmount;
                        data.carDetails = PlayerCarsInfo.Instance.all_car_details;
                        DatabaseManager.Instance.UpdateData(data);
                        AudioManager.Instance.playSound(4);
                        OnCarUpgrade?.Invoke();
                    }
                    else
                    {
                        MessaeBox.insta.showMsg("Not Enough Coins!", true);
                    }
                    break;
                }
            case 3:
                {
                    //3 - NItroTime  

                    int requiredAmount = car_upgrade_details.nitrus_time.cost[car_details.current_nitrus_level];

                    if (data.coins >= requiredAmount)
                    {
                        car_details.current_nitrus_level++;                        
                        data.coins -= requiredAmount;
                        data.carDetails = PlayerCarsInfo.Instance.all_car_details;
                        DatabaseManager.Instance.UpdateData(data);
                        AudioManager.Instance.playSound(4);
                        OnCarUpgrade?.Invoke();
                    }
                    else
                    {
                        MessaeBox.insta.showMsg("Not Enough Coins!", true);
                    }

                    break;
                }
            case 4:
                {
                    //4 - Drift Control  

                    int requiredAmount = car_upgrade_details.handling.cost[car_details.current_handling_level];

                    if (data.coins >= requiredAmount)
                    {

                        car_details.handling_amount = car_upgrade_details.handling.amount[car_details.current_handling_level];
                        car_details.current_handling_level++;                        

                        data.coins -= requiredAmount;
                        data.carDetails = PlayerCarsInfo.Instance.all_car_details;

                        DatabaseManager.Instance.UpdateData(data);
                        AudioManager.Instance.playSound(4);
                        OnCarUpgrade?.Invoke();
                    }
                    else
                    {
                        MessaeBox.insta.showMsg("Not Enough Coins!", true);
                    }

                    break;
                }



        }

        SetCarInfo(lastSelectedCar);
        UIManager.Instance.SetCoinText();;
    }

    int lastSelectedCar;

   
    void SetCarInfo(int carIndex)
    {
        lastSelectedCar = carIndex;
        
       
        CarUpgradeInfo car_details = PlayerCarsInfo.Instance.all_car_details.Find(x => x.car_index == carIndex);
        CarUpgradeInfo car_defaultDetails = PlayerCarsInfo.Instance.carDefaultData.Find(x => x.car_index == carIndex);
        CarUpgradePrice car_upgrade_details = PlayerCarsInfo.Instance.car_upgrades.Find(x => x.car_index == carIndex);



        bool isBought = available_cars.Contains(carIndex);

        buy_select_btn.onClick.RemoveAllListeners();
        if (isBought)
        {
            buy_select_btn.transform.GetChild(0).GetComponent<TMP_Text>().text = "Select";
            Debug.Log("Car is Available");
            buy_select_btn.onClick.AddListener(() => {
                SelectCar(carIndex);
            });

            car_cost_text.gameObject.SetActive(false);
        }
        else
        {
            buy_select_btn.transform.GetChild(0).GetComponent<TMP_Text>().text = "Buy";

            buy_select_btn.onClick.AddListener(() => {
                PurchaseCar(carIndex);
            });

            car_cost_text.gameObject.SetActive(true);
            car_cost_text.text = car_defaultDetails.carCost.ToString();
        }

        // Debug.Log(car_details.car_index);

        if (car_details == null)
        {
            for (int i = 0; i < all_slider.Length; i++)
            {
                all_slider[i].slider.gameObject.SetActive(false);
            }
            return;
        }


        if (DatabaseManager.Instance.allMetaDataServer.Count > carIndex)
        {
            carInfo_txt.text = DatabaseManager.Instance.allMetaDataServer[carIndex].description;
        }
        else
        {
            carInfo_txt.text = "";
        }
            
        
        
        
        for (int i = 0; i < all_slider.Length; i++)
        {
            all_slider[i].slider.gameObject.SetActive(true);


            //0-Accel  1-Max Speed 2-Braking 3-NItroTime  4-Drift Control
            switch (i)
            {
                case 0:
                    {
                       //ACCELERARION
                        all_slider[i].slider.minValue = 1;
                        all_slider[i].slider.maxValue = 50;


                        all_slider[i].min_value.text = "15";
                        all_slider[i].max_value.text = "50";
                        all_slider[i].slider.value = car_upgrade_details.acceleratoin.amount[car_details.current_acceleratoin_level];

                            
                        all_slider[i].upgradeBTN.gameObject.SetActive(isBought);
                        

                        if (car_details.current_acceleratoin_level >= car_upgrade_details.acceleratoin.cost.Length)
                        {
                            all_slider[i].upgradeBTN.gameObject.SetActive(false);
                        }
                        else
                        {
                            all_slider[i].upgrade_text.text = car_upgrade_details.acceleratoin.cost[car_details.current_acceleratoin_level].ToString();
                        }



                        break;
                    }
                case 1:
                    {
                        //MAX SPEED
                        all_slider[i].slider.minValue = 30;
                        all_slider[i].slider.maxValue = 600;


                        all_slider[i].min_value.text = "30";
                        all_slider[i].max_value.text = "600";

                        all_slider[i].slider.value = car_upgrade_details.speed.amount[car_details.current_speed_level];

                        all_slider[i].upgradeBTN.gameObject.SetActive(isBought);



                        if (car_details.current_speed_level >= car_upgrade_details.speed.cost.Length)
                        {
                            all_slider[i].upgradeBTN.gameObject.SetActive(false);
                         
                        }
                        else
                        {
                            all_slider[i].upgrade_text.text = car_upgrade_details.speed.cost[car_details.current_speed_level].ToString();
                        }
                        break;
                    }
                case 2:
                    {
                        //BRAKING
                        all_slider[i].slider.minValue = 200;
                        all_slider[i].slider.maxValue = 1800;


                        all_slider[i].min_value.text = "200";
                        all_slider[i].max_value.text = "1800";

                        all_slider[i].slider.value = car_upgrade_details.braking.amount[car_details.current_braking_level];

                        all_slider[i].upgradeBTN.gameObject.SetActive(isBought);



                        if (car_details.current_braking_level >= car_upgrade_details.braking.cost.Length)
                        {
                            all_slider[i].upgradeBTN.gameObject.SetActive(false);

                        }
                        else
                        {
                            all_slider[i].upgrade_text.text = car_upgrade_details.braking.cost[car_details.current_braking_level].ToString();
                        }
                        break;
                    }
                case 3:
                    {

                        //NITRO TIME
                        all_slider[i].slider.minValue = 0;
                        all_slider[i].slider.maxValue = 7;


                        all_slider[i].min_value.text = "0";
                        all_slider[i].max_value.text = "7";

                        all_slider[i].slider.value = car_upgrade_details.nitrus_time.amount[car_details.current_nitrus_level];

                        all_slider[i].upgradeBTN.gameObject.SetActive(isBought);



                        if (car_details.current_nitrus_level >= car_upgrade_details.nitrus_time.cost.Length)
                        {
                            all_slider[i].upgradeBTN.gameObject.SetActive(false);
                        }
                        else
                        {
                            all_slider[i].upgrade_text.text = car_upgrade_details.nitrus_time.cost[car_details.current_nitrus_level].ToString();
                        }

                        break;
                    }
                case 4:
                    {
                        //DRIFT CONTROL
                        all_slider[i].slider.minValue = 2;
                        all_slider[i].slider.maxValue = 4;


                        all_slider[i].min_value.text = "2";
                        all_slider[i].max_value.text = "4";

                        all_slider[i].slider.value = car_details.handling_amount;

                        all_slider[i].upgradeBTN.gameObject.SetActive(isBought);



                        if (car_details.current_handling_level >= car_upgrade_details.handling.cost.Length)
                        {
                            all_slider[i].upgradeBTN.gameObject.SetActive(false);
                        }
                        else
                        {
                            all_slider[i].upgrade_text.text = car_upgrade_details.handling.cost[car_details.current_handling_level].ToString();
                        }


                        break;
                    }
            }


            //weaponSystem.setWeaponStats();


        }
    }

    #endregion

    #region Car Selection / Buy Area
    private void PurchaseCar(int carIndex)
    {
        LocalData data = DatabaseManager.Instance.GetLocalData();
        if(data.coins< PlayerCarsInfo.Instance.carDefaultData[carIndex].carCost)
        {
            MessaeBox.insta.showMsg("Not Enough Coins!", true);
            return;
        }

        lastSelectedButton = carIndex;
        //car Index Starts from 0 and 0-is default car. So Buy index-1
        BlockchainManager.Instance.purchaseItem(carIndex-1,false);
    }

    private void SelectCar(int carIndex)
    {
        buy_select_btn.transform.GetChild(0).GetComponent<TMP_Text>().text = "Selected";
        LocalData data = DatabaseManager.Instance.GetLocalData();
        data.selected_car = carIndex + 1;
        DatabaseManager.Instance.UpdateData(data);
        if (MetaManager.Instance.myPlayerController)
        {
            MetaManager.Instance.myPlayerController.ChangeCar(carIndex + 1);
        }
        UIManager.Instance.ShowInformationMsg("Car Change", 1f);
        this.gameObject.SetActive(false);
    }
    #endregion
    public void SelectItem(int _no, Texture _texture)
    {
        Debug.Log("Selected item " + _no);
        currentSelectedItem = _no;
      
    }

    public void CloseItemPanel()
    { 
        gameObject.SetActive(false);
    }



    public void DeductCoins(int amount)
    {
        LocalData data = DatabaseManager.Instance.GetLocalData();
        data.coins -= amount;
        DatabaseManager.Instance.UpdateData(data);
        UIManager.Instance.SetCoinText();
    }




    private void Update()
    {
        if (rotating)
        {
            Vector3 delta = Input.mousePosition-prevPos;
            if (Vector3.Dot(carRotater.up, Vector3.up) >= 0)
            {
                carRotater.Rotate(carRotater.up, -Vector3.Dot(delta, carCameraOverlay.right), Space.World);
            }
            else
            {
                carRotater.Rotate(carRotater.up, Vector3.Dot(delta, carCameraOverlay.right), Space.World);
            }

            //carRotater.Rotate(carCameraOverlay.right, Vector3.Dot(delta, carCameraOverlay.up), Space.World);

            prevPos = Input.mousePosition;
        }
    }

    #region On Drag Events

    public Transform carRotater;
    public Transform carCameraOverlay;
    private bool rotating = false;
    Vector3 mousePos;
    Vector3 prevPos;
    public void BeginDrag()
    {
        prevPos = Input.mousePosition;
        rotating = true;
        
    }
    public void StopDrag()
    {
        rotating = false;
    }

    #endregion
}
