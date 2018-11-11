using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hasty {
    public partial class AddRepo : Form {
        public AddRepo() {
            InitializeComponent();
        }

        private async void btnAdd_Click(object sender, EventArgs e) {
            string url = txtUrl.Text;
            if (string.IsNullOrWhiteSpace(url))
                return;

            if (!url.StartsWith("http://"))
                url = "http://" + url;

            if (!url.EndsWith(":8090")) {
                if (url.EndsWith("/")) {
                    url = url.Substring(0, url.Length - 1);
                }
                url = url + ":8090";
            }


            string path = txtPath.Text;

            if (string.IsNullOrWhiteSpace(path)) {
                MessageBox.Show("You must provide an installation path for the mods", "Invalid path", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (!Directory.Exists(path)) {
                MessageBox.Show("Invalid folder path: " + path, "Invalid path", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            Tuple<Repo, Exception> urlResult = await Web.ReadUrlAsync(url);

            Repo repo = urlResult.Item1;

            if (repo == null) {
                MessageBox.Show($"Failed to read the repository url\nError: {urlResult.Item2}", "Adding repo failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            repo.Url = url;
            repo.LastCheck = Misc.UnixTime;
            repo.LastUpdate = Misc.UnixTime;
            repo.Folder = txtPath.Text;

            Settings s = Files.GetSettings();
            s.Repos.Add(repo);
            Files.SetSettings(s);

            MessageBox.Show("Repository added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e) {
            this.Close();
        }

        private void btnFileBrowser_Click(object sender, EventArgs e) {
            FolderBrowserDialog diag = new FolderBrowserDialog();

            DialogResult result = diag.ShowDialog();

            if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(diag.SelectedPath)) {

                string path = diag.SelectedPath;
                if (!Directory.Exists(path))
                    return;

                txtPath.Text = path;
            }

        }

        private void topPanel_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                Win32.ReleaseCapture();
                Win32.SendMessage(Handle, 0xA1, 0x2, 0);
            }
        }
    }
}
