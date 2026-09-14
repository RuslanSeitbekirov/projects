namespace WinFormsApp1
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
            textBoxFloat = new TextBox();
            ButtonPress = new Button();
            Output = new TextBox();
            SuspendLayout();
            // 
            // textBoxFloat
            // 
            textBoxFloat.Location = new Point(329, 186);
            textBoxFloat.Name = "textBoxFloat";
            textBoxFloat.Size = new Size(125, 27);
            textBoxFloat.TabIndex = 0;
            // 
            // ButtonPress
            // 
            ButtonPress.Location = new Point(477, 184);
            ButtonPress.Name = "ButtonPress";
            ButtonPress.Size = new Size(94, 29);
            ButtonPress.TabIndex = 1;
            ButtonPress.Text = "Press";
            ButtonPress.UseVisualStyleBackColor = true;
            ButtonPress.Click += button1_Click;
            // 
            // Output
            // 
            Output.Location = new Point(330, 230);
            Output.Name = "Output";
            Output.Size = new Size(125, 27);
            Output.TabIndex = 2;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(Output);
            Controls.Add(ButtonPress);
            Controls.Add(textBoxFloat);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBoxFloat;
        private Button ButtonPress;
        private TextBox Output;
    }
}
