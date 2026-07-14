using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameLibrary;
using MonoGameLibrary.Graphics;
using System.Collections.Generic;
using static System.Net.Mime.MediaTypeNames;

namespace DungeonSlime;

public class Game1 : Core
{
    //private GraphicsDeviceManager _graphics;
    //private SpriteBatch _spriteBatch;
    //private Texture2D _logo;
    //private int x = 90;
    //private int y = 90;
    //private int _horDirection = 1;
    //private int _verDirection = 0;
    //private int _angle = 0;
    //private float _scale = 0.5f;
    //private float _scaleX = 0.5f;
    //private float _scaleY = 1.5f;
    //private float _scaleModifier = 0.1f;
    //private float _opacity = 0.0f;

    //private TextureRegion _slime;
    //private TextureRegion _bat;
    //private Sprite _slime;
    //private Sprite _bat;
    private AnimatedSprite _slime;
    private AnimatedSprite _bat;
    private Vector2 _slimePosition;
    private const float MOVE_SPEED = 5.0f; // speed multiplier

    // Use a queue directly for input buffering
    private Queue<Vector2> _inputBuffer;
    private const int MAX_BUFFER_SIZE = 2;

    public Game1()
        : base("Dungeon Slime", 1280, 720, false)
    {

    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        _inputBuffer = new Queue<Vector2>(MAX_BUFFER_SIZE);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        // TODO: use this.Content to load your game content here
        //_logo = Content.Load<Texture2D>("Images/logo");

        //Texture2D atlasTexture = Content.Load<Texture2D>("Images/atlas");
        //TextureAtlas atlas = new TextureAtlas(atlasTexture);
        //atlas.AddRegion("slime", 0, 0, 20, 20);
        //atlas.AddRegion("bat", 20, 0, 20, 20);

        // Create the texture atlas from the XML configuration file
        TextureAtlas atlas = TextureAtlas.FromFile(Content, @"Images\atlas-definition.xml");

        // Create the slime animated sprite from the atlas.
        _slime = atlas.CreateAnimatedSprite("slime-animation");
        _slime.Scale = new Vector2(4.0f, 4.0f);

        // Create the bat animated sprite from the atlas.
        _bat = atlas.CreateAnimatedSprite("bat-animation");
        _bat.Scale = new Vector2(4.0f, 4.0f);

        //// Create the slime sprite from the atlas.
        //_slime = atlas.CreateSprite("slime");
        //_slime.Scale = new Vector2(4.0f, 4.0f);

        //// Create the bat sprite from the atlas.
        //_bat = atlas.CreateSprite("bat");
        //_bat.Scale = new Vector2(4.0f, 4.0f);

        //_slime = atlas.GetRegion("slime");
        //_bat = atlas.GetRegion("bat");

        //base.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here
        //if (x >= 140 && y >= 140)
        //{
        //    _horDirection = -1;
        //    _verDirection = 0;
        //}
        //else if (x <= 90 && y <= 90)
        //{
        //    _horDirection = 1;
        //    _verDirection = 0;
        //}
        //else if(x >= 140 && y <= 90)
        //{
        //    _horDirection = 0;
        //    _verDirection = 1;
        //}
        //else if (x <= 90 && y >= 140)
        //{
        //    _horDirection = 0;
        //    _verDirection = -1;
        //}

        //x += _horDirection;
        //y += _verDirection;

        // rotate the logo by 1 degree each frame
        //if (_angle == 360)
        //{
        //    _angle = 0;
        //}
        //else
        //{
        //    _angle += 1;
        //}

        // scale the logo up and down between 0.5 and 1.5
        //if (_scaleX >= 1.5f)
        //{
        //    _scaleModifier = -0.01f;
        //}
        //else if (_scaleX <= 0.5f)
        //{
        //    _scaleModifier = 0.01f;
        //}

        //_scaleX += _scaleModifier;
        //_scaleY += -_scaleModifier;
        //_opacity += _scaleModifier;

        _slime.Update(gameTime);
        _bat.Update(gameTime);

        CheckKeyboardInput();
        CheckGamePadInput();

        base.Update(gameTime);
    }

    private void CheckKeyboardInput()
    {
        KeyboardState keyboardState = Keyboard.GetState();
        Vector2 newDirection = Vector2.Zero;

        float speed = MOVE_SPEED;
        if (keyboardState.IsKeyDown(Keys.Space))
        {
            speed *= 1.5f;            
        }

        if (keyboardState.IsKeyDown(Keys.Up) || keyboardState.IsKeyDown(Keys.W))
        {
            //_slimePosition.Y -= speed;
            //newDirection = -Vector2.UnitY;
            _inputBuffer.Enqueue(-Vector2.UnitY);
        }
        if (keyboardState.IsKeyDown(Keys.Down) || keyboardState.IsKeyDown(Keys.S))
        {
            //_slimePosition.Y += speed;
            //newDirection = Vector2.UnitY;
            _inputBuffer.Enqueue(Vector2.UnitY);
        }
        if (keyboardState.IsKeyDown(Keys.Left) || keyboardState.IsKeyDown(Keys.A))
        {
            //_slimePosition.X -= speed;
            //newDirection = -Vector2.UnitX;
            _inputBuffer.Enqueue(-Vector2.UnitX);
        }
        if (keyboardState.IsKeyDown(Keys.Right) || keyboardState.IsKeyDown(Keys.D))
        {
            //_slimePosition.X += speed;
            //newDirection = Vector2.UnitX;
            _inputBuffer.Enqueue(Vector2.UnitX);            
        }

        // Only add if a valid direction and does not exceed the buffer size.
        //if (newDirection != Vector2.Zero && _inputBuffer.Count < MAX_BUFFER_SIZE)
        //{
        //    _inputBuffer.Enqueue(newDirection);
        //}

        // In movement update code.
        while (_inputBuffer.Count > 0)
        {
            Vector2 nextDirection = _inputBuffer.Dequeue();
            _slimePosition += nextDirection * speed;
        }
    }

