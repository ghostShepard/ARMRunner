using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Management.Automation;
using System.Text;

namespace PowerShellRunner
{
    public partial class Form1 : Form
    {
        private PowerShell ps; // PowerShell 实例

        private static int currentMaxId = 0; // 当前最大ID，用于 ScriptResource

        private ConcurrentDictionary<string, PowerShell> runningScripts = new(); // 存储正在运行的脚本

        private string? selectedItemsString = ""; // ComboBox1 中选定的项目字符串

        private string commandString = ""; // 命令字符串，通常是文件路径

        private string apiVersion = ""; // API 版本

        private List<ScriptResource> scriptResources = new List<ScriptResource>(); // 存储脚本资源的列表

        private ScriptCacheService _scriptCache;

        /// <summary>
        /// Form1 类的构造函数。
        /// </summary>
        public Form1()
        {
            InitializeComponent();

            // 应用现代化样式
            ModernUIHelper.ApplyModernStyle(this);

            // 自定义文本框为黑色背景
            CustomizeSpecificControls();

            _ = InitializeRedis();
        }

        private async Task InitializeRedis()
        {
            try
            {
                bool connected = await RedisManager.Instance.ConnectAsync("localhost:6379");

                if (connected)
                {
                    _scriptCache = new ScriptCacheService();
                    AppendLogSafe("✅ Redis connected - Cache enabled\n");
                }
                else
                {
                    AppendLogSafe("⚠️ Redis not available - Running without cache\n");
                }
            }
            catch (Exception ex)
            {
                AppendLogSafe($"⚠️ Redis initialization failed: {ex.Message}\n");
            }
        }

        private void CustomizeSpecificControls()
        {
            // Script Content 文本框 - 黑色背景
            if (txtScriptContent != null)
            {
                txtScriptContent.BackColor = Color.Black;
                txtScriptContent.ForeColor = Color.FromArgb(226, 232, 240); // 浅灰色文字
                txtScriptContent.Font = new Font("Consolas", 10F);
                txtScriptContent.BorderStyle = BorderStyle.None;
            }

            // Output Log 文本框 - 黑色背景 + 绿色终端风格
            if (logOutput != null)
            {
                logOutput.BackColor = Color.Black;
                logOutput.ForeColor = Color.FromArgb(34, 197, 94); // 绿色终端文字
                logOutput.Font = new Font("Consolas", 9.5F);
                logOutput.BorderStyle = BorderStyle.None;
            }
        }

        /// <summary>
        /// txtScriptContent 文本框内容改变事件处理方法。
        /// </summary>
        private void txtScriptContent_TextChanged(object sender, EventArgs e)
        {
            // 此处可以添加文本框内容改变时的逻辑
        }

        /// <summary>
        /// ScriptResource 类，表示一个脚本资源。
        /// </summary>
        public class ScriptResource
        {
            public int Id { get; set; } // 脚本ID
            public string? ResourceId { get; set; } // 资源ID
            public string? ScriptContent { get; set; } // 脚本内容
        }

        /// <summary>
        /// comboBox1 选中项改变事件处理方法。
        /// </summary>
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedItemsString = comboBox1.SelectedItem?.ToString(); // 获取选中的项目

            if (selectedItemsString == "Public")
            {
                selectedItemsString = null; // 如果选中 "Public"，则设为 null
            }
        }

        /// <summary>
        /// 运行 PowerShell 脚本。
        /// </summary>
        /// <param name="script">要运行的 PowerShell 脚本内容。</param>
        private void RunPowerShellScript(string script)
        {
            if (ps == null)
            {
                ps = PowerShell.Create(); // 如果 PowerShell 实例不存在，则创建
            }

            ps.Commands.Clear(); // 清除之前的命令
            ps.AddScript(script); // 添加脚本
            try
            {
                var results = ps.Invoke(); // 执行脚本
                foreach (var output in results)
                {
                    AppendLogSafe(output.ToString() + "\n"); // 线程安全地追加日志
                }

                if (ps.HadErrors)
                {
                    foreach (var error in ps.Streams.Error)
                    {
                        AppendLogSafe("[错误] " + error.ToString() + "\n"); // 线程安全地追加错误日志
                    }
                }
            }
            catch (Exception ex)
            {
                AppendLogSafe("[异常] " + ex.Message + "\n"); // 线程安全地追加异常日志
            }
            finally
            {
                ps.Commands.Clear(); // 清除命令，保持会话活跃
            }
        }

