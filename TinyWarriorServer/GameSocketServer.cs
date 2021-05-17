using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using TinyWarriorInfo;

namespace TinyWarriorServer
{
        class GameSocketServer
        {
                private static byte[] result = new byte[1024];
                private static string ip = "127.0.0.1";
                private static int port = 8765;
                private static IPEndPoint ipEndPoint;
                private static Socket serverSocket;
                private static List<ClientInfo> clientsInfo;
                private static List<RoomInfo> roomsInfo;
                private static Timer monitorTimer;
                private static int monitorPeriod = 5000;

                static void Main(string[] args)
                {
                        PrintLineInfo("Current ip and port: {0}:{1}. Do you want to change these? (y for yes, others for skip)", ip, port);
                        string isChange = Console.ReadLine().Trim().ToLower();
                        if (isChange == "y")
                        {
                                ChangeIP();
                        }
                        else
                        {
                                ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // create socket object of server and set property
                                serverSocket.Bind(ipEndPoint); // bind ip and port
                                serverSocket.Listen(10); // set max length of request queue
                        }
                        clientsInfo = new List<ClientInfo>();
                        roomsInfo = new List<RoomInfo>();

                        PrintLineInfo("Initialize listening {0} successfully.", serverSocket.LocalEndPoint.ToString());
                        Thread thread = new Thread(ListenClientConnection);
                        thread.Start();
                        monitorTimer = new Timer(MonitorClients, null, monitorPeriod, monitorPeriod);

                        CommandHandle();
                }

                #region -- Command Handle --

                private static void CommandHandle()
                {
                        string command = Console.ReadLine().Trim().ToLower();
                        while (command != "shutdown")
                        {
                                switch (command)
                                {
                                        case "test":
                                                int[] array = GenerateRandomPosition();
                                                for (int i = 0; i < array.Length; i++)
                                                {
                                                        Console.Write(array[i] + " ");
                                                }
                                                Console.WriteLine();
                                                break;
                                        case "clear":
                                                Console.Clear();
                                                break;
                                        case "monitor pause":
                                                MonitorPause();
                                                break;
                                        case "monitor resume":
                                                MonitorResume();
                                                break;
                                        case "remove clients":
                                                RemoveAllConnection();
                                                break;
                                        case "remove rooms":
                                                RemoveAllRooms();
                                                break;
                                        //work in progress
                                        case "'":// work in progress
                                                PrintLineInfo("Add test rooms.");// work in progress

                                                RoomInfo roomInfo = new RoomInfo();// work in progress
                                                roomInfo.GuestsAddressAndName = new Hashtable();// work in progress
                                                roomInfo.GuestsAddressAndName.Add("127.0.0.1:6001", "Player1");// work in progress
                                                roomInfo.GuestsAddressAndName.Add("127.0.0.1:6002", "Player2");// work in progress
                                                roomInfo.MaxPlayer = 2;// work in progress
                                                roomInfo.RoomName = "test room";// work in progress
                                                roomInfo.OwnerAddress = "127.0.0.1:6001";

                                                RoomInfo roomInfo2 = new RoomInfo();// work in progress
                                                roomInfo2.GuestsAddressAndName = new Hashtable();// work in progress
                                                roomInfo2.GuestsAddressAndName.Add("127.0.0.1:6003", "Player3");// work in progress
                                                roomInfo2.GuestsAddressAndName.Add("127.0.0.1:6004", "Player4");// work in progress
                                                roomInfo2.MaxPlayer = 7;// work in progress
                                                roomInfo2.RoomName = "test room2";// work in progress
                                                roomInfo2.OwnerAddress = "127.0.0.1:6004";

                                                RoomInfo roomInfo3 = new RoomInfo();// work in progress
                                                roomInfo3.GuestsAddressAndName = new Hashtable();// work in progress
                                                roomInfo3.GuestsAddressAndName.Add("127.0.0.1:6005", "Player5");// work in progress
                                                roomInfo3.GuestsAddressAndName.Add("127.0.0.1:6006", "Player6");// work in progress
                                                roomInfo3.MaxPlayer = 10;// work in progress
                                                roomInfo3.IsStart = true;
                                                roomInfo3.RoomName = "test room3";// work in progress
                                                roomInfo3.OwnerAddress = "127.0.0.1:6005";

                                                roomsInfo.Add(roomInfo);// work in progress
                                                roomsInfo.Add(roomInfo2);// work in progress
                                                roomsInfo.Add(roomInfo3);// work in progress
                                                break;// work in progress
                                        //work in progress
                                        case "show rooms":
                                                ShowAllRooms();
                                                break;
                                        case "show clients":
                                                ShowAllClients();
                                                break;
                                        case "send":
                                                SendToAll();
                                                break;
                                        default:
                                                PrintLineInfo("There is no this command.");
                                                break;
                                }
                                command = Console.ReadLine().Trim().ToLower();
                        }
                        ShutdownServer();
                }

