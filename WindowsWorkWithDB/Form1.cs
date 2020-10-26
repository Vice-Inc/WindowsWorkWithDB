﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Data;
using System.Data.SqlClient;

namespace WindowsWorkWithDB
{
    public partial class Form1 : Form
    {
        private SqlConnection sqlConnection = null;
        private SqlCommandBuilder sqlCommandBuilder = null;
        private SqlDataAdapter sqlDataAdapter = null;//С помощью этого изменяется бд
        private DataSet dataSet = null;//Посредник между бд и DataGridView
        private bool newRowAdding = false;
        //DataGridView - только отображение данных из DataSet

        public Form1()
        {
            InitializeComponent();
        }

        private void LoadData()
        {
            try
            {
                sqlDataAdapter = new SqlDataAdapter("SELECT * , 'Delete' AS [Command] FROM Users", sqlConnection);

                sqlCommandBuilder = new SqlCommandBuilder(sqlDataAdapter);

                sqlCommandBuilder.GetInsertCommand();
                sqlCommandBuilder.GetUpdateCommand();
                sqlCommandBuilder.GetDeleteCommand();

                dataSet = new DataSet();

                sqlDataAdapter.Fill(dataSet, "Users");

                dataGridView1.DataSource = dataSet.Tables["Users"];

                //кнопки удалить итд чтобы были активны(замена в таблице)
                for(int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    //ячейка таблицы
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();

                    dataGridView1[6, i] = linkCell;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReLoadData()
        {
            try
            {
                dataSet.Tables["Users"].Clear();
                
                sqlDataAdapter.Fill(dataSet, "Users");

                dataGridView1.DataSource = dataSet.Tables["Users"];

                //кнопки удалить итд чтобы были активны(замена в таблице)
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    //ячейка таблицы
                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();

                    dataGridView1[6, i] = linkCell;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //Строка подключения
            sqlConnection = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Xiaomi\Documents\C#\WindowsWorkWithDB\WindowsWorkWithDB\Users.mdf;Integrated Security=True");

            sqlConnection.Open();

            LoadData();
        }

        //Нажатия на ячейки
        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if(e.ColumnIndex == 6)
                {
                    string task = dataGridView1.Rows[e.RowIndex].Cells[6].Value.ToString();

                    if(task == "Delete")
                    {
                        if(MessageBox.Show("Удалить эту строку?", "Удаление", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                            == DialogResult.Yes)
                        {
                            int rowIndex = e.RowIndex;
                            dataGridView1.Rows.RemoveAt(rowIndex);
                            dataSet.Tables["Users"].Rows[rowIndex].Delete();

                            sqlDataAdapter.Update(dataSet, "Users");
                        }
                    }
                    else if (task == "Insert")
                    {
                        int rowIndex = e.RowIndex;

                        DataRow row = dataSet.Tables["Users"].NewRow();

                        row["Name"] = dataGridView1.Rows[rowIndex].Cells["Name"].Value;
                        row["SName"] = dataGridView1.Rows[rowIndex].Cells["SName"].Value;
                        row["Age"] = dataGridView1.Rows[rowIndex].Cells["Age"].Value;
                        row["Email"] = dataGridView1.Rows[rowIndex].Cells["Email"].Value;
                        row["Phone"] = dataGridView1.Rows[rowIndex].Cells["Phone"].Value;

                        dataSet.Tables["Users"].Rows.Add(row);
                        dataSet.Tables["Users"].Rows.RemoveAt(dataSet.Tables["Users"].Rows.Count - 1);
                        dataGridView1.Rows.RemoveAt(dataGridView1.Rows.Count - 2);

                        dataGridView1.Rows[e.RowIndex].Cells[6].Value = "Delete";

                        sqlDataAdapter.Update(dataSet, "Users");

                        newRowAdding = false;
                    }
                    else if(task == "Update")
                    {
                        int rowIndex = e.RowIndex;

                        dataSet.Tables["Users"].Rows[rowIndex]["Name"] = dataGridView1.Rows[rowIndex].Cells["Name"].Value;
                        dataSet.Tables["Users"].Rows[rowIndex]["SName"] = dataGridView1.Rows[rowIndex].Cells["SName"].Value;
                        dataSet.Tables["Users"].Rows[rowIndex]["Age"] = dataGridView1.Rows[rowIndex].Cells["Age"].Value;
                        dataSet.Tables["Users"].Rows[rowIndex]["Email"] = dataGridView1.Rows[rowIndex].Cells["Email"].Value;
                        dataSet.Tables["Users"].Rows[rowIndex]["Phone"] = dataGridView1.Rows[rowIndex].Cells["Phone"].Value;

                        sqlDataAdapter.Update(dataSet, "Users");

                        dataGridView1.Rows[e.RowIndex].Cells[6].Value = "Delete";
                    }

                    ReLoadData();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //Кнопка обновить
        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            ReLoadData();
        }

        //Добавление строки
        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            try
            {
                if(newRowAdding == false)
                {
                    newRowAdding = true;

                    int lastRow = dataGridView1.Rows.Count - 2;

                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView1[6, lastRow] = linkCell;

                    DataGridViewRow row = dataGridView1.Rows[lastRow];
                    row.Cells["Command"].Value = "Insert";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if(newRowAdding == false)
                {
                    int rowIndex = dataGridView1.SelectedCells[0].RowIndex;

                    DataGridViewLinkCell linkCell = new DataGridViewLinkCell();
                    dataGridView1[6, rowIndex] = linkCell;

                    DataGridViewRow editRow = dataGridView1.Rows[rowIndex];
                    editRow.Cells["Command"].Value = "Update";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            e.Control.KeyPress -= new KeyPressEventHandler(Column_KeyPress);

            if(dataGridView1.CurrentCell.ColumnIndex == 3)
            {
                TextBox textBox = e.Control as TextBox;

                if(textBox != null)
                {
                    textBox.KeyPress += new KeyPressEventHandler(Column_KeyPress);
                }
            }
        }

        private void Column_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}