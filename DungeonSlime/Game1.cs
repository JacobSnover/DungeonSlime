using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;

namespace DungeonSlime;

public class Game1 : Core
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _logo;
    private int x = 90;
    private int y = 90;
    private int _horDirection = 1;
    private int _verDirection = 0;

    public Game1()
        : base("Dungeon Slime", 1280, 720, false)
    {
       
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        // TODO: use this.Content to load your game content here
        _logo = Content.Load<Texture2D>("Images/logo");

        base.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        if (x >= 140 && y >= 140)
        {
            _horDirection = -1;
            _verDirection = 0;
        }
        else if (x <= 90 && y <= 90)
        {
            _horDirection = 1;
            _verDirection = 0;
        }
        else if(x >= 140 && y <= 90)
        {
            _horDirection = 0;
            _verDirection = 1;
        }
        else if (x <= 90 && y >= 140)
        {
            _horDirection = 0;
            _verDirection = -1;
        }

        x += _horDirection;
        y += _verDirection;

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        // TODO: Add your drawing code here
        SpriteBatch.Begin();

        


        SpriteBatch.Draw(_logo, new Vector2(x, y), Color.YellowGreen);
        SpriteBatch.End();

        base.Draw(gameTime);
    }
}
