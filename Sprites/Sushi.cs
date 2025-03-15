using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using monogame.Animation;

namespace monogame.Sprites
{
    public class Sushi : Sprite
    {
        private Direction facingDirection;
        private float health;
        private bool isDefeated;
        private bool isAttacking;
        private float attackTimer;
        private float attackCooldown;
        private bool useAttackFrame;
        private byte currentAnimationIndex;
        private SpriteAnimation[] animations;

        public float Health => health;
        public bool IsDefeated => isDefeated;
        public Direction FacingDirection => facingDirection;
        public bool IsAttacking => isAttacking;

        public Sushi(Texture2D texture, Vector2 position, float speed) 
            : base(texture, position, speed)
        {
            health = 4f;
            attackCooldown = 1.5f;
            facingDirection = Direction.Down;
            currentAnimationIndex = 0;
            
            animations = new SpriteAnimation[4];
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (isAttacking)
            {
                attackTimer += deltaTime;
                if (attackTimer >= attackCooldown)
                {
                    isAttacking = false;
                    attackTimer = 0f;
                    useAttackFrame = false;
                }
            }

            animations[(int)facingDirection].Update(gameTime);
        }

        public void StartAttack()
        {
            if (!isAttacking)
            {
                isAttacking = true;
                attackTimer = 0f;
                useAttackFrame = true;
            }
        }

        public void SetDirection(Direction direction)
        {
            facingDirection = direction;
        }

        public void TakeDamage(float damage)
        {
            health = MathHelper.Max(0, health - damage);
            if (health <= 0)
            {
                isDefeated = true;
            }
        }

        protected override void Draw(SpriteBatch spriteBatch, Rectangle sourceRectangle)
        {
            var currentFrame = animations[(int)facingDirection].GetCurrentFrame();
            base.Draw(spriteBatch, currentFrame);
        }

        public Rectangle GetBounds()
        {
            return new Rectangle(
                (int)position.X - 48,
                (int)position.Y - 64,
                96,
                128
            );
        }
    }
}
