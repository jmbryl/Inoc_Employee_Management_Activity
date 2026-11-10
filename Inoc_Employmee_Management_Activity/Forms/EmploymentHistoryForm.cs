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
    public partial class EmploymentHistoryForm : Form
    {
        private readonly EmployeeManagementDataDataContext db =
            new EmployeeManagementDataDataContext(
                Properties.Settings.Default.EmployeeManagementDBConnectionString);


        private void LoadEmploymentHistory()
        {
            if (string.IsNullOrWhiteSpace(txtEmployeeID.Text))
            {
                dgvEmploymentHistory.DataSource = null;
                return;
            }

            var history = db.usp_EmployeeHistory_ReadByEmployee(
                txtEmployeeID.Text.Trim()
            ).ToList();

            dgvEmploymentHistory.DataSource = history;
        }

        private void ClearHistoryFields()
        {
            txtHistoryID.Clear();
            txtCompanyName.Clear();
            txtJobTitle.Clear();
            txtReasonForLeaving.Clear();
            txtNotes.Clear();

            cmbEmploymentType.SelectedIndex = -1;

            dtpStartDate.Value = DateTime.Today;
            dtpEndDate.Value = DateTime.Today;

            chkCurrentEmployment.Checked = false;
        }

        public EmploymentHistoryForm()
        {
            InitializeComponent();
            dgvEmploymentHistory.DataSource = null;
        }

        public void SetEmployee(string employeeId)
        {
            txtEmployeeID.Text = employeeId;
            LoadEmploymentHistory();
        }

        private void dgvEmploymentHistory_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnAddHistory_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmployeeID.Text) ||
        string.IsNullOrWhiteSpace(txtCompanyName.Text) ||
        string.IsNullOrWhiteSpace(txtJobTitle.Text) ||
        string.IsNullOrWhiteSpace(cmbEmploymentType.Text))
            {
                MessageBox.Show("Please complete the required fields.");
                return;
            }

            if (!chkCurrentEmployment.Checked &&
    dtpEndDate.Value.Date < dtpStartDate.Value.Date)
            {
                MessageBox.Show(
                    "End Date cannot be earlier than Start Date.",
                    "Invalid Dates",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            try
            {
                DateTime? endDate = chkCurrentEmployment.Checked
                    ? (DateTime?)null
                    : dtpEndDate.Value.Date;

                string reason = string.IsNullOrWhiteSpace(txtReasonForLeaving.Text)
                    ? null : txtReasonForLeaving.Text.Trim();

                string notes = string.IsNullOrWhiteSpace(txtNotes.Text)
                    ? null : txtNotes.Text.Trim();

                db.usp_EmployeeHistory_Create(
                    txtEmployeeID.Text.Trim(),
                    txtCompanyName.Text.Trim(),
                    txtJobTitle.Text.Trim(),
                    cmbEmploymentType.Text,
                    dtpStartDate.Value.Date,
                    endDate,
                    reason,
                    notes
                );

                MessageBox.Show("Employment history added successfully!");
                LoadEmploymentHistory();
                ClearHistoryFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to add employment history: " + ex.Message);
            }
        }

        private void btnSearchHistory_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmployeeID.Text))
            {
                MessageBox.Show("Please enter an Employee ID.");
                return;
            }

            try
            {
                LoadEmploymentHistory();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to load history: " + ex.Message);
            }
        }

        private void btnUpdateHistory_Click(object sender, EventArgs e)
        {

            if (string.IsNullOrWhiteSpace(txtEmployeeID.Text) ||
    string.IsNullOrWhiteSpace(txtCompanyName.Text) ||
    string.IsNullOrWhiteSpace(txtJobTitle.Text) ||
    string.IsNullOrWhiteSpace(cmbEmploymentType.Text))
            {
                MessageBox.Show("Please complete the required fields.");
                return;
            }

            if (!chkCurrentEmployment.Checked &&
                dtpEndDate.Value.Date < dtpStartDate.Value.Date)
            {
                MessageBox.Show("End date cannot be earlier than start date.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtHistoryID.Text))
            {
                MessageBox.Show("Please select an employment history record first.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtCompanyName.Text) ||
                string.IsNullOrWhiteSpace(txtJobTitle.Text) ||
                string.IsNullOrWhiteSpace(cmbEmploymentType.Text))
            {
                MessageBox.Show("Please complete the required fields.");
                return;
            }

            if (!chkCurrentEmployment.Checked &&
    dtpEndDate.Value.Date < dtpStartDate.Value.Date)
            {
                MessageBox.Show(
                    "End Date cannot be earlier than Start Date.",
                    "Invalid Dates",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning
                );
                return;
            }

            try
            {
                long historyId = Convert.ToInt64(txtHistoryID.Text);

                DateTime? endDate = chkCurrentEmployment.Checked
                    ? (DateTime?)null
                    : dtpEndDate.Value.Date;

                string reason = string.IsNullOrWhiteSpace(txtReasonForLeaving.Text)
                    ? null : txtReasonForLeaving.Text.Trim();

                string notes = string.IsNullOrWhiteSpace(txtNotes.Text)
                    ? null : txtNotes.Text.Trim();

                db.usp_EmployeeHistory_Update(
                    historyId,
                    txtCompanyName.Text.Trim(),
                    txtJobTitle.Text.Trim(),
                    cmbEmploymentType.Text,
                    dtpStartDate.Value.Date,
                    endDate,
                    reason,
                    notes
                );

                MessageBox.Show("Employment history updated successfully!");

                LoadEmploymentHistory();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unable to update history: " + ex.Message);
            }
        }

        private void dgvEmploymentHistory_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (e.RowIndex < 0)
                return;

            var row = dgvEmploymentHistory.Rows[e.RowIndex];

            txtHistoryID.Text = Convert.ToString(
                row.Cells["HistoryId"].Value);

            txtEmployeeID.Text = Convert.ToString(
                row.Cells["EmployeeId"].Value);

            txtCompanyName.Text = Convert.ToString(
                row.Cells["CompanyName"].Value);

            txtJobTitle.Text = Convert.ToString(
                row.Cells["JobTitle"].Value);

            cmbEmploymentType.Text = Convert.ToString(
                row.Cells["EmploymentType"].Value);

            dtpStartDate.Value = Convert.ToDateTime(
                row.Cells["StartDate"].Value);

            object endValue = row.Cells["EndDate"].Value;

            if (endValue == null || endValue == DBNull.Value)
            {
                chkCurrentEmployment.Checked = true;
                dtpEndDate.Value = DateTime.Today;
            }
            else
            {
                chkCurrentEmployment.Checked = false;
                dtpEndDate.Value = Convert.ToDateTime(endValue);
            }

            object reasonValue = row.Cells["ReasonForLeaving"].Value;
            txtReasonForLeaving.Text =
                reasonValue == null || reasonValue == DBNull.Value
                    ? "" : reasonValue.ToString();

            object notesValue = row.Cells["Notes"].Value;
            txtNotes.Text =
                notesValue == null || notesValue == DBNull.Value
                    ? "" : notesValue.ToString();
        }

        private void btnDeleteHistory_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtHistoryID.Text))
            {
                MessageBox.Show("Please select a history record first.");
                return;
            }

            DialogResult result = MessageBox.Show(
                "Are you sure you want to delete this history record?",
                "Confirm Delete",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning
            );

            if (result != DialogResult.Yes)
                return;

            try
            {
                long historyId = Convert.ToInt64(txtHistoryID.Text);

                db.usp_EmployeeHistory_Delete(historyId);

                MessageBox.Show("Employment history deleted successfully!");

                LoadEmploymentHistory();
                ClearHistoryFields();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Unable to delete history: " + ex.Message
                );
            }
        }

        private void btnClearHistory_Click(object sender, EventArgs e)
        {
            ClearHistoryFields();
        }
    }
}
