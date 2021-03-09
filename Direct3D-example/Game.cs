using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Windows;

namespace Para_1
{

    class Game : IDisposable
    {
        const float SPEED = 30f;

        private RenderForm _renderForm;

        private MeshObject _cube;
        private MeshObject _axes;
        private MeshObject _rectangle;
        private Camera _camera;
        private float _cameraSpeed;

        private Scene _scene;

        private Renderer.IlluminationProperties _illumination;

        private DirectX3DGraphics _directX3DGraphics;
        private Renderer _renderer;

        private TimeHelper _timeHelper;
        private InputController _inputController;
        private Random _random;

        public Game()
        {
            _renderForm = new RenderForm();
            _renderForm.UserResized += RenderFormResizedCallback;
            _directX3DGraphics = new DirectX3DGraphics(_renderForm);
            _renderer = new Renderer(_directX3DGraphics);
            _renderer.CreateConstantBuffers();

            _scene = new Scene();
            Loader loader = new Loader(_directX3DGraphics);

            Texture texture;
            Material material;

            string coloredTextureName = "white";
            string emmisiveMaterialName = "emmisive";
            string coloredMaterialName = "colored";

            string cubeTextureName = "cube";
            string cubeMaterialName = "cube";


            texture = loader.LoadTextureFromFile("white.png", _renderer.PointSampler, false);
            _scene.Textures.Add(coloredTextureName, texture);
            material = new Material(texture, new Vector3(1.0f), new Vector3(0.0f), new Vector3(0.0f), new Vector3(0.0f), 1.0f);
            _scene.Materials.Add(emmisiveMaterialName, material);
            material = new Material(texture, new Vector3(0.0f), new Vector3(1.0f), new Vector3(1.0f), new Vector3(0.0f), 1.0f);
            _scene.Materials.Add(coloredMaterialName, material);

            texture = loader.LoadTextureFromFile("cube.png", _renderer.LinearSampler, true, 8);
            _scene.Textures.Add(cubeTextureName, texture);
            material = new Material(texture, new Vector3(0.0f), new Vector3(1.0f), new Vector3(1.0f), new Vector3(1.0f), 8.0f);
            _scene.Materials.Add(cubeMaterialName, material);


            _axes = loader.MakeAxes(_scene.Materials[emmisiveMaterialName], true);
            _scene.Meshes.Add(_axes);
            _cube = loader.MakeCube(new Vector4(0.0f, 3.0f, 0.0f, 1.0f), 0, 0, 0, _scene.Materials[cubeMaterialName], true);
            _scene.Meshes.Add(_cube);

            

            _rectangle = loader.MakeMesh(
                new Vector4(0.0f, -2.0f, 0.0f, 1.0f),
                _scene.Materials[coloredMaterialName]);
            _scene.Meshes.Add(_rectangle);

            _illumination = new Renderer.IlluminationProperties();
            Renderer.LightSource lightSource = new Renderer.LightSource();
            _illumination.globalAmbient = new Vector3(0.02f);
            lightSource.lightSourceType = Renderer.LightSourceType.Base;
            lightSource.constantAttenuation = 1.0f;
            lightSource.color = Vector3.Zero;
            for (int i = 0; i < Renderer.MaxLights; i++)
                _illumination[i] = lightSource;
            lightSource.lightSourceType = Renderer.LightSourceType.Directional;
            lightSource.color = new Vector3(1.0f);
            lightSource.direction = Vector3.Normalize(new Vector3(-0.1f, -1.0f, 0.3f));
            _illumination[0] = lightSource;


            MeshObject lightDirection = loader.MakeLine(new Vector4(lightSource.position, 1.0f), new Vector4(lightSource.direction, 1.0f), 10.0f, new Vector4(lightSource.color, 1.0f), _scene.Materials[emmisiveMaterialName], true);

            _camera = new Camera(new Vector4(0.0f, 5.0f, -10.0f, 1.0f));
            _cameraSpeed = 1.0f;

            _timeHelper = new TimeHelper();
            _inputController = new InputController(_renderForm);
            _random = new Random();

            loader.Dispose();
            loader = null;
        }

