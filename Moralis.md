
## Moralis Unity SDK

* Authentication of user
* Authentication database
* All Smart contract call, run and execute interaction
* Moralis unit conversion tools

### Script :
https://github.com/MoraDefi/MetaRacer/blob/main/MetaGTA-RaceMode/Assets/Scripts/BlockchainManager.cs

``` C#
public async void getUserDataonStart()
    {

        var user = await Moralis.GetClient().GetCurrentUserAsync();
        if (user == null) return;

        PlayerPrefs.SetString("Account", user.ethAddress.ToLower());
        account = user.ethAddress.ToLower();

        SingletonDataManager.userethAdd = account;
        CheckUserBalance();

        GetTokenBalance();

        if (DatabaseManager.Instance)
        {
            DatabaseManager.Instance.GetData(true);
        }

        for (int i = 0; i < toDisableObjectsAfterLogin.Length; i++)
        {
            toDisableObjectsAfterLogin[i].SetActive(false);
        }
        loadingPanel.SetActive(true);

    }



    public void EnablePlayPanels()
    {
        for (int i = 0; i < toEnableObjectsAfterLogin.Length; i++)
        {
            toEnableObjectsAfterLogin[i].SetActive(true);
        }
        loadingPanel.SetActive(false);
    }




    #region BuyCoins
   

    public async UniTaskVoid CoinBuyOnSendContract(int index)
    {

        if (MessaeBox.insta) MessaeBox.insta.showMsg("Coin purchase process started\nThis can up to minute", false);
        string response = await PurchaseCoinsItemFromContract(index, coinCost[index]);
        if (!string.IsNullOrEmpty(response))
        {
            if (DatabaseManager.Instance)
            {
                DatabaseManager.Instance.AddTransaction(response, "pending", index);
            }
            if (DatabaseManager.Instance)
            {
                DatabaseManager.Instance.ChangeTransactionStatus(response, "success");
            }



            if (MessaeBox.insta) MessaeBox.insta.showMsg("Coin purchased successfully", true);

        }
        else
        {
            if (MessaeBox.insta) MessaeBox.insta.showMsg("Transaction Has Been Failed", true);
        }



    }
    private async Task<string> PurchaseCoinsItemFromContract(BigInteger tokenId, float _cost)
    {
        object[] parameters = {
            tokenId
        };

        // Set gas estimate
        HexBigInteger value = new HexBigInteger(UnitConversion.Convert.ToWei(_cost));
        HexBigInteger gas = new HexBigInteger(0);
        // HexBigInteger gas = new HexBigInteger(150000);

        //BigInteger _gascost = UnitConversion.Convert.ToWei(30, 9);
        //HexBigInteger gasPrice = new HexBigInteger(_gascost);
        HexBigInteger gasPrice = new HexBigInteger(0);

        Debug.Log("DataTRansfer buyCoins " + JsonConvert.SerializeObject(parameters));


        string resp = await Moralis.ExecuteContractFunction(contract, abi, "buyCoins", parameters, value, gas, gasPrice);



        if (resp != null && resp != "")
        {
            return resp;
        }

        return null;
    }


    #endregion

    #region NonBurnNFTBuy
    async public void NonBurnNFTBuyContract(int tokenId, string _uri)
    {

        if (MessaeBox.insta) MessaeBox.insta.showMsg("NFT Minting Process Started !", true);

        object[] parameters = {
            tokenId.ToString(),
            _uri
        };

        // Set gas estimate
        HexBigInteger value = new HexBigInteger(0);
        HexBigInteger gas = new HexBigInteger(0);
        HexBigInteger gasPrice = new HexBigInteger(0);

        Debug.Log("DataTRansfer " + JsonConvert.SerializeObject(parameters));


        string response = await Moralis.ExecuteContractFunction(contract, abi, "buyNonBurnItem", parameters, value, gas, gasPrice);

        if (string.IsNullOrEmpty(response))
        {
            if (MessaeBox.insta)
            {
                if (MessaeBox.insta) MessaeBox.insta.showMsg("Your Transaction has been failed", true);
            }
        }
        else
        {
            Debug.Log(response);


            if (MyNFTCollection.insta)
            {
                await CheckPuzzleList();

                //MyNFTCollection.insta.DeductCoins(DatabaseManager.Instance.allMetaDataServer[_no].cost);
                MyNFTCollection.insta.DisableLastSelectedButton();
                MyNFTCollection.insta.SetNewData();
            }

            if (MessaeBox.insta) MessaeBox.insta.showMsg("Your Transaction has been recieved\nIt will reflect to your account once it is completed!", true);

            if (!string.IsNullOrEmpty(response))
            {
                CheckUserBalance();
            }
        }
    }
    
    #endregion


    #region CheckTime
   


    public List<string> nftList = new List<string>();

    public async Task<string> CheckPuzzleList()
    {
        try
        {
            nftList = new List<string>();
            nftList.Clear();
            // Function ABI input parameters
            object[] inputParams = new object[1];
            inputParams[0] = new { internalType = "address", name = "_account", type = "address" };
            // Function ABI Output parameters
            object[] outputParams = new object[1];
            outputParams[0] = new { internalType = "string", name = "", type = "string" };
            // Function ABI
            object[] abi = new object[1];
            abi[0] = new { inputs = inputParams, name = "GetAllUserToken", outputs = outputParams, stateMutability = "view", type = "function" };

            // Define request object
            RunContractDto rcd = new RunContractDto()
            {
                Abi = abi,
                Params = new { _account = SingletonDataManager.userethAdd }
            };
            string response = await Moralis.Web3Api.Native.RunContractFunction<string>(contract, "GetAllUserToken", rcd, ContractChain);

            Debug.Log("GetNFTList " + response);

            string[] splitArray = response.Split(char.Parse(",")); //return one word for each string in the array
                                                                   //here, splitArray[0] = Give; splitArray[1] = me etc...

            for (int i = 0; i < splitArray.Length; i++)
            {
                if (string.IsNullOrEmpty(splitArray[i])) continue;
                nftList.Add(splitArray[i]);
            }

            return response;
        }
        catch (Exception e)
        {
            Debug.Log(e, this);
            return "";
        }

    }
    #endregion

   

    #region CheckUserBalance
   

    public async UniTaskVoid CheckUserBalance()
    {

        // get BSC native balance for a given address
        NativeBalance BSCbalance = await Moralis.Web3Api.Account.GetNativeBalance(SingletonDataManager.userethAdd.ToLower(), ContractChain);
        //Debug.Log(BSCbalance.ToJson());

        if (BSCbalance.Balance != null)
        {
            Debug.Log("Balance " + UnitConversion.Convert.FromWei(BigInteger.Parse(BSCbalance.Balance)));

            userBalance = (float)Math.Round((double)UnitConversion.Convert.FromWei(BigInteger.Parse(BSCbalance.Balance)), 4);
            if (InAppManager.Instance)
            {
                InAppManager.Instance.SetBalanceText();
            }
        }

    }
    #endregion

  
 
    #region NFTUploaded

    public void purchaseItem(int _id, bool _skin)
    {
        if (!_skin) NonBurnNFTBuyContract(_id, "myNFT");
        Debug.Log("purchaseItem");
    }


    #endregion


    #region TokenFunctions

    public async void GetTokenReward()
    {
        if (MessaeBox.insta) MessaeBox.insta.showMsg("Token redeem process started", false);
        object[] parameters = {
        };

        // Set gas estimate
        HexBigInteger value = new HexBigInteger(0);
        HexBigInteger gas = new HexBigInteger(0);
        HexBigInteger gasPrice = new HexBigInteger(0);

        Debug.Log("DataTRansfer buyCoins " + JsonConvert.SerializeObject(parameters));


        string resp = await Moralis.ExecuteContractFunction(contractToken, abiToken, "GetGameToken", parameters, value, gas, gasPrice);

        if (!string.IsNullOrEmpty(resp))
        {
            if (MessaeBox.insta) MessaeBox.insta.showMsg("Token redeemed successfully", true);
            UIManager.Instance.SetTokenBalanceText();
        }
        else
        {
            if (MessaeBox.insta) MessaeBox.insta.showMsg("Transaction Has Been Failed", true);
        }

        //UIManager.Instance.tokenButton.SetActive(false);
    }

    public async UniTaskVoid ExchangeTokenUI(int index)
    {
        if (MessaeBox.insta) MessaeBox.insta.showMsg("Coin exchange process started", false);
        string response = await ExchangeToken(index);

        if (!string.IsNullOrEmpty(response))
        {
            if (DatabaseManager.Instance)
            {
                DatabaseManager.Instance.AddTransaction(response, "pending", index-1);
            }
            if (DatabaseManager.Instance)
            {
                DatabaseManager.Instance.ChangeTransactionStatus(response, "success");
            }



            if (MessaeBox.insta) MessaeBox.insta.showMsg("Coin exchanged successfully", true);

        }
        else
        {
            if (MessaeBox.insta) MessaeBox.insta.showMsg("Transaction Has Been Failed", true);
        }


    }

    async Task<string> ExchangeToken(int packID)
    {
        string _amount = UnitConversion.Convert.ToWei(packID, 18).ToString();
        object[] parameters = {
            contractToken,
            _amount
        };

        // Set gas estimate
        HexBigInteger value = new HexBigInteger(0);
        HexBigInteger gas = new HexBigInteger(0);
        HexBigInteger gasPrice = new HexBigInteger(0);

        Debug.Log("DataTRansfer buyCoins " + JsonConvert.SerializeObject(parameters));


        string resp = await Moralis.ExecuteContractFunction(contractToken, abiToken, "transfer", parameters, value, gas, gasPrice);



        if (resp != null && resp != "")
        {
            return resp;
        }

        return null;
    }




    public async UniTaskVoid GetTokenBalance()
    {
        COMEHERE:
        // Function ABI input parameters
        object[] inputParams = new object[1];
        inputParams[0] = new { internalType = "address", name = "account", type = "address" };
        // Function ABI Output parameters
        object[] outputParams = new object[1];
        outputParams[0] = new { internalType = "uint256", name = "", type = "uint256" };
        // Function ABI
        object[] abiThis = new object[1];
        abiThis[0] = new { inputs = inputParams, name = "balanceOf", outputs = outputParams, stateMutability = "view", type = "function" };
        // Define request object
        RunContractDto rcd = new RunContractDto()
        {
            Abi = abiThis,
            Params = new { account = SingletonDataManager.userethAdd }
        };
        string resp = await Moralis.Web3Api.Native.RunContractFunction<string>(contractToken, "balanceOf", rcd, ContractChain);
        //Debug.Log("GetTokenBalance " + resp);

        if (!string.IsNullOrEmpty(resp))
            SingletonDataManager.userTokenBalance = Math.Round((double)UnitConversion.Convert.FromWei(BigInteger.Parse(resp)), 4).ToString();

        
        if (UIManager.Instance) UIManager.Instance.SetTokenBalanceText();
        Debug.Log("GetTokenBalance " + SingletonDataManager.userTokenBalance);

        await UniTask.Delay(UnityEngine.Random.Range(5000, 10000));
        goto COMEHERE;

    }
    #endregion
```
