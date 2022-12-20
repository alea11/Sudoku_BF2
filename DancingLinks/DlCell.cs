using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DancingLinks
{
    public class DlCell
    {
        internal DlRow _row;
        internal DlColumn _column;

        internal DlCell _prevInRow = null;
        internal DlCell _nextInRow = null;
        internal DlCell _prevInColumn = null;
        internal DlCell _nextInColumn = null;

        internal bool IsDeleted = false;

        public DlCell(DlRow row, DlColumn column)
        {
            _row = row;
            _column = column;
        }

        public override string ToString()
        {
            string l = _prevInRow != null ? _prevInRow._column.Number.ToString() : "-";
            string r = _nextInRow != null ? _nextInRow._column.Number.ToString() : "-"; ;
            string u = _prevInColumn != null ? _prevInColumn._row.Number.ToString() : "-";
            string d = _nextInColumn != null ? _nextInColumn._row.Number.ToString() : "-";
            return $"{_row.Number} x {_column.Number} - {l} <--> {r} / {u} u-d {d}";  
        }
    }
}
