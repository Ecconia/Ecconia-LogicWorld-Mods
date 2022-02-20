//Framework imports:
//Needed for 'ClientMod':
using LogicAPI.Client;
//Needed for 'LConsole' (command feedback) and 'Command':
using LICC;
//Needed for 'Logger':
using LogicLog;

//Needed for reflection:
using System.Reflection;

//Custom imports:
//Needed for 'SceneAndNetworkManager', 'NetworkState':
using LogicWorld;
//Needed for 'Instances':
using LogicWorld.Interfaces;
//Needed for 'IntegratedServer':
using LogicWorld.Networking;

namespace SimulationControl
{
	public class SimulationControl : ClientMod
	{
		private static readonly CColor failureColor = CColor.Red;
		private static PropertyInfo internalServerProperty;

		//Entry point of this mod
		protected override void Initialize()
		{
			var type = typeof(SceneAndNetworkManager);
			PropertyInfo property = type.GetProperty("ActiveIntegratedServer", BindingFlags.Static | BindingFlags.NonPublic);
			if(property == null)
			{
				Logger.Error("Could not get the integrated server property, needed to send commands to it.");
				return;
			}
			internalServerProperty = property;
		}

		private static bool isPrimed()
		{
			return internalServerProperty != null;
		}

		private static bool isConnected()
		{
			return SceneAndNetworkManager.State == NetworkState.ConnectedWithWorldFullyLoaded;
		}

		private static bool hasIntegratedServer()
		{
			var obj = internalServerProperty.GetValue(null);
			return obj != null;
		}

		private static IntegratedServer getIntegratedServer()
		{
			return (IntegratedServer) internalServerProperty.GetValue(null);
		}

		private static void sendCommand(string command)
		{
			if(hasIntegratedServer())
			{
				//We are on an integrated server.
				getIntegratedServer().RunCommand(command, false);
			}
			else
			{
				//Connected to a remote server.
				Instances.SendData.RunCommandOnServer(command);
			}
		}

		//Command of this mod, in charge of toggling, if this mod should be active or not
		[Command("tps", Description = "Allows you to easily change the current ticks per second. Usage: tps < 0/Stop/H/Halt/P/Pause | R/Resume/Play/C | S/Step | <Floating Point Number> >")]
		public static void tps(string argument)
		{
			//Parsing code:
			string command;
			argument = argument.ToLower();
			if(argument.Equals("stop") || argument.Equals("h") || argument.Equals("halt") || argument.Equals("p") || argument.Equals("pause"))
			{
				command = "simulation.pause";
			}
			else if(argument.Equals("s") || argument.Equals("step"))
			{
				command = "step 1";
			}
			else if(argument.Equals("r") || argument.Equals("resume") || argument.Equals("play") || argument.Equals("c"))
			{
				command = "simulation.resume";
			}
			else if(double.TryParse(argument, out var targetTPS))
			{
				if(targetTPS == 0)
				{
					command = "simulation.pause";
				}
				else if(!double.IsFinite(targetTPS))
				{
					LConsole.WriteLine("Only provide finite speeds (not NaN or infinite).", failureColor);
					return;
				}
				else if(targetTPS < 0)
				{
					LConsole.WriteLine("Only provide positive speeds.", failureColor);
					return;
				}
				else
				{
					command = "simulation.rate " + targetTPS;
				}
			}
			else
			{
				LConsole.WriteLine("Could not parse your command. Use 'tps' to see usage.", failureColor);
				return;
			}
			
			//Actual sending code:
			if(!isPrimed())
			{
				LConsole.WriteLine("Something went wrong with initializing this mod, please complain to the developer, or use a supported LogicWorld version.", failureColor);
				return;
			}
			//Okay, we have this mod ready for usage!
			if(!isConnected())
			{
				LConsole.WriteLine("You are currently not (fully) connected to a server, please join a server or wait until the joining has finished.", failureColor);
				return;
			}
			//Okay, we are connected to a server!
			sendCommand(command);
		}
	}
}
