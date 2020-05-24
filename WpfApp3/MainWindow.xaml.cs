using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;

namespace WpfApp3
{
    [System.Security.SuppressUnmanagedCodeSecurity]
    public static class ConsoleManager
    {
        private const string Kernel32_DllName = "kernel32.dll";

        [DllImport(Kernel32_DllName)]
        private static extern bool AllocConsole();

        [DllImport(Kernel32_DllName)]
        private static extern bool FreeConsole();

        [DllImport(Kernel32_DllName)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport(Kernel32_DllName)]
        private static extern int GetConsoleOutputCP();

        public static bool HasConsole
        {
            get { return GetConsoleWindow() != IntPtr.Zero; }
        }

        /// <summary>
        /// Creates a new console instance if the process is not attached to a console already.
        /// </summary>
        public static void Show()
        {
            //#if DEBUG
            if (!HasConsole)
            {
                AllocConsole();
            }
            //#endif
        }

        /// <summary>
        /// If the process has a console attached to it, it will be detached and no longer visible. Writing to the System.Console is still possible, but no output will be shown.
        /// </summary>
        public static void Hide()
        {
            //#if DEBUG
            if (HasConsole)
            {
                SetOutAndErrorNull();
                FreeConsole();
            }
            //#endif
        }

        public static void Toggle()
        {
            if (HasConsole)
            {
                Hide();
            }
            else
            {
                Show();
            }
        }

        static void SetOutAndErrorNull()
        {
            Console.SetOut(TextWriter.Null);
            Console.SetError(TextWriter.Null);
        }
    }

