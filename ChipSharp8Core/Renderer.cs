using System.Collections;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ChipSharp8.ChipSharp8Core
{
    public class Renderer : Game
    {
        private GraphicsDeviceManager _graphics;
        private GraphicsAdapter _graphicsAdapter;
        private SpriteBatch _spriteBatch;
        private RenderTarget2D _renderTarget;
        private Texture2D _squarePixel;
        private int _scaledX;
        private int _scaledY;
        private CPU _cpu;
        private ALU _alu;
        public static Dictionary<Keys, byte> _keyboard;

        public Renderer()
        { 
            _graphics = new GraphicsDeviceManager(this);
            _graphicsAdapter = new GraphicsAdapter();
            _graphics.PreferredBackBufferHeight = _graphicsAdapter.CurrentDisplayMode.Height / 2;
            _graphics.PreferredBackBufferWidth = _graphicsAdapter.CurrentDisplayMode.Width / 2;
            _graphics.IsFullScreen = false;

            Content.RootDirectory = "Content";
            IsFixedTimeStep = true;
            TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 360.0);
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _scaledY = (int)(_graphics.PreferredBackBufferHeight / CPU.MaxHeight);
            _scaledX = (int)(_graphics.PreferredBackBufferWidth / CPU.MaxWidth);

            Console.WriteLine($"{_scaledX}, {_scaledY}");

            _cpu = new CPU();
            _alu = new(_cpu);

            _keyboard = new Dictionary<Keys, byte>() {
                { Keys.D1, 1 },
                { Keys.D2, 2 },
                { Keys.D3, 3 },
                { Keys.D4, 4 },
                { Keys.Q, 5 },
                { Keys.W, 6 },
                { Keys.E, 7 },
                { Keys.R, 8 },
                { Keys.A, 9 },
                { Keys.S, 0 },
                { Keys.D, 10 },
                { Keys.F, 11 },
                { Keys.Z, 12 },
                { Keys.X, 13 },
                { Keys.C, 14 },
                { Keys.V, 15 }
            };

            _spriteBatch = new SpriteBatch(GraphicsDevice);
            base.Initialize();
        }

        protected void DrawScreen()
        {
            for (int y = 0; y < CPU.MaxHeight; y++)
            {
                for (int x = 0; x < CPU.MaxWidth; x++)
                {
                    Vector2 position = new Vector2((float)(x * _scaledX), (float)(y * _scaledY));

                    if (_cpu.display[x, y] > 0)
                    {
                        _spriteBatch.Draw(CreatePixel(), position, Color.Aquamarine);
                    }
                    else
                    {
                        _spriteBatch.Draw(CreatePixel(), position, Color.Black);
                    }
                }
            }
        }

        protected Texture2D CreatePixel()
        {
             _squarePixel = new Texture2D(GraphicsDevice, _scaledX, _scaledY);
            Color[] colorData = new Color[_scaledX * _scaledY];

            for (int i = 0; i < colorData.Length; i++)
            {
                colorData[i] = Color.White;
            }
            _squarePixel.SetData(colorData);
            return _squarePixel;
        }

        protected override void LoadContent()
        {
        
        }

        protected override void Update(GameTime gameTime)
        {   
            _cpu.CycleCPU(_alu);

            KeyboardState keyState = Keyboard.GetState();

            foreach (var key in _keyboard)
            {
                if (keyState.IsKeyDown(key.Key)) {

                    _cpu.keyboard[key.Value] = true;

                    Console.WriteLine($"Debug: {key.Key} down -> _cpu.keyboard[0x{key.Value, 0:X2}] {_cpu.keyboard[key.Value]}");

                    break;       
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            _spriteBatch.Begin();
            DrawScreen();
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
