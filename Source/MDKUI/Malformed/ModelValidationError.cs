namespace Malware.MDKUI.Malformed
{
    public struct ModelValidationError
    {
        public readonly string PropertyName;
        public readonly string Error;

        public ModelValidationError(string propertyName, string error)
        {
            PropertyName = propertyName;
            Error = error;
        }
    }
}