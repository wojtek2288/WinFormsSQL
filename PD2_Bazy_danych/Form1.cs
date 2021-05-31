using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace PD1_Bazy_danych
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        SqlConnection con = new SqlConnection("Data Source=DESKTOP-6170UO7;Initial Catalog=PD1Movies;Integrated Security=True");
        int index = -1;
        private void Form1_Load(object sender, EventArgs e)
        {
            GetRecords();
        }

        private void GetRecords()
        {
            SqlCommand cmd = new SqlCommand("SELECT * FROM Movies", con);
            DataTable dt = new DataTable();

            con.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            dt.Load(sdr);
            con.Close();

            dataGridView1.DataSource = dt;
            dataGridView1.Columns[0].Visible = false;
            dataGridView1.Columns[1].Width = 162;
            dataGridView1.Columns[3].DefaultCellStyle.Format = "0";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SqlCommand cmd = SearchCommand();
            DataTable dt = new DataTable();

            con.Open();
            SqlDataReader sdr = cmd.ExecuteReader();
            dt.Load(sdr);
            con.Close();

            dataGridView1.DataSource = dt;
        }

        private SqlCommand SearchCommand()
        {
            var cmd = new SqlCommand
            {
                Connection = con
            };

            var str = new StringBuilder("SELECT * FROM Movies WHERE ReleaseDate >= @startdate AND ReleaseDate <= @enddate AND Imax3D = @imax");

            var sdate = new SqlParameter
            {
                ParameterName = "@startdate",
                Value = dateTimePicker1.Value,
                SqlDbType = SqlDbType.Date
            };

            var edate = new SqlParameter
            {
                ParameterName = "@enddate",
                Value = dateTimePicker2.Value,
                SqlDbType = SqlDbType.Date
            };

            var imax = new SqlParameter
            {
                ParameterName = "@imax",
                Value = checkBox1.Checked,
                SqlDbType = SqlDbType.Bit
            };

            cmd.Parameters.Add(sdate);
            cmd.Parameters.Add(edate);
            cmd.Parameters.Add(imax);
            
            if(textBox1.Text.Length != 0)
            {
                str.Append(" AND Title LIKE @title");
                var title = new SqlParameter
                {
                    ParameterName = "@title",
                    Value = $"%{textBox1.Text}%",
                    SqlDbType = SqlDbType.VarChar,
                    Size = 50
                };
                cmd.Parameters.Add(title);
            }
            
            if(textBox2.Text.Length != 0)
            {
                str.Append(" AND Budget >= @budgetmin");
                var budgetmin = new SqlParameter
                {
                    ParameterName = "@budgetmin",
                    Value = int.Parse(textBox2.Text),
                    SqlDbType = SqlDbType.Money,
                };
                cmd.Parameters.Add(budgetmin);
            }

            if (textBox3.Text.Length != 0)
            {
                str.Append(" AND Budget <= @budgetmax");
                var budgetmax = new SqlParameter
                {
                    ParameterName = "@budgetmax",
                    Value = int.Parse(textBox3.Text),
                    SqlDbType = SqlDbType.Money,
                };
                cmd.Parameters.Add(budgetmax);
            }

            if(textBox4.Text.Length != 0)
            {
                str.Append(" AND AvgRating >= @avgminrating");
                var avgminrating = new SqlParameter
                {
                    ParameterName = "@avgminrating",
                    Value = double.Parse(textBox4.Text),
                    SqlDbType = SqlDbType.Float,
                    Scale = 18,
                    Precision = 2
                };
                cmd.Parameters.Add(avgminrating);
            }

            if (textBox5.Text.Length != 0)
            {
                str.Append(" AND AvgRating <= @avgmaxrating");
                var avgmaxrating = new SqlParameter
                {
                    ParameterName = "@avgmaxrating",
                    Value = double.Parse(textBox5.Text),
                    SqlDbType = SqlDbType.Float,
                    Scale = 18,
                    Precision = 2
                };
                cmd.Parameters.Add(avgmaxrating);
            }

            cmd.CommandText = str.ToString();
            return cmd;
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("UPDATE Movies SET " + dataGridView1.Columns[e.ColumnIndex].Name + " = @cell WHERE Id = " + dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString(), con);
            switch(dataGridView1.Columns[e.ColumnIndex].Name)
            {
                case "Title":
                    var cell1 = new SqlParameter
                    {
                        ParameterName = "@cell",
                        Value = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString(),
                        SqlDbType = SqlDbType.VarChar,
                        Size = 50
                    };
                    cmd.Parameters.Add(cell1);
                    break;
                case "ReleaseDate":
                    var cell2 = new SqlParameter
                    {
                        ParameterName = "@cell",
                        Value = (DateTime)dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value,
                        SqlDbType = SqlDbType.Date
                    };
                    cmd.Parameters.Add(cell2);
                    break;
                case "Budget":
                    var cell3 = new SqlParameter
                    {
                        ParameterName = "@cell",
                        Value = Int32.Parse(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()),
                        SqlDbType = SqlDbType.Money
                    };
                    cmd.Parameters.Add(cell3);
                    break;
                case "AvgRating":
                    var cell4 = new SqlParameter
                    {
                        ParameterName = "@cell",
                        Value = double.Parse(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()),
                        SqlDbType = SqlDbType.Float,
                        Scale = 18,
                        Precision = 2
                    };
                    cmd.Parameters.Add(cell4);
                    break;
                case "Imax3D":
                    var cell5 = new SqlParameter
                    {
                        ParameterName = "@cell",
                        Value = bool.Parse(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()),
                        SqlDbType = SqlDbType.Bit
                    };
                    cmd.Parameters.Add(cell5);
                    break;
                case "TicketsSold":
                    var cell6 = new SqlParameter
                    {
                        ParameterName = "@cell",
                        Value = Int32.Parse(dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString()),
                        SqlDbType = SqlDbType.Int
                    };
                    cmd.Parameters.Add(cell6);
                    break;
            }
            cmd.ExecuteNonQuery();
            con.Close();
        }
        private void AddRow()
        {
            if (index != -1)
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO Movies VALUES (@title, @date, @budget, @avgrating, @imax, @tickets)", con);
                var title = new SqlParameter
                {
                    ParameterName = "@title",
                    Value = dataGridView1.Rows[index].Cells[1].Value.ToString(),
                    SqlDbType = SqlDbType.VarChar,
                    Size = 50
                };
                var date = new SqlParameter
                {
                    ParameterName = "@date",
                    Value = (DateTime)dataGridView1.Rows[index].Cells[2].Value,
                    SqlDbType = SqlDbType.Date
                };
                var budget = new SqlParameter
                {
                    ParameterName = "@budget",
                    Value = Int32.Parse(dataGridView1.Rows[index].Cells[3].Value.ToString()),
                    SqlDbType = SqlDbType.Money
                };
                var avgrating = new SqlParameter
                {
                    ParameterName = "@avgrating",
                    Value = double.Parse(dataGridView1.Rows[index].Cells[4].Value.ToString()),
                    SqlDbType = SqlDbType.Float,
                    Scale = 18,
                    Precision = 2
                };
                var imax = new SqlParameter();
                imax.ParameterName = "@imax";
                imax.SqlDbType = SqlDbType.Bit;
                if (dataGridView1.Rows[index].Cells[5].Value != null)
                {
                    imax.Value = bool.Parse(dataGridView1.Rows[index].Cells[5].Value.ToString());
                }
                else
                {
                    imax.Value = false;
                }
                    
                var tickets = new SqlParameter
                {
                    ParameterName = "@tickets",
                    Value = Int32.Parse(dataGridView1.Rows[index].Cells[6].Value.ToString()),
                    SqlDbType = SqlDbType.Int
                };

                cmd.Parameters.Add(title);
                cmd.Parameters.Add(date);
                cmd.Parameters.Add(budget);
                cmd.Parameters.Add(avgrating);
                cmd.Parameters.Add(imax);
                cmd.Parameters.Add(tickets);

                cmd.ExecuteNonQuery();
                con.Close();
            }
            index = -1;
        }
        private void dataGridView1_UserAddedRow(object sender, DataGridViewRowEventArgs e)
        {
            index = e.Row.Index - 1;
        }

        private void dataGridView1_RowValidated(object sender, DataGridViewCellEventArgs e)
        {
            AddRow();
        }

        private void dataGridView1_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand("DELETE FROM Movies WHERE Id =" + dataGridView1.Rows[e.Row.Index].Cells[0].Value.ToString(), con);
            cmd.ExecuteNonQuery();
            con.Close();
        }
    }
}
