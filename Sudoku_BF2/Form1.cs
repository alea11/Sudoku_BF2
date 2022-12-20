using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Sudoku_BF
{
    public partial class Form1 : Form
    {
        TextBox[,] gridTxts;
        Font _boldFont;

        Worker _worker;
        int _a;
        int _n;

        List<SolutionItem> InitialItems = new List<SolutionItem>();
        bool _initialItemsEditEnabled = true;

        public Form1()
        {
            InitializeComponent();            

            _boldFont = new Font(Font, FontStyle.Bold);

            SetSize(3);            
        }

        private void InitWorkerDL()
        {
            if(_worker != null)
            {
                _worker.OnMore -= _worker_OnMore;
                _worker.OnSolution -= _worker_OnSolution;
                _worker.OnFail -= _worker_OnFail;
                _worker.OnDone -= _worker_OnDone;
            }

            switch((eMethod)cmbMethod.SelectedItem)
            {
                //case eMethod.BF_Classic:
                //    _worker = new WorkerBF(_a);
                //    break;
                case eMethod.Dancing_Links:
                    _worker = new WorkerDL(_a);
                    break;
                //case eMethod.Dancing_Indexes:
                //    _worker = new WorkerBF(_a);
                //    break;
            }
            
            _worker.OnMore += _worker_OnMore;
            _worker.OnSolution += _worker_OnSolution;
            _worker.OnFail += _worker_OnFail;
            _worker.OnDone += _worker_OnDone;
        }




        private void SetSize(int a)
        {
            _a = a;
            _n = _a * _a;
            this.Text = $"Решение судоку {_n} x {_n}";

            pnlGrid.Visible = false;

            //Thread.Sleep(10);
            this.SuspendLayout();
            pnlGrid.SuspendLayout();
            Thread.Sleep(10);

            if (gridTxts != null)
            {
                foreach (TextBox t in gridTxts)
                    pnlGrid.Controls.Remove(t);
            }

            gridTxts = new TextBox[_n, _n];

            if (_a <= 8)
            {
                int minWidth = 260;
                Width = _n * 22 + (_a - 1) * 5 + 53;
                if (Width < minWidth)
                    Width = minWidth;
                Height = _n * 22 + (_a - 1) * 5 + 242;
            }            
            else
                throw new Exception("Размер более чем 8х8 - не поддерживается.");

            //pnlService.Top = Height - 192;
            int left = 20;
            int top = 20;

            // секционные смещения
            int dx = 0;
            int dy = 0;

            for (int r = 0; r < _n; r++)
            {
                dy = r / _a * 5;
                for (int c = 0; c < _n; c++)
                {
                    dx = c / _a * 5;
                    TextBox t = new TextBox();
                    t.Name = $"txt{r}{c}";                    
                    
                    t.Size = new Size(20, 20);
                    t.Location = new Point(left + c * 22 + dx, top + r * 22 + dy);

                    if (_a <= 3)
                        t.MaxLength = 1;
                    
                    else if (_a <= 10)
                    {                        
                        t.MaxLength = 2;
                    }

                    t.TextAlign = HorizontalAlignment.Center;
                    t.KeyPress += Txt_KeyPress;
                    t.ForeColor = Color.DarkRed;
                    t.Font = _boldFont;
                    pnlGrid.Controls.Add(t);
                    gridTxts[r, c] = t;
                }
            }
            //Thread.Sleep(10);
            //pnlGrid.Refresh();
            pnlGrid.ResumeLayout();
            this.ResumeLayout();
            //Thread.Sleep(1000);
            pnlGrid.Visible = true;

        }        

        private void _worker_OnFail(object sender, EventArgs e)
        {
            if (InvokeRequired)
                Invoke(new Action(_setFail));
            else
                _setFail();
        }

        private void _setFail()
        {
            lblRes.Text = "Нет решения";
            SetControls(eState.Stopped);
        }

        private void _worker_OnSolution(object sender, EventArgs e)
        {
            if (InvokeRequired)
                Invoke(new Action(_setSolution));
            else
                _setSolution();
        }

        private void _setSolution()
        {
            // отображаем решение
            SuspendLayout();            
            for (int r = 0; r < _n; r++)
                for (int c = 0; c < _n; c++)
                    gridTxts[r, c].Text = _worker.Grid[r, c].ToString();
            ResumeLayout();
        }

        private void _worker_OnMore(object sender, NumEventArgs e)
        {
            if (InvokeRequired)
                Invoke(new Action<int>(_setMore), e.Num);
            else
                _setMore(e.Num);
        }

        private void _setMore(int num)
        {
            lblCount.Text = num.ToString();
        }
        

        private void _worker_OnDone(object sender, EventArgs e)
        {
            if (InvokeRequired)
                Invoke(new Action(_setDone));
            else
                _setDone();
        }

        private void _setDone()
        {
            
            lblRes.Text = "Готово. К-во решений:";
            SetControls(eState.Stopped);
        }

        private void Txt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (_a <= 3)
                e.Handled = (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar)) || (e.KeyChar == '0');
            else
                e.Handled = (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar));
        }

        private void TxtA_KeyPress(object sender, KeyPressEventArgs e)
        {           
            // только цифры или служебные, и не 0
            e.Handled = (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar)) || (e.KeyChar == '0');
        }

        private void btnWork_Click(object sender, EventArgs e)
        {
            Upload();
            UploadToWorker();
            string errMsg;
            if(_worker.CheckGrid(out errMsg))
            {

                lblRes.Text = "            решение:";
                _initialItemsEditEnabled = false;
                SetTxtStyle(true);

                btnStop.Enabled = true;


                Thread st = new Thread(_worker.Solve);
                st.IsBackground = true;
                st.Start();
             
                SetControls(eState.Started);
            }
            else
            {
                lblErr.Text = errMsg;
            }
        }

        

        private void SetTxtStyle(bool locking, bool clear = true)
        {
            SuspendLayout();
            for (int r = 0; r < _n; r++)
                for (int c = 0; c < _n; c++)
                {
                    TextBox t = gridTxts[r, c];

                    if (locking)
                    {
                        t.ReadOnly = true;
                        t.BackColor = Color.White;
                        if (t.Text == "")
                        {
                            t.ForeColor = Color.DarkBlue;
                            t.Font = this.Font;
                        }
                    }
                    else
                    {
                        t.ReadOnly = false;
                        if(clear)   
                            t.Text = "";
                        t.ForeColor = Color.DarkRed;
                        t.Font = _boldFont;
                    }

                }
            ResumeLayout();
        }

        private void Upload()
        { 
            if(_initialItemsEditEnabled)
            {
                InitialItems.Clear();

                for (int r = 0; r < _n; r++)
                    for (int c = 0; c < _n; c++)
                    {
                        if (gridTxts[r, c].Text != "")                        
                        {
                            byte val = byte.Parse(gridTxts[r, c].Text);
                            InitialItems.Add(new SolutionItem((byte)c, (byte)r, val));                        
                        }
                    }
            }
            
        }

        private void UploadToWorker()
        {            
            for (int r = 0; r < _n; r++)
                for (int c = 0; c < _n; c++)
                    _worker.Grid[r,c] = 0;

            foreach(var item in InitialItems)
            {
                _worker.Grid[item.Y, item.X] = item.Val;
            }
        }

        private void btnToInit_Click(object sender, EventArgs e)
        {
            InitWorkerDL();
            
           
            for (int r = 0; r < _n; r++)
                for (int c = 0; c < _n; c++)
                    gridTxts[r, c].Text = "";
            foreach (var item in InitialItems)
            {
                gridTxts[item.Y, item.X].Text = item.Val.ToString();
            }
            SetTxtStyle(false, false);
            _initialItemsEditEnabled = true;
            
            SetControls(eState.ReadyToStart);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            Clear();
        }

        private void Clear()
        {
            _initialItemsEditEnabled = true;
            InitialItems.Clear();
            InitWorkerDL();
            
                     

            for (int r = 0; r < _n; r++)
            {
                for (int c = 0; c < _n; c++)
                {
                    _worker.Grid[r, c] = 0;
                }
            }
                
            SetTxtStyle(false);
            SetControls(eState.ReadyToStart);
        }

        private void SetControls(eState state)
        {
            //btnSave.Visible = _a > 2;
            switch(state)
            {
                case eState.ReadyToStart:
                    btnWork.Enabled = true;
                    btnStop.Enabled = false;
                    btnToInit.Enabled = false;
                    btnResize.Enabled = true;
                    btnClear.Enabled = true;
                    btnSave.Enabled = true;
                    btnLoad.Enabled = true;
                    lblCount.Text = "";
                    lblRes.Text = "";
                    lblErr.Text = "";
                    break;
                case eState.Started:
                    btnWork.Enabled = false;
                    btnStop.Enabled = true;
                    btnToInit.Enabled = false;
                    btnResize.Enabled = false;
                    btnClear.Enabled = false;
                    btnSave.Enabled = false;
                    btnLoad.Enabled = false;
                    lblErr.Text = "";
                    break;
                case eState.Stopped:
                    btnWork.Enabled = false;
                    btnStop.Enabled = false;
                    btnToInit.Enabled = true;
                    btnResize.Enabled = true;
                    btnClear.Enabled = true;
                    btnSave.Enabled = false;
                    btnLoad.Enabled = false;
                    break;
            }
        }

        private void btnResize_Click(object sender, EventArgs e)
        {                      
            _initialItemsEditEnabled = true;

            int a;
            if (!int.TryParse(txtA.Text, out a))
                a = 3;

            SetSize(a);
            InitialItems.Clear();
            InitWorkerDL();
            SetControls(eState.ReadyToStart);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmbMethod.DataSource = Enum.GetValues(typeof(eMethod));
            cmbMethod.SelectedItem = eMethod.Dancing_Links;
            txtA.Text = _a.ToString();
            LoadTmp();

            SetControls(eState.ReadyToStart);
        }

        private void cmbMethod_SelectedIndexChanged(object sender, EventArgs e)
        {
            //Clear();
            InitialItems.Clear();
            InitWorkerDL();
            UploadToWorker();
            //SetTxtStyle(false);

            SetControls(eState.ReadyToStart);
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            _worker.Stop();
            lblRes.Text = "Прервано. К-во решений:";
            SetControls(eState.Stopped);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "файлы Судоку  | *.stxt";
            sfd.DefaultExt = "stxt";
            if(sfd.ShowDialog() == DialogResult.OK)
            {
                using (StreamWriter sw = new StreamWriter(sfd.FileName, false))
                {
                    sw.WriteLine($"#{_a}");
                    for (int r = 0; r < _n; r++)
                        for (int c = 0; c < _n; c++)
                        {
                            if (gridTxts[r, c].Text != "")
                            {                                
                                sw.WriteLine($"{r+1} {c+1} {gridTxts[r, c].Text}");
                            }
                        }
                }
                    
            }
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "файлы Судоку  | *.stxt";
            ofd.DefaultExt = "stxt";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                using (StreamReader sr = new StreamReader(ofd.FileName))
                {
                    string line = sr.ReadLine();
                    int a;
                    if(line.Length == 2 && line[0]=='#' && int.TryParse(line.Substring(1), out a))
                    {
                        if(a!=_a)
                        {
                            MessageBox.Show($"Размерность ({a}) загружаемого варианта не совпадает с текущей.");
                            return;
                        }
                        List<SolutionItem> items = new List<SolutionItem>();
                        while ( !string.IsNullOrEmpty(line = sr.ReadLine()))
                        {
                            string[] arr = line.Split(' ');
                            if(arr.Length != 3)
                            {
                                MessageBox.Show("Неизвестный формат файла");
                                return;
                            }
                            int r, c, v;
                            if( !int.TryParse(arr[0], out r) || !int.TryParse(arr[1], out c) || !int.TryParse(arr[2], out v) )
                            {
                                MessageBox.Show("Неизвестный формат файла");
                                return;
                            }
                            items.Add(new SolutionItem((byte)(c-1), (byte)(r-1), (byte)v));
                        }

                        for (int r = 0; r < _n; r++)
                            for (int c = 0; c < _n; c++)
                                gridTxts[r, c].Text = "";
                        foreach (SolutionItem item in items)
                        {
                            gridTxts[item.Y, item.X].Text = item.Val.ToString();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Неизвестный формат файла");
                    }
                }
            }



        }



        private void LoadTmp()
        {
            byte[,] tmp = null;
            switch(_n)
            {
                case 4:
                    break;
                case 9:
                    tmp = new byte[9, 9] {  { 0, 0, 0, 0, 0, 0, 6, 8, 0 },
                                            { 0, 0, 0, 0, 7, 3, 0, 0, 9 },
                                            { 3, 0, 9, 0, 0, 0, 0, 4, 5 },
                                            { 4, 9, 0, 0, 0, 0, 0, 0, 0 },
                                            { 8, 0, 3, 0, 5, 0, 9, 0, 2 },
                                            { 0, 0, 0, 0, 0, 0, 0, 3, 6 },
                                            { 9, 6, 0, 0, 0, 0, 3, 0, 8 },
                                            { 7, 0, 0, 6, 8, 0, 0, 0, 0 },
                                            { 0, 2, 8, 0, 0, 0, 0, 0, 0 }};
                    break;
                case 25:
                    tmp = new byte[25, 25] {{ 01, 02, 03, 04, 05, 06, 07, 08, 09, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25 },
                                            { 06, 07, 08, 09, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 01, 02, 03, 04, 05 },
                                            { 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 01, 02, 03, 04, 05, 06, 07, 08, 09, 10 },
                                            { 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 01, 02, 03, 04, 05, 06, 07, 08, 09, 10, 11, 12, 13, 14, 15 },
                                            { 21, 22, 23, 24, 25, 01, 02, 03, 04, 05, 06, 07, 08, 09, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20 },
                                            { 02, 03, 04, 05, 06, 07, 08, 09, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 01 },
                                            { 07, 08, 09, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 01, 02, 03, 04, 05, 06 },
                                            { 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 01, 02, 03, 04, 05, 06, 07, 08, 09, 10, 11 },
                                            { 17, 18, 19, 20, 21, 22, 23, 24, 25, 01, 02, 03, 04, 05, 06, 07, 08, 09, 10, 11, 12, 13, 14, 15, 16 },
                                            { 22, 23, 24, 25, 01, 02, 03, 04, 05, 06, 07, 08, 09, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21 },
                                            { 03, 04, 05, 06, 07, 08, 09, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 01, 02 },
                                            { 08, 09, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 01, 02, 03, 04, 05, 06, 07 },
                                            { 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 01, 02, 03, 04, 05, 06, 07, 08, 09, 10, 11, 12 },
                                            { 18, 19, 20, 21, 22, 23, 24, 25, 01, 02, 03, 04, 05, 06, 07, 08, 09, 10, 11, 12, 13, 14, 15, 16, 17 },
                                            { 23, 24, 25, 01, 02, 03, 04, 05, 06, 07, 08, 09, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22 },
                                            { 04, 05, 06, 07, 08, 09, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 01, 02, 03 },
                                            { 09, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 01, 02, 03, 04, 05, 06, 07, 08 },
                                            { 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 01, 02, 03, 04, 05, 06, 07, 08, 09, 10, 11, 12, 13 },
                                            { 19, 20, 21, 22, 23, 24, 25, 01, 02, 03, 04, 05, 06, 07, 08, 09, 10, 11, 12, 13, 14, 15, 16, 17, 18 },
                                            { 24, 25, 01, 02, 03, 04, 05, 06, 07, 08, 09, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 },
                                            { 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00 },
                                            { 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00 },
                                            { 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00 },
                                            { 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00 },
                                            { 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00, 00 }};

                    break;
            }

        
            if(tmp != null)
            {
                for (int r = 0; r < _n; r++)
                    for (int c = 0; c < _n; c++)
                    {
                        if(tmp[r, c]>0)
                            gridTxts[r, c].Text = tmp[r, c].ToString();
                    }

            }

            
                    
        }

        private enum eMethod
        {
            //BF_Classic,
            Dancing_Links,
            //Dancing_Indexes

        }

        private enum eState
        {
            ReadyToStart,
            Started,
            Stopped,
        }

        private struct SolutionItem
        {
            public readonly byte X;
            public readonly byte Y;
            public readonly byte Val;
            public SolutionItem(byte x, byte y, byte val)
            {
                X = x;
                Y = y;
                Val = val;
            }            
        }

        
    }
}
