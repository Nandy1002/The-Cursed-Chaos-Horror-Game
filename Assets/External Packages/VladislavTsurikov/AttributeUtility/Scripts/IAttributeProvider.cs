using System;

namespace VladislavTsurikov.AttributeUtility.Scripts
{
    public interface IAttributeProvider
    {
        Attribute[] GetCustomAttributes(bool inherit);
    }
}