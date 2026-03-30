using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace PowerShellRunner
{
    public partial class FormScriptHistory : Form
    {
        private ListView listViewHistory;
        private Button btnLoad;
        private Button btnDelete;
        private Button btnClose;
        private List<ScriptHistoryItem> _historyItems;
        private Form1 _parentForm;

        public FormScriptHistory(List<ScriptHistoryItem> historyItems, Form1 parentForm)
        {
            _historyItems = historyItems;
            _parentForm = parentForm;
            InitializeComponent();
            LoadHistoryData();
        }

        private void InitializeComponent()
        {
            this.listViewHistory = new ListView();
            this.btnLoad = new Button();
            this.btnDelete = new Button();
            this.btnClose = new Button();
            this.SuspendLayout();

            // ListView
            this.listViewHistory.BackColor = Color.Black;
            this.listViewHistory.ForeColor = Color.FromArgb(34, 197, 94);
            this.listViewHistory.Font = new Font("Consolas", 10F);
            this.listViewHistory.FullRowSelect = true;
            this.listViewHistory.GridLines = true;
            this.listViewHistory.View = View.Details;
            this.listViewHistory.Location = new Point(12, 12);
            this.listViewHistory.Size = new Size(760, 480);
            this.listViewHistory.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            this.listViewHistory.Columns.Add("Time", 150);
            this.listViewHistory.Columns.Add("Resource ID", 300);
            this.listViewHistory.Columns.Add("File Name", 200);
            this.listViewHistory.Columns.Add("Size", 80);

            // Load Button
            this.btnLoad.BackColor = Color.FromArgb(37, 99, 235);
            this.btnLoad.FlatStyle = FlatStyle.Flat;
            this.btnLoad.ForeColor = Color.White;
            this.btnLoad.Font = new Font("Segoe UI Semibold", 10F);
            this.btnLoad.Text = "Load Selected";
            this.btnLoad.Size = new Size(120, 40);
            this.btnLoad.Location = new Point(12, 505);
            this.btnLoad.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.btnLoad.Click += BtnLoad_Click;

            // Delete Button
            this.btnDelete.BackColor = Color.FromArgb(220, 38, 38);
            this.btnDelete.FlatStyle = FlatStyle.Flat;
            this.btnDelete.ForeColor = Color.White;
            this.btnDelete.Font = new Font("Segoe UI Semibold", 10F);
            this.btnDelete.Text = "Clear History";
            this.btnDelete.Size = new Size(120, 40);
            this.btnDelete.Location = new Point(142, 505);
            this.btnDelete.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            this.btnDelete.Click += BtnDelete_Click;

            // Close Button
            this.btnClose.BackColor = Color.FromArgb(51, 65, 85);
            this.btnClose.FlatStyle = FlatStyle.Flat;
            this.btnClose.ForeColor = Color.White;
            this.btnClose.Font = new Font("Segoe UI Semibold", 10F);
            this.btnClose.Text = "Close";
            this.btnClose.Size = new Size(120, 40);
            this.btnClose.Location = new Point(652, 505);
            this.btnClose.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            this.btnClose.Click += (s, e) => this.Close();

            // Form
            this.BackColor = Color.FromArgb(15, 23, 42);
            this.ClientSize = new Size(784, 561);
            this.Controls.Add(this.listViewHistory);
            this.Controls.Add(this.btnLoad);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnClose);
            this.MinimumSize = new Size(600, 400);
            this.Name = "FormScriptHistory";
            this.Text = "Script History";
            this.StartPosition = FormStartPosition.CenterParent;
            this.ResumeLayout(false);
        }

        private void LoadHistoryData()
        {
            listViewHistory.Items.Clear();

            foreach (var item in _historyItems)
            {
                var listItem = new ListViewItem(item.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"));
                listItem.SubItems.Add(item.ResourceId ?? "N/A");
                listItem.SubItems.Add(item.FileName ?? "N/A");
                listItem.SubItems.Add($"{item.ScriptContent?.Length ?? 0} chars");
                listItem.Tag = item;
                listViewHistory.Items.Add(listItem);
            }
        }

        private void BtnLoad_Click(object sender, EventArgs e)
        {
            if (listViewHistory.SelectedItems.Count == 0)
            {
                MessageBox.Show("Please select a script from history!", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var selectedItem = listViewHistory.SelectedItems[0].Tag as ScriptHistoryItem;
            if (selectedItem != null)
            {
                _parentForm.LoadScriptFromHistory(selectedItem);
                this.Close();
            }
        }

        private async void BtnDelete_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to clear all script history?",
                "Confirm Clear",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                var cacheService = new ScriptCacheService();
                bool cleared = await cacheService.ClearHistoryAsync();

                if (cleared)
                {
                    MessageBox.Show("History cleared successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Failed to clear history!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}