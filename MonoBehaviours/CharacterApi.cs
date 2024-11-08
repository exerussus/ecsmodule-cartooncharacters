
using System.Collections.Generic;
using CharacterCreator2D;
using Exerussus._1Attributes;
using Leopotam.EcsLite;
using UnityEngine;

namespace ECS.Modules.Exerussus.CartoonCharacters.MonoBehaviours
{
    [RequireComponent(typeof(CharacterViewer))]
    public class CharacterApi : MonoBehaviour
    {
        private static readonly int Aim = Animator.StringToHash("Aim");

        private static readonly Dictionary<AnimationType, int> Animations = new Dictionary<AnimationType, int>
        {
            { AnimationType.None, Animator.StringToHash("None") },
            { AnimationType.Idle, Animator.StringToHash("Idle") },
            { AnimationType.Walk, Animator.StringToHash("Walk") },
            { AnimationType.WalkBack, Animator.StringToHash("Walk Back") },
            { AnimationType.Run, Animator.StringToHash("Run") },
            { AnimationType.RunBack, Animator.StringToHash("Run Back") },
            { AnimationType.Sprint, Animator.StringToHash("Sprint") },
            { AnimationType.Jump, Animator.StringToHash("Jump") },
            { AnimationType.Fall, Animator.StringToHash("Fall") },
            { AnimationType.Hit, Animator.StringToHash("Hit") },
            { AnimationType.Die, Animator.StringToHash("Die") },
            { AnimationType.AimBow, Animator.StringToHash("Aim Bow") },
            { AnimationType.ShotBow, Animator.StringToHash("Shot Bow") },
            { AnimationType.Crouch, Animator.StringToHash("Crouch") },
            { AnimationType.AttackMainHand1, Animator.StringToHash("Attack Main Hand 1") },
            { AnimationType.AttackMainHand2, Animator.StringToHash("Attack Main Hand 2") },
            { AnimationType.AttackMainHand3, Animator.StringToHash("Attack Main Hand 3") },
        };
        
        [SerializeField] private CharacterViewer characterViewer;
        [SerializeField] private Animator animator;
        [SerializeField] private Transform weaponTransform;
        [SerializeField] private Transform neckTransform;
        [SerializeField] private Weapon weaponInfo;
        [SerializeField] private Weapon bowInfo;
        [SerializeField] private AnimationType currentAnimation;
        private CartoonCharacterPooler _pooler;
        private EcsPackedEntity _packedEntity;

        public CharacterViewer CharacterViewer => characterViewer;
        public Animator Animator => animator;
        public Vector3 WeaponPosition => weaponTransform.position;
        public Vector3 NeckPosition => neckTransform.position;
        private bool _isInitialized;
        
        public void Initialize(EcsPackedEntity packedEntity, CartoonCharacterPooler pooler)
        {
            _packedEntity = packedEntity;
            _pooler = pooler;
            ref var characterApiData = ref _pooler.CharacterApi.Add(packedEntity.Id);
            characterApiData.Value = this;
            characterViewer.InitializeView();
            TryFoundWeapon();
            TryFoundNeck();
            _isInitialized = true;
        }

        public void Deinitialize()
        {
            if (_pooler == null) return;
            if (!_isInitialized) return;
            if (!_packedEntity.Unpack(_pooler.World, out var entity)) return;
            _pooler.CharacterApi.Del(entity);
        }

        public void UpdateView()
        {
            characterViewer.UpdateView();
        }

        public void SetAnimationFirstLayer(AnimationType animationType)
        {
            if(currentAnimation == animationType) return;
            currentAnimation = animationType;
            animator.CrossFade(Animations[animationType], 0.2f, 0);
        }

        public void SetAnimationSecondLayer(AnimationType animationType)
        {
            animator.CrossFade(Animations[animationType], 0.1f, 1);
        }
        
        public void SetEmote(EmotionType emotionType)
        {
            characterViewer.Emote(emotionType);
        }
        
        public void SetAim(float value)
        {
            animator.SetFloat(Aim, value);
        }

        public void SetView(TextAsset json)
        {
            var data = JsonUtility.FromJson<CharacterData>(json.text);
            SetView(data);
        }

        public void SetView(CharacterData characterData)
        {
            characterViewer.AssignCharacterData(characterData);
            TryFoundWeapon();
        }

        [Button]
        private void TestSetWeapon()
        {
            SetWeapon(weaponInfo);
            if (weaponTransform as Transform == null) TryFoundWeapon();
        }
        

        [Button]
        private void TestSetBow()
        {
            SetBow(bowInfo);
            if (weaponTransform as Transform == null) TryFoundWeapon();
        }
        
        [Button]
        private void TryFoundWeapon()
        {
            weaponTransform = characterViewer.transform
                .Find("Root")
                .Find("Pos_Hip")
                .Find("Bone_Hip")
                .Find("Pos_Body")
                .Find("Bone_Body")
                .Find("Pos_Upper Arm R")
                .Find("Bone_Upper Arm R")
                .Find("Pos_Lower Arm R")
                .Find("Bone_Lower Arm R")
                .Find("Pos_Hand R")
                .Find("Bone_Hand R")
                .Find("Pos_Weapon R")
                .Find("Bone_Weapon R")
                .Find("Weapon R");
        }
        
        [Button]
        private void TryFoundNeck()
        {
            neckTransform = characterViewer.transform
                .Find("Root")
                .Find("Pos_Hip")
                .Find("Bone_Hip")
                .Find("Pos_Body")
                .Find("Bone_Body")
                .Find("Pos_Neck")
                .Find("Bone_Neck")
                .Find("Neck");
        }
        
        [Button]
        public void UnsetWeapon()
        {
            characterViewer.EquipPart(SlotCategory.MainHand, "");
        }
        
        public void SetBow(Weapon weapon)
        {
            characterViewer.EquipPart(SlotCategory.OffHand, weapon);
        }
        
        public void SetWeapon(Weapon weapon)
        {
            characterViewer.EquipPart(SlotCategory.MainHand, weapon);
        }
        
        private void OnValidate()
        {
            characterViewer ??= GetComponent<CharacterViewer>();
            animator ??= GetComponent<Animator>();
        }
    }

    public enum AnimationType
    {
        None,
        Idle,
        Walk,
        WalkBack,
        Run,
        RunBack,
        Sprint,
        Jump,
        Fall,
        Hit,
        Die,
        AimBow,
        Crouch,
        ShotBow,
        AttackMainHand1,
        AttackMainHand2,
        AttackMainHand3,
    }
}