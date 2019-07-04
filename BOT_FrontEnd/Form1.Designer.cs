namespace BOT_FrontEnd
{
    partial class Form1
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

        private string[] instructions;
        private static int instruction_count;
        private static int select_index;
        private System.Drawing.Font bold_font;

        private int InComTxt_default_position;
        private int InComLbl_default_position;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.OutComTxt = new System.Windows.Forms.RichTextBox();
            this.OutComLbl = new System.Windows.Forms.Label();
            this.OpenFileDlg = new System.Windows.Forms.OpenFileDialog();
            this.MyVCOM = new System.IO.Ports.SerialPort(this.components);
            this.SendBtn = new System.Windows.Forms.Button();
            this.SaveFileDlg = new System.Windows.Forms.SaveFileDialog();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.PortNumber = new System.Windows.Forms.ComboBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.BaudSelect = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.ConnReset = new System.Windows.Forms.Button();
            this.SendTimer = new System.Windows.Forms.Timer(this.components);
            this.ReadTimer = new System.Windows.Forms.Timer(this.components);
            this.ClearOutBtn = new System.Windows.Forms.Button();
            this.PadInPanel = new System.Windows.Forms.Panel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnCtlSettings = new System.Windows.Forms.Button();
            this.ControllerSelect = new System.Windows.Forms.ComboBox();
            this.ControllerPoller = new System.ComponentModel.BackgroundWorker();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.TextInPanel = new System.Windows.Forms.Panel();
            this.SaveFileBtn = new System.Windows.Forms.Button();
            this.OpenFileBtn = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.InComTxt = new System.Windows.Forms.RichTextBox();
            this.PadXY_View = new System.Windows.Forms.PictureBox();
            this.PadZ_View = new System.Windows.Forms.PictureBox();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkRepeat = new System.Windows.Forms.CheckBox();
            this.radioPad = new System.Windows.Forms.RadioButton();
            this.radioText = new System.Windows.Forms.RadioButton();
            this.ConnStatusLbl = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.TimerIntSelect = new System.Windows.Forms.NumericUpDown();
            this.TimerLabel = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.InComLbl = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.PadInPanel.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.TextInPanel.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PadXY_View)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.PadZ_View)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TimerIntSelect)).BeginInit();
            this.SuspendLayout();
            // 
            // OutComTxt
            // 
            this.OutComTxt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OutComTxt.Location = new System.Drawing.Point(3, 244);
            this.OutComTxt.Name = "OutComTxt";
            this.OutComTxt.Size = new System.Drawing.Size(241, 195);
            this.OutComTxt.TabIndex = 1;
            this.OutComTxt.Text = "";
            this.OutComTxt.TextChanged += new System.EventHandler(this.OutComTxt_TextChanged);
            // 
            // OutComLbl
            // 
            this.OutComLbl.AutoSize = true;
            this.OutComLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OutComLbl.Location = new System.Drawing.Point(9, 228);
            this.OutComLbl.Name = "OutComLbl";
            this.OutComLbl.Size = new System.Drawing.Size(76, 13);
            this.OutComLbl.TabIndex = 3;
            this.OutComLbl.Text = "COM Output";
            // 
            // OpenFileDlg
            // 
            this.OpenFileDlg.InitialDirectory = "My Documents";
            this.OpenFileDlg.FileOk += new System.ComponentModel.CancelEventHandler(this.OpenFileDlg_FileOk);
            // 
            // MyVCOM
            // 
            this.MyVCOM.BaudRate = 115200;
            this.MyVCOM.DtrEnable = true;
            this.MyVCOM.PortName = "COM5";
            this.MyVCOM.ReadBufferSize = 2048;
            this.MyVCOM.ReadTimeout = 5;
            this.MyVCOM.WriteTimeout = 1000;
            // 
            // SendBtn
            // 
            this.SendBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SendBtn.Enabled = false;
            this.SendBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SendBtn.Location = new System.Drawing.Point(5, 195);
            this.SendBtn.Name = "SendBtn";
            this.SendBtn.Size = new System.Drawing.Size(241, 23);
            this.SendBtn.TabIndex = 6;
            this.SendBtn.Text = "Send Input";
            this.SendBtn.UseVisualStyleBackColor = true;
            this.SendBtn.Click += new System.EventHandler(this.SendBtn_Click);
            // 
            // SaveFileDlg
            // 
            this.SaveFileDlg.InitialDirectory = "My Documents";
            this.SaveFileDlg.FileOk += new System.ComponentModel.CancelEventHandler(this.SaveFileDlg_FileOk);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.PortNumber);
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.BaudSelect);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox1.Location = new System.Drawing.Point(6, 72);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(240, 88);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "COM Settings";
            // 
            // PortNumber
            // 
            this.PortNumber.FormattingEnabled = true;
            this.PortNumber.Location = new System.Drawing.Point(51, 57);
            this.PortNumber.Name = "PortNumber";
            this.PortNumber.Size = new System.Drawing.Size(92, 21);
            this.PortNumber.TabIndex = 8;
            // 
            // textBox2
            // 
            this.textBox2.Enabled = false;
            this.textBox2.Location = new System.Drawing.Point(190, 23);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(44, 20);
            this.textBox2.TabIndex = 7;
            this.textBox2.Text = "None";
            this.textBox2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.Location = new System.Drawing.Point(152, 27);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(36, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Parity:";
            // 
            // textBox1
            // 
            this.textBox1.Enabled = false;
            this.textBox1.Location = new System.Drawing.Point(210, 53);
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            this.textBox1.Size = new System.Drawing.Size(16, 20);
            this.textBox1.TabIndex = 5;
            this.textBox1.Text = "1";
            this.textBox1.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.Location = new System.Drawing.Point(152, 57);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(52, 13);
            this.label5.TabIndex = 4;
            this.label5.Text = "Stop Bits:";
            // 
            // BaudSelect
            // 
            this.BaudSelect.FormattingEnabled = true;
            this.BaudSelect.Items.AddRange(new object[] {
            "9600",
            "38400",
            "57600",
            "115200"});
            this.BaudSelect.Location = new System.Drawing.Point(67, 23);
            this.BaudSelect.Name = "BaudSelect";
            this.BaudSelect.Size = new System.Drawing.Size(76, 21);
            this.BaudSelect.TabIndex = 3;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.Location = new System.Drawing.Point(6, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Baud Rate:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(6, 60);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Port #:";
            // 
            // ConnReset
            // 
            this.ConnReset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ConnReset.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ConnReset.Location = new System.Drawing.Point(6, 166);
            this.ConnReset.Name = "ConnReset";
            this.ConnReset.Size = new System.Drawing.Size(241, 23);
            this.ConnReset.TabIndex = 8;
            this.ConnReset.Text = "Open Connection";
            this.ConnReset.UseVisualStyleBackColor = true;
            this.ConnReset.Click += new System.EventHandler(this.ConnReset_Click);
            // 
            // SendTimer
            // 
            this.SendTimer.Interval = 50;
            this.SendTimer.Tick += new System.EventHandler(this.SendTimer_Tick);
            // 
            // ReadTimer
            // 
            this.ReadTimer.Interval = 2;
            this.ReadTimer.Tick += new System.EventHandler(this.ReadTimer_Tick);
            // 
            // ClearOutBtn
            // 
            this.ClearOutBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ClearOutBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ClearOutBtn.Location = new System.Drawing.Point(4, 445);
            this.ClearOutBtn.Name = "ClearOutBtn";
            this.ClearOutBtn.Size = new System.Drawing.Size(241, 23);
            this.ClearOutBtn.TabIndex = 10;
            this.ClearOutBtn.Text = "Clear";
            this.ClearOutBtn.UseVisualStyleBackColor = true;
            this.ClearOutBtn.Click += new System.EventHandler(this.ClearOutBtn_Click);
            // 
            // PadInPanel
            // 
            this.PadInPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.PadInPanel.Controls.Add(this.groupBox3);
            this.PadInPanel.Location = new System.Drawing.Point(3, 3);
            this.PadInPanel.Name = "PadInPanel";
            this.PadInPanel.Size = new System.Drawing.Size(247, 61);
            this.PadInPanel.TabIndex = 14;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnCtlSettings);
            this.groupBox3.Controls.Add(this.ControllerSelect);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox3.Location = new System.Drawing.Point(0, 0);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(247, 61);
            this.groupBox3.TabIndex = 0;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Controller";
            // 
            // btnCtlSettings
            // 
            this.btnCtlSettings.Enabled = false;
            this.btnCtlSettings.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCtlSettings.Location = new System.Drawing.Point(11, 36);
            this.btnCtlSettings.Name = "btnCtlSettings";
            this.btnCtlSettings.Size = new System.Drawing.Size(227, 22);
            this.btnCtlSettings.TabIndex = 1;
            this.btnCtlSettings.Text = "Controller Settings";
            this.btnCtlSettings.UseVisualStyleBackColor = true;
            this.btnCtlSettings.Click += new System.EventHandler(this.btnCtlSettings_Click);
            // 
            // ControllerSelect
            // 
            this.ControllerSelect.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ControllerSelect.FormattingEnabled = true;
            this.ControllerSelect.Location = new System.Drawing.Point(12, 14);
            this.ControllerSelect.Name = "ControllerSelect";
            this.ControllerSelect.Size = new System.Drawing.Size(226, 21);
            this.ControllerSelect.TabIndex = 0;
            this.ControllerSelect.SelectedIndexChanged += new System.EventHandler(this.ControllerSelect_SelectedIndexChanged);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 36.66191F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 63.33809F));
            this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(701, 483);
            this.tableLayoutPanel1.TabIndex = 22;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.ClearOutBtn);
            this.panel2.Controls.Add(this.groupBox1);
            this.panel2.Controls.Add(this.OutComLbl);
            this.panel2.Controls.Add(this.SendBtn);
            this.panel2.Controls.Add(this.OutComTxt);
            this.panel2.Controls.Add(this.ConnReset);
            this.panel2.Controls.Add(this.TextInPanel);
            this.panel2.Controls.Add(this.PadInPanel);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(3, 3);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(250, 477);
            this.panel2.TabIndex = 23;
            // 
            // TextInPanel
            // 
            this.TextInPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextInPanel.Controls.Add(this.SaveFileBtn);
            this.TextInPanel.Controls.Add(this.OpenFileBtn);
            this.TextInPanel.Location = new System.Drawing.Point(1, 1);
            this.TextInPanel.Name = "TextInPanel";
            this.TextInPanel.Size = new System.Drawing.Size(251, 61);
            this.TextInPanel.TabIndex = 17;
            // 
            // SaveFileBtn
            // 
            this.SaveFileBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SaveFileBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SaveFileBtn.Location = new System.Drawing.Point(5, 33);
            this.SaveFileBtn.Name = "SaveFileBtn";
            this.SaveFileBtn.Size = new System.Drawing.Size(241, 23);
            this.SaveFileBtn.TabIndex = 5;
            this.SaveFileBtn.Text = "Save File";
            this.SaveFileBtn.UseVisualStyleBackColor = true;
            this.SaveFileBtn.Click += new System.EventHandler(this.SaveFileBtn_Click);
            // 
            // OpenFileBtn
            // 
            this.OpenFileBtn.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.OpenFileBtn.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OpenFileBtn.Location = new System.Drawing.Point(5, 4);
            this.OpenFileBtn.Name = "OpenFileBtn";
            this.OpenFileBtn.Size = new System.Drawing.Size(241, 23);
            this.OpenFileBtn.TabIndex = 4;
            this.OpenFileBtn.Text = "Open File";
            this.OpenFileBtn.UseVisualStyleBackColor = true;
            this.OpenFileBtn.Click += new System.EventHandler(this.OpenFileBtn_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.InComTxt);
            this.panel1.Controls.Add(this.PadXY_View);
            this.panel1.Controls.Add(this.PadZ_View);
            this.panel1.Controls.Add(this.label8);
            this.panel1.Controls.Add(this.groupBox2);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.InComLbl);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(259, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(439, 477);
            this.panel1.TabIndex = 23;
            // 
            // InComTxt
            // 
            this.InComTxt.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.InComTxt.Location = new System.Drawing.Point(5, 85);
            this.InComTxt.Name = "InComTxt";
            this.InComTxt.Size = new System.Drawing.Size(431, 383);
            this.InComTxt.TabIndex = 12;
            this.InComTxt.Text = "";
            this.InComTxt.TextChanged += new System.EventHandler(this.InComTxt_TextChanged);
            // 
            // PadXY_View
            // 
            this.PadXY_View.BackColor = System.Drawing.Color.White;
            this.PadXY_View.Location = new System.Drawing.Point(258, 90);
            this.PadXY_View.Name = "PadXY_View";
            this.PadXY_View.Size = new System.Drawing.Size(138, 128);
            this.PadXY_View.TabIndex = 21;
            this.PadXY_View.TabStop = false;
            // 
            // PadZ_View
            // 
            this.PadZ_View.BackColor = System.Drawing.Color.White;
            this.PadZ_View.Location = new System.Drawing.Point(65, 90);
            this.PadZ_View.Name = "PadZ_View";
            this.PadZ_View.Size = new System.Drawing.Size(138, 128);
            this.PadZ_View.TabIndex = 20;
            this.PadZ_View.TabStop = false;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.Location = new System.Drawing.Point(38, 149);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(15, 13);
            this.label8.TabIndex = 19;
            this.label8.Text = "Z";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.chkRepeat);
            this.groupBox2.Controls.Add(this.radioPad);
            this.groupBox2.Controls.Add(this.radioText);
            this.groupBox2.Controls.Add(this.ConnStatusLbl);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.TimerIntSelect);
            this.groupBox2.Controls.Add(this.TimerLabel);
            this.groupBox2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(6, 5);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(430, 56);
            this.groupBox2.TabIndex = 11;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Input Settings";
            // 
            // chkRepeat
            // 
            this.chkRepeat.AutoSize = true;
            this.chkRepeat.Checked = true;
            this.chkRepeat.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkRepeat.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkRepeat.Location = new System.Drawing.Point(124, 14);
            this.chkRepeat.Name = "chkRepeat";
            this.chkRepeat.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.chkRepeat.Size = new System.Drawing.Size(164, 17);
            this.chkRepeat.TabIndex = 16;
            this.chkRepeat.Text = "Don\'t send repeat commands";
            this.chkRepeat.UseVisualStyleBackColor = true;
            // 
            // radioPad
            // 
            this.radioPad.AutoSize = true;
            this.radioPad.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioPad.Location = new System.Drawing.Point(4, 31);
            this.radioPad.Name = "radioPad";
            this.radioPad.Size = new System.Drawing.Size(69, 17);
            this.radioPad.TabIndex = 15;
            this.radioPad.Text = "Controller";
            this.radioPad.UseVisualStyleBackColor = true;
            this.radioPad.CheckedChanged += new System.EventHandler(this.radioPad_CheckedChanged);
            // 
            // radioText
            // 
            this.radioText.AutoSize = true;
            this.radioText.Checked = true;
            this.radioText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.radioText.Location = new System.Drawing.Point(4, 14);
            this.radioText.Name = "radioText";
            this.radioText.Size = new System.Drawing.Size(46, 17);
            this.radioText.TabIndex = 14;
            this.radioText.TabStop = true;
            this.radioText.Text = "Text";
            this.radioText.UseVisualStyleBackColor = true;
            // 
            // ConnStatusLbl
            // 
            this.ConnStatusLbl.Location = new System.Drawing.Point(324, 35);
            this.ConnStatusLbl.Name = "ConnStatusLbl";
            this.ConnStatusLbl.Size = new System.Drawing.Size(100, 13);
            this.ConnStatusLbl.TabIndex = 13;
            this.ConnStatusLbl.Text = "Not Connected";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.Location = new System.Drawing.Point(324, 14);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(97, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Connection Status:";
            // 
            // TimerIntSelect
            // 
            this.TimerIntSelect.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.TimerIntSelect.Location = new System.Drawing.Point(172, 32);
            this.TimerIntSelect.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.TimerIntSelect.Name = "TimerIntSelect";
            this.TimerIntSelect.Size = new System.Drawing.Size(53, 20);
            this.TimerIntSelect.TabIndex = 11;
            // 
            // TimerLabel
            // 
            this.TimerLabel.AutoSize = true;
            this.TimerLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TimerLabel.Location = new System.Drawing.Point(121, 34);
            this.TimerLabel.Name = "TimerLabel";
            this.TimerLabel.Size = new System.Drawing.Size(45, 13);
            this.TimerLabel.TabIndex = 10;
            this.TimerLabel.Text = "Interval:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(237, 149);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(15, 13);
            this.label2.TabIndex = 18;
            this.label2.Text = "Y";
            // 
            // InComLbl
            // 
            this.InComLbl.AutoSize = true;
            this.InComLbl.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InComLbl.Location = new System.Drawing.Point(7, 68);
            this.InComLbl.Name = "InComLbl";
            this.InComLbl.Size = new System.Drawing.Size(67, 13);
            this.InComLbl.TabIndex = 2;
            this.InComLbl.Text = "COM Input";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(323, 223);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(15, 13);
            this.label1.TabIndex = 17;
            this.label1.Text = "X";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(701, 483);
            this.Controls.Add(this.tableLayoutPanel1);
            this.KeyPreview = true;
            this.Name = "Form1";
            this.Text = "BOT COM";
            this.Load += new System.EventHandler(this.Form_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.Form_KeyDown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.Form_KeyPress);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.Form_KeyUp);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.Form_PreviewKeyDown);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.PadInPanel.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.TextInPanel.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PadXY_View)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.PadZ_View)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TimerIntSelect)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RichTextBox OutComTxt;
        private System.Windows.Forms.Label OutComLbl;
        private System.Windows.Forms.OpenFileDialog OpenFileDlg;
        private System.IO.Ports.SerialPort MyVCOM;
        private System.Windows.Forms.Button SendBtn;
        private System.Windows.Forms.SaveFileDialog SaveFileDlg;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox BaudSelect;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button ConnReset;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Timer SendTimer;
        private System.Windows.Forms.Timer ReadTimer;
        private System.Windows.Forms.ToolTip ttTimer;
        private System.Windows.Forms.Button ClearOutBtn;
        private System.Windows.Forms.Panel PadInPanel;
        private System.ComponentModel.BackgroundWorker ControllerPoller;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox ControllerSelect;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RichTextBox InComTxt;
        private System.Windows.Forms.PictureBox PadXY_View;
        private System.Windows.Forms.PictureBox PadZ_View;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radioPad;
        private System.Windows.Forms.RadioButton radioText;
        private System.Windows.Forms.Label ConnStatusLbl;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown TimerIntSelect;
        private System.Windows.Forms.Label TimerLabel;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label InComLbl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel TextInPanel;
        private System.Windows.Forms.Button SaveFileBtn;
        private System.Windows.Forms.Button OpenFileBtn;
        private System.Windows.Forms.ComboBox PortNumber;
        private System.Windows.Forms.CheckBox chkRepeat;
        private System.Windows.Forms.Button btnCtlSettings;
    }
}

