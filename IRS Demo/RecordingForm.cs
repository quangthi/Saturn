﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using MjpegProcessor;

using Emgu;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using System.Reflection;
using System.IO;
using System.Threading;


//using IRS_Demo;

namespace IRS_Demo
{
    public partial class RecordingForm : Form
    {
        private NewSessionForm _newSessionForm;
        private Vlc.DotNet.Forms.VlcControl vlcRecorder;
        // class attribute
       // MjpegDecoder m_mjpeg;
        //Capture m_capture;
        bool m_bSession;
        Int32 nRecTimeInSecond;
        
        
        
        //Image<Bgr, Byte> m_frame;
        //string vlcLibPath = Path.Combine(new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName,"libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64");
        public RecordingForm(NewSessionForm newSessionForm)
        {
            InitializeComponent();            
            //this.vlcControl.VlcLibDirectory = new DirectoryInfo(vlcLibPath); 
            /////////////////
            _newSessionForm = newSessionForm;

            this.StartPosition = FormStartPosition.CenterScreen;

            m_bSession = true;

            btnStartRec.Enabled = true;
            btnStopRec.Enabled = false;

            //this.vlcPlayer.VlcLibDirectoryNeeded += vlcPlayer_VlcLibDirectoryNeeded;

            CommonParam.LoadConfig();

            
            vlcRecorder = new Vlc.DotNet.Forms.VlcControl();
            ((System.ComponentModel.ISupportInitialize)(this.vlcRecorder)).BeginInit();
            this.vlcRecorder.Name = "vlcRecorder";
            this.vlcRecorder.Size = new System.Drawing.Size(813, 618);
            this.vlcRecorder.Spu = -1;
            this.vlcRecorder.TabIndex = 0;
            this.vlcRecorder.Text = "vlcRecorder1";
            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            vlcRecorder.VlcLibDirectory = new DirectoryInfo(Path.Combine(currentDirectory, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));
            ((System.ComponentModel.ISupportInitialize)(this.vlcRecorder)).EndInit();
            //itnit vlc player
            vlcPlayer.SetMedia(CommonParam.mConfig.videoUrl );
            //label42.Parent = vlcPlayer;            
            label42.BackColor = Color.Transparent;
            vlcPlayer.Play();
            /*
            if (tabControl1.SelectedIndex == 0)
            {
                m_mjpeg = new MjpegDecoder();
                m_mjpeg.imageSizeBytes = 1024 * 1024;
                m_mjpeg.FrameReady += mjpeg_FrameReady;
                m_mjpeg.ParseStream(new Uri(CommonParam.MjpegUrl));
            }

            if (tabControl1.SelectedIndex == 1)
            {
                if (m_capture == null)
                    m_capture = new Emgu.CV.Capture("rtsp://admin:admin@192.168.1.2/live3.sdp");
                    //m_capture = new Emgu.CV.Capture("rtsp://root:root@192.168.1.218/axis-media/media.amp");

                m_capture.ImageGrabbed += m_capture_ImageGrabbed;
                m_capture.Start();
            }
            */
            label23.Text = CommonParam.mSesData.caseCode;
            label24.Text = "Phòng 1";
            label25.Text = DateTime.Now.ToString("dd/MM/yyyy h:mm tt");
            label26.Text = ""; // time ket thuc
            label27.Text = ""; // Thoi diem dien ra
            label28.Text = ""; // Ma phien
            label29.Text = ""; // Thoi gian thuc hien
            label30.Text = CommonParam.mSesData.inspectorName;
            label31.Text = CommonParam.mSesData.inspectorCode;

            textBox19.Text = CommonParam.mSesData.Notes;

        }
        private void vlcPlayer_VlcLibDirectoryNeeded(object sender, Vlc.DotNet.Forms.VlcLibDirectoryNeededEventArgs e)
        {
            var currentAssembly = Assembly.GetEntryAssembly();
            var currentDirectory = new FileInfo(currentAssembly.Location).DirectoryName;
            // Default installation path of VideoLAN.LibVLC.Windows
            e.VlcLibDirectory = new DirectoryInfo(Path.Combine(currentDirectory, "libvlc", IntPtr.Size == 4 ? "win-x86" : "win-x64"));
        }       

        private void btnStartRec_Click(object sender, EventArgs e)
        {
            vlcRecorder.SetMedia(CommonParam.mConfig.videoUrl,
                ":sout=#transcode{vcodec=theo,vb=1000,scale=1,acodec=flac,ab=128,channels=2,samplerate=44100}:std{access=file,mux=ogg,dst="
                + CommonParam.ProgramPath + CommonParam.SessionFolderName + "\\video.mp4}");
            vlcRecorder.Play();


            timer1.Start();
            timer1.Interval = 1000;
            nRecTimeInSecond = 0;

            btnStartRec.Enabled = false;
            btnStopRec.Enabled = true;
            btnPause.Enabled = true;
        }


