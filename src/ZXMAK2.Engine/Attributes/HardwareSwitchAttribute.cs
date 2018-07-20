using System;


namespace ZXMAK2.Engine.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class HardwareSwitchAttribute : Attribute
    {
        public string Name { get; private set; }
        public string Description { get; set; }


        public HardwareSwitchAttribute(string name)
        {
            Name = name;
        }
    }
}
