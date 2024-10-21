using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DataBingding
{
    public partial class Form1 : Form
    {
        private BindingSource  bindingSource = new BindingSource();
        SINHVIENEntities me = new SINHVIENEntities();
        public Form1()
        {
            InitializeComponent();
        }

        private void dgvStudent_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            // Lấy thông tin từ các trường nhập liệu
            string fullname = txtFullname.Text;
            int age;
            if (int.TryParse(txtAge.Text, out age)) // Kiểm tra xem tuổi có phải là số hợp lệ không
            {
                string selectedMajor = cmbMajor.SelectedItem.ToString(); // Lấy chuyên ngành được chọn

                // Tạo đối tượng sinh viên mới
                Student newStudent = new Student
                {
                    FullName = fullname,
                    Age = age,
                    Major = selectedMajor
                };

                // Thêm sinh viên mới vào cơ sở dữ liệu
                me.Students.Add(newStudent);
                me.SaveChanges();

                // Cập nhật lại DataGridView với dữ liệu mới
                bindingSource.DataSource = me.Students.ToList();
                dgvStudent.DataSource = bindingSource;
            }
            else
            {
                MessageBox.Show("Tuổi phải là số hợp lệ.");
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            // Kiểm tra xem có sinh viên nào được chọn hay không
            if (bindingSource.Current != null)
            {
                // Lấy sinh viên hiện tại từ BindingSource
                Student selectedStudent = bindingSource.Current as Student;

                if (selectedStudent != null)
                {
                    // Xóa sinh viên khỏi cơ sở dữ liệu
                    me.Students.Remove(selectedStudent);
                    me.SaveChanges();

                    // Cập nhật lại DataGridView sau khi xóa
                    bindingSource.DataSource = me.Students.ToList();
                    dgvStudent.DataSource = bindingSource;
                }
            }
            else
            {
                MessageBox.Show("Không có sinh viên nào được chọn.");
            }
        }
        //btnSửa_Click(object sender, EventArgs e)
        //{
        //    if (dataGridView1.CurrentRow != null && ValidateInput())
        //    {
        //        var student = (Students)dataGridView1.CurrentRow.DataBoundItem;
        //        student.FullName = txtFullName.Text;
        //        student.Age = int.Parse(txtAge.Text);
        //        student.Major = cmbMajor.SelectedItem.ToString();

        //        context.SaveChanges();

        //        // Cập nhật lại DataGridView
        //        LoadData();
        //    }
        //}
        private void btnEdit_Click(object sender, EventArgs e)
        {

            if (dgvStudent.CurrentRow !=  null)
            {
                // Lấy sinh viên hiện tại từ BindingSource
                Student selectedStudent = (Student)bindingSource.Current ;

                if (selectedStudent != null)
                {
                    // Lấy thông tin mới từ các trường nhập liệu
                    string newFullname = txtFullname.Text;
                    int newAge = int.Parse(txtAge.Text);
                    if (int.TryParse(txtAge.Text, out newAge))
                    {
                        string newMajor = cmbMajor.SelectedItem.ToString();

                        // Cập nhật thông tin sinh viên
                        selectedStudent.FullName = newFullname;
                        selectedStudent.Major = newMajor;                  
                        selectedStudent.Age = newAge;

                        // Lưu thay đổi vào cơ sở dữ liệu
                        me.SaveChanges();

                        // Xóa ràng buộc cũ và thêm lại để cập nhật chính xác giá trị mới
                        txtFullname.DataBindings.Clear();
                        txtAge.DataBindings.Clear();

                        // Thêm lại ràng buộc sau khi chỉnh sửa

                        txtFullname.DataBindings.Add("Text", bindingSource, "FullName");
                        txtAge.DataBindings.Add("Text", bindingSource, "Age");

                        // Cập nhật lại BindingSource và DataGridView
                        bindingSource.ResetBindings(false);
                        dgvStudent.Refresh();
                    }
                    else
                    {
                        MessageBox.Show("Tuổi phải là số hợp lệ.");
                    }
                }
            }
            else
            {
                MessageBox.Show("Không có sinh viên nào được chọn để chỉnh sửa.");
            }
           
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // Lấy danh sách sinh viên từ cơ sở dữ liệu
            var student = me.Students.ToList();
            bindingSource.DataSource = student;

            // Gán DataSource cho DataGridView
            dgvStudent.DataSource = bindingSource;

            // Gọi hàm load chuyên ngành cho ComboBox
            Loadcmb();

            // Xóa các ràng buộc cũ (nếu có)
            txtFullname.DataBindings.Clear();
            txtAge.DataBindings.Clear();
            cmbMajor.DataBindings.Clear();

            // Thêm ràng buộc mới
            txtFullname.DataBindings.Add("Text", bindingSource, "FullName");
            txtAge.DataBindings.Add("Text", bindingSource, "Age");
            cmbMajor.DataBindings.Add("SelectedItem", bindingSource, "Major");

            // Đặt giá trị mặc định cho ComboBox
            if (cmbMajor.Items.Count > 0)
            {
                cmbMajor.SelectedIndex = 0; // Đặt giá trị mặc định nếu có ít nhất một mục
            }

            // Lọc sinh viên theo chuyên ngành
            string selectedMajor = cmbMajor.SelectedItem?.ToString(); // Lấy giá trị chuyên ngành hiện tại
            if (!string.IsNullOrEmpty(selectedMajor))
            {
                var filteredStudents = me.Students.Where(s => s.Major == selectedMajor).ToList();
                bindingSource.DataSource = filteredStudents;
            }

            // Cập nhật lại DataGridView với danh sách sinh viên đã lọc
            dgvStudent.DataSource = bindingSource;

            // Đảm bảo DataGridView hiển thị đúng thông tin
            dgvStudent.Refresh(); // Refre

            //var student = me.Students.ToList();
            //bindingSource.DataSource = student;
            //dgvStudent.DataSource = bindingSource;
            //Loadcmb();

            //txtFullname.DataBindings.Clear();
            //txtAge.DataBindings.Clear();

            //txtFullname.DataBindings.Add("Text", bindingSource, "FullName");
            //txtAge.DataBindings.Add("Text", bindingSource, "Age");
            //string selectedMajor = cmbMajor.SelectedItem.ToString();

            //// Lọc sinh viên theo chuyên ngành
            //var filteredStudents = me.Students.Where(s => s.Major == selectedMajor).ToList();

            //// Cập nhật lại DataGridView với danh sách sinh viên đã lọc
            //bindingSource.DataSource = filteredStudents;
            //dgvStudent.DataSource = bindingSource;


        }
      

        void Loadcmb()
        {
            cmbMajor.Items.Add("Công nghệ thông tin");
            cmbMajor.Items.Add("Kinh tế");
            cmbMajor.Items.Add("Quản trị kinh doanh");

            // Đặt giá trị mặc định cho ComboBox (nếu cần)
            cmbMajor.SelectedIndex = 0;
        }
        private void cmbMajor_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void btnLui_Click(object sender, EventArgs e)
        {
            bindingSource.MovePrevious();
        }

        private void btnToi_Click(object sender, EventArgs e)
        {
            bindingSource.MoveNext();
        }
    }
}
