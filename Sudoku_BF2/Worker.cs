using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sudoku_BF
{
    public abstract class Worker
    {
        protected int _n;
        protected int _a;
        public  byte[,] Grid; //readonly

        public event EventHandler<NumEventArgs> OnMore;
        public event EventHandler OnSolution;
        public event EventHandler OnFail;
        public event EventHandler OnDone;        

        public Worker(int a)
        {
            _a = a;
            _n = a * a;
            Grid = new byte[_n, _n];
        }

        public bool CheckGrid(out string errMsg)
        {
            errMsg = "";

            // проверка невыходf значений за пределы диапазона
            for (int r = 0; r < _n; r++)
            {
                for (int c = 0; c < _n; c++)
                {
                    if (Grid[r, c] > _n)
                    {
                        errMsg = $"строка: {r + 1}, колонка: {c + 1}, значение: {Grid[r, c]}";
                        return false;
                    }
                        
                }
            }

            // проверка уникальности начальных значений в строках
            for (int r = 0; r < _n; r++)
            {
                for (int c = 0; c < _n - 1; c++)
                {
                    if (Grid[r, c] == 0)
                        continue;
                    for (int i = c + 1; i < _n; i++)
                    {
                        if (Grid[r, i] == 0)
                            continue;
                        if (Grid[r, i] == Grid[r, c])
                        {
                            errMsg = $"строка: {r + 1}, колонки: {c + 1}, {i + 1}";
                            return false;
                        }
                    }                        
                }
            }

            // проверка уникальности начальных значений в столбцах
            for (int c = 0; c < _n; c++)
            {
                for (int r = 0; r < _n - 1; r++)
                {
                    if (Grid[r, c] == 0)
                        continue;
                    for (int i = r + 1; i < _n; i++)
                    {
                        if (Grid[i, c] == 0)
                            continue;
                        if (Grid[r, c] == Grid[i, c])
                        {
                            errMsg = $"колонка: {c + 1}, строки: {r + 1}, {i + 1}";
                            return false;
                        }
                    }  
                }                    
            }

            // проверка уникальности начальных значений в блоках
            for (int b = 0; b < _n; b++) // цикл по блокам
            {
                for (int p = 0; p < _n - 1; p++) // цикл по позициям в блоке
                {
                    int rp = b / _a * _a + p / _a;
                    int cp = b % _a * _a + p % _a;
                    if (Grid[rp, cp] == 0)
                        continue;
                    for (int i = p + 1; i < _n; i++) // цикл по позициям в блоке после контрольной
                    {
                        int ri = b / _a * _a + i / _a;
                        int ci = b % _a * _a + i % _a;
                        if (Grid[ri, ci] == 0)
                            continue;
                        if (Grid[rp, cp] == Grid[ri, ci])
                        {
                            errMsg = $"блок: {b + 1}, клетки: {p + 1}, {i + 1}";
                            return false;
                        }
                    }
                }                    
            }

            return true;
        }

        protected void RaiseMore(int num)
        {
            OnMore?.Invoke(this, new NumEventArgs(num));
        }

        protected void RaiseSolution()
        {
            OnSolution?.Invoke(this, EventArgs.Empty);
        }

        protected void RaiseFail()
        {
            OnFail?.Invoke(this, EventArgs.Empty);
        }             

        protected void RaiseDone()
        {
            OnDone?.Invoke(this, EventArgs.Empty);
        }

        public abstract void Solve();
        public abstract void Stop();
    }

    public class NumEventArgs : EventArgs
    {
        public NumEventArgs(int num)
        {
            Num = num;
        }

        public int Num { get; set; }
    }
}
