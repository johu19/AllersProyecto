using Algorithms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithm
{
    public static class Association_Rules<T>
    {
        public static List<Rule<T>> GenerateAllAssociationRules(List<T[]> frecuentItemsSets, IEnumerable<IEnumerable<T>> transactions, double minConfidence, int sizeRule)
        {

            List<Rule<T>> associationRules = new List<Rule<T>>();

            foreach (var l in frecuentItemsSets)
            {

                if (sizeRule < l.Length)
                {
                    double supFrecuentItemSet = BruteForce.Support(l, transactions);

                    List<T[]> arr = Combinaciones(l, sizeRule);

                    foreach (var x in arr)
                    {
                        double supportCombination = BruteForce.Support(x, transactions);
                        double confidenceRule = supFrecuentItemSet / supportCombination;

                        if (confidenceRule >= minConfidence)
                        {
                            Rule<T> rule = new Rule<T>(x, l, confidenceRule);
                            associationRules.Add(rule);
                        }
                    }
                }

            }

            return associationRules;
        }

        public static List<T[]> Combinaciones(T[] original, int largo)
        {
            List<T[]> lista = new List<T[]>();
            ImplementCombinaciones<T>(original, new T[largo], 0, 0, lista);
            return lista;
        }
        private static void ImplementCombinaciones<T>(T[] original, T[] temp, int posorig, int postemp, List<T[]> lista)
        {
            if (postemp == temp.Length)
            {
                T[] copia = new T[postemp];
                Array.Copy(temp, copia, postemp);
                lista.Add(copia);
            }
            else if (posorig == original.Length) return;
            else
            {
                temp[postemp] = original[posorig];
                ImplementCombinaciones<T>(original, temp, posorig + 1, postemp + 1, lista);
                ImplementCombinaciones<T>(original, temp, posorig + 1, postemp, lista);
            }
        }



    }

    public class Rule<T>
    {
        public T[] antecedent { get; set; }
        public T[] consecuent { get; set; }
        public double confidence { get; set; }
        public Rule(T[] antecedent, T[] consecuent, double confidence)
        {
            this.antecedent = antecedent;
            this.consecuent = consecuent;
            this.confidence = confidence;
        }



    }

}
