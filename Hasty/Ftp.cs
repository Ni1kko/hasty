using FluentFTP;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hasty {
    class Ftp {
        public static bool Cancel { get; set; } = false;
        public static async Task<bool> RequestFile(Repo repo, string file, string savePath, long fileLength, Action<long> progress = null) {
            try {
                if (Cancel)
                    return false;

                string ftpUrl = repo.SocketConnection;
                if (ftpUrl.StartsWith("ftp://"))
                    ftpUrl = ftpUrl.Replace("ftp://", "");

                ftpUrl = ftpUrl.Replace("/", "");

                string[] parts = ftpUrl.Split(':');
                if (parts.Length != 2)
                    throw new Exception("Invalid IP, port number not defined or is incorrect: " + ftpUrl);

                FtpClient client = new FtpClient(parts[0], int.Parse(parts[1]), "anonymous", "");

                await client.ConnectAsync();

                await client.DownloadFileAsync(savePath, file, true, FtpVerify.None, new Progress<double>(async x => {
                    progress((long)x);

                    if (Cancel) {
                        await client.DisconnectAsync();
                    }
                }));


            } catch (Exception ex) {
                MessageBox.Show("File download failed: " + ex, "An error occured :(", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }
    }
}
