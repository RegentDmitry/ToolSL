using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ToolSL
{
    public partial class HistoryForm : Form
    {
        private DataTable source;

        private void UpdateData()
        {
            var files = Remote.Service.GetPlayerFileList(Utils.MachineToken,
                                                         startDateEdit.Value.ToString("dd.MM.yyyy"),
                                                         endDateEdit.Value.ToString("dd.MM.yyyy"));

            source = new DataTable();
            source.Columns.Add(new DataColumn("date", typeof(DateTime)));
            source.Columns.Add(new DataColumn("path", typeof(string)));

            foreach (var h in files)
            {
                var newRow = source.NewRow();
                newRow["date"] = h.Item1;
                newRow["path"] = h.Item2;
                source.Rows.Add(newRow);
            }

            gridView.DataSource = source;

            if (files.Length == 0)
                return;

            gridView.Columns[0].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        public HistoryForm()
        {
            InitializeComponent();

            startDateEdit.Value = DateTime.Today.AddDays(-1);
            endDateEdit.Value = DateTime.Today;

            UpdateData();
        }

        private void refreshButton_Click(object sender, EventArgs e)
        {
            UpdateData();
        }
    }
}
