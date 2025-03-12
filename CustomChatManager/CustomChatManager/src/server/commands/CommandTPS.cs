using System;
using EccsLogicWorldAPI.Server;
using EccsLogicWorldAPI.Shared.AccessHelper;
using HarmonyLib;
using JimmysUnityUtilities;
using LogicAPI.Server.Managers;
using LogicWorld.Server.HostServices;
using LogicWorld.SharedCode.ExtraData;

namespace CustomChatManager.Server.Commands
{
	public class CommandTPS : ICommand
	{
		private const double minTPS = 0.1f;
		
		private const string usage = "Usage: /tps [ <tps> | stop/halt/pause | step | resume/play/continue ]";
		
		private readonly ISimulationManager simulationScheduler; //To get if it is actually running
		private readonly ILogicManager simulation; //To step the simulation
		private static ExtraDataAccessor<bool> accessor_tps_paused; //To get and set paused (by player) state
		private static ExtraDataAccessor<double> accessor_tps_speed; //To get and set TPS (by player)
		
		public string name => "TPS";
		public string shortDescription => "Changes the processed ticks per second.";
		
		public CommandTPS()
		{
			//Used to check if simulation is running:
			simulationScheduler = ServiceGetter.getService<ISimulationManager>();
			//Used for stepping the simulation:
			simulation = ServiceGetter.getService<ILogicManager>();
			
			//Hook into the initialization of the simulation manager, as that grants us access to the accessors:
			// Point is, any point after the save has been initialized would do.
			var initializeMethod = Methods.getPublic(simulationScheduler.GetType(), "Initialize");
			var hookMethod = Methods.getPrivateStatic(GetType(), nameof(afterInitialization));
			var harmony = new Harmony("Commands: SlashTPS");
			harmony.Patch(initializeMethod, postfix: new HarmonyMethod(hookMethod));
		}
		
		private static void afterInitialization(ExtraDataAccessor<double> ___Accessor_SimulationSpeedTPS, ExtraDataAccessor<bool> ___Accessor_SimulationPaused)
		{
			//Used for controlling pause and tps:
			// As the official API for getting ExtraData accessors is not working, just extract it from the SimulationManager.
			accessor_tps_speed = ___Accessor_SimulationSpeedTPS;
			accessor_tps_paused = ___Accessor_SimulationPaused;
			//Should never be null at this point. Else Simulation won't work. Probably Harmony will complain if the game changes something.
		}
		
		public void execute(CommandSender sender, string arguments)
		{
			if(arguments.IsNullOrWhiteSpace())
			{
				//Print help:
				if(!isRunning())
				{
					sender.sendMessage("Current tps is " + ChatColors.highlight + getSpeed() + ChatColors.close + " + paused. " + ChatColors.background + "<i>Try '/tps help' for usage.</i>" + ChatColors.close);
				}
				else
				{
					sender.sendMessage("Current tps is " + ChatColors.highlight + getSpeed() + ChatColors.close + ". " + ChatColors.background + "<i>Try '/tps help' for usage.</i>" + ChatColors.close);
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
			if(isRunning())
			{
				sender.sendMessage(ChatColors.failure + "To step the simulation, paused it first." + ChatColors.close);
				return;
			}
			simulation.DoLogicUpdate(); //Cause the stepping in the Manager is ofc private, just do it like it does.
			sender.broadcastConsoleMessage(sender.getPlayerName() + " stepped simulation.");
		}
		
		private void resumeSimulation(CommandSender sender)
		{
			if(!isPaused()) //The simulation might be stopped by the server, although then this likely won't be called.
			{
				sender.sendMessage("Simulation is not paused.");
				return;
			}
			resume(); //Might not resume it though... (If something goes wrong).
			sender.broadcast(ChatColors.background + "<i>" + sender.getPlayerName() + " resumed simulation.</i>" + ChatColors.close);
		}
		
		private void pauseSimulation(CommandSender sender)
		{
			if(isPaused()) //Might be disabled by the server, so query this.
			{
				sender.sendMessage("Simulation is already paused.");
				return;
			}
			pause();
			sender.broadcast(ChatColors.background + "<i>" + sender.getPlayerName() + " paused simulation.</i>" + ChatColors.close);
		}
		
		private void setSpeed(CommandSender sender, double tps)
		{
			var paused = isPaused();
			if(!paused && Math.Abs(accessor_tps_speed.Data - tps) < 0.0000001)
			{
				sender.sendMessage("This is already the current tick speed.");
			}
			else
			{
				accessor_tps_speed.SetData(tps);
				if(paused)
				{
					resume(); //Has to be manually called now.
					sender.broadcast(ChatColors.background + "<i>" + sender.getPlayerName() + " resumed simulation with " + ChatColors.highlight + tps + ChatColors.close + " tps.</i>" + ChatColors.close);
				}
				else
				{
					sender.broadcast(ChatColors.background + "<i>" + sender.getPlayerName() + " changed simulation to " + ChatColors.highlight + tps + ChatColors.close + " tps.</i>" + ChatColors.close);
				}
			}
		}
		
		private double getSpeed()
		{
			return accessor_tps_speed.Data;
		}
		
		private bool isRunning()
		{
			return simulationScheduler.SimulationIsRunning;
		}
		
		private bool isPaused()
		{
			return accessor_tps_paused.Data;
		}
		
		private void pause()
		{
			accessor_tps_paused.SetData(true);
		}
		
		private void resume()
		{
			accessor_tps_paused.SetData(false);
		}
	}
}
