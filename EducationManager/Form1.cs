using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EducationManager
{
    public partial class Form1 : Form
    {
        private static string path;
        private static MySqlConnection connection;

        public Form1()
        {
            InitializeComponent();
            connection = DBUtils.GetDBConnection();

            try
            {
                connection.Open();
                label2.Text = "Connection successful!";
            }
            catch (Exception e)
            {
                MessageBox.Show("Error: " + e.Message);
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBox_output.Text = "";
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                path = openFileDialog.FileName;
                label1.Text = openFileDialog.FileName;
                textBox_speciality.Text = Path.GetFileName(openFileDialog.FileName).Replace(".txt","");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            parse(path);
        }

        private void parse(string path)
        {
            textBox_output.Text = "insert INTO subject (`education_level`, `educational_form`,`speciality`, `year`,`title`,`lecturer`) VALUES ";
            string sql_extra = Environment.NewLine;
            string subject = null;
            string lecturer = null;
            int year = 2022;
            using (StreamReader reader = new StreamReader(path))
            { // (\"@\",\"@\",\"@\",\"@\",@);
                String[] paramses = reader.ReadLine().Split('\t');
                textBox_level.Text = paramses[0];
                textBox_form.Text = paramses[1];
                textBox_speciality.Text = paramses[2];

                sql_extra += "(\""+textBox_level.Text+"\",\""+textBox_form.Text+"\",\""+textBox_speciality.Text+"\",";
                String[] array;
                string line = reader.ReadLine();

                while (line != null)
                {
                    array =  line.Split('\t');
                    if (array[0] == "")
                    {
                        year--;
                        line = reader.ReadLine();
                        continue;
                    }
                    lecturer = array[0];
                    subject = array[1];

                    textBox_output.Text += sql_extra + year.ToString() + ",\"" + subject + "\",\"" + lecturer + "\"),";

                    line = reader.ReadLine();
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            try
            {
                // Команда Insert.
                string sql = "insert INTO subject (`education_level`, `educational_form`, `speciality`, `title`, `year`) VALUES (\"@\",\"@\",\"@\",\"@\",@);";

                MySqlCommand cmd = connection.CreateCommand();
                cmd.CommandText = sql;
                int rowCount = cmd.ExecuteNonQuery();

                Console.WriteLine("Row Count affected = " + rowCount);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void Form1_Leave(object sender, EventArgs e)
        {
            connection.Close();
            connection.Dispose();
            connection = null;

            label2.Text = "Connection exit!";
        }
    }

}
