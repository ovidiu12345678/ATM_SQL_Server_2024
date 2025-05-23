using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AplicatieBancara
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var contextUtilizatori = new ContextUtilizatori())
            {
                contextUtilizatori.Database.EnsureCreated();

                SistemBancar sistemBancar = new SistemBancar(contextUtilizatori);
                sistemBancar.Porneste();
            }
        }
    }

    public class SistemBancar
    {
        private ContextUtilizatori _context;

        public SistemBancar(ContextUtilizatori context)
        {
            _context = context;
        }

        public void Porneste()
        {
            while (true)
            {
                Console.WriteLine("1. Creeaza cont");
                Console.WriteLine("2. Autentificare");
                Console.WriteLine("3. Iesire");
                Console.Write("Alege optiunea: ");
                string optiune = Console.ReadLine();

                switch (optiune)
                {
                    case "1":
                        CreeazaCont();
                        break;
                    case "2":
                        Autentificare();
                        break;
                    case "3":
                        Environment.Exit(0);
                        break;
                    default:
                        Console.WriteLine("Optiune invalida! Incearca din nou.");
                        break;
                }
            }
        }

        private void CreeazaCont()
        {
            Console.Write("Introdu numele de utilizator: ");
            string numeUtilizator = Console.ReadLine();

            Console.Write("Introdu parola: ");
            string parola = Console.ReadLine();

            if (_context.Utilizatori.Any(u => u.NumeUtilizator == numeUtilizator))
            {
                Console.WriteLine("Numele de utilizator exista deja!");
                return;
            }

            var utilizatorNou = new Utilizator { NumeUtilizator = numeUtilizator, Parola = parola, Sold = 0 };
            _context.Utilizatori.Add(utilizatorNou);
            _context.SaveChanges();
            Console.WriteLine("Cont creat cu succes!");
        }

        private void Autentificare()
        {
            Console.Write("Introdu numele de utilizator: ");
            string numeUtilizator = Console.ReadLine();

            Console.Write("Introdu parola: ");
            string parola = Console.ReadLine();

            var utilizator = _context.Utilizatori.FirstOrDefault(u => u.NumeUtilizator == numeUtilizator && u.Parola == parola);

            if (utilizator != null)
            {
                SesiuneUtilizator sesiune = new SesiuneUtilizator(_context, utilizator);
                sesiune.Porneste();
            }
            else
            {
                Console.WriteLine("Nume de utilizator sau parola incorecta!");
            }
        }
    }

    public class SesiuneUtilizator
    {
        private ContextUtilizatori _context;
        private Utilizator _utilizator;

        public SesiuneUtilizator(ContextUtilizatori context, Utilizator utilizator)
        {
            _context = context;
            _utilizator = utilizator;
        }

        public void Porneste()
        {
            while (true)
            {
                Console.WriteLine("1. Depune bani");
                Console.WriteLine("2. Afiseaza soldul");
                Console.WriteLine("3. Retrage bani");
                Console.WriteLine("4. Delogare");

                Console.Write("Alege optiunea: ");
                string optiune = Console.ReadLine();

                switch (optiune)
                {
                    case "1":
                        DepuneBani();
                        break;
                    case "2":
                        AfiseazaSold();
                        break;
                    case "3":
                        RetrageBani();
                        break;
                    case "4":
                        return;
                    default:
                        Console.WriteLine("Optiune invalida! Incearca din nou.");
                        break;
                }
            }
        }

        private void DepuneBani()
        {
            Console.Write("Introdu suma de depus: ");
            if (double.TryParse(Console.ReadLine(), out double suma) && suma > 0)
            {
                _utilizator.Sold += suma;
                _context.SaveChanges();
                Console.WriteLine("Suma depusa cu succes!");
            }
            else
            {
                Console.WriteLine("Suma introdusa nu este valida!");
            }
        }

        private void AfiseazaSold()
        {
            Console.WriteLine($"Soldul tau este: {_utilizator.Sold} RON");
        }

        private void RetrageBani()
        {
            Console.Write("Introdu suma de retras: ");
            if (double.TryParse(Console.ReadLine(), out double suma) && suma > 0)
            {
                if (_utilizator.Sold >= suma)
                {
                    _utilizator.Sold -= suma;
                    _context.SaveChanges();
                    Console.WriteLine("Suma retrasa cu succes!");
                }
                else
                {
                    Console.WriteLine("Fonduri insuficiente!");
                }
            }
            else
            {
                Console.WriteLine("Suma introdusa nu este valida!");
            }
        }
    }

    public class Utilizator
    {
        public int UtilizatorId { get; set; }
        public string NumeUtilizator { get; set; }
        public string Parola { get; set; }
        public double Sold { get; set; }
    }

    public class ContextUtilizatori : DbContext
    {
        public DbSet<Utilizator> Utilizatori { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=;Database=;Trusted_Connection=True;");
        }
    }
}
