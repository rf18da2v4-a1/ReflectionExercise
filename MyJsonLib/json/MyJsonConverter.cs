using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Authentication.ExtendedProtection;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace MyJsonLib.json
{
    public static class MyJsonConverter
    {
        /*
         * SERIALIZE
         */
        public static String Serialize<T>(T obj)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("{");

            // Find all properties 
            Type objType = obj.GetType();
            PropertyInfo[] properties = objType.GetProperties();

            foreach (PropertyInfo info in properties)
            {
                sb.Append($"\"{info.Name}\":{MakeValue(obj, info.PropertyType, info.Name)},");
            }
            sb.Length--; // remove last comma
            sb.Append("}");
            return sb.ToString();
        }

        /*
         * Help Serialize
         */
        private static string MakeValue<T>(T obj, Type propType, String propName)
        {

            String propJsonStr;
            
            if (propType.IsPrimitive && propType.Name != "Char")
            {
                propJsonStr = MakeSimpleProperty(obj, propType, propName);
            }
            else if (propType.Name == "String" || propType.Name == "Char")
            {
                propJsonStr = $"\"{MakeSimpleProperty(obj, propType, propName)}\"";
            }
            else if (CheckIfPropIsEnumerable(propType))
            {
                propJsonStr = MakeEnumerableProperty(obj, propType, propName);
            }
            else
            {
                propJsonStr = MakeObjectProperty(obj, propType, propName);
            }

            return propJsonStr;
        }

        private static bool CheckIfPropIsEnumerable(Type pt)
        {
            return pt.IsArray || 
                   pt.GetInterfaces().Any(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        }

        private static string MakeObjectProperty<T>(T obj, Type propType, string propName)
        {
            object objValue = null;
            if (propName != null)
                objValue = obj.GetType().GetProperty(propName)?.GetValue(obj);
            else
                objValue = obj;

            return Serialize(objValue);
        }

        private static string MakeSimpleProperty<T>(T obj, Type propType, string propName)
        {
            object objValue = null;
            if (propName != null)
                objValue = obj.GetType().GetProperty(propName)?.GetValue(obj);
            else
                objValue = obj;

            return objValue.ToString();
        }

        private static string MakeEnumerableProperty<T>(T obj, Type propType, string propName)
        {
            IEnumerable values = null;
            if (propName != null)
                values = obj.GetType().GetProperty(propName)?.GetValue(obj) as IEnumerable;
            else
                values = obj as IEnumerable;

            StringBuilder sb = new StringBuilder();
            foreach (object o in values)
            {
                sb.Append(MakeValue(o, o.GetType(), null));
                sb.Append(",");
            }

            if(sb.Length > 0) sb.Length--;
            return $"[{sb}]";
        }

        /*
         * DESERIALIZE
         */
        public static T Deserialize<T>(String json) where T : new()
        {
            T t = new T();

            return (T)Deserialize(json, t);
        }

        public static object Deserialize(String json, object obj)
        {
            // assume json string is correct - i.e. no error checking

            json = json.Substring(1); // skip '{'

            while (json != "")
            {
                PropertyPair nextProp = MakeNextProp(json);
                json = ReadValue(nextProp, obj);
            }

            return obj;
        }

        /*
         * Help Deserialize
         */
        private static string ReadValue<T>(PropertyPair nextProp, T t) where T : new()
        {
            string json;
            if (nextProp.Value.StartsWith('['))
            {
                ReadPropList(t, nextProp);
            }
            else if (nextProp.Value.StartsWith('{'))
            {
                ReadPropObject(t, nextProp);
            }
            else // read simple type
            {
                ReadPropSimple(t, nextProp);
            }

            return nextProp.RestOfJson;
        }

        private static void ReadPropSimple<T>(T t, PropertyPair nextProp) where T : new()
        {
            PropertyInfo property = t.GetType().GetProperty(nextProp.PropName);

            object value = FindTypeAndValue(property, nextProp.Value);
            property?.SetValue(t, value);

        }

        private static object FindTypeAndValue(PropertyInfo property, string valueStr)
        {
            switch (property.PropertyType.Name)
            {
                case "Int32":
                    return Int32.Parse(valueStr);
                
                case "Int16":
                    return Int16.Parse(valueStr);
                
                case "Byte":
                    return Byte.Parse(valueStr);
                    
                case "Int64":
                    return Int64.Parse(valueStr);
                    
                case "Char":
                    return char.Parse(valueStr.Substring(1,1));
                    
                case "Boolean":
                    return bool.Parse(valueStr);

                case "String":
                    return valueStr;
            }

            return null;
        }

        private static void ReadPropObject<T>(T t, PropertyPair nextProp) where T : new() 
        {
            PropertyInfo property = t.GetType().GetProperty(nextProp.PropName);
            Type propType = property.PropertyType;
            object propObj = Activator.CreateInstance(propType);

            object obj = Deserialize(nextProp.Value, propObj);

            property.SetValue(t, obj);
            
        }


        private static void ReadPropList<T>(T t, PropertyPair nextProp) where T : new()
        {
            //todo Not working
            /*
            PropertyInfo property = t.GetType().GetProperty(nextProp.PropName);
            IEnumerable valueList = new List<object>();

            string json = nextProp.Value.Substring(1);
            while (!(json.StartsWith(']')))
            {
                //todo read values into valueList
                json = json.Substring(1);
            }
            */
            //property?.SetValue(t, valueList);
            
        }

        /*
         * Help Deserialize
         */
        private static PropertyPair MakeNextProp(string json)
        {
            PropertyPair  retPair = new PropertyPair();
            int ix = json.IndexOf(':');
            int jix = GetLastIndexOfValue(json, ix + 1);

            // Property Name
            retPair.PropName = json.Substring(1, ix - 2).Trim();
            
            // Property Value
            retPair.Value = json.Substring(ix+1,jix-ix).Trim();
            
            // The rest of the json string 
            if (jix == json.Length-1) // at the end of the json string
                retPair.RestOfJson = "";
            else
                retPair.RestOfJson = json.Substring(jix+2).Trim();

            return retPair;
        }

        private static int GetLastIndexOfValue(string json, int ix)
        {
            int retIx = ix;

            bool stop = false;
            while (!stop)
            {
                switch (json[retIx])
                {
                    case '[':
                        retIx = GetLastIndexOfComplexValue(json, retIx + 1);
                        break;
                    case '{':
                        retIx = GetLastIndexOfComplexValue(json, retIx + 1);
                        break;
                    case '}': // Reach the end of Json string 
                        if (json.Length == (retIx+1)) 
                            retIx--;
                        stop = true; 
                        break;
                    case ',': // value ended
                        retIx--; // move one back
                        stop = true;
                        break;
                    default:
                        retIx++;
                        break;
                }
            }

            return retIx;

        }

        private static int GetLastIndexOfComplexValue(string json, int ix)
        {
            int retIx = ix;

            bool stop = false;
            while (!stop)
            {
                switch (json[retIx])
                {
                    case '[':
                        retIx = GetLastIndexOfComplexValue(json, retIx + 1);
                        break;
                    case '{':
                        retIx = GetLastIndexOfComplexValue(json, retIx + 1);
                        break;
                    case '}':
                        stop = true;
                        break;
                    case ']':
                        stop = true;
                        break;
                    default:
                        retIx++;
                        break;
                }
            }

            return retIx;

        }
    }

    /// <summary>
    /// Helper class for deserializing
    /// </summary>
    internal class PropertyPair
    {
        /// <summary>
        /// The name of the property
        /// </summary>
        public String PropName { get; set; }
        /// <summary>
        /// The value as a string of the property
        /// </summary>
        public String Value { get; set; }

        /// <summary>
        /// The rest of the json string
        /// </summary>
        public String RestOfJson { get; set; }

        /// <summary>
        /// Indicate if property is simple or complex, true if simple or string type
        /// </summary>
        public bool IsSimple
        {
            get => !(Value.StartsWith('{') || Value.StartsWith('['));
        }
    }

}
