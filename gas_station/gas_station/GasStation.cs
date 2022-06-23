using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace gas_station
{
    class GasStation
    {
        private class Position
        {
            public readonly string Name;
            public readonly decimal Cost; //per liter
            public decimal Amount; //in liters

            public Position(string name, decimal cost, decimal amount)
            {
                Name = name;
                Cost = cost;
                Amount = amount;
            }

            public bool Sell(Client client, out decimal profit)
            {
                if(Amount==0)
                {
                    profit = 0;
                    return false;
                }
                if(client.Need>=Amount)
                {
                    Output.Record("Sold: " + Amount.ToString() + " leters of " + Name + ". Profit:" + (Amount * Cost).ToString());
                    client.Need -= Amount;
                    profit = Amount * Cost;
                    Amount = 0;
                    
                    return false;
                }
                Output.Record("Sold: " + client.Need.ToString() + " leters of " + Name + ". Profit:" + (client.Need * Cost).ToString());
                Amount -= client.Need;
                profit = client.Need * Cost;
                client.Need = 0;
                return true;
            }

            public string Info()
            {
                return Name + "; Cost:" + Cost.ToString() + "; Amount:" + Amount.ToString();
            }
        }

        private List<Position> positions = new List<Position>();
        private decimal Profit = 0;

        public void CreatePosition(string gasoline, decimal cost, decimal amount)
        {
            Position pos = null;
            pos = positions.Find(p => p.Name == gasoline);
            if (pos != null)
                DeletePosition(gasoline);
            positions.Add(new Position(gasoline, cost, amount));
        }

        public void DeletePosition(string gasoline)
        {
            Position pos = null;
            pos = positions.Find(p => p.Name == gasoline);
            if (pos == null)
                return;
            positions.Remove(pos);
        }

        public bool Sell(Client client)
        {
            Position pos = null;
            pos = positions.Find(p => p.Name == client.GasolineName);
            if (pos == null)
                return false;
            bool result = pos.Sell(client, out decimal profit);
            Profit += profit;
            return result;
        }

        public List<string> Info()
        {
            List<string> info = new List<string>
            {
                "Profit:"+Profit.ToString(),
                "Positions:"
            };
            foreach(Position p in positions)
            {
                info.Add(p.Info());
            }
            return info;
        }
    }

    class Client
    {
        public readonly string GasolineName;
        public decimal Need;

        public Client(string gasoline, decimal need)
        {
            GasolineName = gasoline;
            Need = need;
        }
    }

    class Simulation
    {
        private int Days;
        private int MaxCarsPerDay;
        private int MinCarsPerDay;

        GasStation Station = null;
        List<string> Gasoline = null;
        List<int> Chance = null;

        public Simulation(int days, int min, int max)
        {
            Days = days;
            MaxCarsPerDay = max;
            MinCarsPerDay = min;
        }

        public bool SetParameters(List<string> gasoline, List<int> chance, List<decimal> amount, List<decimal> cost)
        {
            gasoline = gasoline.Distinct().ToList();
            if (gasoline.Count < 1)
                return false;
            if (chance.Count != gasoline.Count || amount.Count != gasoline.Count || cost.Count != gasoline.Count)
                return false;
            foreach (string s in gasoline)
                if (s.Length == 0)
                    return false;
            foreach (int i in chance)
                if (i <= 0)
                    return false;
            foreach (decimal d in amount)
                if (d <= 0)
                    return false;
            foreach (decimal d in cost)
                if (d <= 0)
                    return false;

            Gasoline = new List<string>(gasoline);
            Chance = new List<int>(chance);
            Station = new GasStation();
            for (int j = 0; j < gasoline.Count; j++)
            {
                Station.CreatePosition(Gasoline[j], cost[j], amount[j]);
            }
            return true;
        }

        private Client GetCar()
        {
            Random rand = SingletoneRandomizer.Get();

            int sum = 0;
            foreach (int i in Chance)
                sum += i;

            int r = rand.Next(0, sum + 1);
            int j;
            for (j = 0; j < Gasoline.Count; j++)
            {
                if (r <= Chance[j])
                    break;
                r -= Chance[j];
            }
            string gas = Gasoline[j];

            return new Client(gas, rand.Next(10, 80));
        }

        public void Simulate()
        {
            List<string> rec = new List<string> { "Starting simulation", "Initial state:" };
            rec.AddRange(Station.Info());
            Output.Record(rec);
            Output.Record("");
            rec.Clear();

            Random rand = SingletoneRandomizer.Get();
            Client client;
            int carsNum;

            for (int i = 0; i < Days; i++)
            {
                Output.Record("Day " + (i + 1).ToString());
                carsNum = rand.Next(MinCarsPerDay, MaxCarsPerDay + 1);
                for(int j=0;j<carsNum;j++)
                {
                    client = GetCar();
                    if(!Station.Sell(client))
                    {
                        //Some type of fuel is over
                        rec.Add("");
                        rec.Add("Simulation is over");
                        rec.Add("Final state:");
                        rec.AddRange(Station.Info());
                        Output.Record(rec);
                        return;
                    }
                }
                Output.Record("");
            }
            rec.Add("Simulation is over");
            rec.Add("Final state:");
            rec.AddRange(Station.Info());
            Output.Record(rec);
        }
    }

    static class SingletoneRandomizer
    {
        static Random rand = new Random();

        public static Random Get()
        {
            return rand;
        }
    }

    static class Output
    {
        private static string file = "output.txt";
        private static bool clean = true;

        public static void Record(string s)
        {
            StreamWriter writer = new StreamWriter(file, !clean);
            writer.WriteLine(s);
            writer.Close();
            clean = false;
        }

        public static void Record(List<string> l)
        {
            StreamWriter writer = new StreamWriter(file, !clean);
            foreach(string s in l)
                writer.WriteLine(s);
            writer.Close();
            clean = false;
        }
    }
}