        /// <summary>
        /// 线程安全的日志输出方法。
        /// </summary>
        /// <param name="text">要追加到日志的文本。</param>
        private void AppendLogSafe(string text)
        {
            if (logOutput.InvokeRequired)
            {
                // 如果在非UI线程调用，则通过 Invoke 委托到UI线程执行
                logOutput.Invoke(new Action<string>(AppendLogSafe), text);
            }
            else
            {
                logOutput.AppendText(text); // 直接追加文本
            }
        }

        /// <summary>
        /// 按钮点击事件处理方法，用于选择并加载脚本文件。
        /// </summary>
        private async void button1_Click_1(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Title = "Select scripts";
                openFileDialog.Filter = "PowerShell 脚本|*.ps1|JSON 文件|*.json";
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                openFileDialog.Multiselect = true;

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedFile = openFileDialog.FileName;
                    string filePath = openFileDialog.FileName;
                    string fileExtension = Path.GetExtension(filePath).ToLower();
                    string fileName = Path.GetFileName(filePath);
                    string content = File.ReadAllText(filePath);

                    if (fileExtension == ".json")
                    {
                        dynamic jsonObject = JsonConvert.DeserializeObject(content);
                        txtScriptContent.Text = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);
                    }

                    if (fileExtension == ".ps1")
                    {
                        txtScriptContent.Text = ParsePowerShellScript(content);
                    }

                    commandString = filePath;
                    string resourceId = textBox1.Text;

                    // ✨ 保存到 Redis 历史记录和缓存
                    if (_scriptCache != null && _scriptCache.IsConnected)
                    {
                        // 保存到历史记录
                        bool historySaved = await _scriptCache.SaveToHistoryAsync(
                            resourceId,
                            txtScriptContent.Text,
                            fileName);

                        // 缓存脚本（1小时过期）
                        bool cached = await _scriptCache.CacheScriptAsync(
                            resourceId,
                            txtScriptContent.Text,
                            60);

                        if (historySaved && cached)
                        {
                            AppendLogSafe($"💾 Script saved to history and cached\n");
                        }
                    }

