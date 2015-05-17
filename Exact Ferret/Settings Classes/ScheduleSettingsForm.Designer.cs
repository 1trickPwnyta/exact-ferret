namespace Exact_Ferret.Settings_Classes
{
    partial class ScheduleSettingsForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.andBox = new System.Windows.Forms.ListBox();
            this.label2 = new System.Windows.Forms.Label();
            this.andBrowseButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.conditionBox = new System.Windows.Forms.ComboBox();
            this.certainDaysBox = new System.Windows.Forms.GroupBox();
            this.saturdayCheckBox = new System.Windows.Forms.CheckBox();
            this.fridayCheckBox = new System.Windows.Forms.CheckBox();
            this.thursdayCheckBox = new System.Windows.Forms.CheckBox();
            this.wednesdayCheckBox = new System.Windows.Forms.CheckBox();
            this.tuesdayCheckBox = new System.Windows.Forms.CheckBox();
            this.mondayCheckBox = new System.Windows.Forms.CheckBox();
            this.sundayCheckBox = new System.Windows.Forms.CheckBox();
            this.certainMonthsBox = new System.Windows.Forms.GroupBox();
            this.decemberCheckBox = new System.Windows.Forms.CheckBox();
            this.novemberCheckBox = new System.Windows.Forms.CheckBox();
            this.octoberCheckBox = new System.Windows.Forms.CheckBox();
            this.septemberCheckBox = new System.Windows.Forms.CheckBox();
            this.augustCheckBox = new System.Windows.Forms.CheckBox();
            this.julyCheckBox = new System.Windows.Forms.CheckBox();
            this.juneCheckBox = new System.Windows.Forms.CheckBox();
            this.mayCheckBox = new System.Windows.Forms.CheckBox();
            this.aprilCheckBox = new System.Windows.Forms.CheckBox();
            this.marchCheckBox = new System.Windows.Forms.CheckBox();
            this.februaryCheckBox = new System.Windows.Forms.CheckBox();
            this.januaryCheckBox = new System.Windows.Forms.CheckBox();
            this.certainTimesBox = new System.Windows.Forms.GroupBox();
            this.certainTimesStopPicker = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.certainTimesStartPicker = new System.Windows.Forms.DateTimePicker();
            this.orBox = new System.Windows.Forms.ListBox();
            this.orBrowseButton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.andRemoveButton = new System.Windows.Forms.Button();
            this.orRemoveButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.certainDaysBox.SuspendLayout();
            this.certainMonthsBox.SuspendLayout();
            this.certainTimesBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(296, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "You can schedule different settings to apply at different times.";
            // 
            // andBox
            // 
            this.andBox.FormattingEnabled = true;
            this.andBox.Location = new System.Drawing.Point(16, 53);
            this.andBox.Name = "andBox";
            this.andBox.Size = new System.Drawing.Size(165, 95);
            this.andBox.TabIndex = 1;
            this.andBox.SelectedIndexChanged += new System.EventHandler(this.andBox_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 37);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(168, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Settings files with AND conditions:";
            // 
            // andBrowseButton
            // 
            this.andBrowseButton.Location = new System.Drawing.Point(16, 154);
            this.andBrowseButton.Name = "andBrowseButton";
            this.andBrowseButton.Size = new System.Drawing.Size(75, 23);
            this.andBrowseButton.TabIndex = 3;
            this.andBrowseButton.Text = "Browse...";
            this.andBrowseButton.UseVisualStyleBackColor = true;
            this.andBrowseButton.Click += new System.EventHandler(this.andBrowseButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(185, 37);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(211, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "When do you want these settings to apply?";
            // 
            // conditionBox
            // 
            this.conditionBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.conditionBox.FormattingEnabled = true;
            this.conditionBox.Items.AddRange(new object[] {
            "At certain times of the day",
            "On certain days of the week",
            "In certain months of the year"});
            this.conditionBox.Location = new System.Drawing.Point(188, 54);
            this.conditionBox.Name = "conditionBox";
            this.conditionBox.Size = new System.Drawing.Size(208, 21);
            this.conditionBox.TabIndex = 5;
            this.conditionBox.SelectedIndexChanged += new System.EventHandler(this.conditionBox_SelectedIndexChanged);
            // 
            // certainDaysBox
            // 
            this.certainDaysBox.Controls.Add(this.saturdayCheckBox);
            this.certainDaysBox.Controls.Add(this.fridayCheckBox);
            this.certainDaysBox.Controls.Add(this.thursdayCheckBox);
            this.certainDaysBox.Controls.Add(this.wednesdayCheckBox);
            this.certainDaysBox.Controls.Add(this.tuesdayCheckBox);
            this.certainDaysBox.Controls.Add(this.mondayCheckBox);
            this.certainDaysBox.Controls.Add(this.sundayCheckBox);
            this.certainDaysBox.Location = new System.Drawing.Point(188, 81);
            this.certainDaysBox.Name = "certainDaysBox";
            this.certainDaysBox.Size = new System.Drawing.Size(208, 245);
            this.certainDaysBox.TabIndex = 6;
            this.certainDaysBox.TabStop = false;
            this.certainDaysBox.Visible = false;
            // 
            // saturdayCheckBox
            // 
            this.saturdayCheckBox.AutoSize = true;
            this.saturdayCheckBox.Location = new System.Drawing.Point(15, 154);
            this.saturdayCheckBox.Name = "saturdayCheckBox";
            this.saturdayCheckBox.Size = new System.Drawing.Size(68, 17);
            this.saturdayCheckBox.TabIndex = 6;
            this.saturdayCheckBox.Text = "Saturday";
            this.saturdayCheckBox.UseVisualStyleBackColor = true;
            this.saturdayCheckBox.CheckedChanged += new System.EventHandler(this.conditionParameter_ValueChanged);
            // 
            // fridayCheckBox
            // 
            this.fridayCheckBox.AutoSize = true;
            this.fridayCheckBox.Location = new System.Drawing.Point(15, 131);
            this.fridayCheckBox.Name = "fridayCheckBox";
            this.fridayCheckBox.Size = new System.Drawing.Size(54, 17);
            this.fridayCheckBox.TabIndex = 5;
            this.fridayCheckBox.Text = "Friday";
            this.fridayCheckBox.UseVisualStyleBackColor = true;
            this.fridayCheckBox.CheckedChanged += new System.EventHandler(this.conditionParameter_ValueChanged);
            // 
            // thursdayCheckBox
            // 
            this.thursdayCheckBox.AutoSize = true;
            this.thursdayCheckBox.Location = new System.Drawing.Point(15, 108);
            this.thursdayCheckBox.Name = "thursdayCheckBox";
            this.thursdayCheckBox.Size = new System.Drawing.Size(70, 17);
            this.thursdayCheckBox.TabIndex = 4;
            this.thursdayCheckBox.Text = "Thursday";
            this.thursdayCheckBox.UseVisualStyleBackColor = true;
            this.thursdayCheckBox.CheckedChanged += new System.EventHandler(this.conditionParameter_ValueChanged);
            // 
            // wednesdayCheckBox
            // 
            this.wednesdayCheckBox.AutoSize = true;
            this.wednesdayCheckBox.Location = new System.Drawing.Point(15, 85);
            this.wednesdayCheckBox.Name = "wednesdayCheckBox";
            this.wednesdayCheckBox.Size = new System.Drawing.Size(83, 17);
            this.wednesdayCheckBox.TabIndex = 3;
            this.wednesdayCheckBox.Text = "Wednesday";
            this.wednesdayCheckBox.UseVisualStyleBackColor = true;
            this.wednesdayCheckBox.CheckedChanged += new System.EventHandler(this.conditionParameter_ValueChanged);
            // 
            // tuesdayCheckBox
            // 
            this.tuesdayCheckBox.AutoSize = true;
            this.tuesdayCheckBox.Location = new System.Drawing.Point(15, 62);
            this.tuesdayCheckBox.Name = "tuesdayCheckBox";
            this.tuesdayCheckBox.Size = new System.Drawing.Size(67, 17);
            this.tuesdayCheckBox.TabIndex = 2;
            this.tuesdayCheckBox.Text = "Tuesday";
            this.tuesdayCheckBox.UseVisualStyleBackColor = true;
            this.tuesdayCheckBox.CheckedChanged += new System.EventHandler(this.conditionParameter_ValueChanged);
            // 
            // mondayCheckBox
            // 
            this.mondayCheckBox.AutoSize = true;
            this.mondayCheckBox.Location = new System.Drawing.Point(15, 39);
            this.mondayCheckBox.Name = "mondayCheckBox";
            this.mondayCheckBox.Size = new System.Drawing.Size(64, 17);
            this.mondayCheckBox.TabIndex = 1;
            this.mondayCheckBox.Text = "Monday";
            this.mondayCheckBox.UseVisualStyleBackColor = true;
            this.mondayCheckBox.CheckedChanged += new System.EventHandler(this.conditionParameter_ValueChanged);
            // 
            // sundayCheckBox
            // 
            this.sundayCheckBox.AutoSize = true;
            this.sundayCheckBox.Location = new System.Drawing.Point(15, 16);
            this.sundayCheckBox.Name = "sundayCheckBox";
            this.sundayCheckBox.Size = new System.Drawing.Size(62, 17);
            this.sundayCheckBox.TabIndex = 0;
            this.sundayCheckBox.Text = "Sunday";
            this.sundayCheckBox.UseVisualStyleBackColor = true;
            this.sundayCheckBox.CheckedChanged += new System.EventHandler(this.conditionParameter_ValueChanged);
            // 
            // certainMonthsBox
            // 
            this.certainMonthsBox.Controls.Add(this.decemberCheckBox);
            this.certainMonthsBox.Controls.Add(this.novemberCheckBox);
            this.certainMonthsBox.Controls.Add(this.octoberCheckBox);
            this.certainMonthsBox.Controls.Add(this.septemberCheckBox);
            this.certainMonthsBox.Controls.Add(this.augustCheckBox);
            this.certainMonthsBox.Controls.Add(this.julyCheckBox);
            this.certainMonthsBox.Controls.Add(this.juneCheckBox);
            this.certainMonthsBox.Controls.Add(this.mayCheckBox);
            this.certainMonthsBox.Controls.Add(this.aprilCheckBox);
            this.certainMonthsBox.Controls.Add(this.marchCheckBox);
            this.certainMonthsBox.Controls.Add(this.februaryCheckBox);
            this.certainMonthsBox.Controls.Add(this.januaryCheckBox);
            this.certainMonthsBox.Location = new System.Drawing.Point(188, 81);
            this.certainMonthsBox.Name = "certainMonthsBox";
            this.certainMonthsBox.Size = new System.Drawing.Size(208, 245);
            this.certainMonthsBox.TabIndex = 7;
            this.certainMonthsBox.TabStop = false;
            this.certainMonthsBox.Visible = false;
            // 
            // decemberCheckBox
            // 
            this.decemberCheckBox.AutoSize = true;
            this.decemberCheckBox.Location = new System.Drawing.Point(112, 131);
            this.decemberCheckBox.Name = "decemberCheckBox";
            this.decemberCheckBox.Size = new System.Drawing.Size(75, 17);
            this.decemberCheckBox.TabIndex = 18;
            this.decemberCheckBox.Text = "December";
            this.decemberCheckBox.UseVisualStyleBackColor = true;
            this.decemberCheckBox.CheckedChanged += new System.EventHandler(this.conditionParameter_ValueChanged);
            // 
            // novemberCheckBox
            // 
            this.novemberCheckBox.AutoSize = true;
            this.novemberCheckBox.Location = new System.Drawing.Point(112, 108);
            this.novemberCheckBox.Name = "novemberCheckBox";
            this.novemberCheckBox.Size = new System.Drawing.Size(75, 17);
            this.novemberCheckBox.TabIndex = 17;
            this.novemberCheckBox.Text = "November";
            this.novemberCheckBox.UseVisualStyleBackColor = true;
            this.novemberCheckBox.CheckedChanged += new System.EventHandler(this.conditionParameter_ValueChanged);
            // 
            // octoberCheckBox
            // 
            this.octoberCheckBox.AutoSize = true;
            this.octoberCheckBox.Location = new System.Drawing.Point(112, 85);
            this.octoberCheckBox.Name = "octoberCheckBox";
            this.octoberCheckBox.Size = new System.Drawing.Size(64, 17);
            this.octoberCheckBox.TabIndex = 16;
            this.octoberCheckBox.Text = "October";
            this.octoberCheckBox.UseVisualStyleBackColor = true;
            this.octoberCheckBox.CheckedChanged += new System.EventHandler(this.conditionParameter_ValueChanged);
            // 
            // septemberCheckBox
            // 
            this.septemberCheckBox.AutoSize = true;
            this.septemberCheckBox.Location = new System.Drawing.Point(112, 62);
            this.septemberCheckBox.Name = "septemberCheckBox";
            this.septemberCheckBox.Size = new System.Drawing.Size(77, 17);
            this.septemberCheckBox.TabIndex = 15;
            this.septemberCheckBox.Text = "September";
            this.septemberCheckBox.UseVisualStyleBackColor = true;
            this.septemberCheckBox.CheckedChanged += new System.EventHandler(this.conditionParameter_ValueChanged);
            // 
            // augustCheckBox
            // 
            this.augustCheckBox.AutoSize = true;
            this.augustCheckBox.Location = new System.Drawing.Point(112, 39);
            this.augustCheckBox.Name = "augustCheckBox";
            this.augustCheckBox.Size = new System.Drawing.Size(59, 17);
            this.augustCheckBox.TabIndex = 14;
            this.augustCheckBox.Text = "August";
            this.augustCheckBox.UseVisualStyleBackColor = true;
            this.augustCheckBox.CheckedChanged += new System.EventHandler(this.conditionParameter_ValueChanged);
            // 
            // julyCheckBox
            // 
            this.julyCheckBox.AutoSize = true;
            this.julyCheckBox.Location = new System.Drawing.Point(112, 16);
            this.julyCheckBox.Name = "julyCheckBox";
            this.julyCheckBox.Size = new System.Drawing.Size(44, 17);
            this.julyCheckBox.TabIndex = 13;
            this.julyCheckBox.Text = "July";
            this.julyCheckBox.UseVisualStyleBackColor = true;
            this.julyCheckBox.CheckedChanged += new System.EventHandler(this.conditionParameter_ValueChanged);
            // 
            // juneCheckBox
            // 
            this.juneCheckBox.AutoSize = true;
            this.juneCheckBox.Location = new System.Drawing.Point(16, 131);
            this.juneCheckBox.Name = "juneCheckBox";
            this.juneCheckBox.Size = new System.Drawing.Size(49, 17);
            this.juneCheckBox.TabIndex = 12;
            this.juneCheckBox.Text = "June";
            this.juneCheckBox.UseVisualStyleBackColor = true;
            this.juneCheckBox.CheckedChanged += new System.EventHandler(this.conditionParameter_ValueChanged);
            // 
            // mayCheckBox
            // 
            this.mayCheckBox.AutoSize = true;
            this.mayCheckBox.Location = new System.Drawing.Point(16, 109);
            this.mayCheckBox.Name = "mayCheckBox";
            this.mayCheckBox.Size = new System.Drawing.Size(46, 17);
            this.mayCheckBox.TabIndex = 11;
            this.mayCheckBox.Text = "May";
            this.mayCheckBox.UseVisualStyleBackColor = true;
            this.mayCheckBox.CheckedChanged += new System.EventHandler(this.conditionParameter_ValueChanged);
            // 
            // aprilCheckBox
            // 
            this.aprilCheckBox.AutoSize = true;
            this.aprilCheckBox.Location = new System.Drawing.Point(16, 85);
            this.aprilCheckBox.Name = "aprilCheckBox";
            this.aprilCheckBox.Size = new System.Drawing.Size(46, 17);
            this.aprilCheckBox.TabIndex = 10;
            this.aprilCheckBox.Text = "April";
            this.aprilCheckBox.UseVisualStyleBackColor = true;
            this.aprilCheckBox.CheckedChanged += new System.EventHandler(this.conditionParameter_ValueChanged);
            // 
            // marchCheckBox
            // 
            this.marchCheckBox.AutoSize = true;
            this.marchCheckBox.Location = new System.Drawing.Point(16, 62);
            this.marchCheckBox.Name = "marchCheckBox";
            this.marchCheckBox.Size = new System.Drawing.Size(56, 17);
            this.marchCheckBox.TabIndex = 9;
            this.marchCheckBox.Text = "March";
            this.marchCheckBox.UseVisualStyleBackColor = true;
            this.marchCheckBox.CheckedChanged += new System.EventHandler(this.conditionParameter_ValueChanged);
            // 
            // februaryCheckBox
            // 
            this.februaryCheckBox.AutoSize = true;
            this.februaryCheckBox.Location = new System.Drawing.Point(16, 39);
            this.februaryCheckBox.Name = "februaryCheckBox";
            this.februaryCheckBox.Size = new System.Drawing.Size(67, 17);
            this.februaryCheckBox.TabIndex = 8;
            this.februaryCheckBox.Text = "February";
            this.februaryCheckBox.UseVisualStyleBackColor = true;
            this.februaryCheckBox.CheckedChanged += new System.EventHandler(this.conditionParameter_ValueChanged);
            // 
            // januaryCheckBox
            // 
            this.januaryCheckBox.AutoSize = true;
            this.januaryCheckBox.Location = new System.Drawing.Point(16, 16);
            this.januaryCheckBox.Name = "januaryCheckBox";
            this.januaryCheckBox.Size = new System.Drawing.Size(63, 17);
            this.januaryCheckBox.TabIndex = 7;
            this.januaryCheckBox.Text = "January";
            this.januaryCheckBox.UseVisualStyleBackColor = true;
            this.januaryCheckBox.CheckedChanged += new System.EventHandler(this.conditionParameter_ValueChanged);
            // 
            // certainTimesBox
            // 
            this.certainTimesBox.Controls.Add(this.certainTimesStopPicker);
            this.certainTimesBox.Controls.Add(this.label5);
            this.certainTimesBox.Controls.Add(this.label4);
            this.certainTimesBox.Controls.Add(this.certainTimesStartPicker);
            this.certainTimesBox.Location = new System.Drawing.Point(188, 81);
            this.certainTimesBox.Name = "certainTimesBox";
            this.certainTimesBox.Size = new System.Drawing.Size(208, 245);
            this.certainTimesBox.TabIndex = 7;
            this.certainTimesBox.TabStop = false;
            this.certainTimesBox.Visible = false;
            // 
            // certainTimesStopPicker
            // 
            this.certainTimesStopPicker.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.certainTimesStopPicker.Location = new System.Drawing.Point(62, 46);
            this.certainTimesStopPicker.Name = "certainTimesStopPicker";
            this.certainTimesStopPicker.ShowUpDown = true;
            this.certainTimesStopPicker.Size = new System.Drawing.Size(85, 20);
            this.certainTimesStopPicker.TabIndex = 3;
            this.certainTimesStopPicker.ValueChanged += new System.EventHandler(this.conditionParameter_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(31, 49);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(25, 13);
            this.label5.TabIndex = 2;
            this.label5.Text = "and";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 19);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(49, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Between";
            // 
            // certainTimesStartPicker
            // 
            this.certainTimesStartPicker.Format = System.Windows.Forms.DateTimePickerFormat.Time;
            this.certainTimesStartPicker.Location = new System.Drawing.Point(62, 16);
            this.certainTimesStartPicker.Name = "certainTimesStartPicker";
            this.certainTimesStartPicker.ShowUpDown = true;
            this.certainTimesStartPicker.Size = new System.Drawing.Size(85, 20);
            this.certainTimesStartPicker.TabIndex = 0;
            this.certainTimesStartPicker.ValueChanged += new System.EventHandler(this.conditionParameter_ValueChanged);
            // 
            // orBox
            // 
            this.orBox.FormattingEnabled = true;
            this.orBox.Location = new System.Drawing.Point(16, 213);
            this.orBox.Name = "orBox";
            this.orBox.Size = new System.Drawing.Size(165, 95);
            this.orBox.TabIndex = 8;
            this.orBox.SelectedIndexChanged += new System.EventHandler(this.orBox_SelectedIndexChanged);
            // 
            // orBrowseButton
            // 
            this.orBrowseButton.Location = new System.Drawing.Point(16, 314);
            this.orBrowseButton.Name = "orBrowseButton";
            this.orBrowseButton.Size = new System.Drawing.Size(75, 23);
            this.orBrowseButton.TabIndex = 9;
            this.orBrowseButton.Text = "Browse...";
            this.orBrowseButton.UseVisualStyleBackColor = true;
            this.orBrowseButton.Click += new System.EventHandler(this.orBrowseButton_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 194);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(161, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Settings files with OR conditions:";
            // 
            // andRemoveButton
            // 
            this.andRemoveButton.Enabled = false;
            this.andRemoveButton.Location = new System.Drawing.Point(106, 154);
            this.andRemoveButton.Name = "andRemoveButton";
            this.andRemoveButton.Size = new System.Drawing.Size(75, 23);
            this.andRemoveButton.TabIndex = 11;
            this.andRemoveButton.Text = "Remove";
            this.andRemoveButton.UseVisualStyleBackColor = true;
            this.andRemoveButton.Click += new System.EventHandler(this.andRemoveButton_Click);
            // 
            // orRemoveButton
            // 
            this.orRemoveButton.Enabled = false;
            this.orRemoveButton.Location = new System.Drawing.Point(106, 314);
            this.orRemoveButton.Name = "orRemoveButton";
            this.orRemoveButton.Size = new System.Drawing.Size(75, 23);
            this.orRemoveButton.TabIndex = 12;
            this.orRemoveButton.Text = "Remove";
            this.orRemoveButton.UseVisualStyleBackColor = true;
            this.orRemoveButton.Click += new System.EventHandler(this.orRemoveButton_Click);
            // 
            // okButton
            // 
            this.okButton.Location = new System.Drawing.Point(122, 346);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 13;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(204, 346);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 14;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // ScheduleSettingsForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(411, 379);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.orRemoveButton);
            this.Controls.Add(this.andRemoveButton);
            this.Controls.Add(this.certainMonthsBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.orBrowseButton);
            this.Controls.Add(this.certainDaysBox);
            this.Controls.Add(this.orBox);
            this.Controls.Add(this.conditionBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.andBrowseButton);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.andBox);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.certainTimesBox);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ScheduleSettingsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Schedule Settings";
            this.certainDaysBox.ResumeLayout(false);
            this.certainDaysBox.PerformLayout();
            this.certainMonthsBox.ResumeLayout(false);
            this.certainMonthsBox.PerformLayout();
            this.certainTimesBox.ResumeLayout(false);
            this.certainTimesBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListBox andBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button andBrowseButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox conditionBox;
        private System.Windows.Forms.GroupBox certainDaysBox;
        private System.Windows.Forms.GroupBox certainMonthsBox;
        private System.Windows.Forms.GroupBox certainTimesBox;
        private System.Windows.Forms.DateTimePicker certainTimesStartPicker;
        private System.Windows.Forms.DateTimePicker certainTimesStopPicker;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox orBox;
        private System.Windows.Forms.Button orBrowseButton;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox saturdayCheckBox;
        private System.Windows.Forms.CheckBox fridayCheckBox;
        private System.Windows.Forms.CheckBox thursdayCheckBox;
        private System.Windows.Forms.CheckBox wednesdayCheckBox;
        private System.Windows.Forms.CheckBox tuesdayCheckBox;
        private System.Windows.Forms.CheckBox mondayCheckBox;
        private System.Windows.Forms.CheckBox sundayCheckBox;
        private System.Windows.Forms.CheckBox decemberCheckBox;
        private System.Windows.Forms.CheckBox novemberCheckBox;
        private System.Windows.Forms.CheckBox octoberCheckBox;
        private System.Windows.Forms.CheckBox septemberCheckBox;
        private System.Windows.Forms.CheckBox augustCheckBox;
        private System.Windows.Forms.CheckBox julyCheckBox;
        private System.Windows.Forms.CheckBox juneCheckBox;
        private System.Windows.Forms.CheckBox mayCheckBox;
        private System.Windows.Forms.CheckBox aprilCheckBox;
        private System.Windows.Forms.CheckBox marchCheckBox;
        private System.Windows.Forms.CheckBox februaryCheckBox;
        private System.Windows.Forms.CheckBox januaryCheckBox;
        private System.Windows.Forms.Button andRemoveButton;
        private System.Windows.Forms.Button orRemoveButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
    }
}