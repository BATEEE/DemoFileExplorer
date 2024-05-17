using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;
namespace ListViewDemoNangCao
{
    public partial class Form1 : Form
    {
        String path = "";

        public Form1()
        {
            InitializeComponent();
        }


        private void InitListDrives()
        {
            String[] arrDrives = Directory.GetLogicalDrives();
            foreach (String drive in arrDrives)
            {
                comboBox1.Items.Add(drive);
            }
            comboBox1.SelectedIndex = 1;
            comboBox1.DropDownStyle = ComboBoxStyle.DropDownList;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            InitListDrives();
            foreach (RadioButton item in groupBox1.Controls)
            {
                item.CheckedChanged += Item_CheckedChanged;
            }
            listView1.View = View.LargeIcon;
        }

        private void Item_CheckedChanged(object sender, EventArgs e)
        {

            RadioButton item = (RadioButton)sender;
            if (item.Checked)
            {
                View view = (View)Enum.Parse(typeof(View), item.Text);
                listView1.View = view;
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listView1.Items.Clear();
            path = comboBox1.SelectedItem.ToString();
            String[] files = Directory.GetFileSystemEntries(comboBox1.SelectedItem.ToString());
            txtPath.Text = path;
            foreach (String file in files)
            {
                ListViewItem items = new ListViewItem();
                FileInfo fInfo;
                DirectoryInfo folderInfo;
                if (File.Exists(file))
                {
                    items.ImageIndex = 1;
                    fInfo = new FileInfo(file);
                    items.SubItems[0].Text = fInfo.Name;
                    items.SubItems.Add("");
                    items.SubItems.Add(fInfo.CreationTime.ToString());
                }
                else
                {

                    items.ImageIndex = 0;
                    folderInfo=new DirectoryInfo(file);
                    items.SubItems[0].Text = folderInfo.Name;
                    items.SubItems.Add("");
                    items.SubItems.Add(folderInfo.CreationTime.ToString());
                }
                listView1.Items.Add(items);
            }
        }
        private void listView1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListView item = sender as ListView;
            ListViewItem clickItem = item.GetItemAt(e.X, e.Y);
            if (clickItem != null)
            {
                path += @"\" + clickItem.SubItems[0].Text;
                txtPath.Text = path;
                if (!File.Exists(path))
                {
                    String[] files = Directory.GetFileSystemEntries(path);
                    listView1.Items.Clear();
                    foreach (String file in files)
                    {

                        ListViewItem items = new ListViewItem();
                        FileInfo fInfo;
                        DirectoryInfo folderInfo;
                        if (File.Exists(file))
                        {
                            items.ImageIndex = 1;
                            fInfo = new FileInfo(file);
                            items.SubItems[0].Text = fInfo.Name;
                            items.SubItems.Add("");
                            items.SubItems.Add(fInfo.CreationTime.ToString());
                        }
                        else
                        {

                            items.ImageIndex = 0;
                            folderInfo = new DirectoryInfo(file);
                            items.SubItems[0].Text = folderInfo.Name;
                            items.SubItems.Add("");
                            items.SubItems.Add(folderInfo.CreationTime.ToString());
                        }
                        listView1.Items.Add(items);
                    }
                }
                else
                {
                    try
                    {
                        Process.Start(path);
                        path = path.Substring(0, path.LastIndexOf('\\'));
                    }
                    catch (Exception ex) { MessageBox.Show(ex.Message); }
                }
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            if (path.Length <= 3)
                return;  
               path=path.Substring(0,path.LastIndexOf("\\"));
            txtPath.Text = path;
            String[] files = Directory.GetFileSystemEntries(path);
            if (files == null)
            {
                return;
            }
            else
            { 
                listView1.Items.Clear();
                foreach (String file in files)
                {
                    ListViewItem items = new ListViewItem();
                    FileInfo fInfo;
                    DirectoryInfo folderInfo;
                    if (File.Exists(file))
                    {
                        items.ImageIndex = 1;
                        fInfo = new FileInfo(file);
                        items.SubItems[0].Text = fInfo.Name;
                        items.SubItems.Add("");
                        items.SubItems.Add(fInfo.CreationTime.ToString());
                    }
                    else
                    {

                        items.ImageIndex = 0;
                        folderInfo = new DirectoryInfo(file);
                        items.SubItems[0].Text = folderInfo.Name;
                        items.SubItems.Add("");
                        items.SubItems.Add(folderInfo.CreationTime.ToString());
                    }
                    listView1.Items.Add(items);
                }
            }
            
        }

        // ham them danh sach thu muc va file
        private void FolderAndFile()
        {

        }

        private void txtPath_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (Directory.Exists(txtPath.Text))
                {
                    path = txtPath.Text;
                    txtPath.Text = path;

                    if (!File.Exists(path))
                    {
                        UpdateListView();
                    }
                    else
                    {
                        try
                        {
                            Process.Start(path);
                            path = path.Substring(0, path.LastIndexOf('\\'));
                        }
                        catch (Exception ex) { MessageBox.Show(ex.Message); }
                    }
                }
                else
                {
                    MessageBox.Show("Đường dẫn không tồn tại !!!", "Thông báo");
                }
            }
        }

