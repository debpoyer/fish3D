using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Text;

namespace VoxelTK
{
    internal class Camera
    {
        // Consts
        private float CameraSpeed { get; set; } = 8f;
        private float ScreenWidth;
        private float ScreenHeight;
        private float CameraSensitivity { get; set; } = 100f;

        // Position Variables
        public Vector3 Position;

        Vector3 Up = Vector3.UnitZ;
        Vector3 Front = Vector3.UnitY;
        Vector3 Right = Vector3.UnitX;

        // Rotation Variables

        private float Pitch;
        private float Yaw = 90.0f;
        
        private bool FirstMove { get; set; } = true;
        public Vector2 LastPos;

        public Camera(float width, float height, Vector3 position) 
        {
            ScreenWidth = width;
            ScreenHeight = height;
            this.Position = position;
        }

        public Matrix4 GetViewMatrix() 
        { 
            return Matrix4.LookAt(Position, Position + Front, Up);
        }
        public Matrix4 GetProjectionMatrix() 
        {
            return Matrix4.CreatePerspectiveFieldOfView(
                MathHelper.DegreesToRadians(45.0f),
                (float)ScreenWidth / (float)ScreenHeight,
                0.1f, 100.0f);
        }

        private void UpdateVectors()
        {
            Pitch = Math.Clamp(Pitch, -89f, 89f);

            Front.X = MathF.Cos(MathHelper.DegreesToRadians(Pitch)) * MathF.Cos(MathHelper.DegreesToRadians(Yaw));
            Front.Y = MathF.Cos(MathHelper.DegreesToRadians(Pitch)) * MathF.Sin(MathHelper.DegreesToRadians(Yaw));
            Front.Z = MathF.Sin(MathHelper.DegreesToRadians(Pitch));

            Front = Vector3.Normalize(Front);

            Right = Vector3.Normalize(Vector3.Cross(Front, Vector3.UnitZ));
            Up = Vector3.Normalize(Vector3.Cross(Right, Front));
        }

        public void Input(KeyboardState input, MouseState mouse, FrameEventArgs e) 
        {
            if (input.IsKeyDown(Keys.W))
            {
                Position += Front * CameraSpeed * (float)e.Time;
            }
            if (input.IsKeyDown(Keys.A))
            {
                Position -= Right * CameraSpeed * (float)e.Time;
            }
            if (input.IsKeyDown(Keys.S))
            {
                Position -= Front * CameraSpeed * (float)e.Time;
            }
            if (input.IsKeyDown(Keys.D))
            {
                Position += Right * CameraSpeed * (float)e.Time;
            }

            if (input.IsKeyDown(Keys.Space))
            {
                Position.Z += CameraSpeed * (float)e.Time;
            }
            if (input.IsKeyDown(Keys.LeftControl))
            {
                Position.Z -= CameraSpeed * (float)e.Time;
            }

            if (FirstMove)
            {
                LastPos = new Vector2(mouse.X, mouse.Y);
                FirstMove = false;
            }
            else
            {
                var deltaX = mouse.X - LastPos.X;
                var deltaY = mouse.Y - LastPos.Y;
                LastPos = new Vector2(mouse.X, mouse.Y);

                Yaw -= deltaX * CameraSensitivity * (float)e.Time;
                Pitch -= deltaY * CameraSensitivity * (float)e.Time;
            }

            UpdateVectors();
        }

        public void Update(KeyboardState input, MouseState mouse, FrameEventArgs e) {
            Input(input, mouse, e);
        }


    }
}
