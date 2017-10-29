using System;
using System.Text;
using Malware.MDKModules;
using Microsoft.VisualStudio.Shell.Interop;

namespace MDK
{
    class OutputPane: IOutputPane
    {
        public static readonly Guid Id = new Guid("8A4E0FEC-BFC4-4057-9061-592DBB383AA1");
        readonly MDKPackage _package;
        IVsOutputWindowPane _pane;
        StringBuilder _buffer = new StringBuilder();

        public OutputPane(MDKPackage package)
        {
            _package = package;
        }

        public void Initialize()
        {
            var output = (IVsOutputWindow)_package.ServiceProvider.GetService(typeof(SVsOutputWindow));
            var paneGuid = Id;
            output.CreatePane(ref paneGuid, "MDK", Convert.ToInt32(true), Convert.ToInt32(true));
            output.GetPane(ref paneGuid, out _pane);

            WriteLine($"MDK {_package.Options.Version}");
            WriteLine();
            if (_buffer.Length > 0)
                _pane.OutputStringThreadSafe(_buffer.ToString());
            _buffer = null;
        }

        public void WriteLine(object content = null)
        {
            var text = $"{content}{Environment.NewLine}";
            if (_buffer != null)
                _buffer.Append(text);
            else
                _pane.OutputStringThreadSafe(text);
        }
    }
}
