using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shapes;
using ShapesApp = Shapes.ShapesApp;
using SlimDX;
using System.Diagnostics;

namespace ShapesDXApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Configuration.EnableObjectTracking = true;
            var app = new ShapesApp(Process.GetCurrentProcess().Handle);

            if (!app.Init())
            {
                return;
            }

            Grid g = (Grid) app.AddGrid(20, 20, 20, 20, 0, 0, 0);
            g.SetMaterial(new Color4(0.5f, 0.5f, 0.5f), new Color4(1, 1, 1), new Color4(16.0f, 0.6f, 0.6f, 0.6f));
            app.SetShapeTexture(g, @"D:\3Shape\SlimDX Antonio\SlimDX Tutorial\ShapesDXApp\grass.dds");

            Sphere s = (Sphere)app.AddSphere(1, 10, 10, 0, 1, 0);
            s.SetMaterial(new Color4(0.5f, 0.5f, 0.5f), new Color4(1, 1, 1), new Color4(16.0f, 0.6f, 0.6f, 0.6f));
            app.SetShapeTexture(s, @"D:\3Shape\SlimDX Antonio\SlimDX Tutorial\ShapesDXApp\stone.dds");

            s.Bounce(1, 3);

            app.Run();
        }
    }
}
