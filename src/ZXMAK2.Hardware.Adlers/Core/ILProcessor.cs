using System;
using System.Reflection;
using System.Reflection.Emit;
using ZXMAK2.Engine.Cpu;
using ZXMAK2.Engine.Interfaces;

namespace ZXMAK2.Hardware.Adlers.Core
{
    public static class ILProcessor
    {
        public static Func<bool> EmitCondition(ref IDebuggable i_spectrum, BreakpointInfo i_breakpointInfo)
        {
            Func<bool> o_ILOut = null;

            if ( i_breakpointInfo.AccessType == BreakPointConditionType.registryVsValue )
            {
                //e.g. PC == #0038
                Type[] args = { typeof(CpuRegs) };
                DynamicMethod dynamicMethod = new DynamicMethod(
                    "RegVsValue",
                    typeof(bool), //return type
                    args,         //arguments for the method
                    typeof(CpuRegs).Module); //module as input

                ILGenerator il = dynamicMethod.GetILGenerator();

                //1.Arg0 - registry
                il.Emit(OpCodes.Ldarg_0); // load m_spectrum.CPU.regs on stack
                FieldInfo testInfo1 = typeof(CpuRegs).GetField(i_breakpointInfo.LeftCondition, BindingFlags.Public | BindingFlags.Instance);
                il.Emit(OpCodes.Ldfld, testInfo1);

                //2.Arg1 - number
                il.Emit(OpCodes.Ldc_I4, (int)i_breakpointInfo.RightValue);

                //3.Compare
                EmitComparison(il, i_breakpointInfo.ConditionTypeSign);
                il.Emit(OpCodes.Ret); //Return: 1 => true(breakpoint is reached) otherwise 0 => false

                o_ILOut = (Func<bool>)dynamicMethod.CreateDelegate(typeof(Func<bool>), i_spectrum.CPU.regs);
            }
            else if( i_breakpointInfo.AccessType == BreakPointConditionType.flagVsValue )
            {
                //e.g.: fZ == 1
                Type[] args = { typeof(CpuRegs) };
                DynamicMethod dynamicMethod = new DynamicMethod(
                    "FlagVsValue",
                    typeof(bool), //return type
                    args,         //arguments for the method
                    typeof(CpuRegs).Module); //module as input

                ILGenerator il = dynamicMethod.GetILGenerator();

                //1.Arg0 - flag
                    il.Emit(OpCodes.Ldarg_0); // load m_spectrum.CPU.regs on stack
                    FieldInfo testInfo1 = typeof(CpuRegs).GetField("AF", BindingFlags.Public | BindingFlags.Instance);
                    il.Emit(OpCodes.Ldfld, testInfo1);
               
                    //get flag value(0 or 1)
                    switch( i_breakpointInfo.LeftCondition )
                    {
                        case "FS":
                            il.Emit(OpCodes.Ldc_I4, 0x80);
                            break;
                        case "FZ":
                            il.Emit(OpCodes.Ldc_I4, 0x40);
                            break;
                        case "FH":
                            il.Emit(OpCodes.Ldc_I4, 0x10);
                            break;
                        case "FPV":
                            il.Emit(OpCodes.Ldc_I4, 0x04);
                            break;
                        case "FN":
                            il.Emit(OpCodes.Ldc_I4, 0x02);
                            break;
                        case "FC":
                            il.Emit(OpCodes.Ldc_I4, 0x01);
                            break;
                        default:
                            throw new CommandParseException("Incorrect flag in condition emit...");
                    }
                    il.Emit(OpCodes.And); //get flag value from F registry

                if (i_breakpointInfo.ConditionTypeSign == "==" && i_breakpointInfo.RightValue > 0)
                {
                    i_breakpointInfo.RightValue = 0;
                    i_breakpointInfo.ConditionTypeSign = "!=";
                }

                //2. Arg1 - right condition(must a number)
                il.Emit(OpCodes.Ldc_I4, (int)i_breakpointInfo.RightValue);
                
                //3. Compare
                EmitComparison(il, i_breakpointInfo.ConditionTypeSign);
                il.Emit(OpCodes.Ret); //Return: 1 => true(breakpoint is reached) otherwise 0 => false

                o_ILOut = (Func<bool>)dynamicMethod.CreateDelegate(typeof(Func<bool>), i_spectrum.CPU.regs);
            }
            else if (i_breakpointInfo.AccessType == BreakPointConditionType.memoryVsValue)
            {
                //e.g. (16384) == #FF00
                //ToDo: Because it is not possible to dynamically emit code for interface method(IDebuggable.ReadMemory)
                //      I temporary wrapped it into custom wrapper.
                InterfaceWrapper middleMan = new InterfaceWrapper();
                middleMan.wrapInterface(i_spectrum);

                MethodInfo ReadMemoryMethod;
                if (i_breakpointInfo.RightValue > 0xFF)
                    ReadMemoryMethod = typeof(InterfaceWrapper).GetMethod("invokeReadMemory16Bit",
                                                                           BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                                                                         , null
                                                                         , new Type[] { typeof(ushort) }
                                                                         , null);
                else
                    ReadMemoryMethod = typeof(InterfaceWrapper).GetMethod("invokeReadMemory8Bit",
                                                                           BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                                                                         , null
                                                                         , new Type[] { typeof(ushort) }
                                                                         , null);

                DynamicMethod dynamicMethod = new DynamicMethod("ReadMemory"
                                                               , typeof(bool)
                                                               , new Type[] { typeof(InterfaceWrapper) }
                                                               , typeof(InterfaceWrapper).Module
                                                               );

                ILGenerator IL = dynamicMethod.GetILGenerator();

                //Arg0 - memory reference(static), e.g. (16384)
                IL.Emit(OpCodes.Ldarg_0); // load InterfaceWrapper on stack
                IL.Emit(OpCodes.Ldc_I4, i_breakpointInfo.LeftValue); // method parameter(for ReadMemoryMethod)
                IL.Emit(OpCodes.Call, ReadMemoryMethod);

                //Arg1
                IL.Emit(OpCodes.Ldc_I4, i_breakpointInfo.RightValue); // <- compare to 8 or 16bit

                EmitComparison(IL, i_breakpointInfo.ConditionTypeSign);
                IL.Emit(OpCodes.Ret); //Return: 1 => true(breakpoint is reached) otherwise 0 => false

                o_ILOut = (Func<bool>)dynamicMethod.CreateDelegate(typeof(Func<bool>), middleMan);
            }
            else if (i_breakpointInfo.AccessType == BreakPointConditionType.registryMemoryReferenceVsValue)
            {
                // e.g.: (PC) == #D155 - instruction breakpoint
                //ToDo: Because it is not possible to dynamically emit code for interface method(IDebuggable.ReadMemory)
                //      I temporary wrapped it into custom wrapper.
                InterfaceWrapper middleMan = new InterfaceWrapper();
                middleMan.wrapInterface(i_spectrum);
                middleMan.wrapFields(i_spectrum.CPU.regs);

                MethodInfo ReadMemoryMethod;
                //Type[] args = { typeof(REGS) };
                if (i_breakpointInfo.RightValue > 0xFF)
                    ReadMemoryMethod = typeof(InterfaceWrapper).GetMethod("invokeReadMemory16BitViaRegistryValue",
                                                                           BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                                                                         , null
                                                                         , new Type[] { typeof(string) }
                                                                         , null);
                else
                    ReadMemoryMethod = typeof(InterfaceWrapper).GetMethod("invokeReadMemory8BitViaRegistryValue",
                                                                           BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
                                                                         , null
                                                                         , new Type[] { typeof(string) }
                                                                         , null);

                DynamicMethod dynamicMethod = new DynamicMethod("ReadMemoryViaRegistry", typeof(bool), new Type[] { typeof(InterfaceWrapper) });

                ILGenerator IL = dynamicMethod.GetILGenerator();

                //Arg0, e.g. (PC)
                IL.Emit(OpCodes.Ldarg_0); // load InterfaceWrapper on stack

                string registry = DebuggerManager.getRegistryFromReference(i_breakpointInfo.LeftCondition);
                IL.Emit(OpCodes.Ldstr, registry);
                IL.Emit(OpCodes.Call, ReadMemoryMethod);

                //Arg1, number(right condition)
                IL.Emit(OpCodes.Ldc_I4, i_breakpointInfo.RightValue); // <- compare to 8 or 16bit

                EmitComparison(IL, i_breakpointInfo.ConditionTypeSign);
                IL.Emit(OpCodes.Ret); //Return: 1 => true(breakpoint is reached) otherwise 0 => false

                o_ILOut = (Func<bool>)dynamicMethod.CreateDelegate(typeof(Func<bool>), middleMan);
            }

            return o_ILOut;
        }