        public void RenderFormResizedCallback(object sender, EventArgs args)
        {
            _directX3DGraphics.Resize();
            _camera.Aspect = _renderForm.ClientSize.Width /
                (float)_renderForm.ClientSize.Height;
        }

        private bool _firstRun = true;

        public void RenderLoopCallback()
        {
            if (_firstRun)
            {
                RenderFormResizedCallback(this, EventArgs.Empty);
                _firstRun = false;
            }
            _timeHelper.Update();
            _renderForm.Text = "FPS: " + _timeHelper.FPS.ToString();

            _inputController.UpdateKeyboardState();
            _inputController.UpdateMouseState();

            if(_inputController.KeyboardUpdate)
            {
                if (_inputController.KeyLeft) _cube.MoveBy(-1.0f, 0.0f, 0.0f);
                if (_inputController.KeyRight) _cube.MoveBy(1.0f, 0.0f, 0.0f);
                if (_inputController.KeyUp) _cube.MoveBy(0.0f, 0.0f, 1.0f);
                if (_inputController.KeyDown) _cube.MoveBy(0.0f, 0.0f, -1.0f);

                Vector4 moveDirection = Vector4.Zero;
                if (_inputController.KeyW) moveDirection += Vector4.UnitZ ;
                if (_inputController.KeyS) moveDirection -= Vector4.UnitZ ;
                if (_inputController.KeyD) moveDirection += Vector4.UnitX ;
                if (_inputController.KeyA) moveDirection -= Vector4.UnitX ;
                moveDirection.Normalize();
                Matrix rotation = Matrix.RotationYawPitchRoll(_camera.Yaw, _camera.Pitch, _camera.Roll);
                Vector4.Transform(ref moveDirection, ref rotation, out moveDirection);
                moveDirection *= _cameraSpeed * _timeHelper.DeltaT * SPEED;
                _camera.MoveBy(moveDirection);
            }

            if(_inputController.MouseUpdate)
            {
                float deltaAngle = _camera.FOVY / _renderForm.ClientSize.Height;
                _camera.YawByAngle(deltaAngle * _inputController.MouseRelativePositionX);
                _camera.PitchByAngle(deltaAngle * _inputController.MouseRelativePositionY);
            }

            _cube.PitchByAngle(_timeHelper.DeltaT * MathUtil.TwoPi * 0.10f);
           // _cube.RollByAngle(_timeHelper.DeltaT * MathUtil.TwoPi * 0.15f);
            _rectangle.YawByAngle(_timeHelper.DeltaT * MathUtil.TwoPi * -0.1f);

            Matrix viewMatrix = _camera.GetViewMatrix();
            Matrix projectionMatrix = _camera.GetPojectionMatrix();

            _renderer.UpdatePerFrameConstantBuffers(_timeHelper.Time);

            _illumination.eyePosition = _camera.Position;
            _renderer.UpdateIllumination(_illumination);

            _renderer.BeginRender();

            for (int i = 0; i < _scene.Meshes.Count; i++)
            {
                MeshObject mesh = _scene.Meshes[i];
                _renderer.UpdatePerObjectConstantBuffers(
                    mesh.GetWorldMatrix(), viewMatrix, projectionMatrix,
                    (mesh == _cube ? 1 : 0), mesh.ObjectType);
                _renderer.RenderMeshObject(mesh);
            }

            _renderer.EndRender();
        }

        public void Run()
        {
            RenderLoop.Run(_renderForm, RenderLoopCallback);
        }

        public void Dispose()
        {
            _inputController.Dispose();
            _scene.Dispose();
            _renderer.Dispose();
            _directX3DGraphics.Dispose();
        }
    }
}
