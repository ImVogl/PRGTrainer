namespace SignExtractor.MainHost
{
    partial class MainHostView
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
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.btnSelectWorkDir = new System.Windows.Forms.Button();
            this.PathToImageLabel = new System.Windows.Forms.Label();
            this.btnPreviousImage = new System.Windows.Forms.Button();
            this.btnNextImage = new System.Windows.Forms.Button();
            this.btnRemoveImage = new System.Windows.Forms.Button();
            this.scbScale = new System.Windows.Forms.HScrollBar();
            this.lbScale = new System.Windows.Forms.Label();
            this.btnSaveResult = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(61, 4);
            this.contextMenuStrip2.Text = "File";
            // 
            // btnSelectWorkDir
            // 
            this.btnSelectWorkDir.Location = new System.Drawing.Point(480, 10);
            this.btnSelectWorkDir.Name = "btnSelectWorkDir";
            this.btnSelectWorkDir.Size = new System.Drawing.Size(90, 25);
            this.btnSelectWorkDir.TabIndex = 2;
            this.btnSelectWorkDir.Text = "Рабочая папка";
            this.btnSelectWorkDir.UseVisualStyleBackColor = true;
            // 
            // PathToImageLabel
            // 
            this.PathToImageLabel.AutoSize = true;
            this.PathToImageLabel.Font = new System.Drawing.Font("Times New Roman", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.PathToImageLabel.Location = new System.Drawing.Point(120, 260);
            this.PathToImageLabel.Name = "PathToImageLabel";
            this.PathToImageLabel.Size = new System.Drawing.Size(243, 21);
            this.PathToImageLabel.TabIndex = 3;
            this.PathToImageLabel.Text = "Имя обрабатываемого файла";
            // 
            // btnPreviousImage
            // 
            this.btnPreviousImage.Location = new System.Drawing.Point(10, 260);
            this.btnPreviousImage.Name = "btnPreviousImage";
            this.btnPreviousImage.Size = new System.Drawing.Size(50, 25);
            this.btnPreviousImage.TabIndex = 4;
            this.btnPreviousImage.Text = "←";
            this.btnPreviousImage.UseVisualStyleBackColor = true;
            // 
            // btnNextImage
            // 
            this.btnNextImage.Location = new System.Drawing.Point(60, 260);
            this.btnNextImage.Name = "btnNextImage";
            this.btnNextImage.Size = new System.Drawing.Size(50, 25);
            this.btnNextImage.TabIndex = 5;
            this.btnNextImage.Text = "→";
            this.btnNextImage.UseVisualStyleBackColor = true;
            // 
            // btnRemoveImage
            // 
            this.btnRemoveImage.Location = new System.Drawing.Point(480, 35);
            this.btnRemoveImage.Name = "btnRemoveImage";
            this.btnRemoveImage.Size = new System.Drawing.Size(90, 25);
            this.btnRemoveImage.TabIndex = 6;
            this.btnRemoveImage.Text = "Удалить";
            this.btnRemoveImage.UseVisualStyleBackColor = true;
            // 
            // scbScale
            // 
            this.scbScale.Location = new System.Drawing.Point(390, 260);
            this.scbScale.Name = "scbScale";
            this.scbScale.Size = new System.Drawing.Size(180, 25);
            this.scbScale.TabIndex = 7;
            this.scbScale.Value = 100;
            // 
            // lbScale
            // 
            this.lbScale.AutoSize = true;
            this.lbScale.Font = new System.Drawing.Font("Times New Roman", 14.25F);
            this.lbScale.Location = new System.Drawing.Point(390, 285);
            this.lbScale.Name = "lbScale";
            this.lbScale.Size = new System.Drawing.Size(83, 21);
            this.lbScale.TabIndex = 8;
            this.lbScale.Text = "Масштаб";
            // 
            // btnSaveResult
            // 
            this.btnSaveResult.Location = new System.Drawing.Point(480, 60);
            this.btnSaveResult.Name = "btnSaveResult";
            this.btnSaveResult.Size = new System.Drawing.Size(90, 25);
            this.btnSaveResult.TabIndex = 9;
            this.btnSaveResult.Text = "Сохранить";
            this.btnSaveResult.UseVisualStyleBackColor = true;
            // 
            // MainHostView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 311);
            this.Controls.Add(this.btnSaveResult);
            this.Controls.Add(this.lbScale);
            this.Controls.Add(this.scbScale);
            this.Controls.Add(this.btnRemoveImage);
            this.Controls.Add(this.btnNextImage);
            this.Controls.Add(this.btnPreviousImage);
            this.Controls.Add(this.PathToImageLabel);
            this.Controls.Add(this.btnSelectWorkDir);
            this.MinimumSize = new System.Drawing.Size(600, 350);
            this.Name = "MainHostView";
            this.Text = "Экстрактор признаков";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.Button btnSelectWorkDir;
        internal System.Windows.Forms.Label PathToImageLabel;
        private System.Windows.Forms.Button btnPreviousImage;
        private System.Windows.Forms.Button btnNextImage;
        private System.Windows.Forms.Button btnRemoveImage;
        private System.Windows.Forms.Label lbScale;
        internal System.Windows.Forms.HScrollBar scbScale;
        private System.Windows.Forms.Button btnSaveResult;
    }
}

