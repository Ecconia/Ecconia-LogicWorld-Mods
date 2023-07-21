using System;
using System.Collections.Generic;
using System.Linq;

namespace RadioConnection.Server
{
	public static class ChannelManager
	{
		private static readonly Dictionary<uint, Channel> channels = new Dictionary<uint, Channel>();
		private static DateTime last;
		
		public static void linkChannel(RadioCapable component, uint channelIndex)
		{
			if(!channels.TryGetValue(channelIndex, out Channel channel))
			{
				channel = new Channel(channelIndex);
				channels.Add(channelIndex, channel);
			}
			channel.register(component);
			removeEmptyChannels(); //Cleanup old channels.
		}
		
		public static void unlink(RadioCapable component, uint channel)
		{
			removeEmptyChannels(); //Cleanup old channels.
			channels[channel].unregister(component);
		}
		
		/*
		 * Channels won't be deleted immediately in case that they will be used shortly after again.
		 *  This optimizes performance, as object drop/creation is reduced.
		 * Channels get deleted on the first action after 10 seconds of being unused.
		 */
		private static void removeEmptyChannels()
		{
			//Check if a cleanup should be performed:
			var now = DateTime.UtcNow;
			if((now - last).Seconds < 10)
			{
				return; //10 seconds since the last purge did not pass yet.
			}
			last = now;
			
			//Perform cleanup:
			var channelsToRemove = channels.Where(entry => {
				var timeWhenEmpty = entry.Value.getEmptyTime();
				if(!timeWhenEmpty.HasValue)
				{
					return false; //Channel is used/linked!
				}
				return (now - timeWhenEmpty.Value).Seconds > 10;
			});
			//Actually remove the channels:
			foreach(var entry in channelsToRemove)
			{
				channels.Remove(entry.Key);
				entry.Value.destroy();
			}
		}
	}
}
