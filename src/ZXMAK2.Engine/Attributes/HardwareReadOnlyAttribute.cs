using System;


namespace ZXMAK2.Engine.Attributes
{
	[AttributeUsage(AttributeTargets.Property)]
    public class HardwareReadOnlyAttribute : Attribute
    {
        public bool IsReadOnly { get; private set; }

        public HardwareReadOnlyAttribute(bool isReadOnly)
        {
            IsReadOnly = isReadOnly;
        }
    }
}
