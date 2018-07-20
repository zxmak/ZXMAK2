using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Globalization;
using ZXMAK2.Engine.Attributes;
using ZXMAK2.Engine.Entities;


namespace ZXMAK2.Host.WinForms.Tools
{
    public class BusDeviceTypeConverter : TypeConverter
    {
        public override PropertyDescriptorCollection GetProperties(
            ITypeDescriptorContext context, 
            object value, 
            Attribute[] attributes)
        {
            var propList = new List<PropertyDescriptor>();
            var deviceObj = ((BusDeviceProxy)value).Device;
            var deviceType = deviceObj.GetType();
            
            foreach (var pi in deviceType.GetProperties())
            {
                var list = new List<Attribute>();
                var display = false;
                foreach (Attribute at in pi.GetCustomAttributes(true))
                {
                    //list.Add(at);
                    var hwValue = at as HardwareValueAttribute;
                    var hwSwitch = at as HardwareSwitchAttribute;
                    var ro = at as HardwareReadOnlyAttribute;
                    if (hwValue != null)
                    {
                        display |= true;
                        list.Add(new DescriptionAttribute(hwValue.Description));
                    }
                    if (hwSwitch != null)
                    {
                        display |= true;
                        list.Add(new DescriptionAttribute(hwSwitch.Description));
                    }
                    if (ro != null)
                    {
                        list.Add(new ReadOnlyAttribute(ro.IsReadOnly));
                    }
                }
                if (!display)
                {
                    continue;
                }
                var pd = new InternalPropertyDescriptor(
                    deviceType,
                    pi.Name,
                    pi.PropertyType,
                    list.ToArray());
                propList.Add(pd);
            }
            return new PropertyDescriptorCollection(propList.ToArray());
        }

        public override bool GetPropertiesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        protected class InternalPropertyDescriptor : SimplePropertyDescriptor
        {
            public InternalPropertyDescriptor(
                Type type, 
                string name, 
                Type propType, 
                Attribute[] attrs) 
                : base(type, name, propType, attrs) 
            { 
            }

            public override object GetValue(object component)
            {
                var obj = ((BusDeviceProxy)component).Device;
                return ComponentType
                    .GetProperty(Name)
                    .GetValue(obj, null);
            }

            public override void SetValue(object component, object value)
            {
                var obj = ((BusDeviceProxy)component).Device;
                var propInfo = ComponentType.GetProperty(Name);
                propInfo.SetValue(obj, Convert.ChangeType(value, propInfo.PropertyType), null);
            }
        }
    }

    [TypeConverter(typeof(BusDeviceTypeConverter))]
    public class BusDeviceProxy
    {
        // workaround to display in hex format
        static BusDeviceProxy()
        {
            var originalIntProvider = TypeDescriptor.GetProvider(typeof(int));
            var hexIntProvider = new HexTypeDescriptionProvider(originalIntProvider);
            TypeDescriptor.AddProvider(hexIntProvider, typeof(int));
            var originalByteProvider = TypeDescriptor.GetProvider(typeof(byte));
            var hexByteProvider = new HexTypeDescriptionProvider(originalByteProvider);
            TypeDescriptor.AddProvider(hexByteProvider, typeof(byte));
        }

        
        public BusDeviceBase Device { get; private set; }

        public BusDeviceProxy(BusDeviceBase device)
        {
            Device = device;
        }
    }

    public class HexTypeDescriptionProvider : TypeDescriptionProvider
    {
        private HexCustomTypeDescriptor _descriptor = new HexCustomTypeDescriptor();

        public HexTypeDescriptionProvider(TypeDescriptionProvider parent) 
            : base(parent) 
        { 
        }

        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            if (objectType == typeof(int) || objectType == typeof(byte))
            {
                return _descriptor;
            }
            else
            {
                return base.GetTypeDescriptor(objectType, instance);
            }
        }
    }

    public class HexCustomTypeDescriptor : CustomTypeDescriptor
    {
        private HexTypeConverter _converter = new HexTypeConverter();
        
        public override TypeConverter GetConverter()
        {
            return _converter;
        }
    }

    public class HexTypeConverter : TypeConverter
    {
        public override bool CanConvertFrom(
            ITypeDescriptorContext context, 
            Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            else
            {
                return base.CanConvertFrom(context, sourceType);
            }
        }

        public override bool CanConvertTo(
            ITypeDescriptorContext context, 
            Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }
            else
            {
                return base.CanConvertTo(context, destinationType);
            }
        }

        public override object ConvertTo(
            ITypeDescriptorContext context, 
            CultureInfo culture, 
            object value, 
            Type destinationType)
        {
            if (destinationType == typeof(string) && 
                value.GetType() == typeof(int))
            {
                return string.Format("#{0:X}", value);
            }
            else if (destinationType == typeof(string) &&
                value.GetType() == typeof(byte))
            {
                return string.Format("#{0:X2}", value);
            }
            else
            {
                return base.ConvertTo(context, culture, value, destinationType);
            }
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value.GetType() == typeof(string))
            {
                string input = (string)value;

                if (input.StartsWith("#", StringComparison.OrdinalIgnoreCase))
                {
                    input = input.Substring(1);
                }
                if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                {
                    input = input.Substring(2);
                }
                return int.Parse(input, NumberStyles.HexNumber, culture);
            }
            else
            {
                return base.ConvertFrom(context, culture, value);
            }
        }
    }
}
