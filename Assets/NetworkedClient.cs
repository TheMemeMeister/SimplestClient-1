using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkedClient : MonoBehaviour
{

    int connectionID;
    int maxConnections = 1000;
    int reliableChannelID;
    int unreliableChannelID;
    int hostID;
    int socketPort = 5491;//5492
    byte error;
    bool isConnected = false;
    int ourClientID;
    GameObject gameSystemManager;
    // Start is called before the first frame update
    void Start()
    {
        GameObject[] allobjects = FindObjectsOfType<GameObject>();
        foreach (GameObject go in allobjects)
        {
            if (go.GetComponent<GameSystemManager>() != null)
            {
                gameSystemManager = go;
            }
        }
        Connect();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateNetworkConnection();
    }

    private void UpdateNetworkConnection()
    {
        if (isConnected)
        {
            int recHostID;
            int recConnectionID;
            int recChannelID;
            byte[] recBuffer = new byte[1024];
            int bufferSize = 1024;
            int dataSize;
            NetworkEventType recNetworkEvent = NetworkTransport.Receive(out recHostID, out recConnectionID, out recChannelID, recBuffer, bufferSize, out dataSize, out error);

            switch (recNetworkEvent)
            {
                case NetworkEventType.ConnectEvent:
                    Debug.Log("connected.  " + recConnectionID);
                    ourClientID = recConnectionID;
                    break;
                case NetworkEventType.DataEvent:
                    string msg = Encoding.Unicode.GetString(recBuffer, 0, dataSize);
                    ProcessRecievedMsg(msg, recConnectionID);
                    Debug.Log("got msg = " + msg);
                    break;
                case NetworkEventType.DisconnectEvent:
                    isConnected = false;
                    Debug.Log("disconnected.  " + recConnectionID);
                    break;
            }
        }
    }

    private void Connect()
    {

        if (!isConnected)
        {
            Debug.Log("Attempting to create connection");

            NetworkTransport.Init();

            ConnectionConfig config = new ConnectionConfig();
            reliableChannelID = config.AddChannel(QosType.Reliable);
            unreliableChannelID = config.AddChannel(QosType.Unreliable);
            HostTopology topology = new HostTopology(config, maxConnections);
            hostID = NetworkTransport.AddHost(topology, 0);
            Debug.Log("Socket open.  Host ID = " + hostID);

            connectionID = NetworkTransport.Connect(hostID, "192.168.43.103", socketPort, 0, out error); // server is local on network

            if (error == 0)
            {
                isConnected = true;

                Debug.Log("Connected, id = " + connectionID);

            }
        }
    }

    public void Disconnect()
    {
        NetworkTransport.Disconnect(hostID, connectionID, out error);
    }

    public void SendMessageToHost(string msg)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(msg);
        NetworkTransport.Send(hostID, connectionID, reliableChannelID, buffer, msg.Length * sizeof(char), out error);
    }

    private void ProcessRecievedMsg(string msg, int id)
    {
        Debug.Log("msg recieved = " + msg + ".  connection id = " + id);
        //chk message 
        string[] csv = msg.Split(',');
        if (csv.Length > 0)
        {
            int signifier = int.Parse(csv[0]);
            if (signifier == ServerToClientSignifiers.LoginComplete)//msg format: signifier, name
            {
                Debug.Log("Login successful");
                if (csv.Length > 1)
                {
                    gameSystemManager.GetComponent<GameSystemManager>().updateUserName(csv[1]);
                }
                gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.MainMenu);
            }
            else if (signifier == ServerToClientSignifiers.LoginFailed)//msg format: signifier, name
                Debug.Log("Login Failed");
            else if (signifier == ServerToClientSignifiers.AccountCreationComplete)//msg format: signifier, name
            {
                Debug.Log("account creation successful");
                if (csv.Length > 1)
                {
                    gameSystemManager.GetComponent<GameSystemManager>().updateUserName(csv[1]);
                }
                gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.MainMenu);
            }
            else if (signifier == ServerToClientSignifiers.AccountCreationFailed)//msg format: signifier, name
                Debug.Log("Account creation failed");
            else if (signifier == ServerToClientSignifiers.JoinedPlay)//msg format: signifier,clientid,joined chatter name 
            {
                //waiting for other player
                if (csv.Length > 2)
                    gameSystemManager.GetComponent<GameSystemManager>().updateChat("join player " + csv[2]);
                if (gameSystemManager.GetComponent<GameSystemManager>().getIsPlayer())
                    gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.Chatting);
                else
                    gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.Observer);
            }
            else if (signifier == ServerToClientSignifiers.ChatStart)//msg format: siginifier, list of player from 
            {
                Debug.Log("players1: " + csv[1]);
                Debug.Log("players2: " + csv[2]);
                Debug.Log("players3: " + csv[3]);
                //showing list of chatter
                //2 opponent chatter
                List<string> otherPlayerList = new List<string>();
                if (csv.Length > 3)
                {
                    if (!otherPlayerList.Contains(csv[1]))
                        otherPlayerList.Add(csv[1]);
                    if (!otherPlayerList.Contains(csv[2]))
                        otherPlayerList.Add(csv[2]);
                    if (!otherPlayerList.Contains(csv[3]))
                        otherPlayerList.Add(csv[3]);
                }
                //loading dropdown with other member of chat room
                gameSystemManager.GetComponent<GameSystemManager>().LoadPlayer(otherPlayerList);
                if (gameSystemManager.GetComponent<GameSystemManager>().getIsPlayer())
                    gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.Chatting);
                else
                    gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.Observer);
            }
            else if (signifier == ServerToClientSignifiers.DGameStart)//msg format:signifier, player number of yours, opponent player name
            {
                Debug.Log("self no: " + csv[1]);
                Debug.Log("opp name: " + csv[2]);
                gameSystemManager.GetComponent<GameSystemManager>().playerNumber = int.Parse(csv[1]);
                Debug.Log("pln:" + gameSystemManager.GetComponent<GameSystemManager>().playerNumber);
                gameSystemManager.GetComponent<GameSystemManager>().updateChat("tick tac toe play start with you and " + csv[2] + " player.");
                if (gameSystemManager.GetComponent<GameSystemManager>().getIsPlayer())
                    gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.TickTacToePlay);
                else
                    gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.Observer);
            }
            else if (signifier == ServerToClientSignifiers.OtherPlay)//msg format: signifier,turned player name,turn value,cell played by current player,flag after checking turn
            {
                gameSystemManager.GetComponent<GameSystemManager>().setTextButton(int.Parse(csv[3]), csv[2]);
                gameSystemManager.GetComponent<GameSystemManager>().updateChat("tick tac toe played " + csv[2] + " turn by " + csv[1] + " in " + csv[3] + " cell.");
                if (csv[4] == "1")
                    gameSystemManager.GetComponent<GameSystemManager>().updateChat("Game has ended. " + csv[1] + " player has win.");
                if (csv[4] == "-1")
                    gameSystemManager.GetComponent<GameSystemManager>().updateChat("Game has ended. The result is draw.");
                gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.Observer);
            }
            else if (signifier == ServerToClientSignifiers.OpponentPlay)//msg format: signifier,opponent player name,turn value,cell played by current player,flag after checking turn
            //flag is 1=win,-1=draw,0=running
            {
                Debug.Log("f " + csv[4]);
                gameSystemManager.GetComponent<GameSystemManager>().setTextButton(int.Parse(csv[3]), csv[2]);
                if (gameSystemManager.GetComponent<GameSystemManager>().getIsPlayer())
                {
                    gameSystemManager.GetComponent<GameSystemManager>().updateChat("tick tac toe played " + csv[2] + " turn by " + csv[1] + " in " + csv[3] + " cell.");
                    if (csv[4] == "0")
                    {
                        gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.TickTacToePlay);
                    }
                    else
                    {
                        if (csv[4] == "1")
                            gameSystemManager.GetComponent<GameSystemManager>().updateChat("Game has ended. " + csv[1] + " player has win.");
                        if (csv[4] == "-1")
                            gameSystemManager.GetComponent<GameSystemManager>().updateChat("Game has ended. The result is draw.");
                        gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.TickTacToeEnd);
                    }
                }
                else
                    gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.Observer);
            }
            else if (signifier == ServerToClientSignifiers.SelfPlay)//msg format: signifier,self player name,turn value,cell played by current player,flag after checking turn 
            //flag is 1=win,-1=draw,0=running
            {
                Debug.Log("f " + csv[4]);
                if (gameSystemManager.GetComponent<GameSystemManager>().getIsPlayer())
                {
                    gameSystemManager.GetComponent<GameSystemManager>().updateChat("tick tac toe played " + csv[2] + " turn by " + csv[1] + " in " + csv[3] + " cell.");
                    if (csv[4] == "0")
                    {
                        gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.TickTacToePlay);
                    }
                    else
                    {
                        if (csv[4] == "1")
                            gameSystemManager.GetComponent<GameSystemManager>().updateChat("Game has ended. " + csv[1] + " player has win.");
                        if (csv[4] == "-1")
                            gameSystemManager.GetComponent<GameSystemManager>().updateChat("Game has ended. The result is draw.");
                        gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.TickTacToeEnd);
                    }
                }
                else
                    gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.Observer);
            }

            else if (signifier == ServerToClientSignifiers.ReceiveMsg)//msg format: signifier, client id, message content, msg sender name
            {
                Debug.Log("rece" + csv[2]);
                if (csv.Length > 3)
                    gameSystemManager.GetComponent<GameSystemManager>().updateChat(csv[3] + ":" + csv[2]);
                if (gameSystemManager.GetComponent<GameSystemManager>().getIsPlayer())
                    gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.Chatting);
                else
                    gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.Observer);
            }
            else if (signifier == ServerToClientSignifiers.ReceiveCMsg)//msg format: signifier, clientid, message content, sender name
            {
                // Debug.Log("rece" + csv[1]);
                if (csv.Length > 3)
                    gameSystemManager.GetComponent<GameSystemManager>().updateChat(csv[3] + ":" + csv[2]);
                if (gameSystemManager.GetComponent<GameSystemManager>().getIsPlayer())
                    gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.Chatting);
                else
                    gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.Observer);
            }
            else if (signifier == ServerToClientSignifiers.someoneJoinedAsObserver)//msg format: signifier,client id, observer name
            {
                if (csv.Length > 2)
                    gameSystemManager.GetComponent<GameSystemManager>().updateChat("Some one has joined as Observer " + csv[2]);
                if (gameSystemManager.GetComponent<GameSystemManager>().getIsPlayer())
                    gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.Chatting);
                else
                    gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.Observer);
            }
            else if (signifier == ServerToClientSignifiers.ReplayMsg)//msg format: signifier, msg
            {
                if (csv.Length > 1)
                    gameSystemManager.GetComponent<GameSystemManager>().updateReplay(csv[1]);
                if (gameSystemManager.GetComponent<GameSystemManager>().getIsPlayer())
                    gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.Chatting);
                else
                    gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.Observer);
            }
        }
    }

    public bool IsConnected()
    {
        return isConnected;
    }


}
public static class ClientToServerSignifiers
{
    public const int CreateAccount = 1;
    public const int Login = 2;
    public const int JoinChatRoomQueue = 3;
    public const int PlayGame = 4;
    public const int SendMsg = 5;
    public const int SendPrefixMsg = 6;
    public const int JoinAsObserver = 7;
    public const int SendClientMsg = 8;
    public const int ReplayMsg = 9;
    public const int JoinDGameRoomQueue = 10;
    public const int JoinDAsObserver = 11;
}
public static class ServerToClientSignifiers
{
    public const int LoginComplete = 1;
    public const int LoginFailed = 2;
    public const int AccountCreationComplete = 3;
    public const int AccountCreationFailed = 4;
    public const int OpponentPlay = 5;
    public const int ChatStart = 6;
    public const int ReceiveMsg = 7;
    public const int someoneJoinedAsObserver = 8;
    public const int JoinedPlay = 9;
    public const int ReceiveCMsg = 10;
    public const int ReplayMsg = 11;
    public const int DGameStart = 12;
    public const int SelfPlay = 13;
    public const int OtherPlay = 14;
}