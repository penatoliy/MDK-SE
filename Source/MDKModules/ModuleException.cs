using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Malware.MDKModules
{
    [Serializable]
    public class ModuleException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public ModuleException()
        { }

        public ModuleException(string message) : base(message)
        { }

        public ModuleException(string message, Exception inner) : base(message, inner)
        { }

        protected ModuleException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        { }
    }
}
