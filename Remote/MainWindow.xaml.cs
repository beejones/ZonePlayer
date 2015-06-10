using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZonePlayerInterface;

namespace Remote
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Used to fill the listbox with all commands
        /// </summary>
        public ObservableCollection<CommandListItem> CommandList 
        { 
            get; 
            set; 
        }

        public MainWindow()
        {
            InitializeComponent();

            ObservableCollection<CommandListItem> commands = new ObservableCollection<CommandListItem>();
            List<string> commandList = ZonePlayerInterfaceHelpers.GetCommands();
            commandList.Sort();
            foreach (var c in commandList)
            {
                commands.Add(new CommandListItem(c));
            }

            this.CommandList = commands;
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
           string result = await DoCommand((this.commandListBox.SelectedItems[0] as CommandListItem).Name, this.zone.Text, this.item.Text, this.playlist.Text);
           this.resultLabel.Text = result;
        }



        /// <summary>
        /// Execute a command
        /// </summary>
        /// <param name="args">Command line arguments</param>
        static async Task<string> DoCommand(string doCommand, string zone, string item, string playlist )
        {
            // Get command from commandline
            Commands? command = ZonePlayerInterfaceHelpers.GetCommand(doCommand);
            if (command == null)
            {
                string message = string.Format("Command '{0}' not recognized", doCommand);
                ShowMessage(message);
                ShowUsage();
                return (message);
            }
            else
            {
                string message = string.Format("Command '{0}'", command.Value);
                ShowMessage(message);
            }

            // Connect to interface
            Uri endpoint = null;
            if (!Uri.TryCreate(Properties.Settings.Default.ZonePlayerInterface, UriKind.Absolute, out endpoint))
            {
                string message = string.Format("Invalid uri'{0}' in configuration", Properties.Settings.Default.ZonePlayerInterface);
                ShowMessage(message);
                return (message);
            }

            IZonePlayerService client = Connect<IZonePlayerService>.GetClient(endpoint);
            try
            {
                Task<string> task = client.Remote(command.ToString(), zone, string.IsNullOrWhiteSpace(item) ? null : item, string.IsNullOrWhiteSpace(playlist) ? null : playlist);
                string result = await DoTask(task);
                return result;
            }
            catch (Exception e)
            {
                string message = string.Format("Can't connect to ZonePlayerWpf. Make sure the App is running");
                ShowMessage(message);
                return(message);
            }
        }

        /// <summary>
        /// Get result from task
        /// </summary>
        /// <param name="task">The task returned from the remote command</param>
        /// <returns></returns>
        private static async Task<string> DoTask(Task<string> task)
        {
            string r = await task;
            return r;
        }

        /// <summary>
        /// Show message on console
        /// </summary>
        /// <param name="message"></param>
        private static void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }

        /// <summary>
        /// Show help how to use the command line interface
        /// </summary>
        private static void ShowUsage()
        {
            string usage = @"    
               Usage:
               ZonePlayerWpfSimpleCommandLine <command> <zonename> [argument]
                   play Zone1 : Start playing Zone1
                   stop Zone1 : Stop playing Zone1
                   next Zone1 : Play next item in playlist for Zone1
                   volget Zone1 : Get the current volume of Zone1
                   volset Zone1 10 : Set the current volume of Zone1 to 10
                   volup Zone1 : Increase the current volume of Zone1 with one unit
                   voldown Zone1 : Decrease the current volume of Zone1 with one unit
               ";

            ShowMessage(usage);
        }

    }
}
