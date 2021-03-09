using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace Para_1
{
    class Camera : Game3DObject
    {
        private float _fovY; // поле зрения в вертикальной плоскости
        public float FOVY { get => _fovY; set => _fovY = value; }

        private float _aspect; // соотношение ширины и высоты
        public float Aspect { get => _aspect; set => _aspect = value; }

        public Camera(Vector4 position, float yaw = 0.0f,
            float pitch = 0.0f, float roll = 0.0f, float fovY = MathUtil.PiOverFour,
            float aspect = 1.0f) : base(position, yaw, pitch, roll)
        {
            _fovY = fovY;
            _aspect = aspect;
        }

        public void MoveForZ(float direction)
        {
            Matrix rotation = Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll);
            Vector3 viewTo = (Vector3)Vector4.Transform(Vector4.UnitZ * direction, rotation);
            MoveBy(viewTo.X, viewTo.Y, viewTo.Z);
        }

        public void MoveForX(float direction)
        {
            Matrix rotation = Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll);
            Vector3 viewTo = (Vector3)Vector4.Transform(Vector4.UnitX * direction, rotation);
            MoveBy(viewTo.X, viewTo.Y, viewTo.Z);
        }

        public Matrix GetPojectionMatrix()
        {
            return Matrix.PerspectiveFovLH(_fovY, _aspect, 0.1f, 100.0f);
        }

        public Matrix GetViewMatrix()
        {
            Matrix rotation = Matrix.RotationYawPitchRoll(_yaw, _pitch, _roll);
            Vector3 viewTo = (Vector3)Vector4.Transform(Vector4.UnitZ, rotation);// направление взгляда камеры
            Vector3 viewUp = (Vector3)Vector4.Transform(Vector4.UnitY, rotation);
            return Matrix.LookAtLH((Vector3)_position, (Vector3)_position + viewTo, viewUp);
        }
    }

}
