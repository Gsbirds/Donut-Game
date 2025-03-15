namespace monogame.Config
{
    public static class GameConfig
    {
        // Player configuration
        public const float PLAYER_SPEED = 100f;
        public const float PLAYER_JUMP_HEIGHT = 50f;
        public const float PLAYER_JUMP_DURATION = 1.0f;
        public const float PLAYER_INITIAL_HEALTH = 4f;

        // Enemy configuration
        public const float NACHO_SPEED = 40f;
        public const float NACHO_INITIAL_HEALTH = 4f;
        public const float NACHO_ROTATION_SPEED = 0.01f;
        public const float NACHO_MAX_ROTATION = 0.1f;
        
        public const float EMPANADA_SPEED = 60f;
        public const float EMPANADA_ATTACK_COOLDOWN = 1.5f;
        public const float EMPANADA_ATTACK_RANGE = 100f;
        public const float EMPANADA_ATTACK_DAMAGE = 0.5f;

        // Animation configuration
        public const float DEFAULT_FRAME_DURATION = 0.2f;
        public const int ANIMATION_THRESHOLD = 150;
        
        // Game mechanics
        public const float MIN_DISTANCE_BETWEEN_ENEMIES = 170f;
        public const float CHEESE_DAMAGE = 0.5f;
        public const float CHEESE_VISIBILITY_DURATION = 4f;
        public const float CHEESE_SPLASH_DURATION = 1f;
    }
}