                    scriptResources.Add(new ScriptResource
                    {
                        Id = ++currentMaxId,
                        ResourceId = resourceId,
                        ScriptContent = txtScriptContent.Text
                    });
                }
            }
        }

        private async void btnViewHistory_Click(object sender, EventArgs e)
        {
            if (_scriptCache == null || !_scriptCache.IsConnected)
            {
                MessageBox.Show(
                    "Redis is not connected!\n\nHistory feature requires Redis.",
                    "Redis Not Available",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            var history = await _scriptCache.GetHistoryAsync(20);

            if (history.Count == 0)
            {
                MessageBox.Show(
                    "No script history found.",
                    "History",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            // 创建并显示历史记录窗口
            FormScriptHistory historyForm = new FormScriptHistory(history, this);
            historyForm.ShowDialog();
        }

        /// <summary>
        /// 从历史记录加载脚本（由 FormScriptHistory 调用）
        /// </summary>
        public void LoadScriptFromHistory(ScriptHistoryItem item)
        {
            txtScriptContent.Text = item.ScriptContent;
            textBox1.Text = item.ResourceId;
            AppendLogSafe($"📜 Loaded from history: {item.FileName}\n");
        }

        private async void btnLoadFromCache_Click(object sender, EventArgs e)
        {
            if (_scriptCache == null || !_scriptCache.IsConnected)
            {
                MessageBox.Show("Redis is not connected!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string resourceId = textBox1.Text;
            if (string.IsNullOrWhiteSpace(resourceId))
            {
                MessageBox.Show("Please enter a Resource ID first!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string cachedScript = await _scriptCache.GetCachedScriptAsync(resourceId);

            if (!string.IsNullOrEmpty(cachedScript))
            {
                txtScriptContent.Text = cachedScript;
                AppendLogSafe($"⚡ Loaded from cache: {resourceId}\n");
            }
            else
            {
                MessageBox.Show(
                    $"No cached script found for:\n{resourceId}",
                    "Not Found",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// 解析 PowerShell 脚本内容。
        /// </summary>
        /// <param name="scriptContent">原始脚本内容。</param>
        /// <returns>解析后的脚本内容。</returns>
        private string ParsePowerShellScript(string scriptContent)
        {
            StringBuilder parsedContent = new StringBuilder(); // 用于构建解析后的内容

            string[] lines = scriptContent.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None); // 按行分割
            foreach (string line in lines)
            {
                parsedContent.AppendLine(line); // 逐行追加
            }

            return parsedContent.ToString();
        }

        /// <summary>
        /// comboBox2 选中项改变事件处理方法。
        /// </summary>
        public void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            apiVersion = comboBox2.SelectedItem?.ToString(); // 获取选中的 API 版本
        }

        /// <summary>
        /// 登录按钮点击事件处理方法。
        /// </summary>
        private async void btnLogin_Click_1(object sender, EventArgs e)
        {
            logOutput.Clear(); // 清空日志输出
            btnLogin.Enabled = false; // 禁用按钮，防止重复点击

            try
            {
                // 在后台线程运行 PowerShell 命令
                await Task.Run(() =>
                {
                    using (PowerShell ps = PowerShell.Create())
                    {
                        ps.AddScript("ARMClient login " + selectedItemsString); // 添加登录命令
                        var results = ps.Invoke(); // 执行命令

                        this.Invoke(new Action(() =>
                        {
                            // 在UI线程更新日志
                            foreach (var output in results)
                            {
                                logOutput.AppendText(output.ToString() + "\n");
                            }
                            if (ps.HadErrors)
                            {
                                foreach (var error in ps.Streams.Error)
                                {
                                    logOutput.AppendText("[错误] " + error.ToString() + "\n");
                                }
                            }
                        }));
                    }
                });
            }
            catch (Exception ex)
            {
                logOutput.AppendText("[异常] " + ex.Message + "\n"); // 记录异常
            }
            finally
            {
                btnLogin.Enabled = true; // 恢复按钮
            }
        }

        /// <summary>
        /// 删除按钮点击事件处理方法。
        /// </summary>
        private void button4_Click(object sender, EventArgs e)
        {
            string resourceId = textBox1.Text; // 获取资源ID

            if (string.IsNullOrWhiteSpace(resourceId))
            {
                MessageBox.Show("Please provide resource ID！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 弹出确认对话框
            DialogResult result = MessageBox.Show($"Are you sure want to delete {resourceId} ？",
                                                  "Warnning",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Warning,
                                                  MessageBoxDefaultButton.Button2);

            if (result == DialogResult.Yes)
            {
                string deleteCommand = $"armclient delete \"{resourceId}?api-version={apiVersion}\" -verbose"; // 构建删除命令

                RunPowerShellScript(deleteCommand); // 运行删除命令

                try
                {
                    var results = ps.Invoke(); // 执行 PowerShell 命令并获取结果

                    StringBuilder outputLog = new StringBuilder();

                    foreach (var output in results)
                    {
                        string logEntry = output.ToString();
                        logOutput.AppendText(logEntry + "\n"); // 在 Form1 的日志框显示
                        outputLog.AppendLine(logEntry); // 记录日志
                    }

                    if (ps.HadErrors)
                    {
                        foreach (var error in ps.Streams.Error)
                        {
                            string errorLog = "[错误] " + error.ToString();
                            logOutput.AppendText(errorLog + "\n");
                            outputLog.AppendLine(errorLog);
                        }
                    }
                }
                catch (Exception ex)
                {
                    logOutput.AppendText("[异常] " + ex.Message + "\n"); // 记录异常
                }
                finally
                {
                    ps.Commands.Clear(); // 清除命令，保持会话活跃
                }
            }
            else
            {
                MessageBox.Show("Cancel the deletiong！", "Notification", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        /// <summary>
        /// 编辑脚本按钮点击事件处理方法。
        /// </summary>
        private void button5_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtScriptContent.Text))
            {
                MessageBox.Show("No script is loaded！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using (Form3 editorForm = new Form3(txtScriptContent.Text)) // 创建 Form3 实例并传入当前脚本内容
            {
                if (editorForm.ShowDialog() == DialogResult.OK) // 显示 Form3 对话框，并检查对话框结果
                {
                    txtScriptContent.Text = editorForm.EditedScriptContent; // 获取编辑后的脚本内容并更新到当前文本框

                    try
                    {
                        if (!string.IsNullOrEmpty(commandString) && File.Exists(commandString))
                        {
                            // 将编辑后的内容保存回文件
                            File.WriteAllText(commandString, editorForm.EditedScriptContent, Encoding.UTF8);
                            MessageBox.Show("Script saved successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Failed to save script. File not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error saving script: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        /// <summary>
        /// 运行脚本按钮点击事件处理方法。
        /// </summary>
        private async void button6_Click(object sender, EventArgs e)
        {
            button6.Enabled = false; // 禁用按钮，防止重复点击
            try
            {
                // 构建 PowerShell 脚本，设置执行策略并运行指定路径的脚本
                string script = $"Set-ExecutionPolicy -ExecutionPolicy Unrestricted -Scope Process -Force; & '{commandString}'";
                await Task.Run(() =>
                {
                    RunPowerShellScript(script); // 在后台线程运行 PowerShell 脚本
                });
            }
            catch (Exception ex)
            {
                logOutput.AppendText("[异常] " + ex.Message + "\n"); // 记录异常
            }
            finally
            {
                button6.Enabled = true; // 恢复按钮
            }
        }

        /// <summary>
        /// 清理菜单项点击事件处理方法。
        /// </summary>
        private void cleanToolStripMenuItem_Click(object sender, EventArgs e)
        {
            txtScriptContent.Text = string.Empty; // 清空脚本内容文本框
            textBox1.Text = "Please Input Your Resource ID"; // 清空资源ID文本框

            comboBox1.SelectedIndex = -1; // ComboBox1 取消选中
            comboBox2.SelectedIndex = -1; // ComboBox2 取消选中

            logOutput.Clear(); // 清空日志输出

            selectedItemsString = null; // 重置选定项目字符串
            commandString = string.Empty; // 重置命令字符串
            apiVersion = "API Version..."; // 重置 API 版本

            MessageBox.Show("Page has been cleaned up！", "Warnning", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 获取按钮点击事件处理方法。
        /// </summary>
        private void button3_Click_1(object sender, EventArgs e)
        {
            string resourceId = textBox1.Text; // 获取资源ID

            // string formattedCommandString = string.IsNullOrWhiteSpace(commandString) ? "" : commandString + " "; // 已定义但未使用

            string getCommand = $"armclient get \"{resourceId}?api-version={apiVersion}\""; // 构建获取命令

            RunPowerShellScript(getCommand); // 运行获取命令

            try
            {
                var results = ps.Invoke(); // 执行 PowerShell 命令并获取结果
                foreach (var output in results)
                {
                    // 在 Form1 的 TextBox 中输出
                    logOutput.AppendText(output.ToString() + "\n");
                }

                if (ps.HadErrors)
                {
                    foreach (var error in ps.Streams.Error)
                    {
                        // 输出错误信息到 Form1 的 TextBox
                        logOutput.AppendText("[错误] " + error.ToString() + "\n");
                    }
                }
            }
            catch (Exception ex)
            {
                // 输出异常信息到 Form1 的 TextBox
                logOutput.AppendText("[异常] " + ex.Message + "\n");
            }
            finally
            {
                ps.Commands.Clear(); // 清除命令，保持会话活跃
            }
        }

        /// <summary>
        /// Patch 按钮点击事件处理方法。
        /// </summary>
        private void button2_Click_1(object sender, EventArgs e)
        {
            string resourceId = textBox1.Text; // 获取资源ID

            string formattedCommandString = string.IsNullOrWhiteSpace(commandString) ? "" : commandString + " "; // 格式化命令字符串

            try
            {
                string putCommand = $"armclient patch \"{resourceId}?api-version={apiVersion}\" {formattedCommandString} -verbose"; // 构建 patch 命令

                RunPowerShellScript(putCommand); // 运行 patch 命令

                var results = ps.Invoke(); // 执行 PowerShell 命令并获取结果

                StringBuilder outputLog = new StringBuilder();

                foreach (var output in results)
                {
                    string logEntry = output.ToString();
                    logOutput.AppendText(logEntry + "\n"); // 在 Form1 的日志框显示
                    outputLog.AppendLine(logEntry); // 记录日志
                }

                if (ps.HadErrors)
                {
                    foreach (var error in ps.Streams.Error)
                    {
                        string errorLog = "[错误] " + error.ToString();
                        logOutput.AppendText(errorLog + "\n");
                        outputLog.AppendLine(errorLog);
                    }
                }
            }
            catch (Exception ex)
            {
                logOutput.AppendText("[异常] " + ex.Message + "\n"); // 记录异常
            }
            finally
            {
                ps.Commands.Clear(); // 清除命令，保持会话活跃
            }
        }

        /// <summary>
        /// Put 按钮（或 RunScript 按钮）点击事件处理方法。
        /// </summary>
        private void btnRunScript_Click(object sender, EventArgs e)
        {
            string resourceId = textBox1.Text; // 获取资源ID

            string formattedCommandString = string.IsNullOrWhiteSpace(commandString) ? "" : commandString + " "; // 格式化命令字符串

            string putCommand = $"armclient put \"{resourceId}?api-version={apiVersion}\" {formattedCommandString} -verbose"; // 构建 put 命令

            RunPowerShellScript(putCommand); // 运行 put 命令

            try
            {
                var results = ps.Invoke(); // 执行 PowerShell 命令并获取结果

                StringBuilder outputLog = new StringBuilder();

                foreach (var output in results)
                {
                    string logEntry = output.ToString();
                    logOutput.AppendText(logEntry + "\n"); // 在 Form1 的日志框显示
                    outputLog.AppendLine(logEntry); // 记录日志
                }

                if (ps.HadErrors)
                {
                    foreach (var error in ps.Streams.Error)
                    {
                        string errorLog = "[错误] " + error.ToString();
                        logOutput.AppendText(errorLog + "\n");
                        outputLog.AppendLine(errorLog);
                    }
                }
            }
            catch (Exception ex)
            {
                // 输出异常信息到 Form1 的 TextBox
                logOutput.AppendText("[异常] " + ex.Message + "\n");
            }
            finally
            {
                ps.Commands.Clear(); // 清除命令，保持会话活跃
            }
        }

        /// <summary>
        /// label5 点击事件处理方法。
        /// </summary>
        private void label5_Click(object sender, EventArgs e)
        {
            // 此处可以添加 label5 点击时的逻辑
        }

        /// <summary>
        /// button7 点击事件处理方法，用于显示 Form2。
        /// </summary>
        private void button7_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2(scriptResources); // 创建 Form2 实例并传入脚本资源列表
            form2.Show(); // 显示 Form2
        }

    }
}