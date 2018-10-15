using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;


namespace Algorithms
{
    public static class Clustering_KMeans
    {
        public static List<List<double[]>> CreateInitialClusters(List<double[]> itemSets,int numberOfClusters)
        {
            List<List<double[]>> firstClusters = new List<List<double[]>>();

            List<double> temporaryCentroidsIndex = new List<double>();


            //Gets the random index of the random clusters

            for(int i = 0; i < numberOfClusters; i++)
            {
                Boolean cond = false;
                Random rnd = new Random();
                while (!cond)
                {
                    int n = rnd.Next(0, itemSets.Count);
                    if (!temporaryCentroidsIndex.Contains(n))
                    {
                        temporaryCentroidsIndex.Add(n);
                        cond = true;
                    }
                }
            }


            // Saves the randoms centroids

            List<double[]> temporaryCentroids = new List<double[]>();
            foreach(double d in temporaryCentroidsIndex)
            {
                temporaryCentroids.Add(itemSets.ElementAt((int)d));
            }

            Hashtable hash = new Hashtable();


            //Finds the closest centroid per each itemset and saves the information on the Hashtable hash

            for(int i = 0; i < itemSets.Count; i++)
            {
                double[] actual = itemSets.ElementAt(i);
                List<double> distances = new List<double>();
                foreach(double[] centroid in temporaryCentroids)
                {
                    double distance = Math.Sqrt(Math.Pow(actual[1]-centroid[1],2) + Math.Pow(actual[2]-centroid[2],2));
                    distances.Add(distance);
                }

                double min = distances.First();
                int index = 0;
                for(int j = 0; j < distances.Count; j++)
                {
                    if (distances.ElementAt(j) < min)
                    {
                        min = distances.ElementAt(j);
                        index = j;
                    }
                }

                hash.Add(actual, index);
            }

            for (int i = 0; i < temporaryCentroids.Count; i++)
            {
                List<double[]> cluster = new List<double[]>();
                for (int j = 0; j < itemSets.Count; j++)
                {

                    if (hash[itemSets.ElementAt(j)].Equals(i))
                    {
                        cluster.Add(itemSets.ElementAt(j));
                    }
                }
                firstClusters.Add(cluster);
            }


            return firstClusters;

        }



        public static List<double[]> CalculateCentroids(List<List<double[]>> Clusters)
        {
            List<double[]> newCentroids = new List<double[]>();

            foreach(List<double[]> cluster in Clusters)
            {
                double x = cluster.Average(d => d[1]);
                double y = cluster.Average(d => d[2]);
                double[] centroid = new double[2];
                centroid[0] = x;
                centroid[1] = y;
                newCentroids.Add(centroid);
            }


            return newCentroids;
        }


        public static List<List<double[]>> BuildClusters(List<double[]> Centroids, List<double[]> Itemsets)
        {
            List<List<double[]>> Clusters = new List<List<double[]>>();

            Hashtable hash = new Hashtable();

            foreach (double[] actual in Itemsets)
            {
                List<double> distances = new List<double>();
                foreach(double[] centroid in Centroids)
                {
                    double distance = Math.Sqrt(Math.Pow(actual[1] - centroid[0], 2) + Math.Pow(actual[2] - centroid[1], 2));
                    distances.Add(distance);
                }

                double min = distances.First();
                int index = 0;
                for (int j = 0; j < distances.Count; j++)
                {
                    if (distances.ElementAt(j) < min)
                    {
                        min = distances.ElementAt(j);
                        index = j;
                    }
                }

                hash.Add(actual, index);

            }

            for (int i = 0; i <Centroids.Count; i++)
            {
                List<double[]> cluster = new List<double[]>();
                for (int j = 0; j < Itemsets.Count; j++)
                {

                    if (hash[Itemsets.ElementAt(j)].Equals(i))
                    {
                        cluster.Add(Itemsets.ElementAt(j));
                    }
                }
                Clusters.Add(cluster);
            }



            return Clusters;
        }



        public static Boolean ClustersAreEqual(List<List<double[]>> cluster1, List<List<double[]>> cluster2)
        {
            Boolean result = true;

            for (int i = 0; i < cluster1.Count && result == true; i++)
            {
                List<double[]> actual1 = cluster1.ElementAt(i);
                List<double[]> actual2 = cluster2.ElementAt(i);

                foreach (double[] itemset in actual1)
                {
                    if (!actual2.Contains(itemset))
                    {
                        result = false;
                    }
                }
            }



            return result;
        }


    }
}
