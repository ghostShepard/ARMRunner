using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace PowerShellRunner
{
    /// <summary>
    /// 现代化UI样式辅助类
    /// </summary>
    public static class ModernUIHelper
    {
        // 颜色方案
        public static class Colors
        {
            // 背景色
            public static Color Background = Color.FromArgb(15, 23, 42);          // 深色背景
            public static Color CardBackground = Color.FromArgb(30, 41, 59);      // 卡片背景
            public static Color InputBackground = Color.FromArgb(30, 41, 59);     // 输入框背景

            // 边框色
            public static Color Border = Color.FromArgb(51, 65, 85);
            public static Color BorderFocus = Color.FromArgb(59, 130, 246);

            // 文字色
            public static Color TextPrimary = Color.White;
            public static Color TextSecondary = Color.FromArgb(203, 213, 225);

            // 按钮色
            public static Color ButtonBlue = Color.FromArgb(37, 99, 235);
            public static Color ButtonBlueHover = Color.FromArgb(29, 78, 216);
            public static Color ButtonGreen = Color.FromArgb(22, 163, 74);
            public static Color ButtonGreenHover = Color.FromArgb(21, 128, 61);
            public static Color ButtonPurple = Color.FromArgb(168, 85, 247);
            public static Color ButtonPurpleHover = Color.FromArgb(147, 51, 234);
            public static Color ButtonRed = Color.FromArgb(220, 38, 38);
            public static Color ButtonRedHover = Color.FromArgb(185, 28, 28);
            public static Color ButtonGray = Color.FromArgb(51, 65, 85);
            public static Color ButtonGrayHover = Color.FromArgb(71, 85, 105);
        }

        /// <summary>
        /// 应用现代化样式到整个窗体
        /// </summary>
        public static void ApplyModernStyle(Form form)
        {
            form.BackColor = Colors.Background;
            form.Font = new Font("Segoe UI", 9.5F);
            form.ForeColor = Colors.TextPrimary;

            // 美化所有控件
            BeautifyControls(form.Controls);
        }

        /// <summary>
        /// 递归美化所有控件
        /// </summary>
        private static void BeautifyControls(Control.ControlCollection controls)
        {
            foreach (Control ctrl in controls)
            {
                // ComboBox 样式
                if (ctrl is ComboBox comboBox)
                {
                    StyleComboBox(comboBox);
                }
                // TextBox 样式
                else if (ctrl is TextBox textBox)
                {
                    StyleTextBox(textBox);
                }
                // Button 样式
                else if (ctrl is Button button)
                {
                    StyleButton(button);
                }
                // Label 样式
                else if (ctrl is Label label)
                {
                    label.ForeColor = Colors.TextSecondary;
                    label.Font = new Font("Segoe UI", 9.5F);
                }
                // Panel/GroupBox 样式
                else if (ctrl is Panel || ctrl is GroupBox)
                {
                    ctrl.BackColor = Colors.CardBackground;
                    ctrl.ForeColor = Colors.TextPrimary;
                }
                // MenuStrip 样式
                else if (ctrl is MenuStrip menuStrip)
                {
                    StyleMenuStrip(menuStrip);
                }

                // 递归处理子控件
                if (ctrl.HasChildren)
                {
                    BeautifyControls(ctrl.Controls);
                }
            }
        }

        /// <summary>
        /// ComboBox 样式
        /// </summary>
        public static void StyleComboBox(ComboBox comboBox)
        {
            comboBox.FlatStyle = FlatStyle.Flat;
            comboBox.BackColor = Colors.InputBackground;
            comboBox.ForeColor = Colors.TextPrimary;
            comboBox.Font = new Font("Segoe UI", 10F);
        }

        /// <summary>
        /// TextBox 样式
        /// </summary>
        public static void StyleTextBox(TextBox textBox)
        {
            textBox.BackColor = Colors.InputBackground;
            textBox.ForeColor = Colors.TextPrimary;
            textBox.BorderStyle = BorderStyle.FixedSingle;

            // 如果是多行或代码编辑器，使用等宽字体
            if (textBox.Multiline || textBox.Name.Contains("Script") || textBox.Name.Contains("log"))
            {
                textBox.Font = new Font("Consolas", 9.5F);
            }
            else
            {
                textBox.Font = new Font("Segoe UI", 10F);
            }
        }

        /// <summary>
        /// Button 样式
        /// </summary>
        public static void StyleButton(Button button)
        {
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Font = new Font("Segoe UI Semibold", 10F);
            button.Cursor = Cursors.Hand;
            button.ForeColor = Colors.TextPrimary;
            button.Height = 40; // 统一按钮高度

            // 根据按钮文本或名称设置颜色
            string buttonText = button.Text.ToLower();
            string buttonName = button.Name.ToLower();

            if (buttonText.Contains("login") || buttonName.Contains("login"))
            {
                SetButtonColor(button, Colors.ButtonBlue, Colors.ButtonBlueHover);
            }
            else if (buttonText.Contains("load") || buttonName.Contains("load"))
            {
                SetButtonColor(button, Colors.ButtonBlue, Colors.ButtonBlueHover);
            }
            else if (buttonText.Contains("edit") || buttonName.Contains("edit"))
            {
                SetButtonColor(button, Colors.ButtonBlue, Colors.ButtonBlueHover);
            }
            else if (buttonText.Contains("run") || buttonText.Contains("powershell") || buttonName.Contains("run"))
            {
                SetButtonColor(button, Colors.ButtonGreen, Colors.ButtonGreenHover);
            }
            else if (buttonText.Contains("save") || buttonName.Contains("save"))
            {
                SetButtonColor(button, Colors.ButtonPurple, Colors.ButtonPurpleHover);
            }
            else if (buttonText.Contains("delete") || buttonName.Contains("delete"))
            {
                SetButtonColor(button, Colors.ButtonRed, Colors.ButtonRedHover);
            }
            else if (buttonText.Contains("get") || buttonName.Contains("get"))
            {
                SetButtonColor(button, Colors.ButtonGray, Colors.ButtonGrayHover);
            }
            else if (buttonText.Contains("patch") || buttonName.Contains("patch"))
            {
                SetButtonColor(button, Colors.ButtonGray, Colors.ButtonGrayHover);
            }
            else if (buttonText.Contains("put") || buttonName.Contains("put"))
            {
                SetButtonColor(button, Colors.ButtonGray, Colors.ButtonGrayHover);
            }
            else
            {
                SetButtonColor(button, Colors.ButtonGray, Colors.ButtonGrayHover);
            }
        }

        /// <summary>
        /// 设置按钮颜色和悬停效果
        /// </summary>
        private static void SetButtonColor(Button button, Color normalColor, Color hoverColor)
        {
            button.BackColor = normalColor;
            button.FlatAppearance.MouseOverBackColor = hoverColor;
            button.FlatAppearance.MouseDownBackColor = hoverColor;
        }

        /// <summary>
        /// MenuStrip 样式
        /// </summary>
        public static void StyleMenuStrip(MenuStrip menuStrip)
        {
            menuStrip.BackColor = Colors.CardBackground;
            menuStrip.ForeColor = Colors.TextPrimary;
            menuStrip.Renderer = new ModernMenuRenderer();
        }

        /// <summary>
        /// 创建圆角路径
        /// </summary>
        public static GraphicsPath GetRoundedRectangle(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;

            Rectangle arc = new Rectangle(rect.Location, new Size(diameter, diameter));

            // 左上角
            path.AddArc(arc, 180, 90);

            // 右上角
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);

            // 右下角
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // 左下角
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }
    }

    /// <summary>
    /// 现代化菜单渲染器
    /// </summary>
    public class ModernMenuRenderer : ToolStripProfessionalRenderer
    {
        protected override void OnRenderMenuItemBackground(ToolStripItemRenderEventArgs e)
        {
            if (e.Item.Selected)
            {
                Rectangle rect = new Rectangle(Point.Empty, e.Item.Size);
                using (SolidBrush brush = new SolidBrush(ModernUIHelper.Colors.ButtonBlue))
                {
                    e.Graphics.FillRectangle(brush, rect);
                }
            }
            else
            {
                base.OnRenderMenuItemBackground(e);
            }
        }

        protected override void OnRenderItemText(ToolStripItemTextRenderEventArgs e)
        {
            e.TextColor = ModernUIHelper.Colors.TextPrimary;
            base.OnRenderItemText(e);
        }
    }

    /// <summary>
    /// 自定义现代化Panel（带标题和圆角）
    /// </summary>
    public class ModernPanel : Panel
    {
        public string Title { get; set; }
        private int cornerRadius = 12;

        public ModernPanel()
        {
            this.BackColor = ModernUIHelper.Colors.CardBackground;
            this.Padding = new Padding(15);
            this.DoubleBuffered = true;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            // 绘制圆角背景
            using (GraphicsPath path = ModernUIHelper.GetRoundedRectangle(
                new Rectangle(0, 0, Width - 1, Height - 1), cornerRadius))
            {
                // 填充背景
                using (SolidBrush brush = new SolidBrush(this.BackColor))
                {
                    e.Graphics.FillPath(brush, path);
                }

                // 绘制边框
                using (Pen pen = new Pen(ModernUIHelper.Colors.Border, 1))
                {
                    e.Graphics.DrawPath(pen, path);
                }
            }

            // 绘制标题
            if (!string.IsNullOrEmpty(Title))
            {
                using (Font titleFont = new Font("Segoe UI Semibold", 11F))
                using (SolidBrush brush = new SolidBrush(ModernUIHelper.Colors.TextPrimary))
                {
                    e.Graphics.DrawString(Title, titleFont, brush, new PointF(15, 15));
                }
            }
        }
    }
}