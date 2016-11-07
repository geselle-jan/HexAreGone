using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace HexAreGone
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		Texture2D texture;
		Vector2 position;
		Vector2 size;
		SpriteFont font;
		Vector2 screenSize;
		FrameCounter frameCounter = new FrameCounter();

		public Game1()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		public static Texture2D CreateTexture(GraphicsDevice device, int width, int height, Func<int, Color> paint)
		{
			//initialize a texture
			Texture2D texture = new Texture2D(device, width, height);

			//the array holds the color for each pixel in the texture
			Color[] data = new Color[width * height];
			for (int pixel = 0; pixel < width * height; pixel++)
			{
				//the function applies the color according to the specified pixel
				data[pixel] = paint(pixel);
			}

			//set the color
			texture.SetData(data);

			return texture;
		}

		Vector2 GetDesiredVelocityFromInput()
		{
			Vector2 desiredVelocity = new Vector2();

			TouchCollection touchCollection = TouchPanel.GetState();

			if (touchCollection.Count > 0)
			{
				desiredVelocity.X = touchCollection[0].Position.X - position.X;
				desiredVelocity.Y = touchCollection[0].Position.Y - position.Y;

				if (Math.Abs(desiredVelocity.X) > 3 || Math.Abs(desiredVelocity.Y) > 3)
				{
					desiredVelocity.Normalize();
					const float desiredSpeed = 200;
					desiredVelocity *= desiredSpeed;
				}
			}

			return desiredVelocity;
		}

		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize()
		{
			// TODO: Add your initialization logic here

			screenSize = new Vector2(
				GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width,
				GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height
			);
			position = screenSize / 2;
			size = new Vector2(32f, 32f);
			texture = CreateTexture(graphics.GraphicsDevice, (int)Math.Round(size.X), (int)Math.Round(size.Y), pixel => Color.White);

			base.Initialize();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent()
		{
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch(GraphicsDevice);

			font = Content.Load<SpriteFont>("Font");

			//TODO: use this.Content to load your game content here 
		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update(GameTime gameTime)
		{
			// For Mobile devices, this logic will close the Game when the Back button is pressed
			// Exit() is obsolete on iOS
#if !__IOS__ && !__TVOS__
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();
#endif

			// TODO: Add your update logic here

			var velocity = GetDesiredVelocityFromInput();

			velocity = velocity * 4;

			position.X += velocity.X * (float)gameTime.ElapsedGameTime.TotalSeconds;
			position.Y += velocity.Y * (float)gameTime.ElapsedGameTime.TotalSeconds;

			base.Update(gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw(GameTime gameTime)
		{
			var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
			frameCounter.Update(deltaTime);
			var fps = string.Format("FPS: {0}", Math.Round(frameCounter.AverageFramesPerSecond));

			graphics.GraphicsDevice.Clear(Color.Black);

			spriteBatch.Begin();

			spriteBatch.Draw(texture, position - (size / 2), Color.Red);

			spriteBatch.DrawString(font, $"X: {Math.Round(position.X)}\nY: {Math.Round(position.Y)}", position + size, Color.Red);

			spriteBatch.DrawString(font, fps, new Vector2(1, 1), Color.Red);

			spriteBatch.End();

			base.Draw(gameTime);
		}
	}

	public class FrameCounter
	{
		public FrameCounter()
		{
		}

		public long TotalFrames { get; private set; }
		public float TotalSeconds { get; private set; }
		public float AverageFramesPerSecond { get; private set; }
		public float CurrentFramesPerSecond { get; private set; }

		public const int MAXIMUM_SAMPLES = 100;

		private Queue<float> _sampleBuffer = new Queue<float>();

		public bool Update(float deltaTime)
		{
			CurrentFramesPerSecond = 1.0f / deltaTime;

			_sampleBuffer.Enqueue(CurrentFramesPerSecond);

			if (_sampleBuffer.Count > MAXIMUM_SAMPLES)
			{
				_sampleBuffer.Dequeue();
				AverageFramesPerSecond = _sampleBuffer.Average(i => i);
			}
			else
			{
				AverageFramesPerSecond = CurrentFramesPerSecond;
			}

			TotalFrames++;
			TotalSeconds += deltaTime;
			return true;
		}
	}
}
