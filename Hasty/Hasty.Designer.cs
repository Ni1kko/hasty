namespace Hasty {
    partial class HastyForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.topBar = new System.Windows.Forms.Panel();
            this.btnMinimize = new System.Windows.Forms.Button();
            this.btnExit = new System.Windows.Forms.Button();
            this.labTitle = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.btnRemove = new Bunifu.Framework.UI.BunifuTileButton();
            this.labProcessed = new System.Windows.Forms.Label();
            this.progressFile = new Bunifu.Framework.UI.BunifuProgressBar();
            this.progressTotal = new Bunifu.Framework.UI.BunifuProgressBar();
            this.btnUpdate = new Bunifu.Framework.UI.BunifuTileButton();
            this.labUpdated = new System.Windows.Forms.Label();
            this.labChecked = new System.Windows.Forms.Label();
            this.labName = new System.Windows.Forms.Label();
            this.listRepo = new System.Windows.Forms.ListBox();
            this.btnNewRepo = new Bunifu.Framework.UI.BunifuTileButton();
            this.labCurrentFile = new System.Windows.Forms.Label();
            this.topBar.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // topBar
            // 
            this.topBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.topBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.topBar.Controls.Add(this.btnMinimize);
            this.topBar.Controls.Add(this.btnExit);
            this.topBar.Controls.Add(this.labTitle);
            this.topBar.Location = new System.Drawing.Point(-1, -2);
            this.topBar.Name = "topBar";
            this.topBar.Size = new System.Drawing.Size(656, 44);
            this.topBar.TabIndex = 0;
            this.topBar.MouseDown += new System.Windows.Forms.MouseEventHandler(this.topBar_MouseDown);
            // 
            // btnMinimize
            // 
            this.btnMinimize.BackColor = System.Drawing.Color.DimGray;
            this.btnMinimize.FlatAppearance.BorderSize = 0;
            this.btnMinimize.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnMinimize.ForeColor = System.Drawing.Color.White;
            this.btnMinimize.Location = new System.Drawing.Point(561, 5);
            this.btnMinimize.Name = "btnMinimize";
            this.btnMinimize.Size = new System.Drawing.Size(35, 34);
            this.btnMinimize.TabIndex = 4;
            this.btnMinimize.Text = "_";
            this.btnMinimize.UseVisualStyleBackColor = false;
            this.btnMinimize.Click += new System.EventHandler(this.btnMinimize_Click);
            // 
            // btnExit
            // 
            this.btnExit.BackColor = System.Drawing.Color.DimGray;
            this.btnExit.FlatAppearance.BorderSize = 0;
            this.btnExit.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExit.ForeColor = System.Drawing.Color.White;
            this.btnExit.Location = new System.Drawing.Point(608, 5);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(35, 34);
            this.btnExit.TabIndex = 3;
            this.btnExit.Text = "X";
            this.btnExit.UseVisualStyleBackColor = false;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // labTitle
            // 
            this.labTitle.AutoSize = true;
            this.labTitle.Location = new System.Drawing.Point(6, 10);
            this.labTitle.Name = "labTitle";
            this.labTitle.Size = new System.Drawing.Size(305, 28);
            this.labTitle.TabIndex = 0;
            this.labTitle.Text = "Hasty - ArmA 3 Repo Manager";
            // 
            // panel1
            // 
            this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel1.Controls.Add(this.labCurrentFile);
            this.panel1.Controls.Add(this.btnRemove);
            this.panel1.Controls.Add(this.labProcessed);
            this.panel1.Controls.Add(this.progressFile);
            this.panel1.Controls.Add(this.progressTotal);
            this.panel1.Controls.Add(this.btnUpdate);
            this.panel1.Controls.Add(this.labUpdated);
            this.panel1.Controls.Add(this.labChecked);
            this.panel1.Controls.Add(this.labName);
            this.panel1.Controls.Add(this.listRepo);
            this.panel1.Controls.Add(this.btnNewRepo);
            this.panel1.Location = new System.Drawing.Point(-1, 45);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(656, 529);
            this.panel1.TabIndex = 2;
            // 
            // btnRemove
            // 
            this.btnRemove.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnRemove.color = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnRemove.colorActive = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.btnRemove.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnRemove.Font = new System.Drawing.Font("Lucida Grande", 11F);
            this.btnRemove.ForeColor = System.Drawing.Color.White;
            this.btnRemove.Image = null;
            this.btnRemove.ImagePosition = 0;
            this.btnRemove.ImageZoom = 0;
            this.btnRemove.LabelPosition = 40;
            this.btnRemove.LabelText = "Remove Repo";
            this.btnRemove.Location = new System.Drawing.Point(297, 455);
            this.btnRemove.Margin = new System.Windows.Forms.Padding(0);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(346, 47);
            this.btnRemove.TabIndex = 13;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // labProcessed
            // 
            this.labProcessed.AutoSize = true;
            this.labProcessed.Location = new System.Drawing.Point(292, 251);
            this.labProcessed.Name = "labProcessed";
            this.labProcessed.Size = new System.Drawing.Size(206, 28);
            this.labProcessed.TabIndex = 12;
            this.labProcessed.Text = "Files Processed: 0/0";
            this.labProcessed.Visible = false;
            // 
            // progressFile
            // 
            this.progressFile.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.progressFile.BorderRadius = 15;
            this.progressFile.Location = new System.Drawing.Point(296, 326);
            this.progressFile.Margin = new System.Windows.Forms.Padding(7, 10, 7, 10);
            this.progressFile.MaximumValue = 100;
            this.progressFile.Name = "progressFile";
            this.progressFile.ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.progressFile.Size = new System.Drawing.Size(346, 17);
            this.progressFile.TabIndex = 11;
            this.progressFile.Value = 0;
            this.progressFile.Visible = false;
            // 
            // progressTotal
            // 
            this.progressTotal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.progressTotal.BorderRadius = 15;
            this.progressTotal.Location = new System.Drawing.Point(297, 360);
            this.progressTotal.Margin = new System.Windows.Forms.Padding(5, 7, 5, 7);
            this.progressTotal.MaximumValue = 100;
            this.progressTotal.Name = "progressTotal";
            this.progressTotal.ProgressColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.progressTotal.Size = new System.Drawing.Size(346, 17);
            this.progressTotal.TabIndex = 10;
            this.progressTotal.Value = 0;
            this.progressTotal.Visible = false;
            // 
            // btnUpdate
            // 
            this.btnUpdate.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnUpdate.color = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnUpdate.colorActive = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.btnUpdate.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnUpdate.Font = new System.Drawing.Font("Lucida Grande", 11F);
            this.btnUpdate.ForeColor = System.Drawing.Color.White;
            this.btnUpdate.Image = null;
            this.btnUpdate.ImagePosition = 0;
            this.btnUpdate.ImageZoom = 0;
            this.btnUpdate.LabelPosition = 40;
            this.btnUpdate.LabelText = "Update";
            this.btnUpdate.Location = new System.Drawing.Point(297, 396);
            this.btnUpdate.Margin = new System.Windows.Forms.Padding(0);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(346, 47);
            this.btnUpdate.TabIndex = 9;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // labUpdated
            // 
            this.labUpdated.AutoSize = true;
            this.labUpdated.Location = new System.Drawing.Point(292, 158);
            this.labUpdated.Name = "labUpdated";
            this.labUpdated.Size = new System.Drawing.Size(190, 28);
            this.labUpdated.TabIndex = 8;
            this.labUpdated.Text = "Last Updated: N/A";
            // 
            // labChecked
            // 
            this.labChecked.AutoSize = true;
            this.labChecked.Location = new System.Drawing.Point(292, 120);
            this.labChecked.Name = "labChecked";
            this.labChecked.Size = new System.Drawing.Size(191, 28);
            this.labChecked.TabIndex = 7;
            this.labChecked.Text = "Last Checked: N/A";
            // 
            // labName
            // 
            this.labName.AutoSize = true;
            this.labName.Location = new System.Drawing.Point(292, 82);
            this.labName.Name = "labName";
            this.labName.Size = new System.Drawing.Size(119, 28);
            this.labName.TabIndex = 6;
            this.labName.Text = "Name: N/A";
            // 
            // listRepo
            // 
            this.listRepo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.listRepo.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listRepo.ForeColor = System.Drawing.Color.White;
            this.listRepo.FormattingEnabled = true;
            this.listRepo.ItemHeight = 28;
            this.listRepo.Location = new System.Drawing.Point(11, 82);
            this.listRepo.Name = "listRepo";
            this.listRepo.Size = new System.Drawing.Size(275, 420);
            this.listRepo.TabIndex = 5;
            this.listRepo.SelectedIndexChanged += new System.EventHandler(this.listRepo_SelectedIndexChanged);
            // 
            // btnNewRepo
            // 
            this.btnNewRepo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnNewRepo.color = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.btnNewRepo.colorActive = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(70)))), ((int)(((byte)(70)))));
            this.btnNewRepo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnNewRepo.Font = new System.Drawing.Font("Lucida Grande", 11F);
            this.btnNewRepo.ForeColor = System.Drawing.Color.White;
            this.btnNewRepo.Image = null;
            this.btnNewRepo.ImagePosition = 0;
            this.btnNewRepo.ImageZoom = 0;
            this.btnNewRepo.LabelPosition = 40;
            this.btnNewRepo.LabelText = "New Repo";
            this.btnNewRepo.Location = new System.Drawing.Point(11, 20);
            this.btnNewRepo.Margin = new System.Windows.Forms.Padding(0);
            this.btnNewRepo.Name = "btnNewRepo";
            this.btnNewRepo.Size = new System.Drawing.Size(275, 47);
            this.btnNewRepo.TabIndex = 4;
            this.btnNewRepo.Click += new System.EventHandler(this.btnNewRepo_Click);
            // 
            // labCurrentFile
            // 
            this.labCurrentFile.AutoSize = true;
            this.labCurrentFile.Location = new System.Drawing.Point(292, 288);
            this.labCurrentFile.Name = "labCurrentFile";
            this.labCurrentFile.Size = new System.Drawing.Size(175, 28);
            this.labCurrentFile.TabIndex = 14;
            this.labCurrentFile.Text = "Current File: N/A";
            this.labCurrentFile.Visible = false;
            // 
            // HastyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 28F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(44)))), ((int)(((byte)(44)))), ((int)(((byte)(44)))));
            this.ClientSize = new System.Drawing.Size(651, 567);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.topBar);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Lucida Grande", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.SystemColors.Control;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HastyForm";
            this.ShowIcon = false;
            this.Text = "Form1";
            this.topBar.ResumeLayout(false);
            this.topBar.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel topBar;
        private System.Windows.Forms.Label labTitle;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnMinimize;
        private System.Windows.Forms.Panel panel1;
        private Bunifu.Framework.UI.BunifuTileButton btnNewRepo;
        private System.Windows.Forms.ListBox listRepo;
        private System.Windows.Forms.Label labChecked;
        private System.Windows.Forms.Label labName;
        private Bunifu.Framework.UI.BunifuProgressBar progressTotal;
        private Bunifu.Framework.UI.BunifuTileButton btnUpdate;
        private System.Windows.Forms.Label labUpdated;
        private Bunifu.Framework.UI.BunifuProgressBar progressFile;
        private System.Windows.Forms.Label labProcessed;
        private Bunifu.Framework.UI.BunifuTileButton btnRemove;
        private System.Windows.Forms.Label labCurrentFile;
    }
}

