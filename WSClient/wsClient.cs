using System;
using System.Text;
using Crestron.SimplSharp;// For Basic SIMPL# Classes
using Crestron.SimplSharp.CrestronWebSocketClient; 

namespace WebSocket
{
    //public delegate for connection status
    public delegate void SendDigitalAnalogData(ushort Data);
    //public delegate for return or pushed data from server
    //public delegate void returnStringStringUshort(SimplSharpString Url, SimplSharpString Data);

    /// <summary>
    /// SIMPL+ can only execute the default constructor. If you have variables that require initialization, please
    /// use an Initialize method
    /// </summary>

    public class MyWebSocket
    {
        public WebSocketClient myWSC = new WebSocketClient();
        public WebSocketClient.WEBSOCKET_RESULT_CODES ret;
        public WebSocketClient.WEBSOCKET_PACKET_TYPES opcode;
        public byte[] SendData = new byte[6];
        public byte[] ReceiveData;
        WebSocketClient.WEBSOCKET_RESULT_CODES error;

        public SendDigitalAnalogData UpdateSocketStatus { get; set; }
        //public returnStringStringUshort wsServerData { get; set; }

        public MyWebSocket()
        {
         
        }

        public void Connect(String Url)
        {
            myWSC.URL = Url;
            CrestronConsole.PrintLine(myWSC.URL);
            error = myWSC.Connect();
            if (error == (int)WebSocketClient.WEBSOCKET_RESULT_CODES.WEBSOCKET_CLIENT_SUCCESS)
            {
                UpdateSocketStatus(1);
                CrestronConsole.PrintLine("Websocket connected \r\n");
            }
            else
            {
                CrestronConsole.Print("Websocket could not connect to server.  Connect return code: " + error.ToString());
            }
        }

        public void DisconnectWebSocket()
        {
            myWSC.Disconnect();
            UpdateSocketStatus(0);
            CrestronConsole.PrintLine("Websocket disconnected. \r\n");
        }

        public void SingleSendAndReceive(String data)
        {
            try
            {
                SendData = System.Text.Encoding.ASCII.GetBytes(data);
                ret = (WebSocketClient.WEBSOCKET_RESULT_CODES)myWSC.Send(SendData, (uint)SendData.Length, WebSocketClient.WEBSOCKET_PACKET_TYPES.LWS_WS_OPCODE_07__TEXT_FRAME, WebSocketClient.WEBSOCKET_PACKET_SEGMENT_CONTROL.WEBSOCKET_CLIENT_PACKET_END);
                myWSC.Receive(out ReceiveData, out opcode);
                if (ReceiveData != null)
                {
                    CrestronConsole.Print("Data received after performing send: \r\n");
                    foreach (byte b in ReceiveData)
                    {
                        CrestronConsole.Print(System.Convert.ToChar(b)+"");
                    }
                    CrestronConsole.Print("\r\n");
                }
            }
            catch (Exception e)
            {
                DisconnectWebSocket();
                UpdateSocketStatus(0);
                CrestronConsole.Print("Something went wrong - Error Code:" + e + "\r\n");
            }
        }
    }
}