namespace AxWmpPanel
{
    partial class AxWmpPlayer
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AxWmpPlayer));
            this.axWmp = new AxWMPLib.AxWindowsMediaPlayer();
            ((System.ComponentModel.ISupportInitialize)(this.axWmp)).BeginInit();
            this.SuspendLayout();
            // 
            // axWmp
            // 
            this.axWmp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axWmp.Enabled = true;
            this.axWmp.Location = new System.Drawing.Point(0, 0);
            this.axWmp.Name = "axWmp";
            this.axWmp.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axWmp.OcxState")));
            this.axWmp.Size = new System.Drawing.Size(150, 150);
            this.axWmp.TabIndex = 0;
            // 
            // AxWmpPlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.axWmp);
            this.Name = "AxWmpPlayer";
            ((System.ComponentModel.ISupportInitialize)(this.axWmp)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        public AxWMPLib.AxWindowsMediaPlayer axWmp;
        
    }
}
