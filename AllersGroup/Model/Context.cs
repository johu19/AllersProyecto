using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Algorithms;
using System.Collections;

namespace Model
{
    public class Context
    {
        //public static String path= @"C:\Users\Windows\source\repos\AllersGroup\AllersGroup\Model\Data\";
        public static String path = @"C:\Users\manuel\source\repos\AllersProyecto\AllersGroup\Model\Data\";
        public Dictionary<String, Client> Clients { get; set; }
        public Dictionary<String, Client> ClientPrunned { get; set; }
        public Dictionary<int, Item> Items { get; set; }
        public Dictionary<int, Transaction> Transactions { get; set; }
        public Dictionary<int,Transaction> TransactionsPrunned { get; set; }
        public List<Itemset> FrecuentItemsets { get; set; }
        public Dictionary<int,Transaction> FilteredTransactions { get; set; }



        /**
         * Creates a Context.
         **/
        public Context()
        {
            FrecuentItemsets = new List<Itemset>();
            FilteredTransactions = new Dictionary<int, Transaction>();

            Items = new Dictionary<int, Item>();
            Clients = new Dictionary<String, Client>();
            Transactions = new Dictionary<int, Transaction>();

            LoadItems();
            LoadClients();
            LoadTransactions();
            CalculateAveragePricePerItem();
        }



        public Hashtable RegionsFilteredPerOneItem(int itemCode)
        {
            itemCode = Transactions.Values.ToList().Last().Assets.First().ItemCode;

            List<Transaction> allTrans = Transactions.Values.ToList();

            List<Transaction> filteredTrans = new List<Transaction>();

            foreach (Transaction t in allTrans)
            {
                if (t.Assets.Where(a => a.ItemCode == itemCode).ToList().Count > 0)
                {
                    filteredTrans.Add(t);
                }
            }

            
            Console.WriteLine("Number of transactions for the item: " + filteredTrans.Count);
            Hashtable respuesta = new Hashtable();

            foreach(Transaction t in filteredTrans)
            {
                Client customer = Clients.Values.Where(c => c.Code.Equals(t.ClientCode)).First();

                if (respuesta.ContainsKey(customer.Departament))
                {
                    int n =(int) respuesta[customer.Departament] + 1;
                    respuesta[customer.Departament] = n;
                }
                else
                {
                    respuesta.Add(customer.Departament, 1);
                }

            }


            return respuesta;
        }

        public Hashtable RegionsFilteredPerMonth(int month)
        {

            

            List<Transaction> allTrans = Transactions.Values.ToList();

            List<Transaction> filteredTrans = new List<Transaction>();

            foreach (Transaction t in allTrans)
            {
                if (t.Date.Month == month)
                {
                    filteredTrans.Add(t);
                }
            }


            Console.WriteLine("Number of transactions for the month: " + filteredTrans.Count);
            Hashtable respuesta = new Hashtable();

            foreach (Transaction t in filteredTrans)
            {
                Client customer = Clients.Values.Where(c => c.Code.Equals(t.ClientCode)).First();

                if (respuesta.ContainsKey(customer.Departament))
                {
                    int n = (int)respuesta[customer.Departament] + 1;
                    respuesta[customer.Departament] = n;
                }
                else
                {
                    respuesta.Add(customer.Departament, 1);
                }

            }




            return respuesta;
        }


        public List<Client> ClientsFilteredByRegionAndCity(string dpt, string city)
        {
            List<Client> clients = Clients.Values.Where(c=> c.Departament.Equals(dpt)).ToList().Where(c=> c.City.Equals(city)).ToList();
            return clients;
        }



