using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using monogame.Animation;
using monogame.Effects;

namespace monogame.Sprites
{
    public class Donut : Sprite
    {
        private Direction currentDirection;
        private bool isJumping;
        private float jumpTimer;
        private float jumpDuration;
        private float jumpHeight;
        private float jumpStartY;
        private bool isMoving;
        private bool isAttacking;
        private float attackTimer;
        private const float AttackDuration = 0.5f;
        
        public bool HasAxe { get; private set; } = false;
        public bool InGame2 { get; private set; } = false;
        
        public void PickupAxe() { HasAxe = true; }
        public void SetInGame2(bool inGame2) { InGame2 = inGame2; }
        private const float SecondFrameDuration = 0.8f;
        private float animationTimer;
        private byte currentAnimationIndex;
        private int animationCycleCount;
        private bool useBlinkingFrame;
        private const float AnimationThreshold = 150f;
        private KeyboardState previousKeyboardState;
        private MouseState previousMouseState;
        private const float MinY = 325f;

        public enum Direction { Down, Up, Left, Right }

        public Direction CurrentDirection => currentDirection;
        
        public new float Health => currentHealth;

        private Dictionary<DonutColor, Texture2D> colorTextures = new Dictionary<DonutColor, Texture2D>();
        
        private DonutColor currentColor = DonutColor.Normal;

        public new Texture2D Texture => texture;

        public Donut(Texture2D texture, Vector2 position, float speed) 
            : base(texture, position, speed)
        {
            maxHealth = 100f;
            currentHealth = maxHealth;
            jumpDuration = 1.0f;
            jumpHeight = 50f;
            currentDirection = Direction.Down;
        }

        public override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            var keyboardState = Keyboard.GetState();
            var mouseState = Mouse.GetState();
            Vector2 movement = Vector2.Zero;
            isMoving = false;

            if (keyboardState.IsKeyDown(Keys.Up))
            {
                movement.Y -= 1;
                currentDirection = Direction.Up;
                isMoving = true;
            }
            if (keyboardState.IsKeyDown(Keys.Down))
            {
                movement.Y += 1;
                currentDirection = Direction.Down;
                isMoving = true;
            }
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                movement.X -= 1;
                currentDirection = Direction.Left;
                isMoving = true;
            }
            if (keyboardState.IsKeyDown(Keys.Right))
            {
                movement.X += 1;
                currentDirection = Direction.Right;
                isMoving = true;
            }


            if (movement != Vector2.Zero)
            {
                movement.Normalize();
                Vector2 newPosition = Position + movement * Speed * deltaTime;
                
                if (newPosition.Y >= MinY)
                {
                    Position = newPosition;
                }
                else
                {
                    Position = new Vector2(newPosition.X, MinY);
                }
            }

            if (keyboardState.IsKeyDown(Keys.Space) && !previousKeyboardState.IsKeyDown(Keys.Space) && !isJumping)
            {
                isJumping = true;
                jumpTimer = 0f;
                jumpStartY = Position.Y;
            }

            if (isJumping)
            {
                jumpTimer += deltaTime;
                float progress = jumpTimer / jumpDuration;
                float offset = jumpHeight * (float)Math.Sin(Math.PI * progress);
                Position = new Vector2(Position.X, jumpStartY - offset);

                if (jumpTimer >= jumpDuration)
                {
                    Position = new Vector2(Position.X, jumpStartY);
                    isJumping = false;
                    jumpTimer = 0f;
                }
            }

            if (mouseState.LeftButton == ButtonState.Pressed && !isAttacking && previousMouseState.LeftButton != ButtonState.Pressed)
            {
                isAttacking = true;
                attackTimer = 0f;
            }

            if (isAttacking)
            {
                attackTimer += deltaTime;
                if (attackTimer >= AttackDuration + SecondFrameDuration)
                {
                    isAttacking = false;
                    attackTimer = 0f;
                }
            }

            if (isMoving || isJumping || isAttacking)
            {
                animationTimer += deltaTime * 1000;
                if (animationTimer > AnimationThreshold)
                {
                    currentAnimationIndex = (byte)((currentAnimationIndex + 1) % 3);
                    if (currentAnimationIndex == 0)
                    {
                        animationCycleCount++;
                        useBlinkingFrame = animationCycleCount % 3 == 0;
                    }
                    animationTimer = 0f;
                }
            }
            else
            {
                currentAnimationIndex = 1;
                animationTimer = 0f;
            }