                private static void ChangeIP()
                {
                        try
                        {
                                PrintInfo("Please input ip address: ");
                                ip = Console.ReadLine();
                                PrintInfo("Please input port: ");
                                port = int.Parse(Console.ReadLine());
                                ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                                PrintLineInfo("IP and port have been changed successfully.");
                                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // create socket object of server and set property
                                serverSocket.Bind(ipEndPoint); // bind ip and port
                                serverSocket.Listen(10); // set max length of request queue
                        }
                        catch
                        {
                                ip = "127.0.0.1";
                                port = 8765;
                                ipEndPoint = new IPEndPoint(IPAddress.Parse(ip), port);
                                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); // create socket object of server and set property
                                serverSocket.Bind(ipEndPoint); // bind ip and port
                                serverSocket.Listen(10); // set max length of request queue
                                PrintLineWarning("Error port or ip address, both of two have been set to default.");
                        }

                }

                private static void MonitorPause()
                {
                        monitorTimer.Change(-1, monitorPeriod);
                }

                private static void MonitorResume()
                {
                        monitorTimer.Change(0, monitorPeriod);
                }

                private static void RemoveAllConnection()
                {
                        if (clientsInfo.Count != 0)
                        {
                                foreach (ClientInfo clientInfo in clientsInfo)
                                {
                                        try
                                        {
                                                clientInfo.Socket.Close();
                                        }
                                        catch (Exception ex)
                                        {
                                                Console.WriteLine(ex.Message);
                                        }
                                }
                        }
                        PrintLineInfo("All clients have been disconnected.");
                }

                private static void RemoveAllRooms()
                {
                        roomsInfo.Clear();
                        PrintLineInfo("All rooms have been removed.");
                }

                private static void SendToAll()
                {
                        PrintInfo("Please input a message: ");
                        string message = Console.ReadLine().Trim().ToLower();
                        if (clientsInfo.Count != 0)
                        {
                                foreach (ClientInfo clientInfo in clientsInfo)
                                {
                                        SendMsgToClient(message, clientInfo.Socket);
                                }
                        }
                }

                private static void ShutdownServer()
                {
                        RemoveAllConnection();
                        try
                        {
                                serverSocket.Close();
                        }
                        finally
                        {
                                PrintLineInfo("Game server has exited.");
                                Environment.Exit(0);
                        }
                }

                private static void ShowAllRooms()
                {
                        if (roomsInfo.Count == 0)
                        {
                                PrintLineInfo("No room created.");
                                return;
                        }
                        int i = 1;
                        PrintLineInfo("{0,-8} {1,-20}  {2,-15}  {3,-15}  {4,-15}  {5,-15}", "number", "room name", "current players", "max players", "is playing", "owner's ip");
                        foreach (RoomInfo roomInfo in roomsInfo)
                        {
                                PrintLineInfo("{0,-8} {1,-20}  {2,-15}  {3,-15}  {4,-15}  {5,-15}", i++, roomInfo.RoomName, roomInfo.GuestsAddressAndName.Count, roomInfo.MaxPlayer, roomInfo.IsStart, roomInfo.OwnerAddress);
                                Console.Write("{0,31} {1,6}", "players' list: ", "");
                                foreach (string name in roomInfo.GuestsAddressAndName.Values)
                                {
                                        Console.Write("<" + name + "> ");
                                }
                                Console.WriteLine();
                        }
                }

