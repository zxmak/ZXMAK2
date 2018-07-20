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
    [DebuggerDisplay("Key={_key}, IsPressed={_isPressed}")]
    public sealed class KeyCondition : IKeyCondition
    {
        private readonly Key _key;
        private readonly bool _isPressed;

        public KeyCondition(Key key)
            : this (key, true)
        {
        }
        
        public KeyCondition(Key key, bool isPressed)
        {
            _key = key;
            _isPressed = isPressed;
        }

        public bool Match(IKeyboardState state)
        {
            return state[_key] == _isPressed;
        }

        public static KeyCondition Deserialize(XmlNode node)
        {
            var key = XmlHelper.GetAttribute(node, "Key", null);
            var isPressed = XmlHelper.GetAttribute(node, "IsPressed", null);
            if (key == null)
            {
                return null;
            }
            Key keyValue;
            if (!Enum.TryParse<Key>(key, true, out keyValue))
            {
                Logger.Warn("KeyCondition: Skipped unknown Key value: {0}", key);
                return null;
            }
            if (isPressed == null)
            {
                return new KeyCondition(keyValue);
            }
            bool isPressedValue;
            if (!bool.TryParse(isPressed, out isPressedValue))
            {
                Logger.Warn("KeyCondition: Ignored unknown IsPressed value: {0}", isPressed);
                return new KeyCondition(keyValue);
            }
            return new KeyCondition(keyValue, isPressedValue);
        }
    }
}
