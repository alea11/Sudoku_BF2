using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DancingLinks
{
    public class DlColumn
    {
        internal DlColumn _next = null;
        internal DlColumn _prev = null;
        internal DlCell _firstCell = null;
        internal DlCell _lastCell = null;
        internal int CellCount = 0;

        internal DlColumnsSection _section; //секция, с определенным условием актуальности соответствующей ячейки

        // ключевые параметры столбца (в зависимости от решаемой задачи - могут применяться не все)        
        internal int[] _position; // часть условия - позиция элемента (компоненты позиции). Замечание: если в частичном решении участвует несколько позиций - то это отмечяется несколько соответствующих столбцов
        internal int? _valueOption; //часть условия - значение элемента, может отсутствовать

        internal int Number; // номер, только для отладки

        public DlColumn(DlColumnsSection section, int[] position, int? valueOption)
        {
            _section = section;
            _position = position;
            _valueOption = valueOption;
        }

        internal void AddCell(DlCell cell)
        {
            if (_firstCell == null)
                _firstCell = cell;
            if(_lastCell != null)
            {
                _lastCell._nextInColumn = cell;
            }
            cell._prevInColumn = _lastCell;
            _lastCell = cell;

            CellCount++;
        }

        public override string ToString()
        {
            string spos = _position.Length == 2 ? $"{_position[0]},{_position[1]}" : $"{_position[0]}";
            string sval = _valueOption == null ? "-" : $"{_valueOption}";
            return $"№ {Number} : {spos} - {sval}";
        }
    }
}
