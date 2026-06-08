using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DVLD_Project.People;
using DVLDBusiness;

namespace DVLD_Project
{
    public partial class ManagePeoplefrm : Form
    {
        private static DataTable _dtAllPeople;

        private static DataTable _dtPeople ;
        public ManagePeoplefrm()
        {
            InitializeComponent();
        }

        private void _RefreshPeopleList()
        {
            _dtAllPeople = Person.GetPeople();
            _dtPeople = _dtAllPeople.DefaultView.ToTable(false, "PersonID", "NationalNo",
                "FirstName", "SecondName", "ThirdName"
                , "LastName", "GendorCaption", "DateOfBirth", "CountryName", "Phone", "Email");

            dataGridView1.DataSource = _dtPeople;
        }
        private void ManagePeoplefrm_Load(object sender, EventArgs e)
        {
            try
            {
                _dtAllPeople = Person.GetPeople();

                

                _dtPeople = _dtAllPeople.DefaultView.ToTable(false,
                    "PersonID",
                    "NationalNo",
                    "FirstName",
                    "SecondName",
                    "ThirdName",
                    "LastName",
                    "GendorCaption",
                    "DateOfBirth",
                    "CountryName",
                    "Phone",
                    "Email");

                dataGridView1.DataSource = _dtPeople;
                cmbFilterBy.SelectedIndex = 0;
                NumberOfRecords.Text = dataGridView1.Rows.Count.ToString();

                if (dataGridView1.Rows.Count > 0)
                {
                    dataGridView1.Columns[0].HeaderText = "Person ID";
                    dataGridView1.Columns[0].Width = 60;
                    dataGridView1.Columns[1].HeaderText = "National No.";
                    dataGridView1.Columns[1].Width = 60;
                    dataGridView1.Columns[2].HeaderText = "First Name";
                    dataGridView1.Columns[2].Width = 90;
                    dataGridView1.Columns[3].HeaderText = "Second Name";
                    dataGridView1.Columns[3].Width = 90;
                    dataGridView1.Columns[4].HeaderText = "Third Name";
                    dataGridView1.Columns[4].Width = 90;
                    dataGridView1.Columns[5].HeaderText = "Last Name";
                    dataGridView1.Columns[5].Width = 90;
                    dataGridView1.Columns[6].HeaderText = "Gender";
                    dataGridView1.Columns[6].Width = 50;
                    dataGridView1.Columns[7].HeaderText = "Date Of Birth";
                    dataGridView1.Columns[7].Width = 100;
                    dataGridView1.Columns[8].HeaderText = "Nationality";
                    dataGridView1.Columns[8].Width = 70;
                    dataGridView1.Columns[9].HeaderText = "Phone";
                    dataGridView1.Columns[9].Width = 80;
                    dataGridView1.Columns[10].HeaderText = "Email";
                    dataGridView1.Columns[10].Width = 100;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }     
        private void dataGridView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var hit = dataGridView1.HitTest(e.X, e.Y);

                if (hit.RowIndex >= 0) // يعني ضغطت على صف
                {
                    dataGridView1.ClearSelection();
                    dataGridView1.Rows[hit.RowIndex].Selected = true;

                    dataGridView1.CurrentCell =
                        dataGridView1.Rows[hit.RowIndex].Cells[0];
                }
                else
                {
                    dataGridView1.ClearSelection();
                }
            }
        }
        private void cmsPeople_Opening(object sender, CancelEventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                e.Cancel = true; // يمنع ظهورها
            }
        }
        private void ShowDetailsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int PersonID;
            if (!int.TryParse(dataGridView1.SelectedRows[0].Cells[0].Value.ToString(), out PersonID))
            {
                MessageBox.Show("Invalid PersonID", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            frmShowPersonInfo frm = new frmShowPersonInfo(PersonID);
            frm.ShowDialog();

        }
        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Feature Dosen't Implemnted Yet");
            Form frm = new frmAddUpdatePerson((int)dataGridView1.SelectedRows[0].Cells[0].Value);
            frm.ShowDialog();
            _RefreshPeopleList();
        }
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (MessageBox.Show(
                "Are You Sure You Want To Delete Person [ " + dataGridView1.CurrentRow.Cells[0].Value + " ]",
                "Confirm Delete", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
            {
                if (Person.DeletePerson((int)dataGridView1.CurrentRow.Cells[0].Value))
                {
                    MessageBox.Show("Person Deleted successfully", "Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _RefreshPeopleList();
                }
                else
                    MessageBox.Show("Person was not deleted because it has data linked to it.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

                
        }
        private void sendEmailToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Feature Dosen't Implemnted Yet");
        }
        private void phoneCallToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Feature Dosen't Implemented Yet");
        }
        private void AddNewPerson_Click(object sender, EventArgs e)
        {
            frmAddUpdatePerson frm = new frmAddUpdatePerson();
            frm.ShowDialog();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            frmAddUpdatePerson frm = new frmAddUpdatePerson();
            frm.ShowDialog();
        }
        private void cmbFilterBy_SelectedIndexChanged(object sender, EventArgs e)
        {

            txtFilter.Visible = (cmbFilterBy.Text != "None");
           
            if(txtFilter.Visible)
            {
                txtFilter.Text = "";
                txtFilter.Focus();
            }
        }
        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            string FilterCoulmn = "";

            switch (cmbFilterBy.Text)
            {
                case "First Name":
                    FilterCoulmn = "FirstName";
                    break;
                case "Second Name":
                    FilterCoulmn = "SecondName";
                    break;
                case "Third Name":
                    FilterCoulmn = "ThirdName";
                    break;
                case "Last Name":
                    FilterCoulmn = "LastName";
                    break;
                case "Person ID":
                    FilterCoulmn = "PersonID";
                    break;
                case "Nationality":
                    FilterCoulmn = "CountryName";
                    break;
                case "Phone":
                    FilterCoulmn = "Phone";
                    break;
                case "Email":
                    FilterCoulmn = "Email";
                    break;
                case "National Number":
                    FilterCoulmn = "NationalNo";
                    break;
                case "Gender":
                    FilterCoulmn = "Gendor";
                    break;
                default:
                    FilterCoulmn = "None";
                    break;
            }

            if (txtFilter.Text.Trim() == "" || FilterCoulmn == "None")
            {
                _dtPeople.DefaultView.RowFilter = "";
                NumberOfRecords.Text = dataGridView1.Rows.Count.ToString();
                return;
            }
            if (FilterCoulmn == "PersonID")
            {
                _dtPeople.DefaultView.RowFilter = string.Format("[{0}] = {1}", FilterCoulmn, txtFilter.Text.Trim());
            }
            else
            {
                _dtPeople.DefaultView.RowFilter = string.Format("[{0}] LIKE '{1}%'", FilterCoulmn, txtFilter.Text.Trim());
            }
            NumberOfRecords.Text = dataGridView1.Rows.Count.ToString();
        }
    }
}