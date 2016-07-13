using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Q42.HueApi;
using Q42.HueApi.Interfaces;
using Q42.HueApi.NET;

namespace PiHueSphinx
{
	public enum Room {
		LivingRoom = 1,
		Bedroom = 2
	}

	public class HueController
	{
		ILocalHueClient client;

		private HueController (ILocalHueClient client)
		{
			this.client = client;
		}

		public static async Task<HueController> GetControllerAsync (string appKey = null)
		{
			var locator = new SSDPBridgeLocator ();
			var ip = (await locator.LocateBridgesAsync (TimeSpan.FromSeconds (15))).First ();
			Console.WriteLine ("Found bridge at IP: {0}", ip);
			var client = new LocalHueClient (ip);

			if (!string.IsNullOrEmpty (appKey))
				client.Initialize (appKey);
			else {
				appKey = await client.RegisterAsync ("PiHueSphinx", "buzzerpi");
				Console.WriteLine ("Your HUE app key is: {0}", appKey);
			}

			return new HueController (client);
		}

		public async Task FlashForHotword ()
		{
			var cmd = new LightCommand () {
				Alert = Alert.Once
			};
			await client.SendCommandAsync (cmd);
		}

		public async Task ShutdownAllLights ()
		{
			var cmd = new LightCommand ().TurnOff ();
			await client.SendCommandAsync (cmd);
		}

		public async Task OpenAllLights ()
		{
			var cmd = new LightCommand ().TurnOn ();
			await client.SendCommandAsync (cmd);
		}

		public async Task ShutdownRoom (Room room)
		{
			var cmd = new LightCommand ().TurnOff ();
			await client.SendGroupCommandAsync (cmd, ((int)room).ToString ());
		}

		public async Task OpenRoom (Room room)
		{
			var cmd = new LightCommand ().TurnOn ();
			await client.SendGroupCommandAsync (cmd, ((int)room).ToString ());
		}
	}
}

