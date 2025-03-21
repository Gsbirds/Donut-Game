﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using monogame.Sprites;
using monogame.Animation;

namespace monogame
{
    public class Game1 : IGameState
    {
        #region Fields

        private SpriteBatch _spriteBatch;
        private GraphicsDevice _graphicsDevice;
        private MainGame _mainGame;
        private SpriteFont font;

        private Donut donut;
        private Nacho nachoSprite;
        private Empanada empanadaSprite;
        private const float MinDistanceBetweenNachoAndEmpanada = 170f;
        private const float AttackRange = 50f;
        private Rectangle[] empanadaFrames;
        private Rectangle[] downRectangles;
        private Rectangle[] upRectangles;
        private Rectangle[] leftRectangles;
        private Rectangle[] rightRectangles;
        private int doubleWidth = 192;

        private Projectile cheeseProjectile;
        private float projectileCooldownTimer = 0f;
        private const float ProjectileCooldown = 5f;

        private bool showSplashCheese = false;
        private float splashCheeseTimer = 0f;
        private const float splashCheeseDuration = 1f;
        private Vector2 splashPosition;

        private Texture2D charaset;
        private Texture2D cheeseLaunch;
        private Texture2D sombreroWallpaper;
        private Texture2D splashCheese;
        private Texture2D sombrero;
        private Texture2D background;
        private Texture2D weed;
        private Texture2D pipe;
        private Texture2D mushroom;
        private Texture2D churroTree;
        public static Texture2D WhitePixel;

        private Texture2D[] pipes;
        private Vector2[] pipePositions;
        private int[] currentPipeFrameIndices;
        private float[] pipeAnimationTimers;
        private float pipeFrameDuration = 0.2f;
        private Texture2D puprmushSpritesheet;
        private Rectangle[] puprmushFrames;
        private int currentPuprmushFrame;
        private float puprmushFrameTimer;
        private const float PuprmushFrameDuration = 0.4f;
        private Vector2 sombreroPosition = new Vector2(300, 300);

        #endregion

        private bool showSplashEffect;
        private float splashTimer;
    
        private Texture2D nacho;
        private Texture2D empanada;
        private const float SPLASH_DURATION = 1f;


        public Game1(MainGame mainGame, SpriteBatch spriteBatch)
        {
            _mainGame = mainGame;
            _spriteBatch = spriteBatch;
            _graphicsDevice = mainGame.GraphicsDevice;
        }

        public void LoadContent()
        {

            charaset = _mainGame.Content.Load<Texture2D>("donutsprites20");
            nacho = _mainGame.Content.Load<Texture2D>("nachosprites4");
            font = _mainGame.Content.Load<SpriteFont>("DefaultFont1");
            cheeseLaunch = _mainGame.Content.Load<Texture2D>("cheeselaunch");
            var nachoOpenMouthTexture = _mainGame.Content.Load<Texture2D>("openmountnacho2");
            sombreroWallpaper = _mainGame.Content.Load<Texture2D>("sombrerosetting");
            splashCheese = _mainGame.Content.Load<Texture2D>("splashcheese");
            empanada = _mainGame.Content.Load<Texture2D>("empanadasprites11");
            sombrero = _mainGame.Content.Load<Texture2D>("Sombrero");
            background = _mainGame.Content.Load<Texture2D>("pink_purp_background");
            weed = _mainGame.Content.Load<Texture2D>("weed");
            churroTree = _mainGame.Content.Load<Texture2D>("churroTree");
            pipe = _mainGame.Content.Load<Texture2D>("pipe2");
            puprmushSpritesheet = _mainGame.Content.Load<Texture2D>("Puprmush");

            WhitePixel = new Texture2D(_graphicsDevice, 1, 1);
            WhitePixel.SetData(new[] { Color.White });


            donut = new Donut(charaset, new Vector2(_graphicsDevice.Viewport.Width - 96, _graphicsDevice.Viewport.Height - 128), 100f);
            nachoSprite = new Nacho(nacho, nachoOpenMouthTexture, new Vector2(100, 100), 40f);
            empanadaSprite = new Empanada(empanada, new Vector2(200, 200), 60f);
            cheeseProjectile = new Projectile(cheeseLaunch, Vector2.Zero, 300f);

            pipes = new Texture2D[3];
            pipes[0] = pipe;
            pipes[1] = _mainGame.Content.Load<Texture2D>("pipe2");
            pipes[2] = _mainGame.Content.Load<Texture2D>("pipe2");

            pipePositions = new Vector2[]
            {
                new Vector2(670, 570),
                new Vector2(250, 300),
                new Vector2(465, 300),
            };

            currentPipeFrameIndices = new int[3];
            pipeAnimationTimers = new float[3];

            int frameWidth = puprmushSpritesheet.Width / 5;
            int frameHeight = puprmushSpritesheet.Height;
            puprmushFrames = new Rectangle[5];
            for (int i = 0; i < 5; i++)
            {
                puprmushFrames[i] = new Rectangle(i * frameWidth, 0, frameWidth, frameHeight);
            }
            currentPuprmushFrame = 0;
            puprmushFrameTimer = 0f;
        }

