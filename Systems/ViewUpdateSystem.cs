
using ECS.Modules.Exerussus.CartoonCharacters.MonoBehaviours;
using Exerussus._1EasyEcs.Scripts.Core;
using Leopotam.EcsLite;

namespace ECS.Modules.Exerussus.CartoonCharacters.Systems
{
    public class ViewUpdateSystem : EasySystem<CartoonCharacterPooler>
    {
        private EcsFilter _viewFilter;
        private EcsFilter _attackAnimationProcessFilter;
        private EcsFilter _bowShootAnimationProcessFilter;
        
        protected override void Initialize()
        {
            _viewFilter = World.Filter<CartoonCharacterData.CharacterApi>().End();
            _attackAnimationProcessFilter = World.Filter<CartoonCharacterData.AttackAnimationProcess>().End();
            _bowShootAnimationProcessFilter = World.Filter<CartoonCharacterData.BowShootAnimationProcess>().End();
        }

        protected override void Update()
        {
            foreach (var entity in _attackAnimationProcessFilter)
            {
                ref var attackAnimationProcessData = ref Pooler.AttackAnimationProcess.Get(entity);
                attackAnimationProcessData.TimeRemaining -= DeltaTime;
                if (attackAnimationProcessData.TimeRemaining <= 0)
                {
                    Pooler.AttackAnimationProcess.Del(entity);
                    ref var characterApiData = ref Pooler.CharacterApi.Get(entity);
                    characterApiData.Value.SetAnimation(AnimationType.None);
                }
            }
            foreach (var entity in _bowShootAnimationProcessFilter)
            {
                ref var bowShootAnimationProcessData = ref Pooler.BowShootAnimationProcess.Get(entity);
                bowShootAnimationProcessData.TimeRemaining -= DeltaTime;
                if (bowShootAnimationProcessData.TimeRemaining <= 0)
                {
                    Pooler.BowShootAnimationProcess.Del(entity);
                    ref var characterApiData = ref Pooler.CharacterApi.Get(entity);
                    if (Pooler.AimProcess.Has(entity)) characterApiData.Value.SetAnimation(AnimationType.AimBow);
                    else characterApiData.Value.SetAnimation(AnimationType.None);
                }
            }
            
            foreach (var entity in _viewFilter)
            {
                ref var characterApiData = ref Pooler.CharacterApi.Get(entity);
                characterApiData.Value.UpdateView();
            }
        }
    }
}