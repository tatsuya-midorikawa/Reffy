using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Reffy;

namespace Net35Console
{
    class SampleAttribute : Attribute
    {

    }

    class Foo
    {
        public string MyString { get; }
    }

    enum Sample
    {
        None,
        Some,
    }

    class Program
    {
        static void Main(string[] args)
        {
            // ==================================================
            // Type extensions
            //

            // 1. GetBackingField(Type, string [,BindingFlags] [,bool])
            {
                var backingfield = typeof(Foo).GetBackingField("MyString");
                // do something
            }

            // 2. GetBackingFields(Type [,BindingFlags] [,bool])
            {
                var backingfields = typeof(Foo).GetBackingFields();
                foreach (var backingfield in backingfields)
                {
                    // do something
                }
            }

            // 3. MakeDefault(Type)
            {
                var instance = typeof(Foo).MakeDefault();
                // do something
            }



            // ==================================================
            // MemberInfo extensions
            //

            // 1. GetBackingField(PropertyInfo [, bool])
            {
                foreach (var property in typeof(Foo).GetProperties())
                {
                    var backingfield = property.GetBackingField();
                    // do something
                }
            }

            // 2. GetAttribute<T>(MemberInfo)
            {
                foreach (var property in typeof(Foo).GetProperties())
                {
                    if (property.GetAttribute<SampleAttribute>() is SampleAttribute attribute)
                    {
                        // do something
                    }
                }
                foreach (var field in typeof(Foo).GetFields())
                {
                    if (field.GetAttribute<SampleAttribute>() is SampleAttribute attribute)
                    {
                        // do something
                    }
                }
            }

            // 3. TryGetAttribute<T>(MemberInfo, out T)
            {
                foreach (var property in typeof(Foo).GetProperties())
                {
                    if (property.TryGetAttribute(out SampleAttribute attribute))
                    {
                        // do something
                    }
                }
            }



            // ==================================================
            // Enum extensions
            //

            // GetEnumValue(Type, int) or etc
            {
                var value = typeof(Sample).GetEnumValue(10);
                // do something
            }
        }
    }
}
