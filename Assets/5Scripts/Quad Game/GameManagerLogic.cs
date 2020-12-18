using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Net;

public class GameManagerLogic : MonoBehaviour
{
    public Text mycoin;
    public Player player;
    public GameObject menuPanel;
    public GameObject gamePlayer;
    public GameObject rivalPlayer;
    public GameObject coinImage;
    public GameObject myScore;
    public GameObject rivalScore;

    public InputField enterIp;
    public GameObject chattingBox;
    public InputField enterText;
    public GameObject enterKey; // 채팅창

    private TransportTCP m_transport;
    private GameState m_state = GameState.HOST_TYPE_SELECT;


    private string m_hostAddress = "";
    private const int m_port = 50765;

    private string m_sendComment = "";
    private string m_prevComment = "";
    private string m_chatMessage = "";
    private List<string>[] m_message;

    private static float KADO_SIZE = 16.0f;
    private static float FONT_SIZE = 13.0f;
    private static float FONG_HEIGHT = 18.0f;
    private static int MESSAGE_LINE = 16;
    private static int CHAT_MEMBER_NUM = 2;

    private bool m_isServer = false;

    // 게임 상태 관리
    enum GameState
    {
        HOST_TYPE_SELECT = 0,   // 방 선택.
        INGAME,               // 채팅 중.
        LEAVE,                  // 나가기.
        ERROR,                  // 오류.
    };

    // Use this for initialization
    void Start()
    {
        IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
        System.Net.IPAddress hostAddress = hostEntry.AddressList[0];
        Debug.Log(hostEntry.HostName);
        m_hostAddress = hostAddress.ToString();

        GameObject go = new GameObject("Network");
        m_transport = go.AddComponent<TransportTCP>();

        m_transport.RegisterEventHandler(OnEventHandling);

        m_message = new List<string>[CHAT_MEMBER_NUM];
        for (int i = 0; i < CHAT_MEMBER_NUM; ++i)
        {
            m_message[i] = new List<string>();
        }
    }

    private void Update()
    {
        mycoin.text = player.coin.ToString();
        switch (m_state)
        {
            case GameState.HOST_TYPE_SELECT:
                for (int i = 0; i < CHAT_MEMBER_NUM; ++i)
                {
                    m_message[i].Clear();
                }
                break;

            case GameState.INGAME:
                UpdateINGAME();
                break;

            case GameState.LEAVE:
                UpdateLeave();
                break;
        }
    }

    void UpdateINGAME()
    {
        byte[] buffer = new byte[2000];

        int recvSize = m_transport.Receive(ref buffer, buffer.Length);
        if (recvSize > 0)
        {
            string message = System.Text.Encoding.UTF8.GetString(buffer);
            Debug.Log("Recv data:" + message);
            m_chatMessage += message + "   ";// + "\n";

            int id = (m_isServer == true) ? 1 : 0;
            AddMessage(ref m_message[id], message);
        }
    }

    void AddMessage(ref List<string> messages, string str)
    {
        while (messages.Count >= MESSAGE_LINE)
        {
            messages.RemoveAt(0);
        }

        messages.Add(str);
    }

    void UpdateLeave()
    {
        if (m_isServer == true)
        {
            m_transport.StopServer();
        }
        else
        {
            m_transport.Disconnect();
        }

        // 메시지 삭제.
        for (int i = 0; i < 2; ++i)
        {
            m_message[i].Clear();
        }

        m_state = GameState.HOST_TYPE_SELECT;
    }

    void OnGUI()
    {
        switch (m_state)
        {
            case GameState.HOST_TYPE_SELECT:
                menuPanel.SetActive(true);
                gamePlayer.SetActive(false);
                rivalPlayer.SetActive(false);
                coinImage.SetActive(false);
                myScore.SetActive(false);
                rivalScore.SetActive(false);
                chattingBox.SetActive(false);
                break;
            case GameState.INGAME:
                ChattingGUI();
                break;

            case GameState.ERROR:
                ErrorGUI();
                break;
        }
    }


