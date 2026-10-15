using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace LangStudy {
	internal class ProcessHandling {
		public ImmutableArray<string> BlacklistProcesses { get; }
		
		public ProcessHandling()
		{
			var procList	= Process.GetProcesses();
			if (procList != null && procList.Length != 0)
			{
				HashSet<string> processes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

				foreach (var proc in procList)
				{
					try
					{
						var session = Process.GetCurrentProcess().SessionId;
						var winPath = Environment.GetFolderPath(Environment.SpecialFolder.Windows) + Path.DirectorySeparatorChar;

						if (proc.Id == Environment.ProcessId)
							continue; // do not add this session to the hash set

						// TODO: fix this by requesting kernel32 to give proc handle
						// throws on elevated processes
						if (proc.MainModule == null || proc.SessionId == 0 || proc.MainModule.FileName.StartsWith(winPath))
							continue;

						if (proc.SessionId >= session && proc.MainWindowHandle > 0)
							processes.Add(proc.ProcessName);
					} catch (Exception)
					{
						// an attempt to disregard elevated processes; not working, Win32Exception activates before try-catch
					} finally
					{
						proc.Dispose();
					}
				}

				BlacklistProcesses = processes.ToImmutableArray<string>();
			}
		}
		public ProcessHandling(string[] blacklist) => BlacklistProcesses = blacklist.ToImmutableArray<string>();

		public static void ListProcesses()
		{
			var procList = Process.GetProcesses();
			foreach (var proc in procList)
			{
				try
				{
					Debug.WriteLine($"{proc.ProcessName}, {proc.MachineName}, {proc.Id}, {proc.PriorityClass}, {proc.SessionId}, {proc.MainWindowHandle}");
				} catch (Exception e)
				{
					continue;
				}
			}
		}

		public static bool KillProcesses() { return true; }

		/// <summary>
		/// Used to spawn Anki
		/// </summary>
		/// <returns></returns>
		public static bool SpawnProcess() { return true; }

		/// <summary>
		/// DEMO FOR NOW.
		/// </summary>
		void KillerScript()
		{
			int count = 0;
			string[] blacklistKeywords = { "Riot", "League", "notepad" };

			// span a bunch of notepad processes
			var num = new Random();
			int myNum = (int)(num.NextInt64() % 10 + 5);

			var notepadApp = new ProcessStartInfo();
			notepadApp.FileName = "Notepad";

			for (int i = 0; i < myNum; ++i)
			{
				Process.Start(notepadApp);
			}

			// close the blacklisted processes
			while (true)
			{
				foreach (var keyword in blacklistKeywords)
				{
					var procList = Process.GetProcessesByName(keyword);
					if (procList != null || procList.Length != 0)
					{
						foreach (var proc in procList)
						{
							Debug.WriteLine("You are not allowed to have this open! {0}: {1}", proc.ProcessName, proc.Id);
							proc.Kill();
							count++;
						}
					}
				}
			}

			Debug.WriteLine("There have been a total of {0} processes that were killed.", count);
		}
	}
}
