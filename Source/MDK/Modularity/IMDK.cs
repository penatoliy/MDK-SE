namespace MDK.Modularity
{
    public interface IMDK
    {
        string ExpandMacros(ProjectInfo projectInfo, string source);
    }
}