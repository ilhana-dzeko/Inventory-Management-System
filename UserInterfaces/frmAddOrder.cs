﻿using InventoryManagementApp.Data;
using InventoryManagementApp.DataClasses;
using InventoryManagementApp.Helpers;
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
        private bool editOrder = true;

        public frmAddOrder()
        {
            InitializeComponent();
            dgvOrderDetails.AutoGenerateColumns = false;
        }

        public frmAddOrder(Order order)
        {
            this.order = order;
        }

        private void btnManageOrders_Click(object sender, EventArgs e)
        {
            Panel pnlChildForm = this.Parent as Panel;

            if (pnlChildForm != null)
            {
                frmManageOrders frmManageOrders = new frmManageOrders();
                frmManageOrders.FormBorderStyle = FormBorderStyle.None;
                frmManageOrders.TopLevel = false;
                frmManageOrders.BringToFront();

                pnlChildForm.Controls.Clear();
                pnlChildForm.Controls.Add(frmManageOrders);
                frmManageOrders.Show();
                this.Hide();
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (ValidateOrderData() && (dgvOrderDetails.DataSource) != null)
                {
                    btnMakeOrder.Enabled = true;

                    if (order == null)
                    {
                        order = new Order();
                        editOrder = false;
                    }

                    order.Customer = cmbCustomers.SelectedItem as Customer;
                    order.OrderDate = DateTime.Now.Date;
                    

                    if (editOrder)
                    {
                        InventoryManagementDb.DB.Entry(order).State = System.Data.Entity.EntityState.Modified;
                    }
                    else
                    {
                        InventoryManagementDb.DB.Orders.Add(order);
                    }

                    InventoryManagementDb.DB.SaveChanges();

                    if (editOrder)
                        lblOperationInfo.Text = Messages.SuccessfullyModified;
                    else
                        lblOperationInfo.Text = Messages.SuccessfullyAdded;
                }
                else
                {
                    btnMakeOrder.Enabled = false;
                }
            }
            catch (Exception ex)
            {
                Messages.HandleException(ex);
            }
        }

        private bool ValidateOrderData()
        {
            return (cmbCustomers.SelectedItem as Customer != null); 
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            var orderDetails = dgvOrderDetails.DataSource as List<OrderDetails>;

            foreach (var od in orderDetails)
            {
                InventoryManagementDb.DB.OrderDetails.Remove(od);
                InventoryManagementDb.DB.SaveChanges();
            }

            LoadOrderDetails();
        }

        private void frmAddOrder_Load(object sender, EventArgs e)
        {
            LoadCustomers();

            if (order != null)
            {
                lblEvidentOrder.Text = "Edit Product Info";
                LoadOrderData();
            }
        }

        private void LoadCustomers()
        {
            try
            {
                cmbCustomers.DataSource = InventoryManagementDb.DB.Customers.ToList();
                cmbCustomers.ValueMember = "Id";
                cmbCustomers.DisplayMember = "FullName";
            }
            catch (Exception ex)
            {
                Messages.HandleException(ex);
            }
        }

        private void LoadOrderData()
        {
            cmbCustomers.SelectedValue = order.Customer.Id;
            LoadOrderDetails();
        }

        private void LoadOrderDetails()
        {

            try
            {
                var orderDetails = InventoryManagementDb.DB.OrderDetails.Where(o => o.Order.Id == order.Id).ToList();

                dgvOrderDetails.DataSource = null;
                dgvOrderDetails.DataSource = orderDetails;
            }
            catch (Exception ex)
            {
                Messages.HandleException(ex);
            }
        }

        private void btnAddProduct_Click(object sender, EventArgs e)//TO-DO: if product already exists only add on quantity
        {
            try
            {
                if (order == null)
                {
                    order = new Order()
                    {
                        Customer = (cmbCustomers.SelectedItem as Customer),
                        OrderDate = DateTime.Now.Date,
                        OrderTotal = 0
                    };
                    editOrder = false;
                }
                frmAddOrderDetails frmAddOrderDetails = new frmAddOrderDetails(order);
                frmAddOrderDetails.ShowDialog();

                LoadOrderDetails();
            }
            catch (Exception ex)
            {
                Messages.HandleException(ex);
            }
        }

        private void dgvOrderDetails_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                var orderDetails = dgvOrderDetails.SelectedRows[0].DataBoundItem as OrderDetails;
                if (e.ColumnIndex == 5)
                {
                    Panel pnlChildForm = this.Parent as Panel;

                    if (pnlChildForm != null)
                    {
                        frmAddOrderDetails frmAddOrderDetails = new frmAddOrderDetails(orderDetails);
                        frmAddOrderDetails.FormBorderStyle = FormBorderStyle.None;
                        frmAddOrderDetails.TopLevel = false;
                        frmAddOrderDetails.BringToFront();

                        pnlChildForm.Controls.Clear();
                        pnlChildForm.Controls.Add(frmAddOrderDetails);
                        frmAddOrderDetails.Show();
                        this.Hide();
                    }
                }
                if (e.ColumnIndex == 6
                    && MessageBox.Show(Messages.Delete, Messages.Question, MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    == DialogResult.Yes)
                {
                    InventoryManagementDb.DB.OrderDetails.Remove(orderDetails);
                    InventoryManagementDb.DB.SaveChanges();
                }

                LoadOrderDetails();
            }
            catch (Exception ex)
            {
                Messages.HandleException(ex);
            }
        }
    }
}
