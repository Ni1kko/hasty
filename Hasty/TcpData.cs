using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hasty {
    class TcpData {
        static int _bufferSize = 10240;
        static int _port = 6969;

        public static async Task<bool> RequestFile(Repo repo, string file, string savePath, Action<long> progress = null) {
            byte[] recData = new byte[_bufferSize];
            int recBytes;

            try {
                TcpClient client = new TcpClient(repo.SocketConnection, _port);
                if (!client.Connected) {
                    return false;
                }

                NetworkStream stream = client.GetStream();
                byte[] data = Encoding.UTF8.GetBytes("recv*" + file);
                stream.Write(data, 0, data.Length);

                long totalrecbytes = 0;
                FileStream fileStream = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.Write);
                while ((recBytes = await stream.ReadAsync(recData, 0, recData.Length)) > 0) {
                    await fileStream.WriteAsync(recData, 0, recBytes);
                    totalrecbytes += recBytes;

                    if (progress != null) {
                        progress(totalrecbytes);
                    }
                    Thread.Sleep(1);
                }
                fileStream.Close();
                stream.Close();
                client.Close();

                return true;
            } catch (Exception ex) {
                MessageBox.Show("An error downloading files: " + ex, "An error occured", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        //public static void ReceiveTCP() {
        //    TcpListener Listener = null;
        //    try {
        //        Listener = new TcpListener(IPAddress.Any, _port);
        //        Listener.Start();
        //    } catch (Exception ex) {
        //        Console.WriteLine(ex.Message);
        //    }

            

        //    for (; ; )
        //    {
        //        TcpClient client = null;
        //        NetworkStream netstream = null;
        //        try {

        //            if (Listener.Pending()) {
        //                client = Listener.AcceptTcpClient();
        //                netstream = client.GetStream();

        //                Console.WriteLine("Starting to receive file...");

        //                string SaveFileName = string.Empty;

        //                SaveFileName = @"C:\Users\eelis\Documents\GitHub\hasty\HastyServer\bin\Debug\test\largefile.dat";

        //                int totalrecbytes = 0;
        //                FileStream Fs = new FileStream(SaveFileName, FileMode.OpenOrCreate, FileAccess.Write);
        //                while ((RecBytes = netstream.Read(RecData, 0, RecData.Length)) > 0) {
        //                    Fs.Write(RecData, 0, RecBytes);
        //                    totalrecbytes += RecBytes;
        //                }
        //                Fs.Close();

        //                netstream.Close();
        //                client.Close();

        //            } else
        //                Thread.Sleep(50);
        //        } catch (Exception ex) {
        //            Console.WriteLine(ex.Message);
        //            //netstream.Close();
        //        }
        //    }
        //}
    }
}
