using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace SAT_CNF
{
    class CNF_Converter {

        public CNF_Converter(int n) 
        {
            dimensionMatrix = n;
        }

        int dimensionMatrix = 0;

        public Dictionary<string, string> mapStrToStr(int n) 
        {
            string PosString = "";
            int count = 0;
            Dictionary<string, string> MapMatrix = new Dictionary<string, string>();
            for (int i = 1; i <= n; i++) 
            {
                for (int j = 1; j <= n; j++)
                {
                    count += 1;
                    PosString = "X" + i.ToString() + j.ToString();
                    MapMatrix.Add(PosString, count.ToString());
                }
            }
            return MapMatrix;
        }

        public  int CountOccurrences(string text, string pattern)
        {
            int count = 0;
            int i = 0;

            for (i = text.IndexOf(pattern, i); i != -1;)
            {
                i += pattern.Length;
                count++;
            }
            
            return count;
        }

        public string ConverterForDIMACS(string sSatformat, int inputD) 
        {
            //debugging the program
            //System.Console.WriteLine(sSatformat);

            Dictionary<string, string> dictMap = mapStrToStr(inputD);

            // foreach (KeyValuePair<string, string> ss in dictMap)  
            // {  
            //     Console.WriteLine("Key: {0}, Value: {1}",  ss.Key, ss.Value);  
            // } 


            int set = CountOccurrences(sSatformat, "(");
            string res = "p cnf " + inputD.ToString() + " " + set.ToString() + Environment.NewLine;
            StringBuilder builder = new StringBuilder(sSatformat);
            foreach (KeyValuePair<string, string> pair in dictMap)
            {
                builder.Replace(pair.Key, pair.Value);

            }

            builder.Replace("!", "-");
            builder.Replace(",", Environment.NewLine);
            builder.Replace("V", " ");
            builder.Replace(")", " 0 ");
            builder.Replace("( ", string.Empty);

            res = res + builder.ToString();
            string txtfilename = "SATinputIs" + inputD.ToString() + ".txt";
            using (System.IO.StreamWriter file = new System.IO.StreamWriter(txtfilename))
            {
                file.WriteLine(res.ToString());
            }
            return res;
        }

        public string SearchDiagonalPair(int i, int k , int n , string finalString )
        {
            string resCString = " ";
            for (int j = 1; j <= n ; j++)
            {
                for (int l = 1; l <= n; l++)
                {
                    // this has to be explained in the demo
                    bool flag = (j + l == i + k) && (i != j || k != l) ;
                    while (flag)
                    {
                        if (finalString.Contains("X" + j.ToString() + l.ToString() + "V" + "!X" + i.ToString() + k.ToString()) == false)
                        { 
                            resCString += "( !X" + i.ToString() + k.ToString() + "V" + "!X" + j.ToString() + l.ToString() + ") ,"; 
                        }
                        flag = false;
                    }

                }
            }
            //System.Console.WriteLine(resCString);
            return resCString;
        }

        public string ReductionToSAT()
        {
            int dimension = dimensionMatrix;
            string set = "";
            //initialise all the rows
            for (int i = 1; i <= dimension; i++)
            {
                set += "( ";
                for (int j = 1; j <= dimension; j++)
                {
                    set += "X" + i.ToString() + j.ToString() + " ";
                }
                set += ") ,";
            }
          

            //atleast one position in the row has to have a queen
            for (int i = 1; i <= dimension; i++)
            {
                for (int j = 1; j <= dimension; j++)
                {
                    for (int k = 1; k <= dimension && k != j; k++)
                    {
                        set += "( !X" + i.ToString() + j.ToString() + "V" + "!X" + i.ToString() + k.ToString() + ") ,";
                    }
                }
            }

            //initialise all the columns
            for (int i = 1; i <= dimension; i++) {
                set += "( ";
                for (int j =1; j<= dimension; j ++)
                {
                
                    set += "X" + j.ToString() + i.ToString() + " ";
                    
                }
                set += ") , ";
            }
          
            //atleast one position in the column has to have a queen
            for (int i = 1; i <= dimension; i++)
            {
               
                for (int j = 1; j <= dimension; j++)
                {
                    for (int k = 1; k <= dimension && k != j; k++)
                    {
                        set += "( !X" + j.ToString() + i.ToString() + "V" + "!X" + k.ToString() + i.ToString() + ") ,";
                    }
                }
            }
 
            //handling the central diagonal 
            for (int i = 1; i < dimension; i++)
            {
                for (int j = i + 1; j <= dimension; j++)
                {
                    set += "( !X" + i.ToString() + i.ToString() + "V" + "!X" + j.ToString() + j.ToString() + ") ,";
                }
            }

            //handling the left diagonal left  and right triangle
            for (int i = 1; i < dimension - 1; i++)
            {
                for (int j = 1; (j < dimension + 1 - i); j++)
                {

                    for (int k = i + 1;( k < dimension + 1 - i); k++)
                    {
                        bool a = (j != k);
                       while (a){
                            set += "( !X" + (j + i).ToString() + (j).ToString() + "V" + "!X" + (k + i).ToString() + k.ToString() + "),";
                            set += "( !X" + (j).ToString() + (j + i).ToString() + "V" + "!X" + k.ToString() + (k + i).ToString() + "),";
                            a = false;
                        }
                    }
                }
            }
           
            //hnadling diagonal triangle from upside
            for (int i = 1; i < dimension; i++)
            {
                for (int k = dimension; k > 1; k--)
                {
                    string s = SearchDiagonalPair(i, k, dimension, set);
                    set += s;

                }
            }

            return ConverterForDIMACS(set  , dimension);

        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Enter n:");
            int n = Int32.Parse(args[0]);
            
            CNF_Converter cc = new CNF_Converter(n);
            Console.WriteLine(cc.ReductionToSAT());

        }
    }
}