        public List<Itemset[]> GenerateClustersWithinItemsets(int numberOfClusters)
        {
            Hashtable hash = new Hashtable();
            List<double[]> itemsets = new List<double[]>();
            for(double i = 0.0; i < FrecuentItemsets.Count; i++)
            {
                hash.Add(i, FrecuentItemsets.ElementAt((int)i));
                double[] it = new double[3];
                it[0] = i;


                //Multiplying by 10.000 normalizes the data diference
                it[1] = FrecuentItemsets.ElementAt((int)i).AverageClassification*10000;


                it[2] = FrecuentItemsets.ElementAt((int)i).AveragePrice;
                itemsets.Add(it);
            }

            List<List<double[]>> firstClusters = Clustering_KMeans.CreateInitialClusters(itemsets, numberOfClusters);
            List<double[]> correctedCentroids = Clustering_KMeans.CalculateCentroids(firstClusters);

            List<List<double[]>> pastClusters = firstClusters;
            List<List<double[]>> correctedClusters = Clustering_KMeans.BuildClusters(correctedCentroids, itemsets);

            int m = 0;
            while (Clustering_KMeans.ClustersAreEqual(correctedClusters,pastClusters)==false)
            {
                m++;
                correctedCentroids = Clustering_KMeans.CalculateCentroids(correctedClusters);
                pastClusters = correctedClusters;
                correctedClusters = Clustering_KMeans.BuildClusters(correctedCentroids, itemsets);
            }

            Console.WriteLine("Number of iterations: {0}", m++);



            List<Itemset[]> FinalClusters = new List<Itemset[]>();

            foreach (List<double[]> cluster in correctedClusters)
            {
                int n = cluster.Count;
                Itemset[] actual = new Itemset[n];
                for (int i = 0; i < actual.Length; i++)
                {
                    actual[i] = (Itemset)hash[cluster.ElementAt(i)[0]];
                }
                FinalClusters.Add(actual);
            }


            return FinalClusters;
        }


        





        public void PrunningClientsAndTransactions()
        {
            TransactionsPrunned = FilteredTransactions;
            ClientPrunned = Clients;

            double prunningNumber = FilteredTransactions.Count;
            prunningNumber *=  0.01;

            Console.WriteLine("Prunning number: " + prunningNumber);


            List<String> clientsD = new List<String>();
            List<int> transactiondsD = new List<int>();

            foreach (var c in Clients)
            {
                if (FilteredTransactions.Count(t => t.Value.ClientCode == c.Key) <= prunningNumber)
                {
                    clientsD.Add(c.Key);

                    foreach (var t in FilteredTransactions)
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
                ClientPrunned.Remove(c);
            }

            foreach (var t in transactiondsD)
            {
                TransactionsPrunned.Remove(t);
            }

            Console.WriteLine("Transactions after prunning: " + TransactionsPrunned.Count);
        }





        public void generateFrecuentItemsets(double threshold, string region)
        {

            TimeSpan stop;
            TimeSpan start = new TimeSpan(DateTime.Now.Ticks);

            if (!region.Equals("TODO"))
            {
                

                foreach(int t in Transactions.Keys)
                {
                    if (Clients[Transactions[t].ClientCode].Departament.Equals(region))
                    {
                        FilteredTransactions.Add(Transactions[t].Code, Transactions[t]);
                    }
                }
            }
            else
            {
                FilteredTransactions = Transactions;
            }
            

            
            if(region.Equals("VALLE DEL CAUCA") || region.Equals("BOGOTA") || region.Equals("CUNDINAMARCA") || region.Equals("RISARALDA") || region.Equals("TODO"))
            {
                PrunningClientsAndTransactions();
            }
            else
            {
                TransactionsPrunned = FilteredTransactions;
            }
            

            stop = new TimeSpan(DateTime.Now.Ticks);
            Console.WriteLine("Time prunning clients and transactions: " + stop.Subtract(start).TotalSeconds + " segundos");



            List<List<int>> transactions = TransactionsPrunned.Select(t => t.Value.Items).ToList();
            List<int[]> itemsets = GenerateItemSet_BruteForce(1);

            
            List<int[]> frecuentIS = Apriori.GenerateAllFrecuentItemsets(itemsets, transactions, threshold).ToList();

            Console.WriteLine("Number of frecuent itemsets: "+frecuentIS.Count);
            
            List<Itemset> FIS = new List<Itemset>();

            for(int i = 0; i < frecuentIS.Count; i++)
            {
                int[] actual = frecuentIS.ElementAt(i);
                List<Item> theItems = new List<Item>();
                
                for(int j = 0; j < actual.Length; j++)
                {
                    Item theItem = Items[actual[j]];
                    theItems.Add(theItem);
                    
                    

                }
               
                double averageP = theItems.Average(it => it.AveragePrice);
                double averageC = theItems.Average(it => Convert.ToDouble(it.Clasification));

                Itemset nuevoItemset = new Itemset(theItems, averageP,averageC);

                FIS.Add(nuevoItemset);
            }

            FrecuentItemsets = FIS;
        }




        private List<int[]> GenerateItemSet_BruteForce(int size)
        {
            List<int[]> itemset = null;
            var m = Items.Select(s => s.Value.Code).ToList();
            itemset = BruteForce.Combinations(m, size).ToList();

            return itemset;
        }

        /**
         * Load the items.
         * If the item has it's clasifications as 'NULL' then it's given the '0' clasification. 
         **/
        private void LoadItems()
        {
            try
            {
                StreamReader sr = new StreamReader(path + "Items.csv");

                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    String[] datos = line.Split(';');
                    if (datos[2].Equals("NULL"))
                    {
                        datos[2] = "0";
                    }
                    Item i = new Item(datos);
                    Items.Add(i.Code, i);
                }
                sr.Close();
            }
            catch (Exception e)
            {
                //Console.WriteLine("Exception: " + e.Message);
            }
        }

