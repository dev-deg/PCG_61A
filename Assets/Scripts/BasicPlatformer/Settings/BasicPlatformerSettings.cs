
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace BasicPlatformer.Settings
{
    [System.Serializable]
    public class BasicPlatformerSettings
    {
        //---------------------- FIXED CONSTRAINTS ----------------------
        [FoldoutGroup("Fixed Constraints")] 
        [Tooltip("Seed for the rng")] 
        [SerializeField]
        private int seed = 0;
        
        [FoldoutGroup("Fixed Constraints")] 
        [Tooltip("Width of generated level in tiles")] 
        [SerializeField]
        private int width = 50;
        
         
        [FoldoutGroup("Fixed Constraints")] 
        [Tooltip("Number of dirt layers below the top tile")] 
        [SerializeField]
        private int dirtDepth = 5;
        
        [FoldoutGroup("Fixed Constraints")] 
        [Tooltip("Fill the sky background from the terrain up to the highest grass tile.")] 
        [SerializeField]
        private int skyBackgroundFillExtra = 20;
        
        //---------------------- RANDOMISED CONSTRAINTS ----------------------
        [FoldoutGroup("Randomised Constraints")] 
        [Tooltip("Minimum surface height (y pos) for the top tiles")] 
        [SerializeField]
        private int minSurfaceHeight = 5;
        
        [FoldoutGroup("Randomised Constraints")] 
        [Tooltip("Maximum surface height (y pos) for the top tiles")] 
        [SerializeField]
        private int maxSurfaceHeight = 10;
        
        [FoldoutGroup("Randomised Constraints")] 
        [Tooltip("Minimum width (in cols) of each section")] 
        [SerializeField]
        private int minSectionWidth = 3;
        
        [FoldoutGroup("Randomised Constraints")] 
        [Tooltip("Maximum height variation between sections")] 
        [SerializeField]
        private int maxHeightVariation = 2;
        
        [FoldoutGroup("Randomised Constraints")] 
        [Tooltip("When checking for an adjacent cell. the maximum vertical difference allowed to consider the neighbour as filled")] 
        [SerializeField]
        private int adjacentTolerance = 1;
        
        [FoldoutGroup("Randomised Constraints")] 
        [Range(0,1f)]
        [Tooltip("Probability that a water section spawns in each eligible block of 50 cells")] 
        [SerializeField]
        private float waterSpawnChance = 0.4f;
        
        //---------------------- TILE REFERENCES ----------------------
        [FoldoutGroup("Tile References")]
        [Required]
        [Tooltip("Tile used for grass top")]
        [SerializeField]
        private TileBase grassTile;
        
        [FoldoutGroup("Tile References")]
        [Required]
        [Tooltip("Tile used for dirt beneath the grass")]
        [SerializeField]
        private TileBase dirtTile;
        
        [FoldoutGroup("Tile References")]
        [Required]
        [Tooltip("Tile used for grass top left corner")]
        [SerializeField]
        private TileBase grassLeftTile;
        
        [FoldoutGroup("Tile References")]
        [Required]
        [Tooltip("Tile used for grass top right corner")]
        [SerializeField]
        private TileBase grassRightTile;
        
        [FoldoutGroup("Tile References")]
        [Required]
        [Tooltip("Tile used for water")]
        [SerializeField]
        private TileBase waterTile;
        
        [FoldoutGroup("Tile References")]
        [Required]
        [Tooltip("Tile used for the sky background")]
        [SerializeField]
        private TileBase skyTile;

        public int Seed => seed;

        public int Width => width;

        public int DirtDepth => dirtDepth;

        public int SkyBackgroundFillExtra => skyBackgroundFillExtra;

        public int MinSurfaceHeight => minSurfaceHeight;

        public int MaxSurfaceHeight => maxSurfaceHeight;

        public int MinSectionWidth => minSectionWidth;

        public int MaxHeightVariation => maxHeightVariation;

        public int AdjacentTolerance => adjacentTolerance;

        public float WaterSpawnChance => waterSpawnChance;

        public TileBase GrassTile => grassTile;

        public TileBase DirtTile => dirtTile;

        public TileBase GrassLeftTile => grassLeftTile;

        public TileBase GrassRightTile => grassRightTile;

        public TileBase WaterTile => waterTile;

        public TileBase SkyTile => skyTile;
    }

}
