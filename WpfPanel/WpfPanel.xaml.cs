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
        /// <summary>
        ///  Initializes a new instance of the <see cref="PanelControl"/> class.
        /// </summary>
        public PanelControl()
        {
            InitializeComponent();
            this.HostPanel = new System.Windows.Forms.Panel();
            this.wpfForm.Child = this.HostPanel;
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref="PanelControl"/> class.
        /// </summary>
        public PanelControl(AxWMPLib.AxWindowsMediaPlayer axWmpPlayer)
            : this()
        {
            this.HostPanel.Controls.Clear();
            this.HostPanel.Controls.Add(axWmpPlayer);
        }

#if VlcIsInstalled
        /// <summary>
        ///  Initializes a new instance of the <see cref="PanelControl"/> class.
        /// </summary>
        public PanelControl(AxAXVLC.AxVLCPlugin2 axVlcPlayer)
            : this()
        {
            if (axVlcPlayer != null)
            {
                this.HostPanel.Controls.Clear();
                this.HostPanel.Controls.Add(axVlcPlayer);
            }
        }
#endif

        public System.Windows.Forms.Panel HostPanel
        {
            get;
            private set;
        }

#if VlcIsInstalled
        private AxAXVLC.AxVLCPlugin2 axVlcPlayer
        {
            get
            {
                return InitializeVlc();
            }
        }
#endif

        private AxWMPLib.AxWindowsMediaPlayer axWmpPlayer
        {
            get
            {
                return InitializeWmp();
            }
        }

#if VlcIsInstalled

        /// <summary>
        /// Initialize the vlc activeX control
        /// </summary>
        /// <returns>Reference to the activex control</returns>
        public AxAXVLC.AxVLCPlugin2 InitializeVlc()
        {
            this.axVlc = new AxVlcPanel.AxVlcPlayer();
            this.HostPanel.Controls.Clear();
            this.HostPanel.Controls.Add(this.axVlc.axVlc);
            return this.axVlc.axVlc;
        }
#endif

        /// <summary>
        /// Initialize the wmp activeX control
        /// </summary>
        /// <returns>Reference to the activex control</returns>
        public AxWMPLib.AxWindowsMediaPlayer InitializeWmp()
        {
            this.axWmp = new AxWmpPanel.AxWmpPlayer();
            this.HostPanel.Controls.Clear();
            this.HostPanel.Controls.Add(this.axWmp.axWmp);
            return this.axWmp.axWmp;
        }

#if VlcIsInstalled
        /// <summary>
        /// Gets or sets the Vlc wrapper
        /// </summary>
        public AxVlcPanel.AxVlcPlayer axVlc
        {
            get;
            set;
        }
#endif

        /// <summary>
        /// Gets or sets the Wmp wrapper
        /// </summary>
        public AxWmpPanel.AxWmpPlayer axWmp
        {
            get;
            set;
        }
    }
}
