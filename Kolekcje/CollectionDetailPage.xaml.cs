using System;
using System.IO;
using Microsoft.Maui.Controls;

namespace Kolekcje
{
    public partial class CollectionDetailPage : ContentPage
    {
        private Collection collection; // Aktualnie wy�wietlana kolekcja

        // Konstruktor klasy CollectionDetailPage
        public CollectionDetailPage(Collection collection)
        {
            InitializeComponent();
            this.collection = collection;
            Title = collection.Name; // Ustawianie tytu�u strony na nazw� kolekcji
            UpdateCollectionListView(); // Aktualizacja widoku listy element�w kolekcji
        }

        // Metoda dodaj�ca nowy element do kolekcji
        private async void AddItem_Clicked(object sender, EventArgs e)
        {
            // Wy�wietlenie okna dialogowego
            string newItem = await DisplayPromptAsync("Dodaj nowy element", "Wprowad� nowy element:");
            if (!string.IsNullOrWhiteSpace(newItem))
            {
                // Sprawdzenie czy nowy element ju� istnieje w kolekcji
                if (collection.Items.Contains(newItem))
                {
                    // Wy�wietlenie potwierdzenia od u�ytkownika w przypadku istnienia elementu
                    bool userConfirmed = await DisplayAlert("Element ju� istnieje", "Czy na pewno chcesz doda� ten element?", "Tak", "Nie");
                    if (!userConfirmed)
                        return;
                }
                collection.Items.Add(newItem); // Dodanie nowego elementu do kolekcji
                SaveCollectionItems(); // Zapisanie kolekcji do pliku
                UpdateCollectionListView(); // Aktualizacja widoku listy element�w kolekcji
            }
        }

        // Metoda usuwaj�ca zaznaczony element z kolekcji
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

        // Metoda edytuj�ca zaznaczony element kolekcji
        private async void EditSelected_Clicked(object sender, EventArgs e)
        {
            foreach (var child in CollectionItemsStackLayout.Children)
            {
                var radioButton = child as RadioButton;
                if (radioButton != null && radioButton.IsChecked)
                {
                    var itemToEdit = radioButton.Content as string;
                    string editedItem = await DisplayPromptAsync("Edytuj element", "Wprowad� now� warto��:", initialValue: itemToEdit);
                    if (!string.IsNullOrWhiteSpace(editedItem))
                    {
                        if (collection.Items.Contains(editedItem) && editedItem != itemToEdit)
                        {
                            bool userConfirmed = await DisplayAlert("Element ju� istnieje", "Czy na pewno chcesz edytowa� na istniej�cy element?", "Tak", "Nie");
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

        // Metoda aktualizuj�ca widok listy element�w kolekcji
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

        // Metoda zapisuj�ca elementy kolekcji do pliku
        private void SaveCollectionItems()
        {
            try
            {
                // Tworzenie �cie�ki do pliku kolekcji
                string collectionFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "kolekcje", $"{collection.Name}.txt");
                // Zapis element�w kolekcji do pliku
                File.WriteAllLines(collectionFilePath, collection.Items);
            }
            catch (Exception ex)
            {
                Console.WriteLine("B��d podczas zapisywania element�w kolekcji: " + ex.Message);
            }
        }
    }
}
