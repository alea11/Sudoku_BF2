using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DancingLinks
{
    public class DlRow
    {
        internal DlRow Next = null;
        internal DlRow Prev = null;
        internal DlCell FirstCell = null;
        internal DlCell LastCell = null;


        public readonly DlParameters Parameters;// параметры частичного решения - позиция элемента (компоненты позиции), возможно - позиции нескольких элементов;
                                                    // и значение элемента (-ов) (если актуально) 

        internal int Number; // номер, только для отладки

        internal int CompetingRows=0; // к-во конкурирующих строк, помещаемых в стек удалений при выборе этой строки в качестве частичного решения
        internal bool IsDeleted = false;

        internal bool InitialUsed = false; // признак того, что строка задействована в начальном решении (и в дальнейшем переборе не участвует)
 
        public DlRow(DlParameters parameters)
        {
            Parameters = parameters;
        }

        internal void AddCell(DlCell cell)
        {
            if (FirstCell == null)
                FirstCell = cell;
            if (LastCell != null)
            {
                LastCell._nextInRow = cell;
            }
            cell._prevInRow = LastCell;
            LastCell = cell;
        }

        public override string ToString()
        {
            string coordinates = string.Join(",", Parameters.Locations[0].Coordinates);
            return $"№{Number}: {coordinates}-{Parameters.ValueOption}"; 
        }


    }
}
