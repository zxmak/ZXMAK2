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
using System.Diagnostics;
using ZXMAK2.Host.Entities;
using ZXMAK2.Host.Interfaces;

namespace ZXMAK2.Host.Tools
{
    [DebuggerDisplay("Key={Key}")]
    public sealed class KeyBinding : IKeyCondition
    {
        private readonly IEnumerable<IKeyCondition> _conditions;

        public KeyBinding(SpeccyKey key, IEnumerable<IKeyCondition> conditions)
        {
            Key = key;
            _conditions = conditions;
        }

        public SpeccyKey Key { get; private set; }

        public bool Match(IKeyboardState state)
        {
            return _conditions.Any(arg => arg.Match(state));
        }

        public static KeyBinding Deserialize(XmlNode node)
        {
            var key = XmlHelper.GetAttribute(node, "Key", null);
            if (key == null)
            {
                return null;
            }
            SpeccyKey keyValue;
            if (!Enum.TryParse<SpeccyKey>(key, true, out keyValue))
            {
                Logger.Warn("KeyBinding: Skipped unknown Key value: {0}", key);
                return null;
            }
            var conditions = new List<IKeyCondition>();
            var keySrc = XmlHelper.GetAttribute(node, "Condition", null);
            if (keySrc != null)
            {
                Key keySrcValue;
                if (!Enum.TryParse<Key>(keySrc, true, out keySrcValue))
                {
                    Logger.Warn("KeyBinding: Skipped unknown Condition value: {0}", keySrc);
                    return null;
                }
                conditions.Add(new KeyCondition(keySrcValue));
            }
            else
            {
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
            }
            return new KeyBinding(keyValue, conditions.ToArray());
        }
    }
}
