using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using static System.Formats.Asn1.AsnWriter;

namespace monotest
{
    public class StaticObject
    {
        protected Texture2D _texture; 
        protected Vector2 _position; 
        protected float _scale;     

        public StaticObject(Texture2D texture, Vector2 position, float scale = 1.0f)
        {
            _texture = texture;
            _position = position;
            _scale = scale;
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
               _texture,        
               _position,      
               null,      
               Color.White,    
               0f,          
               Vector2.Zero,  
               _scale,  // изменяем размер    
               SpriteEffects.None, 
               0f   
           );
        }
    }

    public class MovingObject : StaticObject
    {
        protected Vector2 _velocity; 

        public MovingObject(Texture2D texture, Vector2 position, Vector2 velocity, float scale = 1.0f, float layer = 0f)
            : base(texture, position, scale)
        {
            _velocity = velocity;
        }

        public void Update(GameTime gameTime)
        {
            _position += _velocity;

            if (_position.X <= 0 || _position.X + _texture.Width * _scale >= 1920) 
                _velocity.X *= -1; 

            if (_position.Y <= 0 || _position.Y + _texture.Height * _scale >= 1080) 
                _velocity.Y *= -1;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }

    public class Player : MovingObject
    {
        private KeyboardState _currentKeyboardState;
        private KeyboardState _previousKeyboardState;

        public Player(Texture2D texture, Vector2 position, float scale = 1.0f)
            : base(texture, position, Vector2.Zero, scale)
        {
        }

        public void Update(GameTime gameTime)
        {
            _previousKeyboardState = _currentKeyboardState;
            _currentKeyboardState = Keyboard.GetState();

            Vector2 movement = Vector2.Zero;

            if (_currentKeyboardState.IsKeyDown(Keys.W) || _currentKeyboardState.IsKeyDown(Keys.Up))
                movement.Y -= 1;
            if (_currentKeyboardState.IsKeyDown(Keys.S) || _currentKeyboardState.IsKeyDown(Keys.Down))
                movement.Y += 1;
            if (_currentKeyboardState.IsKeyDown(Keys.A) || _currentKeyboardState.IsKeyDown(Keys.Left))
                movement.X -= 1;
            if (_currentKeyboardState.IsKeyDown(Keys.D) || _currentKeyboardState.IsKeyDown(Keys.Right))
                movement.X += 1;

            if (movement.Length() > 0)
                movement.Normalize();

            _velocity = movement * 6f;

            _position += _velocity;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
    }

    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Texture2D sprite;
        private StaticObject _staticObject;
        private List<StaticObject> _staticObjects;
        private List<MovingObject> _movingObjects;
        private Player _player;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);


            // TODO: use this.Content to load your game content here\

            Texture2D cupepTexture = Content.Load<Texture2D>("chupep");
            Texture2D cheliabinskTexture = Content.Load<Texture2D>("cheliabinsk");
            Texture2D sobakaTexture = Content.Load<Texture2D>("sobaka");
            Texture2D playerTexture = Content.Load<Texture2D>("player");

            _staticObjects = new List<StaticObject>();
            _movingObjects = new List<MovingObject>();

            Vector2 startPosition = new Vector2(100, 100);
            float spacing = 500;
            for (int i = 0; i < 4; i++)
            {
                Vector2 position = new Vector2(startPosition.X + i * spacing, startPosition.Y);
                _staticObjects.Add(new StaticObject(cupepTexture, position));
            }

            Vector2 cheliabinskPosition = new Vector2(10, 700);
            float cheliabSpacing = 650;
            for (int i = 0; i < 3; i++)
            {
                Vector2 position = new Vector2(cheliabinskPosition.X + i * cheliabSpacing, cheliabinskPosition.Y);
                _staticObjects.Add(new StaticObject(cheliabinskTexture, position, 0.5f));
            }

            _movingObjects.Add(new MovingObject(sobakaTexture, new Vector2(100, 100), new Vector2(0, 3), 0.3f));
            _movingObjects.Add(new MovingObject(sobakaTexture, new Vector2(200, 200), new Vector2(4, 0), 0.3f));
            _movingObjects.Add(new MovingObject(sobakaTexture, new Vector2(300, 300), new Vector2(2, 2), 0.3f));
            _movingObjects.Add(new MovingObject(sobakaTexture, new Vector2(700, 700), new Vector2(-10, -10), 0.3f));

            _player = new Player(playerTexture, new Vector2(500, 500), 0.3f);
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here

            foreach (var obj in _movingObjects)
            {
                obj.Update(gameTime);
            }

            _player.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // TODO: Add your drawing code here
            _spriteBatch.Begin();

            foreach (var obj in _staticObjects)
            {
                obj.Draw(_spriteBatch);
            }

            _player.Draw(_spriteBatch);

            foreach (var obj in _movingObjects)
            {
                obj.Draw(_spriteBatch);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
