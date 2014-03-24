using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace ListaNomesLib
{
    public interface IListaNomes
    {
        void addName(string name);
        ArrayList getNames();
        void clean();
    }

    public class ListaNomes : IListaNomes
    {
        ArrayList namesList;

        public ListaNomes()
        {
            namesList = new ArrayList();
        }

        public void addName(string name)
        {
            namesList.Add(name);
        }

        public ArrayList getNames()
        {
            return namesList;
        }

        public void clean()
        {
            namesList.Clear();
        }
    }
}
