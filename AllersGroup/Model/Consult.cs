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
            Console.WriteLine("Time loading: "+stop1.Subtract(start1).TotalSeconds+" segundos");

            Console.WriteLine("Number of items: {0}, Number of transactions: {1}, Number of clients: {2}",c.context.Items.Count,c.context.Transactions.Count,c.context.Clients.Count);

            TimeSpan stop;
            TimeSpan start = new TimeSpan(DateTime.Now.Ticks);

            c.context.generateFrecuentItemsets(0.005,25, "VALLE DEL CAUCA");

            
            stop = new TimeSpan(DateTime.Now.Ticks);
            Console.WriteLine("Time generating frecuent itemsets: "+stop.Subtract(start).TotalSeconds+" segundos");


            TimeSpan stop2;
            TimeSpan start2 = new TimeSpan(DateTime.Now.Ticks);

            int n = 2;
            List<Itemset[]> clusters = c.context.GenerateClustersWithinItemsets(n);

            stop2 = new TimeSpan(DateTime.Now.Ticks);
            Console.WriteLine("Time building {0} clusters "+stop2.Subtract(start2).TotalMilliseconds+ " milisegundos",n);


            

            for(int i = 0; i < clusters.Count; i++)
            {
                Console.WriteLine("Cluster number {0}", i + 1);
                Itemset[] actual = clusters.ElementAt(i);
                foreach(Itemset its in actual)
                {
                    string linea = "";
                    foreach(Item item in its.Items)
                    {
                        linea += item.Code + " ";
                    }

                    Console.WriteLine("Items: "+linea);
                    Console.WriteLine("Avg Classification: "+its.AverageClassification + " Avg Price " + its.AveragePrice);
                }
            }

            Console.WriteLine("AFTER PRUNNING  Number of items: {0}, Number of transactions in region: {1}, Number of clients: {2}", c.context.Items.Count, c.context.TransactionsPrunned.Where(i => c.context.Clients[i.Value.ClientCode].Departament.Equals("VALLE DEL CAUCA")).ToList().Count, c.context.ClientPrunned.Count);
            Console.WriteLine("Number of frecuent itemsets: " + c.context.FrecuentItemsets.Count);

            Console.ReadLine();
        }

    }
}
