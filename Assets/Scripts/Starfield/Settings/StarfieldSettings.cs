using Sirenix.OdinInspector;
using UnityEngine;

namespace Starfield.Settings
{
    [System.Serializable]
    public class StarfieldSettings
    {
        [Title("Starfield Settings")] 
        [SerializeField, Required, Tooltip("Reference to the Particle System component")]
        private ParticleSystem starParticleSystem;
        
        [SerializeField, Tooltip("Total number of stars in the starfield")]
        private int starCount = 500;
        
        [SerializeField, Tooltip("Speed at which stars approach the camera")]
        private float starSpeed = 10f;
        
        [SerializeField, Tooltip("Area within which stars are spawned")]
        private Vector3 spawnArea = new Vector3(10f,10f,50f);
        
        [SerializeField, Tooltip("Seed for randomisation")]
        private int seed = 12345;
        
        [Title("Star Appearance Settings")]
        [SerializeField, Required, Tooltip("minimum star size")]
        private float minStarSize = 0.001f;
        
        [SerializeField, Required, Tooltip("minimum star size")]
        private float maxStarSize = 0.1f;
        
        [SerializeField, Required, Tooltip("The material used for the stars")]
        private Material starMaterial;
        
        public ParticleSystem StarParticleSystem
        {
            get => starParticleSystem;
            set => starParticleSystem = value;
        }

        public int StarCount
        {
            get => starCount;
            set => starCount = value;
        }

        public float StarSpeed
        {
            get => starSpeed;
            set => starSpeed = value;
        }

        public Vector3 SpawnArea
        {
            get => spawnArea;
            set => spawnArea = value;
        }

        public int Seed
        {
            get => seed;
            set => seed = value;
        }

        public float MinStarSize
        {
            get => minStarSize;
            set => minStarSize = value;
        }

        public float MaxStarSize
        {
            get => maxStarSize;
            set => maxStarSize = value;
        }

        public Material StarMaterial
        {
            get => starMaterial;
            set => starMaterial = value;
        }
    }
}


