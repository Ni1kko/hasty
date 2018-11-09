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
using System.Diagnostics;

namespace Hasty {
    class TcpData {
        public static int BufferSize { get; set; } = 10240;
        static int _port = 6969;

        public static async Task<bool> RequestFile(Repo repo, string file, string savePath, Action<long, double> progress = null) {
            byte[] recData = new byte[BufferSize];
            int recBytes;

            Stopwatch stopWatch = new Stopwatch();

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
                do {
                    stopWatch.Reset();
                    stopWatch.Start();

                    recBytes = await stream.ReadAsync(recData, 0, recData.Length);
                    Thread.Sleep(1);

                    stopWatch.Stop();

                    await fileStream.WriteAsync(recData, 0, recBytes);
                    totalrecbytes += recBytes;

                    if (progress != null) {
                        double milliSeconds = stopWatch.ElapsedMilliseconds;
                        double speed = recBytes / milliSeconds;
                        progress(totalrecbytes, speed);
                    }
                } while ((recBytes) > 0);

                fileStream.Close();
                stream.Close();
                client.Close();

                return true;
            } catch (Exception ex) {
                MessageBox.Show("An error downloading files: " + ex, "An error occured", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

    }
}
