using System;
using System.Collections.Generic;
using System.Linq;
using Algorithms;


namespace Model
{
    public class Consult
    {
        public Context context;

        public Consult()
        {
            context = new Context();
        }

        /**
         * Return a list of all the itemsets of a determinated size.
         * size: the size of the itemset. 
         **/
        public List<int[]> GenerateItemSet_BruteForce(int size)
        {
            List<int[]> itemset = null;
            var m = context.Items.Select(s => s.Value.Code).ToList();
            itemset = BruteForce.Combinations(m, size).ToList();

            return itemset;
        }

        public List<int[]> FrequentItemset_BruteForce(double threshold, int itemsetSize)
        {
            List<List<int>> transactions = context.Transactions.Select(t => t.Value.Items).ToList();
            var itemset = context.Items.Select(s => s.Value.Code).ToList();

            return BruteForce.GenerateFrecuentItemsets(itemset, transactions, itemsetSize, threshold).ToList();
        }


        /**
         * Frecuency of occurrence of an itemset: Counts in how many transactions a given itemset occurs.
        * itemset : Array of codes of a itemset.
        **/
        public int SupportCount(int[] itemset)
        {
            List<List<int>> dataBase = context.Transactions.Select(t => t.Value.Items).ToList();
            return BruteForce.SupportCount(itemset, dataBase);
        }

        public double Support(int[] itemset)
        {
            List<List<int>> dataBase = context.Transactions.Select(t => t.Value.Items).ToList();
            int supportCount = SupportCount(itemset);
            int totalTransactions = context.Transactions.Select(t => t.Value).GroupBy(t => t.Code).ToList().Count();

            return BruteForce.Support(itemset, dataBase);
        }

        public void PrunningClientsAndTransactions()
        {
            List<String> clientsD = new List<String>();
            List<int> transactiondsD = new List<int>();

            foreach (var c in context.Clients)
            {
                if (context.Transactions.Count(t => t.Value.ClientCode == c.Key) <= 6)
                {
                    clientsD.Add(c.Key);

                    foreach (var t in context.Transactions)
                    {

                        if (t.Value.ClientCode == c.Key)
                        {
                            transactiondsD.Add(t.Key);
                        }
                    }
                }
            }

            foreach (var c in clientsD)
            {
                context.Clients.Remove(c);
            }

            foreach (var t in transactiondsD)
            {
                context.Transactions.Remove(t);
            }
        }

        public void PrunningItems()
        {
            List<int> itemsD = new List<int>();

            List<int[]> itemset_1 = GenerateItemSet_BruteForce(1);
            foreach (var i in itemset_1)
            {
                if (SupportCount(i) == 0)
                {
                    itemsD.Add(i[0]);
                }

            }

            foreach (var i in itemsD)
            {
                context.Items.Remove(i);
            }
        }


        public List<int[]> Apriori(double threshold)
        {

            //Itemsets of size 1.
            List<int[]> itemsets = GenerateItemSet_BruteForce(1);
            List<List<int>> transactions = context.Transactions.Select(t => t.Value.Items).ToList();
            return Algorithms.Apriori.GenerateAllFrecuentItemsets(itemsets, transactions, threshold).ToList();

        }

        static void Main(string[] args)
        {
            TimeSpan stop1;
            TimeSpan start1 = new TimeSpan(DateTime.Now.Ticks);

            Consult c = new Consult();

            stop1 = new TimeSpan(DateTime.Now.Ticks);
            Console.WriteLine(stop1.Subtract(start1).TotalMilliseconds);


            TimeSpan stop;
            TimeSpan start = new TimeSpan(DateTime.Now.Ticks);

            c.context.generateFrecuentItemsets(0.005);

            stop = new TimeSpan(DateTime.Now.Ticks);
            Console.WriteLine(stop.Subtract(start).TotalMilliseconds);

            foreach (Itemset fis in c.context.FrecuentItemsets)
            {
                string a = "";
                foreach (Item i in fis.Items)
                {
                    a += i.Code + " ";
                }

                Console.WriteLine(a);
                Console.WriteLine(fis.AverageClassification + "  " + fis.AveragePrice);
            }




            //Console.WriteLine("clientes: " + c.context.Clients.Count + "  items: " + c.context.Items.Count + "  transacciones: " + c.context.Transactions.Count);


            //Console.WriteLine("Initial clients {0}", c.context.Clients.Count());
            //Console.WriteLine("Initial Transactions {0}", c.context.Transactions.Count());
            //Console.WriteLine("Initial Items {0}", c.context.Items.Count());

            //c.PrunningClientsAndTransactions();
            ////c.PrunningItems();

            //Console.WriteLine("Clients {0}", c.context.Clients.Count());
            //Console.WriteLine("Transactions {0}", c.context.Transactions.Count());
            //Console.WriteLine("Items {0}", c.context.Items.Count());

            //Console.WriteLine(" ");
            //double threshold = 0.005;
            //Console.WriteLine("threshold : {0} ", threshold * 100);

            //List<int[]> FrecuentItems = c.Apriori(threshold);
            //List<double[]> frecuentItems = new List<double[]>();

            //for (int i = 0; i < FrecuentItems.Count; i++)
            //{
            //    int n = FrecuentItems.ElementAt(i).Length;
            //    int[] items = new int[n];
            //    double[] arrayFinal = new double[items.Length + 1];
            //    for (int j = 0; j < FrecuentItems.ElementAt(i).Length; j++)
            //    {
            //        arrayFinal[j] = FrecuentItems.ElementAt(i)[j];
            //    }
            //    arrayFinal[arrayFinal.Length - 1] = BruteForce.Support(FrecuentItems.ElementAt(i), c.context.Transactions.Select(t => t.Value.Items).ToList());
            //    frecuentItems.Add(arrayFinal);
            //}



            //for (int i = 0; i < frecuentItems.Count; i++)
            //{

            //    string a = "";


            //    for (int j = 0; j < frecuentItems.ElementAt(i).Length; j++)
            //    {
            //        a += frecuentItems.ElementAt(i)[j] + " ";
            //    }
            //    //a += "%";
            //    Console.WriteLine(a);
            //}

            //Console.WriteLine("end");





            Console.ReadLine();
        }

    }
}
