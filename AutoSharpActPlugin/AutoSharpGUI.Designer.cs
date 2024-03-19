namespace AutoSharpActPlugin
{
    partial class AutoSharpGUI
    {
        /// <summary>
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置受控資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary>
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改
        /// 這個方法的內容。
        /// </summary>
        private void InitializeComponent()
        {
            this.Button_LocateModulesFolder = new System.Windows.Forms.Button();
            this.Button_Reload = new System.Windows.Forms.Button();
            this.ListBox_Log = new System.Windows.Forms.ListBox();
            this.Button_CopyLog = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // Button_LocateModulesFolder
            // 
            this.Button_LocateModulesFolder.Location = new System.Drawing.Point(2, 6);
            this.Button_LocateModulesFolder.Margin = new System.Windows.Forms.Padding(2);
            this.Button_LocateModulesFolder.Name = "Button_LocateModulesFolder";
            this.Button_LocateModulesFolder.Size = new System.Drawing.Size(122, 27);
            this.Button_LocateModulesFolder.TabIndex = 0;
            this.Button_LocateModulesFolder.Text = "Locate Modules Folder";
            this.Button_LocateModulesFolder.UseVisualStyleBackColor = true;
            this.Button_LocateModulesFolder.Click += new System.EventHandler(this.Button_LocateModulesFolder_Click);
            // 
            // Button_Reload
            // 
            this.Button_Reload.Location = new System.Drawing.Point(128, 6);
            this.Button_Reload.Margin = new System.Windows.Forms.Padding(2);
            this.Button_Reload.Name = "Button_Reload";
            this.Button_Reload.Size = new System.Drawing.Size(60, 27);
            this.Button_Reload.TabIndex = 1;
            this.Button_Reload.Text = "Reload";
            this.Button_Reload.UseVisualStyleBackColor = true;
            this.Button_Reload.Click += new System.EventHandler(this.Button_Reload_Click);
            // 
            // ListBox_Log
            // 
            this.ListBox_Log.FormattingEnabled = true;
            this.ListBox_Log.ItemHeight = 12;
            this.ListBox_Log.Location = new System.Drawing.Point(4, 38);
            this.ListBox_Log.Name = "ListBox_Log";
            this.ListBox_Log.ScrollAlwaysVisible = true;
            this.ListBox_Log.Size = new System.Drawing.Size(593, 136);
            this.ListBox_Log.TabIndex = 2;
            // 
            // Button_CopyLog
            // 
            this.Button_CopyLog.Location = new System.Drawing.Point(602, 38);
            this.Button_CopyLog.Margin = new System.Windows.Forms.Padding(2);
            this.Button_CopyLog.Name = "Button_CopyLog";
            this.Button_CopyLog.Size = new System.Drawing.Size(60, 27);
            this.Button_CopyLog.TabIndex = 3;
            this.Button_CopyLog.Text = "Copy";
            this.Button_CopyLog.UseVisualStyleBackColor = true;
            this.Button_CopyLog.Click += new System.EventHandler(this.Button_CopyLog_Click);
            // 
            // AutoSharpGUI
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Button_CopyLog);
            this.Controls.Add(this.ListBox_Log);
            this.Controls.Add(this.Button_Reload);
            this.Controls.Add(this.Button_LocateModulesFolder);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "AutoSharpGUI";
            this.Size = new System.Drawing.Size(797, 484);
            this.Load += new System.EventHandler(this.AutoSharpGUI_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button Button_LocateModulesFolder;
        private System.Windows.Forms.Button Button_Reload;
        private System.Windows.Forms.ListBox ListBox_Log;
        private System.Windows.Forms.Button Button_CopyLog;
    }
}
