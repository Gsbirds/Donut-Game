using System;
using Microsoft.Xna.Framework;

namespace monogame.Animation
{
    public enum Direction { Down, Up, Left, Right }

    public static class AnimationFrames
    {
        public static Rectangle[] GetCurrentRectanglesNacho(Direction nachoFacingDirection, bool usePostHitFrame, bool useOpenMouthFrame, bool useBlinkingFrame)
        {
            Rectangle[] baseRectangles = nachoFacingDirection switch
            {
                Direction.Up => new Rectangle[]
                {
                    new Rectangle(0, 0, 96, 128),
                    new Rectangle(0, 0, 96, 128),
                    new Rectangle(0, 0, 96, 128)
                },
                Direction.Down => new Rectangle[]
                {
                    usePostHitFrame ? new Rectangle(96, 512, 96, 128)
                                    : (useOpenMouthFrame ? new Rectangle(96, 128, 96, 128)
                                                         : new Rectangle(96, 256, 96, 128)),
                    usePostHitFrame ? new Rectangle(96, 512, 96, 128)
                                    : (useOpenMouthFrame ? new Rectangle(96, 128, 96, 128)
                                                         : new Rectangle(96, 256, 96, 128)),
                    usePostHitFrame ? new Rectangle(96, 512, 96, 128)
                                    : (useOpenMouthFrame ? new Rectangle(96, 128, 96, 128)
                                                         : new Rectangle(96, 256, 96, 128))
                },
                Direction.Left => new Rectangle[]
                {
                    usePostHitFrame ? new Rectangle(0, 512, 96, 128)
                                    : (useOpenMouthFrame ? new Rectangle(192, 128, 96, 128)
                                                         : new Rectangle(192, 256, 96, 128)),
                    usePostHitFrame ? new Rectangle(0, 512, 96, 128)
                                    : (useOpenMouthFrame ? new Rectangle(192, 128, 96, 128)
                                                         : new Rectangle(192, 256, 96, 128)),
                    usePostHitFrame ? new Rectangle(0, 512, 96, 128)
                                    : (useOpenMouthFrame ? new Rectangle(192, 128, 96, 128)
                                                         : new Rectangle(192, 256, 96, 128))
                },
                Direction.Right => new Rectangle[]
                {
                    usePostHitFrame ? new Rectangle(192, 512, 96, 128)
                                    : (useOpenMouthFrame ? new Rectangle(0, 128, 96, 128)
                                                         : new Rectangle(0, 256, 96, 128)),
                    usePostHitFrame ? new Rectangle(192, 512, 96, 128)
                                    : (useOpenMouthFrame ? new Rectangle(0, 128, 96, 128)
                                                         : new Rectangle(0, 256, 96, 128)),
                    usePostHitFrame ? new Rectangle(192, 512, 96, 128)
                                    : (useOpenMouthFrame ? new Rectangle(0, 128, 96, 128)
                                                         : new Rectangle(0, 256, 96, 128))
                },
                _ => new Rectangle[]
                {
                    new Rectangle(0, 384, 96, 128),
                    new Rectangle(96, 384, 96, 128),
                    new Rectangle(192, 384, 96, 128)
                },
            };

            if (useBlinkingFrame && !usePostHitFrame && nachoFacingDirection != Direction.Up)
            {
                baseRectangles[2] = new Rectangle(baseRectangles[2].X, 384, 96, 128);
            }

            return baseRectangles;
        }

        public static Rectangle[][] GetCharacterFrames(Direction down)
        {
            return new Rectangle[][]
            {
                // Down frames
                new Rectangle[]
                {
                    new Rectangle(0, 256, 96, 128),
                    new Rectangle(96, 256, 96, 128),
                    new Rectangle(192, 256, 96, 128)
                },
                // Up frames
                new Rectangle[]
                {
                    new Rectangle(0, 0, 96, 128),
                    new Rectangle(96, 0, 96, 128),
                    new Rectangle(192, 0, 96, 128)
                },
                // Right frames
                new Rectangle[]
                {
                    new Rectangle(0, 128, 96, 128),
                    new Rectangle(96, 128, 96, 128),
                    new Rectangle(192, 128, 96, 128)
                },
                // Left frames
                new Rectangle[]
                {
                    new Rectangle(0, 384, 96, 128),
                    new Rectangle(96, 384, 96, 128),
                    new Rectangle(192, 384, 96, 128)
                }
            };
        }

        public static Rectangle[] GetPurpleMushroomFrames(int spriteWidth, int spriteHeight)
        {
            int frameWidth = spriteWidth / 5;
            Rectangle[] frames = new Rectangle[5];
            for (int i = 0; i < 5; i++)
            {
                frames[i] = new Rectangle(i * frameWidth, 0, frameWidth, spriteHeight);
            }
            return frames;
        }

        internal static object GetCurrentRectanglesNacho(object nachoFacingDirection, bool usePostHitFrame, bool useOpenMouthFrame, bool useBlinkingFrame)
        {
            throw new NotImplementedException();
        }
    }
}
