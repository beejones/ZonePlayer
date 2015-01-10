using System.Runtime.InteropServices;
namespace AxVlcPanel
{
    partial class AxVlcPlayer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AxVlcPlayer));
            this.axVlc = new AxAXVLC.AxVLCPlugin2();
            ((System.ComponentModel.ISupportInitialize)(this.axVlc)).BeginInit();
            this.SuspendLayout();
            // 
            // axVlc
            // 
            this.axVlc.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axVlc.Enabled = true;
            this.axVlc.Location = new System.Drawing.Point(0, 0);
            this.axVlc.Name = "axVlc";
            this.axVlc.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axVlc.OcxState")));
            this.axVlc.Size = new System.Drawing.Size(150, 150);
            this.axVlc.TabIndex = 0;
            // 
            // AxVlcPlayer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.axVlc);
            this.Name = "AxVlcPlayer";
            try
            {
                ((System.ComponentModel.ISupportInitialize)(this.axVlc)).EndInit();
            }
            catch (COMException e)
            {
                //No activex installed
                this.axVlc = null;
                return;
            }

            this.ResumeLayout(false);
        }

        #endregion

        public AxAXVLC.AxVLCPlugin2 axVlc;
    }
}
