using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DancingLinks
{
    public class DlMatrix
    {
        public int RowsCount = 0;
        public int ColumnsCount = 0;

        private DlRow _firstRow = null;
        private DlColumn _firstColumn = null;
        private DlRow _lastRow = null;
        private DlColumn _lastColumn = null;

        DlRowsStack ProcessingStack; // стек строк на обработку - реализация алгоритма обхода в глубину
        DlRowsStack SolutionsStack; // стек строк отобранных частичных решений обрабатываемой ветки решений
        DlRowsStack ExclusionsStack; // стек конкурирующих исключаемых строк

        private volatile bool _cancelToken = false;
        public bool IsCanceled { get { return _cancelToken; } }

        public DlMatrix()
        {   
            _cancelToken = false;
            Done = false;
        }

        public void Cancel()
        {
            _cancelToken = true;
        }

        public IEnumerable<DlRow> GetRows()
        {
            DlRow row = _firstRow;
            while(row != null)
            {
                yield return row;
                row = row.Next;
            }
        }

        

        #region Формирование матрицы

        /// <summary>
        /// стартовое формирование матрицы - добавление очередной секции столбцов, с указанием ключевых парамтров
        /// </summary>       
        /// <param name="coordinatesRange">диапазон (длина) соотвествующих размерностей, например в пространстве - размеры этого пространства по осям</param>
        /// <param name="valueRange">диапазон значений, количество (если есть). Конкретные значения предполагаются равными индексу в этом диапазоне. Но если нужно - можно на клиенте мапить в другие величины</param>
        /// <returns></returns>
        public void AddColumnsSection(int[] coordinatesRange, int? valueRange, Func<DlParameters, int[], int?, bool> rule)
        {
            DlColumnsSection section = new DlColumnsSection(coordinatesRange, valueRange, rule);
            int n = 0;
            foreach(DlColumn column in section.Columns)
            {
                if(_lastColumn != null)
                {
                    _lastColumn._next = column;
                    column._prev = _lastColumn;
                }
                _lastColumn = column;
                if (_firstColumn == null)
                    _firstColumn = column;
                column.Number = ColumnsCount + n;
                n++;
            }
            ColumnsCount += section.Columns.Length;
        }


        /// <summary>
        /// создание строки
        /// </summary>
        /// <param name="parameters">массив параметров частичного решения: каждый элемент - компоненты условия: позиции (возможны несколько компонент), возможны позиции нескольких элементов, и вариант значения элемента (элементов)</param>
        public DlRow AddRow(DlParameters parameters) //, int? idx=null
        {
            DlRow row = new DlRow(parameters);
            if (_lastRow != null)
            {
                _lastRow.Next = row;
                row.Prev = _lastRow;
            }
            _lastRow = row;
            if (_firstRow == null)
                _firstRow = row;
            row.Number = RowsCount;
            RowsCount++;
            //if (idx != null)
            //    Rows[idx.Value] = row;
            return row;
        }

        

        public void Prepare()
        {
            ProcessingStack = new DlRowsStack(RowsCount);
            SolutionsStack = new DlRowsStack(RowsCount);
            ExclusionsStack = new DlRowsStack(RowsCount);

            FillRows();
        }

        /// <summary>
        ///  заполнение значащих ячеек матрицы
        /// </summary>
        void FillRows()
        {
            DlRow row = _firstRow;
            while(row != null)
            {
                DlColumn col = _firstColumn;
                while(col != null)
                {
                    // оформляем ячейку, если срабытывает секционное правило
                    if(col._section.Rule(row.Parameters, col._position, col._valueOption))
                    {
                        DlCell cell = new DlCell(row, col);
                        row.AddCell(cell);
                        col.AddCell(cell);
                    }
                    col = col._next;
                } 
                row = row.Next;
            }
        }

        #endregion Формирование матрицы

        #region Установка начальных значений

        /// <summary>
        /// Установка очередного начального значения - исключение соответствующей строки из матрицы, столбцов, задействованных в этой строке и строк, содержащих ячейки удаляемых столбцов
        /// </summary>
        /// <param name="parameters"></param>
        public void SetInitialSolutionsRow(DlRow row)
        {           
            SetSolvedRow(row, initial:true);
        }

        #endregion Установка начальных значений


        #region Перебор частичных решений, накопление результата

        ////////////////////////////////////////////////////////////////////////
        // организация нерекурсивного варианта алгоритма поиска в глубину:
        //
        // ЦИКЛ:
        //
        //      цикл завершается при отсутствии столбцов, тогда результат - в стеке частичных решений
        //
        //      находим столбец с минимальным числом ячеек
        //      если столбец пустой (либо, возможно, другой вариант невозможности выбора очередного решения) :
        //          откат из стеков решений и удалений (из стека удалений достаем CompetingRows строк основной строки).
        //          восстановление соответствующих столбцов и строк (см. '* восстановление')
        //          - повторяем откаты пока в очередном пакете строк восстановленных из стека удалений не увидим строку, которая лежит сверху в операционном стеке
        //      иначе (столбец с ячейками):
        //          все строки содержащие ячейки столбца помещаем в операционный стек  в обратном порядке
        //
        //      получаем очередную строку из операционного стека
        // 
        //      сохраняем очередную строку в стеке частичных решений, а конкурирующие строки сохраняем в стеке удалений
        //
        //      удаляем эту строку и конкурирующие строки, а также все столбцы содержащие ячейки в текущей строке (тоько из списка столбцов, без обработки ячеек, т.к. ячеек у этих столбцов после удаления строк - уже нет)
        //
        // переход в начало цикла 

        // * восстановление:
        // получаем верхнюю строку из стека решений
        // восстанавливаем ее слолбцы (без обработки ячеек)
        // восстанавливаем эту строку и полученный набор конкурирующих строк из стека удалений (количество строк сохранено в основной строке (поле CompetingRows))
        //
        ////////////////////////////////////////////////////////////////////////



        internal bool FindMinColumn(out DlColumn minColumn)
        {
            DlColumn column = _firstColumn;
            minColumn = column;
            while(column != null)
            {
                if(column.CellCount < minColumn.CellCount)
                    minColumn = column;
                if (minColumn.CellCount == 0)
                    return false;
                column = column._next;
            }
            return true;
        }

        public bool Done { get;  private set; }


        public IEnumerable<DlParameters> Solve()
        {
            if (ProcessingStack.Empty && _firstColumn == null)// уже отсутствуют колонки в матрице при пустом стеке операций (все отработано) 
            {
                Done = true;
            }

            else
            {
                // предварительно откатываем предыдущую комбинацию решений (если чтото есть в ProcessingStack, то это не первый запуск в серии)
                if (!ProcessingStack.Empty)
                {
                    //верхняя строка, в стеке обработки -критерий остановки откатов
                    DlRow nextProcRow = ProcessingStack.Pop();
                    RollbackSolutions(nextProcRow);

                    // и заносим эту строку в стек решений
                    SetSolvedRow(nextProcRow);

                    // далее - входим в общий цикл
                }
                while (!_cancelToken)
                {
                    DlColumn col;
                    if(!FindMinColumn(out col))  // есть пустая колонка - это признак некорректности последнего выбранного частичного решения (или нескольких последних) 
                    {
                        if (SolutionsStack.Empty)
                        {
                            // ошибочная ситуация, неверно поставлена задача (наличие пустой колонки при пустом стеке решений), возможно, не бывает
                            throw new Exception("Наличие пустой колонки при пустом стеке решений");
                        }

                        if (ProcessingStack.Empty)
                        {
                            // операционный стек исчерпан - частичных решений больше нет, общее решение невозможно, выходим из цикла
                            Done = true;
                            break;
                        }

                        // откатываем обратно крайнее частичное решение (или несколько)
                    
                        //верхняя строка, в стеке обработки -критерий остановки откатов
                        DlRow nextProcRow = ProcessingStack.Peek();

                        RollbackSolutions(nextProcRow);                    
                    }

                    else if(col != null)
                    {
                        // заносим  строки, пересекающие эту колонку в операционный стек (в обратном порядке)  - реализация безрекурсивного варианта алгоритма обхода в глубину                  
                        DlCell cell = col._lastCell;
                        while (cell != null)
                        {
                            ProcessingStack.Push(cell._row);
                            cell = cell._prevInColumn;
                        }
                    }
                    else
                    {
                        // колонок больше нет - решение собрано, выход
                    
                        foreach(DlRow solRow in SolutionsStack)
                        {
                            yield return solRow.Parameters;     
                        }
                    
                        break;
                    }

                    // достаем из операционного стека очередную строку
                    // и фиксируем ее в качестве очередного решения
                    DlRow row = ProcessingStack.Pop();
                    SetSolvedRow(row);
                
                } 
            }                    
        }

        public void RollbackSolutions(DlRow untilProcesingRow)
        {
            // откат из стека решений и соответствующего кол-ва строк из стека удалений
            // и восстановление этих строк в матрице (также восстановление колонок откатываемой строки решения).

            // и откатываемся до тех пор, пока не откатим пакет строк из стека удалений, содержащий эту строку

            while (true)
            {
                DlRow backRow = SolutionsStack.Pop();
                
                Restore(backRow);

                DlCell cell = backRow.FirstCell;
                while (cell != null)
                {
                    Restore(cell._column);
                    cell = cell._nextInRow;
                }

                bool done = false;
                while (backRow.CompetingRows > 0)
                {
                    DlRow competingRow = ExclusionsStack.Pop();
                    Restore(competingRow);
                    if (competingRow == untilProcesingRow)
                        done = true;
                    backRow.CompetingRows--;
                }
                
                // в очередном блоке была восстановлена строка, которая должна обрабоаться первой - откаты прекращаем
                if (done)
                    break;
            }            
        }

        private void SetSolvedRow(DlRow row, bool initial = false)
        {
            // , заносим ее в стек решений, а конкурирующие строки - в стек удалений.
            // все эти строки  выключаем из матрицы, а также слолбцы основной строки
            
            SolutionsStack.Push(row);

            DlCell cell1 = row.LastCell;
            while (cell1 != null)
            {
                DlColumn col1 = cell1._column;
                TurnOff(col1);

                DlCell cell2 = col1._lastCell;
                while (cell2 != null)
                {
                    if (cell2._row != row && !cell2._row.IsDeleted)  //IsDeleted тут возможно только при баге. разбираюсь. потом убрать эту часть условия
                    {
                        ExclusionsStack.Push(cell2._row);
                        TurnOff(cell2._row);
                        cell2._row.InitialUsed = initial;
                        row.CompetingRows++;

                    }
                    cell2 = cell2._prevInColumn;
                }
                cell1 = cell1._prevInRow;
            }

            if (!row.IsDeleted)
                TurnOff(row);

            row.InitialUsed = initial;

            
        }
        #endregion Перебор частичных решений, накопление результата



        #region Выключение, включение строк, столбцов
        internal void TurnOff(DlRow row)
        {
            // исключение (обход, игнор ссылок) строки из матрицы
            if (row.Prev != null)
                row.Prev.Next = row.Next;
            else
                _firstRow = row.Next;

            if (row.Next != null)
                row.Next.Prev = row.Prev;
            else
                _lastRow = row.Prev;

            // исключение (обход, игнор ссылок) ячеек строки из колонок
            DlCell cell = row.FirstCell;
            while(cell != null)
            {                
                if(cell._prevInColumn != null)                
                    cell._prevInColumn._nextInColumn = cell._nextInColumn;
                else
                    cell._column._firstCell = cell._nextInColumn;

                if (cell._nextInColumn != null)
                    cell._nextInColumn._prevInColumn = cell._prevInColumn;
                else
                    cell._column._lastCell = cell._prevInColumn;

                cell.IsDeleted = true;
                cell._column.CellCount--;                
                cell = cell._nextInRow;
            }

            row.IsDeleted = true;
        }

        internal void TurnOff(DlColumn column)
        {
            // исключение (обход, игнор ссылок) колонки из матрицы
            if (column._prev != null)
                column._prev._next = column._next;
            else
                _firstColumn = column._next;

            if (column._next != null)
                column._next._prev = column._prev;
            else
                _lastColumn = column._prev;
            // а из строк ячейки удалять не нужно, т.к. по алгоритму вместе с удалением колонок - удаляются соответствующие строки (сначала строки, потом колонки)
        }

        internal void Restore(DlRow row)
        {
            // восстановление строки в матрице
            if (row.Prev != null)
                row.Prev.Next = row;
            else
                _firstRow = row;

            if (row.Next != null)
                row.Next.Prev = row;
            else
                _lastRow = row;

            // восстановление (переключение ссылок) ячеек строки в колонках
            DlCell cell = row.LastCell;
            while (cell != null)
            {                
                if (cell._prevInColumn != null)
                    cell._prevInColumn._nextInColumn = cell;
                else
                    cell._column._firstCell = cell;

                if (cell._nextInColumn != null)
                    cell._nextInColumn._prevInColumn = cell;
                else
                    cell._column._lastCell = cell;

                cell.IsDeleted = false;
                cell._column.CellCount++;
                cell = cell._prevInRow;
            }
            row.IsDeleted = false;
        }

        internal void Restore(DlColumn column)
        {
            // восстановление колонки в матрице
            if (column._prev != null)
                column._prev._next = column;
            else
                _firstColumn = column;

            if (column._next != null)
                column._next._prev = column;
            else
                _lastColumn = column;            
            // а в строках ячейки восстанавливать не нужно, т.к. не удаляли при удалении колонки
        }

        #endregion Выключение, включение строк, столбцов



        #region For debug
        // получение строки по номеру - для отладки
        private DlRow GetRow(int num)
        {
            DlRow r = _firstRow;
            while (r != null)
            {
                if (r.Number == num)
                    break;
                r = r.Next;
            }
            return r;
        }
        #endregion For debug
    }
}
