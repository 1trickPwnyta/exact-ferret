namespace Exact_Ferret.Settings_Classes
{
    partial class ExportForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.allCheckBox = new System.Windows.Forms.CheckBox();
            this.settingsBox = new System.Windows.Forms.GroupBox();
            this.panicModeCheckBox = new System.Windows.Forms.CheckBox();
            this.shortcutCheckBox = new System.Windows.Forms.CheckBox();
            this.logCheckBox = new System.Windows.Forms.CheckBox();
            this.blockedCheckBox = new System.Windows.Forms.CheckBox();
            this.apiCheckBox = new System.Windows.Forms.CheckBox();
            this.processCheckBox = new System.Windows.Forms.CheckBox();
            this.searchCheckBox = new System.Windows.Forms.CheckBox();
            this.behaviorCheckBox = new System.Windows.Forms.CheckBox();
            this.generalCheckBox = new System.Windows.Forms.CheckBox();
            this.desktopLabelCheckBox = new System.Windows.Forms.CheckBox();
            this.settingsBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(115, 318);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(24, 318);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(187, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Select the settings you want to export.";
            // 
            // allCheckBox
            // 
            this.allCheckBox.AutoSize = true;
            this.allCheckBox.Checked = true;
            this.allCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.allCheckBox.Location = new System.Drawing.Point(15, 36);
            this.allCheckBox.Name = "allCheckBox";
            this.allCheckBox.Size = new System.Drawing.Size(108, 17);
            this.allCheckBox.TabIndex = 3;
            this.allCheckBox.Text = "Export all settings";
            this.allCheckBox.UseVisualStyleBackColor = true;
            this.allCheckBox.CheckedChanged += new System.EventHandler(this.allCheckBox_CheckedChanged);
            // 
            // settingsBox
            // 
            this.settingsBox.Controls.Add(this.desktopLabelCheckBox);
            this.settingsBox.Controls.Add(this.panicModeCheckBox);
            this.settingsBox.Controls.Add(this.shortcutCheckBox);
            this.settingsBox.Controls.Add(this.logCheckBox);
            this.settingsBox.Controls.Add(this.blockedCheckBox);
            this.settingsBox.Controls.Add(this.apiCheckBox);
            this.settingsBox.Controls.Add(this.processCheckBox);
            this.settingsBox.Controls.Add(this.searchCheckBox);
            this.settingsBox.Controls.Add(this.behaviorCheckBox);
            this.settingsBox.Controls.Add(this.generalCheckBox);
            this.settingsBox.Enabled = false;
            this.settingsBox.Location = new System.Drawing.Point(15, 59);
            this.settingsBox.Name = "settingsBox";
            this.settingsBox.Size = new System.Drawing.Size(184, 253);
            this.settingsBox.TabIndex = 4;
            this.settingsBox.TabStop = false;
            // 
            // panicModeCheckBox
            // 
            this.panicModeCheckBox.AutoSize = true;
            this.panicModeCheckBox.Checked = true;
            this.panicModeCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.panicModeCheckBox.Location = new System.Drawing.Point(6, 157);
            this.panicModeCheckBox.Name = "panicModeCheckBox";
            this.panicModeCheckBox.Size = new System.Drawing.Size(121, 17);
            this.panicModeCheckBox.TabIndex = 8;
            this.panicModeCheckBox.Text = "Panic mode settings";
            this.panicModeCheckBox.UseVisualStyleBackColor = true;
            // 
            // shortcutCheckBox
            // 
            this.shortcutCheckBox.AutoSize = true;
            this.shortcutCheckBox.Checked = true;
            this.shortcutCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.shortcutCheckBox.Location = new System.Drawing.Point(6, 203);
            this.shortcutCheckBox.Name = "shortcutCheckBox";
            this.shortcutCheckBox.Size = new System.Drawing.Size(105, 17);
            this.shortcutCheckBox.TabIndex = 7;
            this.shortcutCheckBox.Text = "Shortcut settings";
            this.shortcutCheckBox.UseVisualStyleBackColor = true;
            // 
            // logCheckBox
            // 
            this.logCheckBox.AutoSize = true;
            this.logCheckBox.Checked = true;
            this.logCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.logCheckBox.Location = new System.Drawing.Point(6, 226);
            this.logCheckBox.Name = "logCheckBox";
            this.logCheckBox.Size = new System.Drawing.Size(83, 17);
            this.logCheckBox.TabIndex = 6;
            this.logCheckBox.Text = "Log settings";
            this.logCheckBox.UseVisualStyleBackColor = true;
            // 
            // blockedCheckBox
            // 
            this.blockedCheckBox.AutoSize = true;
            this.blockedCheckBox.Checked = true;
            this.blockedCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.blockedCheckBox.Location = new System.Drawing.Point(6, 180);
            this.blockedCheckBox.Name = "blockedCheckBox";
            this.blockedCheckBox.Size = new System.Drawing.Size(144, 17);
            this.blockedCheckBox.TabIndex = 5;
            this.blockedCheckBox.Text = "Blocked pictures settings";
            this.blockedCheckBox.UseVisualStyleBackColor = true;
            // 
            // apiCheckBox
            // 
            this.apiCheckBox.AutoSize = true;
            this.apiCheckBox.Checked = true;
            this.apiCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.apiCheckBox.Location = new System.Drawing.Point(6, 88);
            this.apiCheckBox.Name = "apiCheckBox";
            this.apiCheckBox.Size = new System.Drawing.Size(134, 17);
            this.apiCheckBox.TabIndex = 4;
            this.apiCheckBox.Text = "Search engine settings";
            this.apiCheckBox.UseVisualStyleBackColor = true;
            // 
            // processCheckBox
            // 
            this.processCheckBox.AutoSize = true;
            this.processCheckBox.Checked = true;
            this.processCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.processCheckBox.Location = new System.Drawing.Point(6, 111);
            this.processCheckBox.Name = "processCheckBox";
            this.processCheckBox.Size = new System.Drawing.Size(163, 17);
            this.processCheckBox.TabIndex = 3;
            this.processCheckBox.Text = "Background process settings";
            this.processCheckBox.UseVisualStyleBackColor = true;
            // 
            // searchCheckBox
            // 
            this.searchCheckBox.AutoSize = true;
            this.searchCheckBox.Checked = true;
            this.searchCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.searchCheckBox.Location = new System.Drawing.Point(6, 65);
            this.searchCheckBox.Name = "searchCheckBox";
            this.searchCheckBox.Size = new System.Drawing.Size(99, 17);
            this.searchCheckBox.TabIndex = 2;
            this.searchCheckBox.Text = "Search settings";
            this.searchCheckBox.UseVisualStyleBackColor = true;
            // 
            // behaviorCheckBox
            // 
            this.behaviorCheckBox.AutoSize = true;
            this.behaviorCheckBox.Checked = true;
            this.behaviorCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.behaviorCheckBox.Location = new System.Drawing.Point(6, 42);
            this.behaviorCheckBox.Name = "behaviorCheckBox";
            this.behaviorCheckBox.Size = new System.Drawing.Size(107, 17);
            this.behaviorCheckBox.TabIndex = 1;
            this.behaviorCheckBox.Text = "Behavior settings";
            this.behaviorCheckBox.UseVisualStyleBackColor = true;
            // 
            // generalCheckBox
            // 
            this.generalCheckBox.AutoSize = true;
            this.generalCheckBox.Checked = true;
            this.generalCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.generalCheckBox.Location = new System.Drawing.Point(6, 19);
            this.generalCheckBox.Name = "generalCheckBox";
            this.generalCheckBox.Size = new System.Drawing.Size(102, 17);
            this.generalCheckBox.TabIndex = 0;
            this.generalCheckBox.Text = "General settings";
            this.generalCheckBox.UseVisualStyleBackColor = true;
            // 
            // desktopLabelCheckBox
            // 
            this.desktopLabelCheckBox.AutoSize = true;
            this.desktopLabelCheckBox.Checked = true;
            this.desktopLabelCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.desktopLabelCheckBox.Location = new System.Drawing.Point(6, 134);
            this.desktopLabelCheckBox.Name = "desktopLabelCheckBox";
            this.desktopLabelCheckBox.Size = new System.Drawing.Size(130, 17);
            this.desktopLabelCheckBox.TabIndex = 9;
            this.desktopLabelCheckBox.Text = "Desktop label settings";
            this.desktopLabelCheckBox.UseVisualStyleBackColor = true;
            // 
            // ExportForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(216, 353);
            this.Controls.Add(this.settingsBox);
            this.Controls.Add(this.allCheckBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ExportForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Export Settings";
            this.settingsBox.ResumeLayout(false);
            this.settingsBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox allCheckBox;
        private System.Windows.Forms.GroupBox settingsBox;
        private System.Windows.Forms.CheckBox generalCheckBox;
        private System.Windows.Forms.CheckBox behaviorCheckBox;
        private System.Windows.Forms.CheckBox apiCheckBox;
        private System.Windows.Forms.CheckBox processCheckBox;
        private System.Windows.Forms.CheckBox searchCheckBox;
        private System.Windows.Forms.CheckBox blockedCheckBox;
        private System.Windows.Forms.CheckBox logCheckBox;
        private System.Windows.Forms.CheckBox shortcutCheckBox;
        private System.Windows.Forms.CheckBox panicModeCheckBox;
        private System.Windows.Forms.CheckBox desktopLabelCheckBox;
    }
}