                private static void ShowAllClients()
                {
                        if (clientsInfo.Count == 0)
                        {
                                PrintLineInfo("No client conneted.");
                                return;
                        }
                        int i = 1;
                        PrintLineInfo("{0,-6}  {1,-22}  {2,-10}", "number", "ip", "name");
                        foreach (ClientInfo clientInfo in clientsInfo)
                        {
                                PrintLineInfo("{0,-6}  {1,-22}  {2,-10}", i++, clientInfo.Socket.RemoteEndPoint, "<" + clientInfo.PlayerName + ">");
                        }
                }

                #endregion

                #region -- Listener --

                // periodic check rooms
                private static void MonitorClients(object state)
                {
                        List<RoomInfo> tempRoomsInfo = new List<RoomInfo>(); // used to record roomInfo that should be removed
                        List<ClientInfo> tempClientsInfo = new List<ClientInfo>(); // used to record clientInfo that should be removed
                        List<RoomInfo> tempRoomInfo = new List<RoomInfo>(); // used with below
                        List<string> tempAddress = new List<string>(); // both of two used to record address of roomInfo that should be removed
                        bool isFoundClient; // check whether client exists

                        // here belows can't use foreach loop to remove, it shall throw InvalidOperationException in runtime (Due to IEnumerator returned by Collection is readonly)
                        foreach (RoomInfo roomInfo in roomsInfo) // check every room
                        {
                                foreach (string address in roomInfo.GuestsAddressAndName.Keys) // check every guest's address
                                {
                                        isFoundClient = false;
                                        foreach (ClientInfo clientInfo in clientsInfo) // check every clientInfo to find whether there is a client's address matched
                                        {
                                                try
                                                {
                                                        if (address == clientInfo.Socket.RemoteEndPoint.ToString())
                                                        {
                                                                isFoundClient = true; // matched client
                                                                if (!IsSocketConnected(clientInfo.Socket)) // detected client has disconnected
                                                                {
                                                                        tempClientsInfo.Add(clientInfo);
                                                                }
                                                                break;
                                                        }
                                                }
                                                catch
                                                {
                                                        tempClientsInfo.Add(clientInfo);
                                                        break;
                                                }
                                        }
                                        if (!isFoundClient) // can't find client in current room, then remove it
                                        {
                                                tempRoomInfo.Add(roomInfo);
                                                tempAddress.Add(address);
                                        }
                                }
                        }

                        // check again after clearing up clients in rooms
                        foreach (RoomInfo roomInfo in roomsInfo) // check every room
                        {
                                if (roomInfo.GuestsAddressAndName.Count == 0)
                                {
                                        tempRoomsInfo.Add(roomInfo);
                                }
                        }

                        if (tempClientsInfo.Count == 0 && tempRoomInfo.Count == 0 && tempRoomsInfo.Count == 0)
                        {
                                return;
                        }

                        // use for loops to remove
                        for (int i = 0; i < tempClientsInfo.Count; i++)
                        {
                                clientsInfo.Remove(tempClientsInfo[i]);
                        }
                        for (int i = 0; i < tempRoomInfo.Count; i++)
                        {
                                roomsInfo[roomsInfo.IndexOf(tempRoomInfo[i])].GuestsAddressAndName.Remove(tempAddress[i]);
                        }
                        for (int i = 0; i < tempRoomsInfo.Count; i++)
                        {
                                roomsInfo.Remove(tempRoomsInfo[i]);
                        }
                        PrintLineInfo("The server has cleared up: " + tempRoomInfo.Count + " invalid player(s) in room(s), " + tempClientsInfo.Count + " invalid client(s), " + tempRoomsInfo.Count + " invalid room(s).");
                }

                // listen connection
                private static void ListenClientConnection()
                {
                        while (true)
                        {
                                ClientInfo clientInfo = new ClientInfo();

                                // create a new socket object for new client
                                clientInfo.Socket = serverSocket.Accept();
                                // create a thread for a connected client to receive message
                                clientInfo.ReceiveThread = new Thread(ReceiveMsgFromClient);
                                clientInfo.ReceiveThread.Start(clientInfo);
                                clientsInfo.Add(clientInfo);
                        }
                }

