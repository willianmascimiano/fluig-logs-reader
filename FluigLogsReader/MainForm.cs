using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace FluigLogsReader
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {






        }

        private void serverlogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var filePath = string.Empty;
            var fileContent = string.Empty;

            using (OpenFileDialog fileDialog = new OpenFileDialog())
            {

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    filePath = fileDialog.FileName;




                    var fileStream = fileDialog.OpenFile();


                    //var regex = new Regex(@"^(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2},\d{3}) (\w+)");
                    //var regex = new Regex(@"^(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2},\d{3}) (\w+) \[([^\]]+)\]");

                    //var regex = new Regex(@"^(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2},\d{3}) (\w+) \[([^\]]+)\] \(([^\)]+)\)");
                    //var regex = new Regex(@"^(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2},\d{3}) (\w+) \[([^\]]+)\] \(([^\)]+)\) (.*)");
                    //var regex = new Regex(@"^(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2},\d{3}) (\w+) \[([^\]]+)\](?: \(([^)]+)\))? (.*)");
                    //var regex = new Regex(@"^(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2},\d{3})\s+(\w+)\s+\[([^\]]+)\]\s*\(([^)]*)\)\s*(.*)");
                    //var regex = new Regex(@"^(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2},\d{3})\s+(\w+)\s+\[([^\]]+)\]\s*\(([^)]*)\)\s*(?:([A-Z]{4,8}\d{4}):\s*)?(.*)");
                    var regex = new Regex(@"^(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2},\d{3})\s+(\w+)\s+\[([^\]]+)\]\s*\(([^)]*)\)\s*(?:([A-Z]{4,10}\d{4}):\s*)?(.*)");


                    List<LogFileModel> listLogFile = new List<LogFileModel>();


                    using (var reader = new StreamReader(filePath))
                    {
                        string line;
                        while ((line = reader.ReadLine()) != null)
                        {
                            var match = regex.Match(line);
                            if (match.Success)
                            {

                                LogFileModel logFileModel = new()
                                {
                                    dateTime = DateTime.Parse(match.Groups[1].Value),
                                    logType = match.Groups[2].Value,
                                    logClassName = match.Groups[3].Value,
                                    thread = match.Groups[4].Value,
                                    wildflyError = match.Groups[5].Success ? match.Groups[5].Value.Trim() : "",
                                    message = match.Groups[6].Value
                                };
                                listLogFile.Add(logFileModel);
                                System.Diagnostics.Debug.WriteLine(logFileModel.dateTime);






                            }


                        }
                        dataGridServerLog.DataSource = listLogFile;
                    }




                }
            }



        }

        private void dataGridServerLog_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void splitContainer1_Panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void splitContainer1_Panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {


            if (dataGridServerLog.Rows.Count == 0)
            {
                MessageBox.Show("Você precisa carregar o arquivo de Log antes de tentar exportar o arquivo", "Error",MessageBoxButtons.OK);
                return;
            }

            // Get the data source that's bound to the DataGridView
            List<LogFileModel> logData = (List<LogFileModel>)dataGridServerLog.DataSource;

            try
            {
                // Create Excel application
                var excelApp = new Excel.Application();
                excelApp.Visible = true;

                // Add a new workbook
                excelApp.Workbooks.Add();

                // Get the active worksheet
                Excel._Worksheet worksheet = (Excel.Worksheet)excelApp.ActiveSheet;

                // Set worksheet name
                worksheet.Name = "Server.log";

                // Add headers
                worksheet.Cells[1, 1] = "DateTime";
                worksheet.Cells[1, 2] = "Log Type";
                worksheet.Cells[1, 3] = "Class Name";
                worksheet.Cells[1, 4] = "Thread";
                worksheet.Cells[1, 5] = "WildFly_Error";
                worksheet.Cells[1, 6] = "Message";

                // Format header row
                Excel.Range headerRange = worksheet.Range["A1:F1"];
                headerRange.Font.Bold = true;
                headerRange.Interior.Color = ColorTranslator.ToOle(Color.LightGray);
                headerRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;
                headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

                // Populate data
                int row = 2;
                foreach (var logEntry in logData)
                {
                    worksheet.Cells[row, 1] = logEntry.dateTime.ToString("yyyy-MM-dd HH:mm:ss,fff");
                    worksheet.Cells[row, 2] = logEntry.logType;
                    worksheet.Cells[row, 3] = logEntry.logClassName;
                    worksheet.Cells[row, 4] = logEntry.thread;
                    worksheet.Cells[row, 5] = logEntry.wildflyError;
                    worksheet.Cells[row, 6] = logEntry.message;
                    row++;
                }

                // Auto-fit columns
                worksheet.Columns.AutoFit();

                // Apply borders to the data
                Excel.Range dataRange = worksheet.Range[$"A1:E{row - 1}"];
                dataRange.Borders.LineStyle = Excel.XlLineStyle.xlContinuous;

                MessageBox.Show("Export completed successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting to Excel: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }
    }
}
