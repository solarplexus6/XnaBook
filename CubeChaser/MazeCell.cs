﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CubeChaser
{
    class MazeCell
    {
        public bool[] Walls = new bool[4] { true, true, true, true };
        public bool Visited = false;
    }
}
