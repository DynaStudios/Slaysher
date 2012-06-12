using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Slaysher
{
    public class SceneException : Exception
    {
        public SceneException(String message)
            : base(message)
        {
        }
    }
}