                // judge socket connection status
                private static bool IsSocketConnected(Socket clientSocket)
                {
                        try
                        {
                                if (clientSocket.RemoteEndPoint != null && (!clientSocket.Poll(3000, SelectMode.SelectRead) || clientSocket.Available > 0))
                                {
                                        return true;
                                }
                                return false;
                        }
                        catch
                        {
                                return false;
                        }
                }
                #endregion

                #region -- Receive & Send --

                private static void SendMsgToClient(string message, Socket client)
                {
                        ByteBuffer buffer = new ByteBuffer(message);
                        byte[] result = buffer.ToBytes();
                        client.Send(result);
                }

                private static void SendMsgToRoom(string message, RoomInfo roomInfo)
                {
                        foreach (string guestAddress in roomInfo.GuestsAddressAndName.Keys)
                        {
                                foreach (ClientInfo client in clientsInfo)
                                {
                                        if (client.Socket.RemoteEndPoint.ToString() == guestAddress)
                                        {
                                                SendMsgToClient(message, client.Socket); // tell all clients who in room
                                        }
                                }
                        }
                }

                private static void SendIndexToRoom(RoomInfo roomInfo)
                {
                        int i = 0;
                        foreach (string guestAddress in roomInfo.GuestsAddressAndName.Keys)
                        {
                                foreach (ClientInfo client in clientsInfo)
                                {
                                        if (client.Socket.RemoteEndPoint.ToString() == guestAddress)
                                        {
                                                SendMsgToClient("index=" + i++.ToString(), client.Socket); // tell all clients who's index
                                        }
                                }
                        }
                }

                private static void SendObjectToClient(object obj, Socket client)
                {
                        ByteBuffer buffer = new ByteBuffer();
                        byte[] result = buffer.ToBytes(obj);
                        client.Send(result);
                }

                private static void SendObjectToRoom(object obj, RoomInfo roomInfo)
                {
                        foreach (string guestAddress in roomInfo.GuestsAddressAndName.Keys)
                        {
                                foreach (ClientInfo client in clientsInfo)
                                {
                                        if (client.Socket.RemoteEndPoint.ToString() == guestAddress)
                                        {
                                                SendObjectToClient(obj, client.Socket); // tell all clients who in room
                                        }
                                }
                        }
                }

                private static void SendObjectWithoutSender(object obj, ClientInfo sender)
                {
                        if (clientsInfo.Count != 0)
                        {
                                foreach (ClientInfo clientInfo in clientsInfo)
                                {
                                        if (clientInfo != sender)
                                        {
                                                SendObjectToClient(obj, clientInfo.Socket);
                                        }
                                }
                        }
                }

                // recieve message from client
                private static void ReceiveMsgFromClient(object clientInfo)
                {
                        Socket mClientSocket = ((ClientInfo)clientInfo).Socket;
                        while (true)
                        {
                                try
                                {
                                        if (!IsSocketConnected(mClientSocket))
                                        {
                                                MessageHandle("exit", (ClientInfo)clientInfo);
                                        }
                                        mClientSocket.Receive(result); // receive message to result
                                        object obj = new ByteBuffer().GetObject(result);
                                        if (obj != null)
                                        {
                                                try
                                                {
                                                        PlayerAction playerAction = (PlayerAction)obj;
                                                        SendObjectWithoutSender(playerAction, (ClientInfo)clientInfo);
                                                }
                                                catch
                                                {
                                                        try
                                                        {
                                                                RoomInfo newRoom = (RoomInfo)obj;
                                                                roomsInfo.Add(newRoom);
                                                                SendObjectToClient(newRoom, mClientSocket);
                                                                continue;
                                                        }
                                                        catch
                                                        {
                                                                PrintLineWarning("Get an incorrect object");
                                                        }
                                                }
                                        }
                                        else
                                        {
                                                string message = new ByteBuffer(result).GetString().Trim().ToLower(); // analysis data content to string from result
                                                MessageHandle(message, (ClientInfo)clientInfo);
                                        }
                                }
                                catch (Exception ex)
                                {
                                        PrintLineWarning("" + ex.Message.Replace("\n", "\n<Warning> "));
                                        PrintLineInfo("<{0}> has left the game.", ((ClientInfo)clientInfo).PlayerName);
                                        if (mClientSocket != null)
                                        {
                                                mClientSocket.Close();
                                        }
                                        clientsInfo.Remove((ClientInfo)clientInfo);
                                        break;
                                }
                        }
                }