        private void menuNewFolder_Click(object sender, EventArgs e)
        {
            if (path.Length == 0)
            {
                MessageBox.Show("Vui lòng chọn đường dẫn để tạo thư mục mới.", "Error");
                return;
            }

            string newFolderPath = Path.Combine(path, "New folder");

            try
            {
                //Tao folder moi
                Directory.CreateDirectory(newFolderPath);
                //Cap nhat listview
                UpdateListView();
                ListViewItem currentItem = listView1.Items.Find();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tạo thư mục: " + ex.Message, "Error");
            }
        }

        private void UpdateListView()
        {
            //xoa cac file hien tai
            listView1.Items.Clear();

            String[] files = Directory.GetFileSystemEntries(path);
            foreach (String file in files)
            {

                ListViewItem items = new ListViewItem();
                FileInfo fInfo;
                DirectoryInfo folderInfo;
                if (File.Exists(file)) //kiem tra neu la file
                {  //them hing file
                    items.ImageIndex = 1;
                    fInfo = new FileInfo(file);
                    items.SubItems[0].Text = fInfo.Name;
                    items.SubItems.Add("");
                    items.SubItems.Add(fInfo.CreationTime.ToString());
                }
                else
                {
                    //them hinh thu muc
                    items.ImageIndex = 0;
                    folderInfo = new DirectoryInfo(file);
                    items.SubItems[0].Text = folderInfo.Name;
                    items.SubItems.Add("");
                    items.SubItems.Add(folderInfo.CreationTime.ToString());
                }
                listView1.Items.Add(items);
            }
        }

        private string GetNewFolderName()
        {
            string folderName = "";
            using (var form = new Form())
            {
                form.Width = 500;
                form.Height = 100;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.Text = "New Folder Name";

                var textBox = new TextBox();
                textBox.Dock = DockStyle.Fill;

                var button = new Button();
                button.Text = "OK";
                button.Location = new System.Drawing.Point(0, 0);
                button.Left += form.Width - button.Width - 17;
                button.Top += form.Height - button.Height * 2 - 17;
                button.Click += (sender, e) =>
                {
                    folderName = textBox.Text;
                    form.Close();
                };

                form.Controls.Add(textBox);
                form.Controls.Add(button);
                form.ShowDialog();
            }
            return folderName;
        }

        private void listView1_MouseDown(object sender, MouseEventArgs e)
        {
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //item duoc chon
            ListViewItem selectedItem = listView1.SelectedItems[0];

            if (selectedItem == null)
            {
                return;
            }
            
            //lay duong dan cua file or thu muc muon xoa
            string itemPath = Path.Combine(path, selectedItem.Text);
            
            if(File.Exists(itemPath))
            {
                //xoa File
                try
                {
                    DialogResult result = MessageBox.Show("Bạn có chắc muốn xóa file hoặc folder này không?", "Xác nhận xóa", MessageBoxButtons.OKCancel);
                    if (result == DialogResult.OK)
                    {
                        File.Delete(itemPath);
                        listView1.Items.Remove(selectedItem);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                //xoa folder
                try
                {
                    DialogResult result = MessageBox.Show("Bạn có chắc muốn xóa file hoặc folder này không?", "Xác nhận xóa", MessageBoxButtons.OKCancel);
                    if(result == DialogResult.OK)
                    {
                        Directory.Delete(itemPath, true);
                        listView1.Items.Remove(selectedItem);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void renameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            listView1.LabelEdit = true;
            listView1.SelectedItems[0].BeginEdit();
        }
        //sự kiện xảu ra sau khi chỉnh sửa tên
        private void listView1_AfterLabelEdit(object sender, LabelEditEventArgs e)
        {
            if (e.Label != null)
            {
                string currentName = listView1.Items[e.Item].Text;
                string currentPath = Path.Combine(path, currentName);
                string newPath = Path.Combine(path, e.Label);
                try
                {
                    if (File.Exists(currentPath))
                    {
                        File.Move(currentPath, newPath);
                    }
                    else if (Directory.Exists(currentPath))
                    {
                        Directory.Move(currentPath, newPath);
                    }
                    e.CancelEdit = true;
                    String[] files = Directory.GetFileSystemEntries(path);   //lay thu muc de load lai listview
                    UpdateListView();
                }
                catch (IOException ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }
    }
}
