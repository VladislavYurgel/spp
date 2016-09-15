namespace Tracer_WinForms
{
    partial class Form1
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.removeLabel = new System.Windows.Forms.Label();
            this.treeView0 = new System.Windows.Forms.TreeView();
            this.removeTab = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.файлToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openAs = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Font = new System.Drawing.Font("Microsoft YaHei Light", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tabControl1.Location = new System.Drawing.Point(12, 43);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(869, 481);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.removeLabel);
            this.tabPage1.Controls.Add(this.treeView0);
            this.tabPage1.Controls.Add(this.removeTab);
            this.tabPage1.Location = new System.Drawing.Point(4, 28);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(861, 449);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "XML Page 1";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // removeLabel
            // 
            this.removeLabel.AutoSize = true;
            this.removeLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.removeLabel.Location = new System.Drawing.Point(758, 10);
            this.removeLabel.Name = "removeLabel";
            this.removeLabel.Size = new System.Drawing.Size(84, 20);
            this.removeLabel.TabIndex = 4;
            this.removeLabel.Text = "Remove tab";
            this.removeLabel.Click += new System.EventHandler(this.removeLabel_Click);
            // 
            // treeView0
            // 
            this.treeView0.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.treeView0.Location = new System.Drawing.Point(6, 9);
            this.treeView0.Name = "treeView0";
            this.treeView0.Size = new System.Drawing.Size(695, 434);
            this.treeView0.TabIndex = 3;
            // 
            // removeTab
            // 
            this.removeTab.AutoSize = true;
            this.removeTab.Cursor = System.Windows.Forms.Cursors.Hand;
            this.removeTab.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.removeTab.Font = new System.Drawing.Font("Microsoft YaHei Light", 7.8F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.removeTab.Location = new System.Drawing.Point(771, 9);
            this.removeTab.Name = "removeTab";
            this.removeTab.Size = new System.Drawing.Size(0, 20);
            this.removeTab.TabIndex = 2;
            // 
            // menuStrip1
            // 
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.файлToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(891, 28);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // файлToolStripMenuItem
            // 
            this.файлToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openAs,
            this.saveAs});
            this.файлToolStripMenuItem.Font = new System.Drawing.Font("Microsoft YaHei Light", 9F);
            this.файлToolStripMenuItem.Name = "файлToolStripMenuItem";
            this.файлToolStripMenuItem.Size = new System.Drawing.Size(43, 24);
            this.файлToolStripMenuItem.Text = "File";
            // 
            // openAs
            // 
            this.openAs.Name = "openAs";
            this.openAs.Size = new System.Drawing.Size(152, 26);
            this.openAs.Text = "Open as...";
            this.openAs.Click += new System.EventHandler(this.openAs_Click);
            // 
            // saveAs
            // 
            this.saveAs.Name = "saveAs";
            this.saveAs.Size = new System.Drawing.Size(152, 26);
            this.saveAs.Text = "Save as...";
            this.saveAs.Click += new System.EventHandler(this.saveAs_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(891, 533);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Tracer WinForms";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form1_KeyDown);
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem файлToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAs;
        private System.Windows.Forms.ToolStripMenuItem openAs;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TreeView treeView0;
        private System.Windows.Forms.Label removeTab;
        private System.Windows.Forms.Label removeLabel;
    }
}

