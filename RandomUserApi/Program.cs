using System;
using System.Collections.Generic;
using System.IO;
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

                // Stampa i dati di ciascun utente
                int index = 0;
                foreach (User user in userResponse.Results)
                {
                    index++;
                    Console.WriteLine($"[{index}] Nome: {user.Name.First} {user.Name.Last}");
                    Console.WriteLine($"     Email: {user.Email}");
                    Console.WriteLine($"     Nazione: {user.Location.Country}");
                    Console.WriteLine($"     Numero di telefono: {user.Phone}");
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
                    Console.WriteLine($"Immagine: {selectedUser.Picture.Large}");

                    // Salvataggio dei dati dell'utente in un file
                    string fileName = $"user_{selectedUserIndex}.txt";
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), fileName);

                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        writer.WriteLine($"Nome completo: {selectedUser.Name.First} {selectedUser.Name.Last}");
                        writer.WriteLine($"Email: {selectedUser.Email}");
                        writer.WriteLine($"Nazione: {selectedUser.Location.Country}");
                        writer.WriteLine($"Numero di telefono: {selectedUser.Phone}");
                        writer.WriteLine($"Immagine: {selectedUser.Picture.Large}");
                        // Aggiungi altri dettagli a tua scelta
                    }

                    Console.WriteLine($"Dati salvati correttamente in {filePath}");
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
