using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Drawing;
using Core;
using Core.Camera;
using Core.Vertex;
using SlimDX;
using SlimDX.Direct3D11;
using SlimDX.D3DCompiler;
using SlimDX.DXGI;

namespace Shapes
{
    public class ShapesApp : D3DApp
    {
        private List<ShapeBase> shapes;

        private Effect effect;
        private EffectTechnique effectTechnique;
        private EffectMatrixVariable fxW;
        private EffectMatrixVariable fxWIT;
        private EffectMatrixVariable fxWVP;
        private EffectMatrixVariable fxTT;
        private EffectVariable fxDirLights;
        private EffectVariable fxMaterial;
        private EffectResourceVariable fxDiffuseMap;
        private EffectVectorVariable fxEyePosW;

        private InputLayout inputLayout;

        private DirectionalLight dirLights;

        private Matrix texTransform;
        private Matrix world;
        private Matrix view;
        private Matrix projection;

        private Vector3 eyePosW;
        private float phi, theta, radius;

        private Point lastMousePosition;

        private bool disposed;

        private FpsCamera camera;

        public ShapesApp(IntPtr hInstance) : base(hInstance)
        {
            shapes = new List<ShapeBase>();

            dirLights = new DirectionalLight
            {
                Ambient = new Color4(0.8f, 0.8f, 0.8f),
                Diffuse = new Color4(0.5f, 0.5f, 0.5f),
                Specular = new Color4(0.5f, 0.5f, 0.5f),
                Direction = new Vector3(0.57735f, -0.57735f, 0.57735f)
            };

            texTransform = Matrix.Identity;
            world = Matrix.Identity;
            view = Matrix.Identity;
            projection = Matrix.Identity;

            eyePosW = new Vector3();
            lastMousePosition = new Point();

            phi = 0.4f * MathF.PI;
            theta = 1f * MathF.PI;
            radius = 25;

            camera = new FpsCamera() { Position = new Vector3(radius, 3, 0) };
        }

