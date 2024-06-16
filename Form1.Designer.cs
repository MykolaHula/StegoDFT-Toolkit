namespace StegoDFT_Toolkit
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            groupBox1 = new GroupBox();
            groupBox3 = new GroupBox();
            acceleratorCombo = new ComboBox();
            groupBox2 = new GroupBox();
            computingModeCombo = new ComboBox();
            groupBox4 = new GroupBox();
            fillUpDown = new NumericUpDown();
            groupBox9 = new GroupBox();
            analizeButton = new Button();
            groupBox12 = new GroupBox();
            analizeMethodCombo = new ComboBox();
            groupBox11 = new GroupBox();
            analizeItemCombo = new ComboBox();
            groupBox8 = new GroupBox();
            extractButton = new Button();
            saveButton = new Button();
            groupBox10 = new GroupBox();
            label5 = new Label();
            DIModeBox = new ComboBox();
            EmbedButton = new Button();
            groupBox7 = new GroupBox();
            blocksTextBox = new TextBox();
            sizeTextBox = new TextBox();
            nameTextBox = new TextBox();
            pathTextBox = new TextBox();
            label4 = new Label();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
            LoadButton = new Button();
            groupBox6 = new GroupBox();
            steganomessageBox = new PictureBox();
            groupBox5 = new GroupBox();
            inputImageBox = new PictureBox();
            progressBar1 = new ProgressBar();
            groupBox1.SuspendLayout();
            groupBox3.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)fillUpDown).BeginInit();
            groupBox9.SuspendLayout();
            groupBox12.SuspendLayout();
            groupBox11.SuspendLayout();
            groupBox8.SuspendLayout();
            groupBox10.SuspendLayout();
            groupBox7.SuspendLayout();
            groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)steganomessageBox).BeginInit();
            groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)inputImageBox).BeginInit();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(groupBox3);
            groupBox1.Controls.Add(groupBox2);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(629, 82);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Опції обробки";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(acceleratorCombo);
            groupBox3.Location = new Point(318, 22);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(305, 54);
            groupBox3.TabIndex = 1;
            groupBox3.TabStop = false;
            groupBox3.Text = "Прискорювач";
            // 
            // acceleratorCombo
            // 
            acceleratorCombo.FormattingEnabled = true;
            acceleratorCombo.Location = new Point(6, 22);
            acceleratorCombo.Name = "acceleratorCombo";
            acceleratorCombo.Size = new Size(293, 23);
            acceleratorCombo.TabIndex = 1;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(computingModeCombo);
            groupBox2.Location = new Point(6, 22);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(305, 54);
            groupBox2.TabIndex = 0;
            groupBox2.TabStop = false;
            groupBox2.Text = "Режим виконання";
            // 
            // computingModeCombo
            // 
            computingModeCombo.FormattingEnabled = true;
            computingModeCombo.Location = new Point(6, 22);
            computingModeCombo.Name = "computingModeCombo";
            computingModeCombo.Size = new Size(293, 23);
            computingModeCombo.TabIndex = 0;
            computingModeCombo.SelectedIndexChanged += computingModeCombo_SelectedIndexChanged;
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(fillUpDown);
            groupBox4.Controls.Add(groupBox9);
            groupBox4.Controls.Add(groupBox8);
            groupBox4.Controls.Add(groupBox7);
            groupBox4.Controls.Add(groupBox6);
            groupBox4.Controls.Add(groupBox5);
            groupBox4.Location = new Point(12, 100);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(629, 524);
            groupBox4.TabIndex = 1;
            groupBox4.TabStop = false;
            groupBox4.Text = "Дослідження цифрових зображень";
            // 
            // fillUpDown
            // 
            fillUpDown.Location = new Point(353, 392);
            fillUpDown.Name = "fillUpDown";
            fillUpDown.Size = new Size(51, 23);
            fillUpDown.TabIndex = 5;
            fillUpDown.Value = new decimal(new int[] { 100, 0, 0, 0 });
            // 
            // groupBox9
            // 
            groupBox9.Controls.Add(analizeButton);
            groupBox9.Controls.Add(groupBox12);
            groupBox9.Controls.Add(groupBox11);
            groupBox9.Location = new Point(422, 316);
            groupBox9.Name = "groupBox9";
            groupBox9.Size = new Size(200, 202);
            groupBox9.TabIndex = 4;
            groupBox9.TabStop = false;
            groupBox9.Text = "Стеганоаналіз";
            // 
            // analizeButton
            // 
            analizeButton.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 204);
            analizeButton.Location = new Point(6, 172);
            analizeButton.Name = "analizeButton";
            analizeButton.Size = new Size(188, 23);
            analizeButton.TabIndex = 6;
            analizeButton.Text = "Дослідити";
            analizeButton.UseVisualStyleBackColor = true;
            analizeButton.Click += analizeButton_Click;
            // 
            // groupBox12
            // 
            groupBox12.Controls.Add(analizeMethodCombo);
            groupBox12.Location = new Point(6, 89);
            groupBox12.Name = "groupBox12";
            groupBox12.Size = new Size(188, 61);
            groupBox12.TabIndex = 5;
            groupBox12.TabStop = false;
            groupBox12.Text = "Метод стеганоаналізу";
            // 
            // analizeMethodCombo
            // 
            analizeMethodCombo.Location = new Point(6, 26);
            analizeMethodCombo.Name = "analizeMethodCombo";
            analizeMethodCombo.Size = new Size(176, 23);
            analizeMethodCombo.TabIndex = 2;
            // 
            // groupBox11
            // 
            groupBox11.Controls.Add(analizeItemCombo);
            groupBox11.Location = new Point(6, 22);
            groupBox11.Name = "groupBox11";
            groupBox11.Size = new Size(188, 61);
            groupBox11.TabIndex = 4;
            groupBox11.TabStop = false;
            groupBox11.Text = "Досліджуване зображення ";
            // 
            // analizeItemCombo
            // 
            analizeItemCombo.FormattingEnabled = true;
            analizeItemCombo.Location = new Point(6, 26);
            analizeItemCombo.Name = "analizeItemCombo";
            analizeItemCombo.Size = new Size(176, 23);
            analizeItemCombo.TabIndex = 2;
            // 
            // groupBox8
            // 
            groupBox8.Controls.Add(extractButton);
            groupBox8.Controls.Add(saveButton);
            groupBox8.Controls.Add(groupBox10);
            groupBox8.Controls.Add(EmbedButton);
            groupBox8.Location = new Point(212, 316);
            groupBox8.Name = "groupBox8";
            groupBox8.Size = new Size(204, 202);
            groupBox8.TabIndex = 3;
            groupBox8.TabStop = false;
            groupBox8.Text = "Стеганографія";
            // 
            // extractButton
            // 
            extractButton.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 204);
            extractButton.Location = new Point(10, 143);
            extractButton.Name = "extractButton";
            extractButton.Size = new Size(188, 23);
            extractButton.TabIndex = 5;
            extractButton.Text = "Витягти ДІ";
            extractButton.UseVisualStyleBackColor = true;
            extractButton.Click += extractButton_Click;
            // 
            // saveButton
            // 
            saveButton.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 204);
            saveButton.Location = new Point(10, 172);
            saveButton.Name = "saveButton";
            saveButton.Size = new Size(188, 23);
            saveButton.TabIndex = 4;
            saveButton.Text = "Зберегти стеганоповідомлення";
            saveButton.UseVisualStyleBackColor = true;
            saveButton.Click += saveButton_Click;
            // 
            // groupBox10
            // 
            groupBox10.Controls.Add(label5);
            groupBox10.Controls.Add(DIModeBox);
            groupBox10.Location = new Point(10, 22);
            groupBox10.Name = "groupBox10";
            groupBox10.Size = new Size(188, 82);
            groupBox10.TabIndex = 3;
            groupBox10.TabStop = false;
            groupBox10.Text = "Параметри генрації ДІ";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(6, 56);
            label5.Name = "label5";
            label5.Size = new Size(119, 15);
            label5.TabIndex = 4;
            label5.Text = "Ступінь заповнення:";
            // 
            // DIModeBox
            // 
            DIModeBox.FormattingEnabled = true;
            DIModeBox.Location = new Point(6, 26);
            DIModeBox.Name = "DIModeBox";
            DIModeBox.Size = new Size(176, 23);
            DIModeBox.TabIndex = 2;
            // 
            // EmbedButton
            // 
            EmbedButton.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 204);
            EmbedButton.Location = new Point(10, 114);
            EmbedButton.Name = "EmbedButton";
            EmbedButton.Size = new Size(188, 23);
            EmbedButton.TabIndex = 1;
            EmbedButton.Text = "Занурити ДІ";
            EmbedButton.UseVisualStyleBackColor = true;
            EmbedButton.Click += EmbedButton_Click;
            // 
            // groupBox7
            // 
            groupBox7.Controls.Add(blocksTextBox);
            groupBox7.Controls.Add(sizeTextBox);
            groupBox7.Controls.Add(nameTextBox);
            groupBox7.Controls.Add(pathTextBox);
            groupBox7.Controls.Add(label4);
            groupBox7.Controls.Add(label3);
            groupBox7.Controls.Add(label2);
            groupBox7.Controls.Add(label1);
            groupBox7.Controls.Add(LoadButton);
            groupBox7.Location = new Point(6, 316);
            groupBox7.Name = "groupBox7";
            groupBox7.Size = new Size(200, 202);
            groupBox7.TabIndex = 2;
            groupBox7.TabStop = false;
            groupBox7.Text = "Вхідні дані";
            // 
            // blocksTextBox
            // 
            blocksTextBox.Location = new Point(94, 115);
            blocksTextBox.Name = "blocksTextBox";
            blocksTextBox.ReadOnly = true;
            blocksTextBox.Size = new Size(100, 23);
            blocksTextBox.TabIndex = 6;
            // 
            // sizeTextBox
            // 
            sizeTextBox.Location = new Point(94, 86);
            sizeTextBox.Name = "sizeTextBox";
            sizeTextBox.ReadOnly = true;
            sizeTextBox.Size = new Size(100, 23);
            sizeTextBox.TabIndex = 5;
            // 
            // nameTextBox
            // 
            nameTextBox.Location = new Point(94, 56);
            nameTextBox.Name = "nameTextBox";
            nameTextBox.ReadOnly = true;
            nameTextBox.Size = new Size(100, 23);
            nameTextBox.TabIndex = 4;
            // 
            // pathTextBox
            // 
            pathTextBox.Location = new Point(94, 27);
            pathTextBox.Name = "pathTextBox";
            pathTextBox.ReadOnly = true;
            pathTextBox.Size = new Size(100, 23);
            pathTextBox.TabIndex = 3;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(6, 118);
            label4.Name = "label4";
            label4.Size = new Size(83, 15);
            label4.TabIndex = 2;
            label4.Text = "Блоків/канал:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(6, 89);
            label3.Name = "label3";
            label3.Size = new Size(48, 15);
            label3.TabIndex = 2;
            label3.Text = "Розмір:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(6, 60);
            label2.Name = "label2";
            label2.Size = new Size(42, 15);
            label2.TabIndex = 2;
            label2.Text = "Назва:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 30);
            label1.Name = "label1";
            label1.Size = new Size(40, 15);
            label1.TabIndex = 1;
            label1.Text = "Шлях:";
            // 
            // LoadButton
            // 
            LoadButton.Font = new Font("Segoe UI", 7.8F, FontStyle.Regular, GraphicsUnit.Point, 204);
            LoadButton.Location = new Point(6, 173);
            LoadButton.Name = "LoadButton";
            LoadButton.Size = new Size(188, 23);
            LoadButton.TabIndex = 0;
            LoadButton.Text = "Завантажити зображення";
            LoadButton.UseVisualStyleBackColor = true;
            LoadButton.Click += LoadButton_Click;
            // 
            // groupBox6
            // 
            groupBox6.Controls.Add(steganomessageBox);
            groupBox6.Location = new Point(318, 22);
            groupBox6.Name = "groupBox6";
            groupBox6.Size = new Size(304, 288);
            groupBox6.TabIndex = 1;
            groupBox6.TabStop = false;
            groupBox6.Text = "Стеганоповідомлення";
            // 
            // steganomessageBox
            // 
            steganomessageBox.Location = new Point(6, 22);
            steganomessageBox.Name = "steganomessageBox";
            steganomessageBox.Size = new Size(293, 260);
            steganomessageBox.SizeMode = PictureBoxSizeMode.Zoom;
            steganomessageBox.TabIndex = 1;
            steganomessageBox.TabStop = false;
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(inputImageBox);
            groupBox5.Location = new Point(6, 22);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(305, 288);
            groupBox5.TabIndex = 0;
            groupBox5.TabStop = false;
            groupBox5.Text = "Вхідне зображення";
            // 
            // inputImageBox
            // 
            inputImageBox.Location = new Point(6, 22);
            inputImageBox.Name = "inputImageBox";
            inputImageBox.Size = new Size(293, 260);
            inputImageBox.SizeMode = PictureBoxSizeMode.Zoom;
            inputImageBox.TabIndex = 0;
            inputImageBox.TabStop = false;
            // 
            // progressBar1
            // 
            progressBar1.Location = new Point(12, 628);
            progressBar1.Name = "progressBar1";
            progressBar1.Size = new Size(629, 10);
            progressBar1.TabIndex = 2;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            ClientSize = new Size(653, 649);
            Controls.Add(progressBar1);
            Controls.Add(groupBox4);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Form1";
            Text = "StegoDFT Toolkit";
            Load += Form1_Load;
            groupBox1.ResumeLayout(false);
            groupBox3.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)fillUpDown).EndInit();
            groupBox9.ResumeLayout(false);
            groupBox12.ResumeLayout(false);
            groupBox11.ResumeLayout(false);
            groupBox8.ResumeLayout(false);
            groupBox10.ResumeLayout(false);
            groupBox10.PerformLayout();
            groupBox7.ResumeLayout(false);
            groupBox7.PerformLayout();
            groupBox6.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)steganomessageBox).EndInit();
            groupBox5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)inputImageBox).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private GroupBox groupBox3;
        private GroupBox groupBox2;
        private ComboBox acceleratorCombo;
        private ComboBox computingModeCombo;
        private GroupBox groupBox4;
        private GroupBox groupBox6;
        private GroupBox groupBox5;
        private GroupBox groupBox8;
        private GroupBox groupBox7;
        private GroupBox groupBox9;
        private Label label2;
        private Label label1;
        private Button LoadButton;
        private Label label3;
        private Label label4;
        private GroupBox groupBox10;
        private ComboBox DIModeBox;
        private Button EmbedButton;
        private GroupBox groupBox11;
        private ComboBox analizeItemCombo;
        private Button saveButton;
        private Button analizeButton;
        private GroupBox groupBox12;
        private ComboBox analizeMethodCombo;
        private PictureBox steganomessageBox;
        private PictureBox inputImageBox;
        private Button extractButton;
        private TextBox sizeTextBox;
        private TextBox nameTextBox;
        private TextBox pathTextBox;
        private TextBox blocksTextBox;
        private ProgressBar progressBar1;
        private Label label5;
        private NumericUpDown fillUpDown;
    }
}
