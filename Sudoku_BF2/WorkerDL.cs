using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DancingLinks;

namespace Sudoku_BF
{
    /// <summary>
    /// реализация с использованием алгоритма Dancing Links 
    /// </summary>
    class WorkerDL : Worker
    {
       
        private DlMatrix _matrix;

        public WorkerDL(int a) : base(a)
        {
            CreateMatrix();
        }

        private void CreateMatrix()
        {
            // матрица частичных решений для судоку  
            _matrix = new DlMatrix();
            
            // секции столбцов матрицы с условиями:
            // правило определяющее значащую ячейку в строке - аргументы: p - параметры строки, c - координаты (или позиция) в параметрах столбца, v - значение в параметрах столбца
            _matrix.AddColumnsSection(new int[] { _n, _n }  , null  
                , rule: (p, c, v) => (p.Locations[0].Coordinates[0] == c[0] && p.Locations[0].Coordinates[1] == c[1])); // занятие определенной ячейки
            _matrix.AddColumnsSection(new int[] { _n }      , _n    
                , rule: (p, c, v) => (p.Locations[0].Coordinates[1] == c[0] && p.ValueOption==v)); // уникальность значения в строчке
            _matrix.AddColumnsSection(new int[] { _n }      , _n    
                , rule: (p, c, v) => (p.Locations[0].Coordinates[0] == c[0] && p.ValueOption == v)); // уникальность значения в столбце
            _matrix.AddColumnsSection(new int[] { _n }      , _n    
                , rule: (p, c, v) => (p.Locations[0].Coordinates[1]/_a * _a + p.Locations[0].Coordinates[0]/_a == c[0] && p.ValueOption == v)); // уникальность значения в блоке
                                                                                                                                                // ( <номер блока> = <номер строки> /_a * _a + <номер колонки> / _a )

            // строки матрицы:
            for(int y = 0; y< _n; y++)
            {
                for (int x = 0; x < _n; x++)
                {
                    for(int v=1; v<=_n; v++)
                    {
                        Location loc = new Location(x,y);
                        DlParameters p = new DlParameters() { Locations = new Location[] { loc }, ValueOption = v };
                        _matrix.AddRow(p);
                    }
                }
            }

            // формирование рабочик стеков и значащих ячеек в колонках, 
            _matrix.Prepare();
        }
        

        public override void Solve()
        {
            //установка нач.условий
            foreach(DlRow row in _matrix.GetRows())            
            {
                Location loc = row.Parameters.Locations[0];
                if (Grid[loc.Coordinates[0], loc.Coordinates[1]] == row.Parameters.ValueOption)
                    _matrix.SetInitialSolutionsRow(row);                
            }

            int count = 0;

            while(true)
            {
                // запуск (и построчный вывод частичных решений (параметров строк из стека решений))
                IEnumerable<DlParameters> solvedRows = _matrix.Solve().ToList();
                if (solvedRows == null || solvedRows.Count() == 0 ) // если нет элементов решения
                {
                    if (!_matrix.IsCanceled)
                    {
                        if (count == 0)
                            RaiseFail();
                        else
                            RaiseDone();
                    }
                    break;
                }                    
                                   
                if (count == 0) // заполняем сетку только для первого прохода, остальные только считаем
                {
                    foreach (DlParameters p in solvedRows)
                    {
                        Grid[p.Locations[0].Coordinates[0], p.Locations[0].Coordinates[1]] = (byte)p.ValueOption.Value;
                    }

                    RaiseSolution();
                    System.Threading.Thread.Sleep(20);
                }

                if(_matrix.Done == true)
                {
                    RaiseDone();
                    break;
                }

                count++;
                
                RaiseMore(count);

            }
            
            

        }

        public override void Stop()
        {
            _matrix.Cancel();
        }


    }
}
