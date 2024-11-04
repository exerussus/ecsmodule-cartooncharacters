using Exerussus._1EasyEcs.Scripts.Core;
using UnityEngine;

namespace ECS.Modules.Exerussus.CartoonCharacters
{
    public static class CartoonCharacterData
    {
        public struct CharacterApi : IEcsComponent
        {
            public MonoBehaviours.CharacterApi Value;
        }

        public struct MovingDirection : IEcsComponent
        {
            public bool IsRight;
            public bool IsUp;
            public Vector2 Value;
        }
        
        public struct LookingDirection : IEcsComponent
        {
            public bool IsRight;
            public bool IsUp;
            public Vector2 Value;
        }

        public struct Jumping : IEcsComponent
        {
            
        }

        public struct AttackAnimationProcess : IEcsComponent
        {
            public float TimeRemaining;
        }
        
        public struct BowShootAnimationProcess : IEcsComponent
        {
            public float TimeRemaining;
        }

        public struct CrouchingMark : IEcsComponent
        {
            
        }

        public struct SprintingMark : IEcsComponent
        {
            
        }

        public struct AimProcess : IEcsComponent
        {
            public float Value;
        }
    }
}