using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HastyServer {
    class TcpCommunication {

        static int _port = 6969;
        static int _clientsConnected = 0;
        //static FileStorage _fileStorage = new FileStorage();
        static int _sleepTime = 1;

        public static int BufferSize { get; private set; } = 10240;

        private static void HandleClient(object clientObj) {
            try { 
                
                TcpClient client = (TcpClient)clientObj;

                NetworkStream stream = client.GetStream();

                _clientsConnected++;

                byte[] sendingBuffer = null;

                while (stream.CanRead) {
                    Thread.Sleep(_sleepTime);

                    byte[] buff = new byte[512];
                    int read = stream.Read(buff, 0, 512);
                    if (read == 0)
                        continue;

                    string command = Encoding.UTF8.GetString(buff);
                    command = command.Replace("\0", "");

                    string[] parts = command.Split('*');

                    if (parts.Length != 2) {
                        stream.Close();
                        client.Close();
                        return;
                    }

                    if (parts[0] == "recv") {
                        string file = parts[1];

                        if (!file.StartsWith("mod/"))
                            file = "mod/" + file;

                        if (!File.Exists(file) && !file.Contains("..") && !file.Contains(":")) {
                            return;
                        }

                        Stream fileStream = null; //_fileStorage.ReadFile(file);

                        int NoOfPackets = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(fileStream.Length) / Convert.ToDouble(BufferSize)));
                        int TotalLength = (int)fileStream.Length, CurrentPacketLength;

                        Stopwatch sw = new Stopwatch();

                        for (int i = 0; i < NoOfPackets; i++) {
                            if (TotalLength > BufferSize) {
                                CurrentPacketLength = BufferSize;
                                TotalLength = TotalLength - CurrentPacketLength;
                            } else
                                CurrentPacketLength = TotalLength;

                            

                            sendingBuffer = new byte[CurrentPacketLength];
                            fileStream.Read(sendingBuffer, 0, CurrentPacketLength);
                            sw.Reset();
                            sw.Start();
                            stream.Write(sendingBuffer, 0, (int)sendingBuffer.Length);
                            sw.Stop();
                            Console.WriteLine("MS: " + sw.ElapsedMilliseconds);
                            Thread.Sleep(_sleepTime);
                        }

                        fileStream.Close();
                        
                        byte[] fileEnded = Encoding.UTF8.GetBytes("*file_end*");
                        stream.Write(fileEnded, 0, (int)fileEnded.Length);

                        stream.Close();
                        client.Close();
                        _clientsConnected--;
                    }
                }
            }catch (Exception ex) {
                Console.WriteLine("Client exception: " + ex);
                _clientsConnected--;
            }
        }

        public static void StartListener() {

            Settings settings = Settings.ReadSettings();
        


            TcpListener listener = new TcpListener(IPAddress.Any, _port);
            listener.Start();

            while (true) {
                try {
                    if (listener.Pending()) {
                        TcpClient client = listener.AcceptTcpClient();
                        Thread t = new Thread(new ParameterizedThreadStart(HandleClient));
                        t.Start(client);
                    } else {
                        Thread.Sleep(50);
                    }
                } catch (Exception ex) {
                    Console.WriteLine("Exception: " + ex);
                }
            }
        }
    }
}