        public static void EmitComparison(ILGenerator ilGenerator, string condition)
        {
            switch (condition)
            {
                case "==":
                    ilGenerator.Emit(OpCodes.Ceq);
                    break;
                case "!=":
                    ilGenerator.Emit(OpCodes.Ceq);
                    ilGenerator.Emit(OpCodes.Ldc_I4, 0);
                    ilGenerator.Emit(OpCodes.Ceq);
                    break;
                case ">":
                    ilGenerator.Emit(OpCodes.Cgt);
                    break;
                case "<":
                    ilGenerator.Emit(OpCodes.Clt);
                    break;
                default:
                    throw new CommandParseException(
                        string.Format("Unknown condition: {0}", condition));
            }
        }

        public/*must be public!*/ class InterfaceWrapper
        {
            //fields wrapper
            private CpuRegs a_Z80Registers;

            public void wrapFields(CpuRegs i_regs)
            {
                a_Z80Registers = i_regs;
            }

            public ushort getRegistryValue(string i_registryName)
            {
                //FieldInfo testInfo1 = typeof(REGS).GetField(i_registryName, BindingFlags.Public | BindingFlags.Instance);
                return 0;
            }

            //method wrapper
            delegate TReturn delegateWithReturnAndParameterType<TReturn, TParameter0>(TParameter0 p0);

            private delegateWithReturnAndParameterType<byte, ushort> readMemory8BitDelegate;
            private delegateWithReturnAndParameterType<ushort, ushort> readMemory16BitDelegate;

            public void wrapInterface(IDebuggable i_debuggable)
            {
                readMemory8BitDelegate = delegate(ushort memAdress) { return i_debuggable.ReadMemory(memAdress); };
                readMemory16BitDelegate = delegate(ushort memAdress)
                {
                    return (ushort)(i_debuggable.ReadMemory(memAdress) | i_debuggable.ReadMemory(++memAdress) << 8 );
                };
            }

            public byte invokeReadMemory8Bit(ushort memAdress)
            {
                return readMemory8BitDelegate(memAdress);
            }
            public ushort invokeReadMemory16Bit(ushort memAdress)
            {
                return readMemory16BitDelegate(memAdress);
            }

            public byte invokeReadMemory8BitViaRegistryValue(string registryName)
            {
                return readMemory8BitDelegate(DebuggerManager.getRegistryValueByName(a_Z80Registers, registryName));
            }
            public ushort invokeReadMemory16BitViaRegistryValue(string registryName)
            {
                return readMemory16BitDelegate(DebuggerManager.getRegistryValueByName(a_Z80Registers, registryName));
            }
        }
    }
}
