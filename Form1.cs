using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
namespace testkt
{
    public partial class Form1 : Form
    {
        string cnn = @"provider=Microsoft.ACE.OLEDB.12.0;Data Source=C:\Users\Votha\source\repos\chuong2bai4\qlsv.accdb";
        DataSet ds = new DataSet("qlsv");
        BindingSource bs;
        OleDbDataAdapter dapsv, dapkhoa, dapdiem;
        OleDbCommandBuilder cmbsv;
        public Form1()
        {
            InitializeComponent();
        }

        private void label6_Click(Object sender, EventArgs e)
        {
            
        }

        private void label7_Click(Object sender, EventArgs e)
        {

        }

        private void button6_Click(Object sender, EventArgs e)
        {
            bs.CancelEdit();
        }

        private void Form1_Load(Object sender, EventArgs e)
        {
            loaddata();
            Loadbs();
            loadcombo();
            btnsau.PerformClick();
            btntruoc.PerformClick();

        }

        private void Loadbs()
        {
            bs = new BindingSource(ds ,ds.Tables["sinhvien"].TableName);
            bindingNavigator1.BindingSource = bs;
            foreach (Control item in this.Controls)
            {
                if (item is TextBox&&item.Name!= "txttongdiem")
                {
                    (item as TextBox).DataBindings.Add("text", bs, item.Name.Substring(3),true);
                }
                else if (item is DateTimePicker)
                {
                    (item as DateTimePicker).DataBindings.Add("value", bs, item.Name.Substring(3),true);
                }
            }
            cbokhoa.DataBindings.Add("SelectedValue", bs, "MaKH", true);
            rdbphai.DataBindings.Add("Checked",bs, "Phai", true);
            bs.PositionChanged += PositionChanged;
        }
        private void PositionChanged(Object sender, EventArgs e)
        {
            lblstt.Text = $"{bs.Position+1}/{bs.Count}";
            string diem = ds.Tables["ketqua"].Compute("sum(diem)", $"Masv='{txtMaSV.Text}'") == DBNull.Value ? "0" : ds.Tables["ketqua"].Compute("sum(diem)", $"Masv='{txtMaSV.Text}'").ToString();
            txttongdiem.Text = $"{diem:##.#}";
            txtMaSV.ReadOnly = true;
        }
        private void btnsau_Click(Object sender, EventArgs e)
        {
            bs.MoveNext();
        }

        private void button1_Click(Object sender, EventArgs e)
        {
            bs.MovePrevious();
        }

        private void btnxoa_Click(Object sender, EventArgs e)
        {
            if (ds.Tables["sinhvien"].ChildRelations.Count>0)
            {
                MessageBox.Show("Thất bại đã tồn tại điểm");
                return;
            }
            bs.RemoveCurrent();
            if (dapsv.Update(ds,ds.Tables["sinhvien"].TableName)>0)
            {
                MessageBox.Show("Xoá thành công");
            }
            else
            {
                MessageBox.Show("Thất bại đã tồn tại điểm");
            }
        }

        private void btnhem_Click(Object sender, EventArgs e)
        {
            bs.AddNew();
            txtMaSV.ReadOnly = false;
        }

        private void btnghi_Click(Object sender, EventArgs e)
        {
            if (!txtMaSV.ReadOnly)
            {
                if (ds.Tables["sinhvien"].Rows.Contains(txtMaSV.Text))
                {
                    MessageBox.Show("Thất bại đã tồn tại điểm");
                    return;
                }
            }
                bs.EndEdit();
            if (dapsv.Update(ds, ds.Tables["sinhvien"].TableName) > 0)
            {
                MessageBox.Show("Xoá thành công");
            }
            else
            {
                MessageBox.Show("Thất bại đã tồn tại điểm");
            }
        }

        private void loadcombo()
        {
            cbokhoa.DataSource = ds.Tables["khoa"];
            cbokhoa.DisplayMember = "tenkh";
            cbokhoa.ValueMember = "makh";
        }

        private void loaddata()
        {
            dapsv = new OleDbDataAdapter("select * from sinhvien", cnn);
            dapsv.FillSchema(ds,SchemaType.Source,"SINHVIEN");
            dapsv.Fill(ds,ds.Tables["SINHVIEN"].TableName);
            cmbsv = new OleDbCommandBuilder(dapsv);
            dapkhoa = new OleDbDataAdapter("select * from khoa", cnn);
            dapkhoa.FillSchema(ds, SchemaType.Source, "khoa");
            dapkhoa.Fill(ds, ds.Tables["khoa"].TableName);
            dapdiem= new OleDbDataAdapter("select * from ketqua", cnn);
            dapdiem.FillSchema(ds, SchemaType.Source, "ketqua");
            dapdiem.Fill(ds, ds.Tables["ketqua"].TableName);
            DataRelation[] dsref = new DataRelation[]
            {
                new DataRelation("FK_SINHVIEN_KETQUA",ds.Tables["SINHVIEN"].Columns["MaSV"],ds.Tables["ketqua"].Columns["MaSV"],true),
                new DataRelation("FK_KHOA_SINHVIEN",ds.Tables["khoa"].Columns["MaKH"],ds.Tables["SINHVIEN"].Columns["MaKH"],true),
            };
            ds.Relations.AddRange(dsref);
            foreach (var item in ds.Relations)
            {
                ((DataRelation)item).ChildKeyConstraint.DeleteRule = Rule.None;
            }
        }
    }
}
