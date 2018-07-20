/* 
 *  Copyright 2015 Alex Makeev
 * 
 *  This file is part of ZXMAK2 (ZX Spectrum virtual machine).
 *
 *  ZXMAK2 is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  ZXMAK2 is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with ZXMAK2.  If not, see <http://www.gnu.org/licenses/>.
 *
 */
using System;
using System.Linq;
using System.Xml;
using System.Collections.Generic;
using ZXMAK2.Host.Entities;
using ZXMAK2.Host.Interfaces;

namespace ZXMAK2.Host.Tools
{
    public sealed class KeyboardMatrix
    {
        private readonly IEnumerable<KeyBinding> _keyBindings;
        private readonly Func<IKeyboardState, bool>[][] _rowConditions;

        public KeyboardMatrix(
            SpeccyKey[][] rows,
            IEnumerable<KeyBinding> keyBindings)
        {
            _keyBindings = keyBindings;
            _rowConditions = rows
                .Select(row => row.Select(GetCondition).ToArray())
                .ToArray();
        }

        public int[] Scan(IKeyboardState state)
        {
            //return _rowConditions
            //    .Select(arg => MakeRowValue(arg, state))
            //    .ToArray();
            // Optimized version:
            var result = new int[_rowConditions.Length];
            if (state == null)
            {
                return result;
            }
            for (var j = 0; j < result.Length; j++)
            {
                var conditions = _rowConditions[j];
                var row = 0;
                var mask = 1;
                for (var i = 0; i < conditions.Length; i++, mask <<= 1)
                {
                    if (conditions[i](state))
                    {
                        row |= mask;
                    }
                }
                result[j] = row;
            }
            return result;
        }


        #region Private

        //private int MakeRowValue(
        //    Func<IKeyboardState, bool>[] conditions,
        //    IKeyboardState state)
        //{
        //    return conditions
        //        .Select((arg, index) => arg(state) ? 1 << index : 0)
        //        .Aggregate((seed, item) => seed | item);
        //}

        private Func<IKeyboardState, bool> GetCondition(SpeccyKey key)
        {
            var binding = _keyBindings
                .FirstOrDefault(arg => arg.Key == key);
            if (binding == null)
            {
                return arg => false;
            }
            return arg => binding.Match(arg);
        }

        #endregion Private


        #region Static

        /// <summary>
        /// Scan rows according to spectrum port address
        /// </summary>
        public static int ScanPort(int[] rows, int port)
        {
            if (rows == null)
            {
                return 0;
            }
            //var addrMask = port >> 8;
            //var result = _rows
            //    .Where((arg, index) => (addrMask & (1 << index)) == 0)
            //    .Aggregate((seed, arg) => seed | arg);
            // Optimized version:
            var result = 0;
            var mask = 0x100;
            for (var i = 0; i < rows.Length; i++, mask <<= 1)
            {
                if ((port & mask) == 0)
                {
                    result |= rows[i];
                }
            }
            return result;
        }

        public static KeyboardMatrix Deserialize(
            SpeccyKey[][] rows,
            string fileName)
        {
            var xml = new XmlDocument();
            xml.Load(fileName);
            return Deserialize(rows, xml.DocumentElement);
        }

        public static KeyboardMatrix Deserialize(
            SpeccyKey[][] rows,
            XmlNode node)
        {
            var bindings = new List<KeyBinding>();
            foreach (XmlNode subNode in XmlHelper.SelectNodes(node, "KeyBinding"))
            {
                var binding = KeyBinding.Deserialize(subNode);
                if (binding != null)
                {
                    bindings.Add(binding);
                }
            }
            return new KeyboardMatrix(rows, bindings.ToArray());
        }

        public static readonly SpeccyKey[][] DefaultRows = new SpeccyKey[8][]
        {
            new SpeccyKey[5] { SpeccyKey.CapsShift, SpeccyKey.Z, SpeccyKey.X, SpeccyKey.C, SpeccyKey.V, },
            new SpeccyKey[5] { SpeccyKey.A, SpeccyKey.S, SpeccyKey.D, SpeccyKey.F, SpeccyKey.G, },
            new SpeccyKey[5] { SpeccyKey.Q, SpeccyKey.W, SpeccyKey.E, SpeccyKey.R, SpeccyKey.T, },
            new SpeccyKey[5] { SpeccyKey.D1, SpeccyKey.D2, SpeccyKey.D3, SpeccyKey.D4, SpeccyKey.D5, },
            new SpeccyKey[5] { SpeccyKey.D0, SpeccyKey.D9, SpeccyKey.D8, SpeccyKey.D7, SpeccyKey.D6, },
            new SpeccyKey[5] { SpeccyKey.P, SpeccyKey.O, SpeccyKey.I, SpeccyKey.U, SpeccyKey.Y, },
            new SpeccyKey[5] { SpeccyKey.Enter, SpeccyKey.L, SpeccyKey.K, SpeccyKey.J, SpeccyKey.H, },
            new SpeccyKey[5] { SpeccyKey.Space, SpeccyKey.SymbolShift, SpeccyKey.M, SpeccyKey.N, SpeccyKey.B, },
        };

        #endregion Static
    }
}
