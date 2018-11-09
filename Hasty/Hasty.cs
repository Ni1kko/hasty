using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hasty {
    public partial class HastyForm : Form {

        List<Repo> _repos = new List<Repo>();
        Repo _selected = null;

        public HastyForm() {
            InitializeComponent();
            Files.Init();
            UpdateRepos();

        }

        private void UpdateRepos() {
            try {
                listRepo.Items.Clear();

                Settings s = Files.GetSettings();

                _repos = s.Repos;

                if (_repos.Count == 0) {
                    listRepo.Items.Add("No Repos Found...");
                } else {
                    for (int i = 0; i < _repos.Count; i++) {
                        Repo r = _repos[i];
                        listRepo.Items.Add(r.Name);
                    }

                    listRepo.SelectedIndex = 0;
                    UpdateSelected();
                }
            }
            catch(Exception ex) {
                MessageBox.Show("Invalid config file?\n" + ex.ToString(), "An error occured :(", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateSelected(int index = 0) {
            try {
                Repo repo = _repos[index];
                labName.Text = "Name: " + repo.Name;
                labChecked.Text = "Last Checked: " + Misc.UnixToString(repo.LastCheck);
                labUpdated.Text = "Last Updated: " + Misc.UnixToString(repo.LastUpdate);

                _selected = repo;
            }
            catch(Exception ex) {
                MessageBox.Show(ex.ToString(), "An error occured :(", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void topBar_MouseDown(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                Win32.ReleaseCapture();
                Win32.SendMessage(Handle, 0xA1, 0x2, 0);
            }
        }

        private void btnMinimize_Click(object sender, EventArgs e) {
            WindowState = FormWindowState.Minimized;
        }

        private void btnExit_Click(object sender, EventArgs e) {
            Environment.Exit(0);
        }

        private void btnNewRepo_Click(object sender, EventArgs e) {
            AddRepo window = new AddRepo();
            window.Show();
            window.FormClosed += new FormClosedEventHandler((object cSender, FormClosedEventArgs cEvent) => {
                UpdateRepos();
            });
        }

        private void listRepo_SelectedIndexChanged(object sender, EventArgs e) {
            int index = listRepo.SelectedIndex;

            if (index == -1 || index >= _repos.Count)
                return;

            UpdateSelected(index);
        }

        private void ButtonsEnable(bool enable) {
            btnUpdate.Enabled = enable;
            btnRemove.Enabled = enable;
            btnNewRepo.Enabled = enable;
        }

        int _filesHandled = 0, _totalFiles = 0;
        int _updated = 0;
        private async void btnUpdate_Click(object sender, EventArgs e) {
            if (_selected == null)
                return;

            ButtonsEnable(false);

            Tuple<Repo, Exception> res = await Web.ReadUrlAsync(_selected.Url);
            Repo repo = res.Item1;
            if (repo == null) {
                MessageBox.Show("Error updating repository: " + res.Item2.Message, "An error occured :(", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ButtonsEnable(true);
                return;
            }

            TcpData.BufferSize = repo.BufferSize;

            if (repo != _selected) {
                repo.LastCheck = Misc.UnixTime;
                repo.LastUpdate = _selected.LastUpdate;

                repo = Files.Update(_selected, repo);

                _repos.Remove(_selected);
                _repos.Add(repo);

                Files.UpdateRepos(_repos);
                UpdateRepos();
            }

            // start file downloads and stuff
            Tuple<DynatreeItem, Exception> filesResponse = await Web.ReadRepo(repo.RemoteFolder);

            DynatreeItem files = filesResponse.Item1;
            if (files == null) {
                MessageBox.Show("Error downloading repository info: " + filesResponse.Item2.Message, "An error occured :(", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ButtonsEnable(true);
                return;
            }

            _filesHandled = 0;
            _totalFiles = files.totalFiles;

            progressTotal.Visible = true;
            progressFile.Visible = true;
            labCurrentFile.Visible = true;
            labProcessed.Visible = true;

            bool success = await WalkFolder(repo, files);

            if (_updated > 0 && success) {

                bool removed = _repos.Remove(_selected);
                repo.LastUpdate = Misc.UnixTime;
                _repos.Add(repo);

                Files.UpdateRepos(_repos);
                UpdateRepos();
                MessageBox.Show($"Updating files finished.\nTotal files checked: {_totalFiles}, files updated: {_updated}", "Update Finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
                _updated = 0;
            } else if (!success) {
                MessageBox.Show($"Failed to update repository", "Update Failed", MessageBoxButtons.OK, MessageBoxIcon.Information);
            } else { 
                MessageBox.Show("No new updates found.", "Update Finished", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            ButtonsEnable(true);

        }

        private void btnRemove_Click(object sender, EventArgs e) {
            if (_selected == null)
                return;

            DialogResult res = MessageBox.Show("Are you sure you want to remove this repository?", "Remove repo?", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (res == DialogResult.Yes) {
                _repos.Remove(_selected);
                Files.UpdateRepos(_repos);
                UpdateRepos();
            }
        }

        private async Task<bool> WalkFolder(Repo repo, DynatreeItem folder, string path = "/") {
            string repoFolder = repo.Folder + "/" + repo.FolderName;
            foreach(DynatreeItem item in folder.children) {
                if (item.isFolder) { 
                    string folderPath = path + "/" + item.title;
                    if (!Directory.Exists(repoFolder + "/" + folderPath))
                        Directory.CreateDirectory(repoFolder + "/" + folderPath);


                    if (!await WalkFolder(repo, item, folderPath))
                        return false;
                } else {
                    string remotePath = path + "/" + item.title;
                    string filePath = repoFolder + "/" + remotePath;

                    _filesHandled++;

                    float totalValue = (((float)_filesHandled / (float)_totalFiles) * 100);
                    progressTotal.Value = (int)totalValue;

                    string fileTitle = item.title;
                    if (fileTitle.Length > 18)
                        fileTitle = fileTitle.Substring(0, 18) + "...";
                    labCurrentFile.Text = "Current File: " + fileTitle;

                    if (File.Exists(filePath)) {
                        string checkSum = await Files.CheckSum(filePath);
                        if (checkSum == item.hash) {
                            labProcessed.Text = $"Processed: {_filesHandled}/{_totalFiles} (100%)";
                            continue;
                        }
                    }

                    _updated++;
                    int lastPercent = 0;

                    bool res = await Task.Run(async () => {
                        bool result = await TcpData.RequestFile(repo, remotePath, filePath, (long progress, double speed) => {
                            int percCompleted = (int)(((float)progress / (float)item.fileSize) * 100);

                            if (percCompleted <= lastPercent)
                                return;

                            lastPercent = percCompleted;

                            speed /= 1000;

                            Invoke((MethodInvoker)delegate {
                                progressFile.Value = (int)percCompleted;

                                if (speed != 0)
                                    labProcessed.Text = $"Files Processed: {_filesHandled}/{_totalFiles} ({(int)percCompleted}%, {Math.Round(speed, 1)} MB/s)";
                            });
                        });
                        return result;
                    });

                    
                    if (!res)
                        return false;

                }
            }
            return true;
        }
    }
}
