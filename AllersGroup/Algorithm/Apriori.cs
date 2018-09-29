﻿using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Algorithm
{
    class Apriori
    {
        public T[] GenerateCandidates<T>(T[] itemset1, T[] itemset2)
        {

            if (itemset1.Count() == itemset2.Count())
            {

                int length = itemset1.Count();
                T[] candidate = new T[length];

                bool flag = true;

                for (int i = 0; i < length - 1 && flag; i++)
                {
                    if (itemset1.ElementAt(i).Equals(itemset2.ElementAt(i)))
                    {
                        candidate[i] = itemset1[i];
                    }
                    else
                    {
                        flag = false;
                    }
                }

                if (flag)
                {
                    candidate[length - 2] = itemset1[length - 1];
                    candidate[length - 1] = itemset2[length - 1];
                }

            }
            return null;
        }
    }
}