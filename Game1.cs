﻿using System;
using System.ComponentModel;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace monogame;

public class Game1 : Game
{
    Texture2D charaset;
    Texture2D nacho;
    Vector2 ballPosition;
    Texture2D cheeseLaunch;
    Texture2D nachoMouth;
    Vector2 nachoPosition;

    Vector2 cheesePosition;

    Texture2D sombreroWallpaper;
    float ballSpeed;
    float nachoSpeed = 40f;
    float nachoRotation = 0f;
    bool rotatingRight = true;
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    float health;
    float timer;
    int threshold;
    private SpriteFont font;

    Rectangle[] downRectangles;
    Rectangle[] upRectangles;
    Rectangle[] leftRectangles;
    Rectangle[] rightRectangles;

    Vector2 lastDonutPosition;

    bool cheeseVisible = true;

    byte currentAnimationIndex;

    enum Direction { Down, Up, Left, Right }
    Direction currentDirection;

    float cheeseDisableTimer = 0f;

    bool hasCheeseDealtDamage = false;
    float cheeseRotation = 0f;

    int animationCycleCount = 0;
    bool useBlinkingFrame = false;

    bool useSpacebarFrame = false;

    bool useOpenMouthFrame = false;

    private bool isSpacebarAnimationActive = false;
    private float spacebarAnimationTimer = 0f;
    private float spacebarFirstFrameDuration = 0.5f;
    private float spacebarSecondFrameDuration = 0.8f;
    private int doubleWidth = 192;
    float nachoHealth;
    private bool nachoDamagedThisCycle = false;
    private KeyboardState previousKeyboardState;


    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {

        _graphics.PreferredBackBufferWidth = 650;
        _graphics.PreferredBackBufferHeight = 650;
        _graphics.ApplyChanges();
        ballPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2,
                                   _graphics.PreferredBackBufferHeight / 2);
        nachoPosition = new Vector2(100, 100);
        ballSpeed = 100f;
        lastDonutPosition = ballPosition;

        nachoHealth = 4;
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        charaset = Content.Load<Texture2D>("donutsprites17");
        nacho = Content.Load<Texture2D>("nachosprites2");
        font = Content.Load<SpriteFont>("DefaultFont1");
        cheeseLaunch = Content.Load<Texture2D>("cheeselaunch");
        nachoMouth = Content.Load<Texture2D>("openmountnacho2");
        sombreroWallpaper = Content.Load<Texture2D>("sombrerosetting");


        health = 4;

        timer = 0;
        threshold = 150;

        downRectangles = new Rectangle[3]
        {
            new Rectangle(0, 256, 96, 128),  // First frame in the Down row
            new Rectangle(96, 256, 96, 128), // Second frame
            new Rectangle(192, 256, 96, 128) // Third frame
        };

        upRectangles = new Rectangle[3]
        {
            new Rectangle(0, 0, 96, 128),    // First frame in the Up row
            new Rectangle(96, 0, 96, 128),  // Second frame
            new Rectangle(192, 0, 96, 128)  // Third frame
        };

        rightRectangles = new Rectangle[3]
        {
            new Rectangle(0, 128, 96, 128),  // First frame in the Right row
            new Rectangle(96, 128, 96, 128), // Second frame
            new Rectangle(192, 128, 96, 128) // Third frame
        };

        leftRectangles = new Rectangle[3]
        {
            new Rectangle(0, 384, 96, 128),  // First frame in the Left row
            new Rectangle(96, 384, 96, 128), // Second frame
            new Rectangle(192, 384, 96, 128) // Third frame
        };

