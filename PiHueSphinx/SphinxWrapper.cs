using System;
using System.Runtime.InteropServices;

namespace PiHueSphinx
{
	public static class SphinxWrapper
	{
		const string LibName = "sphinx_hue_detection";

		[DllImport (LibName, EntryPoint = "init_sphinx")]
		public static extern IntPtr InitSphinx (string configFile, string hotword);

		[DllImport (LibName, EntryPoint = "free_sphinx")]
		public static extern void FreeSphinx (IntPtr ps, IntPtr audioDevice);

		[DllImport (LibName, EntryPoint = "open_recording_device")]
		public static extern IntPtr OpenRecordingDevice (IntPtr ps, string deviceName);

		[DllImport (LibName, EntryPoint = "wait_for_hotword")]
		public static extern bool WaitForHotword (IntPtr ps, IntPtr audioDevice);

		[DllImport (LibName, EntryPoint = "wait_for_command")]
		public static extern string WaitForCommand (IntPtr ps, IntPtr audioDevice);
	}
}

