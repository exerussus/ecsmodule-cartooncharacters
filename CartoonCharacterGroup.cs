using ECS.Modules.Exerussus.CartoonCharacters.Systems;
using Exerussus._1EasyEcs.Scripts.Custom;
using Leopotam.EcsLite;

namespace ECS.Modules.Exerussus.CartoonCharacters
{
    public class CartoonCharacterGroup : EcsGroup<CartoonCharacterPooler>
    {
        protected override void SetFixedUpdateSystems(IEcsSystems fixedUpdateSystems)
        {
            fixedUpdateSystems.Add(new ViewUpdateSystem());
        }
    }
}