using System;
using System.IO;
using Microsoft.Maui.Controls;

namespace Kolekcje
{
    public partial class CollectionDetailPage : ContentPage
    {
        private Collection collection; // Aktualnie wyœwietlana kolekcja

        // Konstruktor klasy CollectionDetailPage
        public CollectionDetailPage(Collection collection)
        {
            InitializeComponent();
            this.collection = collection;
            Title = collection.Name; // Ustawianie tytu³u strony na nazwê kolekcji
            UpdateCollectionListView(); // Aktualizacja widoku listy elementów kolekcji
        }

        // Metoda dodaj¹ca nowy element do kolekcji
        private async void AddItem_Clicked(object sender, EventArgs e)
        {
            // Wyœwietlenie okna dialogowego
            string newItem = await DisplayPromptAsync("Dodaj nowy element", "WprowadŸ nowy element:");
            if (!string.IsNullOrWhiteSpace(newItem))
            {
                // Sprawdzenie czy nowy element ju¿ istnieje w kolekcji
                if (collection.Items.Contains(newItem))
                {
                    // Wyœwietlenie potwierdzenia od u¿ytkownika w przypadku istnienia elementu
                    bool userConfirmed = await DisplayAlert("Element ju¿ istnieje", "Czy na pewno chcesz dodaæ ten element?", "Tak", "Nie");
                    if (!userConfirmed)
                        return;
                }
                collection.Items.Add(newItem); // Dodanie nowego elementu do kolekcji
                SaveCollectionItems(); // Zapisanie kolekcji do pliku
                UpdateCollectionListView(); // Aktualizacja widoku listy elementów kolekcji
            }
        }

        // Metoda usuwaj¹ca zaznaczony element z kolekcji
        private void RemoveItem_Clicked(object sender, EventArgs e)
        {
            foreach (var child in CollectionItemsStackLayout.Children)
            {
                var radioButton = child as RadioButton;
                if (radioButton != null && radioButton.IsChecked)
                {
                    var itemToRemove = radioButton.Content as string;
                    collection.Items.Remove(itemToRemove);
                    SaveCollectionItems();
                    UpdateCollectionListView();
                    break;
                }
            }
        }

        // Metoda edytuj¹ca zaznaczony element kolekcji
        private async void EditSelected_Clicked(object sender, EventArgs e)
        {
            foreach (var child in CollectionItemsStackLayout.Children)
            {
                var radioButton = child as RadioButton;
                if (radioButton != null && radioButton.IsChecked)
                {
                    var itemToEdit = radioButton.Content as string;
                    string editedItem = await DisplayPromptAsync("Edytuj element", "WprowadŸ now¹ wartoœæ:", initialValue: itemToEdit);
                    if (!string.IsNullOrWhiteSpace(editedItem))
                    {
                        if (collection.Items.Contains(editedItem) && editedItem != itemToEdit)
                        {
                            bool userConfirmed = await DisplayAlert("Element ju¿ istnieje", "Czy na pewno chcesz edytowaæ na istniej¹cy element?", "Tak", "Nie");
                            if (!userConfirmed)
                                return;
                        }
                        collection.Items[collection.Items.IndexOf(itemToEdit)] = editedItem;
                        SaveCollectionItems();
                        UpdateCollectionListView();
                    }
                    break;
                }
            }
        }

        // Metoda aktualizuj¹ca widok listy elementów kolekcji
        private void UpdateCollectionListView()
        {
            CollectionItemsStackLayout.Children.Clear();
            foreach (var item in collection.Items)
            {
                var radioButton = new RadioButton
                {
                    Content = item
                };
                CollectionItemsStackLayout.Children.Add(radioButton);
            }
        }

        // Metoda zapisuj¹ca elementy kolekcji do pliku
        private void SaveCollectionItems()
        {
            try
            {
                // Tworzenie œcie¿ki do pliku kolekcji
                string collectionFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "kolekcje", $"{collection.Name}.txt");
                // Zapis elementów kolekcji do pliku
                File.WriteAllLines(collectionFilePath, collection.Items);
            }
            catch (Exception ex)
            {
                Console.WriteLine("B³¹d podczas zapisywania elementów kolekcji: " + ex.Message);
            }
        }
    }
}
