using System; // Directiva using pentru spațiul de nume System, care conține clasele de bază din .NET Framework.
using Microsoft.EntityFrameworkCore; // Directiva using pentru spațiul de nume Microsoft.EntityFrameworkCore, care conține clasele necesare pentru lucru cu Entity Framework Core.
using System.Linq; // Directiva using pentru spațiul de nume System.Linq, care permite utilizarea funcționalităților LINQ.
using Microsoft.EntityFrameworkCore.SqlServer; // Directiva using pentru spațiul de nume Microsoft.EntityFrameworkCore.SqlServer, care conține extensiile pentru lucrul cu SQL Server în Entity Framework Core.
using System.Collections.Generic; // Directiva using pentru spațiul de nume System.Collections.Generic, care conține clasele pentru lucrul cu colecții generice.

namespace ContBancarApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var dbContext = new UserDbContext()) // Crearea și utilizarea unei instanțe a clasei UserDbContext într-un bloc using pentru a asigura eliberarea resurselor conexiunii la bază de date.
            {
                dbContext.Database.EnsureCreated(); // Asigurarea că baza de date și tabelele sunt create (dacă nu există deja).

                BankSystem bankSystem = new BankSystem(dbContext); // Inițializarea și crearea unui obiect de tip BankSystem cu contextul bazei de date.
                bankSystem.Start(); // Apelarea metodei Start() pentru a începe execuția sistemului bancar.
            }
        }
    }

    public class BankSystem
    {
        private UserDbContext _dbContext; // Declarația unei variabile private _dbContext de tipul UserDbContext.

        public BankSystem(UserDbContext dbContext) // Constructorul clasei BankSystem care primește un obiect de tipul UserDbContext.
        {
            _dbContext = dbContext; // Inițializarea variabilei _dbContext cu obiectul primit ca parametru.
        }

        public void Start() // Metoda Start() care inițializează și conține bucla principală a sistemului bancar.
        {
            while (true) // Bucla infinită care menține sistemul bancar în execuție până când utilizatorul decide să iasă.
            {
                Console.WriteLine("1. Creare cont"); // Afisarea opțiunii pentru crearea unui cont.
                Console.WriteLine("2. Logare"); // Afisarea opțiunii pentru logare.
                Console.WriteLine("3. Iesire"); // Afisarea opțiunii pentru a ieși din program.

                Console.Write("Alegeti optiunea: "); // Solicitarea utilizatorului să aleagă o opțiune.
                string choice = Console.ReadLine(); // Citirea opțiunii alese de utilizator.

                switch (choice) // Instrucțiunea switch care dirijează programul în funcție de opțiunea aleasă de utilizator.
                {
                    case "1": // Cazul în care utilizatorul alege să creeze un cont.
                        CreateAccount(); // Apelarea metodei pentru crearea unui cont.
                        break;
                    case "2": // Cazul în care utilizatorul alege să se autentifice.
                        Login(); // Apelarea metodei pentru autentificare.
                        break;
                    case "3": // Cazul în care utilizatorul alege să iasă din program.
                        Environment.Exit(0); // Ieșirea din program cu codul 0 (fără erori).
                        break;
                    default: // Cazul în care utilizatorul introduce o opțiune invalidă.
                        Console.WriteLine("Opțiune invalidă! Încercați din nou."); // Afisarea unui mesaj de eroare.
                        break;
                }
            }
        }

        private void CreateAccount() // Metoda pentru crearea unui cont nou.
        {
            Console.Write("Introduceti numele de utilizator: "); // Solicitarea introducerii numelui de utilizator.
            string username = Console.ReadLine(); // Citirea numelui de utilizator introdus.

            Console.Write("Introduceti parola: "); // Solicitarea introducerii parolei.
            string password = Console.ReadLine(); // Citirea parolei introduse.

            if (_dbContext.Users.Any(u => u.Username == username)) // Verificarea dacă numele de utilizator există deja în baza de date.
            {
                Console.WriteLine("Numele de utilizator există deja!"); // Afisarea unui mesaj de eroare.
                return; // Ieșirea prematură din metoda deoarece numele de utilizator există deja.
            }

            var newUser = new User { Username = username, Password = password, Balance = 0 }; // Inițializarea unui obiect de tip User cu datele introduse.
            _dbContext.Users.Add(newUser); // Adăugarea noului utilizator în baza de date.
            _dbContext.SaveChanges(); // Salvarea modificărilor în baza de date.
            Console.WriteLine("Cont creat cu succes!"); // Afisarea unui mesaj de confirmare.
        }

        private void Login() // Metoda pentru autentificarea utilizatorului.
        {
            Console.Write("Introduceti numele de utilizator: "); // Solicitarea introducerii numelui de utilizator.
            string username = Console.ReadLine(); // Citirea numelui de utilizator introdus.

            Console.Write("Introduceti parola: "); // Solicitarea introducerii parolei.
            string password = Console.ReadLine(); // Citirea parolei introduse.

            var user = _dbContext.Users.FirstOrDefault(u => u.Username == username && u.Password == password); // Căutarea utilizatorului în baza de date.

            if (user != null) // Verificarea dacă utilizatorul a fost găsit în baza de date.
            {
                UserSession session = new UserSession(_dbContext, user); // Inițializarea unei sesiuni de utilizator cu datele găsite.
                session.Start(); // Pornirea sesiunii de utilizator.
            }
            else
            {
                Console.WriteLine("Nume de utilizator sau parola incorecta!"); // Afisarea unui mesaj de eroare în cazul în care autentificarea a eșuat.
            }
        }
    }

    public class UserSession
    {
        private UserDbContext _dbContext; // Declarația unei variabile private _dbContext de tipul UserDbContext.
        private User _user; // Declarația unei variabile private _user de tipul User.

        public UserSession(UserDbContext dbContext, User user) // Constructorul clasei UserSession care primește un obiect de tipul UserDbContext și un obiect de tipul User.
        {
            _dbContext = dbContext; // Inițializarea variabilei _dbContext cu obiectul primit ca prim parametru.
            _user = user; // Inițializarea variabilei _user cu obiectul primit ca al doilea parametru.
        }

        public void Start() // Metoda Start() care inițializează și conține bucla principală a sesiunii de utilizator.
        {
            while (true) // Bucla infinită care menține sesiunea de utilizator în execuție până când utilizatorul decide să se delogheze.
            {
                Console.WriteLine("1. Alimentare cont"); // Afisarea opțiunii pentru alimentarea contului.
                Console.WriteLine("2. Interogare sold"); // Afisarea opțiunii pentru interogarea soldului.
                Console.WriteLine("3. Retragere numerar"); // Afisarea opțiunii pentru retragerea numerarului.
                Console.WriteLine("4. Delogare"); // Afisarea opțiunii pentru delogare.

                Console.Write("Alegeti optiunea: "); // Solicitarea utilizatorului să aleagă o opțiune.
                string choice = Console.ReadLine(); // Citirea opțiunii alese de utilizator.

                switch (choice) // Instrucțiunea switch care dirijează programul în funcție de opțiunea aleasă de utilizator.
                {
                    case "1": // Cazul în care utilizatorul alege să alimenteze contul.
                        Deposit(); // Apelarea metodei pentru alimentarea contului.
                        break;
                    case "2": // Cazul în care utilizatorul alege să interogheze soldul.
                        CheckBalance(); // Apelarea metodei pentru interogarea soldului.
                        break;
                    case "3": // Cazul în care utilizatorul alege să retragă numerar.
                        Withdraw(); // Apelarea metodei pentru retragerea numerarului.
                        break;
                    case "4": // Cazul în care utilizatorul alege să se delogheze.
                        return; // Ieșirea din metoda deoarece utilizatorul s-a delogat.
                    default: // Cazul în care utilizatorul introduce o opțiune invalidă.
                        Console.WriteLine("Opțiune invalida! Încercați din nou."); // Afisarea unui mesaj de eroare.
                        break;
                }
            }
        }

        private void Deposit() // Metoda pentru alimentarea contului.
        {
            Console.Write("Introduceti suma de bani de depus: "); // Solicitarea introducerii sumei de bani.
            double amount = Convert.ToDouble(Console.ReadLine()); // Citirea sumei de bani introduse și conversia acesteia la tipul double.

            _user.Balance += amount; // Adăugarea sumei de bani introduse la soldul utilizatorului.
            _dbContext.SaveChanges(); // Salvarea modificărilor în baza de date.
            Console.WriteLine("Suma de bani a fost depusa cu succes!"); // Afisarea unui mesaj de confirmare.
        }

        private void CheckBalance() // Metoda pentru interogarea soldului contului.
        {
            Console.WriteLine($"Soldul contului este: {_user.Balance} RON"); // Afisarea soldului contului.
        }

        private void Withdraw() // Metoda pentru retragerea numerarului din cont.
        {
            Console.Write("Introduceti suma de bani de retras: "); // Solicitarea introducerii sumei de bani pentru retragere.
            double amount = Convert.ToDouble(Console.ReadLine()); // Citirea sumei de bani introduse și conversia acesteia la tipul double.

            if (_user.Balance >= amount) // Verificarea dacă utilizatorul are suficienți bani în cont pentru retragerea sumei dorite.
            {
                _user.Balance -= amount; // Deducerea sumei de bani retrase din soldul utilizatorului.
                _dbContext.SaveChanges(); // Salvarea modificărilor în baza de date.
                Console.WriteLine("Suma de bani a fost retrasa cu succes!"); // Afisarea unui mesaj de confirmare.
            }
            else
            {
                Console.WriteLine("Fonduri insuficiente pentru retragere!"); // Afisarea unui mesaj de eroare în cazul în care fondurile sunt insuficiente.
            }
        }
    }

    public class User // Definirea clasei User care reprezintă un utilizator în sistemul bancar.
    {
        public int UserId { get; set; } // Proprietatea UserId care reprezintă ID-ul utilizatorului.
        public string Username { get; set; } // Proprietatea Username care reprezintă numele de utilizator.
        public string Password { get; set; } // Proprietatea Password care reprezintă parola utilizatorului.
        public double Balance { get; set; } // Proprietatea Balance care reprezintă soldul utilizatorului.
    }

    public class UserDbContext : DbContext // Definirea clasei UserDbContext care reprezintă contextul bazei de date.
    {
        public DbSet<User> Users { get; set; } // Proprietatea Users care reprezintă tabela utilizatorilor din baza de date.

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) // Suprascrierea metodei OnConfiguring pentru configurarea opțiunilor contextului bazei de date.
        {
             optionsBuilder.UseSqlServer("Server=;Database=;Trusted_Connection=True;"); // Configurarea conexiunii la baza de date SQL Server.
           
        }
    }
       
}