using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DancingLinks
{
    public class DlColumnsSection
    {
        public readonly int[] CoordinatesRange; //диапазон (длина) соотвествующих размерностей, например в пространстве - размеры этого пространства по осям 
        public readonly int? ValueRange; //диапазон значений, количество (если есть). Конкретные значения предполагаются равными индексу в этом диапазоне. Но если нужно - можно на клиенте мапить в другие величины
        internal readonly DlColumn[] Columns;
        internal readonly Func<DlParameters, int[], int?, bool> Rule; // правило актуальности ячейки - по параметрам соотв. строки и столбца

        public DlColumnsSection(int[] coordinatesRange, int? valueRange, Func<DlParameters, int[], int?, bool> rule)
        {
            CoordinatesRange = coordinatesRange; 
            ValueRange = valueRange; 

            Columns = CreateColumns();
            Rule = rule;
        }

        private DlColumn[] CreateColumns()
        {           
            int len = 1;
            int d = 0;
            if(CoordinatesRange != null)
            {
                for(; d<CoordinatesRange.Length;d++)
                {
                    len *= CoordinatesRange[d];                    
                }
            }
            if (ValueRange != null)
            {
                len *= ValueRange.Value;
            }
            DlColumn[] columns = new DlColumn[len];

            // массив для перебора значений координат различных осей для очередной позиции (если координаты применяются в этой секции)
            int[] coordinates = CoordinatesRange == null ? null: new int[CoordinatesRange.Length];
            // переменная для перебора варианта величины (если величина применяется в этой секции)
            int? val = ValueRange == null? null: (int?)0;

            for (int i = 0; i< len; i++)
            {
                bool increasePosition = false;
                if (ValueRange != null)
                {
                    val++;
                    if (val > ValueRange)
                    {
                        val = 1;
                        increasePosition = true;
                    }
                }
                else if(i>0)
                    increasePosition = true;

                int[] position = null;

                // накопление значений координат по осям. если на очередной оси нет переполнения - выход из цикла
                if(coordinates != null)
                {
                    if(increasePosition)
                    {
                        for(int axis = coordinates.Length-1; axis>=0; axis--)
                        {
                            coordinates[axis]++;
                            if (coordinates[axis] == CoordinatesRange[axis])
                                coordinates[axis] = 0;
                            else
                                break;
                        }
                    }
                    position = new int[coordinates.Length];
                    for (int axis = 0; axis < coordinates.Length; axis++)
                        position[axis] = coordinates[axis];

                }
                
                // создание столбца с соответствующими параметрами
                columns[i] = new DlColumn(this, position, val);
            }

            return columns;
        }

       
    }
}
