using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace RealTimeGraph
{
	public class FILE_INFO_TYPE
	{
		public string filepath;
		public string filename;
		public string filetype;
	}

	public static class File_save
	{
		public static FILE_INFO_TYPE file_tcp_recv = new FILE_INFO_TYPE();
		public static FILE_INFO_TYPE file_cali_recv = new FILE_INFO_TYPE();
		public static FILE_INFO_TYPE file_log_recv = new FILE_INFO_TYPE();
		public static bool Thread_log_run = true;
		public static Queue<string> Que_tcp_recv = new Queue<string>();
		public static bool FileSaveInit_flag = false;


		static Thread Tparsing = new Thread(new ThreadStart(Thread_Log));

		public static void FileSave_name(ref FILE_INFO_TYPE file, string filename)
		{
			DateTime fileIndex = System.DateTime.Now;
			string filenameAttach = null;
			filenameAttach = fileIndex.ToString("yyyyMMdd_HHmmss");
			file.filepath = Application.StartupPath + "\\YBADS_Files";
			file.filename = file.filepath + "\\" + filename + "_" + filenameAttach + ".csv";
		}

		public static void LogFileSave_name(ref FILE_INFO_TYPE file, string filename)
		{
			DateTime fileIndex = System.DateTime.Now;
			string filenameAttach = null;
			filenameAttach = fileIndex.ToString("yyyyMMdd_HHmm");
			file.filepath = Application.StartupPath + "\\YBADS_LOG_Files";
			file.filename = file.filepath + "\\" + filename + "_" + filenameAttach + ".csv";
		}


		public static void file_save_init()
		{
			Tparsing.Start();
			Tparsing.IsBackground = true;
			FileSaveInit_flag = true;
		}

		public static void FDR_File_name()
		{
			File_save.FileSave_name(ref file_tcp_recv, "Data_RECV");
		}



		public static void Log_File_save(string stringbuilder)
		{
			File_save.LogFileSave_name(ref file_log_recv, "Log_Data");

			FILE_INFO_TYPE f1 = file_log_recv;

			FileInfo file = new FileInfo(f1.filename);
			string title = "";
			if (file.Exists == false)
			{
				title = file_log_recv_title();
				System.IO.File.AppendAllText(f1.filename, title, System.Text.Encoding.UTF8);
			}
			Application.DoEvents();
			System.IO.File.AppendAllText(f1.filename, stringbuilder.ToString(), System.Text.Encoding.UTF8);
		}

        private static string file_log_recv_title()
        {
			string str = "";

			str += "TIME" + ",";
			str += "1 Fatal Code" + ",";
			str += "2 Fatal Code" + ",";
			str += "3 Fatal Code" + ",";
			str += "4 Fatal Code" + ",";
			str += "5 Fatal Code" + ",";
			str += "6 Fatal Code" + ",";
			str += "7 Fatal Code" + ",";
			str += "8 Fatal Code" + ",";
			str += "1 Pressure" + ",";
			str += "2 Pressure" + ",";
			str += "3 Pressure" + ",";
			str += "4 Pressure" + ",";
			str += "5 Pressure" + ",";
			str += "6 Pressure" + ",";
			str += "7 Pressure" + ",";
			str += "8 Pressure" + ",";
			str += "PS raw" + ",";
			str += "PT raw" + ",";
			str += "Altitude" + ",";
			str += "Velocity" + ",";
			str += "AOA raw" + ",";
			str += "AOS raw" + ",";

			str += Environment.NewLine;
			return str;
		}

        public static void file_save_recv(string str)
		{
			try
			{
				if (Que_tcp_recv == null)
				{
					return;
				}


				Que_tcp_recv.Enqueue(str);

				//FDR_FileWrite(str);
			}
			catch (Exception)
			{
				return;
			}
		}

		public static string file_tcp_recv_title()
		{
			string str = "";

			str += "Label" + ",";
			str += "Data1" + ",";
			str += "Altitude" + ",";
            str += "Velocity" + ",";
            str += "Vertical Velocity" + ","; 
			str += "PT Pressure" + ",";
			str += "PS Pressure" + ",";
			str += "PT Temperature" + ",";
			str += "PS Temperature" + ",";
            str += "Sensor Status" + ",";
            str += "END" + ",";

			str += Environment.NewLine;
			return str;
		}



		public static void Thread_Log()
		{

			while (Thread_log_run)
			{
				try
				{
					if (Que_tcp_recv.Count > 0)
					{

						int repeats = Que_tcp_recv.Count;

						//반복 추가
						StringBuilder stringbuilder = new StringBuilder();
						for (int i = 0; i < repeats; i++)
						{
							string value = Que_tcp_recv.Dequeue();
							stringbuilder.Append(value);
						}

						FILE_INFO_TYPE f1 = file_tcp_recv;

						string sDirPath;
						sDirPath = f1.filepath;
						DirectoryInfo di = new DirectoryInfo(sDirPath);
						if (di.Exists == false)
						{
							di.Create();
						}
						FileInfo file = new FileInfo(f1.filename);
						string title = "";
						if (file.Exists == false)
						{
							// 최초 파일 생성시 label;;
							title = file_tcp_recv_title();
							System.IO.File.AppendAllText(f1.filename, title, System.Text.Encoding.UTF8);
						}
						Application.DoEvents();
						System.IO.File.AppendAllText(f1.filename, stringbuilder.ToString(), System.Text.Encoding.UTF8);

					}
					Thread.Sleep(10);
				}
				catch (Exception ex)
				{
					MessageBox.Show("에러\n" + ex.ToString());
					return;
				}
				Thread.Sleep(500);
			}
		}
	}
}
