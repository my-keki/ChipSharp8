using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Renderer : Game
{
    private GraphicsDeviceManager _graphics;
    private GraphicsAdapter _graphicsAdapter;
    private SpriteBatch _spriteBatch;
    private Texture2D _squarePixel;
    private int _scaledX;
    private int _scaledY;

    public Renderer()
    {
        _graphics = new GraphicsDeviceManager(this);

        _graphicsAdapter = new GraphicsAdapter();

        _graphics.PreferredBackBufferHeight = _graphicsAdapter.CurrentDisplayMode.Height / 2;

        _graphics.PreferredBackBufferWidth = _graphicsAdapter.CurrentDisplayMode.Width / 2;

        _graphics.IsFullScreen = false;

        Content.RootDirectory = "Content";

        IsMouseVisible = true;
    }

    protected override void Initialize()
    {   
        CPU cpu = new CPU();

        ALU alu = new ALU(cpu);

        _scaledY = (int)(_graphics.PreferredBackBufferHeight / CPU.maxHeight);

        _scaledX = (int)(_graphics.PreferredBackBufferWidth / CPU.maxWidth);

        Console.WriteLine($"{_scaledX}, {_scaledY}");

        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _squarePixel = new Texture2D(GraphicsDevice, _scaledX, _scaledY);

        Color[] colorData = new Color[_scaledX * _scaledY];

        for (int i = 0; i < colorData.Length; i++)
        {
            colorData[i] = Color.White;
        }

        _squarePixel.SetData(colorData);
    }

    protected override void Update(GameTime gameTime)
    {
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();

        DrawGrid();

        _spriteBatch.End();

        base.Draw(gameTime);    
    }

    private void DrawGrid()
    {
        // Loop through rows and columns to draw squares
        for (int y = 0; y < CPU.maxHeight; y++)
        {
            for (int x = 0; x < CPU.maxWidth; x++)
            {
                Vector2 position = new Vector2((float)(x * _scaledX), (float)(y * _scaledY));

                if (x % 2 == 0) 
                {
                    _spriteBatch.Draw(_squarePixel, position, Color.White);
                } else
                {
                    _spriteBatch.Draw(_squarePixel, position, Color.AliceBlue);
                }       
            }
        }
    }
}
