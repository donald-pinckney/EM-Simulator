using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

namespace EM_Sim
{
    class Camera
    {
        private float lookSensitivityFactor;
        private float moveSpeedFactor;
        private float cameraYaw;
        private float cameraPitch;
        private Vector3 cameraPosition;
        private MouseState mouseState;
        private Matrix projectionMatrix;
        private bool printCameraInfo = false;
        private readonly float nearDistanceDefault = 0.1f;
        private readonly float farDistanceDefault = 1000.0f;
        private GraphicsDevice device;
        private bool mouseLocked = true;
        private int frames = 0;

        #region Constructors
        public Camera(GraphicsDevice deviceTemp)
        {
            cameraPosition = Vector3.Zero;
            cameraYaw = 0;
            cameraPitch = 0;
            lookSensitivityFactor = 0.1f;
            moveSpeedFactor = 30.0f;

            device = deviceTemp;

            float ratio = (float)device.Viewport.Width / (float)device.Viewport.Height;
            SetProjMatrix(MathHelper.PiOver4, nearDistanceDefault, farDistanceDefault);

            printCameraInfo = false;
        }
        public Camera(Vector3 pos, float yaw, float pitch, float sens, float speed, GraphicsDevice deviceTemp)
        {
            cameraPosition = pos;
            cameraYaw = yaw;
            cameraPitch = pitch;

            device = deviceTemp;

            float ratio = (float)device.Viewport.Width / (float)device.Viewport.Height;
            SetProjMatrix(MathHelper.ToRadians(60.0f), nearDistanceDefault, farDistanceDefault);

            lookSensitivityFactor = sens;
            moveSpeedFactor = speed;

            Mouse.SetPosition(device.Viewport.Width / 2, device.Viewport.Height / 2);
            mouseState = Mouse.GetState();

            printCameraInfo = false;
        }
        public Camera(Vector3 pos, float yaw, float pitch, float fov, float closeDistance, float farDistance, float sens, float speed, GraphicsDevice deviceTemp)
        {
            cameraPosition = pos;
            cameraYaw = yaw;
            cameraPitch = pitch;

            device = deviceTemp;

            float ratio = (float)device.Viewport.Width / (float)device.Viewport.Height;
            SetProjMatrix(fov, closeDistance, farDistance);
            

            lookSensitivityFactor = 0.1f;
            moveSpeedFactor = speed;

            printCameraInfo = false;
        }
        #endregion


        #region Camera Update
        public void UpdateCam(float scaleSpeed)
        {
            if ((CheckKey(Keys.LeftControl) || CheckKey(Keys.RightControl)) && frames == 0)
            {
                mouseLocked = !mouseLocked;
                frames = 30;
            }

            float moveSpeed = moveSpeedFactor * scaleSpeed;
            float turnSpeed = lookSensitivityFactor * scaleSpeed;

            MoveCam(CheckMoveVector(moveSpeed));
            RotateCam(turnSpeed);

            if (printCameraInfo)
            {
                Console.WriteLine("Yaw: " + cameraYaw + " pitch: " + cameraPitch);
                //Console.WriteLine("Pos: " + cameraPosition);
            }

            if (frames > 0)
                frames--;
        }
        private Vector3 CheckMoveVector(float moveSpeed)
        {
            Vector3 movement = new Vector3(0, 0, 0);

            // X Movement
            if (CheckKey(Keys.W))
            {
                movement += new Vector3(0, 0, -moveSpeed);
            }
            if (CheckKey(Keys.S))
            {
                movement += new Vector3(0, 0, moveSpeed);
            }

            // Z Movement
            if (CheckKey(Keys.A))
            {
                movement += new Vector3(-moveSpeed, 0, 0);
            }
            if (CheckKey(Keys.D))
            {
                movement += new Vector3(moveSpeed, 0, 0);
            }
            //Console.
            return movement;
        }
        private void MoveCam(Vector3 move)
        {
            Matrix rot = Matrix.CreateRotationX(cameraPitch)
                * Matrix.CreateRotationY(cameraYaw);

            move = Vector3.Transform(move, rot);

            cameraPosition += move;
        }
        private void RotateCam(float speed)
        {
            if (mouseLocked)
            {
                MouseState currentState = Mouse.GetState();
                if ((currentState.X != mouseState.X) || (currentState.Y != mouseState.Y))
                {
                    int dx = mouseState.X - currentState.X;
                    int dy = mouseState.Y - currentState.Y;

                    cameraYaw += dx * speed;
                    cameraPitch += dy * speed;
                }

                Mouse.SetPosition(device.Viewport.Width / 2, device.Viewport.Height / 2);
                mouseState = Mouse.GetState();
            }
        }
        #endregion


        #region Get Statements
        public Matrix GetViewProjMatrix()
        {
            Vector3 lookAt = new Vector3(0, 0, -1);
            lookAt = Vector3.Transform(lookAt, Matrix.CreateRotationX(cameraPitch));
            lookAt = Vector3.Transform(lookAt, Matrix.CreateRotationY(cameraYaw));

            Matrix viewMatrix = Matrix.CreateLookAt(
                cameraPosition,
                cameraPosition + lookAt,
                Vector3.Up
            );

            return viewMatrix * projectionMatrix;
        }
        public Matrix GetViewMatrix()
        {
            Vector3 lookAt = new Vector3(0, 0, -1);
            lookAt = Vector3.Transform(lookAt, Matrix.CreateRotationX(cameraPitch));
            lookAt = Vector3.Transform(lookAt, Matrix.CreateRotationY(cameraYaw));

            Matrix viewMatrix = Matrix.CreateLookAt(
                cameraPosition,
                cameraPosition + lookAt,
                Vector3.Up
            );

            return viewMatrix;
        }
        public Matrix GetProjMatrix()
        {
            return projectionMatrix;
        }
        public Vector3 GetPos()
        {
            return cameraPosition;
        }
        public float GetYaw()
        {
            return cameraYaw;
        }
        public float GetPitch()
        {
            return cameraPitch;
        }
        public float GetSensitivity()
        {
            return lookSensitivityFactor;
        }
        public float GetSpeed()
        {
            return moveSpeedFactor;
        }
        public Vector3 GetLookAt()
        {
            Vector3 lookAt = new Vector3(0, 0, -1);
            lookAt = Vector3.Transform(lookAt, Matrix.CreateRotationX(cameraPitch));
            lookAt = Vector3.Transform(lookAt, Matrix.CreateRotationY(cameraYaw));

            return cameraPosition + lookAt;
        }
        #endregion

        #region Set Statements
        public void SetProjMatrix(float fov, float nearDistance, float farDistance)
        {
            float ratio = (float)device.Viewport.Width / (float)device.Viewport.Height;

            projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                fov,
                ratio,
                nearDistance,
                farDistance
            );
        }
        public void SetProjMatrix(Matrix newProjMatrix)
        {
            projectionMatrix = newProjMatrix;
        }
        public void SetSensitivity(float sens)
        {
            lookSensitivityFactor = sens;
        }
        public void SetMoveSpeed(float speed)
        {
            moveSpeedFactor = speed;
        }
        public void SetPrintInfo(bool value)
        {
            printCameraInfo = value;
        }
        public void SetHeight(float newHeight)
        {
            cameraPosition.Y = newHeight;
        }
        #endregion


        #region Key Checker
        private bool CheckKey(Keys key)
        {
            KeyboardState state = Keyboard.GetState();
            if (state.IsKeyDown(key))
                return true;
            else
                return false;
        }
        #endregion
    }
}