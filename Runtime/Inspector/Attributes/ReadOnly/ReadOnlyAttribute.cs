using System;

namespace VahTyah.Inspector
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ReadOnlyAttribute : Attribute
    {
    }
}
