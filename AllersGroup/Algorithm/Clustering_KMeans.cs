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
        public static List<List<double[]>> createInitialClusters(List<double[]> itemSets,int numberOfClusters)
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


    }
}
