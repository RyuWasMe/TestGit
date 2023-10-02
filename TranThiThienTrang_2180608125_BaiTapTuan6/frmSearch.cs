using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TranThiThienTrang_2180608125_BaiTapTuan6
{
    public partial class frmSearch : Form
    {
        static StudentDBContext context = new StudentDBContext();
        List<Student> listStudent = context.Students.ToList();
        List<Faculty> listFaculties = context.Faculties.ToList();
        private List<Student> searchResults = new List<Student>();
        private int totalSearchResults = 0;

        public frmSearch()
        {
            InitializeComponent();
        }

        private void FillFalcultyCombobox(List<Faculty> listFaculties)
        {
            if (listFaculties != null && listFaculties.Any())
            {
                this.cbbKhoa.DataSource = listFaculties;
                this.cbbKhoa.DisplayMember = "FacultyName";
                this.cbbKhoa.ValueMember = "FacultyID";
            }
            else
            {
                MessageBox.Show("Danh sách rỗng", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void frmSearch_Load(object sender, EventArgs e)
        {
            FillFalcultyCombobox(listFaculties);
            cbbKhoa.Text = string.Empty;
        }

        public void resetNull()
        {
            txtMaSV.Text = txtHoTen.Text = txtKetQua.Text = string.Empty;
        }

        private void btnTimKiem_Click(object sender, EventArgs e)
        {
            string studentID = txtMaSV.Text.Trim().ToLower();
            string fullName = txtHoTen.Text.Trim().ToLower();
            int facultyID = (int)cbbKhoa.SelectedValue;

            try
            {
                var query = from student in listStudent
                            where
                                (string.IsNullOrEmpty(studentID) || student.StudentID.ToString().Contains(studentID)) &&
                                (string.IsNullOrEmpty(fullName) || student.FullName.ToLower().Contains(fullName)) &&
                                (facultyID == 0 || student.FacultyID == facultyID)
                            select student;

                List<Student> newSearchResults = query.ToList();

                if (newSearchResults.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy sinh viên nào trong danh sách.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    foreach (var student in newSearchResults)
                    {
                        bool existsInGrid = dgvTimKiem.Rows.Cast<DataGridViewRow>().Any(row =>
                            row.Cells[0].Value.ToString().ToLower() == student.StudentID.ToString() ||
                            row.Cells[1].Value.ToString().ToLower() == student.FullName.ToLower());

                        if (existsInGrid)
                        {
                            MessageBox.Show("Sinh viên đã tồn tại trong danh sách.", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        else
                        {
                            int index = dgvTimKiem.Rows.Add();
                            dgvTimKiem.Rows[index].Cells[0].Value = student.StudentID;
                            dgvTimKiem.Rows[index].Cells[1].Value = student.FullName;
                            dgvTimKiem.Rows[index].Cells[2].Value = student.Faculty.FacultyName;
                            dgvTimKiem.Rows[index].Cells[3].Value = student.AverageScore;

                            totalSearchResults += 1;
                        }
                    }

                    txtKetQua.Text = totalSearchResults.ToString();
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Đã xảy ra lỗi khi tìm kiếm sinh viên: ", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnXoa_Click(object sender, EventArgs e)
        {
            dgvTimKiem.Rows.Clear();
            searchResults.Clear();
            totalSearchResults = 0;
            txtKetQua.Text = searchResults.Count.ToString();
            cbbKhoa.Text = string.Empty;
            resetNull();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
