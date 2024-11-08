
using ECS.Modules.Exerussus.CartoonCharacters.MonoBehaviours;
using Exerussus._1EasyEcs.Scripts.Core;
using Exerussus._1EasyEcs.Scripts.Custom;
using Exerussus._1Extensions.Scripts.Extensions;
using Leopotam.EcsLite;
using UnityEngine;

namespace ECS.Modules.Exerussus.CartoonCharacters
{
    public class CartoonCharacterPooler : IGroupPooler
    {
        public virtual void Initialize(EcsWorld world)
        {
            CharacterApi = new(world);
            AimProcess = new(world);
            CrouchingMark = new(world);
            JumpingProcess = new(world);
            LookingDirection = new(world);
            MovingDirection = new(world);
            SprintingMark = new(world);
            AttackAnimationProcess = new(world);
            BowShootAnimationProcess = new(world);
        }
        
        private Vector3 _left = new Vector3(-1, 1, 1);
        private Vector3 _right = new Vector3(1, 1, 1);

        private AnimationType[] _attackAnimations = new AnimationType[]
        {
            AnimationType.AttackMainHand1,
            AnimationType.AttackMainHand2,
            AnimationType.AttackMainHand3
        };
        
        [InjectSharedObject] public EcsWorld World { get; private set; }
        public PoolerModule<CartoonCharacterData.CharacterApi> CharacterApi { get; private set; }
        public PoolerModule<CartoonCharacterData.AimProcess> AimProcess { get; private set; }
        public PoolerModule<CartoonCharacterData.CrouchingMark> CrouchingMark { get; private set; }
        public PoolerModule<CartoonCharacterData.SprintingMark> SprintingMark { get; private set; }
        public PoolerModule<CartoonCharacterData.LookingDirection> LookingDirection { get; private set; }
        public PoolerModule<CartoonCharacterData.MovingDirection> MovingDirection { get; private set; }
        public PoolerModule<CartoonCharacterData.JumpingProcess> JumpingProcess { get; private set; }
        public PoolerModule<CartoonCharacterData.AttackAnimationProcess> AttackAnimationProcess { get; private set; }
        public PoolerModule<CartoonCharacterData.BowShootAnimationProcess> BowShootAnimationProcess { get; private set; }

        public void SetAttack(int entity)
        {
            if (!CharacterApi.Has(entity)) return;

            ref var characterData = ref CharacterApi.Get(entity);
            
            if (AimProcess.Has(entity))
            {
                ref var bowShootAnimationProcess = ref BowShootAnimationProcess.AddOrGet(entity);
                bowShootAnimationProcess.TimeRemaining = 0.22f;
                characterData.Value.SetAnimationSecondLayer(AnimationType.ShotBow);
            }
            else
            {
                ref var attackAnimationProcess = ref AttackAnimationProcess.AddOrGet(entity);
                attackAnimationProcess.TimeRemaining = 0.40f;
                characterData.Value.SetAnimationFirstLayer(_attackAnimations.GetRandomItem());
            }
        }
        
        public void SetJump(int entity)
        {
            if (!CharacterApi.Has(entity)) return;
            
            ref var characterApiData = ref CharacterApi.Get(entity);
            ref var jumpData = ref JumpingProcess.AddOrGet(entity);
            jumpData.TimeRemaining = 0.5f;
            characterApiData.Value.SetAnimationFirstLayer(AnimationType.Jump);
        }
        
        public void SetLooking(int entity, Vector2 direction)
        {
            if (!CharacterApi.Has(entity)) return;
            
            ref var characterApiData = ref CharacterApi.Get(entity);
            ref var lookingData = ref LookingDirection.AddOrGet(entity);
            lookingData.IsRight = direction.x > characterApiData.Value.transform.position.x;
            lookingData.IsUp = direction.y > characterApiData.Value.NeckPosition.y;
            
            UpdateCharacterAnimation(entity);
        }
        
