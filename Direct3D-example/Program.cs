using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using SharpDX.Direct3D;
using Device11 = SharpDX.Direct3D11.Device;

namespace Para_1
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            if(Device11.GetSupportedFeatureLevel() != FeatureLevel.Level_11_0)
            {
                MessageBox.Show("DirectX11 not Supported");
                return;
            }

            Game game = new Game();
            game.Run();
            game.Dispose();
        }
    }
}
