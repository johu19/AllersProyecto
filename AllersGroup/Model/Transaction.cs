﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Transaction
    {
        public string ClientCode { get; set; }
        public int Code { get; set; }
        public System.DateTime Date { get; set; }
        public long Total { get; set; }
        public List<Sold> Solds { get; set; }
        public List<Item> Items { get; set; }

        public Transaction(String[] info,Item item)
        {
            Solds = new List<Sold>();
            Items = new List<Item>();

            ClientCode = info[0];
            Code = int.Parse(info[1]);
            Date = Convert.ToDateTime(info[2]);
            Total = long.Parse(info[3]);

           
            Sold a = new Sold(info[4], info[5], info[6], info[7]);
            Solds.Add(a);
            Items.Add(item);
               

        }

        public void AddSold(String ItemCode, String QuantityItems, String ItemPrice, String Subtotal, Item item)
        {
            Sold a = new Sold(ItemCode,QuantityItems,ItemPrice,Subtotal);
            Solds.Add(a);
            Items.Add(item);
        }

    }

}
