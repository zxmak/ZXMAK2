using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections;

namespace ZXMAK2.Hardware.Adlers.Views.CustomControls
{
    public partial class ListViewCustom : ListView
    {
        private int _sortColumnAct = -1;

        public ListViewCustom()
        {
            InitializeComponent();

            this.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView_ColumnClick);
        }

        private void listView_ColumnClick(object sender,
                                   System.Windows.Forms.ColumnClickEventArgs e)
        {
            // Determine whether the column is the same as the last column clicked.
            if (e.Column != _sortColumnAct)
            {
                // Set the sort column to the new column.
                _sortColumnAct = e.Column;
                // Set the sort order to ascending by default.
                this.Sorting = SortOrder.Ascending;
            }
            else
            {
                // Determine what the last sort order was and change it.
                if (this.Sorting == SortOrder.Ascending)
                    this.Sorting = SortOrder.Descending;
                else
                    this.Sorting = SortOrder.Ascending;
            }

            // Call the sort method to manually sort.
            this.ListViewItemSorter = new ListViewItemComparer(e.Column, this.Sorting);
            this.Sort();
        }
    }

    class ListViewItemComparer : IComparer
    {
        private int col;
        private SortOrder order;
        public ListViewItemComparer()
        {
            col = 0;
            order = SortOrder.Ascending;
        }
        public ListViewItemComparer(int column, SortOrder order)
        {
            col = column;
            this.order = order;
        }
        public int Compare(object x, object y) 
        {
            //!!! designed for ListView<string, ushort> only for now.... !!!
            //Link: https://msdn.microsoft.com/en-us/library/ms996467.aspx
            int returnVal= -1;
            var column1 = ((ListViewItem)x);
            var column2 = ((ListViewItem)y);

            //get value to compare
            if (col == 1) // second column is ushort
            {
                //set values
                ushort value1 = ConvertRadix.ConvertNumberWithPrefix(column1.Tag.ToString());
                ushort value2 = ConvertRadix.ConvertNumberWithPrefix(column2.Tag.ToString());
                if (value1 == value2)
                    returnVal = 0;
                else
                    returnVal = value1 > value2 ? 1 : -1;
            }
            else
                returnVal = String.Compare(((ListViewItem)x).Text, ((ListViewItem)y).Text);

            // Determine whether the sort order is descending.
            if (order == SortOrder.Descending)
                // Invert the value returned by String.Compare.
                returnVal *= -1;
            return returnVal;
        }
    }
}
