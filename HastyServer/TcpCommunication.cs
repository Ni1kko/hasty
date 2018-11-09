using System;
using System.Collections.Generic;
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
        static int _bufferSize = 10240;

        public static int BufferSize {
            get {
                return _bufferSize;
            };
        }

        private static void HandleClient(object clientObj) {
            try { 
            TcpClient client = (TcpClient)clientObj;

            NetworkStream stream = client.GetStream();

            byte[] sendingBuffer = null;

                if (stream.CanRead) {
                    byte[] buff = new byte[512];
                    stream.Read(buff, 0, 512);

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

                        FileStream fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);

                        int NoOfPackets = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(fileStream.Length) / Convert.ToDouble(_bufferSize)));
                        int TotalLength = (int)fileStream.Length, CurrentPacketLength;

                        for (int i = 0; i < NoOfPackets; i++) {
                            if (TotalLength > _bufferSize) {
                                CurrentPacketLength = _bufferSize;
                                TotalLength = TotalLength - CurrentPacketLength;
                            } else
                                CurrentPacketLength = TotalLength;


                            sendingBuffer = new byte[CurrentPacketLength];
                            fileStream.Read(sendingBuffer, 0, CurrentPacketLength);
                            stream.Write(sendingBuffer, 0, (int)sendingBuffer.Length);
                            Thread.Sleep(1);
                        }

                        fileStream.Close();
                        stream.Close();
                        client.Close();
                    }
                }
            }catch (Exception ex) {
                Console.WriteLine("Client exception: " + ex);
            }
        }

        public static void StartListener() {

            Settings settings = Settings.ReadSettings();

            // dank maths (50 mbits)
            int maxSpeed = settings.MaxSpeed;

            if (maxSpeed <= 0)
            {
                // cap to 10 megbytes a sec by default (80 mbits)
                _bufferSize = 10240;
            }
            else
            {
                maxSpeed *= 1000000; // mbits to bits
                maxSpeed /= 8; // bits to bytes
                               // 50 * 1 000 000, / 8, = 6 250 000
                _bufferSize = maxSpeed / 1000; // devivded by 1000 because 1000 packets a sec
            }
        


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