        /**
         * Load the clients.
         * If the city equals to 'NULL' then is asigned the value of 'No indica ciudad' 
         * If the department equals to 'NULL' then is asigned the value 'No indica departamento'
         **/
        private void LoadClients()
        {
            try
            {
                StreamReader sr = new StreamReader(path + "Clients.csv");

                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    String[] datos = line.Split(';');

                    if (datos[2].Equals("NULL"))
                    {
                        datos[2] = "No indica ciudad";
                    }
                    else if (datos[3].Equals("NULL"))
                    {
                        datos[3] = "No indica departamento";
                    }

                    if (!Clients.ContainsKey(datos[0]))
                    {
                        Client c = new Client(datos);
                        Clients.Add(c.Code, c);
                    }
                }
                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception: " + e.Message);
            }
        }

        /**
         * Load the Transactions 
         **/
        private void LoadTransactions()
        {
            try
            {
                StreamReader sr = new StreamReader(path + "Transactions.csv");

                String line;
                while ((line = sr.ReadLine()) != null)
                {
                    String[] datos = line.Split(';');

                    if (!datos[4].Equals("NULL"))
                    {
                        if (!Transactions.ContainsKey(int.Parse(datos[1])))
                        {
                            if (Items.ContainsKey(Int32.Parse(datos[4])) && Clients.ContainsKey(datos[0]))
                            {
                                Transaction t = new Transaction(datos);
                                t.AddItem(Int32.Parse(datos[4]));
                                Transactions.Add(t.Code, t);
                                Clients[datos[0]].AddTransaction(t);
                            }
                        }
                        else
                        {
                            if (Items.ContainsKey(Int32.Parse(datos[4])))
                            {
                                Transactions[int.Parse(datos[1])].AddAsset(datos[4], datos[5], datos[6], datos[7]);
                                Transactions[int.Parse(datos[1])].AddItem(Int32.Parse(datos[4]));
                            }                                
                        }
                    }
                }
                sr.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void CalculateAveragePricePerItem()
        {
            foreach(Item i in Items.Values)
            {
                var theTrans = Transactions.Values.Where(t => t.Assets.Any(a => a.ItemCode == i.Code)).ToList();
                List<double> prices = new List<double>();
                foreach(var t in theTrans)
                {
                    prices.Add(t.Assets.Where(a => a.ItemCode == i.Code).ToList().Average(b=>b.Price));
                }
                if (prices.Count > 0)
                {
                    i.AveragePrice = prices.Average();
                }
                
            }
        }
    }
}