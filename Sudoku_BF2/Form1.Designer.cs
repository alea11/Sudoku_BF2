
namespace Sudoku_BF
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblRes = new System.Windows.Forms.Label();
            this.btnWork = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.lblErr = new System.Windows.Forms.Label();
            this.pnlService = new System.Windows.Forms.Panel();
            this.btnLoad = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblCount = new System.Windows.Forms.Label();
            this.btnToInit = new System.Windows.Forms.Button();
            this.btnStop = new System.Windows.Forms.Button();
            this.cmbMethod = new System.Windows.Forms.ComboBox();
            this.btnResize = new System.Windows.Forms.Button();
            this.txtA = new System.Windows.Forms.TextBox();
            this.pnlGrid = new System.Windows.Forms.Panel();
            this.pnlService.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblRes
            // 
            this.lblRes.AutoSize = true;
            this.lblRes.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblRes.Location = new System.Drawing.Point(11, 88);
            this.lblRes.Name = "lblRes";
            this.lblRes.Size = new System.Drawing.Size(0, 13);
            this.lblRes.TabIndex = 1;
            // 
            // btnWork
            // 
            this.btnWork.Location = new System.Drawing.Point(11, 59);
            this.btnWork.Name = "btnWork";
            this.btnWork.Size = new System.Drawing.Size(85, 23);
            this.btnWork.TabIndex = 2;
            this.btnWork.Text = "Решить";
            this.btnWork.UseVisualStyleBackColor = true;
            this.btnWork.Click += new System.EventHandler(this.btnWork_Click);
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(133, 30);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(85, 23);
            this.btnClear.TabIndex = 5;
            this.btnClear.Text = "Очистить";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // lblErr
            // 
            this.lblErr.AutoSize = true;
            this.lblErr.ForeColor = System.Drawing.Color.Red;
            this.lblErr.Location = new System.Drawing.Point(11, 7);
            this.lblErr.Name = "lblErr";
            this.lblErr.Size = new System.Drawing.Size(0, 13);
            this.lblErr.TabIndex = 6;
            // 
            // pnlService
            // 
            this.pnlService.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlService.Controls.Add(this.btnLoad);
            this.pnlService.Controls.Add(this.btnSave);
            this.pnlService.Controls.Add(this.lblCount);
            this.pnlService.Controls.Add(this.btnToInit);
            this.pnlService.Controls.Add(this.btnStop);
            this.pnlService.Controls.Add(this.cmbMethod);
            this.pnlService.Controls.Add(this.btnResize);
            this.pnlService.Controls.Add(this.txtA);
            this.pnlService.Controls.Add(this.btnWork);
            this.pnlService.Controls.Add(this.lblErr);
            this.pnlService.Controls.Add(this.btnClear);
            this.pnlService.Controls.Add(this.lblRes);
            this.pnlService.Location = new System.Drawing.Point(8, 421);
            this.pnlService.Name = "pnlService";
            this.pnlService.Size = new System.Drawing.Size(332, 170);
            this.pnlService.TabIndex = 7;
            // 
            // btnLoad
            // 
            this.btnLoad.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnLoad.Location = new System.Drawing.Point(147, 141);
            this.btnLoad.Name = "btnLoad";
            this.btnLoad.Size = new System.Drawing.Size(85, 23);
            this.btnLoad.TabIndex = 15;
            this.btnLoad.Text = "Загрузить";
            this.btnLoad.UseVisualStyleBackColor = true;
            this.btnLoad.Click += new System.EventHandler(this.btnLoad_Click);
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.Location = new System.Drawing.Point(242, 141);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(85, 23);
            this.btnSave.TabIndex = 14;
            this.btnSave.Text = "Сохранить";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblCount
            // 
            this.lblCount.AutoSize = true;
            this.lblCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lblCount.Location = new System.Drawing.Point(168, 88);
            this.lblCount.Name = "lblCount";
            this.lblCount.Size = new System.Drawing.Size(0, 13);
            this.lblCount.TabIndex = 13;
            // 
            // btnToInit
            // 
            this.btnToInit.Location = new System.Drawing.Point(11, 112);
            this.btnToInit.Name = "btnToInit";
            this.btnToInit.Size = new System.Drawing.Size(164, 23);
            this.btnToInit.TabIndex = 12;
            this.btnToInit.Text = "Вернуться к нач. условиям";
            this.btnToInit.UseVisualStyleBackColor = true;
            this.btnToInit.Click += new System.EventHandler(this.btnToInit_Click);
            // 
            // btnStop
            // 
            this.btnStop.Enabled = false;
            this.btnStop.Location = new System.Drawing.Point(133, 59);
            this.btnStop.Name = "btnStop";
            this.btnStop.Size = new System.Drawing.Size(85, 23);
            this.btnStop.TabIndex = 11;
            this.btnStop.Text = "Остановить";
            this.btnStop.UseVisualStyleBackColor = true;
            this.btnStop.Click += new System.EventHandler(this.btnStop_Click);
            // 
            // cmbMethod
            // 
            this.cmbMethod.FormattingEnabled = true;
            this.cmbMethod.Location = new System.Drawing.Point(262, 7);
            this.cmbMethod.Name = "cmbMethod";
            this.cmbMethod.Size = new System.Drawing.Size(114, 21);
            this.cmbMethod.TabIndex = 10;
            this.cmbMethod.Visible = false;
            this.cmbMethod.SelectedIndexChanged += new System.EventHandler(this.cmbMethod_SelectedIndexChanged);
            // 
            // btnResize
            // 
            this.btnResize.Location = new System.Drawing.Point(11, 30);
            this.btnResize.Name = "btnResize";
            this.btnResize.Size = new System.Drawing.Size(85, 23);
            this.btnResize.TabIndex = 9;
            this.btnResize.Text = "Уст. размер";
            this.btnResize.UseVisualStyleBackColor = true;
            this.btnResize.Click += new System.EventHandler(this.btnResize_Click);
            // 
            // txtA
            // 
            this.txtA.BackColor = System.Drawing.Color.White;
            this.txtA.Location = new System.Drawing.Point(102, 32);
            this.txtA.MaxLength = 1;
            this.txtA.Name = "txtA";
            this.txtA.Size = new System.Drawing.Size(20, 20);
            this.txtA.TabIndex = 8;
            this.txtA.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TxtA_KeyPress);
            // 
            // pnlGrid
            // 
            this.pnlGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlGrid.AutoScroll = true;
            this.pnlGrid.Location = new System.Drawing.Point(0, 0);
            this.pnlGrid.Name = "pnlGrid";
            this.pnlGrid.Size = new System.Drawing.Size(351, 415);
            this.pnlGrid.TabIndex = 8;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(352, 596);
            this.Controls.Add(this.pnlGrid);
            this.Controls.Add(this.pnlService);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Load += new System.EventHandler(this.Form1_Load);
            this.pnlService.ResumeLayout(false);
            this.pnlService.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label lblRes;
        private System.Windows.Forms.Button btnWork;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.Label lblErr;
        private System.Windows.Forms.Panel pnlService;
        private System.Windows.Forms.TextBox txtA;
        private System.Windows.Forms.Button btnResize;
        private System.Windows.Forms.ComboBox cmbMethod;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnToInit;
        private System.Windows.Forms.Label lblCount;
        private System.Windows.Forms.Panel pnlGrid;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Button btnLoad;
    }
}

