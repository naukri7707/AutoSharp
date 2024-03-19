using System;
using System.Reflection;

namespace Naukri.InspectorMaid.Editor.Extensions
{
    public static class MemberInfoExtensions
    {
        public static bool HasAttribute<T>(this MemberInfo self) where T : Attribute
        {
            return Attribute.IsDefined(self, typeof(T));
        }

        public static bool IsStatic(this MemberInfo member)
        {
            if (member is FieldInfo fieldInfo)
            {
                return fieldInfo.IsStatic;
            }
            else if (member is PropertyInfo propertyInfo)
            {
                return IsStatic(propertyInfo.GetAccessors(true)[0]);
            }
            else if (member is MethodBase methodBase)
            {
                return methodBase.IsStatic;
            }
            else
            {
                throw new InvalidOperationException("Unsupported type");
            }
        }
    }
}
