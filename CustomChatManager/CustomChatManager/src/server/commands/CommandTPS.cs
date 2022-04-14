using System;
using JimmysUnityUtilities;
using LogicAPI.Server.Managers;
using LogicWorld.Server;
using LogicWorld.Server.HostServices;

namespace CustomChatManager.Server.Commands
{
	public class CommandTPS : ICommand
	{
		private static readonly Color24 failureColor = new Color24(255, 100, 100);
		private static double minTPS = 0.1f;

		private static string usage = "Usage: /tps [ <tps> | stop/halt/pause | resume/play/continue ]";
		
		private readonly PeriodicTicker simulationScheduler;
		private readonly ILogicManager simulation;
		
		public string name => "tps";
		public string shortDescription => "Changes the processed ticks per second.";

		public CommandTPS()
		{
			simulationScheduler = (PeriodicTicker) Program.Get<ISimulationService>(); //Just do the dirty casting, we need 'IsPaused'.
			if(simulationScheduler == null)
			{
				throw new Exception("Could not get simulation scheduling service. /tps will break.");
			}
			simulation = Program.Get<ILogicManager>();
			if(simulation == null)
			{
				throw new Exception("Could not get simulation service. /tps will break. And the server is probably already broken...");
			}
		}
		
		public void execute(CommandSender sender, string arguments)
		{
			if(arguments.IsNullOrWhiteSpace())
			{
				//Print help:
				if(simulationScheduler.IsPaused)
				{
					sender.sendMessage("Current tps is <color=#fa0>" + simulationScheduler.TicksPerSecond + "</color> + paused. <color=#ccc><i>Use '/tps help' for help.</i></color>");
				}
				else
				{
					sender.sendMessage("Current tps is <color=#fa0>" + simulationScheduler.TicksPerSecond + "</color>. <color=#ccc><i>Use '/tps help' for help.</i></color>");
				}
				return;
			}
			
			//We got some argument:
			arguments = arguments.TrimStart();
			if(arguments.IndexOf(' ') >= 0)
			{
				sender.sendMessage("<color=" + failureColor + ">Too many arguments.</color>\n" + usage);
				return;
			}
			
			//Only one argument remaining:
			string argument = arguments.ToLower();

			if(argument.Equals("?") || argument.Equals("help"))
			{
				sender.sendMessage(usage);
			}
			else if(argument.Equals("stop") || argument.Equals("h") || argument.Equals("halt") || argument.Equals("p") || argument.Equals("pause"))
			{
				pauseSimulation(sender);
			}
			//TODO: Stepping support (without spamming the chat).
			else if(argument.Equals("r") || argument.Equals("resume") || argument.Equals("play") || argument.Equals("c") || argument.Equals("continue"))
			{
				resumeSimulation(sender);
			}
			else if(argument.Equals("s") || argument.Equals("step"))
			{
				stepSimulation(sender);
			}
			else if(double.TryParse(argument, out var targetTPS))
			{
				if(targetTPS == 0)
				{
					pauseSimulation(sender);
				}
				else if(!double.IsFinite(targetTPS))
				{
					sender.sendMessage("<color=" + failureColor + ">Only provide finite speeds (not NaN or infinite).</color>");
				}
				else if(targetTPS < 0)
				{
					sender.sendMessage("<color=" + failureColor + ">Only provide positive speeds.</color>");
				}
				else if(targetTPS < minTPS)
				{
					sender.sendMessage("<color=" + failureColor + ">The minimum allowed tps is " + minTPS + ".</color>");
				}
				else
				{
					setSpeed(sender, targetTPS);
				}
			}
			else
			{
				sender.sendMessage("<color=" + failureColor + ">Could not parse your command.</color>\n" + usage);
			}
		}

		private void stepSimulation(CommandSender sender)
		{
			if(!simulationScheduler.IsPaused)
			{
				sender.sendMessage("<color=" + failureColor + ">To step the simulation, paused it first.</color>");
				return;
			}
			simulation.DoLogicUpdate();
			sender.broadcastConsoleMessage(sender.getPlayerName() + " stepped simulation.");
		}

		private void resumeSimulation(CommandSender sender)
		{
			if(!simulationScheduler.IsPaused)
			{
				sender.sendMessage("Simulation is already running.");
				return;
			}
			simulationScheduler.StartTicks();
			sender.broadcast("<i>" + sender.getPlayerName() + " resumed simulation.</i>");
		}

		private void pauseSimulation(CommandSender sender)
		{
			if(simulationScheduler.IsPaused)
			{
				sender.sendMessage("Simulation is already paused.");
				return;
			}
			simulationScheduler.PauseTicks();
			sender.broadcast("<i>" + sender.getPlayerName() + " paused simulation.</i>");
		}

		private void setSpeed(CommandSender sender, double tps)
		{
			bool paused = simulationScheduler.IsPaused;
			if(!paused && Math.Abs(simulationScheduler.TicksPerSecond - tps) < 0.0000001)
			{
				sender.sendMessage("This is already the current tick speed.");
			}
			else
			{
				simulationScheduler.TicksPerSecond = tps;
				if(paused)
				{
					sender.broadcast("<i>" + sender.getPlayerName() + " resumed simulation with <color=#fa0>" + tps + "</color> tps.</i>");
				}
				else
				{
					sender.broadcast("<i>" + sender.getPlayerName() + " changed simulation to <color=#fa0>" + tps + "</color> tps.</i>");
				}
			}
		}
	}
}
