using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using LICC;
using ThisOtherThing.UI;
using ThisOtherThing.UI.Shapes;
using ThisOtherThing.UI.ShapeUtils;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace EccsWindowHelper.Client.Experimental
{
	/// <summary>
	/// Class <c>LWDebugger</c> is a tool, which prints a GameObject with all its properties into a file.
	/// By default it prints the 'Console' GameObject and only a selected amount of classes will be taken apart.
	/// </summary>
	public class LWDebugger
	{
		private static int depthCounter;
		private const int maxAllowedDepth = 50;

		private const BindingFlags instanceFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

		//Enable command on demand.
		//[Command]
		public static void yolo()
		{
			//This method basically prints all information about the structure of the Console GameObject and the properties of its Components. 
			// StreamWriter writer = new StreamWriter("/dev/null");
			StreamWriter writer = new StreamWriter("eccDebug.txt");
			try
			{
				foreach(GameObject o in Object.FindObjectsOfType(typeof(GameObject)))
				{
					if(!o.name.Equals("Console")) // && !o.name.Equals("Eccs: Root"))
					{
						continue;
					}
					writer.WriteLine("Processing: " + o.name);
					writer.WriteLine();
					GameObject c = o;
					c = getParent(c);
					storeReferences(c, "@->");
					inspectObject(writer, "", c);
					writer.WriteLine();
					writer.Flush();
				}
			}
			catch(Exception e)
			{
				LConsole.WriteLine(e);
				throw;
			}
			writer.Flush();
			writer.Close();
			LConsole.WriteLine("Done!");
		}

		public static void debugGameObject(GameObject obj)
		{
			StreamWriter writer = new StreamWriter("eccGODebug.txt");
			try
			{
				writer.WriteLine("Processing: " + obj.name);
				writer.WriteLine();
				GameObject c = obj;
				c = getParent(c);
				storeReferences(c, "@->");
				inspectObject(writer, "", c);
				writer.WriteLine();
				writer.Flush();
			}
			catch(Exception e)
			{
				LConsole.WriteLine(e);
				throw;
			}
			writer.Flush();
			writer.Close();
			LConsole.WriteLine("Done!");
		}

		private static readonly Dictionary<Object, string> knownReferences = new Dictionary<Object, string>();

		//Will collect all top-level 'Objects', to prevent taking them apart later on.
		private static void storeReferences(Object o, string root)
		{
			if(knownReferences.ContainsKey(o))
			{
				//Already been here.
				return;
			}
			if(o is GameObject go)
			{
				root += "G:" + o.name;
				string childRoot = root + "->";
				foreach(Component c in go.GetComponents<Component>())
				{
					storeReferences(c, childRoot);
				}
				foreach(Transform t in go.transform)
				{
					storeReferences(t.gameObject, childRoot);
				}
			}
			else if(o is Component c)
			{
				root += "C:" + c.GetType().Name;
			}
			else
			{
				root += "?:" + o.GetType().Name + " : " + o.name;
			}
			knownReferences.Add(o, root);
		}

		//Gets the top level parent of a GameObject, to print it top-down.
		private static GameObject getParent(GameObject o)
		{
			return o.transform.parent == null ? o : getParent(o.transform.parent.gameObject);
		}

		private static void inspectObject(StreamWriter writer, String prefix, Object o)
		{
			if(depthCounter >= maxAllowedDepth)
			{
				throw new Exception("Level was too deep! Aborting.");
			}
			prefix += '\t';
			if(o is GameObject go)
			{
				//Skip game objects which are not interesting (yet):
				if(false
					// || o.name.Equals("Blocker")
					// || o.name.Equals("Menu")
					|| o.name.Equals("Menu Content")
					|| o.name.Equals("Top bar")
					// || o.name.Equals("Command History Scroll View")
					// || o.name.Equals("Command Input")
					// || o.name.Equals("Title Bar")
					// || o.name.Equals("Outline")
					// || o.name.Equals("Resizing Friends")
					// || o.name.Equals("Menu Settings")
				)
				{
					//Optionally probe their relevance by turning them off:
					// go.SetActive(false);
					return;
				}

				//Go over game object and print components and children.
				writer.WriteLine(prefix + "GameObjectName: '" + o.name + "'");
				prefix += '\t'; //After the name we indent by one.
				if(go.GetComponents<Object>().Length != 0)
				{
					writer.WriteLine(prefix + "Components (" + go.GetComponents<Object>().Length + "):");
					depthCounter++;
					foreach(Object oComponent in go.GetComponents<Object>())
					{
						inspectObject(writer, prefix, oComponent);
					}
					depthCounter--;
				}
				//Do not print parent, since we are printing top-down.
				if(go.transform.childCount != 0)
				{
					writer.WriteLine(prefix + "Children (" + go.transform.childCount + "):");
					depthCounter++;
					foreach(Transform oChild in go.transform)
					{
						inspectObject(writer, prefix, oChild.gameObject);
					}
					depthCounter--;
				}
			}
			else if(o as Component)
			{
				writer.Write(prefix + "Component: ");
				type(writer, o.GetType(), o);
				prefix += '\t';

				//Handle special classes:
				if(o is RectTransform rectTransform)
				{
					printRectTransform(writer, prefix, rectTransform);
					writer.WriteLine(prefix); //Padding.
					return;
				}
				//Normally Graphic is not debugged, but this information is relevant, print it!
				if(o is Graphic g)
				{
					writer.WriteLine(prefix + "RayCastTarget: " + g.raycastTarget);
				}

				//Handle in general interesting classes:
				if(isInteresting(o.GetType()))
				{
					writer.WriteLine(prefix + "Data:");
					depthCounter++;
					takeApart(writer, prefix + "\t", o, o.GetType());
					depthCounter--;
				}
				writer.WriteLine(prefix); //Padding.
			}
			else
			{
				writer.Write(prefix + "Name: " + o.name);
				writer.Write(prefix + "Class: ");
				type(writer, o.GetType(), o);
				writer.WriteLine(prefix);
			}
		}

		private static void printRectTransform(TextWriter w, string p, RectTransform rectTransform)
		{
			w.WriteLine(p + "Anchor Min: " + toValue(rectTransform.anchorMin));
			w.WriteLine(p + "Anchor Max: " + toValue(rectTransform.anchorMax));
			w.WriteLine(p + "Pivot Point: " + toValue(rectTransform.pivot));
			w.WriteLine(p + "Anchor Pos: " + toValue(rectTransform.anchoredPosition3D));
			w.WriteLine(p + "Delta Size: " + toValue(rectTransform.sizeDelta));
			w.WriteLine(p + "Local Scale: " + toValue(rectTransform.localScale));
		}

		private static void takeApart(StreamWriter w, string p, object o, Type mainType)
		{
			if(depthCounter >= maxAllowedDepth)
			{
				throw new Exception("Level was too deep! Aborting.");
			}
			IEnumerable<FieldInfo> fields = mainType.GetFields(instanceFlags);
			fields = fields.Where(info => info.DeclaringType == mainType); //Reflection cheated this one in, I did not ask for this.
			if(fields.Any())
			{
				w.WriteLine(p + "Fields: " + fields.Count());
				string pp = p + '\t';
				foreach(var f in fields)
				{
					string prefix = pp + (f.IsPublic ? '+' : f.IsPrivate ? '-' : '*') + f.Name;
					Type t = f.FieldType;
					object value;
					try
					{
						value = f.GetValue(o);
					}
					catch(Exception e)
					{
						w.WriteLine(prefix + " = -ERROR-INVOKE-     {" + t.FullName + "}");
						continue;
					}
					if(t.IsArray)
					{
						if(value == null)
						{
							w.WriteLine(prefix + " = null     {" + t.FullName + "}");
						}
						else
						{
							IList array = (IList) value;
							w.WriteLine(prefix + " (" + array.Count + "):     {" + t.FullName + "}");
							string lp = pp + "\t";
							foreach(var entry in array)
							{
								if(entry == null)
								{
									w.WriteLine(lp + "- null");
								}
								else if(entry is Object obj && knownReferences.TryGetValue(obj, out string root))
								{
									w.WriteLine(lp + "- " + root);
								}
								else if(isInteresting(entry.GetType()))
								{
									w.WriteLine(lp + "- " + toValue(value) + ":     {" + entry.GetType().FullName + "}");
									depthCounter++;
									takeApart(w, lp + '\t', entry, entry.GetType());
									depthCounter--;
								}
								else
								{
									w.WriteLine(lp + "- " + toValue(value) + "     {" + entry.GetType().FullName + "}");
								}
							}
						}
					}
					else
					{
						if(value == null)
						{
							w.WriteLine(prefix + " = null     {" + t.FullName + "}");
						}
						else if(value is Object obj && knownReferences.TryGetValue(obj, out string root))
						{
							w.WriteLine(prefix + " = " + root);
						}
						else if(value is Action a)
						{
							Delegate[] list = a.GetInvocationList();
							w.WriteLine(prefix + " = " + toValue(value) + " ActionCount: " + list.Length + "     {" + (value.GetType() == t ? "" : value.GetType().FullName + " / ") + t.FullName + "}");
							foreach(var del in list)
							{
								var m = del.Method;
								string ex = "";
								if(del.Target != null && del.Target is Object ooo)
								{
									if(knownReferences.TryGetValue(ooo, out string r))
									{
										ex = " " + r;
									}
								}
								w.WriteLine(pp + "\t- " + m.Name + " in " + m.DeclaringType.FullName + ex);
							}
						}
						else if(value is GameObject go)
						{
							w.WriteLine(prefix + " = " + toValue(value) + ":     {" + (value.GetType() == t ? "" : value.GetType().FullName + " / ") + t.FullName + "}");
							depthCounter++;
							string newRoot = "@@->" + o.GetType().FullName + ":" + f.Name;
							storeReferences(go, newRoot);
							inspectObject(w, pp + '\t', go);
							depthCounter--;
						}
						else if(isInteresting(value.GetType()))
						{
							w.WriteLine(prefix + " = " + toValue(value) + ":     {" + (value.GetType() == t ? "" : value.GetType().FullName + " / ") + t.FullName + "}");
							depthCounter++;
							takeApart(w, pp + '\t', value, value.GetType());
							depthCounter--;
						}
						else
						{
							w.WriteLine(prefix + " = " + toValue(value) + "     {" + (value.GetType() == t ? "" : value.GetType().FullName + " / ") + t.FullName + "}");
						}
					}
				}
			}

			IEnumerable<PropertyInfo> props = mainType.GetProperties(instanceFlags);
			props = props.Where(info => info.DeclaringType == mainType); //Reflection cheated this one in, I did not ask for this.
			if(props.Any())
			{
				w.WriteLine(p + "Properties: " + props.Count());
				string pp = p + '\t';
				foreach(var f in props)
				{
					string prefix = pp + f.Name;
					Type t = f.PropertyType;
					if(f.CanRead)
					{
						object value;
						try
						{
							value = f.GetValue(o);
						}
						catch(Exception e)
						{
							w.WriteLine(prefix + " = -ERROR-INVOKE-     {" + t.FullName + "}");
							continue;
						}
						if(t.IsArray)
						{
							if(value == null)
							{
								w.WriteLine(prefix + " = null     {" + t.FullName + "}");
							}
							else
							{
								IList array = (IList) value;
								w.WriteLine(prefix + " (" + array.Count + "):     {" + t.FullName + "}");
								string lp = pp + "\t";
								foreach(var entry in array)
								{
									if(entry == null)
									{
										w.WriteLine(lp + "- null");
									}
									else if(entry is Object obj && knownReferences.TryGetValue(obj, out string root))
									{
										w.WriteLine(lp + "- " + root);
									}
									else if(isInteresting(entry.GetType()))
									{
										w.WriteLine(lp + "- " + toValue(value) + ":     {" + entry.GetType().FullName + "}");
										depthCounter++;
										takeApart(w, lp + '\t', entry, entry.GetType());
										depthCounter--;
									}
									else
									{
										w.WriteLine(lp + "- " + toValue(value) + "     {" + entry.GetType().FullName + "}");
									}
								}
							}
						}
						else
						{
							if(value == null)
							{
								w.WriteLine(prefix + " = null     {" + t.FullName + "}");
							}
							else if(value is Object obj && knownReferences.TryGetValue(obj, out string root))
							{
								w.WriteLine(prefix + " = " + root);
							}
							else if(isInteresting(value.GetType()))
							{
								w.WriteLine(prefix + " = " + toValue(value) + ":     {" + (value.GetType() == t ? "" : value.GetType().FullName + " / ") + t.FullName + "}");
								depthCounter++;
								takeApart(w, pp + '\t', value, value.GetType());
								depthCounter--;
							}
							else
							{
								w.WriteLine(prefix + " = " + toValue(value) + "     {" + (value.GetType() == t ? "" : value.GetType().FullName + " / ") + t.FullName + "}");
							}
						}
					}
					else
					{
						w.WriteLine(prefix + " <- setter");
					}
				}
			}

			Type superType = mainType.BaseType;
			if(superType != null && isInteresting(superType))
			{
				w.WriteLine(p + "Super: " + superType.FullName);
				depthCounter++;
				takeApart(w, p, o, superType);
				depthCounter--;
			}
		}

		private static string toValue(object o)
		{
			if(o == null)
			{
				return "null";
			}
			if(o is Object obj && knownReferences.TryGetValue(obj, out string root))
			{
				return root;
			}
			if(o is Vector3 v)
			{
				return "(" + v.x + ", " + v.y + ", " + v.z + ')';
			}
			if(o is Vector2 v2)
			{
				return "(" + v2.x + ", " + v2.y + ')';
			}
			if(o is string s)
			{
				return '"' + s + '"';
			}
			string stringed = o.ToString();
			if(stringed == null)
			{
				return "'Null-String'";
			}
			return stringed.Replace('\n', ' ');
		}

		private static void type(StreamWriter writer, Type t, Object o)
		{
			//Special debugging for rectangle, was used to extract textures.
			// if(t.FullName.Equals("ThisOtherThing.UI.Shapes.Rectangle"))
			// {
			// writer.WriteLine("  Has Sprite:" + (r.Sprite != null));
			// writer.WriteLine("  Has Sprite:" + (r.mainTexture != null));
			// writer.WriteLine("  Has texture:" + r.mainTexture);
			// writer.WriteLine("  Has texture:" + r.mainTexture.graphicsFormat);
			// writer.WriteLine("  Has texture:" + r.mainTexture.width);
			// writer.WriteLine("  Has texture:" + r.mainTexture.height);
			// File.WriteAllBytes("image.png", ((Texture2D) r.mainTexture).EncodeToPNG());
			// }
			if(isKnownType(t))
			{
				writer.WriteLine("[" + t.Name + "]");
				return;
			}
			writer.Write(t.FullName);
			t = t.BaseType;
			while(t != null)
			{
				if(isKnownType(t))
				{
					writer.Write(" -> [" + t.Name + ']');
					break;
				}
				writer.Write(" -> " + t.FullName);
				t = t.BaseType;
			}
			writer.WriteLine();
		}

		//Ugly hack, to check for an internal class.
		// private static readonly Type t1 = typeof(Hotbar).Assembly.GetType("LogicWorld.UI.ClickableMenuBackground");

		private static bool isInteresting(Type t)
		{
			//Whitelist:
			if(false
				//Unity:
				|| t == typeof(LayoutGroup)
				|| t == typeof(HorizontalOrVerticalLayoutGroup)
				|| t == typeof(HorizontalLayoutGroup)
				|| t == typeof(SpriteState)
				|| t == typeof(Navigation)
				|| t == typeof(AnimationTriggers)
				|| t == typeof(RectMask2D)
				|| t == typeof(Selectable)
				|| t == typeof(Mask)
				|| t == typeof(Slider)
				|| t == typeof(LayoutElement)
				|| t == typeof(ContentSizeFitter)
				|| t == typeof(VerticalLayoutGroup)
				// || t == typeof(CanvasRenderer) //Do not take apart this one, since it does not contain anything too relevant.
				|| t == typeof(Transform)
				|| t == typeof(RectTransform)
				|| t == typeof(Canvas)
				|| t == typeof(CanvasScaler)
				|| t == typeof(GraphicRaycaster)
				|| t == typeof(Graphic)
				|| t == typeof(MaskableGraphic)
				|| t == typeof(Image)
				|| t == typeof(AspectRatioFitter)
				|| t == typeof(Rectangle)
				//TMP:
				|| t == typeof(TMP_SelectionCaret)
				|| t == typeof(TMP_InputField)
				|| t == typeof(TextMeshProUGUI)
				|| t == typeof(TMP_Text)
				|| t == typeof(TMP_Style)
				//System:
				
				//LW:
				// || t == typeof(BindingOptionRenderer)
				// || t == typeof(BindingRenderer)
				// || t == typeof(ToggleSwitch)
				// || t == typeof(SliderSettingData)
				// || t == typeof(MakeInputFieldTabbable)
				// || t == typeof(PaletteInputFieldSelection)
				// || t == typeof(InputFieldSettingsApplier)
				// || t == typeof(MakeSliderScrollable)
				// || t == typeof(PaletteSelectable)
				// || t == typeof(InputSlider)
				// || t == typeof(ConfigurableMenuSettings)
				// || t == typeof(ToggleWithText)
				// || t == typeof(Toggle)
				// || t == typeof(ConfigurableMenuUtility)
				// || t == typeof(ConfigurableMenu)
				// || t == typeof(FancyPantsConsole.Console)
				// || t == t1
				// || t == typeof(HoverTagArea_Localized)
				// || t == typeof(ToggleIcon)
				// || t == typeof(HoverButton)
				// || t == typeof(LocalizedTextMesh)
				// || t == typeof(FontIcon)
				// || t == typeof(PaletteData)
				// || t == typeof(PaletteGraphic)
				// || t == typeof(PaletteRectangleOutline)
				// || t == typeof(PaletteElement)
				// || t == typeof(PopupsManager)
				// // || t == typeof(PopupBlocker) //Internal, but not interesting anyway.
				
				//ShapesUI:
				|| t == typeof(GeoUtils.ShadowProperties)
				|| t == typeof(GeoUtils.OutlineShapeProperties)
				|| t == typeof(RoundedRects.RoundedProperties)
				|| t == typeof(GeoUtils.RoundingProperties)
				|| t == typeof(GeoUtils.OutlineProperties)
				|| t == typeof(GeoUtils.ShadowsProperties)
				|| t == typeof(GeoUtils.AntiAliasingProperties)
			)
			{
				return true;
			}

			//Generic blacklist:
			string path = t.ToString();
			if(false
				|| path.StartsWith("System.")
				|| path.StartsWith("TMPro.")
				|| path.StartsWith("UnityEngine.")
			)
			{
				return false;
			}

			//Else allow anything:
			return true;
		}

		//Where to stop resolving the base type:
		private static bool isKnownType(Type t)
		{
			return t == typeof(RectTransform) ||
				t == typeof(Transform) ||
				t == typeof(MaskableGraphic)
				|| t == typeof(Graphic)
				|| t == typeof(UIBehaviour)
				|| t == typeof(MonoBehaviour)
				|| t == typeof(Behaviour)
				|| t == typeof(Component)
				|| t == typeof(GameObject)
				|| t == typeof(Object);
		}

		//Handy but not used, uff.
		public static IEnumerable<GameObject> getChildren(GameObject original)
		{
			foreach(Transform transform in original.transform)
			{
				yield return transform.gameObject;
			}
		}
	}
}
