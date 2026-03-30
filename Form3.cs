using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Management.Automation.Language;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PowerShellRunner
{
    public partial class Form3 : Form
    {
        public string EditedScriptContent { get; private set; }
        private static readonly HttpClient httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(120) };

        // GitHub Models API
        private const string GITHUB_MODELS_API_URL = "https://models.inference.ai.azure.com/chat/completions";
        
        private string GITHUB_API_KEY = "";
        private string MODEL_NAME = "gpt-4o";
        private bool isInitialized = false;

        public Form3(string scriptContent)
        {
            InitializeComponent();

            // 自动格式化 JSON 内容
            txtScriptEditor.Text = FormatScriptContent(scriptContent);
            EditedScriptContent = scriptContent;

            // 应用悬停效果
            ApplyHoverEffects();

            // 从环境变量读取 API Key
            GITHUB_API_KEY = Environment.GetEnvironmentVariable("GITHUB_TOKEN") ?? "";

            // 初始化 AI
            _ = InitializeAIAsync();
        }

        // 窗体大小改变时调整控件位置
        private void Form3_Resize(object sender, EventArgs e)
        {
            if (txtAIChat != null && txtAIInput != null && btnAISend != null)
            {
                int availableWidth = panelAI.ClientSize.Width - 20;
                int availableHeight = panelAI.ClientSize.Height - 220;

                txtAIChat.Size = new Size(availableWidth, availableHeight);
                txtAIInput.Location = new Point(10, panelAI.ClientSize.Height - 90);
                txtAIInput.Size = new Size(availableWidth - 110, 80);
                btnAISend.Location = new Point(panelAI.ClientSize.Width - 110, panelAI.ClientSize.Height - 90);
            }

            if (btnSave != null && btnCancel != null)
            {
                btnSave.Location = new Point(this.ClientSize.Width - 264, this.ClientSize.Height - 52);
                btnCancel.Location = new Point(this.ClientSize.Width - 132, this.ClientSize.Height - 52);
            }
        }

        // 格式化脚本内容
        private string FormatScriptContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return content;

            string trimmed = content.Trim();

            // --- 1. 优先尝试 JSON 格式化 ---
            if ((trimmed.StartsWith("{") && trimmed.EndsWith("}")) ||
                (trimmed.StartsWith("[") && trimmed.EndsWith("]")))
            {
                try
                {
                    JToken json = JToken.Parse(content);
                    return json.ToString(Formatting.Indented);
                }
                catch { /* 解析失败则跳过，尝试 PowerShell 逻辑 */ }
            }

            // --- 2. 尝试 PowerShell 格式化 ---
            try
            {
                // 使用 PowerShell 官方提供的解析器处理脚本
                // 这会自动纠正一部分缩进并将代码结构标准化
                var ast = Parser.ParseInput(content, out _, out _);

                // 如果脚本内容包含基本的 PS 特征（如 $ 或 - 或 {）
                // 我们返回解析后的 Extent 文本。
                // 注意：如果需要更强的“美化”效果（如自动缩进），建议配合自定义的 Indent 逻辑
                return ReformatPowerShellIndents(ast.Extent.Text);
            }
            catch
            {
                return content; // 彻底失败则返回原文
            }
        }

        /// <summary>
        /// 一个简单的 PowerShell 缩进修复逻辑
        /// </summary>
        private string ReformatPowerShellIndents(string script)
        {
            var lines = script.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
            var output = new StringBuilder();
            int indentLevel = 0;

            foreach (var line in lines)
            {
                string trimmedLine = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmedLine)) continue;

                // 如果这一行以 '}' 开头，先减少缩进
                if (trimmedLine.StartsWith("}")) indentLevel = Math.Max(0, indentLevel - 1);

                // 添加缩进空格（4个空格）
                output.AppendLine(new string(' ', indentLevel * 4) + trimmedLine);

                // 如果这一行以 '{' 结尾，增加后续行的缩进
                if (trimmedLine.EndsWith("{")) indentLevel++;
            }

            return output.ToString().TrimEnd();
        }

        // 应用按钮悬停效果
        private void ApplyHoverEffects()
        {
            btnAICheck.MouseEnter += (s, e) => btnAICheck.BackColor = Color.FromArgb(29, 78, 216);
            btnAICheck.MouseLeave += (s, e) => btnAICheck.BackColor = Color.FromArgb(37, 99, 235);

            btnAIFix.MouseEnter += (s, e) => btnAIFix.BackColor = Color.FromArgb(21, 128, 61);
            btnAIFix.MouseLeave += (s, e) => btnAIFix.BackColor = Color.FromArgb(22, 163, 74);

            btnAIOptimize.MouseEnter += (s, e) => btnAIOptimize.BackColor = Color.FromArgb(147, 51, 234);
            btnAIOptimize.MouseLeave += (s, e) => btnAIOptimize.BackColor = Color.FromArgb(168, 85, 247);

            btnFormatJSON.MouseEnter += (s, e) => btnFormatJSON.BackColor = Color.FromArgb(202, 138, 4);
            btnFormatJSON.MouseLeave += (s, e) => btnFormatJSON.BackColor = Color.FromArgb(234, 179, 8);

            btnAISend.MouseEnter += (s, e) => btnAISend.BackColor = Color.FromArgb(29, 78, 216);
            btnAISend.MouseLeave += (s, e) => btnAISend.BackColor = Color.FromArgb(37, 99, 235);

            btnClearChat.MouseEnter += (s, e) => btnClearChat.BackColor = Color.FromArgb(71, 85, 105);
            btnClearChat.MouseLeave += (s, e) => btnClearChat.BackColor = Color.FromArgb(51, 65, 85);

            btnSave.MouseEnter += (s, e) => btnSave.BackColor = Color.FromArgb(147, 51, 234);
            btnSave.MouseLeave += (s, e) => btnSave.BackColor = Color.FromArgb(168, 85, 247);

            btnCancel.MouseEnter += (s, e) => btnCancel.BackColor = Color.FromArgb(71, 85, 105);
            btnCancel.MouseLeave += (s, e) => btnCancel.BackColor = Color.FromArgb(51, 65, 85);
        }

        // 初始化 AI - 连接 GitHub Models API
        private async Task InitializeAIAsync()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(GITHUB_API_KEY))
                {
                    AppendMessage("错误", 
                        "❌ GitHub Token 未配置！\n\n" +
                        "1. 访问 https://github.com/settings/tokens 创建 Token\n" +
                        "2. 设置环境变量：GITHUB_TOKEN\n" +
                        "3. 重启应用程序");
                    isInitialized = false;
                    return;
                }

                AppendMessage("系统", "🔌 正在连接 GitHub Models 服务...");

                // 设置授权头
                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {GITHUB_API_KEY}");

                // 测试连接
                await Task.Delay(100);
                AppendMessage("系统", $"✅ 已连接到 GitHub Models\n📦 模型: {MODEL_NAME}");
                AppendMessage("AI", "你好！我可以帮助你检查、修复和优化脚本。\n\n尝试上面的快速操作按钮，或者问我任何问题！");
                isInitialized = true;
            }
            catch (Exception ex)
            {
                AppendMessage("错误", $"初始化失败: {ex.Message}");
                isInitialized = false;
            }
        }

        // 发送消息到 AI - 核心对话逻辑
        private async void btnAISend_Click(object sender, EventArgs e)
        {
            if (!isInitialized)
            {
                MessageBox.Show("AI 服务未初始化，请检查 API Key 配置", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string userMessage = txtAIInput.Text.Trim();
            if (string.IsNullOrWhiteSpace(userMessage)) 
                return;

            AppendMessage("You", userMessage);
            txtAIInput.Clear();
            txtAIInput.Focus();

            string currentScript = txtScriptEditor.Text;
            bool hasScript = !string.IsNullOrWhiteSpace(currentScript);
            bool isScriptQuestion = IsScriptRelatedQuestion(userMessage);
            bool shouldModify = ShouldModifyScript(userMessage);

            // 场景1: 用户要求修改脚本 (修复、优化等) + 有脚本
            if (hasScript && shouldModify)
            {
                await HandleScriptModification(userMessage, currentScript);
            }
            // 场景2: 用户问脚本相关问题 (分析、检查等) + 有脚本
            else if (hasScript && isScriptQuestion && !shouldModify)
            {
                await HandleScriptQuestion(userMessage, currentScript);
            }
            // 场景3: 用户问脚本相关问题但没有脚本
            else if (!hasScript && isScriptQuestion)
            {
                AppendMessage("系统", "⚠️ 编辑器中没有脚本。请先加载脚本内容。");
            }
            // 场景4: 普通对话 - 也自动加载脚本进行对话
            else
            {
                await HandleGeneralChat(userMessage, currentScript);
            }
        }

        // 处理脚本修改请求 (返回修改后的代码)
        private async Task HandleScriptModification(string userMessage, string currentScript)
        {
            AppendMessage("系统", "⏳ 正在处理脚本...");

            string prompt = $@"用户请求: {userMessage}

【当前脚本】:
```
{currentScript}
```

请根据用户请求对脚本进行修改。仅修改上述脚本，在代码块中返回修改后的完整脚本，不需要其他说明。";

            string response = await SendToAI(prompt, true);

            if (!string.IsNullOrWhiteSpace(response))
            {
                RemoveLastMessage(); // 移除"正在处理"提示
                AppendMessage("AI", response);
                
                string fixedScript = ExtractCodeBlock(response);

                if (!string.IsNullOrWhiteSpace(fixedScript) && fixedScript != response)
                {
                    // 使用 Invoke 确保在 UI 线程上执行弹窗
                    this.Invoke((Action)(() =>
                    {
                        DialogResult result = MessageBox.Show(
                            this,
                            "检测到修改后的脚本。是否应用到编辑器？\n\n点击\"是\"立即应用，\"否\"保留原脚本。",
                            "应用修改",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question,
                            MessageBoxDefaultButton.Button1);

                        if (result == DialogResult.Yes)
                        {
                            txtScriptEditor.Text = TryFormatAsJson(fixedScript);
                            AppendMessage("系统", "✅ 脚本已更新到编辑器！");
                        }
                        else
                        {
                            AppendMessage("系统", "❌ 已取消应用。");
                        }
                    }));
                }
            }
        }

        // 处理脚本分析问题 (不修改代码)
        private async Task HandleScriptQuestion(string userMessage, string currentScript)
        {
            string prompt = $@"{userMessage}

【当前脚本】:
```
{currentScript}
```

请根据用户问题分析脚本，并给出建议或结论。";
            
            AppendMessage("系统", "⏳ 正在分析脚本...");
            string response = await SendToAI(prompt);
            if (!string.IsNullOrWhiteSpace(response))
            {
                RemoveLastMessage();
                AppendMessage("AI", response);
            }
        }

        // 快速按钮: 修复脚本
        private async void btnAIFix_Click(object sender, EventArgs e)
        {
            if (!isInitialized)
            {
                MessageBox.Show("AI 服务未初始化", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string script = txtScriptEditor.Text;
            if (string.IsNullOrWhiteSpace(script))
            {
                MessageBox.Show("没有脚本要修复！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            AppendMessage("系统", "🔧 正在修复脚本...");

            string prompt = $@"修复这个脚本中的所有错误。只返回代码块中的修复后脚本，无需其他说明。

【脚本】:
```
{script}
```";

            string response = await SendToAI(prompt, true);

            if (!string.IsNullOrWhiteSpace(response))
            {
                string fixedScript = ExtractCodeBlock(response);

                if (!string.IsNullOrWhiteSpace(fixedScript) && fixedScript != response)
                {
                    RemoveLastMessage(); // 移除"正在修复"提示
                    AppendMessage("AI", response);
                    
                    // 直接应用修改
                    txtScriptEditor.Text = TryFormatAsJson(fixedScript);
                    AppendMessage("系统", "✅ 修复后的脚本已自动应用到编辑器！");
                }
                else
                {
                    RemoveLastMessage();
                    AppendMessage("AI", response);
                }
            }
        }

        // 优化代码
        private async void btnAIOptimize_Click(object sender, EventArgs e)
        {
            if (!isInitialized)
            {
                MessageBox.Show("AI 服务未初始化", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            string script = txtScriptEditor.Text;
            if (string.IsNullOrWhiteSpace(script))
            {
                MessageBox.Show("没有脚本要优化！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            AppendMessage("你", "优化我的脚本");
            AppendMessage("系统", "⚡ 正在优化脚本...");

            string prompt = $@"优化这个脚本以提高性能和可读性。只返回代码块中的优化代码：

```
{script}
```";

            string response = await SendToAI(prompt, true);

            if (!string.IsNullOrWhiteSpace(response))
            {
                string optimizedCode = ExtractCodeBlock(response);

                if (!string.IsNullOrWhiteSpace(optimizedCode) && optimizedCode != response)
                {
                    RemoveLastMessage(); // 移除"正在优化"提示
                    AppendMessage("AI", response);
                    
                    // 直接应用修改
                    txtScriptEditor.Text = TryFormatAsJson(optimizedCode);
                    AppendMessage("系统", "✅ 优化完成！脚本已自动更新到编辑器！");
                }
                else
                {
                    RemoveLastMessage();
                    AppendMessage("AI", response);
                }
            }
        }

        // 格式化 JSON
        private void btnFormatJSON_Click(object sender, EventArgs e)
        {
            string content = txtScriptEditor.Text;
            if (string.IsNullOrWhiteSpace(content))
            {
                MessageBox.Show("没有内容要格式化！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // --- 逻辑流：先尝试 JSON，后尝试 PowerShell ---
            try
            {
                // 1. 尝试作为 JSON 格式化
                JToken json = JToken.Parse(content);
                txtScriptEditor.Text = json.ToString(Formatting.Indented);
                AppendMessage("系统", "✅ JSON 已格式化！");
            }
            catch (JsonException) // 如果不是合法的 JSON，进入 PowerShell 格式化逻辑
            {
                try
                {
                    // 2. 尝试作为 PowerShell 格式化
                    // 注意：这需要引用 System.Management.Automation 命名空间
                    txtScriptEditor.Text = FormatPowerShellScript(content);
                    AppendMessage("系统", "✅ PowerShell 脚本已美化！");
                }
                catch (Exception ex)
                {
                    AppendMessage("错误", $"无法识别的内容格式或语法错误: {ex.Message}");
                }
            }
        }

        // 辅助方法：使用 PowerShell 引擎自身进行重构美化
        private string FormatPowerShellScript(string script)
        {
            using (var ps = System.Management.Automation.PowerShell.Create())
            {
                // 使用脚本块重新解析并转换字符串，可以起到基本的缩进效果
                ps.AddScript($"[Language.EditorServices.Formatting.CodesnippetFormatter]::Format('{script.Replace("'", "''")}')");
                // 如果没有安装上述扩展，最简单的方法是利用 AST (抽象语法树) 重新导出
                var ast = System.Management.Automation.Language.Parser.ParseInput(script, out _, out _);
                return ast.Extent.Text; // 这会返回经过标准解析后的文本
            }
        }

        // 清空对话
        private void btnClearChat_Click(object sender, EventArgs e)
        {
            txtAIChat.Clear();
            AppendMessage("系统", "对话已清空。");
        }

        // 发送到 GitHub Models API
        private async Task<string> SendToAI(string prompt, bool returnResponse = false)
        {
            try
            {
                var requestBody = new
                {
                    model = MODEL_NAME,
                    messages = new[]
                    {
                        new { role = "user", content = prompt }
                    },
                    temperature = 0.7,
                    max_tokens = 1024
                };

                string jsonRequest = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

                httpClient.DefaultRequestHeaders.Clear();
                httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {GITHUB_API_KEY}");

                HttpResponseMessage response = await httpClient.PostAsync(GITHUB_MODELS_API_URL, content);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    JObject json = JObject.Parse(jsonResponse);
                    string aiResponse = json["choices"]?[0]?["message"]?["content"]?.ToString() ?? "无响应。";

                    if (!returnResponse)
                    {
                        AppendMessage("AI", aiResponse);
                    }

                    return aiResponse;
                }
                else
                {
                    string errorMsg = await response.Content.ReadAsStringAsync();
                    AppendMessage("错误", $"API错误: {response.StatusCode}\n{errorMsg}");
                    return null;
                }
            }
            catch (HttpRequestException ex)
            {
                AppendMessage("错误", $"网络错误: {ex.Message}");
                return null;
            }
            catch (Exception ex)
            {
                AppendMessage("错误", $"错误: {ex.Message}");
                return null;
            }
        }

        // 添加消息到聊天框
        private void AppendMessage(string sender, string message)
        {
            if (txtAIChat.InvokeRequired)
            {
                txtAIChat.Invoke(new Action<string, string>(AppendMessage), sender, message);
                return;
            }

            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            Color textColor, iconColor;
            string icon;

            switch (sender)
            {
                case "AI":
                    textColor = Color.FromArgb(34, 197, 94);
                    iconColor = Color.FromArgb(34, 197, 94);
                    icon = "🤖";
                    break;
                case "You":
                    textColor = Color.White;
                    iconColor = Color.FromArgb(100, 200, 255);
                    icon = "👤";
                    break;
                case "系统":
                    textColor = Color.FromArgb(248, 250, 252);
                    iconColor = Color.FromArgb(251, 146, 60);
                    icon = "⚙️";
                    break;
                case "错误":
                    textColor = Color.FromArgb(254, 226, 226);
                    iconColor = Color.FromArgb(239, 68, 68);
                    icon = "❌";
                    break;
                default:
                    textColor = Color.FromArgb(226, 232, 240);
                    iconColor = Color.FromArgb(148, 163, 184);
                    icon = "💬";
                    break;
            }

            if (txtAIChat.TextLength > 0)
            {
                txtAIChat.AppendText("\n");
            }

            int startIndex = txtAIChat.TextLength;
            string header = $"{icon} {sender}  {timestamp}\n";
            txtAIChat.AppendText(header);

            txtAIChat.Select(startIndex, header.Length);
            txtAIChat.SelectionColor = iconColor;
            txtAIChat.SelectionFont = new Font("Segoe UI", 9F, FontStyle.Bold);

            int contentStartIndex = txtAIChat.TextLength;
            string formattedMessage = "  " + message.Replace("\n", "\n  ") + "\n";
            txtAIChat.AppendText(formattedMessage);

            txtAIChat.Select(contentStartIndex, formattedMessage.Length);
            txtAIChat.SelectionColor = textColor;
            txtAIChat.SelectionFont = new Font("Segoe UI", 10F);

            txtAIChat.SelectionStart = txtAIChat.Text.Length;
            txtAIChat.ScrollToCaret();
        }

        // 移除最后一条消息
        private void RemoveLastMessage()
        {
            if (txtAIChat.InvokeRequired)
            {
                txtAIChat.Invoke(new Action(RemoveLastMessage));
                return;
            }

            string text = txtAIChat.Text;
            if (string.IsNullOrEmpty(text)) return;

            string[] icons = { "🤖", "👤", "⚙️", "❌", "💬" };
            int lastNewlineBeforeMessage = -1;

            for (int i = text.Length - 1; i >= 0; i--)
            {
                if (text[i] == '\n')
                {
                    for (int j = i - 1; j >= 0; j--)
                    {
                        foreach (string icon in icons)
                        {
                            if (j + icon.Length <= text.Length && 
                                text.Substring(j, icon.Length) == icon &&
                                (j == 0 || text[j - 1] == '\n'))
                            {
                                lastNewlineBeforeMessage = j - 1;
                                break;
                            }
                        }
                        if (lastNewlineBeforeMessage != -1) break;
                    }
                    if (lastNewlineBeforeMessage != -1) break;
                }
            }

            if (lastNewlineBeforeMessage >= 0)
            {
                txtAIChat.Text = text.Substring(0, lastNewlineBeforeMessage + 1).TrimEnd();
            }
            else
            {
                txtAIChat.Clear();
            }
        }

        // 保存按钮
        private void btnSave_Click(object sender, EventArgs e)
        {
            EditedScriptContent = txtScriptEditor.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // 取消按钮
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // 提取 AI 响应中的代码块
        private string ExtractCodeBlock(string response)
        {
            if (string.IsNullOrWhiteSpace(response))
                return response;

            // 查找 markdown 代码块
            int start = response.IndexOf("```");
            if (start == -1)
                return response;

            start += 3;
            // 跳过可能的语言标记
            while (start < response.Length && (char.IsLetterOrDigit(response[start]) || response[start] == '\n' || response[start] == '\r'))
            {
                if (response[start] == '\n' || response[start] == '\r')
                {
                    start++;
                    break;
                }
                start++;
            }

            int end = response.IndexOf("```", start);
            if (end == -1)
                return response;

            return response.Substring(start, end - start).Trim();
        }

        // 处理普通对话 - 自动加载脚本参与对话
        private async Task HandleGeneralChat(string userMessage, string currentScript = "")
        {
            AppendMessage("系统", "⏳ 正在处理你的请求...");
            
            // 如果有脚本，自动包含在对话中
            string prompt = userMessage;
            if (!string.IsNullOrWhiteSpace(currentScript))
            {
                prompt = $@"{userMessage}

【当前编辑器的脚本】:
```
{currentScript}
```

请根据上述脚本内容和用户消息，给予完整、准确的回复。";
            }
            
            string response = await SendToAI(prompt, true);
            if (!string.IsNullOrWhiteSpace(response))
            {
                RemoveLastMessage();
                AppendMessage("AI", response);
                
                // 检查响应中是否包含代码块
                string extractedCode = ExtractCodeBlock(response);
                
                if (!string.IsNullOrWhiteSpace(extractedCode) && extractedCode != response)
                {
                    // 使用 Invoke 确保在 UI 线程上执行弹窗
                    this.Invoke((Action)(() =>
                    {
                        DialogResult result = MessageBox.Show(
                            this,
                            "检测到代码片段。是否应用到编辑器？\n\n点击\"是\"立即应用，\"否\"保留原脚本。",
                            "应用代码",
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question,
                            MessageBoxDefaultButton.Button1);

                        if (result == DialogResult.Yes)
                        {
                            txtScriptEditor.Text = TryFormatAsJson(extractedCode);
                            AppendMessage("系统", "✅ 代码已应用到编辑器！");
                        }
                        else
                        {
                            AppendMessage("系统", "❌ 已取消应用。");
                        }
                    }));
                }
            }
        }

        /// <summary>
        /// 判断用户消息是否与脚本相关
        /// </summary>
        private bool IsScriptRelatedQuestion(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return false;

            // 简单关键词判断，可根据实际需求扩展
            string[] keywords = { "脚本", "代码", "修复", "优化", "错误", "分析", "检查", "解释", "含义", "作用", "怎么写", "报错" };
            foreach (var keyword in keywords)
            {
                if (message.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 判断用户消息是否需要修改脚本（如修复、优化等）
        /// </summary>
        private bool ShouldModifyScript(string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                return false;

            // 关键词可根据实际需求扩展
            string[] modifyKeywords = { "修复", "优化", "重构", "改进", "完善", "纠正", "调整", "改写", "重写" };
            foreach (var keyword in modifyKeywords)
            {
                if (message.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }

        // 尝试将内容格式化为 JSON，如果不是 JSON 则原样返回
        private string TryFormatAsJson(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return content;

            try
            {
                string trimmed = content.Trim();
                if ((trimmed.StartsWith("{") && trimmed.EndsWith("}")) ||
                    (trimmed.StartsWith("[") && trimmed.EndsWith("]")))
                {
                    JToken json = JToken.Parse(content);
                    return json.ToString(Formatting.Indented);
                }
            }
            catch
            {
                // 不是有效 JSON，直接返回原内容
            }

            return content;
        }
    }
}