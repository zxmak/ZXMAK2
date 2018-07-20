using System;


namespace ZXMAK2.Engine.Attributes
{
	[AttributeUsage(AttributeTargets.Property)]
	public class HardwareValueAttribute : Attribute
	{
        public string Name { get; private set; }
        public string Description { get; set; }


		public HardwareValueAttribute(string name)
		{
			Name = name;
		}
	}
}
