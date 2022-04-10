using HarmonyLib;
using Timberborn.WarehousesUI;
using TimberbornAPI;
using TimberbornAPI.UIBuilderSystem;
using UnityEngine.UIElements;

namespace TimberAPIExample.Examples.DependencyContainerExample
{
    
    [HarmonyPatch(typeof(StockpileInventoryFragment), "InitializeFragment")]
    public static class StockpileVisualElementPatch
    {
        static void Postfix(VisualElement __result)
        {
            // Finds the singleton from the dependency injection system (Can any class that's connected with Bindito DI) 
            UIBuilder builder = TimberAPI.DependencyContainer.GetInstance<UIBuilder>();
            
            // Adds a new button to the stockpile inventory fragment
            __result.Add(builder.Presets().Buttons().ButtonGame(
                "preview.stockpile.inventory.button", 
                builder: buttonBuilder => buttonBuilder
                    .SetWidth(new Length(100, Length.Unit.Percent))
                    .SetHeight(new Length(25, Length.Unit.Pixel))
                    .BuildAndInitialize())
            );
        }
    }
}