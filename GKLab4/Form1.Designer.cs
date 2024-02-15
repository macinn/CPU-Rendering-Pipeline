namespace CPU_Rendering
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
            Canvas = new PictureBox();
            camerButton = new Button();
            button2 = new Button();
            button3 = new Button();
            shadingBox = new ComboBox();
            comboBox2 = new ComboBox();
            fogInput = new NumericUpDown();
            linesBox = new CheckBox();
            backFaceCullingButton = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)Canvas).BeginInit();
            ((System.ComponentModel.ISupportInitialize)fogInput).BeginInit();
            SuspendLayout();
            // 
            // Canvas
            // 
            Canvas.Location = new Point(23, 14);
            Canvas.Name = "Canvas";
            Canvas.Size = new Size(603, 486);
            Canvas.TabIndex = 0;
            Canvas.TabStop = false;
            // 
            // camerButton
            // 
            camerButton.Location = new Point(709, 19);
            camerButton.Name = "camerButton";
            camerButton.Size = new Size(136, 29);
            camerButton.TabIndex = 1;
            camerButton.Text = "Change camera";
            camerButton.UseVisualStyleBackColor = true;
            camerButton.Click += camerButton_Click;
            // 
            // button2
            // 
            button2.Location = new Point(709, 54);
            button2.Name = "button2";
            button2.Size = new Size(94, 29);
            button2.TabIndex = 2;
            button2.Text = "button2";
            button2.UseVisualStyleBackColor = true;
            // 
            // button3
            // 
            button3.Location = new Point(711, 88);
            button3.Name = "button3";
            button3.Size = new Size(94, 29);
            button3.TabIndex = 3;
            button3.Text = "button3";
            button3.UseVisualStyleBackColor = true;
            // 
            // shadingBox
            // 
            shadingBox.FormattingEnabled = true;
            shadingBox.Location = new Point(714, 134);
            shadingBox.Name = "shadingBox";
            shadingBox.Size = new Size(151, 28);
            shadingBox.TabIndex = 4;
            shadingBox.SelectedIndexChanged += shadingBox_SelectedIndexChanged;
            // 
            // comboBox2
            // 
            comboBox2.FormattingEnabled = true;
            comboBox2.Location = new Point(714, 173);
            comboBox2.Name = "comboBox2";
            comboBox2.Size = new Size(151, 28);
            comboBox2.TabIndex = 5;
            // 
            // fogInput
            // 
            fogInput.DecimalPlaces = 2;
            fogInput.Increment = new decimal(new int[] { 5, 0, 0, 131072 });
            fogInput.Location = new Point(715, 217);
            fogInput.Maximum = new decimal(new int[] { 1, 0, 0, 0 });
            fogInput.Name = "fogInput";
            fogInput.Size = new Size(150, 27);
            fogInput.TabIndex = 6;
            fogInput.ValueChanged += fogInput_ValueChanged;
            // 
            // linesBox
            // 
            linesBox.AutoSize = true;
            linesBox.Location = new Point(714, 256);
            linesBox.Name = "linesBox";
            linesBox.Size = new Size(132, 24);
            linesBox.TabIndex = 7;
            linesBox.Text = "Draw only lines";
            linesBox.UseVisualStyleBackColor = true;
            linesBox.CheckedChanged += checkBox1_CheckedChanged;
            // 
            // backFaceCullingButton
            // 
            backFaceCullingButton.AutoSize = true;
            backFaceCullingButton.Checked = true;
            backFaceCullingButton.CheckState = CheckState.Checked;
            backFaceCullingButton.Location = new Point(712, 284);
            backFaceCullingButton.Name = "backFaceCullingButton";
            backFaceCullingButton.Size = new Size(138, 24);
            backFaceCullingButton.TabIndex = 8;
            backFaceCullingButton.Text = "Backface culling";
            backFaceCullingButton.UseVisualStyleBackColor = true;
            backFaceCullingButton.CheckedChanged += backFaceCullingButton_CheckedChanged;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(950, 530);
            Controls.Add(backFaceCullingButton);
            Controls.Add(linesBox);
            Controls.Add(fogInput);
            Controls.Add(comboBox2);
            Controls.Add(shadingBox);
            Controls.Add(button3);
            Controls.Add(button2);
            Controls.Add(camerButton);
            Controls.Add(Canvas);
            Name = "Form1";
            Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)Canvas).EndInit();
            ((System.ComponentModel.ISupportInitialize)fogInput).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private PictureBox Canvas;
        private Button camerButton;
        private Button button2;
        private Button button3;
        private ComboBox shadingBox;
        private ComboBox comboBox2;
        private NumericUpDown fogInput;
        private CheckBox linesBox;
        private CheckBox backFaceCullingButton;
    }
}
