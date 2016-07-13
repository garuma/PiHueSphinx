using System;
using System.Runtime.InteropServices;

namespace PiHueSphinx
{
	public class PiHueSphinx
	{
		const int CommandTry = 3;

		public static void Main (string [] args)
		{
			var ps = SphinxWrapper.InitSphinx (args [0], args [1]);
			var ad = SphinxWrapper.OpenRecordingDevice (ps, args [2]);

			Process (ps, ad, args.Length > 3 ? args[3] : null);

			Console.ReadLine ();
			Console.WriteLine ("Exiting now");
			SphinxWrapper.FreeSphinx (ps, ad);
		}

		static async void Process (IntPtr ps, IntPtr ad, string appKey)
		{
			var hue = await HueController.GetControllerAsync (appKey);
			Console.WriteLine ("Got the hue controller");

			while (true) {
				if (!SphinxWrapper.WaitForHotword (ps, ad))
					continue;
				Console.WriteLine ("Got hotword");
				await hue.FlashForHotword ();

				for (int i = 0; i < CommandTry; i++) {
					Console.WriteLine ("Waiting for command");
					var command = SphinxWrapper.WaitForCommand (ps, ad);
					if (command == null)
						continue;
					Console.WriteLine ("Got command: {0}", command);

					switch (command.ToLowerInvariant ()) {
					case "shutdown all lights":
						Console.WriteLine ("Shutting down all lights");
						await hue.ShutdownAllLights ();
						break;
					case "open all lights":
						Console.WriteLine ("Opening all lights");
						await hue.OpenAllLights ();
						break;
					case "shutdown bedroom":
						Console.WriteLine ("Shutting down bedroom");
						await hue.ShutdownRoom (Room.Bedroom);
						break;
					case "open bedroom":
						Console.WriteLine ("Opening bedroom");
						await hue.OpenRoom (Room.Bedroom);
						break;
					case "shutdown living room":
						Console.WriteLine ("Shutting down bedroom");
						await hue.ShutdownRoom (Room.LivingRoom);
						break;
					case "open living room":
						Console.WriteLine ("Opening living room");
						await hue.OpenRoom (Room.LivingRoom);
						break;
					default:
						continue;
					}
					break;
				}
			}
		}
	}
}