            previousKeyboardState = keyboardState;
            previousMouseState = mouseState;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Draw(spriteBatch, Color.White);
        }

        public DonutColor SetColor(DonutColor newColor)
        {
            DonutColor oldColor = currentColor;
            currentColor = newColor;
            return oldColor;
        }

        public DonutColor GetColor()
        {
            return currentColor;
        }
        
        public DonutColor CycleToNextColor()
        {
            int colorCount = Enum.GetValues(typeof(DonutColor)).Length;
            int nextColorIndex = ((int)currentColor + 1) % colorCount;
            currentColor = (DonutColor)nextColorIndex;
            
            return currentColor;
        }
        
        public void DrawWithColorReplacement(SpriteBatch spriteBatch)
        {
            GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
            
            if (currentColor == DonutColor.Normal)
            {
                Draw(spriteBatch);
                return;
            }
            
            if (!colorTextures.TryGetValue(currentColor, out Texture2D coloredTexture))
            {
                coloredTexture = ColorReplacer.CreateColoredTexture(graphicsDevice, texture, currentColor);
                colorTextures[currentColor] = coloredTexture;
            }
            
            Rectangle currentFrame = GetCurrentFrame();
            
            Vector2 origin;
            if (isAttacking && attackTimer <= AttackDuration && 
                (currentDirection == Direction.Left || currentDirection == Direction.Right))
            {
                origin = new Vector2(192 / 2, 128 / 2);
            }
            else
            {
                origin = new Vector2(96 / 2, 128 / 2);
            }
            
            spriteBatch.Draw(
                colorTextures[currentColor],
                Position,
                currentFrame,
                Color.White,
                0f,
                origin,
                1.0f,
                SpriteEffects.None,
                0f
            );
            
            DrawDonutHealthBar(spriteBatch);
        }

        public void Draw(SpriteBatch spriteBatch, Color color)
        {
            Rectangle currentFrame = GetCurrentFrame();
            
            Vector2 origin;
            if (isAttacking && attackTimer <= AttackDuration && 
                (currentDirection == Direction.Left || currentDirection == Direction.Right))
            {
                origin = new Vector2(192 / 2, 128 / 2);
            }
            else
            {
                origin = new Vector2(96 / 2, 128 / 2);
            }
            
            spriteBatch.Draw(
                texture,
                Position,
                currentFrame,
                color,
                0f,
                origin,
                1.0f,
                SpriteEffects.None,
                0f
            );
            DrawDonutHealthBar(spriteBatch);
        }
        
        private void DrawDonutHealthBar(SpriteBatch spriteBatch)
        {
            int healthBarWidth = 200;
            int healthBarHeight = 20;
            int borderSize = 2;
            
            GraphicsDevice graphicsDevice = spriteBatch.GraphicsDevice;
            int screenWidth = graphicsDevice.Viewport.Width;
            
            Vector2 healthBarPosition = new Vector2(
                screenWidth - healthBarWidth - 10,
                10
            );
            
            Rectangle borderRectangle = new Rectangle(
                (int)healthBarPosition.X - borderSize,
                (int)healthBarPosition.Y - borderSize,
                healthBarWidth + 2*borderSize,
                healthBarHeight + 2*borderSize
            );
            spriteBatch.Draw(Game1.WhitePixel, borderRectangle, Color.Black);
            
            Rectangle backgroundRectangle = new Rectangle(
                (int)healthBarPosition.X,
                (int)healthBarPosition.Y,
                healthBarWidth,
                healthBarHeight
            );
            spriteBatch.Draw(Game1.WhitePixel, backgroundRectangle, Color.DarkGray);
            
            int currentHealthWidth = (int)(healthBarWidth * (currentHealth / maxHealth));
            Rectangle currentHealthRectangle = new Rectangle(
                (int)healthBarPosition.X,
                (int)healthBarPosition.Y,
                currentHealthWidth,
                healthBarHeight
            );
            
            Color healthColor;
            
            if (currentHealth >= maxHealth * 0.66f)
            {
                healthColor = new Color(0, 255, 255);
            }
            else if (currentHealth >= maxHealth * 0.33f)
            {
                healthColor = new Color(0, 200, 220);
            }
            else
            {
                healthColor = new Color(0, 120, 180);
            }
            
            spriteBatch.Draw(Game1.WhitePixel, currentHealthRectangle, healthColor);
        }

        private Rectangle GetCurrentFrame()
        {
            int frameWidth = 96;
            int frameHeight = 128;
            int doubleWidth = 192;
            
            if (InGame2 && HasAxe && isAttacking)
            {
                int frameX = attackTimer <= AttackDuration / 2 ? 768 : 864;
                
                return currentDirection switch
                {
                    Direction.Up => new Rectangle(frameX, 0, frameWidth, frameHeight),
                    Direction.Down => new Rectangle(frameX, 256, frameWidth, frameHeight),
                    Direction.Left => new Rectangle(frameX, 384, frameWidth, frameHeight),
                    Direction.Right => new Rectangle(frameX, 128, frameWidth, frameHeight),
                    _ => new Rectangle(frameX, 128, frameWidth, frameHeight)
                };
            }

            if (isJumping)
            {
                return currentDirection switch
                {
                    Direction.Up => new Rectangle(288, 0, frameWidth, frameHeight),
                    Direction.Down => new Rectangle(288, 256, frameWidth, frameHeight),
                    Direction.Left => new Rectangle(288, 384, frameWidth, frameHeight),
                    Direction.Right => new Rectangle(288, 118, frameWidth, frameHeight),
                    _ => new Rectangle(288, 128, frameWidth, frameHeight)
                };
            }

            if (isAttacking)
            {
                if (attackTimer <= AttackDuration)
                {
                    return currentDirection switch
                    {
                        Direction.Up => new Rectangle(100, 0, frameWidth, frameHeight),
                        Direction.Down => new Rectangle(520, 256, frameWidth, frameHeight),
                        Direction.Left => new Rectangle(480, 384, doubleWidth, frameHeight),
                        Direction.Right => new Rectangle(488, 128, doubleWidth, frameHeight),
                        _ => new Rectangle(490, 128, doubleWidth, frameHeight)
                    };
                }
                else
                {
                    return currentDirection switch
                    {
                        Direction.Up => new Rectangle(404, 0, frameWidth, frameHeight),
                        Direction.Down => new Rectangle(394, 256, frameWidth, frameHeight),
                        Direction.Left => new Rectangle(384, 384, frameWidth, frameHeight),
                        Direction.Right => new Rectangle(404, 128, frameWidth, frameHeight),
                        _ => new Rectangle(404, 128, frameWidth, frameHeight)
                    };
                }
            }

            Rectangle[] baseRectangles = currentDirection switch
            {
                Direction.Up => new Rectangle[]
                {
                    new Rectangle(0, 0, frameWidth, frameHeight),
                    new Rectangle(96, 0, frameWidth, frameHeight),
                    new Rectangle(192, 0, frameWidth, frameHeight)
                },
                Direction.Down => new Rectangle[]
                {
                    new Rectangle(0, 256, frameWidth, frameHeight),
                    new Rectangle(96, 256, frameWidth, frameHeight),
                    new Rectangle(192, 256, frameWidth, frameHeight)
                },
                Direction.Left => new Rectangle[]
                {
                    new Rectangle(0, 384, frameWidth, frameHeight),
                    new Rectangle(96, 384, frameWidth, frameHeight),
                    new Rectangle(192, 384, frameWidth, frameHeight)
                },
                Direction.Right => new Rectangle[]
                {
                    new Rectangle(0, 128, frameWidth, frameHeight),
                    new Rectangle(96, 128, frameWidth, frameHeight),
                    new Rectangle(192, 128, frameWidth, frameHeight)
                },
                _ => new Rectangle[]
                {
                    new Rectangle(0, 128, frameWidth, frameHeight),
                    new Rectangle(96, 128, frameWidth, frameHeight),
                    new Rectangle(192, 128, frameWidth, frameHeight)
                }
            };

            if (useBlinkingFrame && currentAnimationIndex == 2)
            {
                return new Rectangle(288, baseRectangles[0].Y, frameWidth, frameHeight);
            }

            return baseRectangles[currentAnimationIndex];
        }

        public void StartJump()
        {
            if (!isJumping)
            {
                isJumping = true;
                jumpTimer = 0f;
                jumpStartY = position.Y;
            }
        }

        public override bool TakeDamage(float damage)
        {
            if (isInvulnerable)
            {
                currentHealth = MathHelper.Max(0, currentHealth - damage * 0.5f);
                return true;
            }
            
            bool damageDealt = base.TakeDamage(damage);
            
            invulnerabilityTimer = InvulnerabilityDuration * 0.25f;
            
            if (damageDealt && currentHealth <= 0)
            {
                // Handle donut death if needed
            }
            
            return damageDealt;
        }
        

        public override Rectangle GetBounds()
        {
            return new Rectangle(
                (int)position.X - 14,
                (int)position.Y - 18,
                28,
                36              
            );
        }


    }
}
