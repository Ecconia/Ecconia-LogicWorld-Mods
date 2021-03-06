using System;
using JimmysUnityUtilities;
using LogicAPI.Server.Managers;
using LogicWorld.Server;
using LogicWorld.Server.HostServices;

namespace CustomChatManager.Server.Commands
{
	public class CommandTPS : ICommand
	{
		private static double minTPS = 0.1f;

		private static string usage = "Usage: /tps [ <tps> | stop/halt/pause | resume/play/continue ]";
		
		private readonly PeriodicTicker simulationScheduler;
		private readonly ILogicManager simulation;
		
		public string name => "TPS";
		public string shortDescription => "Changes the processed ticks per second.";

		public CommandTPS()
		{
			simulationScheduler = (PeriodicTicker) Program.Get<ISimulationService>(); //Just do the dirty casting, we need 'IsPaused'.
			if(simulationScheduler == null)
			{
				throw new Exception("Could not get simulation scheduling service. /" + name + " will break.");
			}
			simulation = Program.Get<ILogicManager>();
			if(simulation == null)
			{
				throw new Exception("Could not get simulation service. /" + name + " will break. And the server is probably already broken...");
			}
		}
		
		public void execute(CommandSender sender, string arguments)
		{
			if(arguments.IsNullOrWhiteSpace())
			{
				//Print help:
				if(simulationScheduler.IsPaused)
				{
					sender.sendMessage("Current tps is " + ChatColors.highlight + simulationScheduler.TicksPerSecond + ChatColors.close + " + paused. " + ChatColors.background + "<i>Try '/tps help' for usage.</i>" + ChatColors.close);
				}
				else
				{
					sender.sendMessage("Current tps is " + ChatColors.highlight + simulationScheduler.TicksPerSecond + ChatColors.close + ". " + ChatColors.background + "<i>Try '/tps help' for usage.</i>" + ChatColors.close);
				}
				return;
			}
			
			//We got some argument:
			arguments = arguments.TrimStart();
			if(arguments.IndexOf(' ') >= 0)
			{
				sender.sendMessage(ChatColors.failure + "Too many arguments." + ChatColors.close + "\n" + ChatColors.background + usage + ChatColors.close);
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
					sender.sendMessage(ChatColors.failure + "Only provide finite speeds (not NaN or infinite)." + ChatColors.close);
				}
				else if(targetTPS < 0)
				{
					sender.sendMessage(ChatColors.failure + "Only provide positive speeds." + ChatColors.close);
				}
				else if(targetTPS < minTPS)
				{
					sender.sendMessage(ChatColors.failure + "The minimum allowed tps is " + minTPS + "." + ChatColors.close);
				}
				else
				{
					setSpeed(sender, targetTPS);
				}
			}
			else
			{
				sender.sendMessage(ChatColors.failure + "Could not parse your command." + ChatColors.close + "\n" + usage);
			}
		}

		private void stepSimulation(CommandSender sender)
		{
			if(!simulationScheduler.IsPaused)
			{
				sender.sendMessage(ChatColors.failure + "To step the simulation, paused it first." + ChatColors.close);
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
			sender.broadcast(ChatColors.background + "<i>" + sender.getPlayerName() + " resumed simulation.</i>" + ChatColors.close);
		}

		private void pauseSimulation(CommandSender sender)
		{
			if(simulationScheduler.IsPaused)
			{
				sender.sendMessage("Simulation is already paused.");
				return;
			}
			simulationScheduler.PauseTicks();
			sender.broadcast(ChatColors.background + "<i>" + sender.getPlayerName() + " paused simulation.</i>" + ChatColors.close);
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
					sender.broadcast(ChatColors.background + "<i>" + sender.getPlayerName() + " resumed simulation with " + ChatColors.highlight + tps + ChatColors.close + " tps.</i>" + ChatColors.close);
				}
				else
				{
					sender.broadcast(ChatColors.background + "<i>" + sender.getPlayerName() + " changed simulation to " + ChatColors.highlight + tps + ChatColors.close + " tps.</i>" + ChatColors.close);
				}
			}
		}
	}
}
