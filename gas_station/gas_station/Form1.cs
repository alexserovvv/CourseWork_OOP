using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace gas_station
{
    public partial class Form1 : Form
    {
        List<string> Gasoline = new List<string>();
        List<int> Chance = new List<int>();
        List<decimal> Amount = new List<decimal>();
        List<decimal> Cost = new List<decimal>();

        List<string> ListBoxData = new List<string>();

        public Form1()
        {
            InitializeComponent();
            listBox1.DataSource = ListBoxData;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string name = textBox1.Text;
            int chance = (int)numericUpDown4.Value;
            string s_amount = textBox2.Text;
            string s_cost = textBox3.Text;

            if (name == "")
                return;
            if (!decimal.TryParse(s_amount, out decimal amount))
                return;
            if (!decimal.TryParse(s_cost, out decimal cost))
                return;
            if (amount <= 0)
                return;
            if (cost <= 0)
                return;
            if (Gasoline.Contains(name))
                return;

            Gasoline.Add(name);
            Chance.Add(chance);
            Amount.Add(amount);
            Cost.Add(cost);

            ListBoxData.Add(name + ";chance:" + chance.ToString() + ";amount(liters):" + amount.ToString() + ";cost:" + cost.ToString());
            listBox1.DataSource = new List<string>();
            listBox1.DataSource = ListBoxData;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (ListBoxData.Count == 0)
                return;
            int index = listBox1.SelectedIndex;
            ListBoxData.RemoveAt(index);
            Gasoline.RemoveAt(index);
            Chance.RemoveAt(index);
            Amount.RemoveAt(index);
            Cost.RemoveAt(index);
            listBox1.DataSource = new List<string>();
            listBox1.DataSource = ListBoxData;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (ListBoxData.Count == 0)
                return;
            int days = (int)numericUpDown1.Value;
            int minCars = (int)numericUpDown2.Value;
            int maxCars = (int)numericUpDown3.Value;
            if (minCars > maxCars)
                return;
            Simulation sim = new Simulation(days, minCars, maxCars);
            sim.SetParameters(Gasoline, Chance, Amount, Cost);
            sim.Simulate();
            MessageBox.Show("Файл output.txt перезаписан");
        }
    }
}
