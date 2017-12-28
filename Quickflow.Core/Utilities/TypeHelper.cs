using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Quickflow.Core.Utilities
{
    public static class TypeHelper
    {
        public static bool SetValue(this object obj, string propName, object value)
        {
            Type type = obj.GetType();
            PropertyInfo property = type.GetProperties().FirstOrDefault(x => x.Name.ToLower().Equals(propName.ToLower()));
            if (property == null) return false;

            if (property.PropertyType.Equals(typeof(String)))
            {
                property.SetValue(obj, value?.ToString(), null);
            }
            /*else if (property.PropertyType.IsEnum)
            {
                property.SetValue(obj, int.Parse(value.ToString()), null);
            }
            else if (property.PropertyType.IsGenericType && Nullable.GetUnderlyingType(property.PropertyType) != null && Nullable.GetUnderlyingType(property.PropertyType).IsEnum)
            {
                var enumType = Nullable.GetUnderlyingType(property.PropertyType);
                var enumValue = Enum.ToObject(enumType, value);
                property.SetValue(obj, enumValue, null);
            }
            else if (property.PropertyType.IsGenericType && Nullable.GetUnderlyingType(property.PropertyType) != null && Nullable.GetUnderlyingType(property.PropertyType).Equals(typeof(int)))
            {
                property.SetValue(obj, int.Parse(value.ToString()), null);
            }
            else if (property.PropertyType.IsGenericType && Nullable.GetUnderlyingType(property.PropertyType) != null && Nullable.GetUnderlyingType(property.PropertyType).Equals(typeof(decimal)))
            {
                property.SetValue(obj, decimal.Parse(value.ToString()), null);
            }*/
            else if (property.PropertyType.Equals(typeof(Decimal)))
            {
                property.SetValue(obj, decimal.Parse(value.ToString()), null);
            }
            else
            {
                property.SetValue(obj, value, null);
            }

            return true;
        }

        public static object GetValue(this object obj, string propName)
        {
            Type type = obj.GetType();
            PropertyInfo property = type.GetProperty(propName);
            if (property == null) return null;
            return property.GetValue(obj);
        }

        public static object InvokeMethod(this object obj, string methodName, object[] parameters)
        {
            Type type = obj.GetType();
            MethodInfo method = type.GetMethod(methodName);
            return method.Invoke(obj, parameters);
        }

        public static List<Type> GetClassesWithInterface(Type type, string assemblyName)
        {
            List<Type> types = Assembly.Load(new AssemblyName(assemblyName))
                .GetTypes().ToList();

            types = types.Where(x => !x.GetTypeInfo().IsAbstract && x.GetInterfaces().Contains(type)).ToList();

            return types;
        }

        public static List<Type> GetClassesWithInterface<T>(string assemblyName)
        {
            List<Type> types = Assembly.Load(new AssemblyName(assemblyName))
                .GetTypes().ToList();

            types = types.Where(x => !x.GetTypeInfo().IsAbstract && x.GetInterfaces().Contains(typeof(T))).ToList();

            return types;
        }

        public static List<Type> GetClassesWithInterface<T>(params string[] assemblyNames)
        {
            List<Type> types = new List<Type>();
            assemblyNames.ToList().ForEach(assemblyName => {
                types.AddRange(GetClassesWithInterface<T>(assemblyName));
            });

            return types;
        }

        public static List<Type> GetClassesWithInterface<T>()
        {
            List<Type> types = Assembly.GetEntryAssembly()
                .GetTypes().ToList();

            types = types.Where(x => x.GetInterfaces().Contains(typeof(T))).ToList();

            return types;
        }

        public static List<T> GetInstanceWithInterface<T>(String assemblyName)
        {
            List<Type> types = Assembly.Load(new AssemblyName(assemblyName))
                .GetTypes().Where(x => !x.IsAbstract && !x.FullName.StartsWith("<>f__AnonymousType")).ToList();

            var instances = types.Where(x => x.GetInterfaces().Contains(typeof(T))).Select(x => (T)Activator.CreateInstance(x)).ToList();

            return instances;
        }

        public static List<Object> GetInstanceWithInterface(Type type, String assemblyName)
        {
            List<Type> types = Assembly.Load(new AssemblyName(assemblyName))
                .GetTypes().Where(x => !x.IsAbstract && !x.FullName.StartsWith("<>f__AnonymousType")).ToList();

            var instances = types.Where(x => x.GetInterfaces().Contains(type)).Select(x => Activator.CreateInstance(x)).ToList();

            return instances;
        }

        public static List<T> GetInstanceWithInterface<T>(params string[] assemblyNames)
        {
            List<T> instances = new List<T>();
            assemblyNames.ToList().ForEach(assemblyName => {
                instances.AddRange(GetInstanceWithInterface<T>(assemblyName));
            });

            return instances;
        }

        public static Type GetType(String typeName, params string[] assemblyNames)
        {
            for (int i = 0; i < assemblyNames.Count(); i++)
            {
                List<Type> types = Assembly.Load(new AssemblyName(assemblyNames[i]))
                    .GetTypes().Where(x => !x.IsAbstract && !x.FullName.StartsWith("<>f__AnonymousType")).ToList();

                Type type = types.FirstOrDefault(x => x.Name == typeName);
                if (type != null)
                {
                    return type;
                };
            }

            return null;
        }

        public static Object GetInstance(String typeName, params string[] assemblyNames)
        {
            var type = GetType(typeName, assemblyNames);

            if (type != null)
            {
                return Activator.CreateInstance(type);
            };

            return null;
        }
    }
}
