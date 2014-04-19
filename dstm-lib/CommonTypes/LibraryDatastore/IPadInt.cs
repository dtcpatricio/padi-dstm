using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonTypes.LibraryDatastore
{
    public interface IPadInt
    {
        int Read();
        void Write(int val);
    }
}
