using System.Windows.Controls;
using Malware.MDKModules;

namespace Malware.MDKUI.Options
{
    /// <summary>
    /// Interaction logic for MDKOptionsWindow.xaml
    /// </summary>
    public partial class MDKOptionsControl : UserControl
    {
        /// <summary>
        /// Creates a new instance of <see cref="MDKOptionsControl"/>
        /// </summary>
        public MDKOptionsControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// The data model for this control
        /// </summary>
        public MDKOptionsControlModel Model
        {
            get => (MDKOptionsControlModel)Host.DataContext;
            set => Host.DataContext = value;
        }
    }
}
