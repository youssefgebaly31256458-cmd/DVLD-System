using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DVLDBusiness;
using DVLD_Project.Properties;
using System.IO;
using DVLD_Project.Global_Classes;
using DVLD.Classes;


namespace DVLD_Project
{
    public partial class frmAddUpdatePerson : Form
    {
        public frmAddUpdatePerson()
        {
            InitializeComponent();
        }
        
       
       public enum enMode { AddNew= 0,Update= 1 };
       public enum enGendor { Male = 0,Female = 1 };
        private int _PersonID = -1;
        Person _Person;
        private enGendor Gendor;
        private enMode _Mode;

        public frmAddUpdatePerson(int PersonID)
        {
            InitializeComponent();
            _PersonID = PersonID;
            _Mode = enMode.Update;
        }

        public delegate void DataBackEventHandler(object sender, int PersonID);

        public event DataBackEventHandler DataBack;
        private void _FillCountriesInComboBox()
        {
            DataTable dtAllCOuntries = Countries.GetAllCountries();
            foreach (DataRow row in dtAllCOuntries.Rows)
            {
                cmCountry.Items.Add(row["CountryName"]);
            }
        }       
        private void _ResetDafaultValues()
        {
            _FillCountriesInComboBox();

            if(_Mode ==enMode.AddNew)
            {
                lblTitle.Text = "Add New Person";
                _Person = new Person();
            }
            else
            {
                lblTitle.Text = "Update Person";
            }

            cmCountry.SelectedIndex = cmCountry.FindString("Egypt");
            

            txtbxFirstName.Text = "";
            txtbxSecondName.Text = "";
            txtbxThirdName.Text = "";
            txtbxLastName.Text = "";
            txtbxNationalNumber.Text = "";
            txtbxAddress.Text = "";
            txtbxPhoneNumber.Text = "";

            

            if (rbMale.Checked)
            {
                picbxMainPicture.Image = Resources.Male_512;
            }
            else
            {
                picbxMainPicture.Image = Resources.Female_512;
            }

            lnklblRemoveImage.Visible = (picbxMainPicture.ImageLocation != null);

            dtpDateOfBirth.MaxDate = DateTime.Now.AddYears(-18);
            dtpDateOfBirth.Value = dtpDateOfBirth.MaxDate;

            dtpDateOfBirth.MinDate = DateTime.Now.AddYears(-100);


        }        
        private bool _HandlePersonImage()
        {
            if(_Person.ImagePath != picbxMainPicture.ImageLocation)
            {
                if(_Person.ImagePath!="")
                {
                    try
                    {
                        File.Delete(_Person.ImagePath); 
                    }
                    catch(IOException iox)
                    {

                    }
                }

                if(picbxMainPicture.ImageLocation!=null)
                {
                    string SourceImageFile = picbxMainPicture.ImageLocation.ToString();
                    if(Util.CopyImageToProgectImagesFolder(ref SourceImageFile))
                    {
                        picbxMainPicture.ImageLocation = SourceImageFile;
                        return true;
                    }
                    else
                    {
                        MessageBox.Show("Error Copying Image File", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return false;
                    }
                }
            }
            return true;
        }
        private void _LoadDate()
        {
            _Person = Person.Find(_PersonID);
            if (_Person == null)
            {
                MessageBox.Show("No Person ID With ID :" + _PersonID, "Person Not Found", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.Close();
                return;
            }

            llblPersonID.Text = _PersonID.ToString();
            txtbxFirstName.Text = _Person.FirstName;
            txtbxSecondName.Text = _Person.SecondName;
            txtbxThirdName.Text = _Person.ThirdName;
            txtbxLastName.Text = _Person.LastName;
            txtbxNationalNumber.Text = _Person.NationalNumber;
            dtpDateOfBirth.Value = _Person.DateOfBirth;
            cmCountry.SelectedIndex = cmCountry.FindString(_Person.CountryInfo.CountryName);

            if (_Person.Gendor == 0)
                rbMale.Checked = true;
            else
                rbFemale.Checked = true;

            txtbxAddress.Text = _Person.Address;
            txtbxPhoneNumber.Text = _Person.Phone;
            txtbxEmail.Text = _Person.Email;

                if(_Person.ImagePath !="")
                {
                picbxMainPicture.ImageLocation = _Person.ImagePath;
                }


            lnklblRemoveImage.Visible = (_Person.ImagePath != "");

           
                
        }
        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void frmAddUpdatePerson_Load(object sender, EventArgs e)
        {
            _ResetDafaultValues();
            if(_Mode==enMode.Update)
            {
                _LoadDate();
            }
        }
        private void rbMale_Click(object sender, EventArgs e)
        {
            if (picbxMainPicture.ImageLocation == null)
            {
                picbxMainPicture.Image = Resources.Male_512;
            }
        }
        private void rbFemale_Click(object sender, EventArgs e)
        {
            if (picbxMainPicture.ImageLocation == null)
            {
                picbxMainPicture.Image = Resources.Female_512;
            }
        }
        private void btnSave_Click(object sender, EventArgs e)
        {
            if(!this.ValidateChildren())
            {
                MessageBox.Show("Some Fields Are't Valied,Put the Mouse Over The Red Icon To See The Error", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if(! _HandlePersonImage())
            {
                return;
            }

            var country = Countries.Find(cmCountry.Text.Trim());
            if (country == null)
            {
                MessageBox.Show("Country not found: " + cmCountry.Text, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            int NationalityCountryID = country.ID;

            _Person.FirstName = txtbxFirstName.Text.Trim();
            _Person.SecondName = txtbxSecondName.Text.Trim();
            _Person.ThirdName = txtbxThirdName.Text.Trim();
            _Person.LastName = txtbxLastName.Text.Trim();
            _Person.NationalNumber = txtbxNationalNumber.Text.Trim();
            _Person.Address = txtbxAddress.Text.Trim();
            _Person.Phone = txtbxPhoneNumber.Text.Trim();
            _Person.Email = txtbxEmail.Text.Trim();
            _Person.DateOfBirth = dtpDateOfBirth.Value;

            if (rbMale.Checked)
            {
                _Person.Gendor = (short)enGendor.Male;
            }
            else
                _Person.Gendor = (short)enGendor.Female;

            _Person.NationalityCountryID = NationalityCountryID;

            if (picbxMainPicture.ImageLocation != null)
            {
                _Person.ImagePath = picbxMainPicture.ImageLocation;
            }
            else
                _Person.ImagePath = "";

            
            if (_Person.Save())
            {
                llblPersonID.Text = _Person.PersonID.ToString();

                _Mode = enMode.Update;

                lblTitle.Text = "Update Person";

                MessageBox.Show("Date Saved Successfilly", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);

                DataBack?.Invoke(this, _Person.PersonID);


            }
            else
                MessageBox.Show("Error: Data Is not Saved Successfully.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            
        }
        private void lnklblSetImage_MouseClick(object sender, MouseEventArgs e)
        {
            OpenFileDialog OpenFileDialog1 = new OpenFileDialog();
            OpenFileDialog1.Filter = "Image Files |*.jpg;*jepg;*.png;*.gif;*.bmp";
            OpenFileDialog1.FilterIndex = 1;
            OpenFileDialog1.RestoreDirectory = true;

            if(OpenFileDialog1.ShowDialog() == DialogResult.OK)
            {
                string SelectedFilePath = OpenFileDialog1.FileName;
                picbxMainPicture.Load(SelectedFilePath);
                lnklblRemoveImage.Visible = true;
            }

        }      
        private void txtbxEmail_Validating(object sender, CancelEventArgs e)
        {
            if (txtbxEmail.Text.Trim() == "")
                return;
            if(!Validation.ValidateEmail(txtbxEmail.Text))
            {
                e.Cancel = true;
                errorProvider1.SetError(txtbxEmail, "Invalid Email Address Format !");
            }
            else
            {
                errorProvider1.SetError(txtbxEmail,null);
            }
        }
        private void ValidateEmptyTextBox(object sender,CancelEventArgs e)
        {
            TextBox Temp = ((TextBox)sender);
            if(String.IsNullOrEmpty(Temp.Text.Trim()))
            {
                e.Cancel = true;
                errorProvider1.SetError(Temp, "This Feild Is Required !");
            }
            else
            {

                errorProvider1.SetError(Temp, null);
            }
        }
        private void txtbxNationalNumber_Validating(object sender, CancelEventArgs e)
        {
            if (string.IsNullOrEmpty(txtbxNationalNumber.Text.Trim()))
            {
                e.Cancel = false;
                errorProvider1.SetError(txtbxNationalNumber, "This Feild Is Required !");
                return;
            }
            else
            {
                errorProvider1.SetError(txtbxNationalNumber, null);
            }


            if (txtbxNationalNumber.Text.Trim() != _Person.NationalNumber &&
                Person.isPersonExist(txtbxNationalNumber.Text.Trim()))
            {
                e.Cancel = false;
                errorProvider1.SetError(txtbxNationalNumber, "This Number Is Used For Another Person !");
            }
              
            else
            {
                errorProvider1.SetError(txtbxNationalNumber, null);
            }
        }
        private void lnklblRemoveImage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            picbxMainPicture.ImageLocation = null;
            if(rbMale.Checked)
            {
                picbxMainPicture.Image = Resources.Male_512;
            }
            else
            {
                picbxMainPicture.Image = Resources.Female_512;
            }
            lnklblRemoveImage.Visible = false;
        }
    }
}
