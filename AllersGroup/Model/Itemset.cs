﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Itemset
    {
        public List<Item> Items { get; set; }

        public double AverageClassification { get; set; }

        public double AveragePrice { get; set; }


        public Itemset(List<Item> items, double aP, double aC)
        {
            Items = items;
            AverageClassification = aC;
            AveragePrice = aP;


        }



      


    }
}