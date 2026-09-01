using Business_Logic;
using DVLD.People;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace DVLD
{
    public partial class frmPeople : Form
    {
        private DataView _DataView;
        public frmPeople()
        {
            InitializeComponent();
        }

        private void _RefreshPeople()
        {
            DataTable dt = clsPerson.GetAllPeople();
            dt.Columns.Add("GendorName", typeof(string), "IIF(Gendor = 0, 'Male', 'Female')");

            _DataView = dt.DefaultView;
            dgvPeople.DataSource = _DataView;
            dgvPeople.Columns["Gendor"].Visible = false;
            PrepareColumnsView();

            lblRecordsNumber.Text = dgvPeople.RowCount.ToString();
        }

        private void PrepareColumnsView()
        {
            if(dgvPeople.Rows.Count > 0)
            {
                dgvPeople.Columns["PersonID"].HeaderText = "Person ID";
                // dgvPeople.Columns["PersonID"].Width = 110;

                dgvPeople.Columns["NationalNo"].HeaderText = "National No.";
                // dgvPeople.Columns["NationalNo"].Width = 120;

                dgvPeople.Columns["FirstName"].HeaderText = "First Name";
                // dgvPeople.Columns["FirstName"].Width = 120;

                dgvPeople.Columns["SecondName"].HeaderText = "Second Name";
                // dgvPeople.Columns["SecondName"].Width = 140;

                dgvPeople.Columns["ThirdName"].HeaderText = "Third Name";
                // dgvPeople.Columns["ThirdName"].Width = 120;

                dgvPeople.Columns["LastName"].HeaderText = "Last Name";
                // dgvPeople.Columns["LastName"].Width = 120;

                dgvPeople.Columns["DateOfBirth"].HeaderText = "Date Of Birth";
                // dgvPeople.Columns["DateOfBirth"].Width = 140;

                dgvPeople.Columns["NationalCountryID"].HeaderText = "Nationality";
                // dgvPeople.Columns["NationalCountryID"].Width = 120;

                dgvPeople.Columns["Phone"].HeaderText = "Phone";
                // dgvPeople.Columns["Phone"].Width = 120;

                dgvPeople.Columns["GendorName"].HeaderText = "Gendor Caption";
                // dgvPeople.Columns["GendorName"].Width = 120;

                dgvPeople.Columns["Email"].HeaderText = "Email";
                // dgvPeople.Columns["Email"].Width = 170;

                dgvPeople.Columns["Address"].HeaderText = "Address";
                // dgvPeople.Columns["Address"].Width = 140;
            }
        }

        private void People_Load(object sender, EventArgs e)
        {
            cbFiltersType.SelectedIndex = 0;
            tbFilterValue.Visible = false;
            _RefreshPeople();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void showDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmPersonDetails frm = new frmPersonDetails((int)dgvPeople.CurrentRow.Cells["PersonID"].Value);
            frm.ShowDialog();
        }

        private string MapFilterValue()
        {
            switch (cbFiltersType.Text)
            {
                case "Person ID":
                    return "PersonID";
                case "National No.":
                    return "NationalNo";
                case "First Name":
                    return "FirstName";
                case "Second Name":
                    return "SecondName";
                case "Third Name":
                    return "ThirdName";
                case "Last Name":
                    return "LastName";
                case "Nationality":
                    return "NationalCountryID";
                case "Gendor":
                    return "GendorName";
                case "Phone":
                    return "Phone";
                case "Email":
                    return "Email";
                default:
                    return "None";
            }
        }

        private void tbFilterValue_TextChanged(object sender, EventArgs e)
        {
            string FilterColumn = MapFilterValue();

            if (string.IsNullOrWhiteSpace(tbFilterValue.Text) || FilterColumn == "None")
            {
                _DataView.RowFilter = null;
                lblRecordsNumber.Text = dgvPeople.RowCount.ToString();
                return;
            }

            if (FilterColumn == "PersonID" || FilterColumn == "NationalCountryID")
            {
                _DataView.RowFilter = $"[{FilterColumn}] = {tbFilterValue.Text}";
            }
            else
            {
                string Filter = tbFilterValue.Text.Trim().Replace("'", "''");
                _DataView.RowFilter = $"[{FilterColumn}] LIKE '{Filter}%'";
            }

            lblRecordsNumber.Text = dgvPeople.RowCount.ToString();
        }

        private void cbFiltersType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbFiltersType.Text == "None")
                tbFilterValue.Visible = false;
            else
            {
                tbFilterValue.Visible = true;
                tbFilterValue.Focus();
            }

            tbFilterValue.Text = "";
        }

        private void tbFilterValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (cbFiltersType.Text == "Person ID" || cbFiltersType.Text == "Nationality" || cbFiltersType.Text == "Phone")
            {
                if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                {
                    e.Handled = true;
                    errpNumericFieldsValidation.SetError((TextBox)sender, "Invalid Insertion, Please Insert A Number");
                }
                else
                    errpNumericFieldsValidation.SetError((TextBox)sender, "");
            }
        }

        void DataUpdated()
        {
            _RefreshPeople();
        }

        private void btnAddPerson_Click(object sender, EventArgs e)
        {
            frmAddEditPerson frm = new frmAddEditPerson();
            frm.DataUpdatedEvent += DataUpdated;
            frm.ShowDialog();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show($"Are you sure you want to delete Person [ {dgvPeople.CurrentRow.Cells["PersonID"].Value} ]",
                "Confirm Delete", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                if (clsPerson.DeletePerson(int.Parse(dgvPeople.CurrentRow.Cells["PersonID"].Value.ToString())))
                {
                    MessageBox.Show("Person Deleted Successfully", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _RefreshPeople();
                }
                else
                    MessageBox.Show("Contact is not deleted.");
            }
        }

        private void addNewPersonToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAddEditPerson frm = new frmAddEditPerson();
            frm.DataUpdatedEvent += DataUpdated;
            frm.ShowDialog();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmAddEditPerson frm = new frmAddEditPerson((int)dgvPeople.CurrentRow.Cells["PersonID"].Value);
            frm.DataUpdatedEvent += DataUpdated;
            frm.ShowDialog();
        }

        void _MessageFeatureNotImplemented()
        {
            MessageBox.Show($"Sorry This Feature Not Implemented Yet");
        }

        private void sendEmailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _MessageFeatureNotImplemented();
        }

        private void phoneCallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _MessageFeatureNotImplemented();
        }

        private void dgvPeople_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
            {
                dgvPeople.CurrentCell = dgvPeople.Rows[e.RowIndex].Cells[e.ColumnIndex >= 0 ? e.ColumnIndex : 0];
                dgvPeople.ClearSelection();
                dgvPeople.Rows[e.RowIndex].Selected = true;
            }
        }
    }
}
