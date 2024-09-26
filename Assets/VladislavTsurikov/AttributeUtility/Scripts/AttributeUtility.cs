using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace VladislavTsurikov.AttributeUtility.Scripts
{
    public static class AttributeUtility
    {
        private static readonly Dictionary<object, AttributeCache> optimizedCaches = new Dictionary<object, AttributeCache>();

        private static AttributeCache GetAttributeCache(MemberInfo element)
        {
            // For MemberInfo (and therefore Type), we use the MetadataToken
            // as a key instead of the object itself, because member infos
            // are not singletons but their tokens are, optimizing the cache.
            var key = element;

            lock (optimizedCaches)
            {
                if (!optimizedCaches.TryGetValue(key, out var cache))
                {
                    cache = new AttributeCache(element);
                    optimizedCaches.Add(key, cache);
                }

                return cache;
            }
        }

        private static AttributeCache GetAttributeCache(ParameterInfo element)
        {
            // For ParameterInfo, we maybe also should use the MetadataToken,
            // but I'm not sure they're globally unique or just locally unique. TODO: Check
            var key = element;

            lock (optimizedCaches)
            {
                if (!optimizedCaches.TryGetValue(key, out var cache))
                {
                    cache = new AttributeCache(element);
                    optimizedCaches.Add(key, cache);
                }

                return cache;
            }
        }

        private static AttributeCache GetAttributeCache(IAttributeProvider element)
        {
            var key = element;

            lock (optimizedCaches)
            {
                if (!optimizedCaches.TryGetValue(key, out var cache))
                {
                    cache = new AttributeCache(element);
                    optimizedCaches.Add(key, cache);
                }

                return cache;
            }
        }

        #region Members (& Types)

        public static void CacheAttributes(MemberInfo element)
        {
            GetAttributeCache(element);
        }

        /// <summary>
        /// Gets attributes on an enum member, eg. enum E { [Attr] A }
        /// </summary>
        internal static IEnumerable<T> GetAttributeOfEnumMember<T>(this Enum enumVal) where T : Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString());
            var attributes = memInfo[0].GetCustomAttributes(typeof(T), false);
            return attributes.Cast<T>();
        }

        public static bool HasAttribute(this MemberInfo element, Type attributeType, bool inherit = true)
        {
            return GetAttributeCache(element).HasAttribute(attributeType, inherit);
        }

        public static Attribute GetAttribute(this MemberInfo element, Type attributeType, bool inherit = true)
        {
            return GetAttributeCache(element).GetAttribute(attributeType, inherit);
        }

        public static IEnumerable<Attribute> GetAttributes(this MemberInfo element, Type attributeType, bool inherit = true)
        {
            return GetAttributeCache(element).GetAttributes(attributeType, inherit);
        }

        public static bool HasAttribute<TAttribute>(this MemberInfo element, bool inherit = true)
            where TAttribute : Attribute
        {
            return GetAttributeCache(element).HasAttribute<TAttribute>(inherit);
        }

        public static TAttribute GetAttribute<TAttribute>(this MemberInfo element, bool inherit = true)
            where TAttribute : Attribute
        {
            return GetAttributeCache(element).GetAttribute<TAttribute>(inherit);
        }

        public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this MemberInfo element, bool inherit = true)
            where TAttribute : Attribute
        {
            return GetAttributeCache(element).GetAttributes<TAttribute>(inherit);
        }

        #endregion

        #region Parameters

        public static void CacheAttributes(ParameterInfo element)
        {
            GetAttributeCache(element);
        }

        public static bool HasAttribute(this ParameterInfo element, Type attributeType, bool inherit = true)
        {
            return GetAttributeCache(element).HasAttribute(attributeType, inherit);
        }

        public static Attribute GetAttribute(this ParameterInfo element, Type attributeType, bool inherit = true)
        {
            return GetAttributeCache(element).GetAttribute(attributeType, inherit);
        }

        public static IEnumerable<Attribute> GetAttributes(this ParameterInfo element, Type attributeType, bool inherit = true)
        {
            return GetAttributeCache(element).GetAttributes(attributeType, inherit);
        }

        public static bool HasAttribute<TAttribute>(this ParameterInfo element, bool inherit = true)
            where TAttribute : Attribute
        {
            return GetAttributeCache(element).HasAttribute<TAttribute>(inherit);
        }

        public static TAttribute GetAttribute<TAttribute>(this ParameterInfo element, bool inherit = true)
            where TAttribute : Attribute
        {
            return GetAttributeCache(element).GetAttribute<TAttribute>(inherit);
        }

        public static IEnumerable<TAttribute> GetAttributes<TAttribute>(this ParameterInfo element, bool inherit = true)
            where TAttribute : Attribute
        {
            return GetAttributeCache(element).GetAttributes<TAttribute>(inherit);
        }

        #endregion
    }
}