        public void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState keyboard = Keyboard.GetState();


            donut.Update(gameTime);
            nachoSprite.Update(gameTime);
            
            Vector2 donutPos = donut.Position;
            float distanceToDonut = Vector2.Distance(empanadaSprite.Position, donutPos);
            if (distanceToDonut <= AttackRange)
            {
                empanadaSprite.StartAttack();
            }
            empanadaSprite.Update(deltaTime, donutPos, nachoSprite.Position);

            Vector2 nachoToEmpanada = empanadaSprite.Position - nachoSprite.Position;
            if (nachoToEmpanada.Length() < MinDistanceBetweenNachoAndEmpanada)
            {
                Vector2 separation = Vector2.Normalize(nachoToEmpanada) * MinDistanceBetweenNachoAndEmpanada;
                nachoSprite.Position = empanadaSprite.Position - separation;
            }

            for (int i = 0; i < pipes.Length; i++)
            {
                pipeAnimationTimers[i] += deltaTime;
                if (pipeAnimationTimers[i] >= pipeFrameDuration)
                {
                    pipeAnimationTimers[i] = 0f;
                    currentPipeFrameIndices[i] = (currentPipeFrameIndices[i] + 1) % 3;
                }
            }

            puprmushFrameTimer += deltaTime;
            if (puprmushFrameTimer >= PuprmushFrameDuration)
            {
                puprmushFrameTimer = 0f;
                currentPuprmushFrame = (currentPuprmushFrame + 1) % 5;
            }

            donut.Position = new Vector2(
                Math.Clamp(donut.Position.X, 48, _graphicsDevice.Viewport.Width - 48),
                Math.Clamp(donut.Position.Y, 64, _graphicsDevice.Viewport.Height - 64)
            );

            nachoSprite.SetTargetPosition(donut.Position);
            empanadaSprite.SetTargetPosition(donut.Position);
            nachoSprite.Update(gameTime);
            empanadaSprite.Update(gameTime);

            float distanceToDonutNacho = Vector2.Distance(nachoSprite.Position, donut.Position);

            // Update cooldown timer
            if (projectileCooldownTimer > 0)
            {
                projectileCooldownTimer -= deltaTime;
            }

            if (distanceToDonutNacho < 200f)
            {
                nachoSprite.SetOpenMouthFrame(true);
                
                if (projectileCooldownTimer <= 0 && !cheeseProjectile.IsActive)
                {
                    Vector2 launchPosition = nachoSprite.Position + new Vector2(0, -40);
                    Vector2 directionToDonut = donut.Position - launchPosition;
                    cheeseProjectile.Launch(launchPosition, directionToDonut);
                    
                    projectileCooldownTimer = ProjectileCooldown;
                }
            }
            else
            {
                nachoSprite.SetOpenMouthFrame(false);
            }

