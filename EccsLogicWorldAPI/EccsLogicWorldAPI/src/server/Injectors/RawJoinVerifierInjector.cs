using System;
using System.Collections.Generic;
using System.Reflection;
using EccsLogicWorldAPI.Shared.AccessHelper;
using LICC;
using LogicAPI.Server.Networking.ClientVerification;
using LogicWorld.Server;

namespace EccsLogicWorldAPI.Server.Injectors
{
	public static class RawJoinVerifierInjector
	{
		private static readonly INetworkManager instance;
		private static readonly FieldInfo field;
		private static readonly Func<INetworkManager, IEnumerable<IClientVerifier>> verifiersGetter;
		
		static RawJoinVerifierInjector()
		{
			instance = ServiceGetter.getService<INetworkManager>();
			field = Fields.getPrivate(typeof(NetworkManager), "ClientVerifiers");
			verifiersGetter = Delegator.createFieldGetter<INetworkManager, IEnumerable<IClientVerifier>>(field);
		}
		
		public static IEnumerable<IClientVerifier> getVerifiers()
		{
			return verifiersGetter(instance);
		}
		
		public static void setVerifiers(IEnumerable<IClientVerifier> newVerifiers)
		{
			//TODO: Find a better way to write 'readonly' fields, in a performant way.
			field.SetValue(instance, newVerifiers);
		}
		
		public static void replaceVerifier(Type verifierToReplace, IClientVerifier replacementVerifier)
		{
			var oldVerifiers = getVerifiers();
			if(oldVerifiers is List<IClientVerifier> oldVerifierList)
			{
				LConsole.WriteLine("Verifiers as list (replace)!");
				for(int i = 0; i < oldVerifierList.Count; i++)
				{
					if(oldVerifierList[i].GetType() == verifierToReplace)
					{
						oldVerifierList[i] = replacementVerifier;
						return;
					}
				}
				throw new Exception("Was not able to inject new replacement join verifier, as the original of type '" + verifierToReplace.FullName + "' was not found.");
			}
			//Fallback, if not a list:
			bool notInjected = true;
			var newVerifiers = new List<IClientVerifier>();
			foreach(var oldVerifier in oldVerifiers)
			{
				if(oldVerifier.GetType() == verifierToReplace)
				{
					newVerifiers.Add(replacementVerifier);
					notInjected = false;
				}
				else
				{
					newVerifiers.Add(oldVerifier);
				}
			}
			if(notInjected)
			{
				throw new Exception("Was not able to inject new replacement join verifier, as the original of type '" + verifierToReplace.FullName + "' was not found.");
			}
			setVerifiers(newVerifiers);
		}
		
		public static void addVerifier(IClientVerifier newVerifier)
		{
			var oldVerifiers = getVerifiers();
			if(oldVerifiers is List<IClientVerifier> oldVerifierList)
			{
				LConsole.WriteLine("Verifiers as list (add)!");
				oldVerifierList.Add(newVerifier);
				return;
			}
			//Fallback if not a list:
			var newVerifiers = new List<IClientVerifier>(getVerifiers());
			newVerifiers.Add(newVerifier);
			setVerifiers(newVerifiers);
		}
	}
}
