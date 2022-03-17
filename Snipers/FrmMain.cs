using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Snipers
{
    public partial class FrmMain : Form
    {
        private string _filename = "host.cfg";
        private string _hostname = @"Suporte\mssql2008ent";
        private DatabaseProcessKiller TheSniper { get; set; }

        public FrmMain()
        {
            this.TheSniper = null;
            InitializeComponent();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            LoadConfig();

            GridView.AllowUserToResizeRows = false;
            GridView.AllowUserToOrderColumns = true;
            GridView.RowHeadersVisible = false;
            this.KeyPreview = true;
            GridView.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            GridView.Focus();

            this.KeyPreview = true;
            txtFilter.Focus();
        }

        private void GridView_CellContentDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)    // remove seleção de headers
            {
                int spid = int.Parse(GridView.Rows[e.RowIndex].Cells["Spid"].Value.ToString());
                Kill(spid);
            }
        }

        private void Find()
        {
            Button button = btnUpdate;
            button.Enabled = false;

            string oldHost = (this.TheSniper == null ? "" : this.TheSniper.Connection.Connection.DataSource.ToUpper().Trim());
            string newHost = txtHost.Text.ToUpper().Trim();

            GridView.DataSource = null;

            if (oldHost != newHost)
            {
                try
                {
                    txtHost.ForeColor = Color.Orange;
                    txtHost.Font = new Font(txtHost.Font, FontStyle.Bold);

                    this.TheSniper = new DatabaseProcessKiller(Connection.Connect(newHost));

                    SaveConfig();

                    txtHost.ForeColor = Color.Green;
                    txtHost.Font = new Font(txtHost.Font, FontStyle.Bold);
                }
                catch (Exception ex)
                {
                    this.TheSniper = null;
                    txtHost.ForeColor = Color.Crimson;
                    txtHost.Font = new Font(txtHost.Font, FontStyle.Bold);

                    MessageBox.Show("Não foi possível conectar no servidor especificado. " + ex.Message);
                }
            }

            if (this.TheSniper != null)
            {
                string filter = (txtFilter.Text.Trim() == string.Empty ? null : txtFilter.Text);
                this.TheSniper.Load(filter);
                GridView.DataSource = null;
                GridView.DataSource = this.TheSniper.Processes;
                GridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
            }

            button.Enabled = true;
        }

        private void Kill(int spid)
        {
            if (Program.OcultarProcessosDosDesenvolvedores)
            {
                DatabaseProcess proc = this.TheSniper.Processes.Where(x => x.Spid == spid).FirstOrDefault();

                if (proc != null)
                {
                    if (proc.HostName.ToUpper().Trim() == "PC12" || proc.HostName.ToUpper().Trim() == "RODRIGO")
                    {
                        MessageBox.Show("Êeeeepaaaa! Esse não.", "Exceção");
                        return;
                    }
                }
            }

            this.TheSniper.Kill(spid);
        }

        private void btnUpdate_Click(object sender, EventArgs e)
        {
            Find();
        }

        private void FrmMain_ResizeEnd(object sender, EventArgs e)
        {
            int with = this.Width - 30;
            int height = this.Height - (GridView.Top + 45);

            if (with > 200)
            {
                groupBox1.Width = with;
                GridView.Width = with;
            }

            if (height > 400)
            {
                GridView.Height = height;
            }
        }

        private void GridView_CellContentClick(object sender, DataGridViewCellEventArgs e)
        { }

        private void GridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                string columnName = GridView.Columns[e.ColumnIndex].Name.ToUpper();
                SortDirection direction;

                if (this.TheSniper.Sorting.ColumnName.ToUpper() == columnName)
                {
                    direction = (this.TheSniper.Sorting.Direction == SortDirection.Ascendant ? SortDirection.Descendant : SortDirection.Ascendant);
                }
                else
                {
                    direction = SortDirection.Ascendant;
                }

                this.TheSniper.OrderBy(columnName, direction);
            }
        }

        private void FrmMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F5)
            {
                Find();
            }
            else if (e.KeyCode == Keys.Delete)
            {
                foreach (DataGridViewRow xRow in GridView.SelectedRows)
                {
                    if (xRow.Index >= 0)    // remove seleção de headers
                    {
                        int spid = int.Parse(GridView.Rows[xRow.Index].Cells["Spid"].Value.ToString());
                        Kill(spid);
                    }
                }
            }
        }

        private void btnShoot_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow xRow in GridView.SelectedRows)
            {
                if (xRow.Index >= 0)    // remove seleção de headers
                {
                    int spid = int.Parse(GridView.Rows[xRow.Index].Cells["Spid"].Value.ToString());
                    Kill(spid);
                }
            }
        }

        private void GridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                if (e.RowIndex >= 0)    // remove seleção de headers
                {
                    int spid = int.Parse(GridView.Rows[e.RowIndex].Cells["Spid"].Value.ToString());
                    Kill(spid);
                }
            }
        }

        private void LoadConfig()
        {
            try
            {
                if (File.Exists(_filename))
                {
                    string hostTemp = File.ReadAllLines(_filename).Trim().LastOrDefault();
                    _hostname = string.IsNullOrEmpty(hostTemp) ? _hostname : hostTemp;
                }
            }
            catch { }


            txtHost.Text = _hostname;
        }

        private void SaveConfig()
        {
            try
            {
                _hostname = txtHost.Text;

                if (File.Exists(_filename))
                    File.Delete(_filename);

                File.WriteAllText(_filename, _hostname);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}