using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Zagadka30
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        #region methods
        private static bool NextCombination(IList<int> num, int n, int k)
        {
            bool finished;

            var changed = finished = false;

            if (k <= 0) return false;

            for (var i = k - 1; !finished && !changed; i--)
            {
                if (num[i] < n - 1 - (k - 1) + i)
                {
                    num[i]++;

                    if (i < k - 1)
                        for (var j = i + 1; j < k; j++)
                            num[j] = num[j - 1] + 1;
                    changed = true;
                }
                finished = i == 0;
            }

            return changed;
        }

        private static IEnumerable Combinations<T>(IEnumerable<T> elements, int k)
        {
            var elem = elements.ToArray();
            var size = elem.Length;

            if (k > size) yield break;

            var numbers = new int[k];

            for (var i = 0; i < k; i++)
                numbers[i] = i;

            do
            {
                yield return numbers.Select(n => elem[n]);
            } while (NextCombination(numbers, size, k));
        }

        #endregion

        private void All_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            int Sum(byte[] b1, byte[] b2, byte[] b3)
            {
                int sum = 0;
                foreach (byte b in b1.Concat(b2.ToArray()).Concat(b3).ToArray()) { sum += b; };
                return sum;
            }

            lbok1.Content = ""; lbok2.Content = ""; lbok3.Content = "";
            int[] sums = new int[] { 0, 0, 0 };

            if (all.SelectedItem == null) return;

            Trojkat t = (Trojkat)all.SelectedItem;

            l1.Content = t.Bok1.L[0].ToString();
            l2.Content = t.Bok1.L[1].ToString();
            l3.Content = t.Bok1.L[2].ToString();
            sums[0] = t.Bok1.L[0] + t.Bok1.L[1] + t.Bok1.L[2] + t.Bok1.P + t.Bok2.P;

            l5.Content = t.Bok2.L[0].ToString();
            l6.Content = t.Bok2.L[1].ToString();
            l7.Content = t.Bok2.L[2].ToString();
            sums[1] = t.Bok2.L[0] + t.Bok2.L[1] + t.Bok2.L[2] + t.Bok2.P + t.Bok3.P;

            l9.Content = t.Bok3.L[0].ToString();
            l10.Content = t.Bok3.L[1].ToString();
            l11.Content = t.Bok3.L[2].ToString();
            sums[2] = t.Bok3.L[0] + t.Bok3.L[1] + t.Bok3.L[2] + t.Bok1.P + t.Bok3.P;

            l0.Content = t.Bok1.P.ToString();
            l4.Content = t.Bok2.P.ToString();
            l8.Content = t.Bok3.P.ToString();

            lbok1.Content = sums[0] == 30 ? "30" : "Błąd";
            lbok2.Content = sums[2] == 30 ? "30" : "Błąd";
            lbok3.Content = sums[1] == 30 ? "30" : "Błąd";

            List<int> sumy = new List<int>();

            lSuma.Content = Sum(t.Bok1.L, t.Bok2.L, t.Bok3.L);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            byte[] liczby = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12 };

            const byte k = 5;
            int bok1 = 0;

            List<byte[]> etap1 = new List<byte[]>();

            ///
            int licznik = 0;
            SortedDictionary<CBok, int> oznacz = new SortedDictionary<CBok, int>();
            ///

            //szukam wszystkich 5 elementowych konbinacji liczb których suma wynosi 30
            foreach (IEnumerable<byte> i in Combinations(liczby, k))
            {
                bok1 = 0;
                foreach (byte n in i)
                {
                    bok1 += n;
                }

                if (bok1 == 30)
                {
                    etap1.Add(i.ToArray());
                    oznacz.Add(new CBok() { L = i.ToArray() }, licznik++); //budowanie tabeli z kazdym mozliwym bokiem - wykorzystywane do eliminacji duplikatow
                }
            }

            SortedDictionary<CBok, SortedDictionary<CBok, List<CBok>>> drzewo = new SortedDictionary<CBok, SortedDictionary<CBok, List<CBok>>>();

            ///
            /// Szukam wszystkich par wygerowanych licz, które posiadają jeden wspólny element
            ///
            foreach (byte[] b in etap1)
            {
                CBok cbok = new CBok()
                {
                    L = b
                };

                drzewo.Add(cbok, new SortedDictionary<CBok, List<CBok>>());

                foreach (byte[] b2 in etap1)
                {
                    if (b != b2)
                    {
                        byte[] wspolne = b.Intersect(b2).ToArray();
                        if (wspolne.Count() == 1)
                        {
                            drzewo[cbok].Add(new CBok() { L = b2, P = wspolne[0] }, new List<CBok>());
                        }
                    }
                }

                if (drzewo[cbok].Count() == 0) drzewo.Remove(cbok);
            }

            ///
            /// szukanie wszystkich par które posiadają jeden wspólny element z utworzonymi gałeziami
            /// wstępnie oznaczamy punkty boczne
            ///
            foreach (KeyValuePair<CBok, SortedDictionary<CBok, List<CBok>>> wszystkie in drzewo)
            {
                foreach (KeyValuePair<CBok, SortedDictionary<CBok, List<CBok>>> pPoziom in drzewo)
                {
                    if (pPoziom.Key != wszystkie.Key)
                    {
                        foreach (KeyValuePair<CBok, List<CBok>> dPoziom in pPoziom.Value)
                        {
                            if (dPoziom.Key != wszystkie.Key)
                            {
                                byte[] wspolne2 = wszystkie.Key.L.Intersect(dPoziom.Key.L).ToArray();
                                byte[] wspolne1 = wszystkie.Key.L.Intersect(pPoziom.Key.L).ToArray();
                                if (wspolne1.Count() == 1 && wspolne2.Count() == 1)
                                {
                                    if (wspolne2[0] != dPoziom.Key.P)
                                    {
                                        drzewo[pPoziom.Key][dPoziom.Key].Add(new CBok() { L = wszystkie.Key.L, P = wspolne2[0] });
                                    }
                                }
                            }
                        }
                    }
                }
            }

            HashSet<string> szukaj = new HashSet<string>();

            foreach (KeyValuePair<CBok, SortedDictionary<CBok, List<CBok>>> pPoziom in drzewo)
            {
                foreach (KeyValuePair<CBok, List<CBok>> dPoziom in pPoziom.Value)
                {
                    foreach (CBok cbok in dPoziom.Value)
                    {
                        byte[] wspolne1 = pPoziom.Key.L.Intersect(cbok.L).ToArray();

                        Trojkat t = new Trojkat() { Bok1 = new CBok() { L = pPoziom.Key.L, P = wspolne1[0] }, Bok2 = new CBok() { L = dPoziom.Key.L, P = dPoziom.Key.P }, Bok3 = new CBok() { L = cbok.L, P = cbok.P } };

                        //szukam czy taka trojka boków już nie była dodana
                        List<int> i = new List<int>
                        {
                            oznacz[t.Bok1],
                            oznacz[t.Bok2],
                            oznacz[t.Bok3]
                        };
                        i.Sort();
                        
                        if (szukaj.Add(string.Join(" ", i)))
                        {
                            t.Bok1.L = t.Bok1.L.Except(new byte[] { t.Bok1.P, t.Bok2.P }).ToArray();
                            t.Bok2.L = t.Bok2.L.Except(new byte[] { t.Bok2.P, t.Bok3.P }).ToArray();
                            t.Bok3.L = t.Bok3.L.Except(new byte[] { t.Bok3.P, t.Bok1.P }).ToArray();
                            all.Items.Add(t);
                        }
                        //

                    }
                }
            }
        }
    }

    public struct Trojkat
    {
        public CBok Bok1 { get; set; }
        public CBok Bok2 { get; set; }
        public CBok Bok3 { get; set; }

        public override string ToString()
        {
            return Bok1.P +" "+ string.Join(" ", Bok1)+" "+ Bok2.P + "  " + string.Join(" ", Bok2) + " " + Bok3.P + " "+ string.Join(" ", Bok3) ;
        }
    }


    public class CBok : IComparable
    {
        public byte[] L { get; set; }

        public byte P { get; set; }

        public int CompareTo(object obj)
        {
            if (obj == null) return 1;
            else return ToString().CompareTo(((CBok)obj).ToString());
        }

        public override string ToString()
        {
            return string.Join(" ", L);
        }
    }
}
