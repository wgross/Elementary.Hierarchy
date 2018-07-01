//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using Elementary.Hierarchy.Generic;
//namespace Elementary.Hierarchy.Reflection
//{
//    public static class ReflectedHierarchyExtensions
//    {
//        public static IEnumerable<(object,PropertyInfo)> TraverseProperties<T>(this T instance)
//        {
//            return instance.GetType().GetProperties().Select(pi => ChildrenOfPropertyInfo(((object)instance, pi)));
//            //return ((object)instance, (PropertyInfo)null).Children(ChildrenOfPropertyInfo);
//        }

//        private static IEnumerable<(object, PropertyInfo)> ChildrenOfPropertyInfo((object instance, PropertyInfo property) propertyInstance)
//        {
//            var instance = propertyInstance.property.GetValue(propertyInstance.instance);
//            return instance.GetType()
//                .GetProperties()
//                .Select(pi => (instance,pi));
//        }
//    }
//}