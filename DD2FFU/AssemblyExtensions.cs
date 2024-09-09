using System;
using System.Linq;
using System.Reflection;

namespace DD2FFU
{
    internal static class AssemblyExtensions
    {
        public static T GetAssemblyAttribute<T>(this Assembly ass) where T : Attribute
        {
            object[] attributes = ass.GetCustomAttributes(typeof(T), false);
            return attributes == null || attributes.Length == 0 ? null : attributes.OfType<T>().SingleOrDefault();
        }
    }
}