namespace PowerShellRunner
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtScriptContent = new RichTextBox();
            logOutput = new RichTextBox();
            btnRunScript = new Button();
            label1 = new Label();
            button1 = new Button();
            comboBox1 = new ComboBox();
            comboBox2 = new ComboBox();
            textBox1 = new TextBox();
            label2 = new Label();
            label3 = new Label();
            label4 = new Label();
            label5 = new Label();
            button2 = new Button();
            button3 = new Button();
            btnLogin = new Button();
            button4 = new Button();
            button5 = new Button();
            button6 = new Button();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            cleanToolStripMenuItem = new ToolStripMenuItem();
            button7 = new Button();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // txtScriptContent
            // 
            txtScriptContent.Location = new Point(180, 227);
            txtScriptContent.Name = "txtScriptContent";
            txtScriptContent.Size = new Size(887, 210);
            txtScriptContent.TabIndex = 2;
            txtScriptContent.Text = "";
            txtScriptContent.TextChanged += txtScriptContent_TextChanged;
            // 
            // logOutput
            // 
            logOutput.Location = new Point(180, 462);
            logOutput.Name = "logOutput";
            logOutput.Size = new Size(887, 334);
            logOutput.TabIndex = 3;
            logOutput.Text = "";
            // 
            // btnRunScript
            // 
            btnRunScript.Location = new Point(1114, 739);
            btnRunScript.Name = "btnRunScript";
            btnRunScript.Size = new Size(119, 42);
            btnRunScript.TabIndex = 4;
            btnRunScript.Text = "PUT";
            btnRunScript.UseVisualStyleBackColor = true;
            btnRunScript.Click += btnRunScript_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(107, 44);
            label1.Name = "label1";
            label1.Size = new Size(195, 17);
            label1.TabIndex = 6;
            label1.Text = "Please select Azure environment";
            // 
            // button1
            // 
            button1.Location = new Point(679, 100);
            button1.Name = "button1";
            button1.Size = new Size(163, 40);
            button1.TabIndex = 8;
            button1.Text = "Load Script";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click_1;
            // 
            // comboBox1
            // 
            comboBox1.FormattingEnabled = true;
            comboBox1.Items.AddRange(new object[] { "Public", "Dogfood", "Fairfax", "Mooncake" });
            comboBox1.Location = new Point(376, 41);
            comboBox1.Name = "comboBox1";
            comboBox1.Size = new Size(156, 25);
            comboBox1.TabIndex = 9;
            comboBox1.Text = "Clouds...";
            comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            // 
            // comboBox2
            // 
            comboBox2.FormattingEnabled = true;
            comboBox2.Items.AddRange(new object[] { "2025-04-01-preview", "2025-02-01-preview", "2024-11-01-preview", "2024-10-01", "2023-10-01", "2023-08-15" });
            comboBox2.Location = new Point(376, 109);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(156, 25);
            comboBox2.TabIndex = 10;
            comboBox2.Text = "API Version...";
            comboBox2.SelectedIndexChanged += comboBox2_SelectedIndexChanged;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(376, 174);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(528, 23);
            textBox1.TabIndex = 11;
            textBox1.Tag = "";
            textBox1.Text = "Please Input Your Resource ID";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(226, 117);
            label2.Name = "label2";
            label2.Size = new Size(76, 17);
            label2.TabIndex = 12;
            label2.Text = "API-Version";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(223, 180);
            label3.Name = "label3";
            label3.Size = new Size(79, 17);
            label3.TabIndex = 13;
            label3.Text = "Resource ID";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(84, 322);
            label4.Name = "label4";
            label4.Size = new Size(90, 17);
            label4.TabIndex = 14;
            label4.Text = "Script Content";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(84, 621);
            label5.Name = "label5";
            label5.Size = new Size(74, 17);
            label5.TabIndex = 15;
            label5.Text = "Output Log";
            label5.Click += label5_Click;
            // 
            // button2
            // 
            button2.Location = new Point(1114, 679);
            button2.Name = "button2";
            button2.Size = new Size(119, 42);
            button2.TabIndex = 16;
            button2.Text = "PATCH";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click_1;
            // 
            // button3
            // 
            button3.Location = new Point(1114, 566);
            button3.Name = "button3";
            button3.Size = new Size(119, 42);
            button3.TabIndex = 17;
            button3.Text = "GET";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click_1;
            // 
            // btnLogin
            // 
            btnLogin.Location = new Point(679, 41);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(163, 38);
            btnLogin.TabIndex = 18;
            btnLogin.Text = "Login";
            btnLogin.UseVisualStyleBackColor = true;
            btnLogin.Click += btnLogin_Click_1;
            // 
            // button4
            // 
            button4.Location = new Point(1114, 621);
            button4.Name = "button4";
            button4.Size = new Size(119, 38);
            button4.TabIndex = 19;
            button4.Text = "DELETE";
            button4.UseVisualStyleBackColor = true;
            button4.Click += button4_Click;
            // 
            // button5
            // 
            button5.Location = new Point(898, 100);
            button5.Name = "button5";
            button5.Size = new Size(111, 40);
            button5.TabIndex = 20;
            button5.Text = "Edit Script";
            button5.UseVisualStyleBackColor = true;
            button5.Click += button5_Click;
            // 
            // button6
            // 
            button6.Location = new Point(1114, 501);
            button6.Name = "button6";
            button6.Size = new Size(119, 43);
            button6.TabIndex = 21;
            button6.Text = "Run Powershell Scirpt";
            button6.UseVisualStyleBackColor = true;
            button6.Click += button6_Click;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1292, 25);
            menuStrip1.TabIndex = 22;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { cleanToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(39, 21);
            fileToolStripMenuItem.Text = "File";
            // 
            // cleanToolStripMenuItem
            // 
            cleanToolStripMenuItem.Name = "cleanToolStripMenuItem";
            cleanToolStripMenuItem.Size = new Size(108, 22);
            cleanToolStripMenuItem.Text = "Clean";
            cleanToolStripMenuItem.Click += cleanToolStripMenuItem_Click;
            // 
            // button7
            // 
            button7.Location = new Point(1114, 432);
            button7.Name = "button7";
            button7.Size = new Size(119, 46);
            button7.TabIndex = 23;
            button7.Text = "Save";
            button7.UseVisualStyleBackColor = true;
            button7.Click += button7_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1292, 808);
            Controls.Add(button7);
            Controls.Add(button6);
            Controls.Add(button5);
            Controls.Add(button4);
            Controls.Add(btnLogin);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(textBox1);
            Controls.Add(comboBox2);
            Controls.Add(comboBox1);
            Controls.Add(button1);
            Controls.Add(label1);
            Controls.Add(btnRunScript);
            Controls.Add(logOutput);
            Controls.Add(txtScriptContent);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "ARM Script Runner";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private RichTextBox txtScriptContent;
        private RichTextBox logOutput;
        private Button btnRunScript;
        private Label label1;
        private Button button1;
        private ComboBox comboBox1;
        private ComboBox comboBox2;
        private TextBox textBox1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        private Button button2;
        private Button button3;
        private Button btnLogin;
        private Button button4;
        private Button button5;
        private Button button6;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem cleanToolStripMenuItem;
        private Button button7;
    }
}
