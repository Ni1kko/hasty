using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hasty {
    class Ftp {
        public static bool Cancel { get; set; } = false;
        public static async Task<bool> RequestFile(Repo repo, string file, string savePath, long fileLength, Action<long> progress = null) {

            RestartDownload:

            try {
                string ftpUrl = repo.SocketConnection;
                if (!ftpUrl.StartsWith("ftp://"))
                    ftpUrl = "ftp://" + ftpUrl;

                FtpWebRequest ftp = (FtpWebRequest)WebRequest.Create(ftpUrl + "/" + file);
                ftp.Method = WebRequestMethods.Ftp.DownloadFile;

                ftp.Credentials = new NetworkCredential("anonymous", "");
                FtpWebResponse res;

                try {
                    res = (FtpWebResponse)ftp.GetResponse();
                }
                catch {
                    // timeout, this is gonna be true aids
                    goto RestartDownload;
                }

                Stream stream = res.GetResponseStream();
                

                FileStream fileStream = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.Write);

                int chunkSize = 20480;
                long streamPosition = 0;
                while (true) {
                    byte[] buffer = new byte[Math.Min(chunkSize, fileLength - streamPosition)];

                    int readBytes = await stream.ReadAsync(buffer, 0, buffer.Length);

                    if (readBytes == 0)
                        break;

                    streamPosition += readBytes; // for tracking response stream position

                    fileStream.Write(buffer, 0, readBytes);

                    if (progress != null) {
                        progress(streamPosition);
                    }
                    if (Cancel) {
                        stream.Close();
                        fileStream.Close();
                        return false;
                    }
                }



                //await stream.CopyToAsync(fileStream);

                stream.Close();
                fileStream.Close();
            } catch (Exception ex) {
                MessageBox.Show("File download failed: " + ex, "An error occured :(", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }
    }
}