        currentAnimationIndex = 1;
        currentDirection = Direction.Down;
    }

    private Vector2 GetNachoMouthPosition()
    {
        Rectangle currentRect = GetCurrentRectanglesNacho()[currentAnimationIndex];

        Vector2 mouthOffset = new Vector2(currentRect.Width / 2, currentRect.Height - 20); // Adjust `-20` for your mouth's exact position.

        float sin = (float)Math.Sin(nachoRotation);
        float cos = (float)Math.Cos(nachoRotation);

        Vector2 rotatedOffset = new Vector2(
            mouthOffset.X * cos - mouthOffset.Y * sin,
            mouthOffset.X * sin + mouthOffset.Y * cos
        );

        return nachoPosition + rotatedOffset;
    }


    private void cheeseLauncher(float updatedNachoSpeed, GameTime gameTime)
    {
        int cheeseWidth = 20;
        int cheeseHeight = 20;

        float distanceToDonut = Vector2.Distance(nachoPosition, ballPosition);

        if (distanceToDonut <= 150)
        {
            if (!cheeseVisible)
            {
                cheesePosition = GetNachoMouthPosition();
                cheeseVisible = true;
            }

            Vector2 directionToDonut = ballPosition - cheesePosition;
            if (directionToDonut != Vector2.Zero && directionToDonut.LengthSquared() > 1f) // Prevent small movements
            {
                directionToDonut.Normalize();
                cheeseRotation = (float)Math.Atan2(directionToDonut.Y, directionToDonut.X);
                cheesePosition += directionToDonut * updatedNachoSpeed * 1.5f;
            }


            Rectangle cheeseRect = new Rectangle(
                (int)cheesePosition.X - cheeseWidth / 2,
                (int)cheesePosition.Y - cheeseHeight / 2,
                cheeseWidth,
                cheeseHeight
            );

            Rectangle donutRect = new Rectangle(
                (int)ballPosition.X - 48,
                (int)ballPosition.Y - 64,
                96,
                128
            );

            if (cheeseRect.Intersects(donutRect) && !hasCheeseDealtDamage)
            {
                cheeseVisible = false;
                health -= 0.5f;
                hasCheeseDealtDamage = true;
            }
            if (distanceToDonut <= 50)
            {
                cheeseVisible = false;

            }
        }
        else
        {
            cheeseVisible = false;
            hasCheeseDealtDamage = false;
        }
    }



    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        float updatedBallSpeed = ballSpeed * elapsedTime;
        float updatedNachoSpeed = nachoSpeed * elapsedTime;

        var kstate = Keyboard.GetState();
        KeyboardState currentKeyboardState = Keyboard.GetState();

        bool isMoving = false;

        Rectangle donutRect = new Rectangle(
            (int)ballPosition.X - 48,
            (int)ballPosition.Y - 64,
            96,
            128
            );

        Rectangle nachoRect = new Rectangle(
            (int)nachoPosition.X - 48,
            (int)nachoPosition.Y - 64,
                96,
                128
                );

        if (currentKeyboardState.IsKeyDown(Keys.Space) && !previousKeyboardState.IsKeyDown(Keys.Space))
        {
            if (!isSpacebarAnimationActive)
            {
                isSpacebarAnimationActive = true;
                spacebarAnimationTimer = 0f;
                nachoDamagedThisCycle = false; // Reset damage flag
            }
        }

        if (isSpacebarAnimationActive)
        {
            spacebarAnimationTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (spacebarAnimationTimer <= spacebarFirstFrameDuration)
            {
                useSpacebarFrame = true;
            }
            else if (spacebarAnimationTimer <= spacebarFirstFrameDuration + spacebarSecondFrameDuration)
            {
                useSpacebarFrame = false;
            }
            else
            {
                spacebarAnimationTimer = 0f;
                isSpacebarAnimationActive = false; // End the animation
            }
        }

        Vector2 movement = Vector2.Zero;

        if (kstate.IsKeyDown(Keys.Up))
        {
            movement.Y -= 1;
            currentDirection = Direction.Up;
            isMoving = true;
        }
        if (kstate.IsKeyDown(Keys.Down))
        {
            movement.Y += 1;
            currentDirection = Direction.Down;
            isMoving = true;
        }
        if (kstate.IsKeyDown(Keys.Left))
        {
            movement.X -= 1;
            currentDirection = Direction.Left;
            isMoving = true;
        }
        if (kstate.IsKeyDown(Keys.Right))
        {
            movement.X += 1;
            currentDirection = Direction.Right;
            isMoving = true;
        }

        if (movement != Vector2.Zero)
        {
            movement.Normalize();
            ballPosition += movement * updatedBallSpeed;
        }


        Rectangle currentRect = GetCurrentRectangles()[currentAnimationIndex];
        ballPosition.X = MathHelper.Clamp(ballPosition.X, currentRect.Width / 2, _graphics.PreferredBackBufferWidth - currentRect.Width / 2);
        ballPosition.Y = MathHelper.Clamp(ballPosition.Y, currentRect.Height / 2, _graphics.PreferredBackBufferHeight - currentRect.Height / 2);

        Vector2 directionToDonut = ballPosition - nachoPosition;
        if (directionToDonut != Vector2.Zero)
        {
            directionToDonut.Normalize();
            nachoPosition += directionToDonut * updatedNachoSpeed;
        }

        nachoPosition.X = MathHelper.Clamp(nachoPosition.X, nacho.Width / 2, _graphics.PreferredBackBufferWidth - nacho.Width / 2);
        nachoPosition.Y = MathHelper.Clamp(nachoPosition.Y, nacho.Height / 2, _graphics.PreferredBackBufferHeight - nacho.Height / 2);


        if (rotatingRight)
        {
            nachoRotation += 0.05f;
            if (nachoRotation >= 0.3f)
            {
                rotatingRight = false;
            }
        }
        else
        {
            nachoRotation -= 0.05f;
            if (nachoRotation <= -0.3f)
            {
                rotatingRight = true;
            }
        }

        if (isMoving || isSpacebarAnimationActive)
        {
            if (timer > threshold)
            {
                currentAnimationIndex = (byte)((currentAnimationIndex + 1) % 3);

                if (currentAnimationIndex == 0)
                {
                    animationCycleCount++;

                    if (animationCycleCount % 3 == 0)
                    {
                        useBlinkingFrame = true;
                    }
                    else
                    {
                        useBlinkingFrame = false;
                    }
                }

                timer = 0;
            }
            else
            {
                timer += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
            }
        }
        else
        {
            currentAnimationIndex = 1;
        }

        cheeseLauncher(updatedNachoSpeed, gameTime);


        if (useSpacebarFrame && donutRect.Intersects(nachoRect) && !nachoDamagedThisCycle)
        {
            nachoHealth = Math.Max(0, nachoHealth - 1);
            nachoDamagedThisCycle = true;
        }

        previousKeyboardState = currentKeyboardState;


        base.Update(gameTime);
    }


    protected override void Draw(GameTime gameTime)
    {
        _graphics.GraphicsDevice.Clear(Color.Tan);
        _spriteBatch.Begin();

        _spriteBatch.Draw(
      sombreroWallpaper,
      new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight),
      Color.White
    );
        _spriteBatch.Draw(nacho, nachoPosition, GetCurrentRectanglesNacho()[currentAnimationIndex], Color.White);

        _spriteBatch.Draw(charaset, ballPosition, GetCurrentRectangles()[currentAnimationIndex], Color.White);

        if (Vector2.Distance(nachoPosition, ballPosition) <= 150)
        {
            useOpenMouthFrame = true;
        }
        else
        {
            useOpenMouthFrame = false;

        }
        if (cheeseVisible)
        {
            _spriteBatch.Draw(
                cheeseLaunch,
                cheesePosition,
                null,
                Color.White,
                cheeseRotation,
                new Vector2(cheeseLaunch.Width / 2, cheeseLaunch.Height / 2),
                1.0f,
                SpriteEffects.None,
                0f
            );
        }

        float maxNachoHealth = 4f;
        int nachoHealthBarWidth = 200;
        int nachoHealthBarHeight = 20;
        Vector2 nachoHealthBarPosition = new Vector2(10, 10);

        if (nachoHealthBarWidth != 0)
        {
            _spriteBatch.Draw(
                Texture2DHelper.CreateRectangle(GraphicsDevice, nachoHealthBarWidth, nachoHealthBarHeight, Color.Gray),
                new Rectangle((int)nachoHealthBarPosition.X, (int)nachoHealthBarPosition.Y, nachoHealthBarWidth, nachoHealthBarHeight),
                Color.Gray
            );
        }

        int nachoHealthCurrentWidth = (int)((nachoHealth / maxNachoHealth) * nachoHealthBarWidth);
        if (nachoHealthCurrentWidth != 0)
        {
            _spriteBatch.Draw(
                Texture2DHelper.CreateRectangle(GraphicsDevice, nachoHealthCurrentWidth, nachoHealthBarHeight, Color.WhiteSmoke),
                new Rectangle((int)nachoHealthBarPosition.X, (int)nachoHealthBarPosition.Y, nachoHealthCurrentWidth, nachoHealthBarHeight),
                Color.WhiteSmoke
            );
        }

        string donutHealthText = $"Health: {health}";
        Vector2 donutHealthPosition = new Vector2(_graphics.PreferredBackBufferWidth - font.MeasureString(donutHealthText).X - 10, 10);
        _spriteBatch.DrawString(font, donutHealthText, donutHealthPosition, Color.Black);

        _spriteBatch.End();
        base.Draw(gameTime);
    }


    private Rectangle[] GetCurrentRectanglesNacho()
    {
        Rectangle[] baseRectangles = currentDirection switch
        {
            Direction.Up => new Rectangle[]
            {
            new Rectangle(0, 0, 96, 128),
            new Rectangle(0, 0, 96, 128),
            new Rectangle(0, 0, 96, 128)
            },
            Direction.Down => new Rectangle[]
            {
            useOpenMouthFrame
                ? new Rectangle(96, 128, 96, 128)
                : new Rectangle(96, 256, 96, 128),
            useOpenMouthFrame
                ? new Rectangle(96, 128, 96, 128)
                : new Rectangle(96, 256, 96, 128),
            useOpenMouthFrame
                ? new Rectangle(96, 128, 96, 128)
                : new Rectangle(96, 256, 96, 128)
            },
            Direction.Left => new Rectangle[]
            {
            useOpenMouthFrame
                ? new Rectangle(192, 128, 96, 128)
                : new Rectangle(192, 256, 96, 128),
            useOpenMouthFrame
                ? new Rectangle(192, 128, 96, 128)
                : new Rectangle(192, 256, 96, 128),
            useOpenMouthFrame
                ? new Rectangle(192, 128, 96, 128)
                : new Rectangle(192, 256, 96, 128)
            },
            Direction.Right => new Rectangle[]
            {
            useOpenMouthFrame
                ? new Rectangle(0, 128, 96, 128)
                : new Rectangle(0, 256, 96, 128),
            useOpenMouthFrame
                ? new Rectangle(0, 128, 96, 128)
                : new Rectangle(0, 256, 96, 128),
            useOpenMouthFrame
                ? new Rectangle(0, 128, 96, 128)
                : new Rectangle(0, 256, 96, 128)
            },
            _ => new Rectangle[]
            {
            new Rectangle(0, 384, 96, 128),
            new Rectangle(96, 384, 96, 128),
            new Rectangle(192, 384, 96, 128)
            },
        };

        if (useBlinkingFrame && currentDirection != Direction.Up)
        {
            baseRectangles[2] = new Rectangle(baseRectangles[2].X, 384, 96, 128);
        }

        return baseRectangles;
    }


    private Rectangle[] GetCurrentRectangles()
    {
        Rectangle[] baseRectangles = currentDirection switch
        {
            Direction.Up => new Rectangle[]
            {
            isSpacebarAnimationActive
                ? (useSpacebarFrame
                    ? new Rectangle(384, 0, 96, 128)  // Fifth column
                    : new Rectangle(480, 0, doubleWidth, 128))  // Sixth column
                : new Rectangle(0, 0, 96, 128),
            new Rectangle(96, 0, 96, 128),
            new Rectangle(192, 0, 96, 128)
            },
            Direction.Down => new Rectangle[]
            {
            isSpacebarAnimationActive
                ? (useSpacebarFrame
                    ? new Rectangle(384, 256, 96, 128) // Fifth column
                    : new Rectangle(480, 256, doubleWidth, 128)) // Sixth column
                : new Rectangle(0, 256, 96, 128),
            new Rectangle(96, 256, 96, 128),
            new Rectangle(192, 256, 96, 128)
            },
            Direction.Left => new Rectangle[]
            {
            isSpacebarAnimationActive
                ? (useSpacebarFrame
                    ? new Rectangle(384, 384, 96, 128) // Fifth column
                    : new Rectangle(480, 384, doubleWidth, 128)) // Sixth column
                : new Rectangle(0, 384, 96, 128),
            new Rectangle(96, 384, 96, 128),
            new Rectangle(192, 384, 96, 128)
            },
            Direction.Right => new Rectangle[]
            {
            isSpacebarAnimationActive
                ? (useSpacebarFrame
                    ? new Rectangle(384, 128, 96, 128) // Fifth column
                    : new Rectangle(480, 128, doubleWidth, 128)) // Sixth column
                : new Rectangle(0, 128, 96, 128),
            new Rectangle(96, 128, 96, 128),
            new Rectangle(192, 128, 96, 128)
            },
            _ => new Rectangle[]
            {
            new Rectangle(0, 128, 96, 128),
            new Rectangle(96, 128, 96, 128),
            new Rectangle(192, 128, 96, 128)
            },
        };

        if (useBlinkingFrame && !isSpacebarAnimationActive)
        {
            baseRectangles[2] = new Rectangle(288, baseRectangles[2].Y, 96, 128); // Blinking frame
        }

        return baseRectangles;
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