        public void SetMoving(int entity, Vector2 direction)
        {
            ref var movingData = ref MovingDirection.AddOrGet(entity);
            
            var targetMoving = movingData.IsRight ? -0.1f : 0.1f;
            movingData.IsUp = direction.y > 0;
            movingData.IsRight = direction.x > targetMoving;
            UpdateCharacterAnimation(entity);
        }

        public void StopMoving(int entity)
        {
            MovingDirection.Del(entity);
            UpdateCharacterAnimation(entity);
        }

        public void UpdateCharacterAnimation(int entity)
        {
            if (!CharacterApi.Has(entity)) return;
            
            ref var lookingData = ref LookingDirection.AddOrGet(entity);
            ref var characterData = ref CharacterApi.Get(entity);
            characterData.Value.transform.localScale = lookingData.IsRight ? _right : _left;

            if (AttackAnimationProcess.Has(entity)) return;
            if (JumpingProcess.Has(entity)) return;
            
            if (CrouchingMark.Has(entity))
            {
                if (!MovingDirection.Has(entity)) characterData.Value.SetAnimationFirstLayer(AnimationType.Crouch);
                else
                {
                    ref var movingData = ref MovingDirection.Get(entity);
                    if (lookingData.IsRight == movingData.IsRight) characterData.Value.SetAnimationFirstLayer(AnimationType.Walk);
                    else characterData.Value.SetAnimationFirstLayer(AnimationType.WalkBack);
                }
            }
            else if (SprintingMark.Has(entity) && MovingDirection.Has(entity))
            {
                ref var movingData = ref MovingDirection.Get(entity);
                if (lookingData.IsRight == movingData.IsRight) characterData.Value.SetAnimationFirstLayer(AnimationType.Sprint);
                else characterData.Value.SetAnimationFirstLayer(AnimationType.RunBack);
            }
            else
            {
                if (!MovingDirection.Has(entity)) characterData.Value.SetAnimationFirstLayer(AnimationType.Idle);
                else
                {
                    ref var movingData = ref MovingDirection.Get(entity);
                    if (lookingData.IsRight == movingData.IsRight)
                    {
                        characterData.Value.SetAnimationFirstLayer(AnimationType.Run);
                    }
                    else characterData.Value.SetAnimationFirstLayer(AnimationType.RunBack);
                }
            }
        }
        
        public void SetCharacterAimBow(int entity, Vector2 direction)
        {
            if (!CharacterApi.Has(entity)) return;
            
            ref var aimProcessData = ref AimProcess.AddOrGet(entity);
            ref var lookingDirectionData = ref LookingDirection.AddOrGet(entity);
            ref var characterApiData = ref CharacterApi.Get(entity);
            lookingDirectionData.IsRight = direction.x > characterApiData.Value.transform.position.x;
            
            var rawDirection = direction - (Vector2)characterApiData.Value.NeckPosition;
            var aimDirection = (new Vector2(rawDirection.x, rawDirection.y)).normalized * 0.95f;
            aimProcessData.Value = Mathf.Clamp(aimDirection.y, -1, 1);
            lookingDirectionData.Value = direction;
            characterApiData.Value.SetAim(aimProcessData.Value);
            if(AttackAnimationProcess.Has(entity) || BowShootAnimationProcess.Has(entity) || JumpingProcess.Has(entity)) return;
            characterApiData.Value.SetAnimationSecondLayer(AnimationType.AimBow);
        }
        
        public void SetCharacterAimStop(int entity)
        {
            if (!CharacterApi.Has(entity)) return;
            if (!AimProcess.Has(entity)) return;
            
            AimProcess.Del(entity);
            ref var characterApiData = ref CharacterApi.Get(entity);
            if(AttackAnimationProcess.Has(entity) || BowShootAnimationProcess.Has(entity) || JumpingProcess.Has(entity)) return;
            characterApiData.Value.SetAnimationSecondLayer(AnimationType.None);
        }
    }
}