    public partial class MainWindow : Window
    {
        public void DoStuff(Grid DynamicGrid)
        {
            ColumnDefinition gridCol1 = new ColumnDefinition();
            gridCol1.Width = new System.Windows.GridLength(60);

            DynamicGrid.ColumnDefinitions.Add(gridCol1);
        }
        public void DoStuff1(Grid DynamicGrid)
        {
            RowDefinition gridRow1 = new RowDefinition();
            gridRow1.Height = new System.Windows.GridLength(60);
            DynamicGrid.RowDefinitions.Add(gridRow1);
        }
        public TextBlock DoStuff2(int i, string a)
        {
            TextBlock txtBlock1 = new TextBlock();

            txtBlock1.Text = a;

            txtBlock1.FontSize = 15;

            txtBlock1.VerticalAlignment = VerticalAlignment.Top;

            Grid.SetRow(txtBlock1, 0);

            Grid.SetColumn(txtBlock1, i);

            return txtBlock1;
        }
        public TextBlock DoStuff3(int i, string a)
        {
            TextBlock txtBlock1 = new TextBlock();

            txtBlock1.Text = a;

            txtBlock1.FontSize = 15;

            txtBlock1.VerticalAlignment = VerticalAlignment.Top;

            Grid.SetRow(txtBlock1, i);

            Grid.SetColumn(txtBlock1, 0);

            return txtBlock1;
        }
        public MainWindow()
        {
            Grid DynamicGrid = new Grid();
            DynamicGrid.HorizontalAlignment = HorizontalAlignment.Left;
            DynamicGrid.VerticalAlignment = VerticalAlignment.Top;
            DynamicGrid.ShowGridLines = true;

            string[] lines = File.ReadAllLines("C:\\Users\\ehabi\\source\\repos\\WpfApp3\\WpfApp3\\seqT.txt");
            string[] lines2 = File.ReadAllLines("C:\\Users\\ehabi\\source\\repos\\WpfApp3\\WpfApp3\\seqS.txt");


            ConsoleManager.Show();
            string refSeq = lines[1];
            
            string alignSeq = lines2[1];

            int mismatch = -3;
            int match = 5;
            int gap = -5;

            int x = refSeq.Length + 1;
            int y = alignSeq.Length + 1;

            DynamicGrid.Width = 70 * x;
            DynamicGrid.Height = 80 * y;


            for (int i = 0; i < x + 2; i++)
            {
                DoStuff(DynamicGrid);
            }
            for (int i = 0; i < y + 2; i++)
            {
                DoStuff1(DynamicGrid);
            }

            for (int i = 0; i < refSeq.Length; i++)
            {
                DynamicGrid.Children.Add(DoStuff2(i + 2, refSeq[i].ToString()));

            }
            for (int i = 0; i < alignSeq.Length; i++)
            {
                DynamicGrid.Children.Add(DoStuff3(i + 2, alignSeq[i].ToString()));

            }





            int[,] matrix = new int[y, x];
            string[,] matrixway = new string[y, x];
            int upleft = 0;
            int left = 0;
            int up = 0;

            for (int i = 0; i < y; i++)
            {
                for (int j = 0; j < x; j++)
                {
                    matrixway[i, j] = string.Empty;
                }

            }

            for (int j = 0; j < x; j++)
            {
                matrix[0, j] = j*gap;
            }
            for (int j = 0; j < y; j++)
            {
                matrix[j, 0] = j*gap;
            }

            for (int j = 1; j < y; j++)
            {
                for (int i = 1; i < x; i++)
                {
                    upleft = refSeq[i - 1] == alignSeq[j - 1] ? (matrix[j - 1, i - 1] + match) : (matrix[j - 1, i - 1] + mismatch) ;
                    up = matrix[j - 1, i] + gap;
                    left = matrix[j, i - 1] + gap;
                    matrix[j, i] = Math.Max(Math.Max(upleft, left), up);
                    if (matrix[j,i]== upleft)
                    {
                        if (matrixway[j,i]==string.Empty)
                            matrixway[j, i] += "upleft";
                        else
                        matrixway[j, i] += "+upleft";

                    }
                    if (matrix[j, i] == left)
                    {
                        if (matrixway[j, i] == string.Empty)
                            matrixway[j, i] += "left";
                        else
                            matrixway[j, i] += "+left";
                    }
                    if (matrix[j, i] == up)
                    {
                        if (matrixway[j, i] == string.Empty)
                            matrixway[j, i] += "up";
                        else
                            matrixway[j, i] += "+up";
                    }
                }
            }
            int max =Int32.MinValue;


            int[,] traceback = new int[y, x];
            for (int i = 0; i < y; i++)
            {
                for (int j = 0; j < x; j++)
                {
                    traceback[i, j] = 0;
                }

            }
            traceback[y-1, x-1] = 1;

            var lastx = y-1;
            var lasty = x-1;

            string[] words ;



            while (true)
            {
               words= matrixway[lastx,lasty].Split('+');
                foreach(string word in words)
                {
                    if (word == "upleft")
                    {
                        if (matrix[lastx-1, lasty-1] > max)
                        {
                            max = matrix[lastx-1, lasty-1];
                        }
                    }
                    if (word == "left")
                    {
                        if (matrix[lastx, lasty-1] > max)
                        {
                            max = matrix[lastx, lasty-1];
                        }
                    }
                    if (word == "up")
                    {
                        if (matrix[lastx-1, lasty] > max)
                        {
                            max = matrix[lastx-1, lasty];
                        }
                    }
                }
                

                if(max == matrix[lastx - 1, lasty - 1])
                {
                    traceback[lastx - 1, lasty - 1] = 1;
                    lastx = lastx - 1;
                    lasty = lasty - 1;
                }else if(max == matrix[lastx - 1, lasty])
                {
                    traceback[lastx - 1, lasty] = 1;
                    lastx = lastx - 1;
                }
                else if (max == matrix[lastx, lasty-1])
                {
                    traceback[lastx,lasty-1] = 1;
                    lasty = lasty - 1;
                }
                max = Int32.MinValue;
                if (lastx == 0 && lasty == 0)
                {
                    break;
                }
            }

            for (int i = 0; i < y; i++)
                for (int j = 0; j < x; j++)
                {
                    {
                        TextBlock txtBlock1 = new TextBlock();
                        if (traceback[i, j] == 1) { txtBlock1.Background = Brushes.Red; }
                        txtBlock1.Text = string.Empty;
                        foreach (string word in matrixway[i, j].Split('+')) {
                            if (word == "upleft")
                            {
                                txtBlock1.Text += "↖";
                            }
                            if (word == "left")
                            {
                                txtBlock1.Text += "←";
                            }
                            if (word == "up")
                            {
                                txtBlock1.Text += "↑";
                            }
                        }
                        txtBlock1.Text += matrix[i, j].ToString();
                        txtBlock1.FontSize = 15;

                        txtBlock1.VerticalAlignment = VerticalAlignment.Center;

                        Grid.SetRow(txtBlock1, i + 1);

                        Grid.SetColumn(txtBlock1, j + 1);
                        DynamicGrid.Children.Add(txtBlock1);
                    }
                }
            Application.Current.MainWindow.Content = DynamicGrid;

            char[] alignSeqArray = alignSeq.ToCharArray();
            char[] refSeqArray = refSeq.ToCharArray();

            string AlignmentA = string.Empty;
            string AlignmentB = string.Empty;
            int m = y - 1;
            int n = x - 1;
            while (m > 0 || n > 0)
            {
                int scroeDiag = 0;

                if (m == 0 && n > 0)
                {
                    AlignmentA = refSeqArray[n - 1] + AlignmentA;
                    AlignmentB = "-" + AlignmentB;
                    n = n - 1;
                }
                else if (n == 0 && m > 0)
                {
                    AlignmentA = "-" + AlignmentA;
                    AlignmentB = alignSeqArray[m - 1] + AlignmentB;
                    m = m - 1;
                }
                else
                {

                    if (alignSeqArray[m - 1] == refSeqArray[n - 1])
                        scroeDiag = 5;
                    else
                        scroeDiag = -3;

                    if (m > 0 && n > 0 && matrix[m, n] == matrix[m - 1, n - 1] + scroeDiag&& traceback[m - 1, n - 1]==1)
                    {
                        AlignmentA = refSeqArray[n - 1] + AlignmentA;
                        AlignmentB = alignSeqArray[m - 1] + AlignmentB;
                        m = m - 1;
                        n = n - 1;
                    }
                    else if (n > 0 && matrix[m, n] == matrix[m, n - 1] - 5 && traceback[m , n -1] == 1)
                    {
                        AlignmentA = refSeqArray[n - 1] + AlignmentA;
                        AlignmentB = "-" + AlignmentB;
                        n = n - 1;
                    }
                    else
                    {
                        AlignmentA = "-" + AlignmentA;
                        AlignmentB = alignSeqArray[m - 1] + AlignmentB;
                        m = m - 1;
                    }
                }
            }
 
            //Display the result
            Console.Write(Environment.NewLine);
            Console.WriteLine(AlignmentA);
            Console.WriteLine(AlignmentB);
            Console.Write(Environment.NewLine);
            Console.Write("Score is:" + matrix[y - 1, x - 1]);
        }
    }
}
