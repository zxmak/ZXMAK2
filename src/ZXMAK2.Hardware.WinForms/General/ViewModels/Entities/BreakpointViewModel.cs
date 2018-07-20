using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZXMAK2.Engine.Entities;

namespace ZXMAK2.Hardware.WinForms.General.ViewModels.Entities
{
    public class BreakpointViewModel
    {
        public BreakpointViewModel(Breakpoint entity)
        {
            Entity = entity;
        }

        public Breakpoint Entity { get; private set; }

        public override string ToString()
        {
            return Entity.Label;
        }
    }
}
