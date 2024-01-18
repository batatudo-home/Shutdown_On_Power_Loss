namespace MAINSPACE
{
    partial class Mainform
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Mainform));
            this.pic_running = new System.Windows.Forms.PictureBox();
            this.pic_stopped = new System.Windows.Forms.PictureBox();
            this.lbl_minutes = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lbl3 = new System.Windows.Forms.Label();
            this.lbl1 = new System.Windows.Forms.Label();
            this.pictureBox_green = new System.Windows.Forms.PictureBox();
            this.lbl2 = new System.Windows.Forms.Label();
            this.lbl_last_activity = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.lbl_server = new System.Windows.Forms.Label();
            this.lbl_shutdown = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pic_running)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_stopped)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_green)).BeginInit();
            this.SuspendLayout();
            // 
            // pic_running
            // 
            this.pic_running.Image = ((System.Drawing.Image)(resources.GetObject("pic_running.Image")));
            this.pic_running.Location = new System.Drawing.Point(217, 22);
            this.pic_running.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.pic_running.Name = "pic_running";
            this.pic_running.Size = new System.Drawing.Size(418, 128);
            this.pic_running.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pic_running.TabIndex = 7;
            this.pic_running.TabStop = false;
            this.pic_running.Click += new System.EventHandler(this.Pic_runningClick);
            // 
            // pic_stopped
            // 
            this.pic_stopped.Image = ((System.Drawing.Image)(resources.GetObject("pic_stopped.Image")));
            this.pic_stopped.Location = new System.Drawing.Point(411, 125);
            this.pic_stopped.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.pic_stopped.Name = "pic_stopped";
            this.pic_stopped.Size = new System.Drawing.Size(418, 128);
            this.pic_stopped.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pic_stopped.TabIndex = 8;
            this.pic_stopped.TabStop = false;
            this.pic_stopped.Click += new System.EventHandler(this.Pic_stoppedClick);
            // 
            // lbl_minutes
            // 
            this.lbl_minutes.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_minutes.Location = new System.Drawing.Point(369, 306);
            this.lbl_minutes.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbl_minutes.Name = "lbl_minutes";
            this.lbl_minutes.Size = new System.Drawing.Size(405, 44);
            this.lbl_minutes.TabIndex = 16;
            this.lbl_minutes.Text = "Minutes of Inactivity";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.Timer1Tick);
            // 
            // lbl3
            // 
            this.lbl3.AutoSize = true;
            this.lbl3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl3.Location = new System.Drawing.Point(24, 305);
            this.lbl3.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbl3.Name = "lbl3";
            this.lbl3.Size = new System.Drawing.Size(246, 37);
            this.lbl3.TabIndex = 14;
            this.lbl3.Text = "Shutdown After:";
            // 
            // lbl1
            // 
            this.lbl1.AutoSize = true;
            this.lbl1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl1.Location = new System.Drawing.Point(72, 194);
            this.lbl1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbl1.Name = "lbl1";
            this.lbl1.Size = new System.Drawing.Size(191, 37);
            this.lbl1.TabIndex = 17;
            this.lbl1.Text = "Ping Server:";
            // 
            // pictureBox_green
            // 
            this.pictureBox_green.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox_green.Image")));
            this.pictureBox_green.Location = new System.Drawing.Point(834, 331);
            this.pictureBox_green.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.pictureBox_green.Name = "pictureBox_green";
            this.pictureBox_green.Size = new System.Drawing.Size(34, 33);
            this.pictureBox_green.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox_green.TabIndex = 19;
            this.pictureBox_green.TabStop = false;
            // 
            // lbl2
            // 
            this.lbl2.AutoSize = true;
            this.lbl2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl2.Location = new System.Drawing.Point(104, 245);
            this.lbl2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.lbl2.Name = "lbl2";
            this.lbl2.Size = new System.Drawing.Size(160, 37);
            this.lbl2.TabIndex = 20;
            this.lbl2.Text = "Last Ping:";
            // 
            // lbl_last_activity
            // 
            this.lbl_last_activity.BackColor = System.Drawing.SystemColors.Info;
            this.lbl_last_activity.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_last_activity.Location = new System.Drawing.Point(262, 250);
            this.lbl_last_activity.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_last_activity.Name = "lbl_last_activity";
            this.lbl_last_activity.Size = new System.Drawing.Size(99, 36);
            this.lbl_last_activity.TabIndex = 21;
            this.lbl_last_activity.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(369, 248);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(392, 44);
            this.label1.TabIndex = 22;
            this.label1.Text = "Minutes Ago";
            // 
            // lbl_server
            // 
            this.lbl_server.BackColor = System.Drawing.SystemColors.Info;
            this.lbl_server.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_server.Location = new System.Drawing.Point(262, 194);
            this.lbl_server.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_server.Name = "lbl_server";
            this.lbl_server.Size = new System.Drawing.Size(418, 36);
            this.lbl_server.TabIndex = 23;
            // 
            // lbl_shutdown
            // 
            this.lbl_shutdown.BackColor = System.Drawing.SystemColors.Info;
            this.lbl_shutdown.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_shutdown.Location = new System.Drawing.Point(262, 309);
            this.lbl_shutdown.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lbl_shutdown.Name = "lbl_shutdown";
            this.lbl_shutdown.Size = new System.Drawing.Size(99, 36);
            this.lbl_shutdown.TabIndex = 24;
            this.lbl_shutdown.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Mainform
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(874, 367);
            this.Controls.Add(this.lbl_shutdown);
            this.Controls.Add(this.lbl_server);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lbl_last_activity);
            this.Controls.Add(this.lbl2);
            this.Controls.Add(this.pictureBox_green);
            this.Controls.Add(this.lbl1);
            this.Controls.Add(this.lbl_minutes);
            this.Controls.Add(this.lbl3);
            this.Controls.Add(this.pic_stopped);
            this.Controls.Add(this.pic_running);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Mainform";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Shutdown on Power Loss v1.0";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1FormClosing);
            this.Load += new System.EventHandler(this.Form1Load);
            ((System.ComponentModel.ISupportInitialize)(this.pic_running)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pic_stopped)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox_green)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        #endregion

        private System.Windows.Forms.PictureBox pic_running;
        private System.Windows.Forms.PictureBox pic_stopped;
        private System.Windows.Forms.Label lbl3;
        private System.Windows.Forms.Label lbl_minutes;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label lbl1;
        private System.Windows.Forms.PictureBox pictureBox_green;
        private System.Windows.Forms.Label lbl2;
        private System.Windows.Forms.Label lbl_last_activity;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbl_server;
        private System.Windows.Forms.Label lbl_shutdown;
    }
}