                private static void MessageHandle(string message, ClientInfo clientInfo)
                {
                        if (message.StartsWith("name="))
                        {
                                clientInfo.PlayerName = message.Remove(0, 5);
                                PrintLineInfo("The client {0} named \"{1}\" entered the game.", clientInfo.Socket.RemoteEndPoint.ToString(), clientInfo.PlayerName);
                                return;
                        }
                        if (message.StartsWith("enter="))
                        {
                                ClientEnterRoom(message.Remove(0, 6), clientInfo);
                                return;
                        }
                        if (message.StartsWith("leave="))
                        {
                                ClientLeaveRoom(message.Remove(0, 6), clientInfo);
                                return;
                        }
                        if (message.StartsWith("start="))
                        {
                                ClientStartRoom(message.Remove(0, 6), clientInfo);
                                return;
                        }
                        switch (message)
                        {
                                case "getrooms":
                                        SendObjectToClient(roomsInfo, clientInfo.Socket);
                                        break;
                                case "exit":
                                        clientInfo.Socket.Close();
                                        clientsInfo.Remove((ClientInfo)clientInfo);
                                        break;
                                default:
                                        PrintLineInfo("From client {0}: {1}", clientInfo.Socket.RemoteEndPoint.ToString(), message);
                                        break;
                        }
                }

                private static void ClientEnterRoom(string address, ClientInfo clientInfo)
                {
                        RoomInfo roomInfo = FindRoomInfoByAddress(address);
                        if (roomInfo == null)
                        {
                                SendMsgToClient("房间不存在！", clientInfo.Socket);
                                return;
                        }
                        if (!roomInfo.IsStart)
                        {
                                if (roomInfo.GuestsAddressAndName.Count < roomInfo.MaxPlayer)
                                {
                                        roomInfo.GuestsAddressAndName.Add(clientInfo.Socket.RemoteEndPoint.ToString(), clientInfo.PlayerName);
                                        SendObjectToRoom(roomInfo, roomInfo);
                                }
                                else
                                {
                                        SendMsgToClient("房间人数已满！", clientInfo.Socket);
                                }
                        }
                        else
                        {
                                SendMsgToClient("游戏已开始！", clientInfo.Socket);
                        }
                }

                private static void ClientStartRoom(string address, ClientInfo clientInfo)
                {
                        RoomInfo roomInfo = FindRoomInfoByAddress(address);
                        if (roomInfo == null)
                        {
                                SendMsgToClient("房间不存在！", clientInfo.Socket);
                                return;
                        }
                        if (roomInfo.GuestsAddressAndName.Count < 2)
                        {
                                SendMsgToClient("房间人数不足2人，无法开始游戏！", clientInfo.Socket);
                        }
                        else
                        {
                                roomInfo.IsStart = true;
                                SendIndexToRoom(roomInfo);
                                string startMessage = "start=";
                                int[] randomPosition = GenerateRandomPosition();
                                int i = 0;
                                foreach (string player in roomInfo.GuestsAddressAndName.Values)
                                {
                                        startMessage += (";" + player + "=" + randomPosition[i++].ToString());
                                }
                                SendMsgToRoom(startMessage, roomInfo);
                        }
                }

                private static void ClientLeaveRoom(string address, ClientInfo clientInfo)
                {
                        RoomInfo roomInfo = FindRoomInfoByAddress(address);
                        if (roomInfo == null)
                        {
                                SendMsgToClient("房间不存在！", clientInfo.Socket);
                                return;
                        }
                        roomInfo.GuestsAddressAndName.Remove(clientInfo.Socket.RemoteEndPoint.ToString());
                        // if player who is owner of the room left the game
                        if (roomInfo.OwnerAddress == clientInfo.Socket.RemoteEndPoint.ToString())
                        {
                                SendMsgToRoom("ownerleft", roomInfo);
                                roomsInfo.Remove(roomInfo);
                        }
                        else
                        {
                                // if player who is guest left the game, then send roomInfo to other players
                                SendObjectToRoom(roomInfo, roomInfo);
                        }
                }

