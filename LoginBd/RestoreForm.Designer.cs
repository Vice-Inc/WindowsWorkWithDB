namespace LoginBd
{
    partial class RestoreForm
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.helpLabel3 = new System.Windows.Forms.Label();
            this.helpLabel2 = new System.Windows.Forms.Label();
            this.helpLabel1 = new System.Windows.Forms.Label();
            this.checkButton = new System.Windows.Forms.Button();
            this.sendButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.passwordTwoField = new System.Windows.Forms.TextBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.saveButton = new System.Windows.Forms.Button();
            this.passwordField = new System.Windows.Forms.TextBox();
            this.loginField = new System.Windows.Forms.TextBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.exitLabel = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.Tomato;
            this.panel1.Controls.Add(this.helpLabel3);
            this.panel1.Controls.Add(this.helpLabel2);
            this.panel1.Controls.Add(this.helpLabel1);
            this.panel1.Controls.Add(this.checkButton);
            this.panel1.Controls.Add(this.sendButton);
            this.panel1.Controls.Add(this.cancelButton);
            this.panel1.Controls.Add(this.passwordTwoField);
            this.panel1.Controls.Add(this.pictureBox3);
            this.panel1.Controls.Add(this.saveButton);
            this.panel1.Controls.Add(this.passwordField);
            this.panel1.Controls.Add(this.loginField);
            this.panel1.Controls.Add(this.pictureBox2);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Controls.Add(this.exitLabel);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(545, 464);
            this.panel1.TabIndex = 2;
            this.panel1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseDown);
            this.panel1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseMove);
            // 
            // helpLabel3
            // 
            this.helpLabel3.AutoSize = true;
            this.helpLabel3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.helpLabel3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.helpLabel3.Location = new System.Drawing.Point(32, 386);
            this.helpLabel3.Name = "helpLabel3";
            this.helpLabel3.Size = new System.Drawing.Size(332, 29);
            this.helpLabel3.TabIndex = 14;
            this.helpLabel3.Text = "и нажмите ПОДТВЕРДИТЬ.";
            // 
            // helpLabel2
            // 
            this.helpLabel2.AutoSize = true;
            this.helpLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.helpLabel2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.helpLabel2.Location = new System.Drawing.Point(32, 357);
            this.helpLabel2.Name = "helpLabel2";
            this.helpLabel2.Size = new System.Drawing.Size(437, 29);
            this.helpLabel2.TabIndex = 13;
            this.helpLabel2.Text = "подтверждения. Введите его в поле";
            // 
            // helpLabel1
            // 
            this.helpLabel1.AutoSize = true;
            this.helpLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.helpLabel1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.helpLabel1.Location = new System.Drawing.Point(32, 328);
            this.helpLabel1.Name = "helpLabel1";
            this.helpLabel1.Size = new System.Drawing.Size(395, 29);
            this.helpLabel1.TabIndex = 12;
            this.helpLabel1.Text = "Вам на почту был отправлен код";
            // 
            // checkButton
            // 
            this.checkButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.checkButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.checkButton.FlatAppearance.BorderSize = 3;
            this.checkButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.OrangeRed;
            this.checkButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.checkButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.checkButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.checkButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.checkButton.Location = new System.Drawing.Point(37, 241);
            this.checkButton.Name = "checkButton";
            this.checkButton.Size = new System.Drawing.Size(471, 67);
            this.checkButton.TabIndex = 11;
            this.checkButton.Text = "Подтвердить";
            this.checkButton.UseVisualStyleBackColor = true;
            this.checkButton.Click += new System.EventHandler(this.checkButton_Click);
            // 
            // sendButton
            // 
            this.sendButton.AccessibleRole = System.Windows.Forms.AccessibleRole.MenuItem;
            this.sendButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.sendButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.sendButton.FlatAppearance.BorderSize = 3;
            this.sendButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.OrangeRed;
            this.sendButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.sendButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.sendButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.sendButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.sendButton.Location = new System.Drawing.Point(37, 241);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(471, 67);
            this.sendButton.TabIndex = 10;
            this.sendButton.Text = "Отправить";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cancelButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.cancelButton.FlatAppearance.BorderSize = 3;
            this.cancelButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.OrangeRed;
            this.cancelButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.cancelButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cancelButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.cancelButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.cancelButton.Location = new System.Drawing.Point(37, 328);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(471, 67);
            this.cancelButton.TabIndex = 9;
            this.cancelButton.Text = "Назад";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // passwordTwoField
            // 
            this.passwordTwoField.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.passwordTwoField.Font = new System.Drawing.Font("Times New Roman", 22F);
            this.passwordTwoField.ForeColor = System.Drawing.Color.Tomato;
            this.passwordTwoField.Location = new System.Drawing.Point(142, 253);
            this.passwordTwoField.Name = "passwordTwoField";
            this.passwordTwoField.Size = new System.Drawing.Size(366, 58);
            this.passwordTwoField.TabIndex = 7;
            this.passwordTwoField.UseSystemPasswordChar = true;
            this.passwordTwoField.Enter += new System.EventHandler(this.passwordTwoField_Enter);
            this.passwordTwoField.Leave += new System.EventHandler(this.passwordTwoField_Leave);
            // 
            // pictureBox3
            // 
            this.pictureBox3.Image = global::LoginBd.Properties.Resources.password;
            this.pictureBox3.Location = new System.Drawing.Point(37, 241);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(70, 70);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox3.TabIndex = 9;
            this.pictureBox3.TabStop = false;
            // 
            // saveButton
            // 
            this.saveButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.saveButton.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.saveButton.FlatAppearance.BorderSize = 3;
            this.saveButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.OrangeRed;
            this.saveButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Gray;
            this.saveButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.saveButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 20F);
            this.saveButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.saveButton.Location = new System.Drawing.Point(37, 328);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(471, 67);
            this.saveButton.TabIndex = 8;
            this.saveButton.Text = "Сохранить";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // passwordField
            // 
            this.passwordField.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.passwordField.Font = new System.Drawing.Font("Times New Roman", 22F);
            this.passwordField.ForeColor = System.Drawing.Color.Tomato;
            this.passwordField.Location = new System.Drawing.Point(142, 160);
            this.passwordField.Name = "passwordField";
            this.passwordField.Size = new System.Drawing.Size(366, 58);
            this.passwordField.TabIndex = 6;
            this.passwordField.UseSystemPasswordChar = true;
            this.passwordField.Enter += new System.EventHandler(this.passwordField_Enter);
            this.passwordField.Leave += new System.EventHandler(this.passwordField_Leave);
            // 
            // loginField
            // 
            this.loginField.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.loginField.Font = new System.Drawing.Font("Times New Roman", 22F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.loginField.ForeColor = System.Drawing.Color.Tomato;
            this.loginField.Location = new System.Drawing.Point(142, 160);
            this.loginField.Multiline = true;
            this.loginField.Name = "loginField";
            this.loginField.Size = new System.Drawing.Size(366, 58);
            this.loginField.TabIndex = 4;
            this.loginField.Enter += new System.EventHandler(this.loginField_Enter);
            this.loginField.Leave += new System.EventHandler(this.loginField_Leave);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::LoginBd.Properties.Resources.password;
            this.pictureBox2.Location = new System.Drawing.Point(37, 148);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(70, 70);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 3;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::LoginBd.Properties.Resources.mail;
            this.pictureBox1.Location = new System.Drawing.Point(37, 148);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(70, 70);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 2;
            this.pictureBox1.TabStop = false;
            // 
            // exitLabel
            // 
            this.exitLabel.AutoSize = true;
            this.exitLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.exitLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.exitLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.exitLabel.Location = new System.Drawing.Point(498, 9);
            this.exitLabel.Name = "exitLabel";
            this.exitLabel.Size = new System.Drawing.Size(35, 32);
            this.exitLabel.TabIndex = 1;
            this.exitLabel.Text = "X";
            this.exitLabel.Click += new System.EventHandler(this.exitLabel_Click);
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Top;
            this.label1.Font = new System.Drawing.Font("Arial", 28F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.label1.Location = new System.Drawing.Point(0, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(545, 115);
            this.label1.TabIndex = 0;
            this.label1.Text = "Восстановление";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.label1_MouseDown);
            this.label1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.label1_MouseMove);
            // 
            // RestoreForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(545, 464);
            this.Controls.Add(this.panel1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "RestoreForm";
            this.Text = "RestoreForm";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TextBox passwordTwoField;
        private System.Windows.Forms.PictureBox pictureBox3;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.TextBox passwordField;
        private System.Windows.Forms.TextBox loginField;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label exitLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.Button checkButton;
        private System.Windows.Forms.Label helpLabel3;
        private System.Windows.Forms.Label helpLabel2;
        private System.Windows.Forms.Label helpLabel1;
    }
}