using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpTiles_Example1
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            MainGame mainGame = new MainGame();
            mainGame.Run();
        }
    }
}
