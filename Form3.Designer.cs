namespace PowerShellRunner
{
    partial class Form3
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        // 控件声明
        private System.Windows.Forms.TextBox txtScriptEditor;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel panelAI;
        private System.Windows.Forms.Panel panelAITop;
        private System.Windows.Forms.Panel panelAIBottom;
        private System.Windows.Forms.RichTextBox txtAIChat;  // 改为 RichTextBox
        private System.Windows.Forms.TextBox txtAIInput;
        private System.Windows.Forms.Button btnAISend;
        private System.Windows.Forms.Button btnAICheck;
        private System.Windows.Forms.Button btnAIFix;
        private System.Windows.Forms.Button btnAIOptimize;
        private System.Windows.Forms.Button btnFormatJSON;
        private System.Windows.Forms.Button btnClearChat;
        private System.Windows.Forms.Splitter splitter1;

        private void InitializeComponent()
        {
            txtScriptEditor = new TextBox();
            btnSave = new Button();
            btnCancel = new Button();
            panelAI = new Panel();
            panelAIBottom = new Panel();
            txtAIChat = new RichTextBox();
            txtAIInput = new TextBox();
            btnAISend = new Button();
            panelAITop = new Panel();
            btnAICheck = new Button();
            btnAIFix = new Button();
            btnAIOptimize = new Button();
            btnFormatJSON = new Button();
            btnClearChat = new Button();
            splitter1 = new Splitter();
            panelAI.SuspendLayout();
            panelAIBottom.SuspendLayout();
            panelAITop.SuspendLayout();
            SuspendLayout();
            // 
            // txtScriptEditor
            // 
            txtScriptEditor.BackColor = Color.Black;
            txtScriptEditor.BorderStyle = BorderStyle.None;
            txtScriptEditor.Dock = DockStyle.Left;
            txtScriptEditor.Font = new Font("Consolas", 10F);
            txtScriptEditor.ForeColor = Color.FromArgb(226, 232, 240);
            txtScriptEditor.Location = new Point(0, 0);
            txtScriptEditor.Multiline = true;
            txtScriptEditor.Name = "txtScriptEditor";
            txtScriptEditor.ScrollBars = ScrollBars.Both;
            txtScriptEditor.Size = new Size(438, 747);
            txtScriptEditor.TabIndex = 0;
            // 
            // btnSave
            // 
            btnSave.BackColor = Color.FromArgb(168, 85, 247);
            btnSave.Cursor = Cursors.Hand;
            btnSave.FlatAppearance.BorderSize = 0;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Font = new Font("Segoe UI Semibold", 10F);
            btnSave.ForeColor = Color.White;
            btnSave.Location = new Point(4, 53);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(105, 42);
            btnSave.TabIndex = 0;
            btnSave.Text = "💾 Save";
            btnSave.UseVisualStyleBackColor = false;
            btnSave.Click += btnSave_Click;
            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.FromArgb(51, 65, 85);
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Font = new Font("Segoe UI Semibold", 10F);
            btnCancel.ForeColor = Color.White;
            btnCancel.Location = new Point(131, 53);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(105, 42);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Click += btnCancel_Click;
            // 
            // panelAI
            // 
            panelAI.BackColor = Color.FromArgb(15, 23, 42);
            panelAI.Controls.Add(panelAIBottom);
            panelAI.Controls.Add(panelAITop);
            panelAI.Dock = DockStyle.Fill;
            panelAI.Location = new Point(441, 0);
            panelAI.Name = "panelAI";
            panelAI.Padding = new Padding(9, 11, 9, 11);
            panelAI.Size = new Size(819, 747);
            panelAI.TabIndex = 1;
            // 
            // panelAIBottom
            // 
            panelAIBottom.BackColor = Color.FromArgb(15, 23, 42);
            panelAIBottom.Controls.Add(txtAIChat);
            panelAIBottom.Controls.Add(txtAIInput);
            panelAIBottom.Controls.Add(btnAISend);
            panelAIBottom.Dock = DockStyle.Fill;
            panelAIBottom.Location = new Point(9, 128);
            panelAIBottom.Name = "panelAIBottom";
            panelAIBottom.Padding = new Padding(4, 5, 4, 5);
            panelAIBottom.Size = new Size(801, 608);
            panelAIBottom.TabIndex = 0;
            // 
            // txtAIChat
            // 
            txtAIChat.BackColor = Color.FromArgb(15, 23, 42);
            txtAIChat.BorderStyle = BorderStyle.None;
            txtAIChat.DetectUrls = false;
            txtAIChat.Dock = DockStyle.Fill;
            txtAIChat.Font = new Font("Segoe UI", 10F);
            txtAIChat.ForeColor = Color.FromArgb(226, 232, 240);
            txtAIChat.Location = new Point(4, 5);
            txtAIChat.Name = "txtAIChat";
            txtAIChat.ReadOnly = true;
            txtAIChat.ScrollBars = RichTextBoxScrollBars.Vertical;
            txtAIChat.Size = new Size(709, 513);
            txtAIChat.TabIndex = 0;
            txtAIChat.Text = "";
            // 
            // txtAIInput
            // 
            txtAIInput.BackColor = Color.FromArgb(51, 65, 85);
            txtAIInput.BorderStyle = BorderStyle.FixedSingle;
            txtAIInput.Dock = DockStyle.Bottom;
            txtAIInput.Font = new Font("Segoe UI", 10F);
            txtAIInput.ForeColor = Color.White;
            txtAIInput.Location = new Point(4, 518);
            txtAIInput.Multiline = true;
            txtAIInput.Name = "txtAIInput";
            txtAIInput.ScrollBars = ScrollBars.Vertical;
            txtAIInput.Size = new Size(709, 85);
            txtAIInput.TabIndex = 1;
            // 
            // btnAISend
            // 
            btnAISend.BackColor = Color.FromArgb(37, 99, 235);
            btnAISend.Cursor = Cursors.Hand;
            btnAISend.Dock = DockStyle.Right;
            btnAISend.FlatAppearance.BorderSize = 0;
            btnAISend.FlatStyle = FlatStyle.Flat;
            btnAISend.Font = new Font("Segoe UI Semibold", 10F);
            btnAISend.ForeColor = Color.White;
            btnAISend.Location = new Point(713, 5);
            btnAISend.Name = "btnAISend";
            btnAISend.Size = new Size(84, 598);
            btnAISend.TabIndex = 2;
            btnAISend.Text = "📤 Send";
            btnAISend.UseVisualStyleBackColor = false;
            btnAISend.Click += btnAISend_Click;
            // 
            // panelAITop
            // 
            panelAITop.BackColor = Color.FromArgb(15, 23, 42);
            panelAITop.Controls.Add(btnCancel);
            panelAITop.Controls.Add(btnSave);
            panelAITop.Controls.Add(btnAICheck);
            panelAITop.Controls.Add(btnAIFix);
            panelAITop.Controls.Add(btnAIOptimize);
            panelAITop.Controls.Add(btnFormatJSON);
            panelAITop.Controls.Add(btnClearChat);
            panelAITop.Dock = DockStyle.Top;
            panelAITop.Location = new Point(9, 11);
            panelAITop.Name = "panelAITop";
            panelAITop.Padding = new Padding(4, 5, 4, 5);
            panelAITop.Size = new Size(801, 117);
            panelAITop.TabIndex = 1;
            // 
            // btnAICheck
            // 
            btnAICheck.BackColor = Color.FromArgb(37, 99, 235);
            btnAICheck.Cursor = Cursors.Hand;
            btnAICheck.FlatAppearance.BorderSize = 0;
            btnAICheck.FlatStyle = FlatStyle.Flat;
            btnAICheck.Font = new Font("Segoe UI Semibold", 9F);
            btnAICheck.ForeColor = Color.White;
            btnAICheck.Location = new Point(4, 5);
            btnAICheck.Name = "btnAICheck";
            btnAICheck.Size = new Size(74, 42);
            btnAICheck.TabIndex = 0;
            btnAICheck.Text = "🔍 Check";
            btnAICheck.UseVisualStyleBackColor = false;
            // 
            // btnAIFix
            // 
            btnAIFix.BackColor = Color.FromArgb(22, 163, 74);
            btnAIFix.Cursor = Cursors.Hand;
            btnAIFix.FlatAppearance.BorderSize = 0;
            btnAIFix.FlatStyle = FlatStyle.Flat;
            btnAIFix.Font = new Font("Segoe UI Semibold", 9F);
            btnAIFix.ForeColor = Color.White;
            btnAIFix.Location = new Point(83, 5);
            btnAIFix.Name = "btnAIFix";
            btnAIFix.Size = new Size(74, 42);
            btnAIFix.TabIndex = 1;
            btnAIFix.Text = "🔧 Fix";
            btnAIFix.UseVisualStyleBackColor = false;
            btnAIFix.Click += btnAIFix_Click;
            // 
            // btnAIOptimize
            // 
            btnAIOptimize.BackColor = Color.FromArgb(168, 85, 247);
            btnAIOptimize.Cursor = Cursors.Hand;
            btnAIOptimize.FlatAppearance.BorderSize = 0;
            btnAIOptimize.FlatStyle = FlatStyle.Flat;
            btnAIOptimize.Font = new Font("Segoe UI Semibold", 9F);
            btnAIOptimize.ForeColor = Color.White;
            btnAIOptimize.Location = new Point(162, 5);
            btnAIOptimize.Name = "btnAIOptimize";
            btnAIOptimize.Size = new Size(74, 42);
            btnAIOptimize.TabIndex = 2;
            btnAIOptimize.Text = "⚡ Optimize";
            btnAIOptimize.UseVisualStyleBackColor = false;
            btnAIOptimize.Click += btnAIOptimize_Click;
            // 
            // btnFormatJSON
            // 
            btnFormatJSON.BackColor = Color.FromArgb(234, 179, 8);
            btnFormatJSON.Cursor = Cursors.Hand;
            btnFormatJSON.FlatAppearance.BorderSize = 0;
            btnFormatJSON.FlatStyle = FlatStyle.Flat;
            btnFormatJSON.Font = new Font("Segoe UI Semibold", 9F);
            btnFormatJSON.ForeColor = Color.White;
            btnFormatJSON.Location = new Point(322, 5);
            btnFormatJSON.Name = "btnFormatJSON";
            btnFormatJSON.Size = new Size(74, 42);
            btnFormatJSON.TabIndex = 3;
            btnFormatJSON.Text = "📄 JSON";
            btnFormatJSON.UseVisualStyleBackColor = false;
            btnFormatJSON.Click += btnFormatJSON_Click;
            // 
            // btnClearChat
            // 
            btnClearChat.BackColor = Color.FromArgb(51, 65, 85);
            btnClearChat.Cursor = Cursors.Hand;
            btnClearChat.FlatAppearance.BorderSize = 0;
            btnClearChat.FlatStyle = FlatStyle.Flat;
            btnClearChat.Font = new Font("Segoe UI Semibold", 9F);
            btnClearChat.ForeColor = Color.White;
            btnClearChat.Location = new Point(242, 5);
            btnClearChat.Name = "btnClearChat";
            btnClearChat.Size = new Size(74, 42);
            btnClearChat.TabIndex = 4;
            btnClearChat.Text = "🗑️ Clear";
            btnClearChat.UseVisualStyleBackColor = false;
            btnClearChat.Click += btnClearChat_Click;
            // 
            // splitter1
            // 
            splitter1.BackColor = Color.FromArgb(51, 65, 85);
            splitter1.Location = new Point(438, 0);
            splitter1.Name = "splitter1";
            splitter1.Size = new Size(3, 747);
            splitter1.TabIndex = 2;
            splitter1.TabStop = false;
            // 
            // Form3
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(15, 23, 42);
            ClientSize = new Size(1260, 747);
            Controls.Add(panelAI);
            Controls.Add(splitter1);
            Controls.Add(txtScriptEditor);
            MinimumSize = new Size(790, 635);
            Name = "Form3";
            StartPosition = FormStartPosition.CenterParent;
            Text = "脚本编辑器 - AI助手";
            Resize += Form3_Resize;
            panelAI.ResumeLayout(false);
            panelAIBottom.ResumeLayout(false);
            panelAIBottom.PerformLayout();
            panelAITop.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }
    }
}