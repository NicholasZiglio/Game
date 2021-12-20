using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace Game
{
    public class GameInfo : GH_AssemblyInfo
    {
        public override string Name => "Game";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => null;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "";

        public override Guid Id => new Guid("9C273DA9-D6AF-477B-8DF6-1525DC9C22BA");

        //Return a string identifying you or your company.
        public override string AuthorName => "";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "";
    }
}