    private void CheckGamePadInput()
    {
        GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

        float speed = MOVE_SPEED;
        if (gamePadState.IsButtonDown(Buttons.A))
        {
            speed *= 1.5f;
            GamePad.SetVibration(PlayerIndex.One, 1.0f, 1.0f);
        }
        else
        {
            GamePad.SetVibration(PlayerIndex.One, 0.0f, 0.0f);
        }

        if(gamePadState.ThumbSticks.Left != Vector2.Zero)
        {
            _slimePosition.X += gamePadState.ThumbSticks.Left.X * speed;
            _slimePosition.Y -= gamePadState.ThumbSticks.Left.Y * speed;
        }
        else
        {
            if (gamePadState.IsButtonDown(Buttons.DPadUp))
            {
                _slimePosition.Y -= speed;
            }

            if (gamePadState.IsButtonDown(Buttons.DPadDown))
            {
                _slimePosition.Y += speed;
            }

            if(gamePadState.IsButtonDown(Buttons.DPadLeft))
            {
                _slimePosition.X -= speed;
            }

            if(gamePadState.IsButtonDown(Buttons.DPadRight))
            {
                _slimePosition.X += speed;
            }
        }
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        //// the bounds of the icon within the texture
        //Rectangle icon = new Rectangle(0, 0, 128, 128);

        //// the bounds of the word mark within the texture
        //Rectangle wordMark = new Rectangle(150, 34, 458, 58);

        // TODO: Add your drawing code here
        // begin the sprite batch with SpriteSortMode.FrontToBack to allow for layerDepth sorting
        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

        // Draw the slime sprite.
        _slime.Draw(SpriteBatch, _slimePosition);

        // Draw the bat sprite 10px to the right of the slime.
        _bat.Draw(SpriteBatch, new Vector2(_slime.Width + 10, 0));

        // Draw the slime texture region at a scale of 4.0
        //_slime.Draw(SpriteBatch, Vector2.Zero, Color.White, 0.0f, Vector2.One, 4.0f, SpriteEffects.None, 0.0f);

        // Draw the bat texture region 10px to the right of the slime at a scale of 4.0
        //_bat.Draw(SpriteBatch, new Vector2(_slime.Width * 4.0f + 10, 0), Color.White, 0.0f, Vector2.One, 4.0f, SpriteEffects.None, 1.0f);


        // use middle of the texture as the origin for rotation
        // use Vector2 for scale with dynamic scaling in x and y directions
        // use Color.White * _opacity for opacity with dynamic opacity
        // use rectangle for sourceRectangle to draw the icon portion of the texture separately from the word mark portion of the texture
        // using layerDepth to draw the icon on top of the word mark
        //SpriteBatch.Draw(
        //_logo,                      // texture
        //new Vector2(                // position
        //    Window.ClientBounds.Width,
        //    Window.ClientBounds.Height) * 0.5f,
        //icon,                       // sourceRectangle
        //Color.White * _opacity,                // color
        //MathHelper.ToRadians(_angle), // rotation
        //new Vector2(                // origin
        //    icon.Width,
        //    icon.Height) * 0.5f,
        //new Vector2(_scaleX, _scaleY),          // scale
        //SpriteEffects.None,         // effects
        //1.0f                        // layerDepth
        //);


        //SpriteBatch.Draw(
        //_logo,                      // texture
        //new Vector2(                // position
        //    Window.ClientBounds.Width,
        //    Window.ClientBounds.Height) * 0.5f,
        //wordMark,                   // sourceRectangle
        //Color.White * (1.0f - _opacity),                // color
        //MathHelper.ToRadians(_angle), // rotation
        //new Vector2(                // origin
        //    wordMark.Width,
        //    wordMark.Height) * 0.5f,
        //new Vector2(_scaleX, _scaleY),          // scale
        //SpriteEffects.None,         // effects
        //0.0f                        // layerDepth
        //);

        // use middle of the texture as the origin for rotation
        // use Vector2 for scale with dynamic scaling in x and y directions
        // use Color.White * _opacity for opacity with dynamic opacity
        // Draw the texture.
        //SpriteBatch.Draw(
        //_logo,                      // texture
        //new Vector2(                // position
        //    Window.ClientBounds.Width,
        //    Window.ClientBounds.Height) * 0.5f,
        //null,                       // sourceRectangle
        //Color.White * _opacity,                // color
        //MathHelper.ToRadians(_angle), // rotation
        //new Vector2(                // origin
        //    _logo.Width,
        //    _logo.Height) * 0.5f,
        //new Vector2(_scaleX, _scaleY),          // scale
        //SpriteEffects.None,         // effects
        //0.0f                        // layerDepth
        //);

        // this code centers the logo
        // Draw the logo texture.
        //SpriteBatch.Draw(
        //    _logo,          // texture
        //    new Vector2(    // position
        //        (Window.ClientBounds.Width - _logo.Width) * 0.5f,
        //        (Window.ClientBounds.Height - _logo.Height) * 0.5f
        //    ),
        //    Color.White     // color
        //);
        SpriteBatch.End();

        base.Draw(gameTime);
    }
}
