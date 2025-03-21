using UnityEngine;
using UnityEngine.SceneManagement;
namespace Mirror
{
    /// <summary>Shows NetworkManager controls in a GUI at runtime.</summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("Network/Network Manager HUD")]
    [RequireComponent(typeof(NetworkManager))]
    [HelpURL("https://mirror-networking.gitbook.io/docs/components/network-manager-hud")]
    public class NetworkManagerHUD : MonoBehaviour
    {
        NetworkManager manager;
        public int offsetX;
        public int offsetY;

        void Awake()
        {
            manager = GetComponent<NetworkManager>();
        }
        
        void OnGUI()
        {
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.normal.background = Texture2D.linearGrayTexture; // 设置按钮的背景颜色为白色
            buttonStyle.normal.textColor = Color.white; // 设置按钮的文本颜色为黑色
            buttonStyle.fontSize = 20; // 设置字体大小为20
            buttonStyle.fontStyle = FontStyle.Bold; // 设置字体样式为粗体
            buttonStyle.padding = new RectOffset(10, 10, 10, 10); // 设置按钮的内边距
            // If this width is changed, also change offsetX in GUIConsole::OnGUI
            int width = 300;
            // int height = 20;
            int offsetX = 10;
            int offsetY = 10;
            // Debug.Log("width: " + Screen.width + " height: " + Screen.height);
            // Debug.Log("offsetX: " + offsetX + " offsetY: " + offsetY);
            GUILayout.BeginArea(new Rect(offsetX, offsetY, width, 9999));

            if (!NetworkClient.isConnected && !NetworkServer.active)
                StartButtons();
            else
                StatusLabels();

            if (NetworkClient.isConnected && !NetworkClient.ready)
            {
                if (GUILayout.Button("Client Ready", buttonStyle))
                {
                    // client ready
                    NetworkClient.Ready();
                    if (NetworkClient.localPlayer == null)
                        NetworkClient.AddPlayer();
                }
            }

            StopButtons();

            GUILayout.EndArea();
        }

        void StartButtons()
        {
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.normal.background = Texture2D.linearGrayTexture; // 设置按钮的背景颜色为白色
            buttonStyle.normal.textColor = Color.white; // 设置按钮的文本颜色为黑色
            buttonStyle.fontSize = 20; // 设置字体大小为20
            buttonStyle.fontStyle = FontStyle.Bold; // 设置字体样式为粗体
            buttonStyle.padding = new RectOffset(10, 10, 10, 10); // 设置按钮的内边距

            GUIStyle textFieldStyle = new GUIStyle(GUI.skin.textField);
            textFieldStyle.normal.background = Texture2D.whiteTexture; // 设置文本框的背景颜色为白色
            textFieldStyle.normal.textColor = Color.black; // 设置文本框的文本颜色为黑色
            textFieldStyle.fontSize = 20; // 设置字体大小为20
            textFieldStyle.fontStyle = FontStyle.Bold; // 设置字体样式为粗体
            textFieldStyle.padding = new RectOffset(10, 10, 10, 10); // 设置文本框的内边距
            if (!NetworkClient.active)
            {
#if UNITY_WEBGL
#else
                // Server + Client
                // if (GUILayout.Button("Host (Server + Client)", buttonStyle))
                // {
                //     manager.StartHost();
                // }
#endif          
                // Client + IP (+ PORT)
                GUILayout.BeginHorizontal();

                // if (GUILayout.Button("Client", buttonStyle))
                // { manager.StartClient(); Debug.Log("client in"); }

                manager.networkAddress = GUILayout.TextField(manager.networkAddress, textFieldStyle);
                // only show a port field if we have a port transport
                // we can't have "IP:PORT" in the address field since this only
                // works for IPV4:PORT.
                // for IPV6:PORT it would be misleading since IPV6 contains ":":
                // 2001:0db8:0000:0000:0000:ff00:0042:8329
                if (Transport.active is PortTransport portTransport)
                {
                    // use TryParse in case someone tries to enter non-numeric characters
                    if (ushort.TryParse(GUILayout.TextField(portTransport.Port.ToString(), textFieldStyle), out ushort port))
                        portTransport.Port = port;
                }

                GUILayout.EndHorizontal();
            }
            else
            {
                GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
                labelStyle.normal.textColor = Color.black; // 设置标签的文本颜色为黑色
                labelStyle.fontSize = 20; // 设置字体大小为20
                labelStyle.fontStyle = FontStyle.Bold; // 设置字体样式为粗体
                // Connecting
                GUILayout.Label($"Connecting to {manager.networkAddress}..", labelStyle);
                if (GUILayout.Button("Cancel Connection Attempt", buttonStyle))
                    manager.StopClient();
            }
        }

        void StatusLabels()
        {
            GUIStyle labelStyle = new GUIStyle(GUI.skin.label);
            labelStyle.normal.textColor = Color.black; // 设置标签的文本颜色为黑色
            labelStyle.fontSize = 20; // 设置字体大小为20
            labelStyle.fontStyle = FontStyle.Bold; // 设置字体样式为粗体
            // host mode
            // display separately because this always confused people:
            //   Server: ...
            //   Client: ...
            if (NetworkServer.active && NetworkClient.active)
            {
                // host mode
                GUILayout.Label($"<b>Host</b>: running via {Transport.active}", labelStyle);
            }
            else if (NetworkClient.isConnected)
            {
                // client only
                GUILayout.Label($"<b>Client</b>: connected to {manager.networkAddress} via {Transport.active}", labelStyle);
            }
        }

        void StopButtons()
        {
            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
            buttonStyle.normal.background = Texture2D.linearGrayTexture; // 设置按钮的背景颜色为白色
            buttonStyle.normal.textColor = Color.white; // 设置按钮的文本颜色为黑色
            buttonStyle.fontSize = 20; // 设置字体大小为20
            buttonStyle.fontStyle = FontStyle.Bold; // 设置字体样式为粗体
            buttonStyle.padding = new RectOffset(10, 10, 10, 10); // 设置按钮的内边距
            if (NetworkServer.active && NetworkClient.isConnected)
            {
                GUILayout.BeginHorizontal();
                // stop host if host mode
                if (GUILayout.Button("Stop Host", buttonStyle))
                    manager.StopHost();

                // stop client if host mode, leaving server up
                if (GUILayout.Button("Stop Client", buttonStyle))
                    manager.StopClient();
                GUILayout.EndHorizontal();
            }
            else if (NetworkClient.isConnected)
            {
                // stop client if client-only
                if (GUILayout.Button("Stop Client", buttonStyle))
                    manager.StopClient();
            }
        }
    }
}
