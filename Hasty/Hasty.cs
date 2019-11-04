using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Hasty {
    public partial class HastyForm : Form {

        List<Repo> _repos = new List<Repo>();
        Repo _selected = null;
        int _selectedIndex = 0;

        public HastyForm() {
            InitializeComponent();
            Files.Init();
            UpdateRepos(); 
        }

        private void UpdateRepos() {
            try {
                listRepo.Items.Clear();

                var s = Files.GetSettings(); 
                _repos = s.Repos;

                if (_repos.Count == 0) {
                    listRepo.Items.Add("No Repos Found...");
                } else {
                    foreach (var r in _repos)
                    {
                        listRepo.Items.Add(r.Name);
                    }

                    listRepo.SelectedIndex = _selectedIndex;
                    UpdateSelected(_selectedIndex);
                }
            }
            catch(Exception ex) {
                MessageBox.Show("Invalid config file?\n" + ex.ToString(), "An error occured :(", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdateSelected(int index = 0) {
            try {
                var repo = _repos[index];
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
            if (e.Button != MouseButtons.Left) return;
            Win32.ReleaseCapture();
            Win32.SendMessage(Handle, 0xA1, 0x2, 0);
        }

        private void btnMinimize_Click(object sender, EventArgs e) {
            WindowState = FormWindowState.Minimized;
        }

        private void btnExit_Click(object sender, EventArgs e) {
            Environment.Exit(0);
        }

        private void btnNewRepo_Click(object sender, EventArgs e) {
            var window = new AddRepo();
            window.Show();
            window.FormClosed += new FormClosedEventHandler((object cSender, FormClosedEventArgs cEvent) => {
                UpdateRepos();
            });
        }

        private void listRepo_SelectedIndexChanged(object sender, EventArgs e) {
            var index = listRepo.SelectedIndex;

            if (index == -1 || index >= _repos.Count)
                return;

            _selectedIndex = index;

            UpdateSelected(_selectedIndex);
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

            var p1 = Process.GetProcessesByName("arma3_x64");
            var p2 = Process.GetProcessesByName("arma3");
            if (p1.Length > 0 || p2.Length > 0) {
                var dialogRes =  MessageBox.Show("Arma 3 is running, this may cause errors. Do you want to continue?", "Arma 3 is running", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (dialogRes == DialogResult.No)
                    return;
            }


            ButtonsEnable(false);

            var res = await Web.ReadUrlAsync(_selected.Url);
            var repo = res.Item1;
            if (repo == null) {
                MessageBox.Show("Error updating repository: " + res.Item2.Message, "An error occured :(", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ButtonsEnable(true);
                return;
            }


            if (repo != _selected) {
                repo.LastCheck = Misc.UnixTime;
                repo.LastUpdate = _selected.LastUpdate;

                repo = Files.Update(_selected, repo);

                var index = _repos.IndexOf(_selected);
                _repos[index] = repo;

                Files.UpdateRepos(_repos);
                UpdateRepos();
            }

            // start file downloads and stuff
            var filesResponse = await Web.ReadRepo(repo.RemoteFolder);

            var files = filesResponse.Item1;
            if (files == null) {
                MessageBox.Show("Error downloading repository info: " + filesResponse.Item2.Message, "An error occured :(", MessageBoxButtons.OK, MessageBoxIcon.Error);
                ButtonsEnable(true);
                return;
            }

            _filesHandled = 0;
            _totalFiles = files.totalFiles;

            progressTotal.Visible = true;
            labCurrentFile.Visible = true;
            labProcessed.Visible = true;

            var success = await WalkFolder(repo, files);

            // make sure all the paths are rooted
            for (var i = 0; i < _allFiles.Count; i++) {
                _allFiles[i] = Path.GetFullPath(_allFiles[i]);
            }


            // delete unwanted files
            var currentFiles = Directory.GetFiles(repo.Folder + "/" + repo.FolderName, "*", SearchOption.AllDirectories);
            foreach (var s in currentFiles) {
                var full = Path.GetFullPath(s);

                if (!_allFiles.Contains(full)) {
                    File.Delete(full);
                }
            }

            // wait for all downloads to finish
            while (_activeThreads > 0) await Task.Delay(50);

            // clean up a bit (remove active text)
            if (success) labProcessed.Text = $"Processed: {_filesHandled}/{_totalFiles}";


            if (_updated > 0 && success) {

                var index = _repos.IndexOf(_selected);
                repo.LastUpdate = Misc.UnixTime;
                _repos[index] = repo;

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
            if (_selected == null)  return;

            var res = MessageBox.Show("Are you sure you want to remove this repository?", "Remove repo?", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
            if (res != DialogResult.Yes) return;
            _repos.Remove(_selected);
            Files.UpdateRepos(_repos);
            UpdateRepos();
        }

        private bool _cancel = false;
        private int _activeThreads = 0;
        private Dictionary<string, double> _activeDownloads = new Dictionary<string, double>();

        private List<string> _allFiles = new List<string>();

        private async Task<bool> WalkFolder(Repo repo, DynatreeItem folder, string path = "/") {
            var repoFolder = repo.Folder + "/" + repo.FolderName;

            if (!Directory.Exists(repoFolder)) Directory.CreateDirectory(repoFolder);

            foreach(var item in folder.children) {
                if (_cancel)
                    return false;

                while (_activeThreads >= 5)
                    await Task.Delay(50);

                if (item.isFolder) {
                    var folderPath = path + "/" + item.title;
                    if (!Directory.Exists(repoFolder + "/" + folderPath))
                        Directory.CreateDirectory(repoFolder + "/" + folderPath);


                    if (!await WalkFolder(repo, item, folderPath))
                        return false;
                } else {
                    var remotePath = path + "/" + item.title;
                    var filePath = repoFolder + "/" + remotePath;

                    _allFiles.Add(filePath);

                    var totalValue = (((float)_filesHandled / (float)_totalFiles) * 100);
                    if (totalValue <= progressTotal.MaximumValue)
                        progressTotal.Value = (int)totalValue;

                    var fileTitle = item.title;
                    if (fileTitle.Length > 18)
                        fileTitle = fileTitle.Substring(0, 18) + "...";
                    labCurrentFile.Text = "Current File: " + fileTitle;

                    if (File.Exists(filePath)) {
                        var checkSum = await Files.CheckSum(filePath);
                        if (checkSum == item.hash) {
                            labProcessed.Text = $"Processed: {_filesHandled}/{_totalFiles} (Active: {_activeThreads})";
                            _filesHandled++;
                            continue;
                        }
                    }

                    _updated++;
                    var lastPercent = 0;

                    //if (_progresses.ContainsKey(filePath))
                        //continue;

                   var t = new Thread(async () => {
                        _activeThreads++;
                        var result = await Ftp.RequestFile(repo, remotePath, filePath, item.fileSize, (progress) => {
                            var percentCompleted = (int)progress.Progress;

                            if (_cancel)
                                Ftp.Cancel = true;

                            if (percentCompleted <= lastPercent)
                                return;

                            lastPercent = percentCompleted;
                            _activeDownloads[filePath] = percentCompleted;

                            Invoke((MethodInvoker)delegate {

                                labProcessed.Text = $"Processed: {_filesHandled}/{_totalFiles} (Active: {_activeDownloads.Count})";

                                var tip = "";
                                for (var index = 0; index < _activeDownloads.ToList().Count; index++)
                                {
                                    var kv = _activeDownloads.ToList()[index];
                                    tip += Path.GetFileName(kv.Key) + ": " + kv.Value + "%\n";
                                }

                                toolTip.SetToolTip(labProcessed, tip);
                            });  
                        });
                        if (!result)
                            _cancel = true;

                        _filesHandled++;
                        _activeThreads--;
                        _activeDownloads.Remove(filePath);

                    });
                    t.Start();

                    // avoid too many threads starting? slow start?
                    await Task.Delay(5); 
                }
            }
            return true;
        }
    }
}
