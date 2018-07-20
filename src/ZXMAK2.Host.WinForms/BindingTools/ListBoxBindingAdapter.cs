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
 *  Date:        12-Jule-2015
 *  
 *  Description: standard WinForms binding has issues with SelectedIndex
 *               on DataSource.ListChanged event, so we are using custom
 *               logic to avoid it.
 *  Note:        SelectedValue, SelectedIndex - not implemented
 *               DataSource - virtual property
 */
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using ZXMAK2.Mvvm.BindingTools;


namespace ZXMAK2.Host.WinForms.BindingTools
{
    public class ListBoxBindingAdapter : BaseBindingAdapter
    {
        private object _dataSource;
        private object _selectedItem;
        private int _isVirtualUpdate;
        
        public ListBoxBindingAdapter(ListBox control)
            : base(control)
        {
            control.SelectedIndexChanged += ListBox_OnSelectedIndexChanged;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!disposing)
                return;
            var bindingList = _dataSource as IBindingList;
            if (bindingList != null)
            {
                bindingList.ListChanged -= DataSource_OnListChanged;
            }
            var listBox = (ListBox)Target;
            if (listBox != null)
            {
                listBox.SelectedIndexChanged -= ListBox_OnSelectedIndexChanged;
            }
        }

        public override void SetTargetPropertyValue(string name, object value)
        {
            if (name == "SelectedIndex" || name == "SelectedValue")
            {
                throw new NotSupportedException();
            }
            if (name == "DataSource")  // virtual property
            {
                var listBox = (ListBox)Target;

                if (_dataSource == value)
                {
                    return;
                }
                _isVirtualUpdate++;
                var bindingList = _dataSource as IBindingList;
                if (bindingList != null)
                {
                    bindingList.ListChanged -= DataSource_OnListChanged;
                }
                _dataSource = value;
                bindingList = _dataSource as IBindingList;
                if (bindingList != null)
                {
                    bindingList.ListChanged += DataSource_OnListChanged;
                }
                var collection = _dataSource as IEnumerable<object>;
                listBox.BeginUpdate();
                listBox.ClearSelected();
                listBox.Items.Clear();
                if (collection != null)
                {
                    foreach (var item in collection)
                    {
                        listBox.Items.Add(item);
                    }
                }
                listBox.ClearSelected();
                listBox.EndUpdate();
                if (_selectedItem != null)
                {
                    _selectedItem = null;
                    OnPropertyChanged("SelectedItem", BindingTrigger.PropertyChanged);
                }
                _isVirtualUpdate--;
                return;
            }
            if (name == "SelectedItem") // protected property
            {
                var listBox = (ListBox)Target;

                if (_selectedItem == value)
                {
                    return;
                }
                _selectedItem = value;
                if (_selectedItem == null)
                {
                    listBox.ClearSelected();
                }
                else
                {
                    listBox.SelectedItem = _selectedItem;
                }

                return;
            }
            base.SetTargetPropertyValue(name, value);
        }

        public override object GetTargetPropertyValue(string name)
        {
            if (name == "DataSource")   // virtual property
            {
                return _dataSource;
            }
            if (name == "SelectedItem") // protected property
            {
                return _selectedItem;
            }
            return base.GetTargetPropertyValue(name);
        }

        private void ListBox_OnSelectedIndexChanged(object sender, EventArgs e)
        {
            var listBox = (ListBox)Target;
            if (_isVirtualUpdate == 0 && _selectedItem != listBox.SelectedItem)
            {
                _selectedItem = listBox.SelectedItem;
                OnPropertyChanged("SelectedItem", BindingTrigger.PropertyChanged);
            }
        }

        private void DataSource_OnListChanged(object sender, ListChangedEventArgs e)
        {
            var listBox = (ListBox)Target;
            var dataSource = (IEnumerable<object>)_dataSource;
            var isSelectionChanged = false;

            // synchronize
            _isVirtualUpdate++;
            listBox.BeginUpdate();
            var newItems = dataSource ?? new object[0];
            if (listBox.SelectedItem != null &&
                !newItems.Any(arg => arg == listBox.SelectedItem))
            {
                listBox.ClearSelected();
                isSelectionChanged = true;
            }
            switch (e.ListChangedType)
            {
                case ListChangedType.ItemAdded:
                    listBox.Items.Insert(e.NewIndex, newItems.ElementAt(e.NewIndex));
                    break;
                case ListChangedType.ItemChanged:
                    listBox.Items.RemoveAt(e.NewIndex);
                    listBox.Items.Insert(e.NewIndex, newItems.ElementAt(e.NewIndex));
                    break;
                case ListChangedType.ItemDeleted:
                    listBox.Items.RemoveAt(e.NewIndex);
                    break;
                case ListChangedType.ItemMoved:
                    var item = listBox.Items[e.OldIndex];
                    listBox.Items.RemoveAt(e.OldIndex);
                    listBox.Items.Insert(e.NewIndex, item);
                    break;
                case ListChangedType.Reset:
                    listBox.Items.Clear();
                    newItems
                        .ToList()
                        .ForEach(arg => listBox.Items.Add(arg));
                    break;
            }
            if (!isSelectionChanged && listBox.SelectedItem != _selectedItem)
            {
                if (_selectedItem == null)
                {
                    listBox.ClearSelected();
                }
                else
                {
                    listBox.SelectedItem = _selectedItem;
                }
            }
            listBox.EndUpdate();
            _isVirtualUpdate--;

            if (isSelectionChanged && _selectedItem != listBox.SelectedItem)
            {
                _selectedItem = listBox.SelectedItem;
                OnPropertyChanged("SelectedItem", BindingTrigger.PropertyChanged);
            }
        }
    }
}
