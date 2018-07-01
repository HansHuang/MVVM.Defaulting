using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MVVM.Defaulting.Extensions
{
    public static class ReflectionExtension
    {
        #region GetPublicProperties
        /// <summary>
        /// Get all properties 
        /// Avoid a issue of BindingFlags.FlattenHierarchy not working for interface
        /// See: http://haacked.com/archive/2009/11/10/interface-inheritance-esoterica.aspx/
        /// http://stackoverflow.com/questions/358835/getproperties-to-return-all-properties-for-an-interface-inheritance-hierarchy
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static IEnumerable<PropertyInfo> GetPublicProperties(this Type type)
        {
            if (!type.IsInterface) return type.GetProperties();

            var allPropList = new List<PropertyInfo>();
            (new[] { type }).Concat(type.GetInterfaces()).SelectMany(i => i.GetProperties()).ToList()
                .ForEach(s =>
                {
                    if (!allPropList.Any(x => x.Name == s.Name && s.PropertyType == x.PropertyType))
                        allPropList.Add(s);
                });

            return allPropList;
        }
        #endregion

        #region GetPropsWithAttribute
        public static Dictionary<PropertyInfo, TAttr> GetAttributedProperties<TModel, TAttr>()
            where TAttr : Attribute
        {
            return GetAttributedProperties<TAttr>(typeof(TModel));
        }

        public static Dictionary<PropertyInfo, TAttr> GetAttributedProperties<TAttr>(this Type type)
            where TAttr : Attribute
        {
            return type.GetPublicProperties()
                .Select(s => new { key = s, value = s.GetCustomAttribute<TAttr>(true) })
                .Where(s => s.value != null)
                .ToDictionary(s => s.key, s => s.value);
        }
        #endregion


    }
}
