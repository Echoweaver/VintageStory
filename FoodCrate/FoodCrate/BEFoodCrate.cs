using System.Text;
using Vintagestory.API.Common;
using Vintagestory.GameContent;

namespace FoodCrate;

public class BEFoodCrate : BlockEntityCrate
{
  public override void GetBlockInfo(IPlayer forPlayer, StringBuilder dsc)
  {
    base.GetBlockInfo(forPlayer, dsc);
    ItemSlot contents = Inventory.FirstNonEmptySlot;
    if (contents != null && contents.Itemstack.Collectible.TransitionableProps != null 
      && contents.Itemstack.Collectible.TransitionableProps.Length > 0)
    {
      // This was the best way I could figure out to get the spoilage info into a reasonable place
      // in the tooltip and maintain the chain of calling the parent method. It seems to work
      // pretty well, actually.
      dsc.Replace(contents.GetStackName(), BlockEntityShelf.PerishableInfoCompact(Api, contents, 0));
    }
  }
}