        private void btnStopRec_Click(object sender, EventArgs e)
        {
            DialogResult stopRecDialogResult = MessageBox.Show("Bạn có muốn dừng ghi lại phiên hỏi cung?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (stopRecDialogResult == DialogResult.Yes)
            {
                vlcRecorder.Stop();
                timer1.Stop();
                //btnStartRec.Enabled = true;
                btnStopRec.Enabled = false;
                btnPause.Enabled = false;
                btnExport.Enabled = true;
                btnFinish.Enabled = true;
                if(MessageBox.Show("Dữ liệu video đã được ghi tại " + CommonParam.ProgramPath + CommonParam.SessionFolderName + ". Mở thư mục ghi lưu?", 
                "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Information)
                ==DialogResult.Yes)
                    Process.Start("explorer.exe", CommonParam.ProgramPath + CommonParam.SessionFolderName);

                lblVideoPath.Text = CommonParam.ProgramPath + CommonParam.SessionFolderName;                
            }
            else if (stopRecDialogResult == DialogResult.No)
            {
                //do something else
            }
            
            
        }
                
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            
            if (_newSessionForm.findForm == null)
            {
                _newSessionForm.findForm = new FindSession(_newSessionForm);
                this.Hide();
                _newSessionForm.findForm.ShowDialog();
                this.Show();
            }
            else
            {
                this.Hide();
                _newSessionForm.findForm.ShowDialog();
                this.Show();
            }
        }

        private void RecordingForm_FormClosing(object sender, FormClosingEventArgs e)
        {            
            if (!m_bSession) return; 
            DialogResult stopRecDialogResult = MessageBox.Show("Bạn có muốn thoát khỏi giao diện phiên hỏi cung?", "Thông báo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (stopRecDialogResult == DialogResult.Yes)
            {
                vlcRecorder.Stop();
                vlcPlayer.Stop();
                //axVLCPlugin21.playlist.stop();
                //axVLCPlugin21.playlist.items.clear();
                m_bSession = false;                
            }
            else if (stopRecDialogResult == DialogResult.No)
            {
                e.Cancel = true;
            }            

        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            vlcPlayer.Stop();
            label42.Text = "00:00:00";
            btnPlay.Enabled = true;
        }

        private void btnPlay_Click(object sender, EventArgs e)
        {
            vlcPlayer.SetMedia(new Uri(CommonParam.ProgramPath + CommonParam.SessionFolderName + "\\video.mp4"));
            vlcPlayer.Play();            
        }

        private void button3_Click(object sender, EventArgs e)
        {
            vlcPlayer.Pause();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            vlcPlayer.Play();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            //itnit vlc player
            vlcPlayer.SetMedia(CommonParam.mConfig.videoUrl);
            vlcPlayer.Play();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            vlcPlayer.Stop();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    //string[] files = Directory.GetFiles(fbd.SelectedPath);
                    
                    //copy file ra folder da chon
                    string[] originalFiles = Directory.GetFiles(CommonParam.ProgramPath+CommonParam.SessionFolderName, "*", SearchOption.AllDirectories);

                    // Dealing with a string array, so let's use the actionable Array.ForEach() with a anonymous method
                    Array.ForEach(originalFiles, (originalFileLocation) =>
                    {
                        // Get the FileInfo for both of our files
                        FileInfo originalFile = new FileInfo(originalFileLocation);
                        FileInfo destFile = new FileInfo(originalFileLocation.Replace(CommonParam.ProgramPath + CommonParam.SessionFolderName, fbd.SelectedPath));
                        // ^^ We can fill the FileInfo() constructor with files that don't exist...

                        // ... because we check it here
                        if (destFile.Exists)
                        {
                            // Logic for files that exist applied here; if the original is larger, replace the updated files...
                            if (originalFile.Length > destFile.Length)
                            {
                                originalFile.CopyTo(destFile.FullName, true);
                            }
                        }
                        else // ... otherwise create any missing directories and copy the folder over
                        {
                            Directory.CreateDirectory(destFile.DirectoryName); // Does nothing on directories that already exist
                            originalFile.CopyTo(destFile.FullName, false); // Copy but don't over-write  
                        }

                    });
                }
            }
        }

         
        private void timer1_Tick(object sender, EventArgs e)
        {
            nRecTimeInSecond++;
            TimeSpan time = TimeSpan.FromSeconds(nRecTimeInSecond);
            string recordTime = time.ToString(@"hh\:mm\:ss");
            
            label42.Text = recordTime;            
            
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (btnPause.Text == "Tạm dừng")
            {
                btnPause.Text = "Tiếp tục ghi";
                timer1.Stop();                
                vlcRecorder.Pause();
            }
                
            else if (btnPause.Text == "Tiếp tục ghi")
            {
                btnPause.Text = "Tạm dừng";
                timer1.Start();
                vlcRecorder.Play();
            }
                
        }

        private void btnAddNotes_Click(object sender, EventArgs e)
        {
            CommonParam.mSesData.Notes += "\r\n";
            CommonParam.mSesData.Notes += txtAddNotes.Text;
            textBox19.Text = CommonParam.mSesData.Notes;
        }

    }
}
