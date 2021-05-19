using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using System.Net;
using System.Threading;
using System.Text.RegularExpressions;
using System.Globalization;
using DevExpress.DataAccess.Excel;
using System.IO;

namespace DxImageDownloadBot
{
    public partial class FrmMain : DevExpress.XtraEditors.XtraForm
    {
        WebClient webClient = new WebClient();
        public FrmMain()
        {
            InitializeComponent();
        }

        int count = 0;
        string saveurl,openurl;
        
        
        private void FrmMain_Load(object sender, EventArgs e)
        {
            Thread.Sleep(2000);
            tprogressbar.Enabled = false;
            
            gridControl1.DataSource = null; 
            gridView1.Columns.Clear();
            this.gridView1.OptionsBehavior.Editable = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
        }
        private void bbi_save_ItemClick(object sender, ItemClickEventArgs e)
        {
            if (xfbd_images.ShowDialog() == DialogResult.OK)
            {
                saveurl = xfbd_images.SelectedPath;
            }
        }
        private void bbi_databaseopen_ItemClick(object sender, ItemClickEventArgs e)
        {
            xofd_databaseopen.Filter = "Excel Files|*.xlsx;*.xlsm;*.xlsb;*.xltx;*.xltm;*.xls;*.xlt;*.xls;*.xml;*.xml;*.xlam;*.xla;*.xlw;*.xlr;";
            if (xofd_databaseopen.ShowDialog() == DialogResult.OK)
            {
                openurl = xofd_databaseopen.FileName;

                ExcelDataSource myExcelSource = new ExcelDataSource();
                myExcelSource.FileName = openurl;
                ExcelWorksheetSettings worksheetSettings = new ExcelWorksheetSettings("Sheet1");
                myExcelSource.SourceOptions = new ExcelSourceOptions(worksheetSettings);
                myExcelSource.SourceOptions = new CsvSourceOptions() { CellRange = "A1:A999999" };
                myExcelSource.SourceOptions.SkipEmptyRows = false;
                myExcelSource.SourceOptions.UseFirstRowAsHeader = true;
                myExcelSource.Fill();
                gridControl1.DataSource = myExcelSource;
            }
            else
            {
                return;
            }
        }

        private void bbi_databaseclear_ItemClick(object sender, ItemClickEventArgs e)
        {
            gridControl1.DataSource = null;
            gridView1.Columns.Clear();
        }
        private void bbi_databasesettings_ItemClick(object sender, ItemClickEventArgs e)
        {

            XtraMessageBox.Show(gridView1.RowCount.ToString());
            /*
            FrmDatabaseSettings frmDatabaseSettings = new FrmDatabaseSettings();
            frmDatabaseSettings.ShowDialog();
            */

        }
        private void btn_download_Click(object sender, EventArgs e)
        {
         
            pbc_count.Properties.Maximum = gridView1.RowCount;
            pbc_count.Position = 0;

            tprogressbar.Enabled = true;
         

        }

        static Uri SomeBaseUri = new Uri("https://www.ozcalbilisimhizmetleri.com");

        static string GetFileNameFromUrl(string url)
        {
            Uri uri;
            if (!Uri.TryCreate(url, UriKind.Absolute, out uri))
                uri = new Uri(SomeBaseUri, url);

            return Path.GetFileName(uri.LocalPath);
        }

        private void tprogressbar_Tick(object sender, EventArgs e)
        {
            if (saveurl != null)
            {
                try
                {
                    if (count < 1)
                    {
                        for (int i = 0; i < gridView1.RowCount; i++)
                        {
                            string dataInCell = Convert.ToString(gridView1.GetRowCellValue(i, "web_link"));

                          
                            Uri uri = new Uri(dataInCell);

                            webClient.DownloadFile(uri, @saveurl +"/"+ GetFileNameFromUrl(dataInCell));

                            pbc_count.EditValue = i;
                        }
                        count++;
                    }
                    else
                    {
                        tprogressbar.Enabled = false;
                        XtraMessageBox.Show("DOWNLOAD COMPLETED", "WARING", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                catch (Exception ex)
                {
                    XtraMessageBox.Show(ex.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
            else
            {
                XtraMessageBox.Show("PLEASE SELECT THE FILE TO DOWNLOAD", "ERROR" , MessageBoxButtons.OK,MessageBoxIcon.Error);

                tprogressbar.Enabled = false;
            
            }
        }

       

    }
}