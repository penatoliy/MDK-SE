namespace Malware.MDKServices.Import
{
    public interface IScriptService
    {
        string WrapScript(string script);
        string UnwrapScript(string script);
    }
}