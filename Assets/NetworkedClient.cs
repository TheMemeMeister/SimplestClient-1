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
            if (go.GetComponent<GameSystemManager>()!=null)
            {
                gameSystemManager = go;
            }
        }
            Connect();
    }

    // Update is called once per frame
    void Update()
    {
       // if(Input.GetKeyDown(KeyCode.S))
         //   SendMessageToHost("Hello from client");

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

            connectionID = NetworkTransport.Connect(hostID, "192.168.2.39", socketPort, 0, out error); // server is local on network

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
            if (signifier == ServerToClientSignifiers.LoginComplete)
            {
                Debug.Log("Login successful");
                if (csv.Length > 1)
                {
                    gameSystemManager.GetComponent<GameSystemManager>().updateUserName(csv[1]);                    
                }
                gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.MainMenu);
            }
            else if (signifier == ServerToClientSignifiers.LoginFailed)
                Debug.Log("Login Failed");
            else if (signifier == ServerToClientSignifiers.AccountCreationComplete)
            {
                Debug.Log("account creation successful");
                if (csv.Length > 1)
                { 
                    gameSystemManager.GetComponent<GameSystemManager>().updateUserName(csv[1]);                    
                }
                gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.MainMenu);
            }
            else if (signifier == ServerToClientSignifiers.AccountCreationFailed)
                Debug.Log("Account creation failed");
            else if (signifier == ServerToClientSignifiers.JoinedPlay)
            {
                //waiting for other player
                if(csv.Length>2)
                gameSystemManager.GetComponent<GameSystemManager>().updateChat("join player "+csv[2]);
                if (gameSystemManager.GetComponent<GameSystemManager>().getIsPlayer())
                    gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.TicTacToe);
                else
                    gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.Observer);
            }
            //else if (signifier == ServerToClientSignifiers.JoinedPlayAsOpponent)
            //{
            //    //waiting for other player
            //    gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.);
            //}
            else if (signifier == ServerToClientSignifiers.GameStart)
            {
                Debug.Log("players1: " + csv[1]);
                Debug.Log("players2: " + csv[2]);
                Debug.Log("players3: " + csv[3]);
                //showing list of player
                //2 opponent player
                List<string> otherPlayerList = new List<string>();
                if (csv.Length > 3)
                {
                    if(!otherPlayerList.Contains(csv[1]))
                        otherPlayerList.Add(csv[1]);
                    if (!otherPlayerList.Contains(csv[2]))
                        otherPlayerList.Add(csv[2]);
                    if (!otherPlayerList.Contains(csv[3]))
                        otherPlayerList.Add(csv[3]);
                }
                gameSystemManager.GetComponent<GameSystemManager>().LoadPlayer(otherPlayerList);
                if (gameSystemManager.GetComponent<GameSystemManager>().getIsPlayer())
                    gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.TicTacToe);
                else
                    gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.Observer);
            }
            else if (signifier == ServerToClientSignifiers.ReceiveMsg)
            {
                Debug.Log("rece" + csv[2]);
                if (csv.Length > 3)
                    gameSystemManager.GetComponent<GameSystemManager>().updateChat(csv[3]+":"+csv[2]);
                if (gameSystemManager.GetComponent<GameSystemManager>().getIsPlayer())
                    gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.TicTacToe);
                else
                    gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.Observer);
            }
            else if (signifier == ServerToClientSignifiers.ReceiveCMsg)
            {
               // Debug.Log("rece" + csv[1]);
                if (csv.Length > 3)
                    gameSystemManager.GetComponent<GameSystemManager>().updateChat(csv[3]+":"+csv[2]);
                if (gameSystemManager.GetComponent<GameSystemManager>().getIsPlayer())
                    gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.TicTacToe);
                else
                    gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.Observer);
            }
            else if (signifier == ServerToClientSignifiers.someoneJoinedAsObserver)
            {
                if (csv.Length > 2)
                    gameSystemManager.GetComponent<GameSystemManager>().updateChat("Some one has joined as Observer " + csv[2]);
                if (gameSystemManager.GetComponent<GameSystemManager>().getIsPlayer())
                    gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.TicTacToe);
                else
                    gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.Observer);
            }
            else if (signifier==ServerToClientSignifiers.ReplayMsg)
            {
                if(csv.Length>1)
                    gameSystemManager.GetComponent<GameSystemManager>().updateReplay(csv[1]);
                if(gameSystemManager.GetComponent<GameSystemManager>().getIsPlayer())
                    gameSystemManager.GetComponent<GameSystemManager>().ChangeState(GameStates.TicTacToe);
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
    public const int JoinGammeRoomQueue = 3;
    public const int PlayGame = 4;
    public const int SendMsg = 5;
    public const int SendPrefixMsg = 6;
    public const int JoinAsObserver = 7;
    public const int SendClientMsg = 8;
    public const int ReplayMsg = 9;
}
public static class ServerToClientSignifiers
{
    public const int LoginComplete = 1;
    public const int LoginFailed = 2;
    public const int AccountCreationComplete = 3;
    public const int AccountCreationFailed = 4;
    public const int OpponentPlay = 5;
    public const int GameStart = 6;
    public const int ReceiveMsg = 7;
    public const int someoneJoinedAsObserver = 8;
    public const int JoinedPlay = 9;
    public const int ReceiveCMsg = 10;
    public const int ReplayMsg = 11;
}