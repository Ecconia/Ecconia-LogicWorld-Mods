using System;
using System.Collections.Generic;
using System.Reflection;
using EccsLogicWorldAPI.Shared.AccessHelper;
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
		
		public static IClientVerifier removeVerifier(Type verifierToReplace)
		{
			var oldVerifiers = getVerifiers();
			if(oldVerifiers is List<IClientVerifier> oldVerifierList)
			{
				for(var i = 0; i < oldVerifierList.Count; i++)
				{
					if(oldVerifierList[i].GetType() == verifierToReplace)
					{
						var oldVerifier = oldVerifierList[i];
						oldVerifierList.RemoveAt(i);
						return oldVerifier;
					}
				}
				throw new Exception("Was not able to remove join verifier with type '" + verifierToReplace.FullName + "' was not found.");
			}
			//Fallback, if not a list:
			IClientVerifier removedVerifier = null;
			var newVerifiers = new List<IClientVerifier>();
			foreach(var oldVerifier in oldVerifiers)
			{
				if(oldVerifier.GetType() == verifierToReplace)
				{
					removedVerifier = oldVerifier;
				}
				else
				{
					newVerifiers.Add(oldVerifier);
				}
			}
			if(removedVerifier == null)
			{
				throw new Exception("Was not able to remove join verifier with type '" + verifierToReplace.FullName + "' was not found.");
			}
			setVerifiers(newVerifiers);
			return removedVerifier;
		}
		
		public static void replaceVerifier(Type verifierToReplace, Func<IClientVerifier, IClientVerifier> replacementVerifier)
		{
			var oldVerifiers = getVerifiers();
			//By default, the field contains an array, if so, optimize code for that and maintain field type:
			if(oldVerifiers is IClientVerifier[] oldVerifierArray)
			{
				for(var i = 0; i < oldVerifierArray.Length; i++)
				{
					if(oldVerifierArray[i].GetType() == verifierToReplace)
					{
						oldVerifierArray[i] = replacementVerifier(oldVerifierArray[i]);
						return;
					}
				}
				throw new Exception("Was not able to inject new replacement join verifier, as the original of type '" + verifierToReplace.FullName + "' was not found.");
			}
			//Fallback, if not a list:
			var notInjected = true;
			var newVerifiers = new List<IClientVerifier>();
			foreach(var oldVerifier in oldVerifiers)
			{
				if(oldVerifier.GetType() == verifierToReplace)
				{
					newVerifiers.Add(replacementVerifier(oldVerifier));
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
		
		public static void replaceVerifier(Type verifierToReplace, IClientVerifier replacementVerifier)
		{
			var oldVerifiers = getVerifiers();
			//By default, the field contains an array, if so, optimize code for that and maintain field type:
			if(oldVerifiers is IClientVerifier[] oldVerifierArray)
			{
				for(var i = 0; i < oldVerifierArray.Length; i++)
				{
					if(oldVerifierArray[i].GetType() == verifierToReplace)
					{
						oldVerifierArray[i] = replacementVerifier;
						return;
					}
				}
				throw new Exception("Was not able to inject new replacement join verifier, as the original of type '" + verifierToReplace.FullName + "' was not found.");
			}
			//Fallback, if not an array:
			var notInjected = true;
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
			//By default, the field contains an array, if so, optimize code for that and maintain field type:
			if(oldVerifiers is IClientVerifier[] oldVerifierArray)
			{
				var newVerifierArray = new IClientVerifier[oldVerifierArray.Length + 1];
				Array.Copy(oldVerifierArray, newVerifierArray, oldVerifierArray.Length);
				newVerifierArray[oldVerifierArray.Length] = newVerifier;
				setVerifiers(newVerifierArray);
				return;
			}
			//Fallback if not a list:
			var newVerifiers = new List<IClientVerifier>(getVerifiers());
			newVerifiers.Add(newVerifier);
			setVerifiers(newVerifiers);
		}
	}
}
