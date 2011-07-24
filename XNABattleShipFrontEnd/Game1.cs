using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

namespace XNABattleShipFrontEnd
{
   /// <summary>
   /// This is the main type for your game
   /// </summary>
   public class Game1 : Microsoft.Xna.Framework.Game
   {
      GraphicsDeviceManager graphics;
      SpriteBatch spriteBatch;

      VertexDeclaration vertexDeclaration;
      BasicEffect basicEffect;

      Matrix viewMatrix;
      Matrix projectionMatrix;
      Matrix worldMatrix;

      private const int POINTS = 8;

      VertexPositionColor[] pointList;
      BoardGrid board;

      float cameraYaw = 0;
      float cameraPitch = -1;
      float cameraDistance = 1000.0f;
      Vector3 cameraPosition;
      Vector3 cameraLookAt = Vector3.Zero;

      KeyboardState currentKeyboardState;

      public Game1()
      {
         graphics = new GraphicsDeviceManager(this);
         Content.RootDirectory = "Content";
      }

      /// <summary>
      /// Allows the game to perform any initialization it needs to before starting to run.
      /// This is where it can query for any required services and load any non-graphic
      /// related content.  Calling base.Initialize will enumerate through any components
      /// and initialize them as well.
      /// </summary>
      protected override void Initialize()
      {
         vertexDeclaration = new VertexDeclaration(GraphicsDevice, VertexPositionColor.VertexElements);
         basicEffect = new BasicEffect(GraphicsDevice, null);
         basicEffect.VertexColorEnabled = true;

         projectionMatrix = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(60.0f), GraphicsDevice.Viewport.AspectRatio, 1.0f, 2000.0f);
         worldMatrix = Matrix.CreateTranslation(0, 0, 0);

         basicEffect.World = worldMatrix;
         basicEffect.Projection = projectionMatrix;

         pointList = new VertexPositionColor[POINTS];

         for (int x = 0; x < POINTS / 2; x++)
         {
            for (int z = 0; z < 2; z++)
            {
               pointList[(x * 2) + z] = new VertexPositionColor(new Vector3(x * 100, 0, z * 100), Color.White);
            }
         }

         board = new BoardGrid(10, 10);

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

      }

      /// <summary>
      /// UnloadContent will be called once per game and is the place to unload
      /// all content.
      /// </summary>
      protected override void UnloadContent()
      {
         // TODO: Unload any non ContentManager content here
      }

      /// <summary>
      /// Allows the game to run logic such as updating the world,
      /// checking for collisions, gathering input, and playing audio.
      /// </summary>
      /// <param name="gameTime">Provides a snapshot of timing values.</param>
      protected override void Update(GameTime gameTime)
      {
         currentKeyboardState = Keyboard.GetState();

         // Allows the game to exit
         if (currentKeyboardState.IsKeyDown(Keys.Escape))
            this.Exit();

         if (currentKeyboardState.IsKeyDown(Keys.Down))
            cameraPitch -= 0.1f;

         if (currentKeyboardState.IsKeyDown(Keys.Up))
            cameraPitch += 0.1f;

         if (currentKeyboardState.IsKeyDown(Keys.Left))
            cameraYaw += 0.1f;

         if (currentKeyboardState.IsKeyDown(Keys.Right))
            cameraYaw -= 0.1f;

         if (currentKeyboardState.IsKeyDown(Keys.Home))
            cameraDistance -= 10;

         if (currentKeyboardState.IsKeyDown(Keys.End))
            cameraDistance += 10;

         cameraPitch = MathHelper.Clamp(cameraPitch, -MathHelper.PiOver2 + 0.1f, 0);
         cameraYaw = MathHelper.WrapAngle(cameraYaw);
         cameraDistance = MathHelper.Clamp(cameraDistance, 200, 1500);

         UpdateCameraPosition();

         base.Update(gameTime);
      }

      /// <summary>
      /// This is called when the game should draw itself.
      /// </summary>
      /// <param name="gameTime">Provides a snapshot of timing values.</param>
      protected override void Draw(GameTime gameTime)
      {
         GraphicsDevice.Clear(Color.Indigo);
         GraphicsDevice.VertexDeclaration = vertexDeclaration;

         basicEffect.View = viewMatrix;

         basicEffect.Begin();

         foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
         {
            pass.Begin();

            //GraphicsDevice.RenderState.PointSize = 10;
            //GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList, pointList, 0, POINTS, new short[] { 0, 1, 1, 2, 2, 3, 3, 4, 4, 5, 5, 6, 6, 7 }, 0, 7);
            GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineList, board.VertexList, 0, board.VertexIndexList.Length, board.VertexIndexList, 0, board.VertexIndexList.Length / 2);

            pass.End();
         }


         basicEffect.End();

         base.Draw(gameTime);
      }

      private void UpdateCameraPosition()
      {
         Matrix rotationMatrix = Matrix.CreateFromYawPitchRoll(cameraYaw, cameraPitch, 0.0f);
         Vector3 direction = Vector3.Transform(Vector3.Forward, rotationMatrix);
         direction = Vector3.Negate(direction);
         cameraPosition = cameraLookAt + (direction * cameraDistance);
         viewMatrix = Matrix.CreateLookAt(cameraPosition, cameraLookAt, Vector3.Up);
      }
   }
}