    void ChattingGUI()
    {
        Rect commentRect = new Rect(635, 1020, 520, 30);
        m_sendComment = GUI.TextField(commentRect, m_sendComment, 15);

        bool isSent = GUI.Button(new Rect(1165, 1020, 117, 30), "말하기");
        if (Event.current.isKey &&
            Event.current.keyCode == KeyCode.Return)
        {
            if (m_sendComment == m_prevComment)
            {
                isSent = true;
                m_prevComment = "";
            }
            else
            {
                m_prevComment = m_sendComment;
            }
        }

        if (isSent == true)
        {
            string message = "[" + DateTime.Now.ToString("HH:mm") + "] " + m_sendComment;
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(message);
            m_transport.Send(buffer, buffer.Length);
            AddMessage(ref m_message[(m_isServer == true) ? 0 : 1], message);
            m_sendComment = "";
        }

        if (GUI.Button(new Rect(1820, 1020, 80, 30), "나가기"))
        {
            menuPanel.SetActive(true);
            gamePlayer.SetActive(false);
            rivalPlayer.SetActive(false);
            coinImage.SetActive(false);
            myScore.SetActive(false);
            rivalScore.SetActive(false);
            chattingBox.SetActive(false);
            m_state = GameState.LEAVE;
        }


        if (m_transport.IsServer() || m_transport.IsServer() == false && m_transport.IsConnected())
        {
            DispBalloon(ref m_message[0], new Vector2(810.0f, 875.0f), new Vector2(340.0f, 360.0f), Color.cyan, true, 0);
        }

        if (m_transport.IsServer() == false || m_transport.IsServer() && m_transport.IsConnected())
        {
            DispBalloon(ref m_message[1], new Vector2(1120.0f, 875.0f), new Vector2(340.0f, 360.0f), Color.green, false, 1);
        }
    }

    void ErrorGUI()
    {
        float sx = 1920.0f;
        float sy = 1080.0f;
        float px = sx * 0.5f - 150.0f;
        float py = sy * 0.5f;

        if (GUI.Button(new Rect(px, py, 300, 80), "접속에 실패했습니다.\n\n버튼을 누르세요."))
        {
            m_state = GameState.HOST_TYPE_SELECT;
        }
    }

    public void GameStart()
    {
        m_transport.StartServer(m_port, 1);

        
        m_isServer = true;

        menuPanel.SetActive(false);
        gamePlayer.SetActive(true);
        rivalPlayer.SetActive(true);
        coinImage.SetActive(true);
        myScore.SetActive(true);
        rivalScore.SetActive(true);
        chattingBox.SetActive(true);

        m_state = GameState.INGAME;
        //enterText.SetActive(true);
        //enterKey.SetActive(true);

    }

    public void JoinGame()
    {
        try
        {
            m_hostAddress = enterIp.text;
            Debug.Log(m_hostAddress);
        }
        catch (NullReferenceException ex)
        {
            Debug.Log("myLight was not set in the inspector");
        }
        

        bool ret = m_transport.Connect(m_hostAddress, m_port);
        if (ret)
        {
            m_state = GameState.INGAME;
        }
        else
        {
            m_state = GameState.ERROR;
        }

        menuPanel.SetActive(false);
        gamePlayer.SetActive(true);
        rivalPlayer.SetActive(true);
        coinImage.SetActive(true);
        myScore.SetActive(true);
        rivalScore.SetActive(true);
        chattingBox.SetActive(true);
        //enterText.SetActive(true);
        //enterKey.SetActive(true);

        
    }


    void DispBalloon(ref List<string> messages, Vector2 position, Vector2 size, Color color, bool left, int check)
    {
        // 채팅 문장을 표시합니다. 	
        foreach (string s in messages)
        {
            DrawText(s, position, size, check);
            position.y += FONG_HEIGHT;
        }
    }

    void DrawText(string message, Vector2 position, Vector2 size, int check)
    {
        if (message == "")
        {
            return;
        }

        GUIStyle style = new GUIStyle();
        style.fontSize = 16;

        if (check == 0)
        {
            style.normal.textColor = Color.yellow;
        }
        else
        {
            style.normal.textColor = Color.cyan;
        }
        

        Vector2 balloon_size, text_size;

        text_size.x = message.Length * FONT_SIZE;
        text_size.y = FONG_HEIGHT;

        balloon_size.x = text_size.x + KADO_SIZE * 2.0f;
        balloon_size.y = text_size.y + KADO_SIZE;

        Vector2 p;

        p.x = position.x - size.x / 2.0f + KADO_SIZE;
        p.y = position.y - size.y / 2.0f + KADO_SIZE;
        //p.x = position.x - text_size.x/2.0f;
        //p.y = position.y - text_size.y/2.0f;

        GUI.Label(new Rect(p.x, p.y, text_size.x, text_size.y), message, style);
    }

    void OnApplicationQuit()
    {
        if (m_transport != null)
        {
            m_transport.StopServer();
        }
    }

    public void OnEventHandling(NetEventState state)
    {
        switch (state.type)
        {
            case NetEventType.Connect:
                if (m_transport.IsServer())
                {
                    AddMessage(ref m_message[0], "게스트가 입장했습니다.");
                }
                else
                {
                    AddMessage(ref m_message[1], "호스트와 이야기할 수 있습니다.");
                }
                break;

            case NetEventType.Disconnect:
                if (m_transport.IsServer())
                {
                    AddMessage(ref m_message[0], "게스트가 나갔습니다.");
                }
                else
                {
                    AddMessage(ref m_message[1], "호스트가 나갔습니다.");
                }
                break;
        }
    }
}
