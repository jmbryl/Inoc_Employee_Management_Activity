using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Inoc_Employmee_Management_Activity.Forms
{
    public partial class EmployeeForm : Form
    {
        private readonly EmployeeManagementDataDataContext db =
            new EmployeeManagementDataDataContext(
                Properties.Settings.Default.EmployeeManagementDBConnectionString);

        private bool ValidateEmployeeFields()
        {
            errorProvider1.Clear();

            bool isValid = true;

            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                errorProvider1.SetError(txtFirstName, "First name is required.");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                errorProvider1.SetError(txtLastName, "Last name is required.");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                errorProvider1.SetError(txtEmail, "Email is required.");
                isValid = false;
            }
            else if (!System.Text.RegularExpressions.Regex.IsMatch(
                txtEmail.Text.Trim(),
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                errorProvider1.SetError(txtEmail, "Enter a valid email address.");
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtJobTitle.Text))
            {
                errorProvider1.SetError(txtJobTitle, "Job title is required.");
                isValid = false;
            }

            if (dtpHireDate.Value.Date < dtpBirthDate.Value.Date)
            {
                errorProvider1.SetError(dtpHireDate, "Hire date cannot be before birth date.");
                isValid = false;
            }

            return isValid;
        }


        private void LoadEmployees(string search)
        {
            var rows = db.usp_Employee_Search(
                string.IsNullOrWhiteSpace(search) ? null : search.Trim(),
                null,
                null
            ).ToList();

            dgvEmployees.DataSource = rows;
        }

        private void LoadSearchFilters()
        {
            cmbSearchDepartment.DataSource = db.Departments.ToList();
            cmbSearchDepartment.DisplayMember = "DepartmentName";
            cmbSearchDepartment.ValueMember = "DepartmentId";
            cmbSearchDepartment.SelectedIndex = -1;

            cmbSearchStatus.Items.Clear();
            cmbSearchStatus.Items.Add("Active");
            cmbSearchStatus.Items.Add("On Leave");
            cmbSearchStatus.Items.Add("Resigned");
            cmbSearchStatus.Items.Add("Terminated");
            cmbSearchStatus.Items.Add("Retired");
            cmbSearchStatus.SelectedIndex = -1;
        }


        public EmployeeForm()
        {
            InitializeComponent();

            LoadDepartments();
            LoadEmployees(null);
            LoadSearchFilters();
        }

        private void EmployeeForm_Load(object sender, EventArgs e)
        {

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string search = string.IsNullOrWhiteSpace(txtSearch.Text)
                    ? null
                    : txtSearch.Text.Trim();

                int? departmentId = cmbSearchDepartment.SelectedValue == null
                    ? (int?)null
                    : Convert.ToInt32(cmbSearchDepartment.SelectedValue);

                string status = string.IsNullOrWhiteSpace(cmbSearchStatus.Text)
                    ? null
                    : cmbSearchStatus.Text;

                var rows = db.usp_Employee_Search(
                    search,
                    departmentId,
                    status
                ).ToList();

                dgvEmployees.DataSource = rows;
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to search employees: " + ex.Message,
                    "Search Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error
                );
            }
        }

        private void txtEmployeeID_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateEmployeeFields())
            {
                MessageBox.Show("Please correct the highlighted fields.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFirstName.Text) ||
        string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("Please enter the employee's first and last name.");
                return;
            }

            if (cmbDepartment.SelectedValue == null)
            {
                MessageBox.Show("Please select a department.");
                return;
            }

            try
            {
                string employeeId = null;

                db.usp_Employee_Create(
                    txtFirstName.Text.Trim(),
                    string.IsNullOrWhiteSpace(txtMiddleName.Text)
                        ? null : txtMiddleName.Text.Trim(),
                    txtLastName.Text.Trim(),
                    cmbSex.Text,
                    dtpBirthDate.Value,
                    string.IsNullOrWhiteSpace(txtEmail.Text)
                        ? null : txtEmail.Text.Trim(),
                    string.IsNullOrWhiteSpace(txtPhone.Text)
                        ? null : txtPhone.Text.Trim(),
                    string.IsNullOrWhiteSpace(txtAddress.Text)
                        ? null : txtAddress.Text.Trim(),
                    Convert.ToInt32(cmbDepartment.SelectedValue),
                    txtJobTitle.Text.Trim(),
                    cmbEmploymentStatus.Text,
                    dtpHireDate.Value,
                    numSalary.Value,
                    ref employeeId
                );

                MessageBox.Show("Employee added successfully! ID: " + employeeId);

                LoadEmployees(null);
            }
            catch (Exception ex)
            {
                string errorMessage = ex.ToString().ToLower();

                if (errorMessage.Contains("unique") ||
                    errorMessage.Contains("duplicate") ||
                    errorMessage.Contains("email"))
                {
                    MessageBox.Show(
                        "This email may already be registered. Please use a different email address.",
                        "Duplicate Email",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
                else if (errorMessage.Contains("foreign key") ||
                         errorMessage.Contains("reference constraint"))
                {
                    MessageBox.Show(
                        "The selected department or related record is invalid. Please check your selection.",
                        "Invalid Selection",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
                else if (errorMessage.Contains("check constraint") ||
                         errorMessage.Contains("constraint"))
                {
                    MessageBox.Show(
                        "Some information is invalid. Please review the employee details and try again.",
                        "Invalid Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
                else
                {
                    MessageBox.Show(
                        "Unable to add employee. Please check the information and try again.\n\nDetails: "
                        + ex.GetBaseException().Message,
                        "Database Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void dgvEmployees_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void dgvEmployees_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0)
                return;

            var employee = dgvEmployees.Rows[e.RowIndex]
                .DataBoundItem as usp_Employee_SearchResult;

            if (employee == null)
                return;

            txtEmployeeID.Text = employee.EmployeeId;
            txtFirstName.Text = employee.FirstName;
            txtMiddleName.Text = employee.MiddleName;
            txtLastName.Text = employee.LastName;
            txtEmail.Text = employee.Email;
            txtPhone.Text = employee.Phone;
            txtAddress.Text = employee.Address;
            txtJobTitle.Text = employee.JobTitle;
            cmbDepartment.Text = employee.DepartmentName;

            cmbSex.Text = employee.Sex;
            cmbEmploymentStatus.Text = employee.EmploymentStatus;

            dtpBirthDate.Value = employee.BirthDate;
            dtpHireDate.Value = employee.HireDate;

            lblAge.Text = employee.Age.ToString();
            numSalary.Value = employee.Salary;
        }

        private void cmbDepartment_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void LoadDepartments()
        {
            var departments = db.Departments
                .OrderBy(d => d.DepartmentName)
                .ToList();

            cmbDepartment.DataSource = departments;
            cmbDepartment.DisplayMember = "DepartmentName";
            cmbDepartment.ValueMember = "DepartmentId";
            cmbDepartment.SelectedIndex = -1;
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (!ValidateEmployeeFields())
            {
                MessageBox.Show("Please correct the highlighted fields.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEmployeeID.Text))
            {
                MessageBox.Show("Please select an employee to update.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("First Name and Last Name are required.");
                return;
            }

            if (cmbDepartment.SelectedValue == null)
            {
                MessageBox.Show("Please select a department.");
                return;
            }

            try
            {
                db.usp_Employee_Update(
                    txtEmployeeID.Text,
                    txtFirstName.Text.Trim(),
                    string.IsNullOrWhiteSpace(txtMiddleName.Text)
                        ? null : txtMiddleName.Text.Trim(),
                    txtLastName.Text.Trim(),
                    cmbSex.Text,
                    dtpBirthDate.Value,
                    string.IsNullOrWhiteSpace(txtEmail.Text)
                        ? null : txtEmail.Text.Trim(),
                    string.IsNullOrWhiteSpace(txtPhone.Text)
                        ? null : txtPhone.Text.Trim(),
                    string.IsNullOrWhiteSpace(txtAddress.Text)
                        ? null : txtAddress.Text.Trim(),
                    Convert.ToInt32(cmbDepartment.SelectedValue),
                    txtJobTitle.Text.Trim(),
                    cmbEmploymentStatus.Text,
                    dtpHireDate.Value,
                    numSalary.Value
                );

                MessageBox.Show("Employee updated successfully!");
                LoadEmployees(null);
            }
            catch (Exception ex)
            {
                string errorMessage = ex.ToString().ToLower();

                if (errorMessage.Contains("unique") ||
                    errorMessage.Contains("duplicate") ||
                    errorMessage.Contains("email"))
                {
                    MessageBox.Show(
                        "This email may already be registered. Please use a different email address.",
                        "Duplicate Email",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
                else if (errorMessage.Contains("foreign key") ||
                         errorMessage.Contains("reference constraint"))
                {
                    MessageBox.Show(
                        "The selected department or related record is invalid. Please check your selection.",
                        "Invalid Selection",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
                else if (errorMessage.Contains("check constraint") ||
                         errorMessage.Contains("constraint"))
                {
                    MessageBox.Show(
                        "Some information is invalid. Please review the employee details and try again.",
                        "Invalid Information",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning
                    );
                }
                else
                {
                    MessageBox.Show(
                        "Unable to update employee. Please check the information and try again.\n\nDetails: "
                        + ex.GetBaseException().Message,
                        "Database Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                }
            }
        }

        private void ClearFields()
        {
            txtEmployeeID.Clear();
            txtFirstName.Clear();
            txtMiddleName.Clear();
            txtLastName.Clear();
            txtEmail.Clear();
            txtPhone.Clear();
            txtAddress.Clear();
            txtJobTitle.Clear();

            cmbSex.SelectedIndex = -1;
            cmbDepartment.SelectedIndex = -1;
            cmbEmploymentStatus.SelectedIndex = -1;

            dtpBirthDate.Value = DateTime.Today;
            dtpHireDate.Value = DateTime.Today;

            numSalary.Value = 0;
            lblAge.Text = "Age will appear here";

            dgvEmployees.ClearSelection();
        }


        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmployeeID.Text))
            {
                MessageBox.Show("Please select an employee to delete.");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this employee?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result != DialogResult.Yes)
                return;

            try
            {
                db.usp_Employee_Delete(txtEmployeeID.Text);

                MessageBox.Show("Employee deleted successfully!");

                LoadEmployees(null);
                ClearFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to delete employee: " + ex.Message
                );
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        private void btnViewHistory_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmployeeID.Text))
            {
                MessageBox.Show("Please select an employee first.");
                return;
            }

            EmploymentHistoryForm historyForm = new EmploymentHistoryForm();
            historyForm.SetEmployee(txtEmployeeID.Text.Trim());
            historyForm.ShowDialog();
        }
    }
}
