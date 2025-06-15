using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ApiService.Core.DataHelper
{
    public static class EnumHelper
    {

        public class NameValue
        {
            public string Name { get; set; }
            public object Value { get; set; }
        }

        public static List<NameValue> EnumToList<T>(bool check = false)
        {
            List<NameValue> lst = new List<NameValue>();
            var arrayName = Enum.GetNames(typeof(T)).ToArray<string>();
            foreach (string name in arrayName)
            {
                var item = Enum.Parse(typeof(T), name);
                if ((int)item > 0)
                {
                    if (check && ((Enum)item).GetNotAddToListalue())
                    {
                        continue;
                    }
                    string nameReturn = ((Enum)item).GetDescription();
                    string valueReturn = ((Enum)item).GetStringValue();
                    if (!string.IsNullOrEmpty(valueReturn))
                    {
                        lst.Add(new NameValue { Name = nameReturn, Value = valueReturn });
                    }
                    else
                    {
                        lst.Add(new NameValue { Name = nameReturn, Value = (int)item });

                    }
                }
            }
            return lst;
        }

        public static string GetDescription(this Enum en)
        {
            Type type = en.GetType();

            MemberInfo[] memInfo = type.GetMember(en.ToString());

            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

                if (attrs != null && attrs.Length > 0)
                {
                    return ((DescriptionAttribute)attrs[0]).Description;
                }
            }

            return en.ToString();
        }

        public static string GetStringValue(this Enum value)
        {
            string output = null;
            Type type = value.GetType();

            FieldInfo fi = type.GetField(value.ToString());
            StringValue[] attrs = fi.GetCustomAttributes(typeof(StringValue), false) as StringValue[];
            if (attrs.Length > 0)
            {
                output = attrs[0].Value;
            }

            return output;
        }

        public static bool GetNotAddToListalue(this Enum value)
        {
            bool output = false;
            Type type = value.GetType();

            FieldInfo fi = type.GetField(value.ToString());
            NotAddToList[] attrs = fi.GetCustomAttributes(typeof(NotAddToList), false) as NotAddToList[];
            if (attrs.Length > 0)
            {
                output = attrs[0].Value;
            }

            return output;
        }
    }

    public class StringValue : System.Attribute
    {
        private string _value;

        public StringValue(string value)
        {
            _value = value;
        }

        public string Value
        {
            get { return _value; }
        }
    }

    public class NotAddToList : System.Attribute
    {
        private bool _value;

        public NotAddToList(bool value)
        {
            _value = value;
        }

        public bool Value
        {
            get { return _value; }
        }
    }
}
