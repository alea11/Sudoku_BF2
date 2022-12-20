using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DancingLinks
{
    /// <summary>
    /// Стек с возможностью перечислить элементы без выемки
    /// </summary>
    public class DlRowsStack : IEnumerable<DlRow>
    {
        int _capacity;
        internal int _size;
        DlRow[] rows;

        internal DlRowsStack(int capacity)
        {
            _capacity = capacity;
            Clear();
        }

        internal bool Empty
        { get { return _size == 0; } }

        internal void Push(DlRow row)
        {  rows[_size++] = row; }

        internal DlRow Pop()
        { return rows[--_size]; }

        internal DlRow Peek()
        { return rows[_size-1]; }

        internal void Clear()
        {
            rows = new DlRow[_capacity];
            _size = 0;
        }

        public IEnumerator<DlRow> GetEnumerator()
        {
            for (int i = _size - 1; i >= 0; i--)
                yield return rows[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
