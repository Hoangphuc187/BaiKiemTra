using bt.Class;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.Entity;


namespace TIEUHOANGPHUC1_2280602483
{
    public partial class frmQuanLySanPham : Form
    {
        private Model1 dbContext;
        private bool isEditing = false;
        public frmQuanLySanPham()
        {
            InitializeComponent();
            dbContext = new Model1();
        }

        private void frmQuanLySanPham_Load(object sender, EventArgs e)
        {
            try
            {
                List<LoaiSP> categories = dbContext.LoaiSP.ToList();
                List<Sanpham> products = dbContext.Sanpham.ToList();
                FillCategoryCombobox(categories);
                BindGrid(products);
                fontDialog1_Apply(null, null); // Ẩn các nút Lưu và Không Lưu
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void FillCategoryCombobox(List<LoaiSP> categories)
        {
            cboLoaiSP.DataSource = categories;
            cboLoaiSP.DisplayMember = "TenLoai";
            cboLoaiSP.ValueMember = "MaLoai";
        }

        private void BindGrid(List<Sanpham> products)
        {
            dvgSanPham.Rows.Clear();
            foreach (var product in products)
            {
                int index = dvgSanPham.Rows.Add();
                dvgSanPham.Rows[index].Cells[0].Value = product.MaSP;
                dvgSanPham.Rows[index].Cells[1].Value = product.TenSP;
                dvgSanPham.Rows[index].Cells[2].Value = product.NgayNhap;
                dvgSanPham.Rows[index].Cells[3].Value = product.LoaiSP.TenLoai;
            }
        }

        private void btThem_Click(object sender, EventArgs e)
        {
            ClearInputs();
            isEditing = false;
            txtMaSP.Enabled = true;
            ToggleSaveButtons(true);
        }
        private void ClearInputs()
        {
            txtMaSP.Clear();
            txtTenSP.Clear();
            dtNgaynhap.Value = DateTime.Now;
            cboLoaiSP.SelectedIndex = -1;
        }
        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(txtMaSP.Text) ||
                string.IsNullOrWhiteSpace(txtTenSP.Text) ||
                cboLoaiSP.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin.", "Cảnh báo");
                return false;
            }
            return true;
        }

        private void btSua_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs()) return;
            try
            {
                string productID = txtMaSP.Text;
                var product = dbContext.Sanpham.FirstOrDefault(p => p.MaSP == productID);
                if (product != null)
                {
                    if (MessageBox.Show("Cập nhật sản phẩm này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        product.TenSP = txtTenSP.Text;
                        product.NgayNhap = dtNgaynhap.Value;
                        product.MaLoai = cboLoaiSP.SelectedValue.ToString();
                        dbContext.SaveChanges();
                        BindGrid(dbContext.Sanpham.ToList());
                        MessageBox.Show("Sửa thành công!", "Thông báo");
                        ClearInputs();
                    }
                }
                else
                {
                    MessageBox.Show("Không tìm thấy sản phẩm để sửa.", "Lỗi");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi");
            }
        }

        private void btXoa_Click(object sender, EventArgs e)
        {
            try
            {
                string productID = txtMaSP.Text;
                var product = dbContext.Sanpham.FirstOrDefault(p => p.MaSP == productID);
                if (product != null)
                {
                    if (MessageBox.Show("Xóa sản phẩm này?", "Xác nhận", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        dbContext.Sanpham.Remove(product);
                        dbContext.SaveChanges();
                        BindGrid(dbContext.Sanpham.ToList());
                        MessageBox.Show("Xóa thành công!", "Thông báo");
                        ClearInputs();
                    }
                }
                else
                {
                    MessageBox.Show("Không tìm thấy sản phẩm để xóa.", "Lỗi");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}", "Lỗi");
            }
        }

      

        private void btThoat_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn thoát không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void dvgSanPham_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                DataGridViewRow selectedRow = dvgSanPham.Rows[e.RowIndex];

                txtMaSP.Text = selectedRow.Cells[0].Value.ToString();
                txtTenSP.Text = selectedRow.Cells[1].Value.ToString();
                dtNgaynhap.Value = DateTime.Parse(selectedRow.Cells[2].Value.ToString());
                cboLoaiSP.Text = selectedRow.Cells[3].Value.ToString();
            }
        }

        private void btLuu_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtMaSP.Text) ||
                string.IsNullOrWhiteSpace(txtTenSP.Text) ||
                cboLoaiSP.SelectedValue == null)
            {
                MessageBox.Show("Vui lòng nhập đầy đủ thông tin sản phẩm!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                var product = dbContext.Sanpham.FirstOrDefault(s => s.MaSP == txtMaSP.Text);
                if (product != null) // Cập nhật thông tin
                {
                    product.TenSP = txtTenSP.Text.Trim();
                    product.NgayNhap = dtNgaynhap.Value;
                    product.MaLoai = cboLoaiSP.SelectedValue.ToString();
                }
                else // Thêm mới
                {
                    product = new Sanpham
                    {
                        MaSP = txtMaSP.Text.Trim(),
                        TenSP = txtTenSP.Text.Trim(),
                        NgayNhap = dtNgaynhap.Value,
                        MaLoai = cboLoaiSP.SelectedValue.ToString()
                    };
                    dbContext.Sanpham.Add(product);
                }

                dbContext.SaveChanges();
                BindGrid(dbContext.Sanpham.ToList());
                MessageBox.Show("Lưu thông tin thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                ToggleSaveButtons(false);
                ClearInputs();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi lưu thông tin: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void btKLuu_Click(object sender, EventArgs e)
        {
            ClearInputs();
            isEditing = false;
        }

        private void ToggleSaveButtons(bool enable)
        {
            btLuu.Enabled = enable;
            btKLuu.Enabled = enable;
        }

        private void txtTimkim_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string searchValue = txtTimkim.Text;
                var filteredProducts = dbContext.Sanpham
                    .Where(p => p.TenSP.Contains(searchValue))
                    .ToList();
                BindGrid(filteredProducts);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi tìm kiếm: {ex.Message}", "Lỗi");
            }
        }

        private void fontDialog1_Apply(object sender, EventArgs e)
        {
            ToggleSaveButtons(false);
        }

        private void frmQuanLySanPham_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc muốn thoát không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                e.Cancel = true; 
            }
        }
    }
}

