﻿using InventoryManagementApp.DataClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InventoryManagementApp.UserInterfaces
{
    public partial class frmAddOrder : Form
    {
        private Order order;

        public frmAddOrder()
        {
            InitializeComponent();
        }

        public frmAddOrder(Order order)
        {
            this.order = order;
        }
    }
}
