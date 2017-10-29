namespace Malware.MDKModules
{
    /// <summary>
    /// An interface to Visual Studio's output pane for MDK
    /// </summary>
    public interface IOutputPane
    {
        /// <summary>
        /// Writes a line of text onto the output pane
        /// </summary>
        /// <param name="content"></param>
        void WriteLine(object content = null);
    }
}