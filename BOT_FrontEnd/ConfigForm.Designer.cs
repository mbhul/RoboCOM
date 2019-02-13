namespace BOT_FrontEnd
{
    partial class ConfigForm
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
            this.btnCH1 = new System.Windows.Forms.Button();
            this.btnCH2 = new System.Windows.Forms.Button();
            this.btnCH3 = new System.Windows.Forms.Button();
            this.btnCH4 = new System.Windows.Forms.Button();
            this.btnCH5 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.ControllerPoller = new System.ComponentModel.BackgroundWorker();
            this.button1 = new System.Windows.Forms.Button();
            this.txt1_min = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.txt1_max = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txt1_center = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txt2_center = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txt2_max = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txt2_min = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txt3_center = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txt3_max = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.txt3_min = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.txt4_center = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.txt4_max = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.txt4_min = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.txt5_center = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.txt5_max = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.txt5_min = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkAccum = new System.Windows.Forms.CheckBox();
            this.chkPersist = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCH1
            // 
            this.btnCH1.Location = new System.Drawing.Point(12, 12);
            this.btnCH1.Name = "btnCH1";
            this.btnCH1.Size = new System.Drawing.Size(90, 31);
            this.btnCH1.TabIndex = 0;
            this.btnCH1.Text = "CH1 (Z)";
            this.btnCH1.UseVisualStyleBackColor = true;
            this.btnCH1.Click += new System.EventHandler(this.btnCH1_Click);
            // 
            // btnCH2
            // 
            this.btnCH2.Location = new System.Drawing.Point(12, 49);
            this.btnCH2.Name = "btnCH2";
            this.btnCH2.Size = new System.Drawing.Size(90, 31);
            this.btnCH2.TabIndex = 1;
            this.btnCH2.Text = "CH2 (X)";
            this.btnCH2.UseVisualStyleBackColor = true;
            this.btnCH2.Click += new System.EventHandler(this.btnCH2_Click);
            // 
            // btnCH3
            // 
            this.btnCH3.Location = new System.Drawing.Point(12, 86);
            this.btnCH3.Name = "btnCH3";
            this.btnCH3.Size = new System.Drawing.Size(90, 31);
            this.btnCH3.TabIndex = 2;
            this.btnCH3.Text = "CH3 (Y)";
            this.btnCH3.UseVisualStyleBackColor = true;
            this.btnCH3.Click += new System.EventHandler(this.btnCH3_Click);
            // 
            // btnCH4
            // 
            this.btnCH4.Location = new System.Drawing.Point(12, 123);
            this.btnCH4.Name = "btnCH4";
            this.btnCH4.Size = new System.Drawing.Size(90, 31);
            this.btnCH4.TabIndex = 3;
            this.btnCH4.Text = "CH4";
            this.btnCH4.UseVisualStyleBackColor = true;
            this.btnCH4.Click += new System.EventHandler(this.btnCH4_Click);
            // 
            // btnCH5
            // 
            this.btnCH5.Location = new System.Drawing.Point(12, 160);
            this.btnCH5.Name = "btnCH5";
            this.btnCH5.Size = new System.Drawing.Size(90, 31);
            this.btnCH5.TabIndex = 4;
            this.btnCH5.Text = "CH5";
            this.btnCH5.UseVisualStyleBackColor = true;
            this.btnCH5.Click += new System.EventHandler(this.btnCH5_Click);
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(117, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(75, 15);
            this.label1.TabIndex = 5;
            this.label1.Text = "Input Src 1";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(117, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(75, 15);
            this.label2.TabIndex = 6;
            this.label2.Text = "Input Src 2";
            // 
            // label3
            // 
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(117, 95);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(75, 15);
            this.label3.TabIndex = 7;
            this.label3.Text = "Input Src 3";
            // 
            // label4
            // 
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(117, 132);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 15);
            this.label4.TabIndex = 8;
            this.label4.Text = "Input Src 4";
            // 
            // label5
            // 
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(117, 169);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(75, 15);
            this.label5.TabIndex = 9;
            this.label5.Text = "Input Src 5";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(194, 197);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(303, 24);
            this.button1.TabIndex = 10;
            this.button1.Text = "Apply";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // txt1_min
            // 
            this.txt1_min.Location = new System.Drawing.Point(194, 18);
            this.txt1_min.Name = "txt1_min";
            this.txt1_min.Size = new System.Drawing.Size(74, 20);
            this.txt1_min.TabIndex = 11;
            this.txt1_min.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(274, 21);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(27, 13);
            this.label6.TabIndex = 12;
            this.label6.Text = "MIN";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(387, 21);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(30, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "MAX";
            // 
            // txt1_max
            // 
            this.txt1_max.Location = new System.Drawing.Point(307, 18);
            this.txt1_max.Name = "txt1_max";
            this.txt1_max.Size = new System.Drawing.Size(74, 20);
            this.txt1_max.TabIndex = 13;
            this.txt1_max.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(503, 21);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(51, 13);
            this.label8.TabIndex = 16;
            this.label8.Text = "CENTER";
            // 
            // txt1_center
            // 
            this.txt1_center.Location = new System.Drawing.Point(423, 18);
            this.txt1_center.Name = "txt1_center";
            this.txt1_center.Size = new System.Drawing.Size(74, 20);
            this.txt1_center.TabIndex = 15;
            this.txt1_center.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(503, 58);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(51, 13);
            this.label9.TabIndex = 22;
            this.label9.Text = "CENTER";
            // 
            // txt2_center
            // 
            this.txt2_center.Location = new System.Drawing.Point(423, 55);
            this.txt2_center.Name = "txt2_center";
            this.txt2_center.Size = new System.Drawing.Size(74, 20);
            this.txt2_center.TabIndex = 21;
            this.txt2_center.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(387, 58);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(30, 13);
            this.label10.TabIndex = 20;
            this.label10.Text = "MAX";
            // 
            // txt2_max
            // 
            this.txt2_max.Location = new System.Drawing.Point(307, 55);
            this.txt2_max.Name = "txt2_max";
            this.txt2_max.Size = new System.Drawing.Size(74, 20);
            this.txt2_max.TabIndex = 19;
            this.txt2_max.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(274, 58);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(27, 13);
            this.label11.TabIndex = 18;
            this.label11.Text = "MIN";
            // 
            // txt2_min
            // 
            this.txt2_min.Location = new System.Drawing.Point(194, 55);
            this.txt2_min.Name = "txt2_min";
            this.txt2_min.Size = new System.Drawing.Size(74, 20);
            this.txt2_min.TabIndex = 17;
            this.txt2_min.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(503, 95);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(51, 13);
            this.label12.TabIndex = 28;
            this.label12.Text = "CENTER";
            // 
            // txt3_center
            // 
            this.txt3_center.Location = new System.Drawing.Point(423, 92);
            this.txt3_center.Name = "txt3_center";
            this.txt3_center.Size = new System.Drawing.Size(74, 20);
            this.txt3_center.TabIndex = 27;
            this.txt3_center.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(387, 95);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(30, 13);
            this.label13.TabIndex = 26;
            this.label13.Text = "MAX";
            // 
            // txt3_max
            // 
            this.txt3_max.Location = new System.Drawing.Point(307, 92);
            this.txt3_max.Name = "txt3_max";
            this.txt3_max.Size = new System.Drawing.Size(74, 20);
            this.txt3_max.TabIndex = 25;
            this.txt3_max.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(274, 95);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(27, 13);
            this.label14.TabIndex = 24;
            this.label14.Text = "MIN";
            // 
            // txt3_min
            // 
            this.txt3_min.Location = new System.Drawing.Point(194, 92);
            this.txt3_min.Name = "txt3_min";
            this.txt3_min.Size = new System.Drawing.Size(74, 20);
            this.txt3_min.TabIndex = 23;
            this.txt3_min.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(503, 132);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(51, 13);
            this.label15.TabIndex = 34;
            this.label15.Text = "CENTER";
            // 
            // txt4_center
            // 
            this.txt4_center.Location = new System.Drawing.Point(423, 129);
            this.txt4_center.Name = "txt4_center";
            this.txt4_center.Size = new System.Drawing.Size(74, 20);
            this.txt4_center.TabIndex = 33;
            this.txt4_center.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(387, 132);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(30, 13);
            this.label16.TabIndex = 32;
            this.label16.Text = "MAX";
            // 
            // txt4_max
            // 
            this.txt4_max.Location = new System.Drawing.Point(307, 129);
            this.txt4_max.Name = "txt4_max";
            this.txt4_max.Size = new System.Drawing.Size(74, 20);
            this.txt4_max.TabIndex = 31;
            this.txt4_max.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(274, 132);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(27, 13);
            this.label17.TabIndex = 30;
            this.label17.Text = "MIN";
            // 
            // txt4_min
            // 
            this.txt4_min.Location = new System.Drawing.Point(194, 129);
            this.txt4_min.Name = "txt4_min";
            this.txt4_min.Size = new System.Drawing.Size(74, 20);
            this.txt4_min.TabIndex = 29;
            this.txt4_min.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(503, 169);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(51, 13);
            this.label18.TabIndex = 40;
            this.label18.Text = "CENTER";
            // 
            // txt5_center
            // 
            this.txt5_center.Location = new System.Drawing.Point(423, 166);
            this.txt5_center.Name = "txt5_center";
            this.txt5_center.Size = new System.Drawing.Size(74, 20);
            this.txt5_center.TabIndex = 39;
            this.txt5_center.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(387, 169);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(30, 13);
            this.label19.TabIndex = 38;
            this.label19.Text = "MAX";
            // 
            // txt5_max
            // 
            this.txt5_max.Location = new System.Drawing.Point(307, 166);
            this.txt5_max.Name = "txt5_max";
            this.txt5_max.Size = new System.Drawing.Size(74, 20);
            this.txt5_max.TabIndex = 37;
            this.txt5_max.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(274, 169);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(27, 13);
            this.label20.TabIndex = 36;
            this.label20.Text = "MIN";
            // 
            // txt5_min
            // 
            this.txt5_min.Location = new System.Drawing.Point(194, 166);
            this.txt5_min.Name = "txt5_min";
            this.txt5_min.Size = new System.Drawing.Size(74, 20);
            this.txt5_min.TabIndex = 35;
            this.txt5_min.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.chkAccum);
            this.groupBox1.Controls.Add(this.chkPersist);
            this.groupBox1.Location = new System.Drawing.Point(561, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(102, 217);
            this.groupBox1.TabIndex = 41;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Z-Axis Options";
            // 
            // chkAccum
            // 
            this.chkAccum.AutoSize = true;
            this.chkAccum.Location = new System.Drawing.Point(6, 59);
            this.chkAccum.Name = "chkAccum";
            this.chkAccum.Size = new System.Drawing.Size(92, 17);
            this.chkAccum.TabIndex = 1;
            this.chkAccum.Text = "Self-Centering";
            this.chkAccum.UseVisualStyleBackColor = true;
            // 
            // chkPersist
            // 
            this.chkPersist.AutoSize = true;
            this.chkPersist.Location = new System.Drawing.Point(6, 22);
            this.chkPersist.Name = "chkPersist";
            this.chkPersist.Size = new System.Drawing.Size(72, 17);
            this.chkPersist.TabIndex = 0;
            this.chkPersist.Text = "Show Bar";
            this.chkPersist.UseVisualStyleBackColor = true;
            // 
            // ConfigForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(678, 230);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.txt5_center);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.txt5_max);
            this.Controls.Add(this.label20);
            this.Controls.Add(this.txt5_min);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.txt4_center);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.txt4_max);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.txt4_min);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.txt3_center);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.txt3_max);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.txt3_min);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.txt2_center);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.txt2_max);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.txt2_min);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txt1_center);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txt1_max);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txt1_min);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCH5);
            this.Controls.Add(this.btnCH4);
            this.Controls.Add(this.btnCH3);
            this.Controls.Add(this.btnCH2);
            this.Controls.Add(this.btnCH1);
            this.MaximumSize = new System.Drawing.Size(694, 269);
            this.MinimumSize = new System.Drawing.Size(694, 269);
            this.Name = "ConfigForm";
            this.Text = "Controller Setup";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCH1;
        private System.Windows.Forms.Button btnCH2;
        private System.Windows.Forms.Button btnCH3;
        private System.Windows.Forms.Button btnCH4;
        private System.Windows.Forms.Button btnCH5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.ComponentModel.BackgroundWorker ControllerPoller;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox txt1_min;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txt1_max;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txt1_center;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txt2_center;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txt2_max;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txt2_min;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txt3_center;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txt3_max;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txt3_min;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txt4_center;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txt4_max;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox txt4_min;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.TextBox txt5_center;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox txt5_max;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox txt5_min;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkPersist;
        private System.Windows.Forms.CheckBox chkAccum;
    }
}