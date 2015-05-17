namespace Exact_Ferret.Settings_Classes
{
    partial class Welcome
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Welcome));
            this.logoPictureBox = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.welcomeLabel = new System.Windows.Forms.LinkLabel();
            this.linkLabel6 = new System.Windows.Forms.LinkLabel();
            this.label2 = new System.Windows.Forms.Label();
            this.googleApiKeyBox = new System.Windows.Forms.TextBox();
            this.finishButton = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.bingAccessTokenBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // logoPictureBox
            // 
            this.logoPictureBox.Image = global::Exact_Ferret.Properties.Resources.ferret;
            this.logoPictureBox.Location = new System.Drawing.Point(12, 12);
            this.logoPictureBox.Name = "logoPictureBox";
            this.logoPictureBox.Size = new System.Drawing.Size(131, 259);
            this.logoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.logoPictureBox.TabIndex = 13;
            this.logoPictureBox.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(149, 12);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(207, 20);
            this.label1.TabIndex = 15;
            this.label1.Text = "Welcome to Exact Ferret";
            // 
            // welcomeLabel
            // 
            this.welcomeLabel.AutoSize = true;
            this.welcomeLabel.LinkArea = new System.Windows.Forms.LinkArea(10, 0);
            this.welcomeLabel.Location = new System.Drawing.Point(153, 41);
            this.welcomeLabel.MaximumSize = new System.Drawing.Size(275, 0);
            this.welcomeLabel.Name = "welcomeLabel";
            this.welcomeLabel.Size = new System.Drawing.Size(275, 80);
            this.welcomeLabel.TabIndex = 16;
            this.welcomeLabel.Text = resources.GetString("welcomeLabel.Text");
            this.welcomeLabel.UseCompatibleTextRendering = true;
            // 
            // linkLabel6
            // 
            this.linkLabel6.AutoSize = true;
            this.linkLabel6.LinkArea = new System.Windows.Forms.LinkArea(40, 18);
            this.linkLabel6.Location = new System.Drawing.Point(153, 130);
            this.linkLabel6.MaximumSize = new System.Drawing.Size(275, 0);
            this.linkLabel6.Name = "linkLabel6";
            this.linkLabel6.Size = new System.Drawing.Size(265, 17);
            this.linkLabel6.TabIndex = 22;
            this.linkLabel6.TabStop = true;
            this.linkLabel6.Text = "For detailed instructions, refer to the help document.";
            this.linkLabel6.UseCompatibleTextRendering = true;
            this.linkLabel6.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel6_LinkClicked);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(164, 198);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(135, 13);
            this.label2.TabIndex = 23;
            this.label2.Text = "Enter your Google API key:";
            // 
            // googleApiKeyBox
            // 
            this.googleApiKeyBox.Location = new System.Drawing.Point(305, 195);
            this.googleApiKeyBox.Name = "googleApiKeyBox";
            this.googleApiKeyBox.Size = new System.Drawing.Size(115, 20);
            this.googleApiKeyBox.TabIndex = 24;
            this.googleApiKeyBox.TextChanged += new System.EventHandler(this.apiKeyBox_TextChanged);
            // 
            // finishButton
            // 
            this.finishButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.finishButton.Enabled = false;
            this.finishButton.Location = new System.Drawing.Point(264, 251);
            this.finishButton.Name = "finishButton";
            this.finishButton.Size = new System.Drawing.Size(75, 23);
            this.finishButton.TabIndex = 25;
            this.finishButton.Text = "OK";
            this.finishButton.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(345, 251);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 26;
            this.button2.Text = "Skip";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // bingAccessTokenBox
            // 
            this.bingAccessTokenBox.Location = new System.Drawing.Point(305, 221);
            this.bingAccessTokenBox.Name = "bingAccessTokenBox";
            this.bingAccessTokenBox.Size = new System.Drawing.Size(115, 20);
            this.bingAccessTokenBox.TabIndex = 28;
            this.bingAccessTokenBox.TextChanged += new System.EventHandler(this.apiKeyBox_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(150, 224);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(149, 13);
            this.label3.TabIndex = 27;
            this.label3.Text = "Enter your Bing access token:";
            // 
            // Welcome
            // 
            this.AcceptButton = this.finishButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.button2;
            this.ClientSize = new System.Drawing.Size(435, 283);
            this.Controls.Add(this.bingAccessTokenBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.finishButton);
            this.Controls.Add(this.googleApiKeyBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.linkLabel6);
            this.Controls.Add(this.welcomeLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.logoPictureBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Welcome";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Exact Ferret";
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox logoPictureBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel welcomeLabel;
        private System.Windows.Forms.LinkLabel linkLabel6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button finishButton;
        private System.Windows.Forms.Button button2;
        public System.Windows.Forms.TextBox googleApiKeyBox;
        public System.Windows.Forms.TextBox bingAccessTokenBox;
        private System.Windows.Forms.Label label3;
    }
}