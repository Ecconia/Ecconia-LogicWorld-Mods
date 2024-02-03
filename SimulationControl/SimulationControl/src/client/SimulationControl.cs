//Framework imports:
//Needed for 'ClientMod':
using LogicAPI.Client;
//Needed for 'LConsole' (command feedback) and 'Command':
using LICC;
//Needed for 'Exception':
using System;

//Custom imports:
//Needed for 'SceneAndNetworkManager':
using LogicWorld;

namespace SimulationControl.Client
{
	public class SimulationControl : ClientMod
	{
		private static readonly CColor failureColor = CColor.Red;
		
		//Entry point of this mod
		protected override void Initialize()
		{
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
				command = "simulation.step 1";
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
			try
			{
				SceneAndNetworkManager.RunCommandOnServer(command);
			}
			catch(Exception e)
			{
				//Handle this epic command feedback.
				if(e.Message.Equals("Can't run command on server -- we're not connected to a server!"))
				{
					LConsole.WriteLine("You are currently not (fully) connected to a server, please join a server or wait until the joining has finished.", failureColor);
					return;
				}
				throw;
			}
		}
	}
}
