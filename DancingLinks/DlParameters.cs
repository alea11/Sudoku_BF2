using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace DancingLinks
{
    public struct DlParameters
    {        
        public Location[] Locations;
        public int? ValueOption;
    }

    public struct Location
    {         
        public int[] Coordinates;
        public Location(params int[] coordinates)
        {
            Coordinates = coordinates;
        }
        
    }

   
}
