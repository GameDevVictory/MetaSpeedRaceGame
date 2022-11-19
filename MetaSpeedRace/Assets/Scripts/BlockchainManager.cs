using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class BlockchainManager : MonoBehaviour
{
    #region Singleton
    public static BlockchainManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    #endregion

    public static string abi = "[{\"inputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"ApprovalForAll\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_itemId\",\"type\":\"uint256\"}],\"name\":\"buyCoins\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_tokenId\",\"type\":\"uint256\"},{\"internalType\":\"string\",\"name\":\"_tokenUrl\",\"type\":\"string\"}],\"name\":\"buyNonBurnItem\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"previousOwner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"OwnershipTransferred\",\"type\":\"event\"},{\"inputs\":[],\"name\":\"renounceOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256[]\",\"name\":\"ids\",\"type\":\"uint256[]\"},{\"internalType\":\"uint256[]\",\"name\":\"amounts\",\"type\":\"uint256[]\"},{\"internalType\":\"bytes\",\"name\":\"data\",\"type\":\"bytes\"}],\"name\":\"safeBatchTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"},{\"internalType\":\"bytes\",\"name\":\"data\",\"type\":\"bytes\"}],\"name\":\"safeTransferFrom\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"internalType\":\"bool\",\"name\":\"approved\",\"type\":\"bool\"}],\"name\":\"setApprovalForAll\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256[]\",\"name\":\"ids\",\"type\":\"uint256[]\"},{\"indexed\":false,\"internalType\":\"uint256[]\",\"name\":\"values\",\"type\":\"uint256[]\"}],\"name\":\"TransferBatch\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"newOwner\",\"type\":\"address\"}],\"name\":\"transferOwnership\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"TransferSingle\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":false,\"internalType\":\"string\",\"name\":\"value\",\"type\":\"string\"},{\"indexed\":true,\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"}],\"name\":\"URI\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_recipient\",\"type\":\"address\"}],\"name\":\"withdraw\",\"outputs\":[],\"stateMutability\":\"payable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"}],\"name\":\"balanceOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address[]\",\"name\":\"accounts\",\"type\":\"address[]\"},{\"internalType\":\"uint256[]\",\"name\":\"ids\",\"type\":\"uint256[]\"}],\"name\":\"balanceOfBatch\",\"outputs\":[{\"internalType\":\"uint256[]\",\"name\":\"\",\"type\":\"uint256[]\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_add\",\"type\":\"address\"}],\"name\":\"GetAllUserToken\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"getCurrentTime\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"_result\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"operator\",\"type\":\"address\"}],\"name\":\"isApprovedForAll\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"name\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"bytes4\",\"name\":\"interfaceId\",\"type\":\"bytes4\"}],\"name\":\"supportsInterface\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"id\",\"type\":\"uint256\"}],\"name\":\"uri\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"}]";

    public static string abiToken = "[{\"inputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"constructor\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"Approval\",\"type\":\"event\"},{\"anonymous\":false,\"inputs\":[{\"indexed\":true,\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"indexed\":true,\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"indexed\":false,\"internalType\":\"uint256\",\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"Transfer\",\"type\":\"event\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"_amount\",\"type\":\"uint256\"}],\"name\":\"ExchangeToken\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"GetCurrentTime\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"_result\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"GetGameToken\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"GetSmartContractBalance\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"_account\",\"type\":\"address\"}],\"name\":\"GetuserBalance\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"owner\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"}],\"name\":\"allowance\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"approve\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"account\",\"type\":\"address\"}],\"name\":\"balanceOf\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"decimals\",\"outputs\":[{\"internalType\":\"uint8\",\"name\":\"\",\"type\":\"uint8\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"subtractedValue\",\"type\":\"uint256\"}],\"name\":\"decreaseAllowance\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"spender\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"addedValue\",\"type\":\"uint256\"}],\"name\":\"increaseAllowance\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"value\",\"type\":\"uint256\"}],\"name\":\"mint\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"name\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"owner\",\"outputs\":[{\"internalType\":\"address\",\"name\":\"\",\"type\":\"address\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"symbol\",\"outputs\":[{\"internalType\":\"string\",\"name\":\"\",\"type\":\"string\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[],\"name\":\"totalSupply\",\"outputs\":[{\"internalType\":\"uint256\",\"name\":\"\",\"type\":\"uint256\"}],\"stateMutability\":\"view\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"transfer\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"address\",\"name\":\"from\",\"type\":\"address\"},{\"internalType\":\"address\",\"name\":\"to\",\"type\":\"address\"},{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"transferFrom\",\"outputs\":[{\"internalType\":\"bool\",\"name\":\"\",\"type\":\"bool\"}],\"stateMutability\":\"nonpayable\",\"type\":\"function\"},{\"inputs\":[{\"internalType\":\"uint256\",\"name\":\"amount\",\"type\":\"uint256\"}],\"name\":\"withdraw\",\"outputs\":[],\"stateMutability\":\"nonpayable\",\"type\":\"function\"}]";


    // address of contract
    public static string contract = "0x2B992108c2464A10749656671710a65a828739ED";

    public static string contractToken = "0xe538B088711a6A7e9C3f97933F4622ebD7856287";



    public float[] coinCost = { 0.00015f, 0.00030f, 0.00045f, 0.1f, 0.00060f };

    int[] coinGive = { 1000, 2000, 3500, 6000 };

    public static string userBalance = "0";

    [DllImport("__Internal")]
    private static extern void Web3Connect();

    [DllImport("__Internal")]
    private static extern string ConnectAccount();

    [DllImport("__Internal")]
    private static extern void SetConnectAccount(string value);

    private int expirationTime;
    private string account;

    [SerializeField] TMP_Text _status;
    [SerializeField] GameObject loadingPanel;
    [SerializeField] GameObject[] toEnableObjectsAfterLogin;
    [SerializeField] GameObject[] toDisableObjectsAfterLogin;

    [SerializeField] GameObject authenticationObj;
    [SerializeField] GameObject chainSelectionObj;

    string chainId = "1313161555";
    string networkRPC = "https://testnet.aurora.dev/";
    //moralis stuff


    private void Start()
    {
        //authenticationObj.SetActive(false);
        chainSelectionObj.SetActive(true);
    }

    public async void LoginWallet()
    {
        _status.text = "Connecting...";
#if !UNITY_EDITOR
        Web3Connect();
        OnConnected();
#else
        // get current timestamp
        int timestamp = (int)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds;
        // set expiration time
        int expirationTime = timestamp + 60;
        // set message
        string message = expirationTime.ToString();
        // sign message
        string signature = await Web3Wallet.Sign(message);
        // verify account
        string account = await EVM.Verify(message, signature);
        int now = (int)(System.DateTime.UtcNow.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds;
        // validate
        if (account.Length == 42 && expirationTime >= now)
        {
            // save account
            PlayerPrefs.SetString("Account", account);
            PlayerPrefs.Save();
            print("Account: " + account);
            _status.text = "connected : " + account;

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

#endif

    }


    async private void OnConnected()
    {
        account = ConnectAccount();
        while (account == "")
        {
            await new WaitForSecondsRealtime(2f);
            account = ConnectAccount();
        };
        account = account.ToLower();
        // save account for next scene
        PlayerPrefs.SetString("Account", account);
        PlayerPrefs.Save();
        _status.text = "connected : " + account;
        // reset login message
        SetConnectAccount("");


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

    public void SelectChain(int _no)
    {
        /*switch (_no)
        {
            case 0: //Fantom Mainnet
                contract = "0x2341B6A39c384139AE77EA05a6065368070226Ee";
                contractToken = "0x01eB9e4fF20886C691e9657f4Ce823B8d0C4Ab74";
                CovalentManager.chainID = "250";
                break;
            case 1://Polygone Testnet
                contract = "0x7442Af11345FB8133eeC9307A06b4C337225279c";
                CovalentManager.chainID = "80001";
                break;
            case 2://Binance Testnet
                contract = "0xC39170b2f01e7aaD707D605754dA13d1b3361D3E";
                contractToken = "0xb92016321dab5622206f112E329A710155E5bD3C";
                CovalentManager.chainID = "97";
                break;
        }*/
        chainSelectionObj.SetActive(false);
        LoginWallet();
        //authenticationObj.SetActive(true);
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


    public async UniTaskVoid CoinBuyOnSendContract(int _pack)
    {

        object[] inputParams = { _pack };

        float _amount = coinCost[_pack];
        float decimals = 1000000000000000000; // 18 decimals
        float wei = _amount * decimals;
        print(Convert.ToDecimal(wei).ToString() + " " + inputParams.ToString() + " + " + Newtonsoft.Json.JsonConvert.SerializeObject(inputParams));
        // smart contract method to call
        string method = "buyCoins";

        // array of arguments for contract
        string args = Newtonsoft.Json.JsonConvert.SerializeObject(inputParams);
        // value in wei
        string value = Convert.ToDecimal(wei).ToString();
        // gas limit OPTIONAL
        string gasLimit = "";
        // gas price OPTIONAL
        string gasPrice = "";
        // connects to user's browser wallet (metamask) to update contract state
        try
        {


#if !UNITY_EDITOR
            string response = await Web3GL.SendContract(method, abi, contract, args, value, gasLimit, gasPrice);
            Debug.Log(response);
#else
            // string response = await EVM.c(method, abi, contract, args, value, gasLimit, gasPrice);
            // Debug.Log(response);
            string data = await EVM.CreateContractData(abi, method, args);
            string response = await Web3Wallet.SendTransaction(chainId, contract, value, data, gasLimit, gasPrice);


            Debug.Log(response);
#endif

            if (!string.IsNullOrEmpty(response))
            {
                if (DatabaseManager.Instance)
                {
                    DatabaseManager.Instance.AddTransaction(response, "pending", _pack);
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
        catch (Exception e)
        {
            if (MessaeBox.insta) MessaeBox.insta.showMsg("Transaction Has Been Failed", true);
            Debug.Log(e, this);
        }

    }



    #endregion

    #region NonBurnNFTBuy
    async public void NonBurnNFTBuyContract(int tokenId, string _uri)
    {

        if (MessaeBox.insta) MessaeBox.insta.showMsg("NFT Minting Process Started !", true);


        Debug.Log("Non Burn NFT Buy  " + tokenId + "URI : " + _uri);

        object[] inputParams = { tokenId.ToString(), _uri };

        string method = "buyNonBurnItem"; // buyBurnItem";// "buyCoins";

        // array of arguments for contract
        string args = Newtonsoft.Json.JsonConvert.SerializeObject(inputParams);
        // value in wei
        string value = "";// Convert.ToDecimal(wei).ToString();
        // gas limit OPTIONAL
        string gasLimit = "";
        // gas price OPTIONAL
        string gasPrice = "";
        // connects to user's browser wallet (metamask) to update contract state
        try
        {

#if !UNITY_EDITOR
                string response = await Web3GL.SendContract(method, abi, contract, args, value, gasLimit, gasPrice);
                Debug.Log(response);
#else
            //string response = await EVM.Call(chain, network, contract, abi, args, method, args);
            //Debug.Log(response);
            string data = await EVM.CreateContractData(abi, method, args);
            string response = await Web3Wallet.SendTransaction(chainId, contract, "0", data, gasLimit, gasPrice);
            Debug.Log(response);

#endif

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
        catch (Exception e)
        {
            Debug.Log(e, this);
            if (MessaeBox.insta)
            {

                MessaeBox.insta.showMsg("Server Error", true);

            }

        }
    }

    #endregion


    #region CheckTime



    public List<string> nftList = new List<string>();

    public async Task<string> CheckPuzzleList()
    {
        // smart contract method to call
        nftList = new List<string>();
        nftList.Clear();
        string method = "GetAllUserToken";
        // array of arguments for contract
        object[] inputParams = { PlayerPrefs.GetString("Account") };
        string args = Newtonsoft.Json.JsonConvert.SerializeObject(inputParams);
        Debug.Log("CheckPuzzleList ===================");
        try
        {
            string response = await EVM.Call("", "", contract, abi, method, args, networkRPC);
            Debug.Log("CheckPuzzleList =================== Now");
            Debug.Log(response);
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
        try
        {

            string response = await EVM.BalanceOf("", "", PlayerPrefs.GetString("Account"), networkRPC);
            if (!string.IsNullOrEmpty(response))
            {
                float wei = float.Parse(response);
                float decimals = 1000000000000000000; // 18 decimals
                float eth = wei / decimals;
                // print(Convert.ToDecimal(eth).ToString());
                Debug.Log(Convert.ToDecimal(eth).ToString());
                userBalance = Convert.ToDecimal(eth).ToString();
                if (InAppManager.Instance)
                {
                    InAppManager.Instance.SetBalanceText();
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e, this);
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

        //UIManager.Instance.tokenButton.SetActive(false); new fom here

        object[] inputParams = { };
        string method = "GetGameToken"; // buyBurnItem";// "buyCoins";

        // array of arguments for contract
        string args = Newtonsoft.Json.JsonConvert.SerializeObject(inputParams);
        // value in wei
        string value = "";// Convert.ToDecimal(wei).ToString();
        // gas limit OPTIONAL
        string gasLimit = "";
        // gas price OPTIONAL
        string gasPrice = "";
        string response = "";
        // connects to user's browser wallet (metamask) to update contract state
        try
        {

#if !UNITY_EDITOR
                response = await Web3GL.SendContract(method, abiToken, contractToken, args, value, gasLimit, gasPrice);
                Debug.Log(response);
#else
            string data = await EVM.CreateContractData(abiToken, method, args);
            response = await Web3Wallet.SendTransaction(chainId, contractToken, "0", data, gasLimit, gasPrice);
            Debug.Log(response);
#endif

            if (!string.IsNullOrEmpty(response))
            {
                if (MessaeBox.insta) MessaeBox.insta.showMsg("Token redeemed successfully, It will be credited soon", true);
                UIManager.Instance.SetTokenBalanceText();
            }
            else
            {
                if (MessaeBox.insta) MessaeBox.insta.showMsg("Transaction Has Been Failed", true);
            }

        }
        catch (Exception e)
        {
            Debug.Log("error" + e);
            if (MessaeBox.insta) MessaeBox.insta.showMsg("Server Error", true);
            return;
        }

    }

    public async UniTaskVoid ExchangeTokenUI(int index)
    {
        if (MessaeBox.insta) MessaeBox.insta.showMsg("Coin exchange process started", false);
        /////////// new start here
        float decimals = 1000000000000000000; // 18 decimals
        float wei = (index) * decimals;

        object[] inputParams = { contractToken, Convert.ToDecimal(wei).ToString() };


        // smart contract method to call
        string method = "transfer";

        // array of arguments for contract
        string args = Newtonsoft.Json.JsonConvert.SerializeObject(inputParams);
        // value in wei
        string value = "0";
        // gas limit OPTIONAL
        string gasLimit = "";
        // gas price OPTIONAL
        string gasPrice = "";
        // connects to user's browser wallet (metamask) to update contract state
        try
        {


#if !UNITY_EDITOR
            string response = await Web3GL.SendContract(method, abiToken, contractToken, args, value, gasLimit, gasPrice);
            Debug.Log(response);
#else
            // string response = await EVM.c(method, abi, contract, args, value, gasLimit, gasPrice);
            // Debug.Log(response);
            string data = await EVM.CreateContractData(abiToken, method, args);
            string response = await Web3Wallet.SendTransaction(chainId, contractToken, value, data, gasLimit, gasPrice);


            Debug.Log(response);
#endif

            if (!string.IsNullOrEmpty(response))
            {
                if (DatabaseManager.Instance)
                {
                    DatabaseManager.Instance.AddTransaction(response, "pending", index - 1);
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
        catch (Exception e)
        {
            if (MessaeBox.insta) MessaeBox.insta.showMsg("Transaction Has Been Failed", true);
            Debug.Log(e, this);
        }

    }



    public async UniTaskVoid GetTokenBalance()
    {
        HERE:
        // smart contract method to call
        string method = "balanceOf";
        // array of arguments for contract
        object[] inputParams = { PlayerPrefs.GetString("Account") };
        string args = Newtonsoft.Json.JsonConvert.SerializeObject(inputParams);
        try
        {
            string response = await EVM.Call("", "", contractToken, abiToken, method, args, networkRPC);
            Debug.Log(response);
            try
            {
                float wei = float.Parse(response);
                float decimals = 1000000000000000000; // 18 decimals
                float eth = wei / decimals;
                // print(Convert.ToDecimal(eth).ToString());
                var tokenBalance = Convert.ToDecimal(eth).ToString();
                SingletonDataManager.userTokenBalance = tokenBalance;
                Debug.Log("Token Bal : " + Convert.ToDecimal(eth).ToString() + " | " + response);

                if (UIManager.Instance) { UIManager.Instance.SetTokenBalanceText(); }
                //if (StoreManager.insta) StoreManager.insta.UpdateBalance();
            }
            catch (Exception)
            {
            }


        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
        await UniTask.Delay(6000, true);
        goto HERE;

    }
    #endregion

}
