using Vintagestory.API.Client;
using Vintagestory.API.Server;
using Vintagestory.API.Common;

namespace FoodCrate;

public class FoodCrateModSystem : ModSystem
{
  public override void Start(ICoreAPI api)
  {
    api.RegisterBlockEntityClass(Mod.Info.ModID + ".foodcrate", typeof(BEFoodCrate));
  }

  public override void StartServerSide(ICoreServerAPI api)
  {
  }

  public override void StartClientSide(ICoreClientAPI api)
  {
  }
}