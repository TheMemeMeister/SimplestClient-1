using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameSystemManager : MonoBehaviour
{
    GameObject btnSubmit, txtUserId, txtPwd, chkCreate, btnJoin, lblU, lblP, lblInfo, gameBoard, txtMsg, btnSend, ddlMsg, chatBox, pnlChat, btnSendPrefixMsg, btnJoinObserver, btnReplay, ddlPlayer;
    GameObject txtCMsg, btnCSend, LoginSys, MsgSend, PMsgSend, C2C, JoinSys, txtReplay, pnlReplay, chk2player;
    GameObject btn11, btn12, btn13, btn21, btn22, btn23, btn31, btn32, btn33;
    public GameObject networkedClient;
    string currentPlayerName = "";
    public int playerNumber = 0;
    bool isPlayer = false;
    List<string> preFixMsg = new List<string> { "hello", "test", "bye", "call you later" };
    // Start is called before the first frame update
    void Start()
    {
        GameObject[] allobjects = FindObjectsOfType<GameObject>();
        foreach (GameObject go in allobjects)
        {
            if (go.name == "btnJoin")
            {
                btnJoin = go;
            }
            else if (go.name == "lblInfo")
            {
                lblInfo = go;
            }
            else if (go.name == "btnLogin")
            {
                btnSubmit = go;
            }
            else if (go.name == "txtUser")
            {
                txtUserId = go;
            }
            else if (go.name == "txtPwd")
            {
                txtPwd = go;
            }
            else if (go.name == "chkCreate")
            {
                chkCreate = go;
            }
            else if (go.name == "lblUser")
            {
                lblU = go;
            }
            else if (go.name == "lblPwd")
            {
                lblP = go;
            }
            else if (go.name == "btnReplay")
            {
                btnReplay = go;
            }
            else if (go.name == "txtReplay")
            {
                txtReplay = go;
            }
            else if (go.name == "pnlReplay")
            {
                pnlReplay = go;
            }
            else if (go.name == "txtMsg")
                txtMsg = go;
            else if (go.name == "btnSend")
                btnSend = go;
            else if (go.name == "ddlMsg")
                ddlMsg = go;
            else if (go.name == "chatBox")
                chatBox = go;
            else if (go.name == "pnlChat")
                pnlChat = go;
            else if (go.name == "btnSendPrefixMsg")
                btnSendPrefixMsg = go;
            else if (go.name == "btnJoinObserver")
                btnJoinObserver = go;
            else if (go.name == "ddlPlayer")
            {
                ddlPlayer = go;
            }
            else if (go.name == "PMsgSend")
            {
                PMsgSend = go;
            }
            else if (go.name == "JoinSys")
            {
                JoinSys = go;
            }
            else if (go.name == "C2C")
            {
                C2C = go;
            }
            else if (go.name == "MsgSend")
            {
                MsgSend = go;
            }
            else if (go.name == "LoginSys")
            {
                LoginSys = go;
            }
            else if (go.name == "btnCSend")
            {
                btnCSend = go;
            }
            else if (go.name == "txtCMsg")
            {
                txtCMsg = go;
            }
            else if (go.name == "chk2player")
            {
                chk2player = go;
            }
            else if (go.name == "btn11")
            {
                btn11 = go;
            }
            else if (go.name == "btn12")
            {
                btn12 = go;
            }
            else if (go.name == "btn13")
            {
                btn13 = go;
            }
            else if (go.name == "btn21")
            {
                btn21 = go;
            }
            else if (go.name == "btn22")
            {
                btn22 = go;
            }
            else if (go.name == "btn23")
            {
                btn23 = go;
            }
            else if (go.name == "btn31")
            {
                btn31 = go;
            }
            else if (go.name == "btn32")
            {
                btn32 = go;
            }
            else if (go.name == "btn33")
            {
                btn33 = go;
            }
        }
        btn11.GetComponent<Button>().onClick.AddListener(btn11Click);
        btn21.GetComponent<Button>().onClick.AddListener(btn21Click);
        btn31.GetComponent<Button>().onClick.AddListener(btn31Click);
        btn12.GetComponent<Button>().onClick.AddListener(btn12Click);
        btn22.GetComponent<Button>().onClick.AddListener(btn22Click);
        btn32.GetComponent<Button>().onClick.AddListener(btn32Click);
        btn13.GetComponent<Button>().onClick.AddListener(btn13Click);
        btn23.GetComponent<Button>().onClick.AddListener(btn23Click);
        btn33.GetComponent<Button>().onClick.AddListener(btn33Click);
        btnSubmit.GetComponent<Button>().onClick.AddListener(SubmitButtonPressed);
        btnJoin.GetComponent<Button>().onClick.AddListener(JoinButtonPressed);
        btnJoinObserver.GetComponent<Button>().onClick.AddListener(ObserveButtonPressed);
        btnReplay.GetComponent<Button>().onClick.AddListener(ReplayButtonPressed);
        btnSend.GetComponent<Button>().onClick.AddListener(SendButtonPressed);
        btnSendPrefixMsg.GetComponent<Button>().onClick.AddListener(SendPrefButtonPressed);
        btnCSend.GetComponent<Button>().onClick.AddListener(SendClientButtonPressed);
        chkCreate.GetComponent<Toggle>().onValueChanged.AddListener(CreateToggleChanged);

        ChangeState(GameStates.LoginMenu);

        ddlMsg.GetComponent<Dropdown>().AddOptions(preFixMsg);


    }
    bool change = false;
    public void gmPlay(int btn, int player)
    {
        string choice = "";
        int cell = 0;
        switch (btn)
        {
            case 11:
                choice = filledValue(player, btn11.GetComponentInChildren<Text>().text);
                btn11.GetComponentInChildren<Text>().text = choice;
                cell = 1;
                break;
            case 12:
                choice = filledValue(player, btn12.GetComponentInChildren<Text>().text);
                btn12.GetComponentInChildren<Text>().text = choice;
                cell = 2;
                break;
            case 13:
                choice = filledValue(player, btn13.GetComponentInChildren<Text>().text);
                btn13.GetComponentInChildren<Text>().text = choice;
                cell = 3;
                break;
            case 21:
                choice = filledValue(player, btn21.GetComponentInChildren<Text>().text);
                btn21.GetComponentInChildren<Text>().text = choice;
                cell = 4;
                break;
            case 22:
                choice = filledValue(player, btn22.GetComponentInChildren<Text>().text);
                btn22.GetComponentInChildren<Text>().text = choice;
                cell = 5;
                break;
            case 23:
                choice = filledValue(player, btn23.GetComponentInChildren<Text>().text);
                btn23.GetComponentInChildren<Text>().text = choice;
                cell = 6;
                break;
            case 31:
                choice = filledValue(player, btn31.GetComponentInChildren<Text>().text);
                btn31.GetComponentInChildren<Text>().text = choice;
                cell = 7;
                break;
            case 32:
                choice = filledValue(player, btn32.GetComponentInChildren<Text>().text);
                btn32.GetComponentInChildren<Text>().text = choice;
                cell = 8;
                break;
            case 33:
                choice = filledValue(player, btn33.GetComponentInChildren<Text>().text);
                btn33.GetComponentInChildren<Text>().text = choice;
                cell = 9;
                break;
            default:
                break;
        }

        string msg = ClientToServerSignifiers.PlayGame + "," + currentPlayerName + "," + choice + "," + cell;
        Debug.Log("play " + msg);
        if (change)
            networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(msg);

    }
    public void setTextButton(int btn, string val)
    {
        switch (btn)
        {
            case 1:
                btn11.GetComponentInChildren<Text>().text = val;
                break;
            case 2:
                btn12.GetComponentInChildren<Text>().text = val;
                break;
            case 3:
                btn13.GetComponentInChildren<Text>().text = val;
                break;
            case 4:
                btn21.GetComponentInChildren<Text>().text = val;
                break;
            case 5:
                btn22.GetComponentInChildren<Text>().text = val;
                break;
            case 6:
                btn23.GetComponentInChildren<Text>().text = val;
                break;
            case 7:
                btn31.GetComponentInChildren<Text>().text = val;
                break;
            case 8:
                btn32.GetComponentInChildren<Text>().text = val;
                break;
            case 9:
                btn33.GetComponentInChildren<Text>().text = val;
                break;
            default:
                break;
        }

    }
    public string filledValue(int player, string prevVal)
    {
        string newVal = "";
        if (prevVal == "_")//empty box
        {
            change = true;
            if (player == 1)
                newVal = "X";
            else
                newVal = "O";
        }
        else
        {
            newVal = prevVal;//can't play
            change = false;
        }
        return newVal;
    }
    public void btn11Click()
    {
        gmPlay(11, playerNumber);
    }
    public void btn12Click()
    {
        gmPlay(12, playerNumber);
    }
    public void btn13Click()
    {
        gmPlay(13, playerNumber);
    }
    public void btn21Click()
    {
        gmPlay(21, playerNumber);
    }
    public void btn22Click()
    {
        gmPlay(22, playerNumber);
    }
    public void btn23Click()
    {
        gmPlay(23, playerNumber);
    }
    public void btn31Click()
    {
        gmPlay(31, playerNumber);
    }
    public void btn32Click()
    {
        gmPlay(32, playerNumber);
    }
    public void btn33Click()
    {
        gmPlay(33, playerNumber);
    }
    public bool getchk2player()
    {
        Debug.Log("chk " + chkCreate.GetComponent<Toggle>().isOn);
        return chkCreate.GetComponent<Toggle>().isOn;
    }
    public bool getIsPlayer()
    {
        return isPlayer;
    }
    public void updateChat(string msg)
    {
        chatBox.GetComponent<TMP_Text>().text += msg + "\n";
    }
    public void updateReplay(string msg)
    {
        txtReplay.GetComponent<TMP_Text>().text += msg + "\n";
    }
    public void updateUserName(string name)
    {
        currentPlayerName = name;
        lblInfo.GetComponent<Text>().text = "Logged in user: " + name;
    }
    public void LoadPlayer(List<string> list)
    {
        ddlPlayer.GetComponent<Dropdown>().ClearOptions();
        foreach (string it in list)
        {
            if (it.Contains(currentPlayerName))
            {
                list.Remove(it);
                break;
            }
        }
        ddlPlayer.GetComponent<Dropdown>().AddOptions(list);

    }
    public void ReplayButtonPressed()
    {
        string msg = ClientToServerSignifiers.ReplayMsg + "," + currentPlayerName;
        Debug.Log("replay " + msg);
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(msg);

    }
    // Update is called once per frame
    void Update()
    {

    }
    public void SendClientButtonPressed()
    {
        string msg = ClientToServerSignifiers.SendClientMsg + "," + ddlPlayer.GetComponent<Dropdown>().options[ddlPlayer.GetComponent<Dropdown>().value].text.ToString() + "," + txtCMsg.GetComponent<InputField>().text + "," + currentPlayerName;
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(msg);
        Debug.Log("client send " + msg);
    }
    public void SendPrefButtonPressed()
    {
        string msg = ClientToServerSignifiers.SendPrefixMsg + "," + ddlMsg.GetComponent<Dropdown>().options[ddlMsg.GetComponent<Dropdown>().value].text.ToString() + "," + currentPlayerName;
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(msg);
        Debug.Log("sendpre " + msg);
    }
    public void SendButtonPressed()
    {
        string msg = ClientToServerSignifiers.SendMsg + "," + txtMsg.GetComponent<InputField>().text + "," + currentPlayerName;
        Debug.Log("msg:" + msg);
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(msg);
        Debug.Log("send " + msg);
    }
    public void SubmitButtonPressed()
    {
        Debug.Log("button");
        string p = txtPwd.GetComponent<InputField>().text;
        string n = txtUserId.GetComponent<InputField>().text;
        string msg;
        if (chkCreate.GetComponent<Toggle>().isOn)
            msg = ClientToServerSignifiers.CreateAccount + "," + n + "," + p;
        else
            msg = ClientToServerSignifiers.Login + "," + n + "," + p;
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(msg);
    }
    public void JoinButtonPressed()
    {
        //Debug.Log("join ");
        string msg = "";
        if (chk2player.GetComponent<Toggle>().isOn)
        {
            msg = ClientToServerSignifiers.JoinDGameRoomQueue + "," + currentPlayerName;
        }
        else
        {
            msg = ClientToServerSignifiers.JoinChatRoomQueue + "," + currentPlayerName;
        }

        isPlayer = true;
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(msg);
        //  ChangeState(GameStates.TicTacToe);
    }
    public void ObserveButtonPressed()
    {
        string msg = "";
        if (chk2player.GetComponent<Toggle>().isOn)
            msg = ClientToServerSignifiers.JoinDAsObserver + "," + currentPlayerName;
        else
            msg = ClientToServerSignifiers.JoinAsObserver + "," + currentPlayerName;
        isPlayer = false;
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(msg);
        //ChangeState(GameStates.Observer);
    }
    public void CreateToggleChanged(bool newValue)
    {

    }
    public void PlayButtonPressed()
    {
        string msg = ClientToServerSignifiers.PlayGame + "";
        networkedClient.GetComponent<NetworkedClient>().SendMessageToHost(msg);
        //load tictactoe
        ChangeState(GameStates.TickTacToePlay);
    }
    public void ChangeState(int newState)
    {
        //LoginSys.SetActive(false);
        //JoinSys.SetActive(false);
        btnJoin.SetActive(false);
        btnJoinObserver.SetActive(false);
        btnSubmit.SetActive(false);
        chkCreate.SetActive(false);
        txtPwd.SetActive(false);
        txtUserId.SetActive(false);
        lblU.SetActive(false);
        lblP.SetActive(false);
        //btnPlay.SetActive(false);
        //txtMsg, btnSend, ddlMsg, chatBox, btnSendPrefixMsg
        //MsgSend.SetActive(false);
        txtMsg.SetActive(false);
        btnSend.SetActive(false);
        //PMsgSend.SetActive(false);
        ddlMsg.SetActive(false);
        btnSendPrefixMsg.SetActive(false);
        //C2C.SetActive(false);
        ddlPlayer.SetActive(false);
        btnCSend.SetActive(false);
        txtCMsg.SetActive(false);
        chatBox.SetActive(false);
        pnlChat.SetActive(false);
        txtReplay.SetActive(false);
        btnReplay.SetActive(false);
        pnlReplay.SetActive(false);
        lblInfo.SetActive(false);
        chk2player.SetActive(false);
        btn11.SetActive(false);
        btn12.SetActive(false);
        btn13.SetActive(false);
        btn21.SetActive(false);
        btn22.SetActive(false);
        btn23.SetActive(false);
        btn31.SetActive(false);
        btn32.SetActive(false);
        btn33.SetActive(false);

        if (newState == GameStates.LoginMenu)
        {
            // LoginSys.SetActive(true);
            btnSubmit.SetActive(true);
            chkCreate.SetActive(true);
            txtPwd.SetActive(true);
            txtUserId.SetActive(true);
            lblU.SetActive(true);
            lblP.SetActive(true);
        }
        else if (newState == GameStates.MainMenu)
        {
            lblInfo.SetActive(true);
            //JoinSys.SetActive(true);
            btnJoin.SetActive(true);
            btnJoinObserver.SetActive(true);
            //MsgSend.SetActive(true);
            txtMsg.SetActive(true);
            btnSend.SetActive(true);
            //PMsgSend.SetActive(true);
            ddlMsg.SetActive(true);
            btnSendPrefixMsg.SetActive(true);
            //C2C.SetActive(true);
            ddlPlayer.SetActive(true);
            btnCSend.SetActive(true);
            txtCMsg.SetActive(true);
            chatBox.SetActive(true);
            pnlChat.SetActive(true);

            txtReplay.SetActive(true);
            btnReplay.SetActive(true);
            pnlReplay.SetActive(true);
            chk2player.SetActive(true);
        }
        else if (newState == GameStates.WaitingInQueue)
        {
        }
        else if (newState == GameStates.WaitingForPlayer)
        {
            lblInfo.GetComponent<Text>().text = "waiting for player";
        }
        else if (newState == GameStates.Chatting)
        {
            lblInfo.SetActive(true);
            //btnPlay.SetActive(true);
            //MsgSend.SetActive(true);
            txtMsg.SetActive(true);
            btnSend.SetActive(true);
            //PMsgSend.SetActive(true);
            btnSendPrefixMsg.SetActive(true);
            ddlMsg.SetActive(true);
            //C2C.SetActive(true);
            ddlPlayer.SetActive(true);
            btnCSend.SetActive(true);
            txtCMsg.SetActive(true);
            chatBox.SetActive(true);
            pnlChat.SetActive(true);

            txtReplay.SetActive(true);
            btnReplay.SetActive(true);
            pnlReplay.SetActive(true);
        }
        else if (newState == GameStates.TickTacToePlay)
        {
            lblInfo.SetActive(true);
            //btnPlay.SetActive(true);
            //MsgSend.SetActive(true);
            //txtMsg.SetActive(true);
            //btnSend.SetActive(true);
            //PMsgSend.SetActive(true);
            //btnSendPrefixMsg.SetActive(true);
            //ddlMsg.SetActive(true);
            //C2C.SetActive(true);
            //ddlPlayer.SetActive(true);
            //btnCSend.SetActive(true);
            //txtCMsg.SetActive(true);
            chatBox.SetActive(true);
            pnlChat.SetActive(true);

            // txtReplay.SetActive(true);
            //btnReplay.SetActive(true);
            //pnlReplay.SetActive(true);

            btn11.SetActive(true);
            btn12.SetActive(true);
            btn13.SetActive(true);
            btn21.SetActive(true);
            btn22.SetActive(true);
            btn23.SetActive(true);
            btn31.SetActive(true);
            btn32.SetActive(true);
            btn33.SetActive(true);
            if (btn11.GetComponentInChildren<Text>().text != "_")
                btn11.GetComponent<Button>().interactable = false;
            if (btn12.GetComponentInChildren<Text>().text != "_")
                btn12.GetComponent<Button>().interactable = false;
            if (btn13.GetComponentInChildren<Text>().text != "_")
                btn13.GetComponent<Button>().interactable = false;
            if (btn21.GetComponentInChildren<Text>().text != "_")
                btn21.GetComponent<Button>().interactable = false;
            if (btn22.GetComponentInChildren<Text>().text != "_")
                btn22.GetComponent<Button>().interactable = false;
            if (btn23.GetComponentInChildren<Text>().text != "_")
                btn23.GetComponent<Button>().interactable = false;
            if (btn31.GetComponentInChildren<Text>().text != "_")
                btn31.GetComponent<Button>().interactable = false;
            if (btn32.GetComponentInChildren<Text>().text != "_")
                btn32.GetComponent<Button>().interactable = false;
            if (btn33.GetComponentInChildren<Text>().text != "_")
                btn33.GetComponent<Button>().interactable = false;
        }
        else if (newState == GameStates.TickTacToeEnd)
        {
            chatBox.SetActive(true);
            pnlChat.SetActive(true);

            // txtReplay.SetActive(true);
            //btnReplay.SetActive(true);
            //pnlReplay.SetActive(true);
            btn11.SetActive(true);
            btn12.SetActive(true);
            btn13.SetActive(true);
            btn21.SetActive(true);
            btn22.SetActive(true);
            btn23.SetActive(true);
            btn31.SetActive(true);
            btn32.SetActive(true);
            btn33.SetActive(true);
            btn11.GetComponent<Button>().interactable = false;
            btn12.GetComponent<Button>().interactable = false;
            btn13.GetComponent<Button>().interactable = false;
            btn21.GetComponent<Button>().interactable = false;
            btn22.GetComponent<Button>().interactable = false;
            btn23.GetComponent<Button>().interactable = false;
            btn31.GetComponent<Button>().interactable = false;
            btn32.GetComponent<Button>().interactable = false;
            btn33.GetComponent<Button>().interactable = false;
        }
        else if (newState == GameStates.Observer)
        {
            lblInfo.SetActive(true);
            chatBox.SetActive(true);
            pnlChat.SetActive(true);
            txtReplay.SetActive(true);
            btnReplay.SetActive(true);
            pnlReplay.SetActive(true);
            //btn11.SetActive(true);
            //btn12.SetActive(true);
            //btn13.SetActive(true);
            //btn21.SetActive(true);
            //btn22.SetActive(true);
            //btn23.SetActive(true);
            //btn31.SetActive(true);
            //btn32.SetActive(true);
            //btn33.SetActive(true);
            //btn11.GetComponent<Button>().interactable = false;
            //btn12.GetComponent<Button>().interactable = false;
            //btn13.GetComponent<Button>().interactable = false;
            //btn21.GetComponent<Button>().interactable = false;
            //btn22.GetComponent<Button>().interactable = false;
            //btn23.GetComponent<Button>().interactable = false;
            //btn31.GetComponent<Button>().interactable = false;
            //btn32.GetComponent<Button>().interactable = false;
            //btn33.GetComponent<Button>().interactable = false;
        }
    }

}

static public class GameStates
{
    public const int LoginMenu = 1;
    public const int MainMenu = 2;
    public const int WaitingInQueue = 3;
    public const int Chatting = 4;
    public const int WaitingForPlayer = 5;
    public const int TickTacToePlay = 6;
    public const int TickTacToeEnd = 6;
    public const int Observer = 7;
}