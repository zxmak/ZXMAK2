using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ZXMAK2.Engine;
using ZXMAK2.Engine.Entities;


namespace ZXMAK2.Host.WinForms.Views
{
    public partial class FormAddDeviceWizard : Form
    {
        public FormAddDeviceWizard()
        {
            InitializeComponent();
            tabControl.ItemSize = new Size(0, 1);
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            BindCategoryList();
            tabControl_SelectedIndexChanged(this, EventArgs.Empty);
        }

        public BusDeviceBase Device { get; private set; }
        public List<BusDeviceBase> IgnoreList { get; set; }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.SelectedIndex == 0)
            {
                //lblActionHint.Text = "Device Category";
                //lblActionAim.Text = "What category of device do you want to add?";
                //lblActionHint.Text = "Device Type";
                //lblActionAim.Text = "What type of device do you want to add?";
                btnBack.Enabled = false;
                btnNext.Text = "Finish";
            }
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            if (tabControl.SelectedIndex == 0)
            {
                var bdd = lstDevices.SelectedItem as BusDeviceDescriptor;
                if (bdd == null)
                {
                    return;
                }
                try
                {
                    Device = (BusDeviceBase)Activator.CreateInstance(bdd.Type);
                    DialogResult = System.Windows.Forms.DialogResult.OK;
                    Close();
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            //if (tabControl.SelectedIndex == 1)
            //{
            //    tabControl.SelectedIndex--;
            //    btnNext.Text = "Next >";
            //    btnNext.Enabled = true;     // bcz already selected
            //    btnBack.Enabled = false;
            //}
        }

        private void lstCategory_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            //btnNext.Text = "Finish";
            //btnNext.Enabled = e.IsSelected;
            //lblActionHint.Text = "Device Type";
            //lblActionAim.Text = "What type of device do you want to add?";
            BindDeviceList();
        }

        private void lstDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            var bdd = lstDevices.SelectedItem as BusDeviceDescriptor;
            if (bdd == null)
            {
                txtDescription.Text = string.Empty;
                btnNext.Enabled = false;
                return;
            }
            var lines = (bdd.Description ?? string.Empty).Split(
                new string[] { Environment.NewLine, "\r", "\n" },
                StringSplitOptions.None);
            txtDescription.Lines = lines;
            btnNext.Enabled = true;
        }

        private int GetSelectedCategoryIndex()
        {
            foreach (int index in lstCategory.SelectedIndices)
                return index;
            return -1;
        }

        private IEnumerable<Type> GetIgnoreTypes()
        {
            var ignoreTypes = new List<Type>();
            foreach (var bdd in IgnoreList)
            {
                ignoreTypes.Add(bdd.GetType());
            }
            return ignoreTypes;
        }

        private void BindCategoryList()
        {
            lstCategory.Items.Clear();
            lstCategory.SelectedIndices.Clear();
            var list = new List<BusDeviceCategory>();
            foreach (var bdd in DeviceEnumerator.SelectWithout(GetIgnoreTypes()))
            {
                if (!list.Contains(bdd.Category))
                {
                    list.Add(bdd.Category);
                }
            }
            list.Sort();
            foreach (var category in list)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Tag = category;
                lvi.Text = string.Format("{0}", category);
                lvi.ImageIndex = FormMachineSettings.FindImageIndex(category);
                lstCategory.Items.Add(lvi);
            }
            lstCategory.SelectedIndices.Add(0);
        }

        private void BindDeviceList()
        {
            lstDevices.Items.Clear();
            lstDevices_SelectedIndexChanged(lstDevices, EventArgs.Empty);
            var catIndex = GetSelectedCategoryIndex();
            if (catIndex < 0)
            {
                return;
            }
            var category = (BusDeviceCategory)lstCategory.Items[catIndex].Tag;
            var list = new List<BusDeviceDescriptor>();
            list.AddRange(DeviceEnumerator.SelectByCategoryWithout(category, GetIgnoreTypes()));
            list.Sort(DeviceNameComparison);
            foreach (var bdd in list)
            {
                lstDevices.Items.Add(bdd);
            }
            //lstDevices.SelectedIndices.Add(0);
        }

        private void lstDevices_DoubleClick(object sender, EventArgs e)
        {
            if (lstDevices.SelectedItem != null)
                btnNext_Click(lstDevices, EventArgs.Empty);
        }

        private static int DeviceNameComparison(
            BusDeviceDescriptor left,
            BusDeviceDescriptor right)
        {
            if (left == null && right == null)
            {
                return 0;
            }
            if (left != null && right != null)
            {
                return left.Name.CompareTo(right.Name);
            }
            return left == null ? -1 : 1;
        }
    }
}
