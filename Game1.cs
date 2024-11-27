using System;
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
    bool isCheeseTimerActive = false;

    bool hasCheeseDealtDamage = false;


    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        ballPosition = new Vector2(_graphics.PreferredBackBufferWidth / 2,
                                   _graphics.PreferredBackBufferHeight / 2);
        nachoPosition = new Vector2(100, 100);
        ballSpeed = 100f;
        lastDonutPosition = ballPosition;
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        charaset = Content.Load<Texture2D>("donutsprite9");
        nacho = Content.Load<Texture2D>("nachofrontstant2");
        font = Content.Load<SpriteFont>("DefaultFont1");
        cheeseLaunch = Content.Load<Texture2D>("cheeselaunch");
        nachoMouth = Content.Load<Texture2D>("openmountnacho2");

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

        currentAnimationIndex = 1; // Start with the middle frame
        currentDirection = Direction.Down;
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
        bool isMoving = false;

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

        int cheeseWidth = 20;
        int cheeseHeight = 20;

        Rectangle cheeseRect = new Rectangle(
            (int)nachoPosition.X - cheeseWidth / 2,
            (int)nachoPosition.Y - cheeseHeight / 2,
            cheeseWidth,
            cheeseHeight
        );

        Rectangle donutRect = new Rectangle(
            (int)ballPosition.X - currentRect.Width / 2,
            (int)ballPosition.Y - currentRect.Height / 2,
            currentRect.Width,
            currentRect.Height
        );

        if (cheeseRect.Intersects(donutRect) && !hasCheeseDealtDamage)
        {
            health -= 0.5f;
            cheeseVisible = false;
            hasCheeseDealtDamage = true;
        }

        cheeseDisableTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (cheeseDisableTimer >= 2f)
        {
            cheeseVisible = true;
            hasCheeseDealtDamage = false;
            cheeseDisableTimer = 0f;
        }

        if (isMoving)
        {
            if (timer > threshold)
            {
                currentAnimationIndex = (byte)((currentAnimationIndex + 1) % 3);
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

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        _graphics.GraphicsDevice.Clear(Color.CornflowerBlue);
        _spriteBatch.Begin();

        _spriteBatch.Draw(charaset, ballPosition, GetCurrentRectangles()[currentAnimationIndex], Color.White);

        if (Vector2.Distance(nachoPosition, ballPosition) >= 100)
        {
            _spriteBatch.Draw(
                nacho,
                nachoPosition,
                null,
                Color.White,
                nachoRotation,
                new Vector2(nacho.Width / 2, nacho.Height / 2),
                1.0f,
                SpriteEffects.None,
                0f
            );
        }

        if (Vector2.Distance(nachoPosition, ballPosition) <= 200)
        {
            _spriteBatch.Draw(
                nachoMouth,
                nachoPosition,
                null,
                Color.White,
                nachoRotation,
                new Vector2(nacho.Width / 2, nacho.Height / 2),
                1.0f,
                SpriteEffects.None,
                0f
            );


            if (cheeseVisible)
            {
                _spriteBatch.Draw(
                    cheeseLaunch,
                    new Vector2(nachoPosition.X - 20, nachoPosition.Y - 80),
                    Color.White
                );
            }
        }

        string healthText = $"Health: {health}";
        Vector2 healthPosition = new Vector2(_graphics.PreferredBackBufferWidth - font.MeasureString(healthText).X - 10, 10);
        _spriteBatch.DrawString(font, healthText, healthPosition, Color.Black);

        _spriteBatch.End();
        base.Draw(gameTime);
    }


    private Rectangle[] GetCurrentRectangles()
    {
        return currentDirection switch
        {
            Direction.Up => upRectangles,
            Direction.Down => downRectangles,
            Direction.Left => leftRectangles,
            Direction.Right => rightRectangles,
            _ => downRectangles,
        };
    }
}