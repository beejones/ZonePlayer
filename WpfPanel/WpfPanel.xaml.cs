using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfPanel
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class PanelControl : System.Windows.Controls.UserControl
    {
        public PanelControl()
        {
            InitializeComponent();
        }

        public AxAXVLC.AxVLCPlugin2 axVlcPlayer
        {
            get
            {
                return InitializeVlc();

            }
        }

        public AxWMPLib.AxWindowsMediaPlayer axWmpPlayer
        {
            get
            {
                return InitializeWmp();

            }
        }

        /// <summary>
        /// Initialize the vlc activeX control
        /// </summary>
        public AxAXVLC.AxVLCPlugin2 InitializeVlc()
        {
            this.axVlc = new AxVlcPanel.AxVlcPlayer();
            this.wpfForm.Child = this.axVlc;
            return this.axVlc.axVlc;
        }

        /// <summary>
        /// Initialize the wmp activeX control
        /// </summary>
        public AxWMPLib.AxWindowsMediaPlayer InitializeWmp()
        {
            this.axWmp = new AxWmpPanel.AxWmpPlayer();
            this.wpfForm.Child = this.axWmp;
            return this.axWmp.axWmp;
        }

        private AxVlcPanel.AxVlcPlayer axVlc
        {
            get;
            set;
        }

        private AxWmpPanel.AxWmpPlayer axWmp
        {
            get;
            set;
        }
    }
}
