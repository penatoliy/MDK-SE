using System.Windows.Controls;
using Malware.MDKModules;

namespace Malware.MDKUI.Options
{
    /// <summary>
    /// Interaction logic for MDKOptionsWindow.xaml
    /// </summary>
    public partial class MDKGeneralOptionsControl : UserControl
    {
        /// <summary>
        /// Creates a new instance of <see cref="MDKGeneralOptionsControl"/>
        /// </summary>
        public MDKGeneralOptionsControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The data model for this control
        /// </summary>
        public MDKGeneralOptionsControlModel Model
        {
            get => (MDKGeneralOptionsControlModel)Host.DataContext;
            set => Host.DataContext = value;
        }
    }
}
