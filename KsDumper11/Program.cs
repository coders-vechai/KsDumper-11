﻿using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using KsDumper11.Driver;

namespace KsDumper11
{
	// Token: 0x02000005 RID: 5
	internal static class Program
	{
		// Token: 0x0600004B RID: 75 RVA: 0x000042AF File Offset: 0x000024AF
		[STAThread]
		private static void Main()
		{
			Program.StartDriver();
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Dumper());
		}

		// Token: 0x0600004C RID: 76 RVA: 0x000042D0 File Offset: 0x000024D0
		private static void StartDriver()
		{
			string logPath = Environment.CurrentDirectory + "\\driverLoading.log";
			bool flag = !File.Exists(logPath);
			FileStream outputStream;
			if (flag)
			{
				outputStream = File.Create(logPath);
			}
			else
			{
				outputStream = File.OpenWrite(logPath);
			}
			StreamWriter wr = new StreamWriter(outputStream);
			bool flag2 = !new DriverInterface("\\\\.\\KsDumper").HasValidHandle();
			if (flag2)
			{
				ProcessStartInfo inf = new ProcessStartInfo(Environment.CurrentDirectory + "\\Driver\\kdu.exe", " -prv 1 -map .\\Driver\\KsDumperDriver.sys")
				{
					CreateNoWindow = true,
					UseShellExecute = false,
					RedirectStandardOutput = true,
					RedirectStandardError = true
				};
				Process proc = Process.Start(inf);
				proc.OutputDataReceived += delegate(object sender, DataReceivedEventArgs e)
				{
					bool flag4 = !string.IsNullOrEmpty(e.Data);
					if (flag4)
					{
						wr.WriteLine(e.Data);
					}
				};
				proc.ErrorDataReceived += delegate(object sender, DataReceivedEventArgs e)
				{
					bool flag4 = !string.IsNullOrEmpty(e.Data);
					if (flag4)
					{
						wr.WriteLine(e.Data);
					}
				};
				proc.WaitForExit();
				wr.Flush();
				wr.Close();
				outputStream.Close();
				outputStream.Dispose();
				bool flag3 = !new DriverInterface("\\\\.\\KsDumper").HasValidHandle();
				if (flag3)
				{
					MessageBox.Show("Error! Tried to start driver, and it failed to start!");
					Environment.Exit(0);
				}
			}
		}
	}
}
