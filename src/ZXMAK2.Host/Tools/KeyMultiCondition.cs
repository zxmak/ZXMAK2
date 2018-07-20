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
using ZXMAK2.Host.Interfaces;


namespace ZXMAK2.Host.Tools
{
    public sealed class KeyMultiCondition : IKeyCondition
    {
        private readonly IEnumerable<IKeyCondition> _conditions;

        public KeyMultiCondition(IEnumerable<IKeyCondition> conditions)
        {
            _conditions = conditions;
        }
        
        public bool Match(IKeyboardState state)
        {
            return _conditions.All(arg=>arg.Match(state));
        }

        public static KeyMultiCondition Deserialize(XmlNode node)
        {
            var conditions = new List<IKeyCondition>();
            foreach (XmlNode subNode in XmlHelper.SelectNodes(node, "Condition"))
            {
                var condition = KeyCondition.Deserialize(subNode);
                if (condition != null)
                {
                    conditions.Add(condition);
                }
            }
            foreach (XmlNode subNode in XmlHelper.SelectNodes(node, "MultiCondition"))
            {
                var condition = KeyMultiCondition.Deserialize(subNode);
                if (condition != null)
                {
                    conditions.Add(condition);
                }
            }
            return new KeyMultiCondition(conditions.ToArray());
        }
    }
}
