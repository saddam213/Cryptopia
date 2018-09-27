using System;

namespace Cryptopia.Entity.Auditing
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
    public sealed class AuditableAttribute : Attribute
    {
        readonly bool _obfuscate;
        readonly bool _logAlways;

        public AuditableAttribute(bool obfuscate, bool logAlways)
        {
            _obfuscate = obfuscate;
            _logAlways = logAlways;
        }

        public bool Obfuscate
        {
            get { return _obfuscate; }
        }

        public bool LogAlways
        {
            get { return _logAlways; }
        }
    }
}
