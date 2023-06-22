using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

class Program
{
    static async Task Main(string[] args)
    {
        Console.Write("Inserisci il numero di utenti da generare: ");
        int numUsers = int.Parse(Console.ReadLine());

        using (HttpClient client = new HttpClient())
        {
            string apiUrl = "https://randomuser.me/api/?results=" + numUsers;

            // Esegui la richiesta API
            HttpResponseMessage response = await client.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                // Leggi la risposta come stringa JSON
                string jsonResponse = await response.Content.ReadAsStringAsync();

                // Deserializza la stringa JSON in un oggetto UserResponse
                UserResponse userResponse = JsonConvert.DeserializeObject<UserResponse>(jsonResponse);

                // Aggiungi l'età randomica per ciascun utente
                Random random = new Random();
                foreach (User user in userResponse.Results)
                {
                    // Genera un'età randomica compresa tra 18 e 65
                    user.Age = random.Next(18, 66);
                }

                // Stampa i dati di ciascun utente
                int index = 0;
                foreach (User user in userResponse.Results)
                {
                    index++;
                    Console.WriteLine($"[{index}] Nome: {user.Name.First} {user.Name.Last}");
                    Console.WriteLine($"     Email: {user.Email}");
                    Console.WriteLine($"     Nazione: {user.Location.Country}");
                    Console.WriteLine($"     Numero di telefono: {user.Phone}");
                    Console.WriteLine($"     Età: {user.Age}");
                    Console.WriteLine($"     Immagine: {user.Picture.Large}");
                    Console.WriteLine("-----------------------------");
                }

                Console.Write("Seleziona un utente per visualizzare ulteriori dettagli (inserisci l'indice): ");
                int selectedUserIndex = int.Parse(Console.ReadLine());

                if (selectedUserIndex >= 1 && selectedUserIndex <= userResponse.Results.Count)
                {
                    User selectedUser = userResponse.Results[selectedUserIndex - 1];

                    Console.WriteLine($"Dettagli per l'utente [{selectedUserIndex}]:");
                    Console.WriteLine($"Nome completo: {selectedUser.Name.First} {selectedUser.Name.Last}");
                    Console.WriteLine($"Email: {selectedUser.Email}");
                    Console.WriteLine($"Nazione: {selectedUser.Location.Country}");
                    Console.WriteLine($"Numero di telefono: {selectedUser.Phone}");
                    Console.WriteLine($"Età: {selectedUser.Age}");
                    Console.WriteLine($"Immagine: {selectedUser.Picture.Large}");

                    GetAdditionalStatistics(userResponse.Results);

                    Console.Write("Vuoi salvare i dati dell'utente su file? (S/N): ");
                    string responseSave = Console.ReadLine();

                    if (responseSave.ToUpper() == "S")
                    {
                        string fileName = $"{selectedUser.Name.First}_{selectedUser.Name.Last}.txt";
                        string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName);
                        SaveUserToFile(selectedUser, filePath);
                    }
                }
                else
                {
                    Console.WriteLine("Indice non valido.");
                }
            }
            else
            {
                Console.WriteLine("Errore durante la richiesta API: " + response.ReasonPhrase);
            }
        }
    }

    static void GetAdditionalStatistics(List<User> users)
    {
        // Statistiche sulle nazioni
        Dictionary<string, int> countryStatistics = new Dictionary<string, int>();

        // Statistiche aggiuntive
        int totalAge = 0;
        int maleCount = 0;
        int femaleCount = 0;

        foreach (User user in users)
        {
            totalAge += user.Age;

            if (user.Gender == "male")
            {
                maleCount++;
            }
            else if (user.Gender == "female")
            {
                femaleCount++;
            }

            if (countryStatistics.ContainsKey(user.Location.Country))
            {
                countryStatistics[user.Location.Country]++;
            }
            else
            {
                countryStatistics[user.Location.Country] = 1;
            }
        }

        double averageAge = (double)totalAge / users.Count;

        Console.WriteLine("Statistiche aggiuntive:");
        Console.WriteLine($"Età media degli utenti: {averageAge}");
        Console.WriteLine($"Numero di utenti maschi: {maleCount}");
        Console.WriteLine($"Numero di utenti femmine: {femaleCount}");
        Console.WriteLine("Statistiche sulle nazioni:");

        foreach (var entry in countryStatistics)
        {
            Console.WriteLine($"{entry.Key}: {entry.Value}");
        }
    }

    static void SaveUserToFile(User user, string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            writer.WriteLine($"Nome: {user.Name.First} {user.Name.Last}");
            writer.WriteLine($"Email: {user.Email}");
            writer.WriteLine($"Nazione: {user.Location.Country}");
            writer.WriteLine($"Numero di telefono: {user.Phone}");
            writer.WriteLine($"Età: {user.Age}");
            writer.WriteLine($"Immagine: {user.Picture.Large}");
        }

        Console.WriteLine($"Dati dell'utente salvati con successo in: {filePath}");
    }
}

public class UserResponse
{
    public List<User> Results { get; set; }
}

public class User
{
    public UserName Name { get; set; }
    public string Email { get; set; }
    public UserLocation Location { get; set; }
    public string Phone { get; set; }
    public UserPicture Picture { get; set; }
    public int Age { get; set; }
    public string Gender { get; set; }
}

public class UserName
{
    public string First { get; set; }
    public string Last { get; set; }
}

public class UserLocation
{
    public string Country { get; set; }
}

public class UserPicture
{
    public string Large { get; set; }
}
