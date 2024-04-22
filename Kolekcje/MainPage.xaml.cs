using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Kolekcje
{
    public partial class MainPage : ContentPage
    {
        private List<Collection> collections; // Lista wszystkich kolekcji
        private string collectionsDirectoryPath;

        // Konstruktor klasy MainPage
        public MainPage()
        {
            InitializeComponent();

            // Inicjalizacja ścieżki do katalogu danych aplikacji
            collectionsDirectoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "kolekcje");
            if (!Directory.Exists(collectionsDirectoryPath))
            {
                Directory.CreateDirectory(collectionsDirectoryPath); // Utwórz katalog, jeśli nie istnieje
            }
            Debug.WriteLine(collectionsDirectoryPath);
            // Wczytanie informacji o kolekcjach przy starcie aplikacji
            collections = LoadCollections();
            UpdateCollectionListView(); // Aktualizacja widoku listy kolekcji
        }

        // Metoda wywoływana po kliknięciu na kolekcję
        private async void CollectionListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            // Pobranie wybranej kolekcji
            Collection selectedCollection = e.Item as Collection;

            // Nawigacja do nowej strony CollectionDetailPage i przekazanie wybranej kolekcji jako parametru
            await Navigation.PushAsync(new CollectionDetailPage(selectedCollection));
        }

        // Metoda wywoływana po kliknięciu przycisku "Dodaj kolekcję"
        private async void AddCollection_Clicked(object sender, EventArgs e)
        {
            // Wywołanie okna dialogowego do wprowadzenia danych nowej kolekcji
            string name = await DisplayPromptAsync("Dodaj nową kolekcję", "Wprowadź nazwę kolekcji:");
            if (string.IsNullOrWhiteSpace(name))
                return; // Nie dodawaj kolekcji, jeśli nazwa jest pusta lub złożona tylko z białych znaków

            // Sprawdzenie czy kolekcja o takiej nazwie już istnieje
            if (collections.Any(c => c.Name == name))
            {
                await DisplayAlert("Błąd", "Kolekcja o takiej nazwie już istnieje.", "OK");
                return;
            }

            // Utworzenie nowego obiektu Collection i dodanie go do listy kolekcji
            Collection newCollection = new Collection { Name = name };
            collections.Add(newCollection);

            // Aktualizacja widoku listy kolekcji
            UpdateCollectionListView();

            // Zapis kolekcji do pliku
            SaveCollections();

            // Utworzenie nowego pliku txt dla nowej kolekcji
            string collectionFilePath = Path.Combine(collectionsDirectoryPath, $"{name}.txt");
            if (!File.Exists(collectionFilePath))
            {
                File.Create(collectionFilePath).Close();
            }
        }

        // Metoda aktualizująca widok listy kolekcji
        private void UpdateCollectionListView()
        {
            CollectionListView.ItemsSource = null; // Wyczyść istniejące dane
            CollectionListView.ItemsSource = collections; // Ustaw nowe dane
        }

        // Metoda wczytująca informacje o kolekcjach z pliku
        private List<Collection> LoadCollections()
        {
            List<Collection> loadedCollections = new List<Collection>();

            try
            {
                // Pobranie wszystkich plików txt w katalogu kolekcji
                string[] collectionFiles = Directory.GetFiles(collectionsDirectoryPath, "*.txt");

                foreach (string file in collectionFiles)
                {
                    // Wczytanie nazwy kolekcji z nazwy pliku
                    string collectionName = Path.GetFileNameWithoutExtension(file);

                    // Otwarcie pliku i wczytanie zawartości jako kolekcji
                    List<string> items = File.ReadAllLines(file).ToList();

                    // Tworzenie obiektu kolekcji i dodanie go do listy wczytanych kolekcji
                    Collection collection = new Collection
                    {
                        Name = collectionName,
                        Items = items
                    };

                    loadedCollections.Add(collection);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd podczas wczytywania kolekcji: " + ex.Message);
            }

            return loadedCollections;
        }

        // Metoda zapisująca kolekcje do pliku
        private void SaveCollections()
        {
            try
            {
                foreach (Collection collection in collections)
                {
                    // Ścieżka do pliku kolekcji
                    string collectionFilePath = Path.Combine(collectionsDirectoryPath, $"{collection.Name}.txt");

                    // Zapis elementów kolekcji do pliku
                    File.WriteAllLines(collectionFilePath, collection.Items);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Błąd podczas zapisywania kolekcji: " + ex.Message);
            }
        }
    }
}
