using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace asteroids
{
    class Asteroid
    {
        public Vector2 position;
        public Vector2 direction;
        public Texture2D sprite; 
        public float rotation;

        float rotationDirection;
        float speed;
        float rotationSpeed;

        public Asteroid(GraphicsDeviceManager _graphics, Texture2D spriteTexture)
        {
            Random r = new Random();
            sprite = spriteTexture;

            float randomFloat = r.Next(0, 4);
            speed = r.Next(25, 31);

            if(randomFloat == 0)
            {
                position = new Vector2(0 - (spriteTexture.Width / 2), r.Next(0, _graphics.PreferredBackBufferHeight));
                direction = generate_direction(r.Next(1, 180));
            }
            else if(randomFloat == 1)
            {
                position = new Vector2(r.Next(0, _graphics.PreferredBackBufferWidth), 0 - (spriteTexture.Height / 2));
                direction = generate_direction(r.Next(271, 450));
            }
            else if(randomFloat == 2)
            {
                position = new Vector2(_graphics.PreferredBackBufferWidth + (spriteTexture.Width / 2), r.Next(0, _graphics.PreferredBackBufferHeight));
                direction = generate_direction(r.Next(181, 360));
            }
            else
            {
                position = new Vector2(r.Next(0, _graphics.PreferredBackBufferWidth), _graphics.PreferredBackBufferHeight + (spriteTexture.Height / 2));
                direction = generate_direction(r.Next(91, 270));
            }

            rotation = 0f;
            rotationDirection = (r.Next(0, 2) * 2) - 1;
            rotationSpeed = r.Next(1, 4);

        }
        
        public void move(float game_time, GraphicsDeviceManager _graphics)
        {
            position += direction * speed * game_time;
            rotation += rotationDirection * rotationSpeed * game_time;

            if (position.X + (sprite.Width / 2) < 0)
            {
                position.X = _graphics.PreferredBackBufferWidth + (sprite.Width / 2);
            }
            else if (position.X - (sprite.Width / 2) > _graphics.PreferredBackBufferWidth)
            {
                position.X = 0 - (sprite.Width / 2);
            }
            else if (position.Y + (sprite.Height / 2) < 0)
            {
                position.Y = _graphics.PreferredBackBufferHeight + (sprite.Height / 2);
            }
            else if (position.Y - (sprite.Height / 2) > _graphics.PreferredBackBufferHeight)
            {
                position.Y = 0 - (sprite.Height / 2);
            }
        }

        Vector2 generate_direction(float angle)
        {

            float random_angle = angle * (MathF.PI / 180);
            float y_dir = MathF.Cos(random_angle);
            float x_dir = MathF.Sin(random_angle);

            return new Vector2(x_dir, y_dir);
        }

    }

    class Player
    {
        public Vector2 position;
        public Vector2 velocity;
        public Texture2D sprite;
        public float rotation;
        
        float acceleration;
        float rotationSpeed;

        public Player(GraphicsDeviceManager _graphics, Texture2D spriteTexture)
        {
            position = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
            rotation = 0f;
            velocity = Vector2.Zero;
            sprite = spriteTexture;

            rotationSpeed = 5f;
            acceleration = 6f;
        }

        public void move(GameTime gameTime, GraphicsDeviceManager _graphics)
        {
            KeyboardState kstate = Keyboard.GetState();

            // rotate ship
            if (kstate.IsKeyDown(Keys.Left)) rotation -= rotationSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (kstate.IsKeyDown(Keys.Right)) rotation += rotationSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            //  translate ship 
            if (kstate.IsKeyDown(Keys.Up))
            {
                velocity.X += MathF.Sin(rotation) * acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
                velocity.Y += -MathF.Cos(rotation) * acceleration * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            position.X += velocity.X;
            position.Y += velocity.Y;

            if (position.X + (sprite.Width / 2) < 0)
            {
                position.X = _graphics.PreferredBackBufferWidth + (sprite.Width / 2);
            }
            else if (position.X - (sprite.Width / 2) > _graphics.PreferredBackBufferWidth)
            {
                position.X = 0 - (sprite.Width / 2);
            }else if (position.Y + (sprite.Height / 2) < 0)
            {
                position.Y = _graphics.PreferredBackBufferHeight + (sprite.Height / 2);
            }else if (position.Y - (sprite.Height /2) > _graphics.PreferredBackBufferHeight)
            {
                position.Y = 0 - (sprite.Height / 2);
            }
        
                    
         }

    }
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        //  sprites
        Texture2D playerSprite;
        Texture2D largeAsteroidSprite;

        //  ship
        Player player;

        //  asteroids
        List<Asteroid> asteroids;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            asteroids = new List<Asteroid>();

            base.Initialize();

            player = new Player(_graphics, playerSprite);

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            playerSprite = Content.Load<Texture2D>("asteroid_ship");
            largeAsteroidSprite = Content.Load<Texture2D>("asteroid_large");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            player.move(gameTime, _graphics);

            Random r = new Random();

            if(r.Next(1, 100) == 1)
            {
                asteroids.Add(new Asteroid(_graphics, largeAsteroidSprite));
            }

            for(int i = 0; i < asteroids.Count ; i++)
            {
                asteroids[i].move((float)gameTime.ElapsedGameTime.TotalSeconds, _graphics);
            }
            
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
           
            foreach(Asteroid asteroid in asteroids)
            {
                _spriteBatch.Draw(asteroid.sprite, asteroid.position, null, Color.White, asteroid.rotation, new Vector2(asteroid.sprite.Width / 2, asteroid.sprite.Height / 2), Vector2.One, SpriteEffects.None, 0f);
            }
            
            _spriteBatch.Draw(player.sprite, player.position, null, Color.White, player.rotation, new Vector2(player.sprite.Width /2, player.sprite.Height /2), new Vector2(0.5f, 0.5f), SpriteEffects.None, 0f);
           
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
