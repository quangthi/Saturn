﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IRS_Demo
{
    public partial class NewSessionForm : Form
    {
        public RecordingForm recform;
        public FindSession findForm;    
        public NewSessionForm()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            loadInspectCbBox();
            loadSuspectCbBox();
            loadSupevi1CbBox();
            loadSupevi2CbBox();
        }

        public bool getNewSessionInfo()
        {
            CommonParam.mSesData.caseName = txtCaseName.Text;
            CommonParam.mSesData.suspectData._Ten = cbSuspectName.Text;
            CommonParam.mSesData.inspectorName = cbInspectName.Text;
            CommonParam.mSesData.supervisorName1 = cbSupevName1.Text;
            CommonParam.mSesData.supervisorName2 = cbSupevName2.Text;
            CommonParam.mSesData.caseCode = txtCaseCode.Text;
            CommonParam.mSesData.suspectData._MaDT = txtSuspectCode.Text;
            CommonParam.mSesData.inspectorCode = txtInspectCode.Text;
            CommonParam.mSesData.supervisorCode1 = txtSupeCode1.Text;
            CommonParam.mSesData.supervisorCode2 = txtSupeCode2.Text;
            CommonParam.mSesData.currentPlace = textBox10.Text;
            CommonParam.mSesData.Notes = txtNotes.Text;

            if (CommonParam.mSesData.supervisorCode1 == CommonParam.mSesData.supervisorCode2)
            {
                MessageBox.Show("GSV1 và GSV2 có tên và mã số giống nhau, đề nghị kiểm tra lại!");
                return false;
            }                

            return true;
        }

        private void btnNewSession_Click(object sender, EventArgs e)
        {
            if (!getNewSessionInfo())
                return;
            getSuspectInfoByCode(txtSuspectCode.Text);

            CommonParam.SessionFolderName = "SS_" + DateTime.Now.ToString(@"MM_dd_yyyy.h_mm_tt");
            System.IO.Directory.CreateDirectory(CommonParam.ProgramPath + "\\" + CommonParam.SessionFolderName);
            recform = new RecordingForm(this);
            
            CommonParam.mSesData.beginSessTime = DateTime.Now.Hour.ToString() + " giờ " + DateTime.Now.Minute.ToString() + " phút";            
            this.Hide();
            recform.ShowDialog();
            CommonParam.saveSession();
            this.Show();            
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            findForm = new FindSession(this);
            this.Hide();            
            findForm.ShowDialog();

            this.Show();
        }

        private void NewSessionForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult exitAppResult = MessageBox.Show("Bạn có muốn thoát khỏi chương trình?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            
            if (exitAppResult == DialogResult.No)
            {
                e.Cancel = true;
            }            
            
        }

        private DataTable m_InspectDataTable = new DataTable();
        private DataTable m_SuspectDataTable = new DataTable();
        private DataTable m_SuspeviDataTable = new DataTable();

        void loadInspectCbBox()
        {
            DataSet dataSet = new DataSet();
            CommonParam.GetInspectorsInfo();
            dataSet.Reset();
            CommonParam.sql_DataAdaptInspector.Fill(dataSet);
            m_InspectDataTable = dataSet.Tables[0];
            cbInspectName.DataSource = m_InspectDataTable;
            cbInspectName.DisplayMember = m_InspectDataTable.Columns[1].ToString();

            loadInspecCodeTxtBox();
        }

        void loadSuspectCbBox()
        {
            DataSet dataSet = new DataSet();
            CommonParam.GetSuspectsInfo();
            dataSet.Reset();
            CommonParam.sql_DataAdaptSuspect.Fill(dataSet);
            m_SuspectDataTable = dataSet.Tables[0];
            cbSuspectName.DataSource = m_SuspectDataTable;
            cbSuspectName.DisplayMember = m_SuspectDataTable.Columns[1].ToString();

            loadSuspecCodeTxtBox();
        }

        void loadSupevi1CbBox()
        {
            DataSet dataSet = new DataSet();
            CommonParam.GetSupervisorsInfo();
            dataSet.Reset();
            CommonParam.sql_DataAdaptSupervisor.Fill(dataSet);
            m_SuspeviDataTable = dataSet.Tables[0];
            cbSupevName1.DataSource = m_SuspeviDataTable;
            cbSupevName1.DisplayMember = m_SuspeviDataTable.Columns[1].ToString();

            loadSupevi1CodeTxtBox();
        }

        void loadSupevi2CbBox()
        {
            DataSet dataSet = new DataSet();
            CommonParam.GetSupervisorsInfo();
            dataSet.Reset();
            CommonParam.sql_DataAdaptSupervisor.Fill(dataSet);
            m_SuspeviDataTable = dataSet.Tables[0];
            cbSupevName2.DataSource = m_SuspeviDataTable;
            cbSupevName2.DisplayMember = m_SuspeviDataTable.Columns[1].ToString();

            cbSupevName2.SelectedIndex = 1;

            loadSupevi2CodeTxtBox();
        }

        private string getInspectCodeByName(string inspectName)
        {
            string filterExpression = "inspName=" + "'" + inspectName + "'";
            DataRow[] rows = m_InspectDataTable.Select(filterExpression);
            string strInspecCode = rows[0].ItemArray[2].ToString();
            return strInspecCode;
        }

        private void getSuspectInfoByCode(string suspectCode)
        {
            string filterExpression = "";
            filterExpression = "suspCode=" + "'" + suspectCode + "'";
            DataRow[] rows = m_SuspectDataTable.Select(filterExpression);
            CommonParam.mSesData.suspectData._GioiTinh = rows[0].ItemArray[3].ToString();
            CommonParam.mSesData.suspectData._TenGoiKhac = "........................";
            CommonParam.mSesData.suspectData._NgaySinh = rows[0].ItemArray[4].ToString();
            CommonParam.mSesData.suspectData._NoiSinh = ".......................................";
            CommonParam.mSesData.suspectData._QuocTich = rows[0].ItemArray[9].ToString();
            CommonParam.mSesData.suspectData._DanToc = ".............";
            CommonParam.mSesData.suspectData._TonGiao = ".............";
            CommonParam.mSesData.suspectData._NgheNghiep = rows[0].ItemArray[8].ToString();
            CommonParam.mSesData.suspectData._CMND = rows[0].ItemArray[7].ToString();
            CommonParam.mSesData.suspectData._NgayCapCMND = ".................";
            CommonParam.mSesData.suspectData._NoiCapCMND = ".................";
            CommonParam.mSesData.suspectData._DiaChi = rows[0].ItemArray[5].ToString();   
        }

        private void loadInspecCodeTxtBox() 
        {
            string strInspecName = cbInspectName.GetItemText(cbInspectName.SelectedItem);
            string filterExpression = "inspName=" + "'" + strInspecName + "'";
            DataRow[] rows = m_InspectDataTable.Select(filterExpression);
            string strInspecCode = rows[0].ItemArray[2].ToString();
            txtInspectCode.Text = strInspecCode;
        }

        private void loadSuspecCodeTxtBox()
        {
            string strSuspecName = cbSuspectName.GetItemText(cbSuspectName.SelectedItem);
            string filterExpression = "suspName=" + "'" + strSuspecName + "'";
            DataRow[] rows = m_SuspectDataTable.Select(filterExpression);
            string strSuspecCode = rows[0].ItemArray[2].ToString();
            txtSuspectCode.Text = strSuspecCode;
        }

        private void loadSupevi1CodeTxtBox()
        {
            string strSupevi1Name = cbSupevName1.GetItemText(cbSupevName1.SelectedItem);
            string filterExpression = "supeName=" + "'" + strSupevi1Name + "'";
            DataRow[] rows = m_SuspeviDataTable.Select(filterExpression);
            string strSupevi1Code = rows[0].ItemArray[2].ToString();
            txtSupeCode1.Text = strSupevi1Code;
        }

        private void loadSupevi2CodeTxtBox()
        {
            string strSupevi2Name = cbSupevName2.GetItemText(cbSupevName2.SelectedItem);
            string filterExpression = "supeName=" + "'" + strSupevi2Name + "'";
            DataRow[] rows = m_SuspeviDataTable.Select(filterExpression);
            string strSupevi2Code = rows[0].ItemArray[2].ToString();
            txtSupeCode2.Text = strSupevi2Code;
        }

        private void NewSessionForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void cbInspectorName_SelectionChangeCommitted(object sender, EventArgs e)
        {
            loadInspecCodeTxtBox();
        }

        private void cbSuspectName_SelectionChangeCommitted(object sender, EventArgs e)
        {
            loadSuspecCodeTxtBox();
        }

        private void cbSupevName1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            loadSupevi1CodeTxtBox();
        }

        private void cbSupevName2_SelectionChangeCommitted(object sender, EventArgs e)
        {
            loadSupevi2CodeTxtBox();
        }


    }
}
