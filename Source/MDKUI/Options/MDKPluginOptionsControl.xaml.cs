using System;
using System.Windows.Controls;
using Malware.MDKModules;

namespace Malware.MDKUI.Options
{
    /// <summary>
    /// Interaction logic for MDKOptionsWindow.xaml
    /// </summary>
    public partial class MDKPluginOptionsControl : UserControl
    {
        /// <summary>
        /// Creates a new instance of <see cref="MDKPluginOptionsControl"/>
        /// </summary>
        public MDKPluginOptionsControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The data model for this control
        /// </summary>
        public MDKPluginOptionsControlModel Model
        {
            get => (MDKPluginOptionsControlModel)Host.DataContext;
            set
            {
                Detach(Model);
                Host.DataContext = value;
                Attach(Model);
            }
        }

        void Attach(MDKPluginOptionsControlModel model)
        {
            if (model == null)
                return;
            model.UriRequested += OnModelUriRequested;
        }

        void Detach(MDKPluginOptionsControlModel model)
        {
            if (model == null)
                return;
            model.UriRequested -= OnModelUriRequested;
        }

        void OnModelUriRequested(object sender, UriRequestedEventArgs e)
        {
            var model = new AddPluginLocationDialogModel(Model.HelpPageUrl);
            if (AddPluginLocationDialog.ShowDialog(model) == true)
            {
                Model.PluginLocations.Add(model.Uri);
            }
        }
    }
}