        public override bool Init()
        {
            if (!base.Init())
            {
                return false;
            }

            BuildFX();
            BuildVertexLayout();

            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    Util.ReleaseCom(ref effect);
                    Util.ReleaseCom(ref inputLayout);
                }
                disposed = true;
            }
            base.Dispose(disposing);
        }

        public override void DrawScene()
        {
            ImmediateContext.ClearRenderTargetView(RenderTargetView, Color.SteelBlue);
            ImmediateContext.ClearDepthStencilView(DepthStencilView, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil, 1, 0);

            ImmediateContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            ImmediateContext.InputAssembler.InputLayout = inputLayout;

            var array = Util.GetArray(dirLights);
            fxDirLights.SetRawValue(new DataStream(array, false, false), array.Length);

            fxEyePosW.Set(eyePosW);

            for (int i = 0; i < effectTechnique.Description.PassCount; i++)
            {
                for (int j = 0; j < shapes.Count; j++)
                {
                    ImmediateContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(shapes[j].VertexBuffer, Basic32.Stride, 0));
                    ImmediateContext.InputAssembler.SetIndexBuffer(shapes[j].IndexBuffer, Format.R32_UInt, 0);

                    fxW.SetMatrix(shapes[j].ShapeWorld);
                    fxWIT.SetMatrix(MathF.InverseTranspose(shapes[j].ShapeWorld));
                    array = Util.GetArray(((ShapeWithTextureBase)shapes[j]).ShapeMaterial);
                    fxMaterial.SetRawValue(new DataStream(array, false, false), array.Length);
                    fxDiffuseMap.SetResource(((ShapeWithTextureBase)shapes[j]).ShapeTexture);
                    fxWVP.SetMatrix(shapes[j].ShapeWorld * view * projection);
                    fxTT.SetMatrix(Matrix.Identity);

                    effectTechnique.GetPassByIndex(i).Apply(ImmediateContext);
                    ImmediateContext.DrawIndexed(shapes[j].IndexCount, 0, 0);
                }
            }
            SwapChain.Present(0, PresentFlags.None);
        }

        public override void OnResize()
        {
            base.OnResize();
            projection = Matrix.PerspectiveFovLH(0.25f * MathF.PI, AspectRatio, 1, 1000);
        }

        public override void UpdateScene(float dt)
        {
            base.UpdateScene(dt);

            // Get camera position from polar coords
            var x = radius * MathF.Sin(phi) * MathF.Cos(theta);
            var z = radius * MathF.Sin(phi) * MathF.Sin(theta);
            var y = radius * MathF.Cos(phi);

            // Build the view matrix
            var pos = new Vector3(x, y, z);
            var target = new Vector3(0);
            var up = new Vector3(0, 1, 0);
            view = Matrix.LookAtLH(pos, target, up);

            eyePosW = pos;

            foreach (ShapeBase sb in shapes)
            {
                if (sb is AnimatedShape)
                {
                    AnimatedShape shape = (AnimatedShape)sb;

                    if (shape.Bouncing)
                    {
                        shape.Bounce(shape.BounceFloor, shape.BounceCeiling);
                    }

                    if (shape.BounceTranslate)
                    {
                        shape.BounceAndTranslate(shape.BounceFloor, shape.BounceCeiling, shape.TranslateTarget);
                    }
                }

                camera.UpdateViewMatrix();
            }
        }

        protected override void OnMouseDown(object sender, MouseEventArgs e)
        {
            lastMousePosition = e.Location;
            Window.Capture = true;
        }

        protected override void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                var dx = MathF.ToRadians(0.25f * (e.X - lastMousePosition.X));
                var dy = MathF.ToRadians(0.25f * (e.Y - lastMousePosition.Y));

                theta += dx;
                phi += dy;

                phi = MathF.Clamp(phi, 0.1f, MathF.PI - 0.1f);
            }
            else if (e.Button == MouseButtons.Right)
            {
                var dx = 0.01f * (e.X - lastMousePosition.X);
                var dy = 0.01f * (e.Y - lastMousePosition.Y);
                radius += dx - dy;

                radius = MathF.Clamp(radius, 1.0f, 35.0f);
            }
            lastMousePosition = e.Location;
        }

        protected override void OnMouseUp(object sender, MouseEventArgs e)
        {
            Window.Capture = false;
        }

        public void SetShapeTexture(ShapeWithTextureBase s, string path)
        {
            s.SetTexture(Device, path);
        }

        private void BuildFX()
        {
            ShaderBytecode compiledBytecode = ShaderBytecode.CompileFromFile(@"D:\3Shape\SlimDX Antonio\SlimDX Tutorial\ShapesDX\Basic.fx", "fx_5_0", ShaderFlags.None, EffectFlags.None);
            effect = new Effect(Device, compiledBytecode);

            effectTechnique = effect.GetTechniqueByName("Light1Tex");
            fxWVP = effect.GetVariableByName("gWorldViewProj").AsMatrix();
            fxW = effect.GetVariableByName("gWorld").AsMatrix();
            fxWIT = effect.GetVariableByName("gWorldInvTranspose").AsMatrix();
            fxTT = effect.GetVariableByName("gTexTransform").AsMatrix();
            fxMaterial = effect.GetVariableByName("gMaterial");
            fxEyePosW = effect.GetVariableByName("gEyePosW").AsVector();
            fxDirLights = effect.GetVariableByName("gDirLights");
            fxDiffuseMap = effect.GetVariableByName("gDiffuseMap").AsResource();

            Util.ReleaseCom(ref compiledBytecode);
        }

        private void BuildVertexLayout()
        {
            var vertexDesc = new[]
            {
                new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0, InputClassification.PerVertexData, 0),
                new InputElement("NORMAL", 0, Format.R32G32B32_Float, 12, 0, InputClassification.PerVertexData, 0),
                new InputElement("TEXCOORD", 0, Format.R32G32_Float, 24, 0, InputClassification.PerVertexData, 0)
            };

            var passDesc = effectTechnique.GetPassByIndex(0).Description;
            inputLayout = new InputLayout(Device, passDesc.Signature, vertexDesc);
        }

        public ShapeBase AddBox(float w, float h, float d, float x, float y, float z)
        {
            Box b = new Box(Device, w, h, d, x, y, z);
            shapes.Add(b);
            return b;
        }

        public ShapeBase AddSphere(float r, int slice, int stack, float x, float y, float z)
        {
            Sphere s = new Sphere(Device, r, slice, stack, x, y, z);
            shapes.Add(s);
            return s;
        }

        public ShapeBase AddCylinder(float bottomR, float topR, float height, int sliceCount, int stackCount, float x, float y, float z)
        {
            Cylinder c = new Cylinder(Device, bottomR, topR, height, sliceCount, stackCount, x, y, z);
            shapes.Add(c);
            return c;
        }

        public ShapeBase AddGrid(float w, float d, int rows, int cols, float x, float y, float z)
        {
            Grid g = new Grid(Device, w, d, rows, cols, x, y, z);
            shapes.Add(g);
            return g;
        }
    }
}