                private static RoomInfo FindRoomInfoByAddress(string address)
                {
                        foreach (RoomInfo roomInfo in roomsInfo)
                        {
                                if (roomInfo.OwnerAddress == address)
                                {
                                        return roomInfo;
                                }
                        }
                        return null;
                }

                private static int[] GenerateRandomPosition()
                {
                        int[] array = new int[10];
                        Random random = new Random();
                        for (int i = 0; i < array.Length; i++)
                        {
                                array[i] = random.Next(1, 10);

                                for (int j = 0; j < i; j++)
                                {
                                        while (array[j] == array[i])
                                        {
                                                j = 0;
                                                array[i] = random.Next(0, 10);
                                        }
                                }
                        }
                        return array;
                }

                #endregion

                #region -- Output Style --

                private static void PrintDebug(string info)
                {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write("<Debug> " + info);
                        Console.ResetColor();
                }

                private static void PrintLineDebug(string info)
                {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.WriteLine("<Debug> " + info);
                        Console.ResetColor();
                }

                private static void PrintInfo(string info)
                {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("<Info> " + info);
                        Console.ResetColor();
                }

                private static void PrintInfo(string info, object arg0)
                {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("<Info> " + info, arg0);
                        Console.ResetColor();
                }

                private static void PrintInfo(string info, object arg0, object arg1)
                {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("<Info> " + info, arg0, arg1);
                        Console.ResetColor();
                }

                private static void PrintInfo(string info, object arg0, object arg1, object arg2)
                {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("<Info> " + info, arg0, arg1, arg2);
                        Console.ResetColor();
                }

                private static void PrintInfo(string info, object arg0, object arg1, object arg2, object arg3)
                {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("<Info> " + info, arg0, arg1, arg2, arg3);
                        Console.ResetColor();
                }

                private static void PrintInfo(string info, object arg0, object arg1, object arg2, object arg3, object arg4)
                {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("<Info> " + info, arg0, arg1, arg2, arg3, arg4);
                        Console.ResetColor();
                }

                private static void PrintInfo(string info, object arg0, object arg1, object arg2, object arg3, object arg4, object arg5)
                {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("<Info> " + info, arg0, arg1, arg2, arg3, arg4, arg5);
                        Console.ResetColor();
                }

                private static void PrintLineInfo(string info)
                {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("<Info> " + info);
                        Console.ResetColor();
                }

                private static void PrintLineInfo(string info, object arg0)
                {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("<Info> " + info, arg0);
                        Console.ResetColor();
                }

                private static void PrintLineInfo(string info, object arg0, object arg1)
                {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("<Info> " + info, arg0, arg1);
                        Console.ResetColor();
                }

                private static void PrintLineInfo(string info, object arg0, object arg1, object arg2)
                {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("<Info> " + info, arg0, arg1, arg2);
                        Console.ResetColor();
                }

                private static void PrintLineInfo(string info, object arg0, object arg1, object arg2, object arg3)
                {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("<Info> " + info, arg0, arg1, arg2, arg3);
                        Console.ResetColor();
                }

                private static void PrintLineInfo(string info, object arg0, object arg1, object arg2, object arg3, object arg4)
                {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("<Info> " + info, arg0, arg1, arg2, arg3, arg4);
                        Console.ResetColor();
                }

                private static void PrintLineInfo(string info, object arg0, object arg1, object arg2, object arg3, object arg4, object arg5)
                {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("<Info> " + info, arg0, arg1, arg2, arg3, arg4, arg5);
                        Console.ResetColor();
                }

                private static void PrintWarning(string warning)
                {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("<Warning> " + warning);
                        Console.ResetColor();
                }

                private static void PrintLineWarning(string warning)
                {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine("<Warning> " + warning);
                        Console.ResetColor();
                }

                private static void PrintError(string error)
                {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("<Error> " + error);
                        Console.ResetColor();
                }

                private static void PrintLineError(string error)
                {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("<Error> " + error);
                        Console.ResetColor();
                }

                #endregion
        }
}