            if (cheeseProjectile.IsActive)
            {
                cheeseProjectile.Update(gameTime);

                Rectangle cheeseRect = new Rectangle(
                    (int)cheeseProjectile.Position.X - 32,
                    (int)cheeseProjectile.Position.Y - 32,
                    64,
                    64
                );

                Rectangle donutRect = new Rectangle(
                    (int)donut.Position.X - 48,
                    (int)donut.Position.Y - 64,
                    96,
                    128
                );

                if (cheeseRect.Intersects(donutRect) && !cheeseProjectile.HasDealtDamage)
                {
                    showSplashEffect = true;
                    cheeseProjectile.Reset();
                    splashPosition = donut.Position;
                    splashTimer = 0f;
                    cheeseProjectile.SetDealtDamage();
                }
            }
            if (showSplashEffect)
            {
                splashTimer += deltaTime;
                if (splashTimer >= SPLASH_DURATION)
                {
                    showSplashEffect = false;
                }
            }

            puprmushFrameTimer += deltaTime;
            if (puprmushFrameTimer >= PuprmushFrameDuration)
            {
                puprmushFrameTimer = 0f;
                currentPuprmushFrame = (currentPuprmushFrame + 1) % puprmushFrames.Length;
            }
        }

        public void Draw(GameTime gameTime)
        {

            _spriteBatch.Draw(
                background,
                new Rectangle(0, 0, 850, 850), 
                Color.White
            );

            _spriteBatch.Draw(
                churroTree,
                new Rectangle(500, 350, 400, 400), 
                Color.White
            );

            _spriteBatch.Draw(
                sombrero,
                sombreroPosition,
                null,
                Color.White,
                0f,
                new Vector2(350, -100),
                1.0f,
                SpriteEffects.None,
                0f
            );

            Vector2 puprmushPosition = new Vector2(
                _graphicsDevice.Viewport.Width / 2 + 20,
                _graphicsDevice.Viewport.Height / 2 - 70
            );

            _spriteBatch.Draw(
                puprmushSpritesheet,
                puprmushPosition,
                puprmushFrames[currentPuprmushFrame],
                Color.White,
                0f,
                new Vector2(puprmushFrames[currentPuprmushFrame].Width / 2, puprmushFrames[currentPuprmushFrame].Height / 2),
                0.18f,
                SpriteEffects.None,
                0f
            );

            donut.Draw(_spriteBatch);
            nachoSprite.Draw(_spriteBatch);
            empanadaSprite.Draw(_spriteBatch);

            if (cheeseProjectile.IsActive)
            {
                cheeseProjectile.Draw(_spriteBatch);
            }

            if (showSplashEffect)
            {
                _spriteBatch.Draw(
                    splashCheese,
                    new Vector2(splashPosition.X - 32, splashPosition.Y - 32),
                    null,
                    Color.White
                );
            }

            int nachoHealthBarWidth = 200;
            int nachoHealthBarHeight = 20;
            Vector2 nachoHealthBarPosition = new Vector2(10, 10);

            if (nachoHealthBarWidth != 0)
            {
                _spriteBatch.Draw(
                    Texture2DHelper.CreateRectangle(_graphicsDevice, nachoHealthBarWidth, nachoHealthBarHeight, Color.Gray),
                    new Rectangle((int)nachoHealthBarPosition.X, (int)nachoHealthBarPosition.Y, nachoHealthBarWidth, nachoHealthBarHeight),
                    Color.Gray
                );
            }
        }

        public static class Texture2DHelper
        {
            public static Texture2D CreateRectangle(GraphicsDevice graphicsDevice, int width, int height, Color color)
            {
                Texture2D texture = new Texture2D(graphicsDevice, width, height);
                Color[] colorData = new Color[width * height];
                for (int i = 0; i < colorData.Length; i++)
                {
                    colorData[i] = color;
                }
                texture.SetData(colorData);
                return texture;
            }
        }
    }
}