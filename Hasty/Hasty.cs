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
            new AddRepo().Show();
        }

        private void listRepo_SelectedIndexChanged(object sender, EventArgs e) {
            int index = listRepo.SelectedIndex;

            if (index == -1 || index >= _repos.Count)
                return;

            UpdateSelected(index);
        }

        private async void btnUpdate_Click(object sender, EventArgs e) {
            if (_selected == null)
                return;

            Tuple<Repo, Exception> res = await Web.ReadUrlAsync(_selected.Url);
            Repo repo = res.Item1;
            if (repo == null) {
                MessageBox.Show("Error updating repository: " + res.Item2.Message, "An error occured :(", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            if (repo != _selected) {
                _repos.Remove(_selected);
                _repos.Add(repo);

                Files.UpdateRepos(_repos);
            }

            // start file uploads and stuff
        }
    }
}
