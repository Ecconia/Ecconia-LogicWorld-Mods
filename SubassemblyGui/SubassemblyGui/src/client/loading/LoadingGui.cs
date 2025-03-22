using System.Collections.Generic;
using System.Linq;
using EccsGuiBuilder.Client.Layouts.Helper;
using EccsGuiBuilder.Client.Wrappers;
using EccsGuiBuilder.Client.Wrappers.AutoAssign;
using FancyInput;
using JetBrains.Annotations;
using JimmysUnityUtilities;
using LogicUI;
using LogicUI.MenuParts;
using LogicUI.MenuTypes;
using LogicUI.MenuTypes.Searching;
using LogicWorld.GameStates;
using UnityEngine;
using UnityEngine.UI;

namespace SubassemblyGui.Client.loading
{
	public class LoadingGui : ToggleableSingletonMenu<LoadingGui>, IAssignMyFields, ISearchList<SubassemblyCard>
	{
		public static void initialize()
		{
			WS.window("SubassemblyGuiLoadSubassemblyWindow")
				.setLocalizedTitle("SubassemblyGui.LoadSubassemblyWindow")
				.doNotBlurBuildingCanvas()
				.setResizeable()
				.setDefaultSize(800, 1000)
				.setYPosition(null) // Center automatically based on default size
				.configureContent(content => content
					.layoutGrowElement(
						padding: new RectOffset(20, 20, 15, 15),
						elementIndex: IndexHelper.Last
					)
					.add(WS.searchBox
						.injectionKey(nameof(searchBox))
						.addAndConfigure<LayoutElement>(layout => {
							layout.minHeight = layout.preferredHeight = 50f;
						})
					)
					.add(WS.scrollableVertical
						.addAndConfigure<LayoutElement>(layout => {
							layout.minWidth = 300f;
							layout.preferredWidth = 500f;
							layout.minHeight = 200f;
							layout.preferredHeight = 300f;
						})
						.configureContent(scrollContent => scrollContent
							.injectionKey(nameof(scrollAreaContent))
							.setAlignment(Alignment.Top) //Expand horizontally. Used to have the width from parent be applied. The stock scroller supports this by itself.
							.addAndConfigure<ContentSizeFitter>(fitter => {
								fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
								fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
							})
							.layoutGrowGap(
								padding: new RectOffset(15, 15, 10, 10),
								gapIndex: IndexHelper.Last
							)
							.add(WS.textLine
								.injectionKey(nameof(loadingIndicator))
								.setLocalizationKey("SubassemblyGui.Gui.LoadingIndicator")
							)
						)
					)
				)
				.add<LoadingGui>()
				.build();
			
			OnMenuShown += () => Instance.onMenuShown();
		}
		
		[AssignMe] [UsedImplicitly]
		private SearchBox searchBox;
		private Searcher<SubassemblyCard> searcher;
		
		[AssignMe] [UsedImplicitly]
		private GameObject scrollAreaContent;
		
		[AssignMe] [UsedImplicitly]
		private GameObject loadingIndicator;
		
		private readonly List<SubassemblyCard> loadedSubassemblyCards = new List<SubassemblyCard>();
		private readonly ObjectPoolUtility<SubassemblyCard> subassemblyCardPool = new ObjectPoolUtility<SubassemblyCard>(parent => {
			var card = Instantiate(SubassemblyCard.pattern, parent).GetComponent<SubassemblyCard>();
			card.Initialize();
			return card;
		});
		
		public override void Initialize()
		{
			base.Initialize();
			
			searcher = new Searcher<SubassemblyCard>(this, searchBox);
			searcher.OnSearchSubmit += onSearchSubmit;
		}
		
		private static void onSearchSubmit(SubassemblyCard card)
		{
			card?.onClicked();
		}
		
		private void onMenuShown()
		{
			if (loadingIndicator.activeSelf)
			{
				//The loading indicator is visible, this essentially means, that loading is not done and stuff should be loaded...
				refreshSubassemblies();
			}
			//Attempt to grab focus:
			searchBox.ActivateInputField();
		}
		
		public static void triggerRefreshOnOpen()
		{
			//Kind of hacky, but that is exactly how this works.
			// When this window is opened and the loading indicator is visible, it refreshes.
			Instance.loadingIndicator.SetActive(true);
		}
		
		public void OnRun()
		{
			if (UITrigger.Back.DownThisFrame())
			{
				GameStateManager.TransitionBackToBuildingState();
			}
			
			if (RawInput.F5.DownThisFrame())
			{
				refreshSubassemblies();
			}
		}
		
		private void refreshSubassemblies()
		{
			loadingIndicator.SetActive(true);
			loadedSubassemblyCards.ForEach(subassemblyCardPool.Recycle);
			loadedSubassemblyCards.Clear();
			
			//Load a frame later, to give the 'Loading...' indicator a chance of being rendered.
			CoroutineUtility.RunAfterOneFrame(() => {
				loadingIndicator.SetActive(false);
				
				//TBI: Normally I would want to sort this ordinarily - to prevent funny surprises.
				// However in this funny case, sorting ordinarily does not sort at all. Got to love C#.
				var subassemblyMetas = SubassemblyQuery.gatherSubassemblyMeta().OrderBy(a => a.title);
				foreach (var subassemblyMeta in subassemblyMetas)
				{
					var subassemblyCard = subassemblyCardPool.Get(scrollAreaContent.transform);
					loadedSubassemblyCards.Add(subassemblyCard);
					subassemblyCard.subassemblyMeta = subassemblyMeta;
				}
				
				searcher.SearchListContentsChanged();
			});
		}
		
		public IEnumerable<SubassemblyCard> GetAllSearchItems()
		{
			return loadedSubassemblyCards;
		}
		
		public IEnumerable<GameObject> GetAllDependentObjects()
		{
			yield break;
		}
	}
}
